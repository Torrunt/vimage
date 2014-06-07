using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using vimage;

namespace vimage_settings
{
    public partial class ConfigWindow : Form
    {
        private Config vimageConfig;

        public ConfigWindow()
        {
            InitializeComponent();

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
            // todo : name reset of control text boxes and clear buttons then set them here

            checkBox_ContextMenuShowMargin.Checked = vimageConfig.ContextMenuShowMargin;
            numericUpDown_ContextMenu_Animation_InsertAtIndex.Value = vimageConfig.ContextMenu_Animation_InsertAtIndex;
            // todo : Context Menu Customisation - drag and drop system? or just use a simple text box for now?


            // todo : Click/Change events for every button/tickbox/numericUpDown
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            vimageConfig.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"));
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://torrunt.net/vimage");
        }
    }
}
