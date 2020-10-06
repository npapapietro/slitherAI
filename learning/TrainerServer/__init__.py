import grpc
from concurrent import futures

from .handler import Service
from .proto import add_SlitherTrainerServicer_to_server
from .game import Trainer


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    add_SlitherTrainerServicer_to_server(
        Service(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()

