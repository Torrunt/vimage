using System;
using System.IO;
using System.Windows.Forms;
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
            numericUpDown_MinImageSize.Value = vimageConfig.Setting_MinImageSize;
            numericUpDown_SmoothingMinImageSize.Value = vimageConfig.Setting_SmoothingMinImageSize;

            checkBox_ContextMenuShowMargin.Checked = vimageConfig.Setting_ContextMenuShowMargin;
        }

        private void button_Save_Click(object sender, EventArgs e)
        {

        }
    }
}
