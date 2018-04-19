namespace Chemipad
{
    partial class RenameDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenameDialog));
            this.NameBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.LeftRadio = new System.Windows.Forms.RadioButton();
            this.CenterRadio = new System.Windows.Forms.RadioButton();
            this.RightRadio = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(12, 12);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(344, 20);
            this.NameBox.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(66, 77);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(188, 77);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // LeftRadio
            // 
            this.LeftRadio.AutoSize = true;
            this.LeftRadio.Checked = true;
            this.LeftRadio.Location = new System.Drawing.Point(12, 47);
            this.LeftRadio.Name = "LeftRadio";
            this.LeftRadio.Size = new System.Drawing.Size(69, 17);
            this.LeftRadio.TabIndex = 3;
            this.LeftRadio.TabStop = true;
            this.LeftRadio.Text = "Align Left";
            this.LeftRadio.UseVisualStyleBackColor = true;
            // 
            // CenterRadio
            // 
            this.CenterRadio.AutoSize = true;
            this.CenterRadio.Location = new System.Drawing.Point(137, 47);
            this.CenterRadio.Name = "CenterRadio";
            this.CenterRadio.Size = new System.Drawing.Size(82, 17);
            this.CenterRadio.TabIndex = 4;
            this.CenterRadio.Text = "Align Center";
            this.CenterRadio.UseVisualStyleBackColor = true;
            // 
            // RightRadio
            // 
            this.RightRadio.AutoSize = true;
            this.RightRadio.Location = new System.Drawing.Point(271, 47);
            this.RightRadio.Name = "RightRadio";
            this.RightRadio.Size = new System.Drawing.Size(76, 17);
            this.RightRadio.TabIndex = 5;
            this.RightRadio.Text = "Align Right";
            this.RightRadio.UseVisualStyleBackColor = true;
            // 
            // RenameDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 112);
            this.Controls.Add(this.RightRadio);
            this.Controls.Add(this.CenterRadio);
            this.Controls.Add(this.LeftRadio);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.NameBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RenameDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rename";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RadioButton LeftRadio;
        private System.Windows.Forms.RadioButton CenterRadio;
        private System.Windows.Forms.RadioButton RightRadio;
    }
}