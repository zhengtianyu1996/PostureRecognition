using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Posture
{
    public class DispHandle
    {

        /// <summary>
        /// Disp index of current frame and running time info
        /// </summary>
        /// <param name="frameCounter">index of the current frame</param>
        /// <param name="timeList">a list of different time info</param>
        /// <param name="maxRecord">the running time of each frame can not exceed (maxRecord FPS)</param>
        /// <param name="statusStrip"></param>
        public void DispRunningTime(int frameCounter, List<double> timeList, int maxRecord, ref StatusStrip statusStrip)
        {
            // Remove the old time
            if (timeList.Count > maxRecord)
            {
                timeList.RemoveRange(0, timeList.Count - maxRecord);
            }

            // Frame counter
            statusStrip.Items["toolStripStatusLabelFrameCounter"].Text = string.Format("{0}", frameCounter.ToString("00000"));

            // Cur time
            statusStrip.Items["toolStripStatusLabelCurTime"].Text = string.Format("Cur: {0} ms", timeList[timeList.Count - 1].ToString("00.00"));

            // Avg time
            statusStrip.Items["toolStripStatusLabelAvgTime"].Text = string.Format("Avg: {0} ms", timeList.Average().ToString("00.00"));

            // Max time
            statusStrip.Items["toolStripStatusLabelMaxTime"].Text = string.Format("Max: {0} ms", timeList.Max().ToString("00.00"));
        }


        /// <summary>
        /// Disp skeleton data in TextBox (when grabing)
        /// </summary>
        /// <param name="skeletonData">A list that include the skeleton data</param>
        /// <param name="textBox"></param>
        public void DispSkeletonData(List<double> skeletonData, ref TextBox textBox)
        {
            string disp_str = "          X           Y           Z   ";
            for (int i = 0; i < skeletonData.Count; i += 3)
            {
                disp_str += Environment.NewLine;
                disp_str += string.Format("{0, 2}   {1, 9}   {2, 9}   {3, 9}", (i / 3 + 1).ToString("00"),
                                                                               skeletonData[i + 0].ToString("0.000"),
                                                                               skeletonData[i + 1].ToString("0.000"),
                                                                               skeletonData[i + 2].ToString("0.000"));
            }
            textBox.Text = disp_str;
        }


        /// <summary>
        /// Disp skeleton data list in ListBox (when Labelling)
        /// </summary>
        /// <param name="dataList">a string array; raw data including skeleton data and index number</param>
        /// <param name="labelList">an int 2-d array; 1st-d:index; 2nd-d:label</param>
        /// <param name="listBox"></param>
        /// <param name="statusStrip"></param>
        public void DispSkeletonDataList(string[] dataList, int[,] labelList, ref ListBox listBox, ref StatusStrip statusStrip)
        {
            // Get the data count
            statusStrip.Items["toolStripStatusLabelAllData"].Text = (dataList.Length - 1).ToString("00000");

            // Refresh the textBox
            listBox.Items.Clear();
            string itemStr = string.Empty;
            for (int i = 1; i < dataList.Length; i++)
            {
                string line = dataList[i];
                string index = line.Split(':')[0];
                string[] data = line.Split(':')[1].Split(',');

                int label = labelList[i - 1, 1];

                itemStr = string.Format("{0}   {1}   {2}", index, data.Length == 75 ? "⚑" : " ", label > -1 ? label.ToString() : " ");
                listBox.Items.Add(itemStr);
            }
            listBox.SelectedIndex = 0;
        }


        /// <summary>
        /// Disp and update the Button and NumericUpDown for all the classes
        /// </summary>
        /// <param name="labels_list">a 2-d array; 1st-d:index number; 2nd-d:label</param>
        /// <param name="fileHandle"></param>
        /// <param name="buttonCnt">button group</param>
        public void DispLabelCnt(int[,] labels_list, FileHandle fileHandle, ref Button[] buttonCnt)
        {
            for (int labelindex = 0; labelindex < buttonCnt.Length; labelindex++)
            {
                if (labels_list == null)
                {
                    return;
                }
                List<string> fileUrls = new List<string>();
                string[] imgPaths = fileHandle.ImgaeFolder.Split(Path.DirectorySeparatorChar);
                for (int i = 0; i < labels_list.GetUpperBound(0) + 1; i++)
                {
                    if (labels_list[i, 1] == labelindex)
                    {
                        string imgPath = Path.Combine("..", "..", "..", imgPaths[imgPaths.Length - 2], imgPaths[imgPaths.Length - 1], labels_list[i, 0].ToString() + ".bmp");
                        fileUrls.Add(imgPath);
                    }
                }

                // Disp
                buttonCnt[labelindex].Text = fileUrls.Count.ToString();
            }
        }
    }
}
