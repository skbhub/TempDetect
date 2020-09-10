namespace TempDetect
{
    partial class CheckForm
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
            this.ccd1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ccd1)).BeginInit();
            this.SuspendLayout();
            // 
            // ccd1
            // 
            this.ccd1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ccd1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ccd1.Location = new System.Drawing.Point(0, 0);
            this.ccd1.Name = "ccd1";
            this.ccd1.Size = new System.Drawing.Size(608, 516);
            this.ccd1.TabIndex = 0;
            this.ccd1.TabStop = false;
            // 
            // CheckForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 516);
            this.Controls.Add(this.ccd1);
            this.Name = "CheckForm";
            this.Text = "漏光查看";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CheckForm_FormClosed);
            this.Load += new System.EventHandler(this.CheckForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ccd1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ccd1;
    }
}