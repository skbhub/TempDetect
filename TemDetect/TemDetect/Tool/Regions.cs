using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempDetect;

namespace Base
{
    public class Regions
    {
        private List<GraphicsPoint> _Polygon;
        private GraphicsPoint _Point;
        private double _Threshold;
        private float _WidthRatio;
        private float _HeightRatio;
        private int _ID;

        public List<GraphicsPoint> Polygon
        {
            get
            {
                return _Polygon;
            }

            set
            {
                _Polygon = value;
            }
        }

        public GraphicsPoint Point
        {
            get
            {
                return _Point;
            }

            set
            {
                _Point = value;
            }
        }

        public double Threshold
        {
            get
            {
                return _Threshold;
            }

            set
            {
                _Threshold = value;
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

        public int ID
        {
            get
            {
                return _ID;
            }

            set
            {
                _ID = value;
            }
        }

        public Regions()
        {
            Polygon = new List<GraphicsPoint>();

        }

    }
}
