import unittest
import numpy as np
import os.path as p

from src.game import Trainer, BATCH_SIZE, WINDOW_SIZE


class TrainerTests(unittest.TestCase):
    
    def test_ctor(self):
        try:
            Trainer()
        except Exception as e:
            self.assertTrue(False)
    
    def test_move_rando(self):
        trainer = Trainer()
        trainer.memory = []

        fake_state = np.random.rand(1,2048)

        move, boost = trainer.move(fake_state)

        self.assertTrue(move < trainer.N_moves)
        self.assertTrue(isinstance(boost, bool))

    def test_move_memory(self):
        trainer = Trainer()
  

        fake_state = np.random.rand(2048)

        move, boost = trainer.move(fake_state)

        self.assertTrue(move < trainer.N_moves)
        self.assertTrue(isinstance(boost, bool))

    def test_move_reset(self):
        trainer = Trainer()
        trainer._weight_path = p.join(trainer.model_path, 'temp.h5')

        try:
            trainer.reset(0,0,0)
        except:
            self.assertTrue(False)
        finally:
            import os
            if p.isfile(trainer._weight_path):
                os.remove(trainer._weight_path)

    def test_remember(self):
        import src.game as g
        g.MEMORY_SIZE = 50
        trainer = g.Trainer()
        
        for i in range(65):
            trainer.remember(np.random.rand(1,2048), np.random.rand(1,2048), i, i, i, False)

        self.assertTrue(len(trainer.memory) == g.MEMORY_SIZE)

    def test_train(self):
        import src.game as g
        g.BATCH_SIZE = 16
        trainer = g.Trainer()
        for _ in range(g.BATCH_SIZE):
            trainer.memory.append({
                'currentImage': np.random.rand(2048),
                'nextImage': np.random.rand(2048),
                'action': 1,
                'reward': 1,
                "didBoost": 0,
                'died': False
            })
        try:
            trainer._train()
        except Exception as e:
            print(e)
            self.assertTrue(False)

    