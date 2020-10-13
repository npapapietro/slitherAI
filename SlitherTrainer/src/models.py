from tensorflow.keras.applications.resnet50 import ResNet50
import tensorflow as tf
from tensorflow.keras.models import Model
from tensorflow.keras.layers import Input, Dense, LSTM
import keras2onnx
from tensorflow.keras import backend as K
from tensorflow.python.keras.layers.core import Flatten



def export():
    K.set_image_data_format('channels_first')
    model = ResNet50(include_top=False, weights='imagenet', input_shape=(3, 299, 299), pooling="max") # (2048,)
    
    onnx_model = keras2onnx.convert_keras(model, model.name)

    with open("ResNet50.onnx", 'wb') as f:
        f.write(onnx_model.SerializeToString())

def RNN(input_shape, moves, summary=False):

    input = Input(shape=input_shape)
    x = LSTM(1024,activation='relu', return_sequences=True)(input)
    x = LSTM(512,activation='relu')(x)
    x = Dense(256)(x)
    x = Dense(moves)(x)

    model = Model(input, x)
    model.compile(loss="mean_squared_error", metrics=["accuracy", "loss"])

    if summary:
        model.summary()
    
    return model

if __name__ == "__main__":
    RNN()