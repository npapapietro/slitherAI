# Playing Slither.io with Reinforcement Learning

![SlitherTrainer unit tests](https://github.com/npapapietro/slitherAI/workflows/SlitherTrainer%20unit%20tests/badge.svg)

![SlitherPlayer unit tests](https://github.com/npapapietro/slitherAI/workflows/SlitherPlayer%20unit%20tests/badge.svg)

## Components

### SlitherPlayer

The player, runs on selenium. It interfaces with a web browser to "see" the game. It communicates to the trainer via gRPC stream.

### SlitherTrainer

Runs a gRPC server and holds the deep learning models that make the moves.

## Setup

* Run pip install on requirements.txt inside SlitherTrainer
* Inside ./SlitherTrainer run `python -m src --export` to export image featurizer to correct placement (should be in repo root)

## Running

* Navigate to the SlitherTrainer directory and run `python -m src --server`
* Navigate to the SlitherPlayer directory and run `dotnet run`
* Watch it play

## Training

* While the game plays, after a certain step size it will start a training loop. Currently adding error handling to ensure that it reaches that step.
* Backup log files, data files and image are being taken in `data`

More documentation to come.
