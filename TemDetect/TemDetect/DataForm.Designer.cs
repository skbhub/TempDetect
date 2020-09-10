namespace TempDetect
{
    partial class DataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataForm));
            this.datatable = new ContorlsLibrary.UserControls.Table();
            this.SuspendLayout();
            // 
            // datatable
            // 
            this.datatable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datatable.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.datatable.database = null;
            this.datatable.EndTime = new System.DateTime(((long)(0)));
            this.datatable.Location = new System.Drawing.Point(3, 12);
            this.datatable.MainTableName = null;
            this.datatable.Name = "datatable";
            this.datatable.password = null;
            this.datatable.port = null;
            this.datatable.server = null;
            this.datatable.Size = new System.Drawing.Size(912, 526);
            this.datatable.StartTime = new System.DateTime(((long)(0)));
            this.datatable.TabIndex = 0;
            this.datatable.user = null;
            // 
            // DataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 527);
            this.Controls.Add(this.datatable);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataForm";
            this.Text = "数据报表";
            this.Load += new System.EventHandler(this.DataForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ContorlsLibrary.UserControls.Table datatable;
    }
}