using Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TempDetect
{
    public partial class ImgShow : Form
    {
        private int _CameraWidth;
        private int _CameraHeight;
        private float _WidthRatio;
        private float _HeightRatio;
        private Point _MouseHoverPoint;
        private float[,] _Tvalues;

        public int CameraWidth
        {
            get
            {
                return _CameraWidth;
            }

            set
            {
                _CameraWidth = value;
                WidthRatio = (float)pbCamera.Width / _CameraWidth;
            }
        }

        public int CameraHeight
        {
            get
            {
                return _CameraHeight;
            }

            set
            {
                _CameraHeight = value;
                HeightRatio = (float)pbCamera.Height / _CameraHeight;
            }
        }

        public float WidthRatio
        {
            get
            {
                return _WidthRatio;
            }

            set
            {
                _WidthRatio = value;
            }
        }

        public float HeightRatio
        {
            get
            {
                return _HeightRatio;
            }

            set
            {
                _HeightRatio = value;
            }
        }

        public Point MouseHoverPoint
        {
            get
            {
                return _MouseHoverPoint;
            }

            set
            {
                _MouseHoverPoint = value;
            }
        }

        public float[,] Tvalues
        {
            get
            {
                return _Tvalues;
            }

            set
            {
                _Tvalues = value;
            }
        }

        float imgScale = 1;
        Rectangle dest = new Rectangle();

        public ImgShow()
        {
            InitializeComponent();
            OWBGlobal.Camera = new OWBCamera();
            MouseHoverPoint = new Point(-1, -1);
            dest.Height = Height;
            dest.Width = Width;
           // this.pbCamera.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pbCamera_MouseWheel);

        }

        public void GetImg(Image img)
        {
            this.pbCamera.Image = img;
     
        }

        private void pbCamera_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int rx = (int)(MouseHoverPoint.X * WidthRatio);
            int ry = (int)(MouseHoverPoint.Y * HeightRatio);
            g.DrawLine(Pens.White, rx, ry - 4, rx, ry + 4);
            g.DrawLine(Pens.White, rx - 4, ry, rx + 4, ry);
            //g.DrawImage(pbCamera.Image, dest);
        }

        private void pbCamera_MouseWheel(object sender,MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                imgScale = 1.3f;

            }
            else
            {
                imgScale = 0.7f;
            }
            dest.X += (int)((e.X - dest.X) * (1 - imgScale));
            dest.Y += (int)((e.Y - dest.Y) * (1 - imgScale));
            dest.Width = (int)(dest.Width * imgScale);
            dest.Height = (int)(dest.Height * imgScale);
            Refresh();
            base.OnMouseWheel(e);
        }

        private void pbCamera_MouseMove(object sender, MouseEventArgs e)
        {
            OWBGlobal.Camera.TValues = Tvalues;
            CameraWidth = OWBGlobal.Camera.TValues.GetLength(0);
            CameraHeight = OWBGlobal.Camera.TValues.GetLength(1);
            Point p = new Point();
            p.X = (int)((float)e.X / WidthRatio);
            p.Y = (int)((float)e.Y / HeightRatio);
            MouseHoverPoint = p;
       
                pbCamera.Invalidate();

                string temperature = OWBGlobal.Camera.TValues[MouseHoverPoint.X, MouseHoverPoint.Y].ToString("F1") + "℃";
                if (IsDisposed || !this.IsHandleCreated)
                    return;
                lblFileLocValue.BeginInvoke((MethodInvoker)delegate ()
                {
                    lblFileLocValue.Text = "(" + MouseHoverPoint.X + "," + MouseHoverPoint.Y + ")";
                });
                lblFileTempValue.BeginInvoke((MethodInvoker)delegate ()
                {
                    lblFileTempValue.Text = temperature;
                });            
        }

        private void pbCamera_MouseLeave(object sender, EventArgs e)
        {
            MouseHoverPoint = new Point(-1, -1);
        }
    }
}
