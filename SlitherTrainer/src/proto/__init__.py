from .RLRecords_pb2_grpc import SlitherTrainerServicer, add_SlitherTrainerServicer_to_server
from .RLRecords_pb2 import (
    MoveRequest, 
    MoveResponse, 
    RememberRequest,
    StepRequest,
    ResetRequest,
    Moves, 
    Nothing,
)