from logging import FileHandler, Logger
from os.path import abspath, dirname, isfile, join
import numpy as np
import random
from typing import Tuple, Union, List
import pickle
import h5py
from tqdm import tqdm
import tensorflow as tf

from .models import FFN
from .proto import Moves

WINDOW_SIZE = 10  # frame count
FEATURE_SIZE = 2048
MEMORY_SIZE = 900000
BATCH_SIZE = 2048
EXPLORATION_MAX = 1.0
EXPLORATION_MIN = 0.1
EXPLORATION_TEST = 0.02
EXPLORATION_STEPS = 850000
EXPLORATION_DECAY = (EXPLORATION_MAX-EXPLORATION_MIN)/EXPLORATION_STEPS


GAMMA = 0.99
TRAINING_FREQUENCY = 2048
TARGET_NETWORK_UPDATE_FREQUENCY = 40000
MODEL_PERSISTENCE_UPDATE_FREQUENCY = 10000
REPLAY_START_SIZE = 50000


def chunks(lst, n):
    """Yield successive n-sized chunks from lst."""
    for i in range(0, len(lst), n):
        yield lst[i:i + n]


class Trainer:

    def __init__(self, print_summary=False):
        self.model_path = join(abspath(dirname(__file__)),'..',"..","data")
        self.log_path = join(self.model_path, "logs")
        self.N_moves = len(Moves.values())
        self._weight_path = join(self.model_path, 'weights.h5')
        self.datafile = join(self.log_path, 'data.p')

        self._setlogs()

        self._load_models(print_summary)
        self._reset()

        self.epsilon = EXPLORATION_MAX
        self.memory = []
    
    @classmethod
    def train(cls):
        obj = cls()
        
        with h5py.File(obj.log_path + '/preprocessed.h5','r') as hf:
            nextImages = hf['nextImage'][...]
            currentImages = hf['currentImage'][...]
            metadata = hf['metadata'][...]

        obj.memory= [{ 
            "currentImage": ci,
            "nextImage": ni,
            "action": int(md[0]),
            "reward": md[1],
            "died": int(md[2]),
            "didBoost": int(md[3])} for ni, ci, md in zip(nextImages, currentImages, metadata)]
        
        obj._train()
        obj._save_model()

    @classmethod
    def train_override(cls, memory: List[dict], print_summary=False, **kwargs) -> tf.keras.models.Model:
        """Trains from import (such as in a jupyter notebook) and returns the trained keras object.


        Args:
            memory (List[dict]): Memory dataset
            kwargs: keras model kwargs

        Returns:
            tf.keras.models.Model: fit model
        """
        obj = cls(print_summary)
        obj.memory = memory

        mdl = obj._train(ret_mdl=True, **kwargs)
        obj._save_model()

        return mdl

    def move(self, state: np.array) -> Tuple[int, bool]:
        """State comes in and based on it and previous 9 states,
        a prediction of a move and boost flag is made.

        Args:
            state (np.array): Shape of (2048) coming in from prefeaturized InceptionV3

        Returns:
            Tuple[int, bool]: Returns a integer corresponding to the proto Move enum and and bool flag if 
            whether to initiate boost or not
        """
        if np.random.rand() < self.epsilon or len(self.memory) < REPLAY_START_SIZE:
            return random.randrange(0, self.N_moves), bool(random.randrange(0, 2))

        # back_9 = [x['currentImage'] for x in self.memory[-9:]]

        # observation = np.array(back_9 + [state])

        # assert observation.shape == (10, 2048), "Move observation shape mismatch"

        [q_values, q_boost_values] = self.model.predict(
            np.expand_dims(state, axis=0), batch_size=1)

        return int(np.argmax(q_values[0])), bool(np.argmax(q_boost_values[0]))

    def reset(self, score: float, step: int, run: int):
        """Logs the current score, step count for said run and run count
        then resets the models.
        """
        self.logger.info(
            f"Reseting with score: {round(score,2)} step: {step} run: {run}"
        )
        self._reset()

    def step_update(self, total_step):
        """Based on the total steps in the entire game
        it looks to make update, train, reset decisions based on
        preconfigured options
        """
        if total_step < REPLAY_START_SIZE:
            return

        if total_step % TRAINING_FREQUENCY == 0:
            self._train()

        self._update_epsilon()

        if total_step % MODEL_PERSISTENCE_UPDATE_FREQUENCY == 0:
            self._save_model()

        if total_step % TARGET_NETWORK_UPDATE_FREQUENCY == 0:
            self._reset()
            print('{{"metric": "epsilon", "value": {}}}'.format(self.epsilon))
            print('{{"metric": "total_step", "value": {}}}'.format(total_step))

    def remember(self, id: str, currentImage: list, nextImage: list, action: Moves, reward: float, died: bool, didBoost: bool):
        """Remembers the current state coming in from the player instance. If this memory stack 
        is over 50 records, it pops first one and then adds to pickle file

        Args:
            id (str): UUID of image saved in logs folder
            currentImage (list): float[2048] coming in of featurized image
            nextImage (list): float[2048] coming in of a featurized image
            action (Moves): Move enum
            reward (float): current reward value
            died (bool): Died at this time
            didBoost (bool): Used boost
        """
        record = {
            'currentImage': np.array(currentImage),
            'nextImage': np.array(nextImage),
            'action': action,
            "didBoost": int(didBoost),
            'reward': reward,
            'died': died,
            'id': id.encode(),
        }

        self.memory.append(record)
        
        try:
            with open(self.datafile, 'ab') as f:
                pickle.dump(record, f)
        except Exception as e:
            print(e)

        if len(self.memory) > MEMORY_SIZE:
            self.memory.pop(0)
        

    def _train(self, ret_mdl = False, **kwargs) -> Union[tf.keras.models.Model, None]:
        """Implementation of the training loop. Takes in self.memory
        and reshapes it accordingly for training.

        Args:
            ret_mdl (bool, optional): For use when module is imported, returns the fit keras Model object. Defaults to False.

        Returns:
            Union[tf.keras.models.Model, None]: If `ret_mdl` flag is true, then returns the keras Model otherwise a void function.
        """
        batch = random.sample(self.memory, BATCH_SIZE)
        if len(batch) < BATCH_SIZE:
            return

        current_states = []
        q_values = []
        max_q_values = []
        boost_q_values = []
        max_boost_q_values = []

        for entry in tqdm(batch):
            current_state = np.expand_dims(entry["currentImage"], axis=0)
            current_states.append(current_state)

            next_state = np.expand_dims(np.array(entry["nextImage"]), axis=0)
            [next_state_prediction, is_boost] = self.model_target.predict(
                next_state)

            next_q_value = np.max(next_state_prediction.ravel())
            next_boost_q_value = np.max(is_boost.ravel())

            [q, bq] = self.model.predict(current_state)

            q = list(q[0])
            bq = list(bq[0])

            # print(entry)

            if entry["died"]:
                q[entry["action"]] = entry["reward"]
                bq[entry["didBoost"]] = entry["reward"]
            else:
                q[entry["action"]] = entry["reward"] + GAMMA * next_q_value
                bq[entry["didBoost"]] = entry["reward"] + \
                    GAMMA * next_boost_q_value

            q_values.append(q)
            boost_q_values.append(bq)

            max_q_values.append(np.max(q))
            max_boost_q_values.append(np.max(bq))

        X = np.array(current_states).squeeze()
        y = [np.array(q_values).squeeze(), np.array(
            max_boost_q_values).squeeze()]

        fit = self.model.fit(
            X, y,
            batch_size=BATCH_SIZE,
            verbose=0,
            **kwargs
        )

        if ret_mdl:
            return fit

        loss = fit.history["loss"][0]
        macc = fit.history["move_accuracy"][0]
        bacc = fit.history["boost_accuracy"][0]
        self.logger.info(f"Loss {round(loss,2)}")
        self.logger.info(f"Acc move: {round(macc,2)} bacc: {round(bacc,2)}")
        self.logger.info(
            f"Mean mQ: {round(np.mean(max_q_values),2)} Mean bQ: {round(np.mean(max_boost_q_values),2)}")

    def _setlogs(self):
        self.logger = Logger("TrainerLogs")
        try:
            self.logger.addHandler(FileHandler(
                join(self.log_path, "trainer.log")))            
        except FileNotFoundError:
            # probably here from unit tests
            pass
        self.logger.info("Logger initialized")

    def _update_epsilon(self):
        self.epsilon -= EXPLORATION_DECAY
        self.epsilon = max(EXPLORATION_MIN, self.epsilon)

    def _reset(self):
        self.model_target.set_weights(self.model.get_weights())

    def _save_model(self):
        self.model.save_weights(self._weight_path)

    def _load_models(self, print_summary=False):
        self.model = FFN(input_shape=(
            FEATURE_SIZE,), moves=self.N_moves, summary=print_summary)

        if isfile(self._weight_path):
            self.model.load_weights(self._weight_path)
            self.logger.info("Weights loaded")


        self.model_target = FFN(input_shape=(
            FEATURE_SIZE,), moves=self.N_moves)
