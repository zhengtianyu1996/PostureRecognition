"""

#!/usr/bin/env python
# -*- coding: utf-8 -*-
# @Time    : 2019/7/31 22:37
# @Author  : Tianyu Zheng
# @File    : test_mannual.py
# @Version : 1
# @Feature : 
1.use tkinter to open a sample file
2.read pbfile and make prediction
3.calc the accuracy
"""
import numpy as np
import tkinter.filedialog as tkdlg
import tensorflow as tf


def main():
    ## parameters
    # input shape
    shape=[60,75]
    # .pb file url
    path_pbfile="./train/posture_recognition.pb"

    # choose a file
    file = tkdlg.askopenfilename(initialdir = "./data/Samples/", title = "Select file",
                                                  filetypes = (("txt files","*.txt"),("all files","*.*")))

    # read skeleton data and label
    skeleton_data=np.ones(shape=[1,shape[0], shape[1]])*-1
    label_data = np.ones(shape=[shape[0]])*-1
    curIndex = 0

    f = open(file)
    line = f.readline()[0:-2]
    while line:
        line_arr = np.array(line.split('\t'), dtype=float)
        label_data[curIndex] = line_arr[1] - 1  # -1 because we replace 0 by 2
        data = line_arr[2:]
        data = np.reshape(data, [1, -1])
        skeleton_data[0,curIndex,:] = data
        curIndex += 1
        line = f.readline()[0:-2]
    f.close()

    # load pbfile
    with tf.gfile.FastGFile(path_pbfile, "rb") as f:
        graph_def = tf.GraphDef()
        graph_def.ParseFromString(f.read())
        input, output = tf.import_graph_def(graph_def, return_elements=["masking_1_input:0", "output_1:0"])

    # make prediction
    with tf.Session() as sess:
        init = tf.global_variables_initializer()
        sess.run(init)
        output_value = sess.run(output, feed_dict={input: skeleton_data})

    # calculate accuracy
    pred=np.argmax(output_value,axis=1)
    num_positive = np.count_nonzero(np.where(label_data>-1,1,0))
    cntEq=0.0
    for i in range(num_positive):
        if label_data[i]==pred[i]:
            cntEq+=1
    print("FileUrl: %s" % file)
    print("Accuracy: %s%%" % (cntEq*100/num_positive))


if __name__ == '__main__':
    main()