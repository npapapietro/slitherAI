import numpy as np
from typing import List

from .game import Trainer
from .proto import (
    Response, 
    Request, 
    RewardRequest, 
    Moves, 
    Nothing,
    SlitherTrainerServicer
)


class Service(SlitherTrainerServicer):
    def __init__(self):
        self.trainer = Trainer()
        super().__init__()

    def NextMove(self, request: Request, *args, **kwargs) -> Response:
        state = {
            "Died": request.Died,
            "TimeStamp": request.TimeStamp,
            "Lenght": request.Length,
            "Image": np.array(request.Image)
        }

        action = self.trainer.move(state)

        return Response(Action=action)

    def Reset(self, request: RewardRequest, *args, **kwargs) -> Nothing:
        self.trainer.reset(request.reward, request.state)
        return Nothing()
    
    def Reward(self, request: RewardRequest, *args, **kwargs) -> Nothing:
        self.trainer.reset(request.reward, request.state)
        return Nothing()