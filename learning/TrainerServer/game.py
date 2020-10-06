from logging import FileHandler, Logger
from os.path import abspath, dirname, isfile, join
import numpy as np
import random
from enum import Enum

from .models import RNN

WINDOW_SIZE = 10  # frame count
FEATURE_SIZE = 2048

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
        self.steps = 0

    def move(self, state: np.array) -> Moves:
        if state.shape[0] < WINDOW_SIZE:
            return Moves(random.randrange(self.movelist))
        q_values = self.model.predict(
            np.expand_dims(state, axis=0), batch_size=1)
        self.steps += 1
        return Moves(np.argmax(q_values[0]))

    def reset(self, reward, state):
        self._reset()

    def reward(self,  reward, state):
        pass

    def step_update(self, total_step):
        if self.steps < REPLAY_START_SIZE:
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

        
    def _train(self):
        pass

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

