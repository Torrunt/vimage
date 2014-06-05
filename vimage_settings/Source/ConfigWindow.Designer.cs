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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigWindow));
            this.TabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_SmoothingMinImageSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_MinImageSize = new System.Windows.Forms.NumericUpDown();
            this.checkBox_PreloadNextImage = new System.Windows.Forms.CheckBox();
            this.checkBox_PositionLargeWideImagesInCorner = new System.Windows.Forms.CheckBox();
            this.checkBox_LimitImagesToMonitorHeight = new System.Windows.Forms.CheckBox();
            this.checkBox_BackgroundForImagesWithTransparencyDefault = new System.Windows.Forms.CheckBox();
            this.checkBox_SmoothingDefault = new System.Windows.Forms.CheckBox();
            this.checkBox_OpenAtMousePosition = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBox_ContextMenuShowMargin = new System.Windows.Forms.CheckBox();
            this.button_Save = new System.Windows.Forms.Button();
            this.TabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SmoothingMinImageSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MinImageSize)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.tabPage1);
            this.TabControl.Controls.Add(this.tabPage2);
            this.TabControl.Controls.Add(this.tabPage3);
            this.TabControl.Location = new System.Drawing.Point(8, 9);
            this.TabControl.Margin = new System.Windows.Forms.Padding(0);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(376, 323);
            this.TabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.numericUpDown_SmoothingMinImageSize);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.numericUpDown_MinImageSize);
            this.tabPage1.Controls.Add(this.checkBox_PreloadNextImage);
            this.tabPage1.Controls.Add(this.checkBox_PositionLargeWideImagesInCorner);
            this.tabPage1.Controls.Add(this.checkBox_LimitImagesToMonitorHeight);
            this.tabPage1.Controls.Add(this.checkBox_BackgroundForImagesWithTransparencyDefault);
            this.tabPage1.Controls.Add(this.checkBox_SmoothingDefault);
            this.tabPage1.Controls.Add(this.checkBox_OpenAtMousePosition);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(368, 297);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 169);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Don\'t Smooth Images Smaller than";
            // 
            // numericUpDown_SmoothingMinImageSize
            // 
            this.numericUpDown_SmoothingMinImageSize.Location = new System.Drawing.Point(302, 167);
            this.numericUpDown_SmoothingMinImageSize.Name = "numericUpDown_SmoothingMinImageSize";
            this.numericUpDown_SmoothingMinImageSize.Size = new System.Drawing.Size(60, 20);
            this.numericUpDown_SmoothingMinImageSize.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(261, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Min Image Size (if image is smaller, it will be scaled up)";
            // 
            // numericUpDown_MinImageSize
            // 
            this.numericUpDown_MinImageSize.Location = new System.Drawing.Point(302, 144);
            this.numericUpDown_MinImageSize.Name = "numericUpDown_MinImageSize";
            this.numericUpDown_MinImageSize.Size = new System.Drawing.Size(60, 20);
            this.numericUpDown_MinImageSize.TabIndex = 6;
            // 
            // checkBox_PreloadNextImage
            // 
            this.checkBox_PreloadNextImage.AutoSize = true;
            this.checkBox_PreloadNextImage.Location = new System.Drawing.Point(9, 122);
            this.checkBox_PreloadNextImage.Name = "checkBox_PreloadNextImage";
            this.checkBox_PreloadNextImage.Size = new System.Drawing.Size(119, 17);
            this.checkBox_PreloadNextImage.TabIndex = 5;
            this.checkBox_PreloadNextImage.Text = "Preload Next Image";
            this.checkBox_PreloadNextImage.UseVisualStyleBackColor = true;
            // 
            // checkBox_PositionLargeWideImagesInCorner
            // 
            this.checkBox_PositionLargeWideImagesInCorner.AutoSize = true;
            this.checkBox_PositionLargeWideImagesInCorner.Location = new System.Drawing.Point(9, 99);
            this.checkBox_PositionLargeWideImagesInCorner.Name = "checkBox_PositionLargeWideImagesInCorner";
            this.checkBox_PositionLargeWideImagesInCorner.Size = new System.Drawing.Size(345, 17);
            this.checkBox_PositionLargeWideImagesInCorner.TabIndex = 4;
            this.checkBox_PositionLargeWideImagesInCorner.Text = "Position Large/Wide Images in Corner (ie: Wallpapers/Screenshots)";
            this.checkBox_PositionLargeWideImagesInCorner.UseVisualStyleBackColor = true;
            // 
            // checkBox_LimitImagesToMonitorHeight
            // 
            this.checkBox_LimitImagesToMonitorHeight.AutoSize = true;
            this.checkBox_LimitImagesToMonitorHeight.Location = new System.Drawing.Point(9, 76);
            this.checkBox_LimitImagesToMonitorHeight.Name = "checkBox_LimitImagesToMonitorHeight";
            this.checkBox_LimitImagesToMonitorHeight.Size = new System.Drawing.Size(168, 17);
            this.checkBox_LimitImagesToMonitorHeight.TabIndex = 3;
            this.checkBox_LimitImagesToMonitorHeight.Text = "Limit Images to Monitor Height";
            this.checkBox_LimitImagesToMonitorHeight.UseVisualStyleBackColor = true;
            // 
            // checkBox_BackgroundForImagesWithTransparencyDefault
            // 
            this.checkBox_BackgroundForImagesWithTransparencyDefault.AutoSize = true;
            this.checkBox_BackgroundForImagesWithTransparencyDefault.Location = new System.Drawing.Point(9, 53);
            this.checkBox_BackgroundForImagesWithTransparencyDefault.Name = "checkBox_BackgroundForImagesWithTransparencyDefault";
            this.checkBox_BackgroundForImagesWithTransparencyDefault.Size = new System.Drawing.Size(276, 17);
            this.checkBox_BackgroundForImagesWithTransparencyDefault.TabIndex = 2;
            this.checkBox_BackgroundForImagesWithTransparencyDefault.Text = "Show Background behind Images with Transparency";
            this.checkBox_BackgroundForImagesWithTransparencyDefault.UseVisualStyleBackColor = true;
            // 
            // checkBox_SmoothingDefault
            // 
            this.checkBox_SmoothingDefault.AutoSize = true;
            this.checkBox_SmoothingDefault.Location = new System.Drawing.Point(9, 30);
            this.checkBox_SmoothingDefault.Name = "checkBox_SmoothingDefault";
            this.checkBox_SmoothingDefault.Size = new System.Drawing.Size(150, 17);
            this.checkBox_SmoothingDefault.TabIndex = 1;
            this.checkBox_SmoothingDefault.Text = "Smooth Images by Default";
            this.checkBox_SmoothingDefault.UseVisualStyleBackColor = true;
            // 
            // checkBox_OpenAtMousePosition
            // 
            this.checkBox_OpenAtMousePosition.AutoSize = true;
            this.checkBox_OpenAtMousePosition.Location = new System.Drawing.Point(9, 7);
            this.checkBox_OpenAtMousePosition.Name = "checkBox_OpenAtMousePosition";
            this.checkBox_OpenAtMousePosition.Size = new System.Drawing.Size(176, 17);
            this.checkBox_OpenAtMousePosition.TabIndex = 0;
            this.checkBox_OpenAtMousePosition.Text = "Open Images at Mouse Position";
            this.checkBox_OpenAtMousePosition.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(365, 297);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Control Bindings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.checkBox_ContextMenuShowMargin);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(365, 297);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Context Menu";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBox_ContextMenuShowMargin
            // 
            this.checkBox_ContextMenuShowMargin.AutoSize = true;
            this.checkBox_ContextMenuShowMargin.Location = new System.Drawing.Point(9, 7);
            this.checkBox_ContextMenuShowMargin.Name = "checkBox_ContextMenuShowMargin";
            this.checkBox_ContextMenuShowMargin.Size = new System.Drawing.Size(152, 17);
            this.checkBox_ContextMenuShowMargin.TabIndex = 1;
            this.checkBox_ContextMenuShowMargin.Text = "Show Margin/Checkboxes";
            this.checkBox_ContextMenuShowMargin.UseVisualStyleBackColor = true;
            // 
            // button_Save
            // 
            this.button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Save.Location = new System.Drawing.Point(309, 334);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 23);
            this.button_Save.TabIndex = 10;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // ConfigWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 361);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.TabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 280);
            this.Name = "ConfigWindow";
            this.Text = "vimage settings";
            this.TabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SmoothingMinImageSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MinImageSize)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
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
        private System.Windows.Forms.CheckBox checkBox_LimitImagesToMonitorHeight;
        private System.Windows.Forms.CheckBox checkBox_PreloadNextImage;
        private System.Windows.Forms.CheckBox checkBox_ContextMenuShowMargin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown_SmoothingMinImageSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown_MinImageSize;
        private System.Windows.Forms.Button button_Save;
    }
}

