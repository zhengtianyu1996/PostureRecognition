using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

using nuitrack;
using nuitrack.issues;
using TensorFlow;

namespace Posture
{
    public partial class MainForm : Form
    {
        private DirectBitmap _bitmap;                   // To store color image which can be showed in from
        private bool _visualizeColorImage = true;       // true: color model, false: depth model

        private DepthSensor       _depthSensor;
        private ColorSensor       _colorSensor;
        private UserTracker       _userTracker;
        private SkeletonTracker   _skeletonTracker;
        private HandTracker       _handTracker;
        private GestureRecognizer _gestureRecognizer;

        private DepthFrame _depthFrame;                 // depth frame

        private SkeletonData    _skeletonData;          // person body skeleton data
        private HandTrackerData _handTrackerData;       // person hands data
        private IssuesData      _issuesData = null;

        // Labels
        static readonly string[] m_LABELNAMES = {"Standing", "Sitting", "Walking", "StandUp", "SitDown", "Falling"};
        int[] m_LABELINDEXS = { 0, 1, 2, 3, 4, 5 };
        public Button[] buttonCntGroup;
        public Button[] buttonGroup;

        // Global flag
        bool m_IS_GRAB       = false;       // Grab the images?
        bool m_IS_WRITE_DATA = false;       // Write skeleton data?
        bool m_IS_AUTO       = false;       // Recognize the gesture automatically

        // Running time: Record the running time of each frame, which cannot exceed (1000.0 / FPS)
        static readonly int m_MAX_TIME_RECORD = 100;
        List<double> m_runningTimeList = new List<double>();

        // Frame counter
        int m_frameCounter = 0;

        // Define handle
        FileHandle m_fileHandle = new FileHandle(m_LABELNAMES);     // Output infos
        DispHandle m_DispHandle = new DispHandle();                 // All the functions for disp
        DataHandle m_DataHandle = new DataHandle();                 // All the functions for data

        // Data variables
        List<double> m_skeletonDataList = new List<double>();       // Skeleton data per frame, assign value from _skeletonData
        int m_invalidDataCnt = 0;                                   // Count the continue invalid data
        string[] m_skeletonDataLines;                               // Store all the skeleton data by reading a group txt files
        string m_labels_filename = "Data_labels.md";               // FileName of the txt file to store the labels
        int[,] m_labels;                                             // Store all the corresponding labels for each frame

        // Call net
        //static readonly string m_PB_URL = Path.Combine(Application.StartupPath, @"Model\2018-12-19 14-42-50.pb");
        static readonly string m_PB_URL = @"D:\Postgraduate\PostureRecognition\train\posture_recognition.pb";
        private int[] shape_data = new int[2] {60, 75}; //60:frames 75:25points * 3 xyz
        static int m_INPUT_DATA_NUM = 4500;    // 25(points) x 3(xyz) x 60(frames)
        List<double> m_tfList = new List<double>();     // To store the data
        TFTensor m_tfTensor;                            // Input data
        TFGraph m_tfGraph;                              // tf.Graph
        TFSession m_tfSess;                             // tf.Session


        public MainForm()
        {
            InitializeComponent();

            buttonCntGroup = new Button[] { buttonStandingCnt, buttonSittingCnt, buttonWalkingCnt, buttonStandUpCnt, buttonSitDownCnt, buttonFallingCnt };

            buttonGroup = new Button[] { buttonStanding, buttonSitting, buttonWalking, buttonStandUp, buttonSitDown, buttonFalling };

            // Set timer interval
            numericUpDownFPS_ValueChanged(null, null);

			// Enable double buffering to prevent flicker
			this.DoubleBuffered = true;   
        }

        ~MainForm()
		{
			_bitmap.Dispose();
		}

        // ---------------------------------------------------------------------------------------------------- //
        // ----------------------------------------- Important events ----------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //

        #region The events of timerGrab, buttonGrab, buttonWrite, buttonAuto

        /// <summary>
        /// timerGrab, doing everything
        ///     Updata the _skeletonTracker
        ///     Get and write data
        ///     Recognize the gesture
        ///     Disp result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerGrab_Tick(object sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;

            // -------------------- Update Nuitrack data -------------------- //
			try
			{
                // Data will be synchronized with skeleton time stamps.
                // The code will activate event: 
                    // onColorSensorUpdate: Update _bitmap only for color model
                    // onDepthSensorUpdate: Update _depthFrame
                    // onUserTrackerUpdate: Update _bitmap only for depth model
                    // onSkeletonUpdate   : Update _skeletonData
                    // onHandTrackerUpdate: Update _handTrackerData
                    // onIssueDataUpdate  : Update _issuesData
				Nuitrack.Update(_skeletonTracker);

                // Count frame
                m_frameCounter++;
			}
			catch(LicenseNotAcquiredException exception)
			{
                string output = string.Format("LicenseNotAcquired exception.\nException message: {0}", exception.Message);
                MessageBox.Show(output);
				Trace.WriteLine(output);
                return;
			}
			catch(System.Exception exception)
			{
                string output = string.Format("Nuitrack update failed.\nException message: {0}", exception.Message);
                MessageBox.Show(output);
				Trace.WriteLine(output);
                return;
			}

            // -------------------- Get skeleton data -------------------- //
            // Clear
            m_skeletonDataList.Clear();

			// Write skeleton joints
			if (_skeletonData != null)
			{
                // Draw joint size
	            const int jointSize = 5;

                // Loop for users 
				foreach (var skeleton in _skeletonData.Skeletons)
				{
                    // Loop for joints
					foreach (var joint in skeleton.Joints)
					{
                        // Update the _bitmap
                        _bitmap.SetSquare((int)(joint.Proj.X * _bitmap.Width), (int)(joint.Proj.Y * _bitmap.Height),
							              jointSize, Color.FromArgb(255, 255, 0, 0));

                        // Add skeleton data
                        m_skeletonDataList.Add((double)joint.Real.X);
                        m_skeletonDataList.Add((double)joint.Real.Y);
                        m_skeletonDataList.Add((double)joint.Real.Z);
					}

                    // Only add user 1
                    break;
				}
			}

            // -------------------- Add data to m_tfList for calling model -------------------- //
            if (m_IS_AUTO)      // Auto mode: recognize the gesture automatically
            {
                // Count the continue invalid data
                if (m_skeletonDataList.Count > 0)
                {
                    m_invalidDataCnt = 0;
                    m_tfList.AddRange(m_skeletonDataList);
                }
                else
                {
                    // If the invalid data occur 10 times, we will cleal the m_tfList
                    if (++m_invalidDataCnt > 10)
                    {
                        m_tfList.Clear();
                    }
                }

                // Call the model
                if (m_tfList.Count == m_INPUT_DATA_NUM)
                {
                    // m_tfList (List) --> dataArr (float array)
                    float[, ,] dataArr = new float[1, shape_data[0], shape_data[1]];
                    for (int i = 0; i < m_tfList.Count; i++)
                    {
                        int index_frame = (int)(i / shape_data[1]);
                        int index_joints = (int)(i % shape_data[1]);
                        //for (int j=0; j<75; )
                        dataArr[0, index_frame, index_joints] = (float)m_tfList[i];
                    }

                    // Get the m_tfTensor
                    m_tfTensor = new TFTensor(dataArr);

                    // Call model
                    // Model input: is_training = false, batch_images = skeleton data
                    TFSession.Runner tfRunner = m_tfSess.GetRunner();
                    //tfRunner.AddInput(m_tfGraph["is_training"][0], false);
                    //tfRunner.AddInput(m_tfGraph["batch_images"][0], m_tfTensor);
                    tfRunner.AddInput(m_tfGraph["masking_1_input"][0], m_tfTensor);

                    DateTime time = DateTime.Now;
                    TFTensor testPct = tfRunner.Run(m_tfGraph["output_1"][0]);
                    Trace.WriteLine(string.Format("Time: {0}", (DateTime.Now - time).TotalMilliseconds.ToString()));

                    // Get the label with max value
                    float[] runTestPct = (float[])testPct.GetValue();
                    float maxValue = 0;
                    int maxIndex   = -1;
                    for (int i = 0; i < runTestPct.Length; i++)
                    {
                        if (runTestPct[i] > maxValue)
                        {
                            maxValue = runTestPct[i];
                            maxIndex = i;
                        }
                    }

                    // Disp
                    labelDisp.Text = m_LABELNAMES[maxIndex];
                    labelDisp.Refresh();

                    // Remove the old (m_tfList.Count/2) data
                    m_tfList.RemoveRange(0, m_tfList.Count / 2);
                }
            }

            // -------------------- Write data -------------------- //
            // There are 25 points-3d in skeletonDataList for each user.
            if (m_IS_WRITE_DATA)
            {
                string skeletonDataStr = m_DataHandle.SkeletonData2Str(m_frameCounter, m_skeletonDataList);
                m_fileHandle.WriteMsg(m_fileHandle.DataFile, skeletonDataStr);
                m_fileHandle.WriteImg(_bitmap.Bitmap, string.Format("{0}.bmp", m_frameCounter.ToString("00000")));
            }

            // -------------------- Disp -------------------- //
            // Disp running time
            m_runningTimeList.Add((DateTime.Now - startTime).TotalMilliseconds);
            m_DispHandle.DispRunningTime(m_frameCounter, m_runningTimeList, m_MAX_TIME_RECORD, ref statusStripGrab);

            // Disp skeleton data in textBox
            m_DispHandle.DispSkeletonData(m_skeletonDataList, ref textBoxSkeletonData);

            // Refresh pictureBox
            pictureBox.Image = _bitmap.Bitmap;
            pictureBox.Refresh();
        }

        /// <summary>
        /// Start or Stop the camera grab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGrab_Click(object sender, EventArgs e)
        {
            if (!m_IS_GRAB)
            {
                // -------------------- Initialize nuitrack -------------------- //
			    try
			    {
				    Nuitrack.Init("");
			    }
			    catch(System.Exception exception)
			    {
                    string output = string.Format("Cannot initialize Nuitrack.\nException message: {0}", exception.Message);
                    MessageBox.Show(output);
				    Trace.WriteLine(output);
                    return;
			    }

                // -------------------- Create modules -------------------- //
			    try
			    {
				    // Create and setup all required modules
				    _depthSensor       = DepthSensor.Create();
				    _colorSensor       = ColorSensor.Create();
				    _userTracker       = UserTracker.Create();
				    _skeletonTracker   = SkeletonTracker.Create();
				    _handTracker       = HandTracker.Create();
				    _gestureRecognizer = GestureRecognizer.Create();

                    // Config
			        _depthSensor.SetMirror(false);      // Mirror left and right
			    }
			    catch(System.Exception exception)
			    {
                    string output = string.Format("Cannot create Nuitrack module.\nException message: {0}", exception.Message);
                    MessageBox.Show(output);
				    Trace.WriteLine(output);
                    return;
			    }

                // -------------------- Add events -------------------- //
			    // Add event handlers for all modules
			    _depthSensor.OnUpdateEvent             += onDepthSensorUpdate;
			    _colorSensor.OnUpdateEvent             += onColorSensorUpdate;
			    _userTracker.OnUpdateEvent             += onUserTrackerUpdate;
			    _skeletonTracker.OnSkeletonUpdateEvent += onSkeletonUpdate;
			    _handTracker.OnUpdateEvent             += onHandTrackerUpdate;
			    _gestureRecognizer.OnNewGesturesEvent  += onNewGestures;

			    // Add an event handler for the IssueUpdate event 
			    Nuitrack.onIssueUpdateEvent += onIssueDataUpdate;

                // -------------------- Bitmap object -------------------- //
			    // Create and configure the Bitmap object according to the depth sensor output mode
			    OutputMode mode      = _depthSensor.GetOutputMode();
			    OutputMode colorMode = _colorSensor.GetOutputMode();

			    if (mode.XRes < colorMode.XRes)
				    mode.XRes = colorMode.XRes;
			    if (mode.YRes < colorMode.YRes)
				    mode.YRes = colorMode.YRes;

			    _bitmap = new DirectBitmap(mode.XRes, mode.YRes);
			    for (int y = 0; y < mode.YRes; ++y)
			    {
				    for (int x = 0; x < mode.XRes; ++x)
					    _bitmap.SetPixel(x, y, Color.FromKnownColor(KnownColor.Aqua));
			    }

                // -------------------- Run Nuitrack -------------------- //
			    // Run Nuitrack. This starts sensor data processing.
			    try
			    {
				    Nuitrack.Run();
			    }
			    catch(System.Exception exception)
			    {
                    string output = string.Format("Cannot start Nuitrack.\nException message: {0}", exception.Message);
                    MessageBox.Show(output);
				    Trace.WriteLine(output);
                    return;
			    }

                // ------- ------------- Update global variables -------------------- //
                m_IS_GRAB               = true;
                m_frameCounter          = 0;
                timerGrab.Enabled       = true;
                buttonGrab.Text         = "Grab ◼";
                buttonGrab.BackColor    = Color.GreenYellow;

                // Disabled auto button
                checkBox_auto.Enabled = false;
            }
            else
            {
                // -------------------- Release Nuitrack and remove all modules -------------------- //
			    try
			    {
				    Nuitrack.onIssueUpdateEvent -= onIssueDataUpdate;

				    _depthSensor.OnUpdateEvent             -= onDepthSensorUpdate;
				    _colorSensor.OnUpdateEvent             -= onColorSensorUpdate;
				    _userTracker.OnUpdateEvent             -= onUserTrackerUpdate;
				    _skeletonTracker.OnSkeletonUpdateEvent -= onSkeletonUpdate;
				    _handTracker.OnUpdateEvent             -= onHandTrackerUpdate;
				    _gestureRecognizer.OnNewGesturesEvent  -= onNewGestures;

				    Nuitrack.Release();
			    }
			    catch(System.Exception exception)
			    {
                    string output = string.Format("Nuitrack release failed.\nException message: {0}", exception.Message);
                    MessageBox.Show(output);
				    Trace.WriteLine(output);
                    return;
			    }

                // -------------------- Update global variables -------------------- //
                m_IS_GRAB            = false;
                timerGrab.Enabled    = false;
                buttonGrab.Text      = "Grab ▶";
                buttonGrab.BackColor = Color.FromArgb(0, 255, 255, 255);

                // Enabled auto button
                checkBox_auto.Enabled = true;
            }
        }


        #endregion

        // ---------------------------------------------------------------------------------------------------- //
        // -------------------------------------- Load and test pb model -------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //

        #region Load and test pb model

        /// <summary>
        /// Load a pb model
        /// </summary>
        /// <param name="pbFile">The url of pb model</param>
        /// <returns></returns>
        private bool LoadPbModel(string pbFile)
        {
            try
            {
                // Load pb model
                m_tfGraph = new TFGraph();
                m_tfGraph.Import(File.ReadAllBytes(pbFile));

                // Initial tf.Session
                m_tfSess = new TFSession(m_tfGraph);

                // -------------------- Do a sess.run -------------------- //
                // The first run need 3000 ms, then only need <10 ms every run, I donot know why?
                //float[,] dataArr = new float[1, m_INPUT_DATA_NUM];
                float[,,] dataArr = new float[1, shape_data[0], shape_data[1]];
                TFTensor dataTensor = new TFTensor(dataArr);

                // Call model
                TFSession.Runner tfRunner = m_tfSess.GetRunner();
                tfRunner.AddInput(m_tfGraph["masking_1_input"][0], dataTensor);
                //tfRunner.AddInput(m_tfGraph["batch_images"][0], dataTensor);
                TFTensor output = tfRunner.Run(m_tfGraph["output_1"][0]);

                // Return
                return (true);
            }
            catch (System.Exception)
            {
                // Return
                
                return (false);
            }
        }

        /// <summary>
        /// Load a pb model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadPb_Click(object sender, EventArgs e)
        {
            // Selete a pb model file
            string pbFile = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "..", "..", "train")),
                Title = "Please select pb model file. ",
                Filter = "pb|*.pb"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pbFile = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            // Load pb model
            bool isLoad = LoadPbModel(pbFile);
            if (isLoad)
            {
                string msg = string.Format("\r\nSuccessfully load the pb model:\r\n {0} \r\n", pbFile);
                textBox_Info.Text += msg;
            }
            else
            {
                MessageBox.Show("An error occurred in loading model. ");
            }
        }

        /// <summary>
        /// Test a sample using the loaded model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTestSample_Click(object sender, EventArgs e)
        {
            // -------------------- Select a txt file and a md file -------------------- //
            // Selete a txt or md file
            string selectFile = string.Empty;
               
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "..", "..", "data", "Samples")),
                Title = "Please select a txt",
                Filter = "data(txt)|*.txt"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectFile = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            // Get the url of txt file and md file
            string txtFile = selectFile.Split('.')[0] + ".txt";
            //string mdFile  = selectFile.Split('.')[0] + ".md";

            // -------------------- txt file for calling model -------------------- //
            // This part of the code is similar to the code in function timerGrab_Tick.
            try
            {
                // Read all lines
                string[] dataLines = File.ReadAllLines(txtFile);

                // Loop, get the skeleton data array
                float[,,] dataArr = new float[1, shape_data[0], shape_data[1]];
                int[] labelArr = new int[shape_data[0]];
                for (int i = 0; i < shape_data[0]; i++)
                {
                    if (i< dataLines.Length)
                    {
                        string[] data_line = dataLines[i].Split('\t');
                        for (int j = 0; j < shape_data[1]; j++)
                        {
                            dataArr[0, i, j] = (float)Convert.ToDouble(data_line[j + 2]);
                            labelArr[i]= Convert.ToInt32(data_line[1])-1;    // -1 because we replace 0 by 2
                        }
                    }
                    else
                    {
                        for (int j = 0; j < shape_data[1]; j++)
                        {
                            dataArr[0, i, j] = -1;
                            labelArr[i] = -1;
                        }
                    }
                    
                }
                
                // Get the m_tfTensor
                m_tfTensor = new TFTensor(dataArr);

                // Call model
                TFSession.Runner tfRunner = m_tfSess.GetRunner();
                tfRunner.AddInput(m_tfGraph["masking_1_input"][0], dataArr);
                TFTensor tf_output = tfRunner.Run(m_tfGraph["output_1"][0]);

                DateTime time = DateTime.Now;
                Trace.WriteLine(string.Format("Time: {0}", (DateTime.Now - time).TotalMilliseconds.ToString()));

                // Get the label with max value
                float[,] value_output = (float[,])tf_output.GetValue();
                int[] predArr = new int[shape_data[0]];
                
                //int maxIndex = -1;
                for (int i = 0; i < value_output.GetLength(0); i++)
                {
                    float maxValue = 0;
                    for (int j = 0; j < value_output.GetLength(1); j++)
                    {
                        if (value_output[i, j] > maxValue)
                        {
                            maxValue = value_output[i, j];
                            predArr[i] = j;
                        }
                    }
                }

                int cntEq = 0;
                for (int i = 0; i < dataLines.Length; i++)
                {
                    if (predArr[i]==labelArr[i])
                    {
                        cntEq++;
                    }
                }

                float accuracy = (float)cntEq / dataLines.Length;


                string msg="";
                msg += string.Format("\r\nFileUrl:\r\n{0} \r\n", txtFile);
                msg += string.Format("\r\nAccuracy:\r\n{0}% \r\n", accuracy*100);
                textBox_Info.Text += msg;
            }
            catch (System.Exception)
            {
                MessageBox.Show("Please load model correctly! ");
                return;
            }

            // -------------------- md file for disping vedio -------------------- //
            //m_DispHandle.DispSample(mdFile, m_fileHandle, ref pictureBox, Convert.ToInt32(hScrollBar_speed.Value));
        }

        #endregion

        // ---------------------------------------------------------------------------------------------------- //
        // ------------------------------------------ Label Control ------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //

        #region Label control

        /// <summary>
        /// Load skeleton data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadData_Click(object sender, EventArgs e)
        {
            // Selete a data file
            string dataFile = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.Combine(Application.StartupPath, "Output"),
                RestoreDirectory = true,
                Filter = "data(txt)|*.txt",
                Title = "Please select the data file. "
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                dataFile = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            // Set the m_fileHandle
            string dataFolder = Path.GetDirectoryName(dataFile);
            if (m_fileHandle.IsValidFolder(dataFolder))
            {
                // Set folder
                m_fileHandle.SetFolder(dataFolder);

                // Read the data file
                m_skeletonDataLines = m_DataHandle.ReadAllLine(m_fileHandle.DataFile);

                // Read the labels, or Create one if no label information found. Notice the length shoudl minus 1
                m_labels = m_DataHandle.GetLabels(Path.Combine(dataFolder, m_labels_filename), m_skeletonDataLines.Length - 1);

                // Disp label count
                m_DispHandle.DispLabelCnt(m_labels, m_fileHandle, ref buttonCntGroup);

                // Disp the data list
                m_DispHandle.DispSkeletonDataList(m_skeletonDataLines, m_labels, ref listBoxData, ref statusStripLabel);

                // Disp Folder
                string[] dataFolderSplit = dataFolder.Split(Path.DirectorySeparatorChar);
                toolStripStatusLabelFolder.Text = "URL: " + dataFolderSplit[dataFolderSplit.Length - 1];
            }
            else
            {
                MessageBox.Show("Invalid folder. ");
                return;
            }
        }

        /// <summary>
        /// Select data and disp the corresponding image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxData_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Disp the select items count
            statusStripLabel.Items["toolStripStatusLabelSelectData"].Text = listBoxData.SelectedItems.Count.ToString("00");

            // Show the image
            if (listBoxData.SelectedItems.Count > 0)
            {
                string[] item    = listBoxData.SelectedItems[listBoxData.SelectedItems.Count-1].ToString().Split(' ');
                string imgName = item[0];
                string imgPath = Path.Combine(m_fileHandle.ImgaeFolder, imgName + ".bmp");
                Bitmap bitmap  = m_fileHandle.ReadImg(imgPath);

                // Draw the image
                pictureBox.Image = bitmap;
                pictureBox.Refresh();

                // show the label
                string labelindexstr = item[item.Length - 1];
                if (!string.IsNullOrWhiteSpace(labelindexstr))
                {
                    int labelindexint = Convert.ToInt32(labelindexstr);
                    labelDisp.Text = m_LABELNAMES[labelindexint];
                }
            }
        }

        /// <summary>
        /// Disp the select data and image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDispVedio_Click(object sender, EventArgs e)
        {
            // Show the image
            foreach (var selectItem in listBoxData.SelectedItems)
            {
                string item    = selectItem.ToString();
                string imgName = item.Split(' ')[0];
                string imgPath = Path.Combine(m_fileHandle.ImgaeFolder, imgName + ".bmp");
                Bitmap bitmap  = m_fileHandle.ReadImg(imgPath);

                // Draw the image
                pictureBox.Image = bitmap;
                pictureBox.Refresh();

                // Delay
                System.Threading.Thread.Sleep(Convert.ToInt32(hScrollBar_speed.Value));
            }
        }

        /// <summary>
        /// Auto select the next batch images, batchSize = CombineData
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAutoSelect_Click(object sender, EventArgs e)
        {
            // Calculate the count about select items
            int cntTotal = Convert.ToInt32(numericUpDownCombineData.Value);
            int cntCover = Convert.ToInt32(numericUpDownCoverData.Value);
            m_DataHandle.SelectSamples(ref listBoxData, cntTotal, cntCover, true);
        }


        /// <summary>
        /// Delete useless images after labeling
        /// The function is not important, just to reduce the memory footprint of the image 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Press shift: delete unused images
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Selete a data file
                string dataFile = string.Empty;
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = Path.Combine(Application.StartupPath, "Output"),
                    RestoreDirectory = true,
                    Title = "Please select the data file. "
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataFile = openFileDialog.FileName;
                }
                else
                {
                    return;
                }

                // Set the m_fileHandle
                string dataFolder = Path.GetDirectoryName(dataFile);
                DialogResult result = MessageBox.Show(string.Format("Are you sure to delete the unused images in the folder: \n{0}?", dataFolder),
                                                      "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    // Get all the useful images
                    List<string> imgUrlsList = new List<string>();
                    string labelFolder = Path.Combine(dataFolder, m_fileHandle.LABEL_FOLDER_NAME);
                    foreach (string labelname in m_LABELNAMES)
                    {
                        // Every label folder
                        string mdFolder = Path.Combine(labelFolder, labelname);
                        string[] mdUrls = Directory.GetFiles(mdFolder);
                        foreach (string mdUrl in mdUrls)
                        {
                            // Continue if not markdown file
                            if (Path.GetExtension(mdUrl) != ".md")
                            {
                                continue;
                            }

                            // Read md file
                            string[] lines = File.ReadAllLines(mdUrl);
                            foreach (string line in lines)
                            {
                                // Continue if line is empty
                                if (string.IsNullOrWhiteSpace(line))
                                {
                                    continue;
                                }

                                // Get the img url
                                string[] lineSplit = line.Split('(');
                                string imgUrl = lineSplit[1].Split(')')[0];

                                // Add 
                                if (string.IsNullOrWhiteSpace(imgUrlsList.Find(zp => zp == imgUrl)))
                                {
                                    imgUrlsList.Add(imgUrl);
                                }
                            }
                        }
                    }

                    // Delete the unused images
                    string imgFolder = Path.Combine(dataFolder, m_fileHandle.IMAGE_FOLDER_NAME);
                    string[] imgUrls = Directory.GetFiles(imgFolder);
                    foreach (string imgUrl in imgUrls)
                    {
                        // add image string: ..\..\..\DataFloder\ImagesFolder\ImageName
                        string[] imgUrlSplit = imgUrl.Split(Path.DirectorySeparatorChar);
                        string imgPath = Path.Combine("..", "..", "..", imgUrlSplit[imgUrlSplit.Length - 3], imgUrlSplit[imgUrlSplit.Length - 2], imgUrlSplit[imgUrlSplit.Length - 1]);
                        if (string.IsNullOrWhiteSpace(imgUrlsList.Find(zp => zp == imgPath)))
                        {
                            if (File.Exists(imgUrl))
                            {
                                File.Delete(imgUrl);
                            }
                        }
                    }
                }
                else
                {
                    return;
                }

                // Unused images have been deleted.
                MessageBox.Show("Unused images have been deleted. ");
            }
            // No shift: Clear labels
            else
            {
                m_DataHandle.UpdateLabel(ref m_labels, Path.Combine(m_fileHandle.OutputFolder, m_labels_filename), ref listBoxData, -1, m_fileHandle);
                m_DispHandle.DispLabelCnt(m_labels, m_fileHandle, ref buttonCntGroup);
            }

        }
        
        #endregion

        #region Write label data

        /// <summary>
        /// Write Stand label data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStanding_Click(object sender, EventArgs e)
        {
            try
            {
                m_DataHandle.UpdateLabel(ref m_labels, Path.Combine(m_fileHandle.OutputFolder, m_labels_filename), ref listBoxData, m_LABELINDEXS[0], m_fileHandle);
                m_DispHandle.DispLabelCnt(m_labels, m_fileHandle, ref buttonCntGroup);
            }
            catch (System.Exception exception)
            {
                string output = string.Format("buttonStand_Click error.\nException message: {0}", exception.Message);
                MessageBox.Show(output);
				Trace.WriteLine(output);
                return;
            }
        }

        /// <summary>
        /// Write Sit label data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSitting_Click(object sender, EventArgs e)
        {
            try
            {
                m_DataHandle.UpdateLabel(ref m_labels, Path.Combine(m_fileHandle.OutputFolder, m_labels_filename), ref listBoxData, m_LABELINDEXS[1], m_fileHandle);
                m_DispHandle.DispLabelCnt(m_labels, m_fileHandle, ref buttonCntGroup);
            }
            catch (System.Exception exception)
            {
                string output = string.Format("buttonStand_Click error.\nException message: {0}", exception.Message);
                MessageBox.Show(output);
				Trace.WriteLine(output);
                return;
            }
        }

        /// <summary>
        /// Write Walking label data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWalking_Click(object sender, EventArgs e)
        {
            try
            {
                m_DataHandle.UpdateLabel(ref m_labels, Path.Combine(m_fileHandle.OutputFolder, m_labels_filename), ref listBoxData, m_LABELINDEXS[2], m_fileHandle);
                m_DispHandle.DispLabelCnt(m_labels, m_fileHandle, ref buttonCntGroup);
            }
            catch (System.Exception exception)
            {
                string output = string.Format("buttonStand_Click error.\nException message: {0}", exception.Message);
                MessageBox.Show(output);
				Trace.WriteLine(output);
                return;
            }
        }

        /// <summary>
        /// Write StandUp label data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStandUp_Click(object sender, EventArgs e)
        {
            try
            {
                m_DataHandle.UpdateLabel(ref m_labels, Path.Combine(m_fileHandle.OutputFolder, m_labels_filename), ref listBoxData, m_LABELINDEXS[3], m_fileHandle);
                m_DispHandle.DispLabelCnt(m_labels, m_fileHandle, ref buttonCntGroup);
            }
            catch (System.Exception exception)
            {
                string output = string.Format("buttonStand_Click error.\nException message: {0}", exception.Message);
                MessageBox.Show(output);
				Trace.WriteLine(output);
                return;
            }
        }

        /// <summary>
        /// Write SitDown label data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSitDown_Click(object sender, EventArgs e)
        {
            try
            {
                m_DataHandle.UpdateLabel(ref m_labels, Path.Combine(m_fileHandle.OutputFolder, m_labels_filename), ref listBoxData, m_LABELINDEXS[4], m_fileHandle);
                m_DispHandle.DispLabelCnt(m_labels, m_fileHandle, ref buttonCntGroup);
            }
            catch (System.Exception exception)
            {
                string output = string.Format("buttonStand_Click error.\nException message: {0}", exception.Message);
                MessageBox.Show(output);
				Trace.WriteLine(output);
                return;
            }
        }

        /// <summary>
        /// Write TurnBack label data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTurnBack_Click(object sender, EventArgs e)
        {
            try
            {
                m_DataHandle.UpdateLabel(ref m_labels, Path.Combine(m_fileHandle.OutputFolder, m_labels_filename), ref listBoxData, m_LABELINDEXS[5], m_fileHandle);
                m_DispHandle.DispLabelCnt(m_labels, m_fileHandle, ref buttonCntGroup);
            }
            catch (System.Exception exception)
            {
                string output = string.Format("buttonStand_Click error.\nException message: {0}", exception.Message);
                MessageBox.Show(output);
				Trace.WriteLine(output);
                return;
            }
        }

        /// <summary>
        /// Write Others label data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFalling_Click(object sender, EventArgs e)
        {
            try
            {
                m_DataHandle.UpdateLabel(ref m_labels, Path.Combine(m_fileHandle.OutputFolder, m_labels_filename), ref listBoxData, m_LABELINDEXS[5], m_fileHandle);
                m_DispHandle.DispLabelCnt(m_labels, m_fileHandle, ref buttonCntGroup);
            }
            catch (System.Exception exception)
            {
                string output = string.Format("buttonStand_Click error.\nException message: {0}", exception.Message);
                MessageBox.Show(output);
				Trace.WriteLine(output);
                return;
            }
        }

        #endregion

        #region Open label folder

        /// <summary>
        /// Open Stand label folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStandingCnt_Click(object sender, EventArgs e)
        {
            string folderUrl = Path.Combine(m_fileHandle.LabelFolder, m_LABELNAMES[0]);
            if (Directory.Exists(folderUrl))
            {
                Process.Start(folderUrl);
            }
        }

        /// <summary>
        /// Open Sit label folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSittingCnt_Click(object sender, EventArgs e)
        {
            string folderUrl = Path.Combine(m_fileHandle.LabelFolder, m_LABELNAMES[1]);
            if (Directory.Exists(folderUrl))
            {
                Process.Start(folderUrl);
            }
        }

        /// <summary>
        /// Open Walking label folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWalkingCnt_Click(object sender, EventArgs e)
        {
            string folderUrl = Path.Combine(m_fileHandle.LabelFolder, m_LABELNAMES[2]);
            if (Directory.Exists(folderUrl))
            {
                Process.Start(folderUrl);
            }
        }

        /// <summary>
        /// Open StandUp label folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStandUpCnt_Click(object sender, EventArgs e)
        {
            string folderUrl = Path.Combine(m_fileHandle.LabelFolder, m_LABELNAMES[3]);
            if (Directory.Exists(folderUrl))
            {
                Process.Start(folderUrl);
            }
        }

        /// <summary>
        /// Open SitDown label folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSitDownCnt_Click(object sender, EventArgs e)
        {
            string folderUrl = Path.Combine(m_fileHandle.LabelFolder, m_LABELNAMES[4]);
            if (Directory.Exists(folderUrl))
            {
                Process.Start(folderUrl);
            }
        }

        /// <summary>
        /// Open TurnBack label folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTurnbackCnt_Click(object sender, EventArgs e)
        {
            string folderUrl = Path.Combine(m_fileHandle.LabelFolder, m_LABELNAMES[5]);
            if (Directory.Exists(folderUrl))
            {
                 System.Diagnostics.Process.Start(folderUrl);
            }
        }

        /// <summary>
        /// Open Other label folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOthersCnt_Click(object sender, EventArgs e)
        {
            string folderUrl = Path.Combine(m_fileHandle.LabelFolder, m_LABELNAMES[6]);
            if (Directory.Exists(folderUrl))
            {
                Process.Start(folderUrl);
            }
        }

        #endregion

        // ---------------------------------------------------------------------------------------------------- //
        // --------------------------------------- Form control events ---------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //

        #region Form control events

        /// <summary>
        /// Switch visualization mode on a mouse click.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnClick(EventArgs args)
		{
			base.OnClick(args);

			//_visualizeColorImage = !_visualizeColorImage;
		}

        /// <summary>
        /// Event handler for the FormClosing event.
        /// Release Nuitrack and remove all modules.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
		{
			try
			{
                if (m_IS_GRAB)
                {
				    Nuitrack.onIssueUpdateEvent -= onIssueDataUpdate;

				    _depthSensor.OnUpdateEvent             -= onDepthSensorUpdate;
				    _colorSensor.OnUpdateEvent             -= onColorSensorUpdate;
				    _userTracker.OnUpdateEvent             -= onUserTrackerUpdate;
				    _skeletonTracker.OnSkeletonUpdateEvent -= onSkeletonUpdate;
				    _handTracker.OnUpdateEvent             -= onHandTrackerUpdate;
				    _gestureRecognizer.OnNewGesturesEvent  -= onNewGestures;

				    Nuitrack.Release();
                }
			}
			catch(System.Exception exception)
			{
                string output = string.Format("Nuitrack release failed.\nException message: {0}", exception.Message);
                MessageBox.Show(output);
				Trace.WriteLine(output);
                return;
			}
		}

        /// <summary>
        /// Set the FPS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownFPS_ValueChanged(object sender, EventArgs e)
        {
            // Timer grab interval = 1000.0 / FPS
            timerGrab.Interval = Convert.ToInt32(1000.0 / (double)numericUpDownFPS.Value);
        }

        #endregion

        // ---------------------------------------------------------------------------------------------------- //
        // --------------------------------------- Updata camera datas ---------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //

        #region Updata camera datas

        /// <summary>
        /// Event handler for the IssueDataUpdate event: Update _issuesData
        /// </summary>
        /// <param name="issuesData"></param>
        private void onIssueDataUpdate(IssuesData issuesData)
		{
			_issuesData = issuesData;
		}

		/// <summary>
        /// Event handler for the DepthSensorUpdate event: Update _depthFrame
        /// </summary>
        /// <param name="depthFrame"></param>
		private void onDepthSensorUpdate(DepthFrame depthFrame)
		{
			_depthFrame = depthFrame;
		}

		/// <summary>
        /// Event handler for the ColorSensorUpdate event: Update _bitmap
        /// </summary>
        /// <param name="colorFrame"></param>
        /// Notes: Convert the color frame data to bitmap, only sampling directly
		private void onColorSensorUpdate(ColorFrame colorFrame)
		{
            // Only for color model
			if (!_visualizeColorImage)
				return;

            // Step value in width and height
			float wStep = (float)_bitmap.Width / colorFrame.Cols;
			float hStep = (float)_bitmap.Height / colorFrame.Rows;
			
			// color frame data: width x height x 3
            // color frame data format: b, g, r, b, g, r, ...
            // bitmap fromat: argb
			const int elemSizeInBytes = 3;  
			Byte[] data   = colorFrame.Data;
			int colorPtr  = 0;
			int bitmapPtr = 0;

            // Loop for rows
			float nextVerticalBorder = hStep;
			for (int i = 0; i < _bitmap.Height; ++i)
			{
				if (i == (int)nextVerticalBorder)
				{
					colorPtr += colorFrame.Cols * elemSizeInBytes;
					nextVerticalBorder += hStep;
				}

				int offset = 0;
				int argb = data[colorPtr] | (data[colorPtr + 1] << 8) | (data[colorPtr + 2] << 16) | (0xFF << 24);

                // Loop for cols
				float nextHorizontalBorder = wStep;
				for (int j = 0; j < _bitmap.Width; ++j)
				{
					if (j == (int)nextHorizontalBorder)
					{
						offset += elemSizeInBytes;
						argb = data[colorPtr + offset] | (data[colorPtr + offset + 1] << 8) | (data[colorPtr + offset + 2] << 16) | (0xFF << 24);
						nextHorizontalBorder += wStep;
					}

                    // Assign the bitmap
					_bitmap.Bits[bitmapPtr++] = argb;
				}
			}
		}

        #endregion

        // ---------------------------------------------------------------------------------------------------- //
        // ---------------------------------------- NUITRICK handlers ----------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //

        #region NUITRICK handlers

        /// <summary>
        /// Event handler for the UserTrackerUpdate event
        /// </summary>
        /// <param name="userFrame"></param>
        private void onUserTrackerUpdate(UserFrame userFrame)
		{
            // Only for depth model
			if (_visualizeColorImage)
				return;
			if (_depthFrame == null)
				return;

            // labelIssueState???
			const int MAX_LABELS = 7;
			bool[] labelIssueState = new bool[MAX_LABELS];
			for (UInt16 label = 0; label < MAX_LABELS; ++label)
			{
				labelIssueState[label] = false;
				if (_issuesData != null)
				{
					FrameBorderIssue frameBorderIssue = _issuesData.GetUserIssue<FrameBorderIssue>(label);
					labelIssueState[label] = (frameBorderIssue != null);
				}
			}

            // Step value in width and height
			float wStep = (float)_bitmap.Width / _depthFrame.Cols;
			float hStep = (float)_bitmap.Height / _depthFrame.Rows;

            // depth Frame data: width x height x 2
			const int elemSizeInBytes = 2;
			Byte[] dataDepth = _depthFrame.Data;
			Byte[] dataUser = userFrame.Data;
			int dataPtr = 0;
			int bitmapPtr = 0;

            // Loop for rows
			float nextVerticalBorder = hStep;
			for (int i = 0; i < _bitmap.Height; ++i)
			{
				if (i == (int)nextVerticalBorder)
				{
					dataPtr += _depthFrame.Cols * elemSizeInBytes;
					nextVerticalBorder += hStep;
				}

				int offset = 0;
				int argb = 0;
				int label = dataUser[dataPtr] | dataUser[dataPtr + 1] << 8;
				int depth = Math.Min(255, (dataDepth[dataPtr] | dataDepth[dataPtr + 1] << 8) / 32);

                // Loop for cols
				float nextHorizontalBorder = wStep;
				for (int j = 0; j < _bitmap.Width; ++j)
				{
					if (j == (int)nextHorizontalBorder)
					{
						offset += elemSizeInBytes;
						label = dataUser[dataPtr + offset] | dataUser[dataPtr + offset + 1] << 8;
						if (label == 0)
							depth = Math.Min(255, (dataDepth[dataPtr + offset] | dataDepth[dataPtr + offset + 1] << 8) / 32);
						nextHorizontalBorder += wStep;
					}

					if (label > 0)
					{
						int user = label * 40;
						if (!labelIssueState[label])
							user += 40;
						argb = 0 | (user << 8) | (0 << 16) | (0xFF << 24);
					}
					else
					{
						argb = depth | (depth << 8) | (depth << 16) | (0xFF << 24);
					}

                    // Assign the bitmap
					_bitmap.Bits[bitmapPtr++] = argb;
				}
			}
		}

		/// <summary>
        /// Event handler for the SkeletonUpdate event: Update _skeletonData
        /// </summary>
        /// <param name="skeletonData"></param>
		private void onSkeletonUpdate(SkeletonData skeletonData)
		{
			_skeletonData = skeletonData;
		}

		/// <summary>
        /// Event handler for the HandTrackerUpdate event: Update _handTrackerData
        /// </summary>
        /// <param name="handTrackerData"></param>
		private void onHandTrackerUpdate(HandTrackerData handTrackerData)
		{
			_handTrackerData = handTrackerData;
		}

		/// <summary>
        /// Event handler for the gesture detection event: Disp gestures in the console
        /// </summary>
        /// <param name="gestureData"></param>
		private void onNewGestures(GestureData gestureData)
		{
			// Display the information about detected gestures in the console
			foreach (var gesture in gestureData.Gestures)
				Trace.WriteLine(string.Format("Recognized {0} from user {1}", gesture.Type.ToString(), gesture.UserID));
		}

        #endregion


        private int cntSamples = 0;
        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            int minsteps = 15;
            int gap = 3;
            
            // set the state of the button
            buttonGenerate.Text = "Running";
            buttonGenerate.BackColor = Color.OrangeRed;
            buttonGenerate.Enabled = false;

            // [One-Click Function] If Shift Key is pressed, start one-click processing
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog
                {
                    SelectedPath = Path.Combine(System.Environment.CurrentDirectory,"Output"),
                    Description = "Please choose the data folder."
                };
                string foldPath="";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foldPath = dialog.SelectedPath;
                }
                else
                {
                    return;
                }
                MessageBox.Show("Welcome to One-Click function. Please check parameters [step] and [cover] and wait until next MessageBox.");

                DirectoryInfo TheFolder = new DirectoryInfo(foldPath);
                foreach (DirectoryInfo Dir in TheFolder.GetDirectories())
                {
                    if (string.Equals(Dir.Name, "All"))
                    {
                        continue;
                    }
                    // Set folder
                    m_fileHandle.SetFolder(Dir.FullName);

                    // Read the data file
                    m_skeletonDataLines = m_DataHandle.ReadAllLine(m_fileHandle.DataFile);

                    // Read the labels, or Create one if no label information found. Notice the length shoudl minus 1
                    m_labels = m_DataHandle.GetLabels(Path.Combine(Dir.FullName, m_labels_filename), m_skeletonDataLines.Length - 1);

                    // Create the outpu folder
                    string[] dataFolder1 = m_fileHandle.OutputFolder.Split('\\');
                    string dataName1 = dataFolder1[dataFolder1.Length - 1];
                    string path_sample1 = Path.Combine("..", "..", "..", "..", "data", "Samples", dataName1);
                    if (!Directory.Exists(path_sample1))
                    {
                        Directory.CreateDirectory(path_sample1);
                    }

                    // get the number of existing files
                    cntSamples = Directory.GetFiles(path_sample1,"*.txt").Length;

                    // Calculate the count about select items
                    int cntTotal = Convert.ToInt32(numericUpDownCombineData.Value);
                    int cntCover = Convert.ToInt32(numericUpDownCoverData.Value);
                    m_fileHandle.WriteSampleData(m_skeletonDataLines, m_labels, m_fileHandle, path_sample1, ref textBox_Info, ref cntSamples,
                        cntTotal, minsteps, cntCover, gap);
                }
                // Reset the state
                buttonGenerate.Text = "Generate";
                buttonGenerate.Enabled = true;
                buttonGenerate.BackColor = Color.Transparent;

                MessageBox.Show("All Done! Please refer to the dest folder.");
                return;
            }

            // Create the outpu folder
            string[] dataFolder = m_fileHandle.OutputFolder.Split('\\');
            string dataName = dataFolder[dataFolder.Length - 1];
            string path_sample = Path.Combine("..", "..", "..", "..", "data", "Samples", dataName);
            //m_ztyHandle.WriteSampleData(m_skeletonDataLines, m_labels, m_fileHandle, path_sample);
            if (!Directory.Exists(path_sample))
            {
                Directory.CreateDirectory(path_sample);
            }

            // get the number of existing files
            cntSamples = Directory.GetFiles(path_sample).Length;

            if (radioBtnGenAuto.Checked)
            {
                // Calculate the count about select items
                int cntTotal = Convert.ToInt32(numericUpDownCombineData.Value);
                int cntCover = Convert.ToInt32(numericUpDownCoverData.Value);
                m_fileHandle.WriteSampleData(m_skeletonDataLines, m_labels, m_fileHandle, path_sample, ref textBox_Info, ref cntSamples,
                    cntTotal, minsteps, cntCover, gap);
            }
            else if (radioBtnGenMann.Checked)
            {
                // Calculate the count about select items
                int cntTotal = Convert.ToInt32(numericUpDownCombineData.Value);
                int cntCover = Convert.ToInt32(numericUpDownCoverData.Value);
                m_fileHandle.WriteSampleData(m_skeletonDataLines, listBoxData, m_fileHandle, path_sample, ref textBox_Info, ref cntSamples, minsteps);
            }

            //// Write the label file to new folder
            //m_fileHandle.WriteLabelsMD(Path.Combine(path_sample,"Data_labels.md"), m_labels);

            // Reset the state
            buttonGenerate.Text = "Generate";
            buttonGenerate.Enabled = true;
            buttonGenerate.BackColor = Color.Transparent;
        }

        private void radioBtnGenAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnGenAuto.Checked)
            {
                DialogResult result = MessageBox.Show(string.Format("The program will run automatically in auto mode.\nPlease pay attention to parameters: [steps] and [cover]."),
                                                  "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void listBoxData_Click(object sender, EventArgs e)
        {
            // Clear all the color
            foreach (Button btn in buttonGroup)
            {
                btn.BackColor = Color.Transparent;
            }

            // Highlight the label
            if (listBoxData.SelectedItems.Count > 0)
            {
                string[] item = listBoxData.SelectedItems[listBoxData.SelectedItems.Count - 1].ToString().Split(' ');
                string labelindexstr = item[item.Length - 1];
                if (!string.IsNullOrWhiteSpace(labelindexstr))
                {
                    int labelindexint = Convert.ToInt32(labelindexstr);
                    buttonGroup[labelindexint].BackColor = Color.HotPink;
                    labelDisp.Text = m_LABELNAMES[labelindexint];
                }
            }
        }

        private void checkBox_auto_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_IS_AUTO)
            {
                m_INPUT_DATA_NUM = shape_data[0] * shape_data[1];
                // Load model
                // Selete a pb model file
                string pbFile = string.Empty;
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "..", "..", "train")),
                    Title = "Please select pb model file. ",
                    Filter = "pb|*.pb"
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pbFile = openFileDialog.FileName;
                }
                else
                {
                    this.checkBox_auto.CheckedChanged -= new System.EventHandler(this.checkBox_auto_CheckedChanged);
                    checkBox_auto.Checked = false;
                    this.checkBox_auto.CheckedChanged += new System.EventHandler(this.checkBox_auto_CheckedChanged);
                    return;
                }
                bool isLoad = LoadPbModel(pbFile);
                if (isLoad)
                {
                    string msg = string.Format("\r\nSuccessfully load the pb model:\r\n {0} \r\n", pbFile);
                    textBox_Info.Text += msg;
                }
                else
                {
                    MessageBox.Show("An error occurred in loading model.");
                }

                // Set the global flag
                m_IS_AUTO = true;

                // Update the button control format
                checkBox_auto.ForeColor = Color.LimeGreen;
            }
            else
            {
                // Set the global flag
                m_IS_AUTO = false;

                // Update the button control format
                checkBox_auto.ForeColor = SystemColors.ControlText;
            }
        }

        private void checkBox_write_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_IS_WRITE_DATA)
            {
                // Set the global flag
                m_IS_WRITE_DATA = true;

                // Update the button control format
                checkBox_write.ForeColor = Color.LimeGreen;

                // Reset
                m_frameCounter = 0;

                // Set the output file path
                m_fileHandle.SetFolder(string.Format("Output//{0}", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")));
                m_fileHandle.WriteMsg(m_fileHandle.DataFile, "Skeleton data (X, Y, Z) * 25 points. ");
            }
            else
            {
                // Set the global flag
                m_IS_WRITE_DATA = false;

                // Update the button control format
                checkBox_write.ForeColor = SystemColors.ControlText;
            }
        }

        private void button_NextLabel_Click(object sender, EventArgs e)
        {
            if (listBoxData.Items.Count < 1)
            {
                return;
            }

            int search_label_index = comboBox_LabelList.SelectedIndex;
            if (listBoxData.SelectedIndices.Count<1)
            {
                DialogResult dr = MessageBox.Show("A start position is needed. Do you want to search from the beginning?", "GoBack", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Yes)
                {
                    listBoxData.SelectedIndices.Add(0);
                    listBoxData.SelectedIndex = 0;
                }
            }
            int current_index;
            current_index = listBoxData.SelectedIndices.Count == 0?0:listBoxData.SelectedIndices[listBoxData.SelectedIndices.Count - 1] + 1;

            listBoxData.SelectedIndices.Clear();
            for (int i = current_index; i < m_labels.GetUpperBound(0)-1; i++)
            {
                if (m_labels[i, 1]== search_label_index)
                {
                    listBoxData.SelectedIndices.Add(i);
                    if (m_labels[i + 1, 1] != search_label_index)
                    {
                        break;
                    }
                    else
                    {
                        listBoxData.SelectedIndices.Add(i+1);
                    }
                }
            }
        }


        private void button_LastLabel_Click(object sender, EventArgs e)
        {
            if (listBoxData.Items.Count<1)
            {
                return;
            }
            int search_label_index = comboBox_LabelList.SelectedIndex;
            if (listBoxData.SelectedIndices.Count < 1)
            {
                DialogResult dr = MessageBox.Show("A start position is needed. Do you want to search from the end?", "GoBack", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Yes)
                {
                    listBoxData.SelectedIndices.Add(listBoxData.Items.Count-1);
                    listBoxData.SelectedIndex = listBoxData.Items.Count - 1;
                }
            }
            int current_index;
            current_index = listBoxData.SelectedIndices.Count == listBoxData.Items.Count - 1 ? 0 : listBoxData.SelectedIndices[0] - 1;

            listBoxData.SelectedIndices.Clear();
            for (int i = current_index; i > 1; i--)
            {
                if (m_labels[i, 1] == search_label_index)
                {
                    listBoxData.SelectedIndices.Add(i);
                    if (m_labels[i - 1, 1] != search_label_index)
                    {
                        break;
                    }
                    else
                    {
                        listBoxData.SelectedIndices.Add(i - 1);
                    }
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach (var LabelName in m_LABELNAMES)
            {
                comboBox_LabelList.Items.Add(LabelName);
            }
            comboBox_LabelList.SelectedIndex = 0;
        }

    }
}
