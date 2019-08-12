"""

#!/usr/bin/env python
# -*- coding: utf-8 -*-
# @Time    : 2019/6/21 11:32
# @Author  : Tianyu Zheng
# @File    : network_lstm.py
# @Version : 1
# @Feature : 
1.define some networks

"""
import numpy as np
import tensorflow as tf
from tensorflow.contrib import rnn

from keras import backend as K
from keras import optimizers
from keras.models import Sequential
from keras.layers import Dense, LSTM, Dropout, GlobalMaxPooling2D, Conv2D, Masking, Flatten, Reshape, TimeDistributed
from keras.layers import Bidirectional, BatchNormalization
from keras.initializers import TruncatedNormal
from sklearn.metrics import accuracy_score
from keras_contrib.layers import CRF
from keras_contrib.losses import crf_loss
from keras_contrib.metrics import crf_viterbi_accuracy, crf_marginal_accuracy


def build_model_bilstm_sequence(num_output, input_shape=None, learning_rate = 1e-3, learning_rate_decay=1e-5):
    """
    Network definition with a 2-D input shape
    :param num_output: classify class number
    :param input_shape: input shape, a 2-D tuple
    :param learning_rate: learning rate
    :param learning_rate_decay: learning rate decay
    :return: keras model
    """
    if input_shape is None:
        input_shape=[60,75]

    K.clear_session()
    initalizer = TruncatedNormal(stddev=0.1)

    # Building the model
    model = Sequential()
    model.add(Masking(mask_value=-1, input_shape=input_shape))

    model.add(TimeDistributed(Dense(256, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer)))
    model.add(TimeDistributed(BatchNormalization()))
    model.add(TimeDistributed(Dropout(0.4)))
    # model.add(Masking(mask_value=-1, input_shape=[60,75]))
    # model.add(Bidirectional(LSTM(num_layer_lstm, return_sequences=True, stateful=False),
    #                         merge_mode='ave'))
    model.add(Bidirectional(LSTM(200, return_sequences=True, stateful=False)))
    model.add(TimeDistributed(Dropout(0.4)))
    # model.add(Bidirectional(LSTM(100, return_sequences=True, stateful=False)))
    # model.add(Dropout(0.3))
    # model.add(Dense(256, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer))
    # model.add(BatchNormalization())
    # model.add(Dropout(0.3))
    # model.add(Bidirectional(LSTM(100, return_sequences=True, stateful=False)))
    # model.add(Dropout(0.3))
    # model.add(Bidirectional(LSTM(100, return_sequences=True, stateful=False)))
    # model.add(Dropout(0.3))
    # model.add(Bidirectional(LSTM(50, return_sequences=True)))
    # model.add(Dropout(0.3))

    # model.add(GlobalMaxPool1D())
    # model.add(Dropout(0.3))
    # model.add(Dense(1024, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer))
    # model.add(BatchNormalization())
    # model.add(Dropout(0.2))
    model.add(TimeDistributed(Dense(256, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer)))
    model.add(TimeDistributed(BatchNormalization()))
    model.add(TimeDistributed(Dropout(0.4)))
    # model.add(Dense(128, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer))
    # model.add(TimeDistributed(Dense(num_output, activation="softmax")))

    crf = CRF(num_output, learn_mode='join', test_mode='viterbi',
              kernel_initializer=initalizer, bias_initializer=initalizer, sparse_target=False)
    model.add(crf)

    # Compiling the model
    # optimizer = optimizers.RMSprop(lr=learning_rate, rho=0.9, epsilon=1e-6, decay=learning_rate_decay)
    # optimizer = optimizers.adadelta()
    optimizer = optimizers.adam(lr=learning_rate, decay=learning_rate_decay)
    model.compile(loss=crf_loss, optimizer=optimizer, metrics=[crf_viterbi_accuracy])
    # model.compile(loss='categorical_crossentropy', optimizer=optimizer, metrics=['accuracy'])
    return model


def build_model_bilstm_sequence_3(num_output, input_shape=None, learning_rate = 1e-3, learning_rate_decay=1e-5):
    """
    Network definition with a 3-D input shape
    :param num_output: classify class number
    :param input_shape: input shape, a 3-D tuple
    :param learning_rate: learning rate
    :param learning_rate_decay: learning rate decay
    :return: keras model
    """
    if input_shape is None:
        input_shape=[60,3,25]
    K.clear_session()
    initalizer = TruncatedNormal(stddev=0.1)

    # Building the model
    model = Sequential()
    # model.add(Masking(mask_value=-1, input_shape=[60, 3, 25]))
    model.add(Reshape((input_shape[0],input_shape[1],input_shape[2],1), input_shape=input_shape))
    model.add(TimeDistributed(Conv2D(64, kernel_size=3, padding='same', activation='relu')))
    model.add(TimeDistributed(BatchNormalization()))
    model.add(TimeDistributed(Dropout(0.4)))
    model.add(TimeDistributed(Conv2D(256, kernel_size=3, padding='same', activation='relu', strides=2)))
    model.add(TimeDistributed(BatchNormalization()))
    model.add(TimeDistributed(Dropout(0.4)))
    model.add(TimeDistributed(Conv2D(256, kernel_size=3, padding='same', activation='relu')))
    model.add(TimeDistributed(BatchNormalization()))
    model.add(TimeDistributed(Dropout(0.4)))
    model.add(TimeDistributed(Conv2D(32, kernel_size=3, padding='same', activation='relu', strides=[1,2])))
    model.add(TimeDistributed(BatchNormalization()))
    model.add(TimeDistributed(Dropout(0.4)))
    # model.add(Conv2D(32, kernel_size=1, activation='relu'))
    # model.add(Flatten())
    model.add(Reshape((60,-1)))
    # model.add(TimeDistributed(GlobalMaxPooling2D()))

    model.add(TimeDistributed(Dense(256, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer)))
    model.add(TimeDistributed(BatchNormalization()))
    model.add(TimeDistributed(Dropout(0.4)))
    # model.add(Masking(mask_value=-1, input_shape=[60,75]))
    # model.add(Bidirectional(LSTM(num_layer_lstm, return_sequences=True, stateful=False),
    #                         merge_mode='ave'))
    model.add(Bidirectional(LSTM(200, return_sequences=True, stateful=False)))
    model.add(TimeDistributed(Dropout(0.4)))
    # model.add(Bidirectional(LSTM(100, return_sequences=True, stateful=False)))
    # model.add(Dropout(0.3))
    # model.add(Dense(256, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer))
    # model.add(BatchNormalization())
    # model.add(Dropout(0.3))
    # model.add(Bidirectional(LSTM(100, return_sequences=True, stateful=False)))
    # model.add(Dropout(0.3))
    # model.add(Bidirectional(LSTM(100, return_sequences=True, stateful=False)))
    # model.add(Dropout(0.3))
    # model.add(Bidirectional(LSTM(50, return_sequences=True)))
    # model.add(Dropout(0.3))

    # model.add(GlobalMaxPool1D())
    # model.add(Dropout(0.3))
    # model.add(Dense(1024, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer))
    # model.add(BatchNormalization())
    # model.add(Dropout(0.2))
    model.add(TimeDistributed(Dense(256, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer)))
    model.add(TimeDistributed(BatchNormalization()))
    model.add(TimeDistributed(Dropout(0.4)))
    # model.add(Dense(128, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer))
    model.add(TimeDistributed(Dense(num_output, activation="softmax")))

    # crf = CRF(num_output, learn_mode='join', test_mode='viterbi',
    #           kernel_initializer=initalizer, bias_initializer=initalizer, sparse_target=False)
    # model.add(crf)

    # Compiling the model
    # optimizer = optimizers.RMSprop(lr=learning_rate, rho=0.9, epsilon=1e-6, decay=learning_rate_decay)
    # optimizer = optimizers.adadelta()
    optimizer = optimizers.adam(lr=learning_rate,decay=learning_rate_decay)
    # model.compile(loss=crf_loss, optimizer=optimizer, metrics=[crf_viterbi_accuracy])
    model.compile(loss='categorical_crossentropy', optimizer=optimizer, metrics=['accuracy'])
    return model

def build_model_bilstm(num_output, input_shape=None, learning_rate = 1e-3, num_layer_lstm=500):
    if input_shape is None:
        input_shape=[60,75]

    K.clear_session()
    initalizer = TruncatedNormal(stddev=0.1)

    # Building the model
    model = Sequential()

    # model.add(Conv2D)
    model.add(Dense(32, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer, input_shape=input_shape))
    model.add(BatchNormalization())
    model.add(Dropout(0.4))

    model.add(Bidirectional(LSTM(num_layer_lstm, return_sequences=False),
                            merge_mode='ave'))
    # model.add(Bidirectional(LSTM(50, return_sequences=False)))
    # model.add(GlobalMaxPool1D())
    model.add(Dropout(0.3))
    model.add(Dense(1024, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer))
    model.add(BatchNormalization())
    model.add(Dropout(0.1))
    model.add(Dense(512, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer))
    model.add(BatchNormalization())
    model.add(Dropout(0.1))
    # model.add(Dense(128, activation='relu', kernel_initializer=initalizer, bias_initializer=initalizer))
    model.add(Dense(num_output, activation="softmax"))

    # Compiling the model
    optimizer = optimizers.RMSprop(lr=learning_rate, rho=0.9, epsilon=1e-6, decay=0.0)
    model.compile(loss='categorical_crossentropy', optimizer=optimizer, metrics=['accuracy'])
    return model