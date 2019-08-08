"""

#!/usr/bin/env python
# -*- coding: utf-8 -*-
# @Time    : 2019/7/19 13:31
# @Author  : Tianyu Zheng
# @File    : sample_reduce.py
# @Version : 1
# @Feature : 
To balance the data. Remove those samples which have many label-0 or label-2
"""
import os
import shutil
import numpy as np
import random
from glob import glob

def main():
    path_sample_ori='./Samples/'                    # url of the original samples
    path_sample_reduced='./SamplesReduced/'         # url of the reduced samples
    rate=0.75                                       # if the targets (0 or 2) labels take up more than [rate], it will be recorded
    rate_dropout=0.95                               # drop out [rate] of all the recorded samples

    # create a new folder
    if os.path.exists(path_sample_reduced):
        shutil.rmtree(path_sample_reduced)
    shutil.copytree(path_sample_ori, path_sample_reduced)

    # generate a list of all the samples
    file_list_info = get_file_list(path_sample_reduced, shuffle=False)

    # count the total number of label==0 and label==2
    num_label0=0
    num_label2=0
    for file in file_list_info:
        data=np.loadtxt(file)
        label = data[:,1]
        cnt_label0 = np.count_nonzero(np.where(label == 0, 1, 0))
        cnt_label2 = np.count_nonzero(np.where(label == 2, 1, 0))

        if cnt_label0>np.ceil(rate*len(label)):
            num_label0+=1
        if cnt_label2>np.ceil(rate*len(label)):
            num_label2+=1

    # decide how many samples should be removed
    num_label0_remove = np.ceil(num_label0*rate_dropout)
    num_label2_remove = np.ceil(num_label2*rate_dropout)

    # remove some samples randomly, according to [rate_dropout]
    file_list_info = get_file_list(path_sample_reduced, shuffle=True)
    for file in file_list_info:
        data = np.loadtxt(file)
        label = data[:, 1]
        cnt_label0 = np.count_nonzero(np.where(label == 0, 1, 0))
        cnt_label2 = np.count_nonzero(np.where(label == 2, 1, 0))

        if cnt_label0 > np.ceil(rate * len(label)):
            if num_label0_remove>0:
                os.remove(file)
                num_label0_remove -= 1
        if cnt_label2 > np.ceil(rate * len(label)):
            if num_label2_remove>0:
                os.remove(file)
                num_label2_remove -= 1


def get_file_list(path_data, shuffle=True):
    '''
    :param path_data: url of the data
    :param shuffle: whether shuffle or not
    :return: a list
    '''
    FileList_data = []
    # list_label_files = []
    for _, dirnames, _ in os.walk(path_data):
        for dirname in dirnames:
            data_file = glob(path_data+dirname+'/*.txt')
            for file in data_file:
                FileList_data.append(file)

    if shuffle:
        randnum = random.randint(0, 100)
        random.seed(randnum)
        random.shuffle(FileList_data)
        random.seed(randnum)
        # random.shuffle(list_label_files)
    return FileList_data

if __name__ == '__main__':
    main()