using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TempDetect
{
    public partial class DataConnectForm : Form
    {

        public DataConnectForm()
        {
            InitializeComponent();
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (tbx_IP.Text == string.Empty || tbx_IP.Text == null)
            {
                tbx_IP.Focus();
                toolTip1.Show("主机名或IP不能为空。", tbx_IP, 0, -15, 1000);
                return;
            }

            if (tbx_BaseName.Text == string.Empty || tbx_BaseName.Text == null)
            {
                tbx_BaseName.Focus();
                toolTip1.Show("数据库名称不能为空。", tbx_BaseName, 0, -15, 1000);
                return;
            }

            if (tbx_UserName.Text == string.Empty || tbx_UserName.Text == null)
            {
                tbx_UserName.Focus();
                toolTip1.Show("用户名不能为空。", tbx_UserName, 0, -15, 1000);
                return;
            }

            if (tbx_PassWord.Text == string.Empty || tbx_PassWord.Text == null)
            {
                tbx_PassWord.Focus();
                toolTip1.Show("密码不能为空。", tbx_PassWord, 0, -15, 1000);
                return;
            }

            if (tbx_Table.Text == string.Empty || tbx_Table.Text == null)
            {
                tbx_Table.Focus();
                toolTip1.Show("密码不能为空。", tbx_Table, 0, -15, 1000);
                return;
            }

            DataForm dataForm = new DataForm();            
            dataForm.Server = tbx_IP.Text.Trim();
            dataForm.DataBase = tbx_BaseName.Text.Trim();
            dataForm.UserName = tbx_UserName.Text.Trim();
            dataForm.PassWord = tbx_PassWord.Text.Trim();
            dataForm.Port = tbx_Port.Text.Trim();
            dataForm.TableName = tbx_Table.Text.Trim();
            if (dataForm.Connect())
            {
                this.Hide();
                dataForm.ShowDialog();
                this.Dispose();   
            }
            else
            {
                MessageBox.Show("数据库连接失败，请检查参数","提示信息");
            }
        }
    }
}
