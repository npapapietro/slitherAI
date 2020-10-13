from logging import FileHandler, Logger
from os.path import abspath, dirname, isfile, join
import numpy as np
import random
from enum import Enum

from .models import RNN

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
TRAINING_FREQUENCY = 4
TARGET_NETWORK_UPDATE_FREQUENCY = 40000
MODEL_PERSISTENCE_UPDATE_FREQUENCY = 10000
REPLAY_START_SIZE = 50000


class Moves(Enum):
    Left = 0
    Right = 1
    Boost = 2
    BoostLeft = 3
    BoostRight = 4
    Wait = 5


class Trainer:

    def __init__(self):
        self.model_path = join(abspath(dirname(__file__)), 'logs')
        self._setlogs()

        self.movelist = [x.value for x in list(Moves)]
        self._load_models()
        self._reset()

        self.epsilon = EXPLORATION_MAX
        self.memory = []

    def move(self, state: np.array) -> int:
        if np.random.rand() < self.epsilon or len(self.memory) < WINDOW_SIZE:
            return random.randrange(min(self.movelist), max(self.movelist))

        q_values = self.model.predict(
            np.expand_dims(state, axis=0), batch_size=1)

        return np.argmax(q_values[0])

    def reset(self, score, step, run):
        self._reset()

    def step_update(self, total_step):
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

    def remember(self, currentImage, nextImage, action, reward, died):
        self.memory.append({
            'currentImage': np.array(currentImage),
            'nextImage': np.array(nextImage),
            'action': action,
            'reward': reward,
            'died': died
        })
        if len(self.memory) > MEMORY_SIZE:
            self.memory.pop(0)

    def _train(self):
        batch = random.sample(self.memory, BATCH_SIZE)
        if len(batch) < BATCH_SIZE:
            return

        current_states = []
        q_values = []
        max_q_values = []

        for entry in batch:
            current_state = np.expand_dims(entry["currentImage"], axis=0)
            current_states.append(current_state)

            next_state = np.expand_dims(np.array(entry["nextImage"]), axis=0)
            next_state_prediction = self.model_target.predict(
                next_state).ravel()

            next_q_value = np.max(next_state_prediction)
            q = list(self.model.predict(current_state)[0])

            if entry["died"]:
                q[entry["action"]] = entry["reward"]
            else:
                q[entry["action"]] = entry["reward"] + GAMMA * next_q_value
            q_values.append(q)
            max_q_values.append(np.max(q))

        fit = self.model.fit(
            np.array(current_states).squeeze(),
            np.array(q_values).squeeze(),
            batch_size=BATCH_SIZE,
            verbose=0)

        loss = fit.history["loss"][0]
        acc = fit.history["acc"][0]
        self.logger.info(f"Loss {round(loss,2)}")
        self.logger.info(f"Acc {round(acc,2)}")
        self.logger.info(f"Mean Q {round(np.mean(max_q_values),2)}")

    def _setlogs(self):
        self.logger = Logger("TrainerLogs")
        self.logger.addHandler(FileHandler(
            join(self.model_path, "trainer.log")))
        self.logger.info("Logger initialized")

    def _update_epsilon(self):
        self.epsilon -= EXPLORATION_DECAY
        self.epsilon = max(EXPLORATION_MIN, self.epsilon)

    def _reset(self):
        self.model_target.set_weights(self.model.get_weights())

    def _save_model(self):
        self.model.save_weights()

    def _load_models(self):
        self.model = RNN(input_shape=(
            WINDOW_SIZE, FEATURE_SIZE), moves=max(self.movelist))
        self.model_weights = join(self.model_path, 'weights.h5')

        if isfile(self.model_weights):
            self.model.load_weights(self.model_weights)

        self.model_target = RNN(input_shape=(
            WINDOW_SIZE, FEATURE_SIZE), moves=max(self.movelist))
