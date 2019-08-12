"""

#!/usr/bin/env python
# -*- coding: utf-8 -*-
# @Time    : 2019/6/20 15:19
# @Author  : Tianyu Zheng
# @File    : train.py
# @Version : 1
# @Feature :
1. Basic program for BiLSTM network

"""
import os
from keras.models import load_model
from network_lstm import build_model_bilstm, build_model_bilstm_sequence, build_model_bilstm_sequence_3
from utils import data_generator
from utils import train_utils
from keras_contrib.layers import CRF
from keras_contrib.losses import crf_loss
from keras_contrib.metrics import crf_viterbi_accuracy, crf_marginal_accuracy


def main():
    # params
    path_data='./data/'
    path_data_samples=path_data+'SamplesReduced/'
    path_train='./train/'
    path_error='./error/'

    rate_testset=0.2                # rate for the testset. All_files * rate
    batch_size_train=16
    batch_size_val=8
    epochs=2
    learning_rate=1e-4
    learning_rate_decay=1e-6
    threshold_error=20              # during testing, if [threshold_error] frames are wrong predictd, then record as error
    data_shape_in_batch=[60,75]     # input shape

    if not os.path.exists(path_train):
        os.mkdir(path_train)

    # get all the label names
    list_label_names, num_output_labels = data_generator.get_labels_list(os.path.join(path_data,'label.txt'))

    # get file list and corresponding labels
    file_list_info = data_generator.get_file_list(path_data_samples, shuffle=True)
    # split the data to train set and val set
    number_train = len(file_list_info) - int(len(file_list_info)*rate_testset)
    TrainInfo, ValInfo = file_list_info[0:number_train-1], file_list_info[number_train:]

    # get the data generator
    gen_data_train = data_generator.data_generator_lstm(TrainInfo,
                                                        shape=data_shape_in_batch,
                                                        batch_size=batch_size_train,
                                                        one_hot=True,
                                                        num_classes=num_output_labels,
                                                        return_sample_weights=False)
    gen_data_val = data_generator.data_generator_lstm(ValInfo,
                                                      shape=data_shape_in_batch,
                                                      batch_size=batch_size_val,
                                                      one_hot=True,
                                                      num_classes=num_output_labels,
                                                      return_sample_weights=False)


    # generate keras model
    model=build_model_bilstm_sequence(num_output=num_output_labels, input_shape=data_shape_in_batch, learning_rate = learning_rate, learning_rate_decay=learning_rate_decay)
    model.summary()

    # load model
    # custom_objects = {'CRF': CRF,
    #                   'crf_loss': crf_loss,
    #                   'crf_viterbi_accuracy': crf_viterbi_accuracy}
    # model = load_model(os.path.join(path_train, 'posture_recognition.h5'), custom_objects=custom_objects)

    # Train
    model_history = model.fit_generator(gen_data_train, validation_data=gen_data_val, epochs=epochs,
                                        steps_per_epoch=len(TrainInfo) // batch_size_train + 1,
                                        validation_steps=len(ValInfo) // batch_size_val + 1,
                                        verbose=1)

    # save model file
    train_utils.save_model(model, save_dir=path_train, model_base_name='posture_recognition')
    # save curve
    train_utils.save_model_history(model_history, 'loss', save_path=path_train)
    train_utils.save_model_history(model_history, 'crf_viterbi_accuracy', save_path=path_train, show_test_value=True)


    # save confusion matrix
    gen_data_train = data_generator.data_generator_lstm(TrainInfo,
                                                        shape=data_shape_in_batch,
                                                        batch_size=batch_size_train,
                                                        one_hot=False,
                                                        num_classes=num_output_labels,
                                                        is_training=False,
                                                        return_sample_weights=False)
    gen_data_val = data_generator.data_generator_lstm(ValInfo,
                                                      shape=data_shape_in_batch,
                                                      batch_size=batch_size_val,
                                                      one_hot=False,
                                                      num_classes=num_output_labels,
                                                      is_training=False,
                                                      return_sample_weights=False)

    train_utils.predict_from_generator_save_error(model, gen_data_train, step= len(TrainInfo) // batch_size_train + 1,
                                      threshold=threshold_error, save_path_error=path_error, save_cm=True,
                                      save_path_cm=path_train, list_label_names=list_label_names, title='train')
    train_utils.predict_from_generator_save_error(model, gen_data_val, step=len(ValInfo) // batch_size_val + 1,
                                      threshold=threshold_error, save_path_error=path_error, save_cm=True,
                                      save_path_cm=path_train, list_label_names=list_label_names, title='val')

    # train_utils.predict_from_generator(model, gen_data_train, step=len(TrainInfo) // batch_size_train + 1,
    #                        save_cm=True, save_path_cm=path_train, list_label_names=list_label_names, title='train')
    # train_utils.predict_from_generator(model, gen_data_val, step=len(ValInfo) // batch_size_val + 1,
    #                        save_cm=True, save_path_cm=path_train, list_label_names=list_label_names, title='val')


if __name__ == '__main__':
    main()