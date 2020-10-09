# Playing Slither.io with Reinforcement Learning

## Components

### SlitherPlayer

The player, runs on selenium. It interfaces with a web browser to "see" the game. It communicates to the trainer via gRPC stream.

### SlitherTrainer

Runs a gRPC server and holds the deep learning models that make the moves.

## Running

* Navigate to the SlitherTrainer directory and run `python -m src --server`
* Navigate to the SlitherPlayer directory and run `dotnet run`
* Watch it learn

More documentation to come.
