namespace TempDetect
{
    partial class DataConnectForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataConnectForm));
            this.label1 = new System.Windows.Forms.Label();
            this.tbx_BaseName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbx_IP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbx_Port = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbx_UserName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbx_PassWord = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.tbx_Table = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据库名称：";
            // 
            // tbx_BaseName
            // 
            this.tbx_BaseName.Location = new System.Drawing.Point(105, 38);
            this.tbx_BaseName.Name = "tbx_BaseName";
            this.tbx_BaseName.Size = new System.Drawing.Size(113, 25);
            this.tbx_BaseName.TabIndex = 0;
            this.tbx_BaseName.Text = "aa";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(241, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "主机名/IP：";
            // 
            // tbx_IP
            // 
            this.tbx_IP.Location = new System.Drawing.Point(328, 38);
            this.tbx_IP.Name = "tbx_IP";
            this.tbx_IP.Size = new System.Drawing.Size(191, 25);
            this.tbx_IP.TabIndex = 1;
            this.tbx_IP.Text = "localhost";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "端口号：";
            // 
            // tbx_Port
            // 
            this.tbx_Port.Location = new System.Drawing.Point(105, 87);
            this.tbx_Port.Name = "tbx_Port";
            this.tbx_Port.ReadOnly = true;
            this.tbx_Port.Size = new System.Drawing.Size(113, 25);
            this.tbx_Port.TabIndex = 2;
            this.tbx_Port.Text = "3306";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "用户名：";
            // 
            // tbx_UserName
            // 
            this.tbx_UserName.Location = new System.Drawing.Point(105, 129);
            this.tbx_UserName.Name = "tbx_UserName";
            this.tbx_UserName.Size = new System.Drawing.Size(113, 25);
            this.tbx_UserName.TabIndex = 4;
            this.tbx_UserName.Text = "root";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(241, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "密码：";
            // 
            // tbx_PassWord
            // 
            this.tbx_PassWord.Location = new System.Drawing.Point(328, 129);
            this.tbx_PassWord.Name = "tbx_PassWord";
            this.tbx_PassWord.PasswordChar = '*';
            this.tbx_PassWord.Size = new System.Drawing.Size(191, 25);
            this.tbx_PassWord.TabIndex = 5;
            this.tbx_PassWord.Text = "mysql123";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(373, 223);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(117, 36);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(241, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "数据源：";
            // 
            // tbx_Table
            // 
            this.tbx_Table.Location = new System.Drawing.Point(328, 87);
            this.tbx_Table.Name = "tbx_Table";
            this.tbx_Table.Size = new System.Drawing.Size(113, 25);
            this.tbx_Table.TabIndex = 3;
            // 
            // DataConnectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 323);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.tbx_PassWord);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbx_IP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbx_UserName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbx_Table);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbx_Port);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbx_BaseName);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataConnectForm";
            this.Text = "数据库连接";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbx_BaseName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbx_IP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbx_Port;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbx_UserName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbx_PassWord;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbx_Table;
    }
}