using System;
using System.Drawing;
using System.Windows.Forms;

namespace vimage_settings
{
    public partial class FileAssociationItem : UserControl
    {
        private string Extension;
        private FileAssociation Association;

        public FileAssociationItem(string extension)
        {
            InitializeComponent();
            
            Dock = DockStyle.Top;
            Extension = extension;
            Association = new FileAssociation(extension);

            label_Name.Text = extension + " files";

            UpdateKnowledge();
        }

        private void UpdateKnowledge()
        {
            // Declare the program associated if vimage is set to open it.

            if (Association.UserExecutablePath == Program.vimagePath)
            {
                SetAssociatedState(true);
                lbl_AssociatedWith.Text = "Associated with vimage! :)";
            }
            else
            {
                SetAssociatedState(false);

                if (Association.Registered)
                {
                    lbl_AssociatedWith.Text = "Associated with " + System.IO.Path.GetFileName(Association.UserExecutablePath);
                }
                else
                {
                    lbl_AssociatedWith.Text = "Not associated with anything.";
                }
            }
            
            SetIcon(SharpGMad.FileAssocation.GetIcon(Extension));
        }

        private void SetAssociatedState(bool associated)
        {
            if (associated)
            {
                // Associated extensions should
                // NOT have the 'Associate' button shown
                tableLayoutPanel1.ColumnStyles[3].Width = 0;
                btn_Associate.Enabled = false;
                btn_Associate.Visible = false;

                //     have the tick icon shown
                tableLayoutPanel1.ColumnStyles[4].Width = 18;
                pibTickIcon.Visible = true;
            }
            else
            {
                // Not associated extensions should
                //     have the 'Associate' button shown
                tableLayoutPanel1.ColumnStyles[3].Width = 75;
                btn_Associate.Enabled = Program.IsAdministrator;
                btn_Associate.Visible = true;

                // NOT have the tick icon shown
                tableLayoutPanel1.ColumnStyles[4].Width = 0;
                pibTickIcon.Visible = false;
            }
        }

        private void SetIcon(Icon icon)
        {
            if (icon != null)
            {
                // If an icon is set, make it appear in the row
                pibIcon.Image = icon.ToBitmap();
                pibIcon.Visible = true;
            }
            else
            {
                // If not, have it disappear
                pibIcon.Image = default(Image);
                pibIcon.Visible = false;
            }
        }

        private void btn_Associate_Click(object sender, EventArgs e)
        {
            // Don't attempt to do anything if the user is not Administrator.
            // The registry calls would fail.
            if (!Program.IsAdministrator)
                return;
        }
    }
}
