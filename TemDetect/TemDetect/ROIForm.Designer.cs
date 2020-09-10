using TempDetect;

namespace TempDetect
{
    partial class ROIFFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ROIFFrm));
            this.ttISPAddForm = new System.Windows.Forms.ToolTip(this.components);
            this.cbxMarkerName = new System.Windows.Forms.ComboBox();
            this.cbxwLocate = new System.Windows.Forms.ComboBox();
            this.tbAlarm = new System.Windows.Forms.TextBox();
            this.cbAlarm = new System.Windows.Forms.CheckBox();
            this.cbAPI = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Negativerdbtn = new System.Windows.Forms.RadioButton();
            this.Positiverdbtn = new System.Windows.Forms.RadioButton();
            this.cbOSD = new System.Windows.Forms.CheckBox();
            this.lblAlarm = new System.Windows.Forms.Label();
            this.lblAPI = new System.Windows.Forms.Label();
            this.lblOSD = new System.Windows.Forms.Label();
            this.nudEmissivityValue = new System.Windows.Forms.NumericUpDown();
            this.lblOffsetRange = new System.Windows.Forms.Label();
            this.lblDistanceRange = new System.Windows.Forms.Label();
            this.nudReflectedTemperatureValue = new System.Windows.Forms.NumericUpDown();
            this.lblReflectedTemperatureRange = new System.Windows.Forms.Label();
            this.nudOffsetValue = new System.Windows.Forms.NumericUpDown();
            this.nudDistanceValue = new System.Windows.Forms.NumericUpDown();
            this.lblEmissivityRange = new System.Windows.Forms.Label();
            this.lblEmissivity = new System.Windows.Forms.Label();
            this.lblOffset = new System.Windows.Forms.Label();
            this.lblDistance = new System.Windows.Forms.Label();
            this.lblReflectedTemperature = new System.Windows.Forms.Label();
            this.lblMarkerName = new System.Windows.Forms.Label();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnSaveROI = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbArrow = new System.Windows.Forms.ToolStripButton();
            this.tsbLine = new System.Windows.Forms.ToolStripButton();
            this.tsbPoint = new System.Windows.Forms.ToolStripButton();
            this.tsbPolygon = new System.Windows.Forms.ToolStripButton();
            this.tsmiDel = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsISP = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tpbTA = new TempDetect.OWBPictureBox(this.components);
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmissivityValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReflectedTemperatureValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOffsetValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDistanceValue)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.cmsISP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tpbTA)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxMarkerName
            // 
            this.cbxMarkerName.FormattingEnabled = true;
            this.cbxMarkerName.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30"});
            this.cbxMarkerName.Location = new System.Drawing.Point(262, 126);
            this.cbxMarkerName.Name = "cbxMarkerName";
            this.cbxMarkerName.Size = new System.Drawing.Size(121, 23);
            this.cbxMarkerName.TabIndex = 196;
            this.cbxMarkerName.SelectedIndexChanged += new System.EventHandler(this.cbxMarkerName_SelectedIndexChanged);
            // 
            // cbxwLocate
            // 
            this.cbxwLocate.FormattingEnabled = true;
            this.cbxwLocate.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40"});
            this.cbxwLocate.Location = new System.Drawing.Point(262, 9);
            this.cbxwLocate.Name = "cbxwLocate";
            this.cbxwLocate.Size = new System.Drawing.Size(121, 23);
            this.cbxwLocate.TabIndex = 195;
            this.cbxwLocate.SelectedIndexChanged += new System.EventHandler(this.cbxwLocate_SelectedIndexChanged);
            // 
            // tbAlarm
            // 
            this.tbAlarm.Enabled = false;
            this.tbAlarm.Location = new System.Drawing.Point(223, 433);
            this.tbAlarm.Margin = new System.Windows.Forms.Padding(4);
            this.tbAlarm.Name = "tbAlarm";
            this.tbAlarm.Size = new System.Drawing.Size(160, 25);
            this.tbAlarm.TabIndex = 191;
            this.tbAlarm.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbAlarm_KeyDown);
            // 
            // cbAlarm
            // 
            this.cbAlarm.AutoSize = true;
            this.cbAlarm.Location = new System.Drawing.Point(31, 437);
            this.cbAlarm.Margin = new System.Windows.Forms.Padding(4);
            this.cbAlarm.Name = "cbAlarm";
            this.cbAlarm.Size = new System.Drawing.Size(18, 17);
            this.cbAlarm.TabIndex = 190;
            this.cbAlarm.UseVisualStyleBackColor = true;
            this.cbAlarm.CheckedChanged += new System.EventHandler(this.cbAlarm_CheckedChanged);
            // 
            // cbAPI
            // 
            this.cbAPI.AutoSize = true;
            this.cbAPI.Checked = true;
            this.cbAPI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAPI.Location = new System.Drawing.Point(31, 402);
            this.cbAPI.Margin = new System.Windows.Forms.Padding(4);
            this.cbAPI.Name = "cbAPI";
            this.cbAPI.Size = new System.Drawing.Size(18, 17);
            this.cbAPI.TabIndex = 189;
            this.cbAPI.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.cbxMarkerName);
            this.panel2.Controls.Add(this.cbxwLocate);
            this.panel2.Controls.Add(this.tbAlarm);
            this.panel2.Controls.Add(this.cbAlarm);
            this.panel2.Controls.Add(this.cbAPI);
            this.panel2.Controls.Add(this.cbOSD);
            this.panel2.Controls.Add(this.lblAlarm);
            this.panel2.Controls.Add(this.lblAPI);
            this.panel2.Controls.Add(this.lblOSD);
            this.panel2.Controls.Add(this.nudEmissivityValue);
            this.panel2.Controls.Add(this.lblOffsetRange);
            this.panel2.Controls.Add(this.lblDistanceRange);
            this.panel2.Controls.Add(this.nudReflectedTemperatureValue);
            this.panel2.Controls.Add(this.lblReflectedTemperatureRange);
            this.panel2.Controls.Add(this.nudOffsetValue);
            this.panel2.Controls.Add(this.nudDistanceValue);
            this.panel2.Controls.Add(this.lblEmissivityRange);
            this.panel2.Controls.Add(this.lblEmissivity);
            this.panel2.Controls.Add(this.lblOffset);
            this.panel2.Controls.Add(this.lblDistance);
            this.panel2.Controls.Add(this.lblReflectedTemperature);
            this.panel2.Controls.Add(this.lblMarkerName);
            this.panel2.Controls.Add(this.btnCreate);
            this.panel2.Controls.Add(this.btnSaveROI);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(636, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(403, 586);
            this.panel2.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Negativerdbtn);
            this.panel1.Controls.Add(this.Positiverdbtn);
            this.panel1.Location = new System.Drawing.Point(66, 37);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(154, 80);
            this.panel1.TabIndex = 197;
            // 
            // Negativerdbtn
            // 
            this.Negativerdbtn.AutoSize = true;
            this.Negativerdbtn.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Negativerdbtn.Location = new System.Drawing.Point(4, 48);
            this.Negativerdbtn.Name = "Negativerdbtn";
            this.Negativerdbtn.Size = new System.Drawing.Size(50, 23);
            this.Negativerdbtn.TabIndex = 0;
            this.Negativerdbtn.TabStop = true;
            this.Negativerdbtn.Text = "反";
            this.Negativerdbtn.UseVisualStyleBackColor = true;
            // 
            // Positiverdbtn
            // 
            this.Positiverdbtn.AutoSize = true;
            this.Positiverdbtn.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Positiverdbtn.Location = new System.Drawing.Point(4, 13);
            this.Positiverdbtn.Name = "Positiverdbtn";
            this.Positiverdbtn.Size = new System.Drawing.Size(50, 23);
            this.Positiverdbtn.TabIndex = 0;
            this.Positiverdbtn.TabStop = true;
            this.Positiverdbtn.Text = "正";
            this.Positiverdbtn.UseVisualStyleBackColor = true;
            // 
            // cbOSD
            // 
            this.cbOSD.AutoSize = true;
            this.cbOSD.Checked = true;
            this.cbOSD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOSD.Location = new System.Drawing.Point(31, 359);
            this.cbOSD.Margin = new System.Windows.Forms.Padding(4);
            this.cbOSD.Name = "cbOSD";
            this.cbOSD.Size = new System.Drawing.Size(18, 17);
            this.cbOSD.TabIndex = 188;
            this.cbOSD.UseVisualStyleBackColor = true;
            // 
            // lblAlarm
            // 
            this.lblAlarm.AutoSize = true;
            this.lblAlarm.Location = new System.Drawing.Point(63, 439);
            this.lblAlarm.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblAlarm.Name = "lblAlarm";
            this.lblAlarm.Size = new System.Drawing.Size(97, 15);
            this.lblAlarm.TabIndex = 183;
            this.lblAlarm.Text = "温度报警阈值";
            // 
            // lblAPI
            // 
            this.lblAPI.AutoSize = true;
            this.lblAPI.Location = new System.Drawing.Point(63, 404);
            this.lblAPI.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblAPI.Name = "lblAPI";
            this.lblAPI.Size = new System.Drawing.Size(82, 15);
            this.lblAPI.TabIndex = 182;
            this.lblAPI.Text = "标记流输出";
            // 
            // lblOSD
            // 
            this.lblOSD.AutoSize = true;
            this.lblOSD.Location = new System.Drawing.Point(63, 360);
            this.lblOSD.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblOSD.Name = "lblOSD";
            this.lblOSD.Size = new System.Drawing.Size(61, 15);
            this.lblOSD.TabIndex = 184;
            this.lblOSD.Text = "OSD显示";
            // 
            // nudEmissivityValue
            // 
            this.nudEmissivityValue.DecimalPlaces = 2;
            this.nudEmissivityValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudEmissivityValue.Location = new System.Drawing.Point(287, 173);
            this.nudEmissivityValue.Margin = new System.Windows.Forms.Padding(5);
            this.nudEmissivityValue.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudEmissivityValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudEmissivityValue.Name = "nudEmissivityValue";
            this.nudEmissivityValue.Size = new System.Drawing.Size(97, 25);
            this.nudEmissivityValue.TabIndex = 177;
            this.nudEmissivityValue.Value = new decimal(new int[] {
            100,
            0,
            0,
            131072});
            // 
            // lblOffsetRange
            // 
            this.lblOffsetRange.AutoSize = true;
            this.lblOffsetRange.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblOffsetRange.Location = new System.Drawing.Point(167, 309);
            this.lblOffsetRange.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblOffsetRange.Name = "lblOffsetRange";
            this.lblOffsetRange.Size = new System.Drawing.Size(87, 15);
            this.lblOffsetRange.TabIndex = 174;
            this.lblOffsetRange.Text = "(-200~200)";
            // 
            // lblDistanceRange
            // 
            this.lblDistanceRange.AutoSize = true;
            this.lblDistanceRange.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDistanceRange.Location = new System.Drawing.Point(151, 266);
            this.lblDistanceRange.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblDistanceRange.Name = "lblDistanceRange";
            this.lblDistanceRange.Size = new System.Drawing.Size(103, 15);
            this.lblDistanceRange.TabIndex = 175;
            this.lblDistanceRange.Text = "(0.1~1000.0)";
            // 
            // nudReflectedTemperatureValue
            // 
            this.nudReflectedTemperatureValue.DecimalPlaces = 1;
            this.nudReflectedTemperatureValue.Location = new System.Drawing.Point(287, 217);
            this.nudReflectedTemperatureValue.Margin = new System.Windows.Forms.Padding(5);
            this.nudReflectedTemperatureValue.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            65536});
            this.nudReflectedTemperatureValue.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147418112});
            this.nudReflectedTemperatureValue.Name = "nudReflectedTemperatureValue";
            this.nudReflectedTemperatureValue.Size = new System.Drawing.Size(97, 25);
            this.nudReflectedTemperatureValue.TabIndex = 180;
            this.nudReflectedTemperatureValue.Value = new decimal(new int[] {
            200,
            0,
            0,
            65536});
            // 
            // lblReflectedTemperatureRange
            // 
            this.lblReflectedTemperatureRange.AutoSize = true;
            this.lblReflectedTemperatureRange.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblReflectedTemperatureRange.Location = new System.Drawing.Point(167, 222);
            this.lblReflectedTemperatureRange.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblReflectedTemperatureRange.Name = "lblReflectedTemperatureRange";
            this.lblReflectedTemperatureRange.Size = new System.Drawing.Size(87, 15);
            this.lblReflectedTemperatureRange.TabIndex = 173;
            this.lblReflectedTemperatureRange.Text = "(-200~200)";
            // 
            // nudOffsetValue
            // 
            this.nudOffsetValue.DecimalPlaces = 1;
            this.nudOffsetValue.Location = new System.Drawing.Point(287, 305);
            this.nudOffsetValue.Margin = new System.Windows.Forms.Padding(5);
            this.nudOffsetValue.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            65536});
            this.nudOffsetValue.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147418112});
            this.nudOffsetValue.Name = "nudOffsetValue";
            this.nudOffsetValue.Size = new System.Drawing.Size(97, 25);
            this.nudOffsetValue.TabIndex = 178;
            // 
            // nudDistanceValue
            // 
            this.nudDistanceValue.DecimalPlaces = 1;
            this.nudDistanceValue.Location = new System.Drawing.Point(287, 261);
            this.nudDistanceValue.Margin = new System.Windows.Forms.Padding(5);
            this.nudDistanceValue.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudDistanceValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudDistanceValue.Name = "nudDistanceValue";
            this.nudDistanceValue.Size = new System.Drawing.Size(97, 25);
            this.nudDistanceValue.TabIndex = 179;
            this.nudDistanceValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblEmissivityRange
            // 
            this.lblEmissivityRange.AutoSize = true;
            this.lblEmissivityRange.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblEmissivityRange.Location = new System.Drawing.Point(159, 178);
            this.lblEmissivityRange.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblEmissivityRange.Name = "lblEmissivityRange";
            this.lblEmissivityRange.Size = new System.Drawing.Size(95, 15);
            this.lblEmissivityRange.TabIndex = 176;
            this.lblEmissivityRange.Text = "(0.01~1.00)";
            // 
            // lblEmissivity
            // 
            this.lblEmissivity.AutoSize = true;
            this.lblEmissivity.Location = new System.Drawing.Point(63, 178);
            this.lblEmissivity.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblEmissivity.Name = "lblEmissivity";
            this.lblEmissivity.Size = new System.Drawing.Size(52, 15);
            this.lblEmissivity.TabIndex = 171;
            this.lblEmissivity.Text = "发射率";
            // 
            // lblOffset
            // 
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(63, 309);
            this.lblOffset.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(37, 15);
            this.lblOffset.TabIndex = 170;
            this.lblOffset.Text = "偏移";
            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.Location = new System.Drawing.Point(63, 266);
            this.lblDistance.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(37, 15);
            this.lblDistance.TabIndex = 169;
            this.lblDistance.Text = "距离";
            // 
            // lblReflectedTemperature
            // 
            this.lblReflectedTemperature.AutoSize = true;
            this.lblReflectedTemperature.Location = new System.Drawing.Point(63, 222);
            this.lblReflectedTemperature.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblReflectedTemperature.Name = "lblReflectedTemperature";
            this.lblReflectedTemperature.Size = new System.Drawing.Size(67, 15);
            this.lblReflectedTemperature.TabIndex = 172;
            this.lblReflectedTemperature.Text = "反射温度";
            // 
            // lblMarkerName
            // 
            this.lblMarkerName.AutoSize = true;
            this.lblMarkerName.Font = new System.Drawing.Font("宋体", 9F);
            this.lblMarkerName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblMarkerName.Location = new System.Drawing.Point(67, 134);
            this.lblMarkerName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMarkerName.Name = "lblMarkerName";
            this.lblMarkerName.Size = new System.Drawing.Size(67, 15);
            this.lblMarkerName.TabIndex = 168;
            this.lblMarkerName.Text = "标记名称";
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(262, 49);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(137, 31);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.Text = "创建测温对象";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnSaveROI
            // 
            this.btnSaveROI.Location = new System.Drawing.Point(283, 530);
            this.btnSaveROI.Name = "btnSaveROI";
            this.btnSaveROI.Size = new System.Drawing.Size(100, 35);
            this.btnSaveROI.TabIndex = 5;
            this.btnSaveROI.Text = "保存";
            this.btnSaveROI.UseVisualStyleBackColor = true;
            this.btnSaveROI.Click += new System.EventHandler(this.btnSaveROI_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "工位编号：";
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(241)))), ((int)(((byte)(246)))));
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbArrow,
            this.tsbLine,
            this.tsbPoint,
            this.tsbPolygon});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1051, 27);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Click += new System.EventHandler(this.tsbPolygon_Click);
            // 
            // tsbArrow
            // 
            this.tsbArrow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbArrow.Image = ((System.Drawing.Image)(resources.GetObject("tsbArrow.Image")));
            this.tsbArrow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbArrow.Name = "tsbArrow";
            this.tsbArrow.Size = new System.Drawing.Size(24, 24);
            this.tsbArrow.Text = "指针";
            this.tsbArrow.Click += new System.EventHandler(this.tsbArrow_Click);
            // 
            // tsbLine
            // 
            this.tsbLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLine.Image = ((System.Drawing.Image)(resources.GetObject("tsbLine.Image")));
            this.tsbLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLine.Name = "tsbLine";
            this.tsbLine.Size = new System.Drawing.Size(24, 24);
            this.tsbLine.Text = "线";
            this.tsbLine.Click += new System.EventHandler(this.tsbLine_Click);
            // 
            // tsbPoint
            // 
            this.tsbPoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPoint.Image = ((System.Drawing.Image)(resources.GetObject("tsbPoint.Image")));
            this.tsbPoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPoint.Name = "tsbPoint";
            this.tsbPoint.Size = new System.Drawing.Size(24, 24);
            this.tsbPoint.Text = "点";
            this.tsbPoint.Click += new System.EventHandler(this.tsbPoint_Click);
            // 
            // tsbPolygon
            // 
            this.tsbPolygon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPolygon.Image = ((System.Drawing.Image)(resources.GetObject("tsbPolygon.Image")));
            this.tsbPolygon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPolygon.Name = "tsbPolygon";
            this.tsbPolygon.Size = new System.Drawing.Size(24, 24);
            this.tsbPolygon.Text = "多边形";
            // 
            // tsmiDel
            // 
            this.tsmiDel.Name = "tsmiDel";
            this.tsmiDel.Size = new System.Drawing.Size(108, 24);
            this.tsmiDel.Text = "删除";
            this.tsmiDel.Click += new System.EventHandler(this.tsmiDelItem_Click);
            // 
            // cmsISP
            // 
            this.cmsISP.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsISP.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDel});
            this.cmsISP.Name = "contextMenuStrip1";
            this.cmsISP.Size = new System.Drawing.Size(109, 28);
            this.cmsISP.Opening += new System.ComponentModel.CancelEventHandler(this.cmsISP_Opening);
            // 
            // tpbTA
            // 
            this.tpbTA.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tpbTA.ContextMenuStrip = this.cmsISP;
            this.tpbTA.Draw = null;
            this.tpbTA.Location = new System.Drawing.Point(12, 30);
            this.tpbTA.Name = "tpbTA";
            this.tpbTA.Size = new System.Drawing.Size(618, 586);
            this.tpbTA.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.tpbTA.TabIndex = 9;
            this.tpbTA.TabStop = false;
            this.tpbTA.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tpbTA_MouseDoubleClick);
            this.tpbTA.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tpbTA_MouseDown);
            this.tpbTA.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tpbTA_MouseMove);
            // 
            // ROIFFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1051, 644);
            this.Controls.Add(this.tpbTA);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ROIFFrm";
            this.Text = "温区编辑";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmissivityValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReflectedTemperatureValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOffsetValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDistanceValue)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.cmsISP.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tpbTA)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip ttISPAddForm;
        private System.Windows.Forms.ComboBox cbxMarkerName;
        private System.Windows.Forms.ComboBox cbxwLocate;
        private System.Windows.Forms.TextBox tbAlarm;
        private System.Windows.Forms.CheckBox cbAlarm;
        private System.Windows.Forms.CheckBox cbAPI;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox cbOSD;
        private System.Windows.Forms.Label lblAlarm;
        private System.Windows.Forms.Label lblAPI;
        private System.Windows.Forms.Label lblOSD;
        private System.Windows.Forms.NumericUpDown nudEmissivityValue;
        private System.Windows.Forms.Label lblOffsetRange;
        private System.Windows.Forms.Label lblDistanceRange;
        private System.Windows.Forms.NumericUpDown nudReflectedTemperatureValue;
        private System.Windows.Forms.Label lblReflectedTemperatureRange;
        private System.Windows.Forms.NumericUpDown nudOffsetValue;
        private System.Windows.Forms.NumericUpDown nudDistanceValue;
        private System.Windows.Forms.Label lblEmissivityRange;
        private System.Windows.Forms.Label lblEmissivity;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.Label lblReflectedTemperature;
        private System.Windows.Forms.Label lblMarkerName;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnSaveROI;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbArrow;
        private System.Windows.Forms.ToolStripButton tsbLine;
        private System.Windows.Forms.ToolStripButton tsbPoint;
        private System.Windows.Forms.ToolStripButton tsbPolygon;
        private System.Windows.Forms.ToolStripMenuItem tsmiDel;
        private System.Windows.Forms.ContextMenuStrip cmsISP;
        private TempDetect.OWBPictureBox tpbTA;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton Negativerdbtn;
        private System.Windows.Forms.RadioButton Positiverdbtn;
    }
}