import argparse



parser = argparse.ArgumentParser()
parser.add_argument("--rebuild", action="store_true", help="Rebuilds the python proto generation files")
parser.add_argument("--server", action="store_true", help="Starts the grpc server")


def update_protos():
    from grpc_tools import protoc
    import pkg_resources
    import os
    import sys
    import glob
    import re

    PROTOPATH = os.path.abspath(
        os.path.join(
            os.path.dirname(__file__),
            "..",
            "..",
            "SlitherPlayer",
            "Protos"
        )
    )

    PYTHON_OUT = os.path.join(os.path.abspath(os.path.dirname(__file__)), "proto")

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

def run_server():
    from . import serve
    serve()

if __name__ == "__main__":
    args = parser.parse_args()

    if args.rebuild:
        update_protos()

    if args.server:
        run_server()