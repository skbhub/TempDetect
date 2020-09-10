using Base;
using SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;


namespace TempDetect
{


    public partial class ROIFFrm : Form
    {

        private const int COLUMN_X = 0;
        private const int COLUMN_Y = 1;

        private static Pen BorderPen = new Pen(Color.Black, 3.0F);
        private static Pen PadPen = new Pen(Color.White, 1.0F);
        private static Brush MaskBursh = new SolidBrush(Color.White);
        private static List<Regions> _RegionList;
        Regions region;

        private static string _XmlPath;
        private  string _ProID;



        private ShapeType _ShapeType;
        private List<GraphicsPoint> _ShapePoints;
        private bool _IsPainting;
        private float _WidthRatio;
        private float _HeightRatio;
        private int _ImageWidth;
        private int _ImageHeight;

        private Point _Spot;
        private List<Point> _PointList;
        private byte[] _Mask;


        public Point Spot
        {
            get
            {
                return _Spot;
            }

            set
            {
                _Spot = value;
            }
        }

        public List<Point> PointList
        {
            get
            {
                return _PointList;
            }

            set
            {
                _PointList = value;
            }
        }

        public byte[] Mask
        {
            get
            {
                return _Mask;
            }

            set
            {
                _Mask = value;
            }
        }

        public bool IsPainting
        {
            get
            {
                return _IsPainting;
            }

            set
            {
                _IsPainting = value;
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

        public int ImageWidth
        {
            get
            {
                return _ImageWidth;
            }

            set
            {
                _ImageWidth = value;
                WidthRatio = (float)tpbTA.Width / _ImageWidth;
            }
        }

        public int ImageHeight
        {
            get
            {
                return _ImageHeight;
            }

            set
            {
                _ImageHeight = value;
                HeightRatio = (float)tpbTA.Height / _ImageHeight;
            }
        }

        public List<GraphicsPoint> ShapePoints
        {
            get
            {
                return _ShapePoints;
            }

            set
            {
                _ShapePoints = value;
            }
        }

        public ShapeType ShapeType
        {
            get
            {
                return _ShapeType;
            }

            set
            {
                _ShapeType = value;
                SetShapeType();
            }
        }

        public static List<Regions> RegionList
        {
            get
            {
                return _RegionList;
            }

            set
            {
                _RegionList = value;
            }
        }

        public static string XmlPath
        {
            get
            {
                return _XmlPath;
            }

            set
            {
                _XmlPath = value;
            }
        }

        public  string ProID
        {
            get
            {
                return _ProID;
            }

            set
            {
                _ProID = value;
            }
        }

        public ROIFFrm(OWBCamera camera)
        {
            InitializeComponent();
            OWBGlobal.Camera = camera;  
            ImageWidth = OWBGlobal.Camera.Width;
            ImageHeight = OWBGlobal.Camera.Height;
            ShapePoints = new List<GraphicsPoint>();
            RefreshForm();
        }

        private void SetShapeType()
        {
            tsbArrow.Checked = false;
            tsbPoint.Checked = false;
            tsbPolygon.Checked = false;
            tsbLine.Checked = false;

            switch (ShapeType)
            {
                case ShapeType.None:
                    tsbArrow.Checked = true;
                    break;
                case ShapeType.Spot:
                    tsbPoint.Checked = true;
                    break;
                case ShapeType.Polygon:
                    tsbPolygon.Checked = true;
                    break;
                case ShapeType.Line:
                    tsbLine.Checked = true;
                    break;

            }
            ShapePoints.Clear();
            Draw();
        }

        private void Draw()
        {
            Action<Graphics> draw = g =>
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;


                if (IsPainting)
                {
                    switch (ShapeType)
                    {
                        case ShapeType.Spot:
                            if (ShapePoints.Count == 0) return;
                            int rx = (int)(ShapePoints[0].Point.X);
                            int ry = (int)(ShapePoints[0].Point.Y);
                            g.DrawLine(BorderPen, rx, ry - 8, rx, ry + 8);
                            g.DrawLine(PadPen, rx, ry - 8, rx, ry + 8);
                            g.DrawLine(BorderPen, rx - 8, ry, rx + 8, ry);
                            g.DrawLine(PadPen, rx - 8, ry, rx + 8, ry);
                            break;
                        case ShapeType.Polygon:
                            if (ShapePoints.Count == 0) return;
                            if (ShapePoints.Count <= 2)
                            {
                                g.DrawLines(BorderPen, ShapePoints.Select(p => p.Point).ToArray());
                                g.DrawLines(PadPen, ShapePoints.Select(p => p.Point).ToArray());
                            }
                            else
                            {
                                g.DrawPolygon(BorderPen, ShapePoints.Select(p => p.Point).ToArray());
                                g.DrawPolygon(PadPen, ShapePoints.Select(p => p.Point).ToArray());
                            }
                            break;
                        case ShapeType.Mask:
                            if (ShapePoints.Count == 0) return;
                            using (var path = new GraphicsPath())
                            {
                                path.AddLines(ShapePoints.Select(p => p.Point).ToArray());
                                if (IsPainting)
                                {
                                    g.FillPath(MaskBursh, path);
                                }
                            }
                            break;
                        case ShapeType.Line:
                            if (ShapePoints.Count == 0) return;
                            g.DrawLines(BorderPen, ShapePoints.Select(p => p.Point).ToArray());
                            g.DrawLines(PadPen, ShapePoints.Select(p => p.Point).ToArray());
                            break;
                    }

                }
            };

            tpbTA.Rander(draw);
        }

        private void tpbTA_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsPainting = true;
                ShapePoints.Add(new GraphicsPoint { IsTemp = false, Point = e.Location });
                if (ShapeType == ShapeType.Spot)
                {
                    Draw();
                }
            }
        }

        private void tpbTA_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsPainting)
            {
                switch (ShapeType)
                {
                    case ShapeType.Polygon:
                    case ShapeType.Line:
                        if (ShapePoints.Count == 0)
                        {
                            return;
                        }
                        GraphicsPoint lastPoint = ShapePoints.Last();

                        if (lastPoint.IsTemp)
                        {
                            ShapePoints.Remove(lastPoint);
                        }
                        ShapePoints.Add(new GraphicsPoint { IsTemp = true, Point = e.Location });
                        Draw();
                        break;
                    case ShapeType.Mask:
                        ShapePoints.Add(new GraphicsPoint { IsTemp = false, Point = e.Location });
                        Draw();
                        break;
                }
            }
        }

        private void tpbTA_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Func<Point, Point> tranFunc = (p) =>
            {
                return new Point((int)(p.X / WidthRatio), (int)(p.Y / HeightRatio));
            };
            if (IsPainting)
            {
                IsPainting = false;

                switch (ShapeType)
                {
                    case ShapeType.Spot:
                        Spot = tranFunc(ShapePoints[0].Point);
                        region.Point = DeepCopyByXml<GraphicsPoint>(ShapePoints[0]);
                        region.WidthRatio = WidthRatio;
                        region.HeightRatio = HeightRatio;
                        RegionList.Add(region);
                        break;
                    case ShapeType.Polygon:
                    case ShapeType.Line:
                        region.Polygon = new List<GraphicsPoint>(ShapePoints);
                        region.WidthRatio = WidthRatio;
                        region.HeightRatio = HeightRatio;
                        RegionList.Add(region);
                        PointList = ShapePoints.Where(p => !p.IsTemp).Select(p => tranFunc(p.Point)).ToList();
                        break;

                }
                if (ShapeType != ShapeType.None)
                {
                    if (string.IsNullOrEmpty(cbxMarkerName.Text.Trim()))
                    {
                        ttISPAddForm.Show("标记名不能为空！", cbxMarkerName, 0, -15, 1000);
                        return;
                    }
                    if (Regex.IsMatch(cbxMarkerName.Text.Trim(), @"[\u4e00-\u9fa5]"))
                    {
                        cbxMarkerName.Focus();
                        ttISPAddForm.Show("标记名不能为中文！", cbxMarkerName, 0, -15, 1000);
                        return;
                    }
                    if (cbAlarm.Checked)
                    {
                        if (!Regex.IsMatch(tbAlarm.Text.Trim(), @"^([1-9][0-9]*)+(.[0-9]{1,2})?$"))
                        {
                            tbAlarm.Focus();
                            ttISPAddForm.Show("温度报警阈值必须为数字！", tbAlarm, 0, -15, 1000);
                            return;
                        }
                    }
                    
                    Put2Cam();
                    RefreshForm();
                }
            }
            ShapeType = ShapeType.None;

        }

        //深拷贝
        public static T DeepCopyByXml<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }

        private void tsbArrow_Click(object sender, EventArgs e)
        {
            ShapeType = ShapeType.None;

        }

        private void tsbPoint_Click(object sender, EventArgs e)
        {
            ShapeType = ShapeType.Spot;
        }

        private void tsbPolygon_Click(object sender, EventArgs e)
        {
            ShapeType = ShapeType.Polygon;
        }

        private void tsbLine_Click(object sender, EventArgs e)
        {
            ShapeType = ShapeType.Line;
        }

        private void cmsISP_Opening(object sender, CancelEventArgs e)
        {
            tsmiDel.DropDownItems.Clear();
            List<MarkerItem> markerItems = OWBGlobal.Camera.GetMarkerItems();
            if (markerItems != null)
            {
                foreach (MarkerItem markerItem in markerItems)
                {
                    ToolStripMenuItem tsiDelItem = new ToolStripMenuItem();
                    tsiDelItem.Text = markerItem.ToString();
                    tsiDelItem.Tag = markerItem;
                    tsiDelItem.Click += new EventHandler(tsmiDelItem_Click);
                    tsmiDel.DropDownItems.Add(tsiDelItem);
                }
            }
        }

        private void tsmiDelItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsiDelItem = sender as ToolStripMenuItem;
            MarkerItem markerItem = tsiDelItem.Tag as MarkerItem;
        

            if (MessageBox.Show("是否删除测温标记\"" + markerItem.MarkerName + "\"", "信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (RegionList.Count>0)
                {
                    for (int i = 0; i < RegionList.Count; i++)
                    {
                        if (RegionList[i].ID.ToString()==markerItem.MarkerName)
                        {
                            RegionList.RemoveAt(i);
                        }
                    }
                       
                    
                }
               

                switch (markerItem.MarkerType)
                {
                    case MarkerType.Spot:
                        OWBGlobal.Camera.DelPoint(markerItem.MarkerName);
                        break;
                    case MarkerType.Region:
                        OWBGlobal.Camera.DelRegion(markerItem.MarkerName);
                        break;
                    case MarkerType.Line:
                        OWBGlobal.Camera.DelLine(markerItem.MarkerName);
                        break;
                }
            }
            RefreshForm();
        }

        private void DelMarkers(List<MarkerItem> markerItems)
        {
            foreach (MarkerItem markerItem in markerItems)
            {
                switch (markerItem.MarkerType)
                {
                    case MarkerType.Spot:
                        OWBGlobal.Camera.DelPoint(markerItem.MarkerName);
                        break;
                    case MarkerType.Region:
                        OWBGlobal.Camera.DelRegion(markerItem.MarkerName);
                        break;
                    case MarkerType.Line:
                        OWBGlobal.Camera.DelLine(markerItem.MarkerName);
                        break;
                }
            }

        }

        private void RefreshForm()
        {
            if (OWBGlobal.Camera.IsConnected)
            {
                byte[] buf = OWBGlobal.Camera.Snapshot();
                if (buf == null)
                {
                    return;
                }
                Bitmap bmp = OWBGlobal.Camera.CreateBitmap(buf);
                tpbTA.Image = bmp;

            }
            else
            {

            }
        }

        private void Put2Cam()
        {
            switch (ShapeType)
            {
                case ShapeType.Spot:
                    OWBTypes.SpotMarker pointMarker = new OWBTypes.SpotMarker();
                    pointMarker.Api_visible = cbAPI.Checked;
                    pointMarker.Osd_visible = cbOSD.Checked;
                    pointMarker.Emission = (float)nudEmissivityValue.Value;
                    pointMarker.Distance = (float)nudDistanceValue.Value;
                    pointMarker.ReflectionTemp = (float)nudReflectedTemperatureValue.Value;
                    pointMarker.Offset = (float)nudOffsetValue.Value;
                    pointMarker.Point = new OWBTypes.Pos();
                    pointMarker.Point.X = Spot.X;
                    pointMarker.Point.Y = Spot.Y;
                    pointMarker.Label = cbxMarkerName.Text.Trim();
                    if (OWBGlobal.Camera.PutPoint(cbxMarkerName.Text.Trim(), pointMarker))
                    {

                        if (cbAlarm.Checked)
                        {
                            OWBGlobal.Camera.PutAlarm(cbxMarkerName.Text.Trim(), MarkerType.Spot, new OWBTypes.Alarm { Value = float.Parse(tbAlarm.Text.Trim()) });
                        }

                    }
                    else
                    {
                        MessageBox.Show("添加点失败！");
                    }
                    break;
                case ShapeType.Polygon:
                    OWBTypes.PolygonMarker regionMarker = new OWBTypes.PolygonMarker();
                    regionMarker.Api_visible = cbAPI.Checked;
                    regionMarker.Osd_visible = cbOSD.Checked;
                    regionMarker.Emission = (float)nudEmissivityValue.Value;
                    regionMarker.Distance = (float)nudDistanceValue.Value;
                    regionMarker.ReflectionTemp = (float)nudReflectedTemperatureValue.Value;
                    regionMarker.Offset = (float)nudOffsetValue.Value;
                    regionMarker.PointList = new OWBTypes.Pos[PointList.Count];
                    regionMarker.Label = cbxMarkerName.Text.Trim();

                    for (int i = 0; i < PointList.Count; i++)
                    {
                        OWBTypes.Pos point = new OWBTypes.Pos();
                        point.X = PointList[i].X;
                        point.Y = PointList[i].Y;
                        regionMarker.PointList[i] = point;
                    }
                    if (OWBGlobal.Camera.PutRegion(cbxMarkerName.Text.Trim(), regionMarker))
                    {

                        if (cbAlarm.Checked)
                        {
                            OWBGlobal.Camera.PutAlarm(cbxMarkerName.Text.Trim(), MarkerType.Region, new OWBTypes.Alarm { Value = float.Parse(tbAlarm.Text.Trim()) });
                        }

                    }
                    else
                    {
                        MessageBox.Show("添加区域失败！");
                    }
                    break;

            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (Positiverdbtn.Checked)
            {
                if (!Directory.Exists(ProID + "\\0\\"))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(ProID + "\\0\\");
                }
                XmlPath = Path.Combine(ProID + "\\0\\", cbxwLocate.Text + ".xml");                
            }
            if (Negativerdbtn.Checked)
            {
                if (!Directory.Exists(ProID + "\\1\\")) 
                {
                    DirectoryInfo dir = Directory.CreateDirectory(ProID + "\\1\\");
                }
                XmlPath = Path.Combine(ProID + "\\1\\", cbxwLocate.Text + ".xml");
            }
            //XmlPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, cbxwLocate.Text + ".xml");
            RegionList = new List<Regions>();

        }

        private void btnSaveROI_Click(object sender, EventArgs e)
        {

            xml_serializer_manager manager = new xml_serializer_manager();
            manager.serialize_to_xml(XmlPath, RegionList);
        }

        private void cbAlarm_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAlarm.Checked)
            {
                tbAlarm.Enabled = true;
            }
            else
            {
                tbAlarm.Enabled = false;
            }
        }

        private void ROIFFrm_Load(object sender, EventArgs e)
        {
            //for (int i = 1; i <= 30; i++)
            //{
            //    for (int j = 1; j <= 30; j++)
            //    {
            //        this.cbxMarkerName.Items.Add(i.ToString() + "0" + j.ToString());
            //    }
            //}
        }

        private void cbxMarkerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxMarkerName.Text!=string.Empty)
            {
                region = new Regions();
                region.ID = Convert.ToInt32(cbxMarkerName.Text);
            }
        }

        private void tbAlarm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (tbAlarm.Text.Trim() != "")
                {
                    region.Threshold = Convert.ToDouble(tbAlarm.Text.Trim());
                }
                tbAlarm.Text = "";
                cbAlarm.Checked = false;
            }

        }

        private void cbxwLocate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OWBGlobal.Camera.IsConnected)
            {
                List<MarkerItem> markeritems = OWBGlobal.Camera.GetMarkerItems();
                DelMarkers(markeritems);
            }
            RefreshForm();

        }
    }
    [Serializable]
    public class GraphicsPoint
    {
        public Point Point { get; set; }

        public bool IsTemp { get; set; }
    }
}
