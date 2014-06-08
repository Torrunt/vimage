using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using vimage;

namespace vimage_settings
{
    public partial class ConfigWindow : Form
    {
        private Config vimageConfig;

        private TextBox TextBoxWithMouseFocus = null;

        public ConfigWindow()
        {
            InitializeComponent();

            linkLabel1.TabStop = false; // won't set to false in the Designer.cs for weird reason

            // Load Config File
            vimageConfig = new Config();
            vimageConfig.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"));

            // Set Values
            checkBox_OpenAtMousePosition.Checked = vimageConfig.Setting_OpenAtMousePosition;
            checkBox_SmoothingDefault.Checked = vimageConfig.Setting_SmoothingDefault;
            checkBox_BackgroundForImagesWithTransparencyDefault.Checked = vimageConfig.Setting_BackgroundForImagesWithTransparencyDefault;
            checkBox_LimitImagesToMonitorHeight.Checked = vimageConfig.Setting_LimitImagesToMonitorHeight;
            checkBox_PositionLargeWideImagesInCorner.Checked = vimageConfig.Setting_PositionLargeWideImagesInCorner;
            checkBox_PreloadNextImage.Checked = vimageConfig.Setting_PreloadNextImage;
            checkBox_OpenSettingsEXE.Checked = vimageConfig.Setting_OpenSettingsEXE;
            numericUpDown_MinImageSize.Value = vimageConfig.Setting_MinImageSize;
            numericUpDown_SmoothingMinImageSize.Value = vimageConfig.Setting_SmoothingMinImageSize;

            textBox_Drag.Text = Config.ControlsToString(vimageConfig.Control_Drag);
            textBox_Close.Text = Config.ControlsToString(vimageConfig.Control_Close);
            textBox_OpenContextMenu.Text = Config.ControlsToString(vimageConfig.Control_OpenContextMenu);
            textBox_PrevImage.Text = Config.ControlsToString(vimageConfig.Control_PrevImage);
            textBox_NextImage.Text = Config.ControlsToString(vimageConfig.Control_NextImage);
            textBox_RotateClockwise.Text = Config.ControlsToString(vimageConfig.Control_RotateClockwise);
            textBox_RotateAntiClockwise.Text = Config.ControlsToString(vimageConfig.Control_RotateAntiClockwise);
            textBox_Flip.Text = Config.ControlsToString(vimageConfig.Control_Flip);
            textBox_FitToMonitorHeight.Text = Config.ControlsToString(vimageConfig.Control_FitToMonitorHeight);
            textBox_FitToMonitorHeightAlternative.Text = Config.ControlsToString(vimageConfig.Control_FitToMonitorHeightAlternative);
            textBox_ZoomFaster.Text = Config.ControlsToString(vimageConfig.Control_ZoomFaster);
            textBox_ZoomAlt.Text = Config.ControlsToString(vimageConfig.Control_ZoomAlt);
            textBox_ToggleSmoothing.Text = Config.ControlsToString(vimageConfig.Control_ToggleSmoothing);
            textBox_ToggleBackgroundForTransparency.Text = Config.ControlsToString(vimageConfig.Control_ToggleBackgroundForTransparency);
            textBox_ToggleAlwaysOnTop.Text = Config.ControlsToString(vimageConfig.Control_ToggleAlwaysOnTop);
            textBox_PauseAnimation.Text = Config.ControlsToString(vimageConfig.Control_PauseAnimation);
            textBox_PrevFrame.Text = Config.ControlsToString(vimageConfig.Control_PrevFrame);
            textBox_NextFrame.Text = Config.ControlsToString(vimageConfig.Control_NextFrame);
            textBox_OpenConfig.Text = Config.ControlsToString(vimageConfig.Control_OpenConfig);
            textBox_ReloadConfig.Text = Config.ControlsToString(vimageConfig.Control_ReloadConfig);
            textBox_ResetImage.Text = Config.ControlsToString(vimageConfig.Control_ResetImage);
            textBox_OpenAtLocation.Text = Config.ControlsToString(vimageConfig.Control_OpenAtLocation);
            textBox_Delete.Text = Config.ControlsToString(vimageConfig.Control_Delete);
            textBox_OpenDuplicateImage.Text = Config.ControlsToString(vimageConfig.Control_OpenDuplicateImage);

            checkBox_ContextMenuShowMargin.Checked = vimageConfig.ContextMenuShowMargin;
            numericUpDown_ContextMenu_Animation_InsertAtIndex.Value = vimageConfig.ContextMenu_Animation_InsertAtIndex;
            textBox_ContextMenu.Text = vimageConfig.ContextMenuSetup;
            // todo : Context Menu Customisation - drag and drop system?
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            // Update Values
            vimageConfig.Setting_OpenAtMousePosition = checkBox_OpenAtMousePosition.Checked;
            vimageConfig.Setting_SmoothingDefault = checkBox_SmoothingDefault.Checked;
            vimageConfig.Setting_BackgroundForImagesWithTransparencyDefault = checkBox_BackgroundForImagesWithTransparencyDefault.Checked;
            vimageConfig.Setting_LimitImagesToMonitorHeight = checkBox_LimitImagesToMonitorHeight.Checked;
            vimageConfig.Setting_PositionLargeWideImagesInCorner = checkBox_PositionLargeWideImagesInCorner.Checked;
            vimageConfig.Setting_PreloadNextImage = checkBox_PreloadNextImage.Checked;
            vimageConfig.Setting_OpenSettingsEXE = checkBox_OpenSettingsEXE.Checked;
            vimageConfig.Setting_MinImageSize = (int)numericUpDown_MinImageSize.Value;
            vimageConfig.Setting_SmoothingMinImageSize = (int)numericUpDown_SmoothingMinImageSize.Value;

            vimageConfig.ContextMenuShowMargin = checkBox_ContextMenuShowMargin.Checked;
            vimageConfig.ContextMenu_Animation_InsertAtIndex = (int)numericUpDown_ContextMenu_Animation_InsertAtIndex.Value;

            vimageConfig.ContextMenuSetup = textBox_ContextMenu.Text;

            // Save Config File
            vimageConfig.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"));
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://torrunt.net/vimage");
        }

        private void button_ContextMenuDefault_Click(object sender, EventArgs e)
        {
            // Reset Context Menu Setup to Default
            vimageConfig.ContextMenuSetup = vimageConfig.ContextMenuSetupDefault;

            textBox_ContextMenu.Text = vimageConfig.ContextMenuSetup;
        }

        private void control_OnKeyDown(object sender, KeyEventArgs e)
        {
            // Record Key Press
            string name = ((TextBox)sender).Name.Replace("textBox_", "");
            string key = e.KeyCode.ToString().ToUpper();

            if (key.Equals("SCROLL") || key.Equals("NUMLOCK") || key.Equals("CAPITAL") ||
                key.Equals("LWIN") || key.Equals("RWIN"))
                return;

            // fix up some weird names KeyEventArgs gives
            switch (key)
            {
                case "OEMOPENBRACKETS": key = "["; break;
                case "OEM6": key = "]"; break;
                case "OEM5": key = "\\"; break;
                case "OEM1": key = ";"; break;
                case "OEM7": key = "'"; break;
            }

            key = key.Replace("OEM", ""); // eg: OEMTILDE to TILDE
            key = key.Replace("KEY", ""); // eg: CONTROLKEY to CONTROL

            // Update Control and Text Box
            List<int> Control = vimageConfig.UpdateControl(name, (int)Config.StringToKey(key));
            if (Control != null)
                ((TextBox)sender).Text = Config.ControlsToString(Control);
        }
        private void control_OnMouseUp(object sender, MouseEventArgs e)
        {
            if (TextBoxWithMouseFocus != (TextBox)sender)
            {
                TextBoxWithMouseFocus = (TextBox)sender;
                return;
            }

            // Record Mouse Button Press
            string name = ((TextBox)sender).Name.Replace("textBox_", "");
            string button = e.Button.ToString().ToUpper();

            // Update Control and Text Box
            List<int> Control = vimageConfig.UpdateControl(name, Config.StringToMouseButton("MOUSE" + button));
            if (Control != null)
                ((TextBox)sender).Text = Config.ControlsToString(Control);
        }

        private void control_Clear(object sender, EventArgs e)
        {
            // Clear Control
            string name = ((Button)sender).Name.Replace("button_Clear_", "");

            ((TextBox)Controls.Find("textBox_" + name, true)[0]).Clear();
            vimageConfig.UpdateControl(name, -1);
        }

        private void control_OnLoseFocus(object sender, EventArgs e)
        {
            TextBoxWithMouseFocus = null;
        }

    }
}
