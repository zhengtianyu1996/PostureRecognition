"""

#!/usr/bin/env python
# -*- coding: utf-8 -*-
# @Time    : 2019/8/1 0:08
# @Author  : Tianyu Zheng
# @File    : data_generator.py
# @Version : 
# @Feature : 
Some useful functions related to the data
"""
import os
import keras
import random
import numpy as np
from glob import glob

def data_generator_lstm(file_list_info, shape=None, batch_size=32, one_hot=True, num_classes=7,
                        is_training=True, return_sample_weights=False):
    """
    Define a data generator object with a 2-d input
    :param file_list_info: a file name list recording the sample url
    :param shape: a 2-D input shape. [timesteps, features]
    :param batch_size: batch size
    :param one_hot: whether use One-Hot or not
    :param num_classes: how many classes to be classified
    :param is_training: if False, yield an extra data: batch_filename
    :param return_sample_weights: if True, return sample weights
    :return:
    Yield a tuple of data
    """

    # input: 2-D array
    if shape is None:
        shape = [60, 75]

    step = len(file_list_info) // batch_size + 1
    i = 0

    while True:
        # output data
        batch_data = []
        batch_label = []
        batch_filename = []

        # read skeleton data from a batch of files
        list_txt_files_temp = file_list_info[i*batch_size: i*batch_size+batch_size]
        # batch_size_real=0
        for file in list_txt_files_temp:
            f = open(file)
            line = f.readline()[0:-2]

            # set all the value to -1 in the beginning
            data_sample = np.ones(shape=[shape[0], shape[1]]) * -1
            label_sample = np.ones(shape=[shape[0]]) * -1

            curIndex=0
            while line:
                line_arr=np.array(line.split('\t'), dtype=float)
                label_sample[curIndex] = line_arr[1]-1  # -1 because we replace 0 by 2
                data = line_arr[2:]
                data = np.reshape(data, [1,-1])
                data_sample[curIndex] = data
                curIndex+=1
                line = f.readline()[0:-2]

            # add the current sample into the batch
            batch_data.append(data_sample)
            batch_label.append(label_sample)
            batch_filename.append(file)
            # batch_size_real += 1
            f.close()

        batch_sample_weights = np.where(np.array(batch_label)>-1,1,0)

        # one-hot
        if one_hot:
            batch_label = keras.utils.to_categorical(batch_label, num_classes)

        # reset the index
        i+=1
        if i>=step:
            i=0

        # convert list to array
        batch_data = np.array(batch_data)
        batch_label = np.array(batch_label)

        # yield
        if is_training:
            if  return_sample_weights:
                yield batch_data, batch_label, batch_sample_weights
            else:
                yield batch_data, batch_label
        else:
            if  return_sample_weights:
                yield batch_data, batch_label, batch_filename, batch_sample_weights
            else:
                yield batch_data, batch_label, batch_filename


def data_generator_lstm_3(file_list_info, shape=None, batch_size=32, one_hot=True, num_classes=7,
                          is_training=True, return_sample_weights=False):
    """
    Define a data generator object with a 3-d input
    :param file_list_info: a file name list recording the sample url
    :param shape: a 3-D input shape. [timesteps, axises, features]
    :param batch_size: batch size
    :param one_hot: whether use One-Hot or not
    :param num_classes: how many classes to be classified
    :param is_training: if False, yield an extra data: batch_filename
    :param return_sample_weights: if True, return sample weights
    :return:
    Yield a tuple of data
    """

    # input: 3-D array
    if shape is None:
        shape = [60, 3, 25]

    step = len(file_list_info) // batch_size + 1
    i = 0

    while True:
        # output data
        batch_data = []
        batch_label = []
        batch_filename = []

        # read skeleton data from a batch of files
        list_txt_files_temp = file_list_info[i*batch_size: i*batch_size+batch_size]
        # batch_size_real=0
        for file in list_txt_files_temp:
            f = open(file)
            line = f.readline()[0:-2]

            # set all the value to -1 in the beginning
            data_sample = np.ones(shape=[shape[0], shape[1], shape[2]]) * -1
            label_sample = np.ones(shape=[shape[0]]) * -1

            curIndex=0
            label_temp=[]       # record the last valid label
            data_temp=[]        # record the last valid data
            while line:
                line_arr=np.array(line.split('\t'), dtype=float)
                label_temp = line_arr[1]-1
                label_sample[curIndex] = line_arr[1]-1  # -1 because we replace 0 by 2
                data_temp = line_arr[2:]
                data_temp = np.reshape(data_temp, [1, shape[1], shape[2]])
                data_sample[curIndex] = data_temp
                curIndex+=1
                line = f.readline()[0:-2]

            # data and label in one sample
            data_sample = np.where(data_sample<0, data_temp, data_sample)
            label_sample = np.where(label_sample < 0, label_temp, label_sample)

            # add the current sample into the batch
            batch_data.append(data_sample)
            batch_label.append(label_sample)
            # batch_label.append(label_temp)
            batch_filename.append(file)
            # batch_size_real += 1
            f.close()

        batch_sample_weights = np.where(np.array(batch_label)>-1,1,0)

        # one-hot
        if one_hot:
            batch_label = keras.utils.to_categorical(batch_label, num_classes)

        # reset the index
        i+=1
        if i>=step:
            i=0

        # convert list to array
        batch_data = np.array(batch_data)
        batch_label = np.array(batch_label)

        # yield
        if is_training:
            if return_sample_weights:
                yield batch_data, batch_label
            else:
                yield batch_data, batch_label, batch_sample_weights
        else:
            if return_sample_weights:
                yield batch_data, batch_label, batch_filename
            else:
                yield batch_data, batch_label, batch_filename, batch_sample_weights


def get_file_list(path_data, shuffle=True):
    """
    Get file list from all the sample folders
    :param path_data: a folder that contains all the samples, even some sub folders
    :param shuffle: shuffle the file list randomly or not
    :return:
    a list of file urls
    """

    # get the whole list
    FileList_data = []
    for _, dirnames, _ in os.walk(path_data):
        for dirname in dirnames:
            data_file = glob(path_data+dirname+'/*.txt')
            for file in data_file:
                FileList_data.append(file)

    # shuffle the data
    if shuffle:
        randnum = random.randint(0, 100)
        random.seed(randnum)
        random.shuffle(FileList_data)

    return FileList_data

def get_labels_list(path_label_list):
    """
    Get a list contains all the label names
    :param path_label_list: url of the label list .txt
    :return:
    1. a list of label names
    2. the number of these labels
    """

    f = open(path_label_list)
    list_label_names = f.read().split()
    f.close()
    num_output_labels = len(list_label_names)

    return  list_label_names, num_output_labels