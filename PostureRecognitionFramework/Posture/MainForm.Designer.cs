namespace Posture
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panelControl = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageGrab = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.labelFPS = new System.Windows.Forms.Label();
            this.numericUpDownFPS = new System.Windows.Forms.NumericUpDown();
            this.labelWidth = new System.Windows.Forms.Label();
            this.labelHeight = new System.Windows.Forms.Label();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.checkBox_auto = new System.Windows.Forms.CheckBox();
            this.checkBox_write = new System.Windows.Forms.CheckBox();
            this.buttonLoadPb = new System.Windows.Forms.Button();
            this.buttonTestSample = new System.Windows.Forms.Button();
            this.buttonGrab = new System.Windows.Forms.Button();
            this.textBoxSkeletonData = new System.Windows.Forms.TextBox();
            this.statusStripGrab = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelFrameCounter = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelCurTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelAvgTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelMaxTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabPageLabel = new System.Windows.Forms.TabPage();
            this.button_LastLabel = new System.Windows.Forms.Button();
            this.comboBox_LabelList = new System.Windows.Forms.ComboBox();
            this.button_NextLabel = new System.Windows.Forms.Button();
            this.hScrollBar_speed = new System.Windows.Forms.HScrollBar();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.radioBtnGenMann = new System.Windows.Forms.RadioButton();
            this.radioBtnGenAuto = new System.Windows.Forms.RadioButton();
            this.numericUpDownCoverData = new System.Windows.Forms.NumericUpDown();
            this.labelCoverData = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonFallingCnt = new System.Windows.Forms.Button();
            this.buttonSittingCnt = new System.Windows.Forms.Button();
            this.buttonFalling = new System.Windows.Forms.Button();
            this.buttonSitting = new System.Windows.Forms.Button();
            this.buttonAutoSelect = new System.Windows.Forms.Button();
            this.buttonDispVedio = new System.Windows.Forms.Button();
            this.buttonSitDownCnt = new System.Windows.Forms.Button();
            this.buttonStandUpCnt = new System.Windows.Forms.Button();
            this.buttonWalkingCnt = new System.Windows.Forms.Button();
            this.buttonStandingCnt = new System.Windows.Forms.Button();
            this.numericUpDownCombineData = new System.Windows.Forms.NumericUpDown();
            this.labelCombineData = new System.Windows.Forms.Label();
            this.listBoxData = new System.Windows.Forms.ListBox();
            this.statusStripLabel = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelSelectData = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSeparator = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelAllData = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelFolder = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonLoadData = new System.Windows.Forms.Button();
            this.buttonStanding = new System.Windows.Forms.Button();
            this.buttonSitDown = new System.Windows.Forms.Button();
            this.buttonStandUp = new System.Windows.Forms.Button();
            this.buttonWalking = new System.Windows.Forms.Button();
            this.timerGrab = new System.Windows.Forms.Timer(this.components);
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.labelDisp = new System.Windows.Forms.Label();
            this.panelDisp = new System.Windows.Forms.Panel();
            this.textBox_Info = new System.Windows.Forms.TextBox();
            this.panelControl.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageGrab.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            this.statusStripGrab.SuspendLayout();
            this.tabPageLabel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCoverData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCombineData)).BeginInit();
            this.statusStripLabel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelDisp.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl
            // 
            this.panelControl.Controls.Add(this.tabControl);
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControl.Location = new System.Drawing.Point(640, 0);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(368, 480);
            this.panelControl.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageGrab);
            this.tabControl.Controls.Add(this.tabPageLabel);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(368, 480);
            this.tabControl.TabIndex = 8;
            // 
            // tabPageGrab
            // 
            this.tabPageGrab.Controls.Add(this.groupBox1);
            this.tabPageGrab.Controls.Add(this.checkBox_auto);
            this.tabPageGrab.Controls.Add(this.checkBox_write);
            this.tabPageGrab.Controls.Add(this.buttonLoadPb);
            this.tabPageGrab.Controls.Add(this.buttonTestSample);
            this.tabPageGrab.Controls.Add(this.buttonGrab);
            this.tabPageGrab.Controls.Add(this.textBoxSkeletonData);
            this.tabPageGrab.Controls.Add(this.statusStripGrab);
            this.tabPageGrab.Location = new System.Drawing.Point(4, 29);
            this.tabPageGrab.Name = "tabPageGrab";
            this.tabPageGrab.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGrab.Size = new System.Drawing.Size(360, 447);
            this.tabPageGrab.TabIndex = 0;
            this.tabPageGrab.Text = "Grab";
            this.tabPageGrab.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownWidth);
            this.groupBox1.Controls.Add(this.labelFPS);
            this.groupBox1.Controls.Add(this.numericUpDownFPS);
            this.groupBox1.Controls.Add(this.labelWidth);
            this.groupBox1.Controls.Add(this.labelHeight);
            this.groupBox1.Controls.Add(this.numericUpDownHeight);
            this.groupBox1.Location = new System.Drawing.Point(253, 229);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(99, 115);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CamInfo";
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Enabled = false;
            this.numericUpDownWidth.Location = new System.Drawing.Point(44, 55);
            this.numericUpDownWidth.Maximum = new decimal(new int[] {
            1280,
            0,
            0,
            0});
            this.numericUpDownWidth.Minimum = new decimal(new int[] {
            480,
            0,
            0,
            0});
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(48, 26);
            this.numericUpDownWidth.TabIndex = 14;
            this.numericUpDownWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownWidth.Value = new decimal(new int[] {
            640,
            0,
            0,
            0});
            // 
            // labelFPS
            // 
            this.labelFPS.AutoSize = true;
            this.labelFPS.Location = new System.Drawing.Point(6, 26);
            this.labelFPS.Name = "labelFPS";
            this.labelFPS.Size = new System.Drawing.Size(46, 20);
            this.labelFPS.TabIndex = 10;
            this.labelFPS.Text = "FPS:";
            this.labelFPS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDownFPS
            // 
            this.numericUpDownFPS.Location = new System.Drawing.Point(44, 24);
            this.numericUpDownFPS.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownFPS.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownFPS.Name = "numericUpDownFPS";
            this.numericUpDownFPS.Size = new System.Drawing.Size(48, 26);
            this.numericUpDownFPS.TabIndex = 11;
            this.numericUpDownFPS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownFPS.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownFPS.ValueChanged += new System.EventHandler(this.numericUpDownFPS_ValueChanged);
            // 
            // labelWidth
            // 
            this.labelWidth.AutoSize = true;
            this.labelWidth.Location = new System.Drawing.Point(19, 57);
            this.labelWidth.Name = "labelWidth";
            this.labelWidth.Size = new System.Drawing.Size(30, 20);
            this.labelWidth.TabIndex = 12;
            this.labelWidth.Text = "W:";
            this.labelWidth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeight
            // 
            this.labelHeight.AutoSize = true;
            this.labelHeight.Location = new System.Drawing.Point(22, 88);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(27, 20);
            this.labelHeight.TabIndex = 13;
            this.labelHeight.Text = "H:";
            this.labelHeight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.Enabled = false;
            this.numericUpDownHeight.Location = new System.Drawing.Point(44, 86);
            this.numericUpDownHeight.Maximum = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.numericUpDownHeight.Minimum = new decimal(new int[] {
            270,
            0,
            0,
            0});
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Size = new System.Drawing.Size(48, 26);
            this.numericUpDownHeight.TabIndex = 15;
            this.numericUpDownHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownHeight.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
            // 
            // checkBox_auto
            // 
            this.checkBox_auto.AutoSize = true;
            this.checkBox_auto.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.checkBox_auto.Location = new System.Drawing.Point(257, 45);
            this.checkBox_auto.Name = "checkBox_auto";
            this.checkBox_auto.Size = new System.Drawing.Size(79, 30);
            this.checkBox_auto.TabIndex = 20;
            this.checkBox_auto.Text = "Auto";
            this.checkBox_auto.UseVisualStyleBackColor = true;
            this.checkBox_auto.CheckedChanged += new System.EventHandler(this.checkBox_auto_CheckedChanged);
            // 
            // checkBox_write
            // 
            this.checkBox_write.AutoSize = true;
            this.checkBox_write.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.checkBox_write.Location = new System.Drawing.Point(257, 77);
            this.checkBox_write.Name = "checkBox_write";
            this.checkBox_write.Size = new System.Drawing.Size(85, 30);
            this.checkBox_write.TabIndex = 19;
            this.checkBox_write.Text = "Write";
            this.checkBox_write.UseVisualStyleBackColor = true;
            this.checkBox_write.CheckedChanged += new System.EventHandler(this.checkBox_write_CheckedChanged);
            // 
            // buttonLoadPb
            // 
            this.buttonLoadPb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonLoadPb.Location = new System.Drawing.Point(253, 350);
            this.buttonLoadPb.Name = "buttonLoadPb";
            this.buttonLoadPb.Size = new System.Drawing.Size(100, 32);
            this.buttonLoadPb.TabIndex = 18;
            this.buttonLoadPb.Tag = "0";
            this.buttonLoadPb.Text = "Load";
            this.buttonLoadPb.UseVisualStyleBackColor = true;
            this.buttonLoadPb.Click += new System.EventHandler(this.buttonLoadPb_Click);
            // 
            // buttonTestSample
            // 
            this.buttonTestSample.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonTestSample.Location = new System.Drawing.Point(253, 388);
            this.buttonTestSample.Name = "buttonTestSample";
            this.buttonTestSample.Size = new System.Drawing.Size(100, 32);
            this.buttonTestSample.TabIndex = 17;
            this.buttonTestSample.Tag = "0";
            this.buttonTestSample.Text = "Test";
            this.buttonTestSample.UseVisualStyleBackColor = true;
            this.buttonTestSample.Click += new System.EventHandler(this.buttonTestSample_Click);
            // 
            // buttonGrab
            // 
            this.buttonGrab.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonGrab.Location = new System.Drawing.Point(254, 6);
            this.buttonGrab.Name = "buttonGrab";
            this.buttonGrab.Size = new System.Drawing.Size(100, 32);
            this.buttonGrab.TabIndex = 6;
            this.buttonGrab.Tag = "0";
            this.buttonGrab.Text = " Grab ▶";
            this.buttonGrab.UseVisualStyleBackColor = true;
            this.buttonGrab.Click += new System.EventHandler(this.buttonGrab_Click);
            // 
            // textBoxSkeletonData
            // 
            this.textBoxSkeletonData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.textBoxSkeletonData.Location = new System.Drawing.Point(6, 6);
            this.textBoxSkeletonData.Multiline = true;
            this.textBoxSkeletonData.Name = "textBoxSkeletonData";
            this.textBoxSkeletonData.Size = new System.Drawing.Size(241, 414);
            this.textBoxSkeletonData.TabIndex = 7;
            this.textBoxSkeletonData.Text = "          X           Y           Z   ";
            this.textBoxSkeletonData.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // statusStripGrab
            // 
            this.statusStripGrab.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripGrab.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelFrameCounter,
            this.toolStripStatusLabelCurTime,
            this.toolStripStatusLabelAvgTime,
            this.toolStripStatusLabelMaxTime});
            this.statusStripGrab.Location = new System.Drawing.Point(3, 419);
            this.statusStripGrab.Name = "statusStripGrab";
            this.statusStripGrab.Size = new System.Drawing.Size(354, 25);
            this.statusStripGrab.TabIndex = 5;
            this.statusStripGrab.Text = "statusStripTime";
            // 
            // toolStripStatusLabelFrameCounter
            // 
            this.toolStripStatusLabelFrameCounter.Name = "toolStripStatusLabelFrameCounter";
            this.toolStripStatusLabelFrameCounter.Size = new System.Drawing.Size(54, 20);
            this.toolStripStatusLabelFrameCounter.Text = "00000";
            // 
            // toolStripStatusLabelCurTime
            // 
            this.toolStripStatusLabelCurTime.Name = "toolStripStatusLabelCurTime";
            this.toolStripStatusLabelCurTime.Size = new System.Drawing.Size(107, 20);
            this.toolStripStatusLabelCurTime.Text = "Cur: 00.00 ms";
            // 
            // toolStripStatusLabelAvgTime
            // 
            this.toolStripStatusLabelAvgTime.Name = "toolStripStatusLabelAvgTime";
            this.toolStripStatusLabelAvgTime.Size = new System.Drawing.Size(111, 20);
            this.toolStripStatusLabelAvgTime.Text = "Avg: 00.00 ms";
            // 
            // toolStripStatusLabelMaxTime
            // 
            this.toolStripStatusLabelMaxTime.Name = "toolStripStatusLabelMaxTime";
            this.toolStripStatusLabelMaxTime.Size = new System.Drawing.Size(113, 20);
            this.toolStripStatusLabelMaxTime.Tag = "";
            this.toolStripStatusLabelMaxTime.Text = "Max: 00.00 ms";
            // 
            // tabPageLabel
            // 
            this.tabPageLabel.Controls.Add(this.button_LastLabel);
            this.tabPageLabel.Controls.Add(this.comboBox_LabelList);
            this.tabPageLabel.Controls.Add(this.button_NextLabel);
            this.tabPageLabel.Controls.Add(this.hScrollBar_speed);
            this.tabPageLabel.Controls.Add(this.buttonGenerate);
            this.tabPageLabel.Controls.Add(this.radioBtnGenMann);
            this.tabPageLabel.Controls.Add(this.radioBtnGenAuto);
            this.tabPageLabel.Controls.Add(this.numericUpDownCoverData);
            this.tabPageLabel.Controls.Add(this.labelCoverData);
            this.tabPageLabel.Controls.Add(this.buttonDelete);
            this.tabPageLabel.Controls.Add(this.buttonFallingCnt);
            this.tabPageLabel.Controls.Add(this.buttonSittingCnt);
            this.tabPageLabel.Controls.Add(this.buttonFalling);
            this.tabPageLabel.Controls.Add(this.buttonSitting);
            this.tabPageLabel.Controls.Add(this.buttonAutoSelect);
            this.tabPageLabel.Controls.Add(this.buttonDispVedio);
            this.tabPageLabel.Controls.Add(this.buttonSitDownCnt);
            this.tabPageLabel.Controls.Add(this.buttonStandUpCnt);
            this.tabPageLabel.Controls.Add(this.buttonWalkingCnt);
            this.tabPageLabel.Controls.Add(this.buttonStandingCnt);
            this.tabPageLabel.Controls.Add(this.numericUpDownCombineData);
            this.tabPageLabel.Controls.Add(this.labelCombineData);
            this.tabPageLabel.Controls.Add(this.listBoxData);
            this.tabPageLabel.Controls.Add(this.statusStripLabel);
            this.tabPageLabel.Controls.Add(this.buttonLoadData);
            this.tabPageLabel.Controls.Add(this.buttonStanding);
            this.tabPageLabel.Controls.Add(this.buttonSitDown);
            this.tabPageLabel.Controls.Add(this.buttonStandUp);
            this.tabPageLabel.Controls.Add(this.buttonWalking);
            this.tabPageLabel.Location = new System.Drawing.Point(4, 29);
            this.tabPageLabel.Name = "tabPageLabel";
            this.tabPageLabel.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLabel.Size = new System.Drawing.Size(360, 447);
            this.tabPageLabel.TabIndex = 1;
            this.tabPageLabel.Text = "Label";
            this.tabPageLabel.UseVisualStyleBackColor = true;
            // 
            // button_LastLabel
            // 
            this.button_LastLabel.Location = new System.Drawing.Point(239, 172);
            this.button_LastLabel.Name = "button_LastLabel";
            this.button_LastLabel.Size = new System.Drawing.Size(48, 24);
            this.button_LastLabel.TabIndex = 57;
            this.button_LastLabel.Text = "◀";
            this.button_LastLabel.UseVisualStyleBackColor = true;
            this.button_LastLabel.Click += new System.EventHandler(this.button_LastLabel_Click);
            // 
            // comboBox_LabelList
            // 
            this.comboBox_LabelList.FormattingEnabled = true;
            this.comboBox_LabelList.Location = new System.Drawing.Point(133, 172);
            this.comboBox_LabelList.Name = "comboBox_LabelList";
            this.comboBox_LabelList.Size = new System.Drawing.Size(100, 28);
            this.comboBox_LabelList.TabIndex = 56;
            // 
            // button_NextLabel
            // 
            this.button_NextLabel.Location = new System.Drawing.Point(293, 172);
            this.button_NextLabel.Name = "button_NextLabel";
            this.button_NextLabel.Size = new System.Drawing.Size(48, 24);
            this.button_NextLabel.TabIndex = 55;
            this.button_NextLabel.Text = "▶";
            this.button_NextLabel.UseVisualStyleBackColor = true;
            this.button_NextLabel.Click += new System.EventHandler(this.button_NextLabel_Click);
            // 
            // hScrollBar_speed
            // 
            this.hScrollBar_speed.Location = new System.Drawing.Point(239, 82);
            this.hScrollBar_speed.Maximum = 50;
            this.hScrollBar_speed.Name = "hScrollBar_speed";
            this.hScrollBar_speed.Size = new System.Drawing.Size(102, 15);
            this.hScrollBar_speed.TabIndex = 54;
            this.hScrollBar_speed.Value = 20;
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.BackColor = System.Drawing.Color.Transparent;
            this.buttonGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.buttonGenerate.Location = new System.Drawing.Point(133, 113);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(85, 32);
            this.buttonGenerate.TabIndex = 53;
            this.buttonGenerate.Text = "Generate";
            this.buttonGenerate.UseVisualStyleBackColor = false;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // radioBtnGenMann
            // 
            this.radioBtnGenMann.Checked = true;
            this.radioBtnGenMann.Location = new System.Drawing.Point(286, 119);
            this.radioBtnGenMann.Name = "radioBtnGenMann";
            this.radioBtnGenMann.Size = new System.Drawing.Size(61, 21);
            this.radioBtnGenMann.TabIndex = 52;
            this.radioBtnGenMann.TabStop = true;
            this.radioBtnGenMann.Text = "Mann";
            this.radioBtnGenMann.UseVisualStyleBackColor = true;
            // 
            // radioBtnGenAuto
            // 
            this.radioBtnGenAuto.Location = new System.Drawing.Point(235, 119);
            this.radioBtnGenAuto.Name = "radioBtnGenAuto";
            this.radioBtnGenAuto.Size = new System.Drawing.Size(55, 21);
            this.radioBtnGenAuto.TabIndex = 51;
            this.radioBtnGenAuto.Text = "Auto";
            this.radioBtnGenAuto.UseVisualStyleBackColor = true;
            this.radioBtnGenAuto.CheckedChanged += new System.EventHandler(this.radioBtnGenAuto_CheckedChanged);
            // 
            // numericUpDownCoverData
            // 
            this.numericUpDownCoverData.Location = new System.Drawing.Point(293, 46);
            this.numericUpDownCoverData.Name = "numericUpDownCoverData";
            this.numericUpDownCoverData.Size = new System.Drawing.Size(48, 26);
            this.numericUpDownCoverData.TabIndex = 33;
            this.numericUpDownCoverData.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownCoverData.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // labelCoverData
            // 
            this.labelCoverData.AutoSize = true;
            this.labelCoverData.Location = new System.Drawing.Point(245, 47);
            this.labelCoverData.Name = "labelCoverData";
            this.labelCoverData.Size = new System.Drawing.Size(60, 20);
            this.labelCoverData.TabIndex = 48;
            this.labelCoverData.Text = "cover: ";
            this.labelCoverData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonDelete
            // 
            this.buttonDelete.BackColor = System.Drawing.Color.OrangeRed;
            this.buttonDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonDelete.Location = new System.Drawing.Point(293, 6);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(50, 32);
            this.buttonDelete.TabIndex = 45;
            this.buttonDelete.Text = "Del";
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonFallingCnt
            // 
            this.buttonFallingCnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonFallingCnt.Location = new System.Drawing.Point(293, 381);
            this.buttonFallingCnt.Name = "buttonFallingCnt";
            this.buttonFallingCnt.Size = new System.Drawing.Size(48, 32);
            this.buttonFallingCnt.TabIndex = 37;
            this.buttonFallingCnt.Text = "0";
            this.buttonFallingCnt.UseVisualStyleBackColor = true;
            this.buttonFallingCnt.Click += new System.EventHandler(this.buttonOthersCnt_Click);
            // 
            // buttonSittingCnt
            // 
            this.buttonSittingCnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonSittingCnt.Location = new System.Drawing.Point(293, 241);
            this.buttonSittingCnt.Name = "buttonSittingCnt";
            this.buttonSittingCnt.Size = new System.Drawing.Size(48, 32);
            this.buttonSittingCnt.TabIndex = 36;
            this.buttonSittingCnt.Text = "0";
            this.buttonSittingCnt.UseVisualStyleBackColor = true;
            this.buttonSittingCnt.Click += new System.EventHandler(this.buttonSittingCnt_Click);
            // 
            // buttonFalling
            // 
            this.buttonFalling.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonFalling.Location = new System.Drawing.Point(133, 381);
            this.buttonFalling.Name = "buttonFalling";
            this.buttonFalling.Size = new System.Drawing.Size(154, 32);
            this.buttonFalling.TabIndex = 35;
            this.buttonFalling.Text = "Falling";
            this.buttonFalling.UseVisualStyleBackColor = true;
            this.buttonFalling.Click += new System.EventHandler(this.buttonFalling_Click);
            // 
            // buttonSitting
            // 
            this.buttonSitting.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonSitting.Location = new System.Drawing.Point(133, 241);
            this.buttonSitting.Name = "buttonSitting";
            this.buttonSitting.Size = new System.Drawing.Size(154, 32);
            this.buttonSitting.TabIndex = 34;
            this.buttonSitting.Text = "Sitting";
            this.buttonSitting.UseVisualStyleBackColor = true;
            this.buttonSitting.Click += new System.EventHandler(this.buttonSitting_Click);
            // 
            // buttonAutoSelect
            // 
            this.buttonAutoSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonAutoSelect.Location = new System.Drawing.Point(133, 73);
            this.buttonAutoSelect.Name = "buttonAutoSelect";
            this.buttonAutoSelect.Size = new System.Drawing.Size(68, 32);
            this.buttonAutoSelect.TabIndex = 31;
            this.buttonAutoSelect.Text = "Select";
            this.buttonAutoSelect.UseVisualStyleBackColor = true;
            this.buttonAutoSelect.Click += new System.EventHandler(this.buttonAutoSelect_Click);
            // 
            // buttonDispVedio
            // 
            this.buttonDispVedio.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonDispVedio.Location = new System.Drawing.Point(203, 73);
            this.buttonDispVedio.Name = "buttonDispVedio";
            this.buttonDispVedio.Size = new System.Drawing.Size(30, 32);
            this.buttonDispVedio.TabIndex = 29;
            this.buttonDispVedio.Text = "▶";
            this.buttonDispVedio.UseVisualStyleBackColor = true;
            this.buttonDispVedio.Click += new System.EventHandler(this.buttonDispVedio_Click);
            // 
            // buttonSitDownCnt
            // 
            this.buttonSitDownCnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonSitDownCnt.Location = new System.Drawing.Point(293, 346);
            this.buttonSitDownCnt.Name = "buttonSitDownCnt";
            this.buttonSitDownCnt.Size = new System.Drawing.Size(48, 32);
            this.buttonSitDownCnt.TabIndex = 27;
            this.buttonSitDownCnt.Text = "0";
            this.buttonSitDownCnt.UseVisualStyleBackColor = true;
            this.buttonSitDownCnt.Click += new System.EventHandler(this.buttonSitDownCnt_Click);
            // 
            // buttonStandUpCnt
            // 
            this.buttonStandUpCnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonStandUpCnt.Location = new System.Drawing.Point(293, 311);
            this.buttonStandUpCnt.Name = "buttonStandUpCnt";
            this.buttonStandUpCnt.Size = new System.Drawing.Size(48, 32);
            this.buttonStandUpCnt.TabIndex = 26;
            this.buttonStandUpCnt.Text = "0";
            this.buttonStandUpCnt.UseVisualStyleBackColor = true;
            this.buttonStandUpCnt.Click += new System.EventHandler(this.buttonStandUpCnt_Click);
            // 
            // buttonWalkingCnt
            // 
            this.buttonWalkingCnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonWalkingCnt.Location = new System.Drawing.Point(293, 276);
            this.buttonWalkingCnt.Name = "buttonWalkingCnt";
            this.buttonWalkingCnt.Size = new System.Drawing.Size(48, 32);
            this.buttonWalkingCnt.TabIndex = 25;
            this.buttonWalkingCnt.Text = "0";
            this.buttonWalkingCnt.UseVisualStyleBackColor = true;
            this.buttonWalkingCnt.Click += new System.EventHandler(this.buttonWalkingCnt_Click);
            // 
            // buttonStandingCnt
            // 
            this.buttonStandingCnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonStandingCnt.Location = new System.Drawing.Point(293, 206);
            this.buttonStandingCnt.Name = "buttonStandingCnt";
            this.buttonStandingCnt.Size = new System.Drawing.Size(48, 32);
            this.buttonStandingCnt.TabIndex = 24;
            this.buttonStandingCnt.Text = "0";
            this.buttonStandingCnt.UseVisualStyleBackColor = true;
            this.buttonStandingCnt.Click += new System.EventHandler(this.buttonStandingCnt_Click);
            // 
            // numericUpDownCombineData
            // 
            this.numericUpDownCombineData.Location = new System.Drawing.Point(178, 46);
            this.numericUpDownCombineData.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownCombineData.Name = "numericUpDownCombineData";
            this.numericUpDownCombineData.Size = new System.Drawing.Size(55, 26);
            this.numericUpDownCombineData.TabIndex = 15;
            this.numericUpDownCombineData.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownCombineData.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // labelCombineData
            // 
            this.labelCombineData.AutoSize = true;
            this.labelCombineData.Location = new System.Drawing.Point(132, 47);
            this.labelCombineData.Name = "labelCombineData";
            this.labelCombineData.Size = new System.Drawing.Size(60, 20);
            this.labelCombineData.TabIndex = 14;
            this.labelCombineData.Text = "steps: ";
            this.labelCombineData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listBoxData
            // 
            this.listBoxData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.listBoxData.FormattingEnabled = true;
            this.listBoxData.ItemHeight = 16;
            this.listBoxData.Location = new System.Drawing.Point(6, 6);
            this.listBoxData.Name = "listBoxData";
            this.listBoxData.ScrollAlwaysVisible = true;
            this.listBoxData.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxData.Size = new System.Drawing.Size(107, 404);
            this.listBoxData.TabIndex = 13;
            this.listBoxData.Click += new System.EventHandler(this.listBoxData_Click);
            this.listBoxData.SelectedIndexChanged += new System.EventHandler(this.listBoxData_SelectedIndexChanged);
            // 
            // statusStripLabel
            // 
            this.statusStripLabel.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripLabel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelSelectData,
            this.toolStripStatusLabelSeparator,
            this.toolStripStatusLabelAllData,
            this.toolStripStatusLabelFolder});
            this.statusStripLabel.Location = new System.Drawing.Point(3, 419);
            this.statusStripLabel.Name = "statusStripLabel";
            this.statusStripLabel.Size = new System.Drawing.Size(354, 25);
            this.statusStripLabel.TabIndex = 12;
            this.statusStripLabel.Text = "statusStrip1";
            // 
            // toolStripStatusLabelSelectData
            // 
            this.toolStripStatusLabelSelectData.Name = "toolStripStatusLabelSelectData";
            this.toolStripStatusLabelSelectData.Size = new System.Drawing.Size(27, 20);
            this.toolStripStatusLabelSelectData.Text = "00";
            // 
            // toolStripStatusLabelSeparator
            // 
            this.toolStripStatusLabelSeparator.Name = "toolStripStatusLabelSeparator";
            this.toolStripStatusLabelSeparator.Size = new System.Drawing.Size(15, 20);
            this.toolStripStatusLabelSeparator.Text = "/";
            // 
            // toolStripStatusLabelAllData
            // 
            this.toolStripStatusLabelAllData.Name = "toolStripStatusLabelAllData";
            this.toolStripStatusLabelAllData.Size = new System.Drawing.Size(54, 20);
            this.toolStripStatusLabelAllData.Text = "00000";
            // 
            // toolStripStatusLabelFolder
            // 
            this.toolStripStatusLabelFolder.Name = "toolStripStatusLabelFolder";
            this.toolStripStatusLabelFolder.Size = new System.Drawing.Size(46, 20);
            this.toolStripStatusLabelFolder.Text = "URL: ";
            // 
            // buttonLoadData
            // 
            this.buttonLoadData.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonLoadData.Location = new System.Drawing.Point(133, 6);
            this.buttonLoadData.Name = "buttonLoadData";
            this.buttonLoadData.Size = new System.Drawing.Size(154, 32);
            this.buttonLoadData.TabIndex = 11;
            this.buttonLoadData.Text = "Load Data";
            this.buttonLoadData.UseVisualStyleBackColor = true;
            this.buttonLoadData.Click += new System.EventHandler(this.buttonLoadData_Click);
            // 
            // buttonStanding
            // 
            this.buttonStanding.BackColor = System.Drawing.Color.Transparent;
            this.buttonStanding.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonStanding.Location = new System.Drawing.Point(133, 206);
            this.buttonStanding.Name = "buttonStanding";
            this.buttonStanding.Size = new System.Drawing.Size(154, 32);
            this.buttonStanding.TabIndex = 9;
            this.buttonStanding.Text = "Standing";
            this.buttonStanding.UseVisualStyleBackColor = false;
            this.buttonStanding.Click += new System.EventHandler(this.buttonStanding_Click);
            // 
            // buttonSitDown
            // 
            this.buttonSitDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonSitDown.Location = new System.Drawing.Point(133, 346);
            this.buttonSitDown.Name = "buttonSitDown";
            this.buttonSitDown.Size = new System.Drawing.Size(154, 32);
            this.buttonSitDown.TabIndex = 7;
            this.buttonSitDown.Text = "Sit Down";
            this.buttonSitDown.UseVisualStyleBackColor = true;
            this.buttonSitDown.Click += new System.EventHandler(this.buttonSitDown_Click);
            // 
            // buttonStandUp
            // 
            this.buttonStandUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonStandUp.Location = new System.Drawing.Point(133, 311);
            this.buttonStandUp.Name = "buttonStandUp";
            this.buttonStandUp.Size = new System.Drawing.Size(154, 32);
            this.buttonStandUp.TabIndex = 6;
            this.buttonStandUp.Text = "Stand Up";
            this.buttonStandUp.UseVisualStyleBackColor = true;
            this.buttonStandUp.Click += new System.EventHandler(this.buttonStandUp_Click);
            // 
            // buttonWalking
            // 
            this.buttonWalking.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonWalking.Location = new System.Drawing.Point(133, 276);
            this.buttonWalking.Name = "buttonWalking";
            this.buttonWalking.Size = new System.Drawing.Size(154, 32);
            this.buttonWalking.TabIndex = 5;
            this.buttonWalking.Text = "Walking";
            this.buttonWalking.UseVisualStyleBackColor = true;
            this.buttonWalking.Click += new System.EventHandler(this.buttonWalking_Click);
            // 
            // timerGrab
            // 
            this.timerGrab.Interval = 32;
            this.timerGrab.Tick += new System.EventHandler(this.timerGrab_Tick);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(640, 480);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // labelDisp
            // 
            this.labelDisp.Font = new System.Drawing.Font("Microsoft Sans Serif", 64F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelDisp.Location = new System.Drawing.Point(67, 0);
            this.labelDisp.Name = "labelDisp";
            this.labelDisp.Size = new System.Drawing.Size(500, 138);
            this.labelDisp.TabIndex = 2;
            this.labelDisp.Text = "StandUp";
            this.labelDisp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelDisp
            // 
            this.panelDisp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDisp.Controls.Add(this.textBox_Info);
            this.panelDisp.Controls.Add(this.labelDisp);
            this.panelDisp.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDisp.Location = new System.Drawing.Point(0, 480);
            this.panelDisp.Name = "panelDisp";
            this.panelDisp.Size = new System.Drawing.Size(1008, 139);
            this.panelDisp.TabIndex = 3;
            // 
            // textBox_Info
            // 
            this.textBox_Info.Dock = System.Windows.Forms.DockStyle.Right;
            this.textBox_Info.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_Info.Location = new System.Drawing.Point(639, 0);
            this.textBox_Info.Multiline = true;
            this.textBox_Info.Name = "textBox_Info";
            this.textBox_Info.ReadOnly = true;
            this.textBox_Info.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Info.Size = new System.Drawing.Size(367, 137);
            this.textBox_Info.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 619);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.panelControl);
            this.Controls.Add(this.panelDisp);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "Posture Recognition";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelControl.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageGrab.ResumeLayout(false);
            this.tabPageGrab.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            this.statusStripGrab.ResumeLayout(false);
            this.statusStripGrab.PerformLayout();
            this.tabPageLabel.ResumeLayout(false);
            this.tabPageLabel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCoverData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCombineData)).EndInit();
            this.statusStripLabel.ResumeLayout(false);
            this.statusStripLabel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panelDisp.ResumeLayout(false);
            this.panelDisp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.Timer timerGrab;
        private System.Windows.Forms.StatusStrip statusStripGrab;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelAvgTime;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMaxTime;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelCurTime;
        private System.Windows.Forms.Button buttonGrab;
        private System.Windows.Forms.TextBox textBoxSkeletonData;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageLabel;
        private System.Windows.Forms.Button buttonStanding;
        private System.Windows.Forms.Button buttonSitDown;
        private System.Windows.Forms.Button buttonStandUp;
        private System.Windows.Forms.Button buttonWalking;
        private System.Windows.Forms.TabPage tabPageGrab;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFrameCounter;
        private System.Windows.Forms.Button buttonLoadData;
        private System.Windows.Forms.StatusStrip statusStripLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelAllData;
        private System.Windows.Forms.ListBox listBoxData;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSelectData;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSeparator;
        private System.Windows.Forms.Label labelCombineData;
        private System.Windows.Forms.NumericUpDown numericUpDownCombineData;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button buttonStandingCnt;
        private System.Windows.Forms.Button buttonSitDownCnt;
        private System.Windows.Forms.Button buttonStandUpCnt;
        private System.Windows.Forms.Button buttonWalkingCnt;
        private System.Windows.Forms.NumericUpDown numericUpDownHeight;
        private System.Windows.Forms.NumericUpDown numericUpDownWidth;
        private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.Label labelWidth;
        private System.Windows.Forms.NumericUpDown numericUpDownFPS;
        private System.Windows.Forms.Label labelFPS;
        private System.Windows.Forms.Button buttonDispVedio;
        private System.Windows.Forms.NumericUpDown numericUpDownCoverData;
        private System.Windows.Forms.Button buttonFallingCnt;
        private System.Windows.Forms.Button buttonSittingCnt;
        private System.Windows.Forms.Button buttonFalling;
        private System.Windows.Forms.Button buttonSitting;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFolder;
        private System.Windows.Forms.Button buttonAutoSelect;
        private System.Windows.Forms.Button buttonTestSample;
        private System.Windows.Forms.Button buttonLoadPb;
        private System.Windows.Forms.Label labelDisp;
        private System.Windows.Forms.Panel panelDisp;
        private System.Windows.Forms.TextBox textBox_Info;
        private System.Windows.Forms.Label labelCoverData;
        private System.Windows.Forms.RadioButton radioBtnGenMann;
        private System.Windows.Forms.RadioButton radioBtnGenAuto;
        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.HScrollBar hScrollBar_speed;
        private System.Windows.Forms.CheckBox checkBox_auto;
        private System.Windows.Forms.CheckBox checkBox_write;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_NextLabel;
        private System.Windows.Forms.ComboBox comboBox_LabelList;
        private System.Windows.Forms.Button button_LastLabel;
    }
}

