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
    public partial class ConnectForm : Form
    {
        public ConnectForm()
        {
            InitializeComponent();
        }

        public string IP
        {
            get { return txt_ip.Text.Trim(); }
            set { txt_ip.Text = value; }
        }

        private void btn_Conn1_Click(object sender, EventArgs e)
        {
            if (txt_ip.Text == string.Empty || txt_ip.Text == null)
            {
                txt_ip.Focus();
                toolTip1.Show("IP不能为空。", txt_ip, 0, -15, 1000);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
