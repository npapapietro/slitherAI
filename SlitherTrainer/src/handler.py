import numpy as np
from typing import List
from grpc import ServicerContext


from .game import Trainer
from .proto import (
    MoveRequest, 
    MoveResponse, 
    RememberRequest,
    StepRequest,
    ResetRequest,
    Moves, 
    Nothing,
    SlitherTrainerServicer
)


class Service(SlitherTrainerServicer):
    def __init__(self):
        self.trainer = Trainer()
        super().__init__()
        print("Ready")

    def NextMove(self, request: MoveRequest, context: ServicerContext) -> MoveResponse:
        img = np.array(request.Image)
        move, boost = self.trainer.move(img)
        print("NextMove",move, "with boost?", boost)
        return MoveResponse(Action=Moves.Name(move), Boost=boost)

    def Remember(self, request: RememberRequest, context: ServicerContext) -> Nothing:
        self.trainer.remember(
            request.CurrentImage, 
            request.NextImage,
            request.Action,
            request.Reward,
            request.Died,
            request.DidBoost
        )
        print("Remember")
        return Nothing()

    def StepUpdate(self, request: StepRequest, context: ServicerContext) -> Nothing:
        print("Step", request.TotalStep)
        self.trainer.step_update(request.TotalStep)
        return Nothing()

    def Reset(self, request: ResetRequest, context: ServicerContext) -> Nothing:
        print("Reset")
        self.trainer.reset(request.Score, request.Step, request.Run)
        return Nothing()