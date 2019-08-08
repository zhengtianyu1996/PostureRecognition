using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Posture
{
    public class DataHandle
    {
        FileHandle m_fileHandle = new FileHandle();

        /// <summary>
        /// Read all the labels from a .MD file, if the file does not exist, then create one
        /// </summary>
        /// <param name="url">the url of the .MD file</param>
        /// <param name="data_length">the number of all frames</param>
        /// <returns></returns>
        public int[,] GetLabels(string url, int data_length)
        {
            int[,] LabelsList = new int[data_length, 2];
            // If Label List does not exist, create one
            if (!File.Exists(url))
            {
                // Create a list with the same length
                for (int i = 0; i < data_length; i++)
                {
                    //string itemStr = string.Format("{0}:{1}", i.ToString("00000"), "-1");
                    LabelsList[i, 0] = i + 1;
                    LabelsList[i, 1] = -1;
                }
                m_fileHandle.WriteLabelsMD(url, LabelsList);
            }
            else
            {
                // If Label List exists, read it
                string[] labels_string = File.ReadAllLines(url);
                for (int i = 0; i < labels_string.Length; i++)
                {
                    string[] line = labels_string[i].Split(':');
                    int index = Convert.ToInt32(line[0]);
                    int label = Convert.ToInt32(line[1]);

                    LabelsList[i, 0] = index;
                    LabelsList[i, 1] = label;                //string itemStr = string.Format("{0}:{1}", index, "-1");
                }
            }

            return LabelsList;
        }


        /// <summary>
        /// Read all the Skeleton data from the data file
        /// </summary>
        /// <param name="url">url of the skeleton data file</param>
        /// <returns></returns>
        public string[] ReadAllLine(string url)
        {
            if (File.Exists(url))
            {
                return (File.ReadAllLines(url));
            }
            else
            {
                return (null);
            }
        }


        /// <summary>
        /// Convert SkeletonData to string with a special format, e.g.:
        /// 00015:     239.3,  -3454.49,
        /// </summary>
        /// <param name="index">index of the current frame</param>
        /// <param name="skeletonData">a list contains skeleton data</param>
        /// <returns></returns>
        public string SkeletonData2Str(int index, List<double> skeletonData)
        {
            string str = string.Format("{0}: ", index.ToString("00000"));
            for (int i = 0; i < skeletonData.Count; i++)
            {
                str += string.Format("{0, 9}", skeletonData[i].ToString("0.000"));
                if (i < skeletonData.Count - 1)
                {
                    str += ", ";
                }
            }

            return (str);
        }


        /// <summary>
        /// After label, this function help to update Listbox and global label list
        /// And save the label list to a .md file
        /// </summary>
        /// <param name="labels_list">global label list</param>
        /// <param name="url">url of the .md file to save the labels</param>
        /// <param name="listBox"></param>
        /// <param name="labelindex">[-1,0,1,2,3,4] -1 means ignored</param>
        /// <param name="fileHandle"></param>
        public void UpdateLabel(ref int[,] labels_list, string url, ref ListBox listBox, int labelindex, FileHandle fileHandle)
        {
            // add image string: ..\..\..\DataFloder\ImagesFolder\ImageName
            string[] imgPaths = fileHandle.ImgaeFolder.Split(Path.DirectorySeparatorChar);

            foreach (int selectedIndex in listBox.SelectedIndices)
            {
                // Modify the listBox
                string line = listBox.Items[selectedIndex].ToString();
                string[] line_arr = line.Split(' ');
                if (string.IsNullOrWhiteSpace(line_arr[3]))
                {
                    continue;
                }

                string line_new = line.Substring(0, line.Length - 1) + (labelindex == -1 ? " " : labelindex.ToString());
                listBox.Items[selectedIndex] = line_new;
                listBox.SetSelected(selectedIndex, true);

                // Modify the labels list
                labels_list[selectedIndex, 1] = labelindex;
            }

            // Write
            fileHandle.WriteLabelsMD(url, labels_list);
        }


        /// <summary>
        /// Select some frames in the ListBox
        /// </summary>
        /// <param name="listboxdata"></param>
        /// <param name="num_total">how many frames need to be shown</param>
        /// <param name="num_cover">how many frames need to be coverd between different samples</param>
        /// <param name="lengthfixed">bool, whether to fix the length of the data</param>
        /// <returns></returns>
        public bool SelectSamples(ref ListBox listboxdata, int num_total, int num_cover, bool lengthfixed = true)
        {
            // listBoxData must have the first selected index
            if (listboxdata.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Please select some data first. ");
                return false;
            }

            // All of the listBoxData.SelectedItems must be valid
            foreach (var selectedItem in listboxdata.SelectedItems)
            {
                string[] itemSplit = selectedItem.ToString().Split(' ');
                //int index = Convert.ToInt32(itemSplit[0]);
                string itemFlag = itemSplit[3];
                if (string.IsNullOrWhiteSpace(itemFlag))
                {
                    MessageBox.Show("The selected data is not valid. ");
                    return false;
                }
            }

            // Calculate the count about select items
            int cntCur = listboxdata.SelectedIndices.Count;
            // Calculate how many data do we need now?
            int cntNeed = 0;
            // Calculate the cur index
            int indexCur = listboxdata.SelectedIndices[cntCur - 1];
            if (cntCur >= num_total)
            {
                cntNeed = num_total;
                // Remove all items and add more items: it will refresh the pictureBox when add items.
                // In order to see the whole gesture processing.
                listboxdata.SelectedIndices.Clear();
                indexCur = indexCur - num_cover;
            }
            else
            {
                cntNeed = num_total - cntCur;
            }


            // Add items: start at indexCur, add cntNeed items
            for (int i = 0; i < cntNeed;)
            {
                // Next items
                indexCur++;

                // Make sure have enough items. 
                if (indexCur >= listboxdata.Items.Count)
                {
                    MessageBox.Show("There are not enough items. ");
                    return false;
                }

                // Make sure the items is valid
                string[] itemSplit = listboxdata.Items[indexCur].ToString().Split(' ');
                string itemFlag = itemSplit[3];
                if (!lengthfixed)
                {
                    i++;
                }

                if (string.IsNullOrWhiteSpace(itemFlag))
                {
                    continue;
                }
                else
                {
                    if (lengthfixed)
                    {
                        i++;
                    }
                    listboxdata.SelectedIndices.Add(indexCur);
                }
            }
            return true;
        }
    }
}
