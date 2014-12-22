namespace vimage_settings
{
    partial class FileAssociationItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_Name = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_Associate = new System.Windows.Forms.Button();
            this.lbl_AssociatedWith = new System.Windows.Forms.Label();
            this.pibTickIcon = new System.Windows.Forms.PictureBox();
            this.pibIcon = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pibTickIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pibIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // label_Name
            // 
            this.label_Name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Name.Location = new System.Drawing.Point(23, 3);
            this.label_Name.Margin = new System.Windows.Forms.Padding(3);
            this.label_Name.Name = "label_Name";
            this.label_Name.Size = new System.Drawing.Size(69, 19);
            this.label_Name.TabIndex = 6;
            this.label_Name.Text = ".ext files";
            this.label_Name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.Controls.Add(this.btn_Associate, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_Name, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbl_AssociatedWith, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.pibTickIcon, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.pibIcon, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(365, 25);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // btn_Associate
            // 
            this.btn_Associate.Location = new System.Drawing.Point(271, 3);
            this.btn_Associate.Name = "btn_Associate";
            this.btn_Associate.Size = new System.Drawing.Size(69, 19);
            this.btn_Associate.TabIndex = 8;
            this.btn_Associate.Text = "Associate";
            this.btn_Associate.UseVisualStyleBackColor = true;
            this.btn_Associate.Click += new System.EventHandler(this.btn_Associate_Click);
            // 
            // lbl_AssociatedWith
            // 
            this.lbl_AssociatedWith.AutoSize = true;
            this.lbl_AssociatedWith.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_AssociatedWith.Location = new System.Drawing.Point(98, 3);
            this.lbl_AssociatedWith.Margin = new System.Windows.Forms.Padding(3);
            this.lbl_AssociatedWith.Name = "lbl_AssociatedWith";
            this.lbl_AssociatedWith.Size = new System.Drawing.Size(167, 19);
            this.lbl_AssociatedWith.TabIndex = 7;
            this.lbl_AssociatedWith.Text = "Associated with Program";
            this.lbl_AssociatedWith.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pibTickIcon
            // 
            this.pibTickIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pibTickIcon.Image = global::vimage_settings.Properties.Resources.tick;
            this.pibTickIcon.Location = new System.Drawing.Point(346, 3);
            this.pibTickIcon.Name = "pibTickIcon";
            this.pibTickIcon.Size = new System.Drawing.Size(16, 19);
            this.pibTickIcon.TabIndex = 9;
            this.pibTickIcon.TabStop = false;
            // 
            // pibIcon
            // 
            this.pibIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pibIcon.Location = new System.Drawing.Point(2, 2);
            this.pibIcon.Margin = new System.Windows.Forms.Padding(2);
            this.pibIcon.Name = "pibIcon";
            this.pibIcon.Size = new System.Drawing.Size(16, 21);
            this.pibIcon.TabIndex = 10;
            this.pibIcon.TabStop = false;
            // 
            // FileAssociationItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "FileAssociationItem";
            this.Size = new System.Drawing.Size(365, 25);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pibTickIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pibIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label_Name;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbl_AssociatedWith;
        private System.Windows.Forms.Button btn_Associate;
        private System.Windows.Forms.PictureBox pibTickIcon;
        private System.Windows.Forms.PictureBox pibIcon;

    }
}
