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
import tensorflow as tf
import os
import keras
import itertools
import time
import random
import re
import numpy as np
import matplotlib.pyplot as plt
from glob import glob
from keras import backend as K
from keras.models import load_model
from network_lstm import build_model_bilstm, build_model_bilstm_sequence
from sklearn.model_selection import StratifiedShuffleSplit
from sklearn.metrics import confusion_matrix


def main():
    # params
    path_data='./data/'
    path_data_samples=path_data+'Samples/'
    path_train='./train/'
    rate_testset=0.2
    batch_size_train=32
    batch_size_val=8
    epochs=25
    learning_rate=1e-4


    if not os.path.exists(path_train):
        os.mkdir(path_train)

    # get all the label names
    f = open(os.path.join(path_data,'label.txt'))
    list_label_names = f.read().split()
    f.close()
    num_output_labels=len(list_label_names)

    # Method 1: self-made data generator
    # get file list and corresponding labels
    list_data_files, list_label_files = get_file_list_lstm(path_data_samples, shuffle=True)
    # list_txt_files, list_labels = get_file_list(path_data, list_label_names, shuffle=True)
    # split the data to train set and val set
    number_train = len(list_data_files) - int(len(list_data_files)*rate_testset)
    trainX, valX = list_data_files[0:number_train-1], list_data_files[number_train:]
    trainY, valY = list_label_files[0:number_train-1], list_label_files[number_train:]

    # get the data generator
    gen_data_train = data_generator_lstm((trainX, trainY), shape=[60, 75], batch_size=batch_size_train,
                                    one_hot=True, num_classes=num_output_labels)
    # gen_data_train = data_generator((trainX, trainY), shape=[60, 75], batch_size = batch_size_train,
    #                                 one_hot=True, num_classes=num_output_labels)
    gen_data_val = data_generator((valX, valY), shape=[60, 75], batch_size = batch_size_val,
                                  one_hot=True, num_classes=num_output_labels)


    # # Method 2: pre Read all the data into an array
    # # get data
    # list_data, list_labels_onehot = get_all_data(path_data, list_label_names)
    #
    # # shuffle the data
    # # create shuffle-split object
    # ss = StratifiedShuffleSplit(n_splits=1, test_size=rate_testset, random_state=0)
    # # Create shuffled indices
    # train_idx, val_idx = next(ss.split(list_data, list_labels_onehot))
    # # Assign back to data variables again
    # trainX, trainY = list_data[train_idx], list_labels_onehot[train_idx]
    # valX, valY = list_data[val_idx], list_labels_onehot[val_idx]


    # generate dataset
    model=build_model_bilstm_sequence(num_output=num_output_labels, learning_rate = learning_rate)
    model.summary()
    model = load_model(os.path.join(path_train, 'gesture_recognition_model.h5'))

    # Train
    model_history = model.fit_generator(gen_data_train, validation_data=gen_data_val, epochs=epochs,
                                        steps_per_epoch=len(trainX) // batch_size_train + 1,
                                        validation_steps=len(valX) // batch_size_val + 1,
                                        verbose=1)
    # model_history = model.fit(trainX, trainY, validation_data=(valX,valY),
    #                           shuffle=True,
    #                           epochs=epochs, batch_size=batch_size_train, verbose=1)

    save_model(model, save_dir=path_train, model_base_name='posture_recognition')
    # save visualize
    save_model_result(model_history, 'loss', save_path=path_train)
    save_model_result(model_history, 'acc', save_path=path_train, show_val_value=True)


    # save confusion matrix
    get_confusion_matrix_from_generator(model, gen_data_train, step= len(trainX) // batch_size_train + 1,
                                                   list_label_names=list_label_names, title='train',
                                                   save=True, save_path = path_train)
    get_confusion_matrix_from_generator(model, gen_data_val, step=len(valX) // batch_size_val + 1,
                                                   list_label_names=list_label_names, title='val',
                                                   save=True, save_path = path_train)

    # train_cm = get_confusion_matrix(model, (trainX, trainY), batch_size=batch_size_train)
    # valid_cm = get_confusion_matrix(model, (valX, valY), batch_size=batch_size_val)
    # save_confusion_matrix(train_cm, list_label_names,title='train', save_path=path_train)
    # save_confusion_matrix(valid_cm, list_label_names,title='valid', save_path=path_train)


def get_file_list_lstm(path_data, shuffle=True):

    FileList_data = []
    # list_label_files = []
    for _, dirnames, _ in os.walk(path_data):
        for dirname in dirnames:
            data_file = glob(path_data+dirname+'/*.txt')
            label_file = glob(path_data + dirname + '/*.md')
            for file in data_file:
                FileList_data.append(file)
            # for file in label_file:
            #     list_label_files.append(file)

    if shuffle:
        randnum = random.randint(0, 100)
        random.seed(randnum)
        random.shuffle(FileList_data)
        random.seed(randnum)
        # random.shuffle(list_label_files)
    return FileList_data


def get_file_list(path_data, list_label_names, shuffle=True):
    """
    :param path_data: path of the data
    :param list_label_names: list of all output labels
    :param shuffle: bool value. shuffle the list or not
    :return: list_txt_files, list_labels
    """
    list_txt_files = []
    list_labels = []
    label_index = 0
    for label_name in list_label_names:
        files = os.listdir(os.path.join(path_data, label_name))
        for file in files:
            list_txt_files.append(os.path.join(path_data, label_name, file))
        list_labels.extend(np.ones(len(files))*label_index)
        label_index+=1
    if shuffle:
        randnum = random.randint(0, 100)
        random.seed(randnum)
        random.shuffle(list_txt_files)
        random.seed(randnum)
        random.shuffle(list_labels)
    return list_txt_files, list_labels


def data_generator_lstm(file_list_data, shape=None, batch_size=32, one_hot=True, num_classes=7):
    if shape is None:
        shape = [60, 75]
    # list_data_files = data[0]
    # list_label_files = data[1]

    step = len(file_list_data) // batch_size + 1
    i = 0
    while True:
        # data_sample=np.ones(shape=[batch_size, shape[0]*shape[1]])*-1
        batch_data = []
        batch_label = []
        # datas=np.empty(shape=[1, shape[0]*shape[1]])
        list_txt_files_temp = file_list_data[i*batch_size: i*batch_size+batch_size]
        batch_size_real=0
        for file in list_txt_files_temp:
            f = open(file)
            line = f.readline()[0:-2]
            data_sample = np.ones(shape=[shape[0], shape[1]]) * -1
            label_sample = np.ones(shape=[shape[0]]) * -1
            curIndex=0

            while line:
                line_arr=np.array(line.split('\t'), dtype=float)
                # label_sample = np.reshape(label_sample.append(line_arr[1]),[1,-1])
                label_sample[curIndex] = line_arr[1]
                data = line_arr[2:]
                data = np.reshape(data, [1,-1])
                data_sample[curIndex] = data
                curIndex+=1
                line = f.readline()[0:-2]

            # data_sample = np.expand_dims(data_sample,axis=0)
            # label_sample = np.expand_dims(label_sample, axis=0)
            batch_data.append(data_sample)
            batch_label.append(label_sample)

            batch_size_real += 1
            f.close()

        # datas = np.reshape(datas[1:], [batch_size_real,shape[0],shape[1]])
        # labels = list_label_files[i*batch_size:i*batch_size+batch_size]
        if one_hot:
            label_batch = keras.utils.to_categorical(batch_label, num_classes)
        i+=1
        if i>=step:
            i=0

        yield batch_data, batch_label

def data_generator(data, shape=None, batch_size=32, one_hot=True, num_classes=6):
    """
    :param data: tuple (list_files, list_labels)
    :param shape: shape of each data sample eg.[60, 75]
    :param batch_size: batch size
    :param one_hot: bool value. Encode labels to one_hot or not
    :param num_classes: output class number
    :return: A tuple generator. (data, label)
    """
    # global datas
    if shape is None:
        shape = [60, 75]
    list_txt_files = data[0]
    list_labels = data[1]

    step = len(list_labels) // batch_size + 1
    i = 0
    while True:
        datas=np.empty(shape=[1, shape[0]*shape[1]])
        list_txt_files_temp = list_txt_files[i*batch_size: i*batch_size+batch_size]
        for file in list_txt_files_temp:
            f = open(file)
            data = np.array(f.read().split(), dtype=float)
            data = np.reshape(data, [1,-1])
            datas = np.append(datas, data, axis=0)
            f.close()

        datas = np.reshape(datas[1:], [-1,shape[0],shape[1]])
        labels = list_labels[i*batch_size:i*batch_size+batch_size]
        if one_hot:
            labels = keras.utils.to_categorical(labels, num_classes)
        i+=1
        if i>=step:
            i=0

        yield datas, labels


def get_all_data(path_data, list_label_names):
    """
    :param path_data: path of the data, with some categories
    :param list_label_names: a list of the output label names
    :return: A tuple, (list_data, list_labels_onehot)
    """

    # number of the output labels
    num_output_labels = len(list_label_names)

    # get the total number of text files
    num_samples = 0
    for label_name in list_label_names:
        num_samples += len(os.listdir(os.path.join(path_data, label_name)))

    list_labels = np.empty([num_samples])
    list_data = np.empty([num_samples, 60, 75])

    label_index = 0
    i = 0
    for label_name in list_label_names:
        path = os.listdir(os.path.join(path_data, label_name))
        for txt_file in path:
            f = open(os.path.join(path_data, label_name, txt_file))
            data = f.read().split()
            data = tuple(map(float, data))
            data = np.reshape(data, [60, 75])
            list_data[i] = data
            list_labels[i] = label_index
            i += 1
            f.close()
        label_index += 1

    list_labels_onehot = keras.utils.to_categorical(list_labels, num_output_labels)
    return list_data, list_labels_onehot


def save_model_result(model_history, mode='acc', save_path='./', show_figure=False, show_train_value=False, show_val_value=False):
    """
    :param model_history: History object after model.fit
    :param mode: 'acc' or 'loss'
    :param save_path: path to store the fig
    :param show_figure: bool value. Show the figure or not
    :param show_train_value: ool value. Show train value or not
    :param show_val_value: bool value. Show val value or not
    :return:
    """

    plt.clf()
    plt.plot(model_history.history[mode], label='train '+mode)
    plt.plot(model_history.history['val_'+mode], label='val '+mode)

    if show_train_value:
        i = 0
        for value in model_history.history[mode]:
            value = float('%.2f' % value)
            plt.text(i, value + 0.005, str(value), ha='center', va='bottom', fontsize=8.5)
            i += 1

    if show_val_value:
        i = 0
        for value in model_history.history['val_'+mode]:
            value = float('%.2f' % value)
            plt.text(i, value + 0.005, str(value), ha='center', va='bottom', fontsize=8.5)
            i += 1
    plt.legend()

    if show_figure:
        plt.show()
    path = os.path.join(save_path, mode+'.png')
    plt.savefig(path)


def get_confusion_matrix_from_generator(model, data_generator, step, list_label_names, title='train', save=True, save_path='./'):
    """
    :param model: network model
    :param data_generator: a generator object. Generate a tuple (data, label)
    :param step: how many steps to run
    :return: confusion matrix
    """

    print("Generating confusion matrix of %s..." % title)
    predictions = []
    targets = []

    # step = num_samples // batch_size + 1
    for i in range(step):
        data, label = next(data_generator)

        x = np.reshape(data, [-1, np.size(data, 1), np.size(data, 2)])
        y = np.reshape(label, [-1, np.size(label, 1)])

        start = time.perf_counter()
        p = model.predict(x, steps=1)
        end = time.perf_counter()
        print('Now calculating %s in %s, predict speed: %s sec/sample' % (i, step, str((end - start) / np.size(data, 0))))

        p = np.argmax(p, axis=1)
        y = np.argmax(y, axis=1)
        predictions = np.concatenate((predictions, p))
        targets = np.concatenate((targets, y))
        # if len(targets) >= num_samples:
        #     break
    cm = confusion_matrix(targets, predictions)
    print("Confusion matrix of %s generated." % title)

    if save:
        save_confusion_matrix(cm, label_list=list_label_names, title=title, save_path=save_path)
    return cm


def get_confusion_matrix(model, data, batch_size):
    """
    :param model: network model
    :param data: tuple with 2 elements, (train_data, val_data)
    :param batch_size: batch size for confusion matrix
    :return: confusion matrix
    """

    num_samples = np.size(data[0], 0)
    print("Generating confusion matrix...", num_samples)
    predictions = []
    targets = []

    step = num_samples // batch_size + 1
    for i in range(step):
        x = np.reshape(data[0][i*batch_size:i*batch_size+batch_size],[-1,np.size(data[0],1),np.size(data[0],2)])
        y = np.reshape(data[1][i*batch_size:i*batch_size+batch_size],[-1,np.size(data[1],1)])

        start = time.perf_counter()
        p = model.predict(x, steps=1)
        end = time.perf_counter()
        print('Now calculating %s in %s, speed: %s' % (i, step, str((end - start)/batch_size)))

        p = np.argmax(p, axis=1)
        y = np.argmax(y, axis=1)
        predictions = np.concatenate((predictions, p))
        targets = np.concatenate((targets, y))
        if len(targets) >= num_samples:
            break
    cm = confusion_matrix(targets, predictions)
    return cm


def save_confusion_matrix(cm, label_list, title, save_path='./', show_figure=False):
    """
    :param cm: confusion matrix
    :param label_list: list of all the labels
    :param title: name of the cm
    :param save_path: path to store the fig
    :param show_figure: bool value, whether to show the figure
    :return:
    """

    plt.clf()
    plt.imshow(cm, interpolation='nearest', cmap=plt.get_cmap('Blues'))
    plt.title(title+' confusion matrix')
    plt.colorbar()
    tick_marks = np.arange(len(label_list))
    plt.xticks(tick_marks, label_list, rotation=45)
    plt.yticks(tick_marks, label_list)

    fmt = 'd'
    thresh = cm.max() / 2.
    for i, j in itertools.product(range(cm.shape[0]), range(cm.shape[1])):
        plt.text(j, i, format(cm[i, j], fmt),
                 horizontalalignment="center",
                 color="white" if cm[i, j] > thresh else "black")

    plt.tight_layout()
    plt.ylabel('True label')
    plt.xlabel('Predicted label')
    if show_figure:
        plt.show()
    path = os.path.join(save_path, title + '_cm.png')
    plt.savefig(path)
    print("Save confusion matrix of %s successfully! " % title)


def save_model(model, save_dir, model_base_name='posture_recognition', save_h5=True, save_pb=True):
    if not os.path.exists(save_dir):
        os.mkdir(save_dir)
    if save_h5:
        model.save(os.path.join(save_dir, model_base_name+'.h5'))
    if save_pb:
        h5_to_pb(model, output_dir=save_dir, model_name=model_base_name + '.pb')


def h5_to_pb(h5_model,output_dir,model_name,out_prefix = "output_",log_tensorboard = True):
    if not os.path.exists(output_dir):
        os.mkdir(output_dir)
    out_nodes = []
    for i in range(len(h5_model.outputs)):
        out_nodes.append(out_prefix + str(i + 1))
        tf.identity(h5_model.output[i],out_prefix + str(i + 1))
    sess = K.get_session()
    from tensorflow.python.framework import graph_util,graph_io
    init_graph = sess.graph.as_graph_def()
    main_graph = graph_util.convert_variables_to_constants(sess,init_graph,out_nodes)
    graph_io.write_graph(main_graph,output_dir,name = model_name,as_text = False)
    if log_tensorboard:
        from tensorflow.python.tools import import_pb_to_tensorboard
        import_pb_to_tensorboard.import_to_tensorboard(os.path.join(output_dir,model_name),output_dir)


if __name__ == '__main__':
    main()