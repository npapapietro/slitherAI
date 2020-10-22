from tensorflow.keras.applications.resnet50 import ResNet50
import tensorflow as tf
from tensorflow.keras.models import Model
from tensorflow.keras.layers import Input, Dense, Dropout
import keras2onnx
from tensorflow.keras import backend as K



def export():
    K.set_image_data_format('channels_first')
    model = ResNet50(include_top=False, weights='imagenet', input_shape=(3, 299, 299), pooling="max") # (2048,)
    
    onnx_model = keras2onnx.convert_keras(model, model.name)

    with open("ResNet50.onnx", 'wb') as f:
        f.write(onnx_model.SerializeToString())

def FFN(input_shape, moves, summary=False):

    input = Input(shape=input_shape)
    x = Dense(512, activation='relu')(input)
    x = Dropout(0.15)(x)
    x = Dense(1024*2, activation='relu')(x)
    x = Dropout(0.15)(x)
    x = Dense(256*4, activation='relu')(x)
    out_move = Dense(moves, name="move")(x) # Movement direction
    is_boost = Dense(2, name="boost")(x) # Is boosting

    model = Model(input, outputs=[out_move, is_boost])
    model.compile(loss="mean_squared_error", metrics=["accuracy"])

    if summary:
        model.summary()
    
    return model

if __name__ == "__main__":
    FFN((2048,), 8, True)