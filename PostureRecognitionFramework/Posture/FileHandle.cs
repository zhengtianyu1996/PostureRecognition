using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Posture
{
    public class FileHandle
    {
        private string[] m_labelnames;

        private string m_IMAGE_FOLDER_NAME = "Images";      // The image folder name
        private string m_LABEL_FOLDER_NAME = "Labels";      // The label folder name
        private string m_DATA_FILE_NAME    = "Data.txt";    // The data file name

        private string m_outputFolder  = string.Empty;      // The output folder
        private string m_imageFolder   = "Images";          // The image folder
        private string m_labelFolder   = "Labels";          // The image folder
        private string m_dataFile      = "default.txt";     // The data file

        /// <summary>
        /// Init function 0
        /// </summary>
        public FileHandle()
        {
        }


        /// <summary>
        /// Init function 1
        /// </summary>
        /// <param name="label_names">{"Standing", "Sitting", "Walking", "StandUp", "SitDown", "Falling"}</param>
        public FileHandle(string[] label_names)
        {
            m_labelnames = label_names;
            SetFolder("Output\\All");
        }


        /// <summary>
        /// Init function 2
        /// </summary>
        /// <param name="label_names">{"Standing", "Sitting", "Walking", "StandUp", "SitDown", "Falling"}</param>
        /// <param name="filefolder"></param>
        public FileHandle(string[] label_names, string filefolder)
        {
            m_labelnames = label_names;
            SetFolder(filefolder);
        }


        /// <summary>
        /// Set the output folder and create sub folders and files
        /// </summary>
        /// <param name="outputFolder"></param>
        public void SetFolder(string outputFolder)
        {
            // Set the output root folder
            m_outputFolder = outputFolder;
            if (!Directory.Exists(m_outputFolder))
            {
                Directory.CreateDirectory(m_outputFolder);
            }

            // Set the image folder
            m_imageFolder = Path.Combine(m_outputFolder, m_IMAGE_FOLDER_NAME);
            if (!Directory.Exists(m_imageFolder))
            {
                Directory.CreateDirectory(m_imageFolder);
            }

            // Set the data txt file
            m_dataFile = Path.Combine(m_outputFolder, m_DATA_FILE_NAME);

            // Set label folder
            m_labelFolder = Path.Combine(m_outputFolder, m_LABEL_FOLDER_NAME);
            if (!Directory.Exists(m_labelFolder))
            {
                Directory.CreateDirectory(m_labelFolder);
            }

            for (int i = 0; i < m_labelnames.Length; i++)
            {
                string tmpFolder = Path.Combine(m_labelFolder, m_labelnames[i]);
                if (!Directory.Exists(tmpFolder))
                {
                    Directory.CreateDirectory(tmpFolder);
                }
            }
        }


        /// <summary>
        /// Does the given folder has all the sub folders and files
        /// </summary>
        /// <param name="outputFolder"></param>
        /// <returns></returns>
        public bool IsValidFolder(string outputFolder)
        {
            // Is the output root folder exist
            if (!Directory.Exists(outputFolder))
            {
                return (false);
            }

            // Is the image folder exist
            string imageFolder = Path.Combine(outputFolder, m_IMAGE_FOLDER_NAME);
            if (!Directory.Exists(imageFolder))
            {
                return (false);
            }

            // Is the data txt file exist
            string dataFile = Path.Combine(outputFolder, m_DATA_FILE_NAME);
            if (!File.Exists(dataFile))
            {
                return (false);
            }

            return (true);
        }


        /// <summary>
        /// Generate only one sample according to the selected items in ListBox
        /// </summary>
        /// <param name="skeletonDataLines">a string array; raw data including skeleton data and index number</param>
        /// <param name="listBox"></param>
        /// <param name="fileHandle"></param>
        /// <param name="url">folder:where to store the samples</param>
        /// <param name="tb"></param>
        /// <param name="cntSamples">the total number of the existing samples</param>
        /// <param name="minsteps">minimum frames in each sample,if less than minsteps, no sample will be generated</param>
        public void WriteSampleData(string[] skeletonDataLines, ListBox listBox, FileHandle fileHandle, string url,
            ref TextBox tb, ref int cntSamples, int minsteps = 15)
        {
            int cntValid = 0;
            // Create a sample
            string dataStr = string.Empty;
            string imgStr = string.Empty;
            foreach (var selectedItem in listBox.SelectedItems)
            {
                // Make sure all the datas are valid
                string[] itemSplit = selectedItem.ToString().Split(' ');
                if (string.IsNullOrWhiteSpace(itemSplit[3]))
                {
                    MessageBox.Show("Empty data exists in the select data. ");
                    return;
                }
                cntValid++;
                // add data string
                string label = itemSplit[0];
                string dataline = skeletonDataLines[Convert.ToInt32(itemSplit[0])].Split(':')[1];
                string[] datas = dataline.Split(',');

                dataStr += label + "\t";
                for (int j = 0; j < datas.Length; j++)
                {
                    dataStr += string.Format("{0}\t", datas[j].Trim());
                }

                dataStr += "\n";
            }

            if (cntValid >= minsteps)
            {
                // Write
                // TXT file to write the skeleton data
                cntSamples++;
                string txtName = string.Format("{0}_{1}_{2}.txt", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), cntSamples.ToString("000"), cntValid);
                fileHandle.WriteMsg(Path.Combine(url, txtName), dataStr);

                // output the information
                string msg = string.Format("Done!\r\nSample No.{0}\r\nValid frames:{1}\r\n\r\n", cntSamples.ToString("000"), cntValid);
                tb.Text += msg;
            }
            else
            {
                MessageBox.Show($"You should at least choose {minsteps} frames.");
            }
        }


        /// <summary>
        /// Generate all the samples according to all the labeled data
        /// Pay attention to the paramaters
        /// </summary>
        /// <param name="skeletonDataLines">a string array; raw data including skeleton data and index number</param>
        /// <param name="labels_list">a 2-d array; 1st-d: index number; 2nd-d: label</param>
        /// <param name="fileHandle"></param>
        /// <param name="url">folder:where to store the samples</param>
        /// <param name="tb"></param>
        /// <param name="cntSamples">the total number of the existing samples</param>
        /// <param name="maxsteps">maximum frames in each sample</param>
        /// <param name="minsteps">minimum frames in each sample,if less than minsteps, no sample will be generated</param>
        /// <param name="cover">how many frames to be covered between different samples</param>
        /// <param name="gap">if the length of empty part is larger than gap, then set all the following labels to -1</param>
        public void WriteSampleData(string[] skeletonDataLines, int[,] labels_list, FileHandle fileHandle,
            string url, ref TextBox tb, ref int cntSamples, int maxsteps = 60, int minsteps = 15, int cover = 30, int gap = 3)
        {
            int cntValidAll = 0;
            for (int i = 0; i < labels_list.GetLength(0);)
            {
                // Make sure first data is valid
                int label = labels_list[i, 1];
                if (label < 0)
                {
                    i++;
                    continue;
                }

                //get data by maxsteps fistly
                int[,] sample = new int[maxsteps, 2];
                int step = Math.Min(labels_list.GetLength(0) - i, maxsteps);
                for (int j = 0; j < step; j++)
                {
                    sample[j, 0] = labels_list[i + j, 0];
                    sample[j, 1] = labels_list[i + j, 1];
                }

                // detect the gap and set the following data to -1
                int cntGap = 0;
                int cntValid = 0;
                //int LstIndex = maxsteps-1;
                for (int j = 0; j < step; j++)
                {
                    if (cntGap > gap)
                    {
                        sample[j, 1] = -1;
                        continue;
                    }
                    if (sample[j, 1] == -1)
                    {
                        cntGap++;
                    }
                    else
                    {
                        cntValid++;
                        cntGap = 0;
                    }
                }

                // initial for next batch
                i = i + cover;

                //check the number of the valid data, move to next batch if not enough
                if (cntValid < minsteps)
                {
                    continue;
                }
                else
                {
                    cntSamples++;
                    cntValidAll += cntValid;
                    string baseName = string.Format("{0}_{1}_{2}", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), cntSamples.ToString("000"), cntValid);
                    string txtName = baseName + ".txt";
                    //string labelName = baseName + ".md";
                    // write the data which is not -1          
                    for (int m = 0; m < step; m++)
                    {
                        string dataStr = string.Empty;
                        //string labelStr = string.Empty;
                        if (sample[m, 1] > -1)
                        {
                            string dataline = skeletonDataLines[sample[m, 0]].Split(':')[1];
                            string[] data_joints = dataline.Split(',');
                            // replace all the Standing with Walking
                            int label_temp = sample[m, 1] == 0 ? 2 : sample[m, 1];
                            dataStr += sample[m, 0].ToString("00000") + "\t" + label_temp.ToString() + "\t";
                            //labelStr += sample[m, 0].ToString("00000") + "\t" + sample[m, 1].ToString();
                            for (int j = 0; j < data_joints.Length; j++)
                            {
                                dataStr += string.Format("{0}\t", data_joints[j].Trim());
                            }
                            // Write
                            fileHandle.WriteMsg(Path.Combine(url, txtName), dataStr);
                        }
                    }
                }
            }

            // output the information
            string msg = string.Format("Done!\r\nUrl:{0}\r\nSample number:{1}\r\nValid frames:{2}\r\n\r\n", url, cntSamples, cntValidAll);
            tb.Text += msg;
        }


        /// <summary>
        /// Write Label data to a .MD file
        /// </summary>
        /// <param name="url">folder: where to save the label info</param>
        /// <param name="labels_list">a 2-d array; 1st-d:index number; 2nd-d:label</param>
        public void WriteLabelsMD(string url, int[,] labels_list)
        {
            if (labels_list==null)
            {
                return;
            }
            FileStream fs = new FileStream(url, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < labels_list.GetUpperBound(0) + 1; i++)
            {
                int index = labels_list[i, 0];
                int label = labels_list[i, 1];

                string itemStr = string.Format("{0}:{1}", index.ToString("00000"), label.ToString());

                // Writing
                sw.WriteLine(itemStr);
                sw.Flush();
            }
            // Close
            sw.Close();
            fs.Close();
        }
       

        /// <summary>
        /// Write txt file
        /// </summary>
        /// <param name="url">folder: where to save the msg</param>
        /// <param name="msg">string</param>
        public void WriteMsg(string url, string msg)
        {
            // Create the file if the file does not exist
            FileMode fileMode = FileMode.Append;
            if (!File.Exists(url))
            {
                fileMode = FileMode.Create;
            }

            // Create file stream and writer
            FileStream fs = new FileStream(url, fileMode, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            // Writing
            sw.WriteLine(msg);
            sw.Flush();

            // Close
            sw.Close();
            fs.Close();
        }

        
        /// <summary>
        /// Read image
        /// </summary>
        /// <param name="imgPath"></param>
        /// <returns></returns>
        public Bitmap ReadImg(string imgPath)
        {
            if (File.Exists(imgPath))
            {
                Bitmap bitmap = new Bitmap(imgPath);
                return (bitmap);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Write image
        /// </summary>
        /// <param name="img">Bitmap</param>
        /// <param name="imgName"></param>
        public void WriteImg(Bitmap img, string imgName)
        {
            string url = Path.Combine(m_imageFolder, imgName);
            img.Save(url, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        // ---------------------------------------------------------------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //

        public string IMAGE_FOLDER_NAME
        {
            get
            {
                return m_IMAGE_FOLDER_NAME;
            }
        }

        public string LABEL_FOLDER_NAME
        {
            get
            {
                return m_LABEL_FOLDER_NAME;
            }
        }

        public string DATA_FILE_NAME
        {
            get
            {
                return m_DATA_FILE_NAME;
            }
        }

        public string OutputFolder
        {
            get
            {
                return m_outputFolder;
            }
        }

        public string ImgaeFolder
        {
            get
            {
                return m_imageFolder;
            }
        }

        public string LabelFolder
        {
            get
            {
                return m_labelFolder;
            }
        }

        public string DataFile
        {
            get
            {
                return m_dataFile;
            }
        }
    }
}
