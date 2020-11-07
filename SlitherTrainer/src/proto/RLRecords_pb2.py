# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: RLRecords.proto
"""Generated protocol buffer code."""
from google.protobuf.internal import enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='RLRecords.proto',
  package='',
  syntax='proto3',
  serialized_options=b'\252\002\rSlither.Proto',
  create_key=_descriptor._internal_create_key,
  serialized_pb=b'\n\x0fRLRecords.proto\"\x1c\n\x0bMoveRequest\x12\r\n\x05Image\x18\x01 \x03(\x02\"5\n\x0cMoveResponse\x12\x16\n\x06\x41\x63tion\x18\x01 \x01(\x0e\x32\x06.Moves\x12\r\n\x05\x42oost\x18\x02 \x01(\x08\"\x90\x01\n\x0fRememberRequest\x12\x14\n\x0c\x43urrentImage\x18\x01 \x03(\x02\x12\x11\n\tNextImage\x18\x02 \x03(\x02\x12\x16\n\x06\x41\x63tion\x18\x03 \x01(\x0e\x32\x06.Moves\x12\x0e\n\x06Reward\x18\x04 \x01(\x02\x12\x10\n\x08\x44idBoost\x18\x05 \x01(\x08\x12\x0c\n\x04\x44ied\x18\x06 \x01(\x08\x12\x0c\n\x04Guid\x18\x07 \x01(\t\" \n\x0bStepRequest\x12\x11\n\tTotalStep\x18\x01 \x01(\x05\"8\n\x0cResetRequest\x12\r\n\x05Score\x18\x01 \x01(\x02\x12\x0c\n\x04Step\x18\x02 \x01(\x05\x12\x0b\n\x03Run\x18\x03 \x01(\x05\"\t\n\x07Nothing*f\n\x05Moves\x12\x07\n\x03Pi0\x10\x00\x12\t\n\x05Pi1_4\x10\x01\x12\t\n\x05Pi1_2\x10\x02\x12\t\n\x05Pi3_4\x10\x03\x12\x06\n\x02Pi\x10\x04\x12\t\n\x05Pi5_4\x10\x05\x12\t\n\x05Pi3_2\x10\x06\x12\t\n\x05Pi7_4\x10\x07\x12\n\n\x06NoMove\x10\x08\x32\xa9\x01\n\x0eSlitherTrainer\x12\'\n\x08NextMove\x12\x0c.MoveRequest\x1a\r.MoveResponse\x12&\n\x08Remember\x12\x10.RememberRequest\x1a\x08.Nothing\x12$\n\nStepUpdate\x12\x0c.StepRequest\x1a\x08.Nothing\x12 \n\x05Reset\x12\r.ResetRequest\x1a\x08.NothingB\x10\xaa\x02\rSlither.Protob\x06proto3'
)

_MOVES = _descriptor.EnumDescriptor(
  name='Moves',
  full_name='Moves',
  filename=None,
  file=DESCRIPTOR,
  create_key=_descriptor._internal_create_key,
  values=[
    _descriptor.EnumValueDescriptor(
      name='Pi0', index=0, number=0,
      serialized_options=None,
      type=None,
      create_key=_descriptor._internal_create_key),
    _descriptor.EnumValueDescriptor(
      name='Pi1_4', index=1, number=1,
      serialized_options=None,
      type=None,
      create_key=_descriptor._internal_create_key),
    _descriptor.EnumValueDescriptor(
      name='Pi1_2', index=2, number=2,
      serialized_options=None,
      type=None,
      create_key=_descriptor._internal_create_key),
    _descriptor.EnumValueDescriptor(
      name='Pi3_4', index=3, number=3,
      serialized_options=None,
      type=None,
      create_key=_descriptor._internal_create_key),
    _descriptor.EnumValueDescriptor(
      name='Pi', index=4, number=4,
      serialized_options=None,
      type=None,
      create_key=_descriptor._internal_create_key),
    _descriptor.EnumValueDescriptor(
      name='Pi5_4', index=5, number=5,
      serialized_options=None,
      type=None,
      create_key=_descriptor._internal_create_key),
    _descriptor.EnumValueDescriptor(
      name='Pi3_2', index=6, number=6,
      serialized_options=None,
      type=None,
      create_key=_descriptor._internal_create_key),
    _descriptor.EnumValueDescriptor(
      name='Pi7_4', index=7, number=7,
      serialized_options=None,
      type=None,
      create_key=_descriptor._internal_create_key),
    _descriptor.EnumValueDescriptor(
      name='NoMove', index=8, number=8,
      serialized_options=None,
      type=None,
      create_key=_descriptor._internal_create_key),
  ],
  containing_type=None,
  serialized_options=None,
  serialized_start=354,
  serialized_end=456,
)
_sym_db.RegisterEnumDescriptor(_MOVES)

Moves = enum_type_wrapper.EnumTypeWrapper(_MOVES)
Pi0 = 0
Pi1_4 = 1
Pi1_2 = 2
Pi3_4 = 3
Pi = 4
Pi5_4 = 5
Pi3_2 = 6
Pi7_4 = 7
NoMove = 8



_MOVEREQUEST = _descriptor.Descriptor(
  name='MoveRequest',
  full_name='MoveRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='Image', full_name='MoveRequest.Image', index=0,
      number=1, type=2, cpp_type=6, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=19,
  serialized_end=47,
)


_MOVERESPONSE = _descriptor.Descriptor(
  name='MoveResponse',
  full_name='MoveResponse',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='Action', full_name='MoveResponse.Action', index=0,
      number=1, type=14, cpp_type=8, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='Boost', full_name='MoveResponse.Boost', index=1,
      number=2, type=8, cpp_type=7, label=1,
      has_default_value=False, default_value=False,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=49,
  serialized_end=102,
)


_REMEMBERREQUEST = _descriptor.Descriptor(
  name='RememberRequest',
  full_name='RememberRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='CurrentImage', full_name='RememberRequest.CurrentImage', index=0,
      number=1, type=2, cpp_type=6, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='NextImage', full_name='RememberRequest.NextImage', index=1,
      number=2, type=2, cpp_type=6, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='Action', full_name='RememberRequest.Action', index=2,
      number=3, type=14, cpp_type=8, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='Reward', full_name='RememberRequest.Reward', index=3,
      number=4, type=2, cpp_type=6, label=1,
      has_default_value=False, default_value=float(0),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='DidBoost', full_name='RememberRequest.DidBoost', index=4,
      number=5, type=8, cpp_type=7, label=1,
      has_default_value=False, default_value=False,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='Died', full_name='RememberRequest.Died', index=5,
      number=6, type=8, cpp_type=7, label=1,
      has_default_value=False, default_value=False,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='Guid', full_name='RememberRequest.Guid', index=6,
      number=7, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=105,
  serialized_end=249,
)


_STEPREQUEST = _descriptor.Descriptor(
  name='StepRequest',
  full_name='StepRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='TotalStep', full_name='StepRequest.TotalStep', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=251,
  serialized_end=283,
)


_RESETREQUEST = _descriptor.Descriptor(
  name='ResetRequest',
  full_name='ResetRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='Score', full_name='ResetRequest.Score', index=0,
      number=1, type=2, cpp_type=6, label=1,
      has_default_value=False, default_value=float(0),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='Step', full_name='ResetRequest.Step', index=1,
      number=2, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='Run', full_name='ResetRequest.Run', index=2,
      number=3, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=285,
  serialized_end=341,
)


_NOTHING = _descriptor.Descriptor(
  name='Nothing',
  full_name='Nothing',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=343,
  serialized_end=352,
)

_MOVERESPONSE.fields_by_name['Action'].enum_type = _MOVES
_REMEMBERREQUEST.fields_by_name['Action'].enum_type = _MOVES
DESCRIPTOR.message_types_by_name['MoveRequest'] = _MOVEREQUEST
DESCRIPTOR.message_types_by_name['MoveResponse'] = _MOVERESPONSE
DESCRIPTOR.message_types_by_name['RememberRequest'] = _REMEMBERREQUEST
DESCRIPTOR.message_types_by_name['StepRequest'] = _STEPREQUEST
DESCRIPTOR.message_types_by_name['ResetRequest'] = _RESETREQUEST
DESCRIPTOR.message_types_by_name['Nothing'] = _NOTHING
DESCRIPTOR.enum_types_by_name['Moves'] = _MOVES
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

MoveRequest = _reflection.GeneratedProtocolMessageType('MoveRequest', (_message.Message,), {
  'DESCRIPTOR' : _MOVEREQUEST,
  '__module__' : 'RLRecords_pb2'
  # @@protoc_insertion_point(class_scope:MoveRequest)
  })
_sym_db.RegisterMessage(MoveRequest)

MoveResponse = _reflection.GeneratedProtocolMessageType('MoveResponse', (_message.Message,), {
  'DESCRIPTOR' : _MOVERESPONSE,
  '__module__' : 'RLRecords_pb2'
  # @@protoc_insertion_point(class_scope:MoveResponse)
  })
_sym_db.RegisterMessage(MoveResponse)

RememberRequest = _reflection.GeneratedProtocolMessageType('RememberRequest', (_message.Message,), {
  'DESCRIPTOR' : _REMEMBERREQUEST,
  '__module__' : 'RLRecords_pb2'
  # @@protoc_insertion_point(class_scope:RememberRequest)
  })
_sym_db.RegisterMessage(RememberRequest)

StepRequest = _reflection.GeneratedProtocolMessageType('StepRequest', (_message.Message,), {
  'DESCRIPTOR' : _STEPREQUEST,
  '__module__' : 'RLRecords_pb2'
  # @@protoc_insertion_point(class_scope:StepRequest)
  })
_sym_db.RegisterMessage(StepRequest)

ResetRequest = _reflection.GeneratedProtocolMessageType('ResetRequest', (_message.Message,), {
  'DESCRIPTOR' : _RESETREQUEST,
  '__module__' : 'RLRecords_pb2'
  # @@protoc_insertion_point(class_scope:ResetRequest)
  })
_sym_db.RegisterMessage(ResetRequest)

Nothing = _reflection.GeneratedProtocolMessageType('Nothing', (_message.Message,), {
  'DESCRIPTOR' : _NOTHING,
  '__module__' : 'RLRecords_pb2'
  # @@protoc_insertion_point(class_scope:Nothing)
  })
_sym_db.RegisterMessage(Nothing)


DESCRIPTOR._options = None

_SLITHERTRAINER = _descriptor.ServiceDescriptor(
  name='SlitherTrainer',
  full_name='SlitherTrainer',
  file=DESCRIPTOR,
  index=0,
  serialized_options=None,
  create_key=_descriptor._internal_create_key,
  serialized_start=459,
  serialized_end=628,
  methods=[
  _descriptor.MethodDescriptor(
    name='NextMove',
    full_name='SlitherTrainer.NextMove',
    index=0,
    containing_service=None,
    input_type=_MOVEREQUEST,
    output_type=_MOVERESPONSE,
    serialized_options=None,
    create_key=_descriptor._internal_create_key,
  ),
  _descriptor.MethodDescriptor(
    name='Remember',
    full_name='SlitherTrainer.Remember',
    index=1,
    containing_service=None,
    input_type=_REMEMBERREQUEST,
    output_type=_NOTHING,
    serialized_options=None,
    create_key=_descriptor._internal_create_key,
  ),
  _descriptor.MethodDescriptor(
    name='StepUpdate',
    full_name='SlitherTrainer.StepUpdate',
    index=2,
    containing_service=None,
    input_type=_STEPREQUEST,
    output_type=_NOTHING,
    serialized_options=None,
    create_key=_descriptor._internal_create_key,
  ),
  _descriptor.MethodDescriptor(
    name='Reset',
    full_name='SlitherTrainer.Reset',
    index=3,
    containing_service=None,
    input_type=_RESETREQUEST,
    output_type=_NOTHING,
    serialized_options=None,
    create_key=_descriptor._internal_create_key,
  ),
])
_sym_db.RegisterServiceDescriptor(_SLITHERTRAINER)

DESCRIPTOR.services_by_name['SlitherTrainer'] = _SLITHERTRAINER

# @@protoc_insertion_point(module_scope)
