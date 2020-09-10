using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TempDetect
{
  
    public partial class ProductForm : Form
    {
        public ProductForm()
        {
            InitializeComponent();
        }

        private void DeleteDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                DirectoryInfo[] childs = dir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    child.Delete(true);
                }
                dir.Delete(true);
            }
        }


        public GetProIdDel getid;

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (tbx_proID.Text!=string.Empty||tbx_proID.Text!=null)
            {
                if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory+"\\Product\\"+tbx_proID.Text))
                {
                    Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "\\Product\\"+tbx_proID.Text);
                    getid(System.AppDomain.CurrentDomain.BaseDirectory + "\\Product\\" + tbx_proID.Text);
                }
                else
                {
                    DialogResult result = MessageBox.Show("已存在相同产品型号，是否覆盖？", "提示信息", MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {
                        DeleteDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "\\Product\\" + tbx_proID.Text);
                        Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "\\Product\\" + tbx_proID.Text);
                        getid(System.AppDomain.CurrentDomain.BaseDirectory + "\\Product\\" + tbx_proID.Text);
                    }
                    else
                    {                     
                        getid(System.AppDomain.CurrentDomain.BaseDirectory + "\\Product\\" + tbx_proID.Text);
                    }                           
                }         
            }
            else
            {
                tbx_proID.Focus();
                toolTip1.Show("产品型号不能为空", tbx_proID, 0, -15, 1000);               
            }
            this.Close();
        }
    }
}
