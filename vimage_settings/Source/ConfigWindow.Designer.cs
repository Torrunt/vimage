namespace vimage_settings
{
    partial class ConfigWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigWindow));
            this.TabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.comboBox_DefaultSortDir = new System.Windows.Forms.ComboBox();
            this.comboBox_DefaultSortBy = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBox_LimitImagesToMonitor = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDown_ZoomSpeedFast = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDown_ZoomSpeed = new System.Windows.Forms.NumericUpDown();
            this.checkBox_ListenForConfigChanges = new System.Windows.Forms.CheckBox();
            this.checkBox_OpenSettingsEXE = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_SmoothingMinImageSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_MinImageSize = new System.Windows.Forms.NumericUpDown();
            this.checkBox_PreloadNextImage = new System.Windows.Forms.CheckBox();
            this.checkBox_PositionLargeWideImagesInCorner = new System.Windows.Forms.CheckBox();
            this.checkBox_BackgroundForImagesWithTransparencyDefault = new System.Windows.Forms.CheckBox();
            this.checkBox_SmoothingDefault = new System.Windows.Forms.CheckBox();
            this.checkBox_OpenAtMousePosition = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panel_Controls = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button_ControlsDefault = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.checkBox_ContextMenuShowMargin = new System.Windows.Forms.CheckBox();
            this.numericUpDown_ContextMenu_Animation_InsertAtIndex = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_ContextMenuAddNew = new System.Windows.Forms.Button();
            this.button_ContextMenuDefault = new System.Windows.Forms.Button();
            this.tabControl_ContextMenus = new System.Windows.Forms.TabControl();
            this.tabPage_ContextMenuGeneral = new System.Windows.Forms.TabPage();
            this.tabPage_ContextMenuAnimation = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label_version = new System.Windows.Forms.Label();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.contextMenuStrip_Empty = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.button_Save = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ZoomSpeedFast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ZoomSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SmoothingMinImageSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MinImageSize)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ContextMenu_Animation_InsertAtIndex)).BeginInit();
            this.panel2.SuspendLayout();
            this.tabControl_ContextMenus.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.tabPage1);
            this.TabControl.Controls.Add(this.tabPage2);
            this.TabControl.Controls.Add(this.tabPage3);
            this.TabControl.Controls.Add(this.tabPage4);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Margin = new System.Windows.Forms.Padding(0);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(434, 331);
            this.TabControl.TabIndex = 0;
            this.TabControl.TabStop = false;
            this.TabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.comboBox_DefaultSortDir);
            this.tabPage1.Controls.Add(this.comboBox_DefaultSortBy);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.comboBox_LimitImagesToMonitor);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.numericUpDown_ZoomSpeedFast);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.numericUpDown_ZoomSpeed);
            this.tabPage1.Controls.Add(this.checkBox_ListenForConfigChanges);
            this.tabPage1.Controls.Add(this.checkBox_OpenSettingsEXE);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.numericUpDown_SmoothingMinImageSize);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.numericUpDown_MinImageSize);
            this.tabPage1.Controls.Add(this.checkBox_PreloadNextImage);
            this.tabPage1.Controls.Add(this.checkBox_PositionLargeWideImagesInCorner);
            this.tabPage1.Controls.Add(this.checkBox_BackgroundForImagesWithTransparencyDefault);
            this.tabPage1.Controls.Add(this.checkBox_SmoothingDefault);
            this.tabPage1.Controls.Add(this.checkBox_OpenAtMousePosition);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(426, 305);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // comboBox_DefaultSortDir
            // 
            this.comboBox_DefaultSortDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DefaultSortDir.FormattingEnabled = true;
            this.comboBox_DefaultSortDir.Items.AddRange(new object[] {
            "Ascending",
            "Descending"});
            this.comboBox_DefaultSortDir.Location = new System.Drawing.Point(320, 276);
            this.comboBox_DefaultSortDir.Name = "comboBox_DefaultSortDir";
            this.comboBox_DefaultSortDir.Size = new System.Drawing.Size(82, 21);
            this.comboBox_DefaultSortDir.TabIndex = 13;
            this.toolTip1.SetToolTip(this.comboBox_DefaultSortDir, "Default sorting when stepping through images in a folder");
            // 
            // comboBox_DefaultSortBy
            // 
            this.comboBox_DefaultSortBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DefaultSortBy.FormattingEnabled = true;
            this.comboBox_DefaultSortBy.Items.AddRange(new object[] {
            "Name",
            "Date Modified",
            "Date Created",
            "Size"});
            this.comboBox_DefaultSortBy.Location = new System.Drawing.Point(235, 276);
            this.comboBox_DefaultSortBy.Name = "comboBox_DefaultSortBy";
            this.comboBox_DefaultSortBy.Size = new System.Drawing.Size(82, 21);
            this.comboBox_DefaultSortBy.TabIndex = 12;
            this.toolTip1.SetToolTip(this.comboBox_DefaultSortBy, "Default sorting when stepping through images in a folder");
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 279);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(77, 13);
            this.label16.TabIndex = 22;
            this.label16.Text = "Default Sorting";
            // 
            // comboBox_LimitImagesToMonitor
            // 
            this.comboBox_LimitImagesToMonitor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_LimitImagesToMonitor.FormattingEnabled = true;
            this.comboBox_LimitImagesToMonitor.Items.AddRange(new object[] {
            "NONE",
            "HEIGHT",
            "WIDTH",
            "AUTO"});
            this.comboBox_LimitImagesToMonitor.Location = new System.Drawing.Point(320, 166);
            this.comboBox_LimitImagesToMonitor.Name = "comboBox_LimitImagesToMonitor";
            this.comboBox_LimitImagesToMonitor.Size = new System.Drawing.Size(82, 21);
            this.comboBox_LimitImagesToMonitor.TabIndex = 7;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 169);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(113, 13);
            this.label15.TabIndex = 20;
            this.label15.Text = "Limit images to monitor";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(382, 212);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(18, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "px";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(382, 190);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(18, 13);
            this.label14.TabIndex = 18;
            this.label14.Text = "px";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(372, 256);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(15, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "%";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 256);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(91, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "Zoom Speed Fast";
            // 
            // numericUpDown_ZoomSpeedFast
            // 
            this.numericUpDown_ZoomSpeedFast.Location = new System.Drawing.Point(320, 254);
            this.numericUpDown_ZoomSpeedFast.Name = "numericUpDown_ZoomSpeedFast";
            this.numericUpDown_ZoomSpeedFast.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown_ZoomSpeedFast.TabIndex = 11;
            this.toolTip1.SetToolTip(this.numericUpDown_ZoomSpeedFast, "Zoom speed when holding down SHIFT (default)");
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(372, 234);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(15, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "%";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 234);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Zoom Speed";
            // 
            // numericUpDown_ZoomSpeed
            // 
            this.numericUpDown_ZoomSpeed.Location = new System.Drawing.Point(320, 232);
            this.numericUpDown_ZoomSpeed.Name = "numericUpDown_ZoomSpeed";
            this.numericUpDown_ZoomSpeed.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown_ZoomSpeed.TabIndex = 10;
            // 
            // checkBox_ListenForConfigChanges
            // 
            this.checkBox_ListenForConfigChanges.AutoSize = true;
            this.checkBox_ListenForConfigChanges.Location = new System.Drawing.Point(9, 145);
            this.checkBox_ListenForConfigChanges.Name = "checkBox_ListenForConfigChanges";
            this.checkBox_ListenForConfigChanges.Size = new System.Drawing.Size(147, 17);
            this.checkBox_ListenForConfigChanges.TabIndex = 6;
            this.checkBox_ListenForConfigChanges.Text = "Listen for setting changes";
            this.toolTip1.SetToolTip(this.checkBox_ListenForConfigChanges, "If ticked, vimage will reload settings automatically when they are changed.");
            this.checkBox_ListenForConfigChanges.UseVisualStyleBackColor = true;
            // 
            // checkBox_OpenSettingsEXE
            // 
            this.checkBox_OpenSettingsEXE.AutoSize = true;
            this.checkBox_OpenSettingsEXE.Location = new System.Drawing.Point(9, 122);
            this.checkBox_OpenSettingsEXE.Name = "checkBox_OpenSettingsEXE";
            this.checkBox_OpenSettingsEXE.Size = new System.Drawing.Size(324, 17);
            this.checkBox_OpenSettingsEXE.TabIndex = 5;
            this.checkBox_OpenSettingsEXE.Text = "Use vimage_settings.exe (otherwise will open config.txt directly)";
            this.toolTip1.SetToolTip(this.checkBox_OpenSettingsEXE, "What will open when the \'Open Config\' button is clicked or the shortcut is used.");
            this.checkBox_OpenSettingsEXE.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 212);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Don\'t smooth images smaller than";
            // 
            // numericUpDown_SmoothingMinImageSize
            // 
            this.numericUpDown_SmoothingMinImageSize.Location = new System.Drawing.Point(320, 210);
            this.numericUpDown_SmoothingMinImageSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_SmoothingMinImageSize.Name = "numericUpDown_SmoothingMinImageSize";
            this.numericUpDown_SmoothingMinImageSize.Size = new System.Drawing.Size(60, 20);
            this.numericUpDown_SmoothingMinImageSize.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 190);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(258, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Min image size (if image is smaller, it will be scaled up)";
            // 
            // numericUpDown_MinImageSize
            // 
            this.numericUpDown_MinImageSize.Location = new System.Drawing.Point(320, 188);
            this.numericUpDown_MinImageSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_MinImageSize.Name = "numericUpDown_MinImageSize";
            this.numericUpDown_MinImageSize.Size = new System.Drawing.Size(60, 20);
            this.numericUpDown_MinImageSize.TabIndex = 8;
            // 
            // checkBox_PreloadNextImage
            // 
            this.checkBox_PreloadNextImage.AutoSize = true;
            this.checkBox_PreloadNextImage.Location = new System.Drawing.Point(9, 99);
            this.checkBox_PreloadNextImage.Name = "checkBox_PreloadNextImage";
            this.checkBox_PreloadNextImage.Size = new System.Drawing.Size(116, 17);
            this.checkBox_PreloadNextImage.TabIndex = 4;
            this.checkBox_PreloadNextImage.Text = "Preload next image";
            this.toolTip1.SetToolTip(this.checkBox_PreloadNextImage, "When using the next/prev image buttons, the image after the one just loaded will " +
        "be loaded as well.");
            this.checkBox_PreloadNextImage.UseVisualStyleBackColor = true;
            // 
            // checkBox_PositionLargeWideImagesInCorner
            // 
            this.checkBox_PositionLargeWideImagesInCorner.AutoSize = true;
            this.checkBox_PositionLargeWideImagesInCorner.Location = new System.Drawing.Point(9, 76);
            this.checkBox_PositionLargeWideImagesInCorner.Name = "checkBox_PositionLargeWideImagesInCorner";
            this.checkBox_PositionLargeWideImagesInCorner.Size = new System.Drawing.Size(331, 17);
            this.checkBox_PositionLargeWideImagesInCorner.TabIndex = 3;
            this.checkBox_PositionLargeWideImagesInCorner.Text = "Position large/wide images in corner (ie: wallpapers/screenshots)";
            this.toolTip1.SetToolTip(this.checkBox_PositionLargeWideImagesInCorner, "Images that are wider than or the same width as the current monitor will be place" +
        "d in the top left corner.");
            this.checkBox_PositionLargeWideImagesInCorner.UseVisualStyleBackColor = true;
            // 
            // checkBox_BackgroundForImagesWithTransparencyDefault
            // 
            this.checkBox_BackgroundForImagesWithTransparencyDefault.AutoSize = true;
            this.checkBox_BackgroundForImagesWithTransparencyDefault.Location = new System.Drawing.Point(9, 53);
            this.checkBox_BackgroundForImagesWithTransparencyDefault.Name = "checkBox_BackgroundForImagesWithTransparencyDefault";
            this.checkBox_BackgroundForImagesWithTransparencyDefault.Size = new System.Drawing.Size(270, 17);
            this.checkBox_BackgroundForImagesWithTransparencyDefault.TabIndex = 2;
            this.checkBox_BackgroundForImagesWithTransparencyDefault.Text = "Show background behind images with transparency";
            this.toolTip1.SetToolTip(this.checkBox_BackgroundForImagesWithTransparencyDefault, "Images with partial transparency will have a grey background behind them.");
            this.checkBox_BackgroundForImagesWithTransparencyDefault.UseVisualStyleBackColor = true;
            // 
            // checkBox_SmoothingDefault
            // 
            this.checkBox_SmoothingDefault.AutoSize = true;
            this.checkBox_SmoothingDefault.Location = new System.Drawing.Point(9, 30);
            this.checkBox_SmoothingDefault.Name = "checkBox_SmoothingDefault";
            this.checkBox_SmoothingDefault.Size = new System.Drawing.Size(147, 17);
            this.checkBox_SmoothingDefault.TabIndex = 1;
            this.checkBox_SmoothingDefault.Text = "Smooth images by default";
            this.checkBox_SmoothingDefault.UseVisualStyleBackColor = true;
            // 
            // checkBox_OpenAtMousePosition
            // 
            this.checkBox_OpenAtMousePosition.AutoSize = true;
            this.checkBox_OpenAtMousePosition.Location = new System.Drawing.Point(9, 7);
            this.checkBox_OpenAtMousePosition.Name = "checkBox_OpenAtMousePosition";
            this.checkBox_OpenAtMousePosition.Size = new System.Drawing.Size(173, 17);
            this.checkBox_OpenAtMousePosition.TabIndex = 0;
            this.checkBox_OpenAtMousePosition.Text = "Open images at mouse position";
            this.toolTip1.SetToolTip(this.checkBox_OpenAtMousePosition, "When an image is opened it will be centered at the current mouse position (common" +
        "ly where the image file is).");
            this.checkBox_OpenAtMousePosition.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.tableLayoutPanel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(426, 334);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Control Bindings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.panel_Controls, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.panel4, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(420, 328);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // panel_Controls
            // 
            this.panel_Controls.AutoScroll = true;
            this.panel_Controls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Controls.Location = new System.Drawing.Point(3, 3);
            this.panel_Controls.Name = "panel_Controls";
            this.panel_Controls.Size = new System.Drawing.Size(414, 292);
            this.panel_Controls.TabIndex = 13;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button_ControlsDefault);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 298);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(420, 30);
            this.panel4.TabIndex = 14;
            // 
            // button_ControlsDefault
            // 
            this.button_ControlsDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ControlsDefault.Location = new System.Drawing.Point(346, 5);
            this.button_ControlsDefault.Name = "button_ControlsDefault";
            this.button_ControlsDefault.Size = new System.Drawing.Size(75, 23);
            this.button_ControlsDefault.TabIndex = 12;
            this.button_ControlsDefault.TabStop = false;
            this.button_ControlsDefault.Text = "Default";
            this.button_ControlsDefault.UseVisualStyleBackColor = true;
            this.button_ControlsDefault.Click += new System.EventHandler(this.button_ControlsDefault_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tableLayoutPanel2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(426, 334);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Context Menu";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.tabControl_ContextMenus, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(426, 334);
            this.tableLayoutPanel2.TabIndex = 17;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.checkBox_ContextMenuShowMargin);
            this.panel3.Controls.Add(this.numericUpDown_ContextMenu_Animation_InsertAtIndex);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(420, 24);
            this.panel3.TabIndex = 18;
            // 
            // checkBox_ContextMenuShowMargin
            // 
            this.checkBox_ContextMenuShowMargin.AutoSize = true;
            this.checkBox_ContextMenuShowMargin.Location = new System.Drawing.Point(6, 4);
            this.checkBox_ContextMenuShowMargin.Name = "checkBox_ContextMenuShowMargin";
            this.checkBox_ContextMenuShowMargin.Size = new System.Drawing.Size(150, 17);
            this.checkBox_ContextMenuShowMargin.TabIndex = 1;
            this.checkBox_ContextMenuShowMargin.TabStop = false;
            this.checkBox_ContextMenuShowMargin.Text = "Show margin/checkboxes";
            this.toolTip1.SetToolTip(this.checkBox_ContextMenuShowMargin, "Context menu will have a margin or the left and some items will have checkboxes.");
            this.checkBox_ContextMenuShowMargin.UseVisualStyleBackColor = true;
            // 
            // numericUpDown_ContextMenu_Animation_InsertAtIndex
            // 
            this.numericUpDown_ContextMenu_Animation_InsertAtIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown_ContextMenu_Animation_InsertAtIndex.Location = new System.Drawing.Point(379, 2);
            this.numericUpDown_ContextMenu_Animation_InsertAtIndex.Name = "numericUpDown_ContextMenu_Animation_InsertAtIndex";
            this.numericUpDown_ContextMenu_Animation_InsertAtIndex.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown_ContextMenu_Animation_InsertAtIndex.TabIndex = 2;
            this.numericUpDown_ContextMenu_Animation_InsertAtIndex.TabStop = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(223, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Insert animation menu at index";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_ContextMenuAddNew);
            this.panel2.Controls.Add(this.button_ContextMenuDefault);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 307);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(420, 24);
            this.panel2.TabIndex = 16;
            // 
            // button_ContextMenuAddNew
            // 
            this.button_ContextMenuAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_ContextMenuAddNew.BackgroundImage = global::vimage_settings.Properties.Resources.add;
            this.button_ContextMenuAddNew.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_ContextMenuAddNew.Location = new System.Drawing.Point(-1, -1);
            this.button_ContextMenuAddNew.Name = "button_ContextMenuAddNew";
            this.button_ContextMenuAddNew.Size = new System.Drawing.Size(23, 23);
            this.button_ContextMenuAddNew.TabIndex = 4;
            this.button_ContextMenuAddNew.TabStop = false;
            this.toolTip1.SetToolTip(this.button_ContextMenuAddNew, "Add item below currently selected");
            this.button_ContextMenuAddNew.UseVisualStyleBackColor = true;
            this.button_ContextMenuAddNew.Click += new System.EventHandler(this.button_ContextMenuAddNew_Click);
            // 
            // button_ContextMenuDefault
            // 
            this.button_ContextMenuDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ContextMenuDefault.Location = new System.Drawing.Point(346, -1);
            this.button_ContextMenuDefault.Name = "button_ContextMenuDefault";
            this.button_ContextMenuDefault.Size = new System.Drawing.Size(75, 23);
            this.button_ContextMenuDefault.TabIndex = 5;
            this.button_ContextMenuDefault.TabStop = false;
            this.button_ContextMenuDefault.Text = "Default";
            this.button_ContextMenuDefault.UseVisualStyleBackColor = true;
            this.button_ContextMenuDefault.Click += new System.EventHandler(this.button_ContextMenuDefault_Click);
            // 
            // tabControl_ContextMenus
            // 
            this.tabControl_ContextMenus.Controls.Add(this.tabPage_ContextMenuGeneral);
            this.tabControl_ContextMenus.Controls.Add(this.tabPage_ContextMenuAnimation);
            this.tabControl_ContextMenus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_ContextMenus.Location = new System.Drawing.Point(3, 33);
            this.tabControl_ContextMenus.Name = "tabControl_ContextMenus";
            this.tabControl_ContextMenus.SelectedIndex = 0;
            this.tabControl_ContextMenus.Size = new System.Drawing.Size(420, 268);
            this.tabControl_ContextMenus.TabIndex = 3;
            this.tabControl_ContextMenus.SelectedIndexChanged += new System.EventHandler(this.tabControl_ContextMenus_SelectedIndexChanged);
            // 
            // tabPage_ContextMenuGeneral
            // 
            this.tabPage_ContextMenuGeneral.AutoScroll = true;
            this.tabPage_ContextMenuGeneral.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage_ContextMenuGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPage_ContextMenuGeneral.Name = "tabPage_ContextMenuGeneral";
            this.tabPage_ContextMenuGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ContextMenuGeneral.Size = new System.Drawing.Size(412, 242);
            this.tabPage_ContextMenuGeneral.TabIndex = 0;
            this.tabPage_ContextMenuGeneral.Text = "General";
            // 
            // tabPage_ContextMenuAnimation
            // 
            this.tabPage_ContextMenuAnimation.AutoScroll = true;
            this.tabPage_ContextMenuAnimation.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage_ContextMenuAnimation.Location = new System.Drawing.Point(4, 22);
            this.tabPage_ContextMenuAnimation.Name = "tabPage_ContextMenuAnimation";
            this.tabPage_ContextMenuAnimation.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ContextMenuAnimation.Size = new System.Drawing.Size(412, 242);
            this.tabPage_ContextMenuAnimation.TabIndex = 1;
            this.tabPage_ContextMenuAnimation.Text = "Animation";
            // 
            // tabPage4
            // 
            this.tabPage4.AutoScroll = true;
            this.tabPage4.Controls.Add(this.label_version);
            this.tabPage4.Controls.Add(this.linkLabel6);
            this.tabPage4.Controls.Add(this.linkLabel5);
            this.tabPage4.Controls.Add(this.linkLabel4);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.linkLabel3);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Controls.Add(this.linkLabel2);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(426, 334);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "About";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label_version
            // 
            this.label_version.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_version.AutoSize = true;
            this.label_version.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_version.Location = new System.Drawing.Point(346, 13);
            this.label_version.Name = "label_version";
            this.label_version.Size = new System.Drawing.Size(72, 20);
            this.label_version.TabIndex = 20;
            this.label_version.Text = "version #";
            // 
            // linkLabel6
            // 
            this.linkLabel6.AutoSize = true;
            this.linkLabel6.Location = new System.Drawing.Point(10, 87);
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.Size = new System.Drawing.Size(137, 13);
            this.linkLabel6.TabIndex = 2;
            this.linkLabel6.TabStop = true;
            this.linkLabel6.Text = "github.com/Torrunt/vimage";
            this.linkLabel6.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel6_LinkClicked);
            // 
            // linkLabel5
            // 
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.Location = new System.Drawing.Point(10, 71);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(94, 13);
            this.linkLabel5.TabIndex = 1;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "torrunt.net/vimage";
            this.linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(48, 168);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(150, 13);
            this.linkLabel4.TabIndex = 5;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "famfamfam.com/lab/icons/silk";
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel4_LinkClicked);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Icons -";
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(142, 145);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(112, 13);
            this.linkLabel3.TabIndex = 4;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "openil.sourceforge.net";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(132, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Image Loading via DevIL -";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(220, 122);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(80, 13);
            this.linkLabel2.TabIndex = 3;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "me@torrunt.net";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 122);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(211, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Created by Corey Zeke Womack (Torrunt) -\r\n";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(8, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 25);
            this.label5.TabIndex = 1;
            this.label5.Text = "vimage";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(188, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "A simplistic image viewer for Windows.";
            // 
            // contextMenuStrip_Empty
            // 
            this.contextMenuStrip_Empty.Name = "contextMenuStrip1";
            this.contextMenuStrip_Empty.Size = new System.Drawing.Size(61, 4);
            // 
            // button_Save
            // 
            this.button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Save.Location = new System.Drawing.Point(354, 1);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 23);
            this.button_Save.TabIndex = 10;
            this.button_Save.TabStop = false;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(3, 6);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(94, 13);
            this.linkLabel1.TabIndex = 11;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "torrunt.net/vimage";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Controls.Add(this.button_Save);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 334);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(428, 24);
            this.panel1.TabIndex = 12;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.TabControl, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(434, 361);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // ConfigWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 361);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 322);
            this.Name = "ConfigWindow";
            this.Text = "vimage settings";
            this.TabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ZoomSpeedFast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ZoomSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SmoothingMinImageSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MinImageSize)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ContextMenu_Animation_InsertAtIndex)).EndInit();
            this.panel2.ResumeLayout(false);
            this.tabControl_ContextMenus.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox checkBox_OpenAtMousePosition;
        private System.Windows.Forms.CheckBox checkBox_SmoothingDefault;
        private System.Windows.Forms.CheckBox checkBox_BackgroundForImagesWithTransparencyDefault;
        private System.Windows.Forms.CheckBox checkBox_PositionLargeWideImagesInCorner;
        private System.Windows.Forms.CheckBox checkBox_PreloadNextImage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown_SmoothingMinImageSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown_MinImageSize;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.CheckBox checkBox_OpenSettingsEXE;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown_ContextMenu_Animation_InsertAtIndex;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Empty;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox checkBox_ContextMenuShowMargin;
        private System.Windows.Forms.TabControl tabControl_ContextMenus;
        private System.Windows.Forms.TabPage tabPage_ContextMenuGeneral;
        private System.Windows.Forms.TabPage tabPage_ContextMenuAnimation;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.Label label_version;
        private System.Windows.Forms.CheckBox checkBox_ListenForConfigChanges;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numericUpDown_ZoomSpeedFast;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDown_ZoomSpeed;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox comboBox_LimitImagesToMonitor;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button button_ControlsDefault;
        private System.Windows.Forms.Panel panel_Controls;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button_ContextMenuAddNew;
        private System.Windows.Forms.Button button_ContextMenuDefault;
        private System.Windows.Forms.ComboBox comboBox_DefaultSortDir;
        private System.Windows.Forms.ComboBox comboBox_DefaultSortBy;
        private System.Windows.Forms.Label label16;
    }
}

