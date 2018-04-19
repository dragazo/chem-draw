namespace Chemipad
{
    partial class ApplicationSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplicationSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TransparentCheck = new System.Windows.Forms.CheckBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DrawingColorDisplay = new Chemipad.ColorDisplay();
            this.BackgroundColorDisplay = new Chemipad.ColorDisplay();
            this.UnselectedColorDisplay = new Chemipad.ColorDisplay();
            this.SelectedColorDisplay = new Chemipad.ColorDisplay();
            this.DefaultsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Drawing Color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Background Color";
            // 
            // TransparentCheck
            // 
            this.TransparentCheck.AutoSize = true;
            this.TransparentCheck.Location = new System.Drawing.Point(221, 90);
            this.TransparentCheck.Name = "TransparentCheck";
            this.TransparentCheck.Size = new System.Drawing.Size(131, 17);
            this.TransparentCheck.TabIndex = 4;
            this.TransparentCheck.Text = "Transparent on Export";
            this.TransparentCheck.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(12, 268);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(92, 23);
            this.OKButton.TabIndex = 5;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(119, 268);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(116, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 202);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Selected Color";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Unselected Color";
            // 
            // DrawingColorDisplay
            // 
            this.DrawingColorDisplay.Color = System.Drawing.Color.White;
            this.DrawingColorDisplay.Location = new System.Drawing.Point(12, 25);
            this.DrawingColorDisplay.Name = "DrawingColorDisplay";
            this.DrawingColorDisplay.Size = new System.Drawing.Size(200, 20);
            this.DrawingColorDisplay.TabIndex = 7;
            // 
            // BackgroundColorDisplay
            // 
            this.BackgroundColorDisplay.Color = System.Drawing.Color.White;
            this.BackgroundColorDisplay.Location = new System.Drawing.Point(12, 88);
            this.BackgroundColorDisplay.Name = "BackgroundColorDisplay";
            this.BackgroundColorDisplay.Size = new System.Drawing.Size(200, 20);
            this.BackgroundColorDisplay.TabIndex = 8;
            // 
            // UnselectedColorDisplay
            // 
            this.UnselectedColorDisplay.Color = System.Drawing.Color.White;
            this.UnselectedColorDisplay.Location = new System.Drawing.Point(12, 155);
            this.UnselectedColorDisplay.Name = "UnselectedColorDisplay";
            this.UnselectedColorDisplay.Size = new System.Drawing.Size(200, 20);
            this.UnselectedColorDisplay.TabIndex = 9;
            // 
            // SelectedColorDisplay
            // 
            this.SelectedColorDisplay.Color = System.Drawing.Color.White;
            this.SelectedColorDisplay.Location = new System.Drawing.Point(12, 218);
            this.SelectedColorDisplay.Name = "SelectedColorDisplay";
            this.SelectedColorDisplay.Size = new System.Drawing.Size(200, 20);
            this.SelectedColorDisplay.TabIndex = 10;
            // 
            // DefaultsButton
            // 
            this.DefaultsButton.Location = new System.Drawing.Point(254, 268);
            this.DefaultsButton.Name = "DefaultsButton";
            this.DefaultsButton.Size = new System.Drawing.Size(92, 23);
            this.DefaultsButton.TabIndex = 11;
            this.DefaultsButton.Text = "Defaults";
            this.DefaultsButton.UseVisualStyleBackColor = true;
            this.DefaultsButton.Click += new System.EventHandler(this.DefaultsButton_Click);
            // 
            // ApplicationSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 303);
            this.Controls.Add(this.DefaultsButton);
            this.Controls.Add(this.SelectedColorDisplay);
            this.Controls.Add(this.UnselectedColorDisplay);
            this.Controls.Add(this.BackgroundColorDisplay);
            this.Controls.Add(this.DrawingColorDisplay);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.TransparentCheck);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ApplicationSettings";
            this.Text = "Application Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox TransparentCheck;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private ColorDisplay DrawingColorDisplay;
        private ColorDisplay BackgroundColorDisplay;
        private ColorDisplay UnselectedColorDisplay;
        private ColorDisplay SelectedColorDisplay;
        private System.Windows.Forms.Button DefaultsButton;
    }
}