using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TempDetect
{
    public partial class OWBPictureBox : PictureBox
    {
        private Action<Graphics> _Draw;
        public OWBPictureBox()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        public OWBPictureBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        //public Action<Graphics> Draw { get => Draw; set => Draw = value; }
        public Action<Graphics> Draw
        {
            get
            {
                return _Draw;
            }

            set
            {
                _Draw = value;
            }
        }

        public void Rander(Action<Graphics> draw)
        {
            Invalidate();
            Draw = draw;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Draw?.Invoke(pe.Graphics);
        }
    }
}

