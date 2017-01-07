namespace vimage_settings
{
    partial class ContextMenuItem
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
            this.comboBox_Function = new System.Windows.Forms.ComboBox();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.button_Right = new System.Windows.Forms.Button();
            this.button_Left = new System.Windows.Forms.Button();
            this.button_Down = new System.Windows.Forms.Button();
            this.button_Up = new System.Windows.Forms.Button();
            this.button_Delete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox_Function
            // 
            this.comboBox_Function.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Function.FormattingEnabled = true;
            this.comboBox_Function.Location = new System.Drawing.Point(146, 3);
            this.comboBox_Function.Name = "comboBox_Function";
            this.comboBox_Function.Size = new System.Drawing.Size(160, 21);
            this.comboBox_Function.TabIndex = 2;
            this.comboBox_Function.Click += new System.EventHandler(this.comboBox_Function_Click);
            // 
            // textBox_Name
            // 
            this.textBox_Name.Location = new System.Drawing.Point(3, 3);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(140, 20);
            this.textBox_Name.TabIndex = 1;
            this.textBox_Name.Click += new System.EventHandler(this.textBox_Name_Click);
            // 
            // button_Right
            // 
            this.button_Right.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Right.FlatAppearance.BorderSize = 0;
            this.button_Right.Image = global::vimage_settings.Properties.Resources.bullet_arrow_right;
            this.button_Right.Location = new System.Drawing.Point(355, 3);
            this.button_Right.Name = "button_Right";
            this.button_Right.Size = new System.Drawing.Size(16, 21);
            this.button_Right.TabIndex = 6;
            this.button_Right.UseVisualStyleBackColor = true;
            this.button_Right.Click += new System.EventHandler(this.button_Right_Click);
            // 
            // button_Left
            // 
            this.button_Left.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Left.FlatAppearance.BorderSize = 0;
            this.button_Left.Image = global::vimage_settings.Properties.Resources.bullet_arrow_left;
            this.button_Left.Location = new System.Drawing.Point(339, 3);
            this.button_Left.Name = "button_Left";
            this.button_Left.Size = new System.Drawing.Size(16, 21);
            this.button_Left.TabIndex = 5;
            this.button_Left.UseVisualStyleBackColor = true;
            this.button_Left.Click += new System.EventHandler(this.button_Left_Click);
            // 
            // button_Down
            // 
            this.button_Down.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Down.FlatAppearance.BorderSize = 0;
            this.button_Down.Image = global::vimage_settings.Properties.Resources.bullet_arrow_down;
            this.button_Down.Location = new System.Drawing.Point(323, 3);
            this.button_Down.Name = "button_Down";
            this.button_Down.Size = new System.Drawing.Size(16, 21);
            this.button_Down.TabIndex = 4;
            this.button_Down.UseVisualStyleBackColor = true;
            this.button_Down.Click += new System.EventHandler(this.button_Down_Click);
            // 
            // button_Up
            // 
            this.button_Up.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Up.FlatAppearance.BorderSize = 0;
            this.button_Up.Image = global::vimage_settings.Properties.Resources.bullet_arrow_up;
            this.button_Up.Location = new System.Drawing.Point(307, 3);
            this.button_Up.Name = "button_Up";
            this.button_Up.Size = new System.Drawing.Size(16, 21);
            this.button_Up.TabIndex = 3;
            this.button_Up.UseVisualStyleBackColor = true;
            this.button_Up.Click += new System.EventHandler(this.button_Up_Click);
            // 
            // button_Delete
            // 
            this.button_Delete.BackgroundImage = global::vimage_settings.Properties.Resources.cross;
            this.button_Delete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Delete.FlatAppearance.BorderSize = 0;
            this.button_Delete.Location = new System.Drawing.Point(371, 3);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(20, 21);
            this.button_Delete.TabIndex = 7;
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
            // 
            // ContextMenuItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.button_Right);
            this.Controls.Add(this.button_Left);
            this.Controls.Add(this.button_Down);
            this.Controls.Add(this.button_Up);
            this.Controls.Add(this.button_Delete);
            this.Controls.Add(this.comboBox_Function);
            this.Controls.Add(this.textBox_Name);
            this.Name = "ContextMenuItem";
            this.Size = new System.Drawing.Size(392, 25);
            this.Click += new System.EventHandler(this.ContextMenuItem_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_Function;
        private System.Windows.Forms.TextBox textBox_Name;
        private System.Windows.Forms.Button button_Delete;
        private System.Windows.Forms.Button button_Up;
        private System.Windows.Forms.Button button_Down;
        private System.Windows.Forms.Button button_Right;
        private System.Windows.Forms.Button button_Left;
    }
}
