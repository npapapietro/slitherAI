# Playing Slither.io with Reinforcement Learning

## Components

### SlitherPlayer

The player, runs on selenium. It interfaces with a web browser to "see" the game. It communicates to the trainer via gRPC stream.

### learning/TrainerServer

Runs a gRPC server and holds the deep learning models that make the moves.
