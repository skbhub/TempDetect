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
    public partial class CheckForm : Form
    {
        private int ID;
        private List<string> Cameralist;
        private MVCamera Camera = new MVCamera();

        public CheckForm(int id)
        {
            MVCamera.EnumDevice(ref Cameralist);
            InitializeComponent();
            ID = id;


        }

        private void CheckForm_Load(object sender, EventArgs e)
        {
            Camera.OpenDevice(Cameralist[ID]);
            Camera.StartGrab(ccd1.Handle);
        }

        private void CheckForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Camera.StopGrab();
            Camera.CloseDevice();
        }
    }
}
