namespace TempDetect
{
    partial class ImgShow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImgShow));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblFileTempValue = new System.Windows.Forms.Label();
            this.lblFileLocValue = new System.Windows.Forms.Label();
            this.pbCamera = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCamera)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblFileTempValue);
            this.panel1.Controls.Add(this.lblFileLocValue);
            this.panel1.Controls.Add(this.pbCamera);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(860, 693);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(197, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "温度：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "位置：";
            // 
            // lblFileTempValue
            // 
            this.lblFileTempValue.AutoSize = true;
            this.lblFileTempValue.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFileTempValue.Location = new System.Drawing.Point(258, 7);
            this.lblFileTempValue.Name = "lblFileTempValue";
            this.lblFileTempValue.Size = new System.Drawing.Size(40, 19);
            this.lblFileTempValue.TabIndex = 3;
            this.lblFileTempValue.Text = "0℃";
            // 
            // lblFileLocValue
            // 
            this.lblFileLocValue.AutoSize = true;
            this.lblFileLocValue.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFileLocValue.Location = new System.Drawing.Point(83, 7);
            this.lblFileLocValue.Name = "lblFileLocValue";
            this.lblFileLocValue.Size = new System.Drawing.Size(64, 19);
            this.lblFileLocValue.TabIndex = 3;
            this.lblFileLocValue.Text = "(0,0)";
            // 
            // pbCamera
            // 
            this.pbCamera.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pbCamera.Location = new System.Drawing.Point(3, 30);
            this.pbCamera.Name = "pbCamera";
            this.pbCamera.Size = new System.Drawing.Size(854, 660);
            this.pbCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCamera.TabIndex = 2;
            this.pbCamera.TabStop = false;
            this.pbCamera.Paint += new System.Windows.Forms.PaintEventHandler(this.pbCamera_Paint);
            this.pbCamera.MouseLeave += new System.EventHandler(this.pbCamera_MouseLeave);
            this.pbCamera.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbCamera_MouseMove);
            // 
            // ImgShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 693);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ImgShow";
            this.Text = "离线检验";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCamera)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbCamera;
        private System.Windows.Forms.Label lblFileTempValue;
        private System.Windows.Forms.Label lblFileLocValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}