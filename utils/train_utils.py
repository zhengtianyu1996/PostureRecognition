"""

#!/usr/bin/env python
# -*- coding: utf-8 -*-
# @Time    : 2019/8/1 0:30
# @Author  : Tianyu Zheng
# @File    : train_utils.py
# @Version : 1
# @Feature : Some useful functions during training

"""
import os
import time
import shutil
import re
import itertools
import numpy as np
import tensorflow as tf
import matplotlib.pyplot as plt
from keras import backend as K
from sklearn.metrics import confusion_matrix, accuracy_score

def save_model_history(model_history, mode='acc', save_path='./', show_figure=False, show_train_value=False, show_test_value=False):
    """
    Save 'loss' or 'accuracy' curve during training
    :param model_history: History object after model.fit
    :param mode: 'acc' or 'loss'
    :param save_path: path to store the fig
    :param show_figure: bool value. Show the figure or not
    :param show_train_value: ool value. Show train value or not
    :param show_test_value: bool value. Show test value or not
    :return:
    """

    plt.clf()
    plt.plot(model_history.history[mode], label='train '+mode)
    plt.plot(model_history.history['val_'+mode], label='val '+mode)

    # show the value of training curve
    if show_train_value:
        i = 0
        for value in model_history.history[mode]:
            value = float('%.2f' % value)
            plt.text(i, value + 0.005, str(value), ha='center', va='bottom', fontsize=8.5)
            i += 1

    # show the value of test curve
    if show_test_value:
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

def predict_from_generator(model, data_generator, step, save_cm=True, save_path_cm='./',
                           list_label_names=None, title='train'):
    """
    Predict all the test data.
    :param model: keras model file after training
    :param data_generator: data generator object
    :param step: how many times to run the prediction.(related to batch size)
    :param save_cm: bool value. Whether to save the confusion matrix or not.
    :param save_path_cm: url to save the confusion matrix figure
    :param list_label_names: a list of different label names. e.g.[Standing, Walking, Falling, ...]
    :param title: string. The figure title
    :return: an accuracy value of the whole test dataset
    """

    print("Predicting %s set..." % title)
    predictions = []
    targets = []

    for i in range(step):
        # get data and label from data generator
        data, label, _ = next(data_generator)

        # make prediction and record the time
        start = time.perf_counter()
        p = model.predict(data, steps=1)
        end = time.perf_counter()
        print('Now predicting %s in %s, predict speed: %s sec/sample' % (i+1, step, str((end - start) / np.size(data, 0))))

        # Collect predictions and ground trues (Notice here the label without one-hot)
        p = np.argmax(p, axis=2).reshape(-1)
        y = label.reshape(-1)
        y = np.where(y>-1,y,p)
        predictions = np.concatenate((predictions, p))
        targets = np.concatenate((targets, y))

    # if the label is -1, we need to use sample_weight to ignore it. Then set all -1 label to 1
    sample_weight = np.where(targets > -1, 1, 0)

    # abs is used to set -1 to 1, to avoid 6*6 confusion matrix
    targets = np.abs(targets)
    # accuracy of the whole dataset
    acc = accuracy_score(targets, predictions, sample_weight=sample_weight)
    # confusion matrix
    cm = confusion_matrix(targets, predictions, sample_weight=sample_weight)
    print("All done!\nThe accuracy is: %s" % acc)
    print("The confusion matrix is:")
    print(cm)

    # save confusion matrix figure to disk
    if save_cm:
        save_confusion_matrix(cm, acc, label_list=list_label_names, title=title, save_path=save_path_cm)
    return acc


def predict_from_generator_save_error(model, data_generator, step, threshold=20, save_path_error='./error/',
                                      save_cm=True,save_path_cm='./', list_label_names=None, title='train', rm=True):
    """
    Predict all the test data. And save the wrong predicting samples
    :param model: keras model file after training
    :param data_generator: data generator object
    :param step: how many times to run the prediction.(related to batch size)
    :param threshold: if there are [threshold] frames difference between the prediction and GT, record it as error sample
    :param save_path_error: url to save the error samples
    :param save_cm: bool value. Whether to save the confusion matrix or not.
    :param save_path_cm: url to save the confusion matrix figure
    :param list_label_names: a list of different label names. e.g.[Standing, Walking, Falling, ...]
    :param title: string. The figure title
    :param rm: bool value. Whether remove the existing error images
    :return: an accuracy value of the whole test dataset
    """

    # if rm=True, remove the existing files in error url
    if rm and os.path.exists(save_path_error):
        shutil.rmtree(save_path_error)
    if not os.path.exists(save_path_error):
        os.mkdir(save_path_error)

    print("Predicting %s set..." % title)
    predictions = []
    targets = []

    for i in range(step):
        # get data and label from data generator
        data, label, filenames = next(data_generator)

        # make prediction and record the time
        start = time.perf_counter()
        for j in range(len(filenames)):
            p = model.predict(np.expand_dims(data[j,:,:], axis=0))
            p = np.argmax(p, axis=2).reshape(-1)
            y = label[j,:].reshape(-1)

            predictions = np.concatenate((predictions, p))
            targets = np.concatenate((targets, y))

            # if different frames are too much, save the original txt file
            y1 = np.where(y > -1, y, p)  # set y=p, if y=-1
            error_index=np.where(p!=y1,1,0)
            if np.count_nonzero(error_index)>threshold:
                filenames_arr = re.split('[/\\\]', filenames[j])
                filepath = os.path.join(save_path_error, filenames_arr[3])
                if not os.path.exists(filepath):
                    os.makedirs(filepath)
                error_path=os.path.join(filepath, filenames_arr[4])
                shutil.copyfile(filenames[j],error_path)

                # record the prediction labels
                info=np.concatenate((np.expand_dims(y,axis=1),(np.expand_dims(p,axis=1))),axis=1)
                np.savetxt(os.path.join(filepath, filenames_arr[4].split('.')[0]+'.md'), info, fmt='%d')


        end = time.perf_counter()
        print('Now calculating %s in %s, predict speed: %s sec/sample' % (i+1, step, str((end - start) / np.size(data, 0))))

    # if the label is -1, we need to use sample_weight to ignore it. Then set all -1 label to 1
    sample_weight=np.where(targets>-1,1,0)

    # generate a sample weight array. All the value equals to 0 except for the last valid data.
    # Use this weight array to calculate the accuracy on the last data in each sample
    targets_re=np.reshape(sample_weight,[-1,60])
    zeross=np.zeros([targets_re.shape[0],1])
    kk=np.append(targets_re,zeross,axis=1)
    sample_weight_final=np.where(np.diff(kk)==-1,1,0).reshape(-1)

    # abs is used to set -1 to 1, to avoid 6*6 confusion matrix
    targets = np.abs(targets)
    # accuracy of the whole dataset
    acc = accuracy_score(targets, predictions, sample_weight=sample_weight)
    # accuracy of only the last valid data
    acc_final = accuracy_score(targets, predictions, sample_weight=sample_weight_final)
    # confusion matrix
    cm = confusion_matrix(targets, predictions, sample_weight=sample_weight)
    # accuracy of only the "Falling" label
    acc_fall=cm[4,4]/np.sum(cm[4,:])

    print("All done!\nThe accuracy is: %s" % acc)
    print("The accuracy of the last predict is: %s" % acc_final)
    print("The accuracy of the fall is: %s" % acc_fall)
    print("The confusion matrix is:")
    print(cm)

    # save confusion matrix figure to disk
    if save_cm:
        save_confusion_matrix(cm, acc, label_list=list_label_names, title=title, save_path=save_path_cm)
    return acc


def save_confusion_matrix(cm, accuracy, label_list, title, save_path='./', show_figure=False):

    """
    Save confusion matrix to a figure.
    :param cm: confusion matrix array
    :param accuracy: the average accuracy of each dataset
    :param label_list: a list of different label names. e.g.[Standing, Walking, Falling, ...]
    :param title: figure title prefix
    :param save_path: url to save the figure
    :param show_figure: whether show the figure in cmd
    :return: None
    """

    plt.clf()
    plt.imshow(cm, interpolation='nearest', cmap=plt.get_cmap('Blues'))
    plt.title(title+' confusion matrix & acc: %.4f' % accuracy)
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

    # show the figure
    if show_figure:
        plt.show()

    path = os.path.join(save_path, title + '_cm.png')
    plt.savefig(path)
    print("Save confusion matrix of %s set successfully! " % title)


def save_model(model, save_dir, model_base_name='posture_recognition', save_h5=True, save_pb=True):
    """
    Save model to different files.
    :param model: keras model after training
    :param save_dir: url to save the model
    :param model_base_name: basename of the model file
    :param save_h5: whether to save the model as a keras .h5 file
    :param save_pb: whether to save the model as a tensorflow .pb file
    :return:
    """

    if not os.path.exists(save_dir):
        os.mkdir(save_dir)

    # save as .h5 file
    if save_h5:
        model.save(os.path.join(save_dir, model_base_name+'.h5'))
    # save as .pb file
    if save_pb:
        h5_to_pb(model, output_dir=save_dir, model_name=model_base_name + '.pb')


def h5_to_pb(h5_model,output_dir,model_name,out_prefix = "output_",log_tensorboard = True):
    """
    Convert keras .h5 file to tensorflow .pb file
    :param h5_model: keras model after training
    :param output_dir: url to save the .pb file
    :param model_name: basename of the file
    :param out_prefix: output prefix string
    :param log_tensorboard: Whether to save an event file for tensorborad
    :return:
    """
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