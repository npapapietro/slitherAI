import argparse
from argparse import ArgumentError
import os


parser = argparse.ArgumentParser()
parser.add_argument("--rebuild", action="store_true", help="Rebuilds the python proto generation files")
parser.add_argument("--server", action="store_true", help="Starts the grpc server")
parser.add_argument("--export", action="store_true", help="Exports the CNN image featurizer")
parser.add_argument("--train", action="store_true")
parser.add_argument("--reload-memory", action="store_true")

def update_protos():
    from grpc_tools import protoc
    import pkg_resources
    import sys
    import glob
    import re

    PROTOPATH = os.path.abspath(
        os.path.join(
            os.path.dirname(__file__),
            "..",
            "SlitherPlayer",
            "Protos"
        )
    )

    PYTHON_OUT = os.path.join(os.path.abspath(os.path.dirname(__file__)), "src","proto")

    argv = [
        sys.argv[0],
        f"--proto_path={PROTOPATH}",
        f"--python_out={PYTHON_OUT}",
        f"--grpc_python_out={PYTHON_OUT}",
        os.path.join(PROTOPATH, "RLRecords.proto")
    ]

    proto_include = pkg_resources.resource_filename('grpc_tools', '_proto')
    protoc.main(argv + ['-I{}'.format(proto_include)])

    for script in glob.iglob(f'{PYTHON_OUT}/*.py'):
        print("Updaing imports in", script)
        with open(script, 'r+') as file:
            code = file.read()
            file.seek(0)
            file.write(re.sub(r'\n(import .+_pb2.*)', 'from . \\1', code))
            file.truncate()

def run_server(**trainer_args):
    from .src import serve
    serve(**trainer_args)

def run_export():
    from .src import export
    export()

def train():
    from .src import Trainer    
    Trainer.train()

if __name__ == "__main__":
    DATADIR = os.path.abspath(os.path.join(os.path.dirname(__file__),'..','data'))
    LOGDIR = os.path.join(DATADIR,'logs')
    if not os.path.isdir(DATADIR):
        os.mkdir(DATADIR)
        os.mkdir(LOGDIR)
    elif not os.path.isdir(LOGDIR):
        os.mkdir(LOGDIR)


    args = parser.parse_args()

    if sum([int(args.rebuild), int(args.server), int(args.export), int(args.train)]) != 1:
        raise ArgumentError("SlitherTrainer can only be ran with one flag")

    if args.rebuild:
        update_protos()

    elif args.server:
        run_server(reload_memory=args.reload_memory)

    elif args.export:
        run_export()
    
    elif args.train:
        train()
