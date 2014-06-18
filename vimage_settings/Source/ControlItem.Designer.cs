namespace vimage_settings
{
    partial class ControlItem
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
            this.components = new System.ComponentModel.Container();
            this.button_Clear = new System.Windows.Forms.Button();
            this.label_Name = new System.Windows.Forms.Label();
            this.textBox_Binding = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Clear
            // 
            this.button_Clear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Clear.Location = new System.Drawing.Point(320, 2);
            this.button_Clear.Margin = new System.Windows.Forms.Padding(3, 2, 4, 1);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(41, 22);
            this.button_Clear.TabIndex = 7;
            this.button_Clear.TabStop = false;
            this.button_Clear.Text = "Clear";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // label_Name
            // 
            this.label_Name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Name.Location = new System.Drawing.Point(3, 0);
            this.label_Name.Name = "label_Name";
            this.label_Name.Size = new System.Drawing.Size(194, 25);
            this.label_Name.TabIndex = 6;
            this.label_Name.Text = "Control";
            this.label_Name.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox_Binding
            // 
            this.textBox_Binding.ContextMenuStrip = this.contextMenuStrip1;
            this.textBox_Binding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Binding.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.textBox_Binding.Location = new System.Drawing.Point(203, 3);
            this.textBox_Binding.Name = "textBox_Binding";
            this.textBox_Binding.ReadOnly = true;
            this.textBox_Binding.Size = new System.Drawing.Size(111, 20);
            this.textBox_Binding.TabIndex = 5;
            this.textBox_Binding.TabStop = false;
            this.textBox_Binding.KeyDown += new System.Windows.Forms.KeyEventHandler(this.control_OnKeyDown);
            this.textBox_Binding.Leave += new System.EventHandler(this.control_OnLoseFocus);
            this.textBox_Binding.MouseUp += new System.Windows.Forms.MouseEventHandler(this.control_OnMouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.Controls.Add(this.button_Clear, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox_Binding, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_Name, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(365, 25);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // ControlItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ControlItem";
            this.Size = new System.Drawing.Size(365, 25);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Label label_Name;
        private System.Windows.Forms.TextBox textBox_Binding;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;

    }
}
