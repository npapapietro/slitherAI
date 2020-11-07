# Playing Slither.io with Reinforcement Learning

![SlitherTrainer unit tests](https://github.com/npapapietro/slitherAI/workflows/SlitherTrainer%20unit%20tests/badge.svg) ![SlitherPlayer unit tests](https://github.com/npapapietro/slitherAI/workflows/SlitherPlayer%20unit%20tests/badge.svg)

This is a fun project to showcase some fun RL learning on a simple to play webgame.

## Components

### SlitherPlayer

The player, runs on selenium. It interfaces with a web browser to "see" the game. It communicates to the trainer via gRPC stream.

### SlitherTrainer

Runs a gRPC server and holds the deep learning models that make the moves.

## Setup

* Run pip install on requirements.txt inside SlitherTrainer
* Inside ./SlitherTrainer run `python -m src --export` to export image featurizer to correct placement (should be in repo root)

## Running


* Run `python -m SlitherTrainer --server`
* Navigate to the SlitherPlayer directory and run `dotnet run` 
* Watch it play
Note: Because of how dotnet runs, C\# is setup to run from `dotnet run` not from an executable at the moment

## Training

* While the game plays, after a certain step size it will start a training loop. Currently adding error handling to ensure that it reaches that step.
* Backup log files, data files and image are being taken in `data`

More documentation to come.
