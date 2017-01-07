namespace vimage_settings
{
    partial class CustomActionItem
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.textBox_Func = new System.Windows.Forms.TextBox();
            this.button_Delete = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.25928F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.74072F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.Controls.Add(this.textBox_Name, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox_Func, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_Delete, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(400, 25);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // textBox_Name
            // 
            this.textBox_Name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Name.Location = new System.Drawing.Point(3, 0);
            this.textBox_Name.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(100, 20);
            this.textBox_Name.TabIndex = 1;
            // 
            // textBox_Func
            // 
            this.textBox_Func.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Func.Location = new System.Drawing.Point(109, 0);
            this.textBox_Func.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.textBox_Func.Name = "textBox_Func";
            this.textBox_Func.Size = new System.Drawing.Size(265, 20);
            this.textBox_Func.TabIndex = 2;
            // 
            // button_Delete
            // 
            this.button_Delete.BackgroundImage = global::vimage_settings.Properties.Resources.cross;
            this.button_Delete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Delete.FlatAppearance.BorderSize = 0;
            this.button_Delete.Location = new System.Drawing.Point(377, 0);
            this.button_Delete.Margin = new System.Windows.Forms.Padding(0);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(20, 20);
            this.button_Delete.TabIndex = 3;
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
            // 
            // CustomActionItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CustomActionItem";
            this.Size = new System.Drawing.Size(400, 25);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox_Name;
        private System.Windows.Forms.TextBox textBox_Func;
        private System.Windows.Forms.Button button_Delete;
    }
}
