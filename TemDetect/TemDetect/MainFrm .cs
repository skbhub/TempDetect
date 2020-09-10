using Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TempDetect
{
    public delegate void GetProIdDel(string id);

    public partial class MainFrm : Form
    {
        #region Fields
        private bool _isOpenFile;
        private string _FileDirectory;
        private OWBCamera _Camera1;
        private OWBCamera _Camera2;
        private Thread _TempAcqThread1;
        private bool _TempAcqDone1;
        private Thread _TempAcqThread2;
        private bool _TempAcqDone2;
        private int _CameraWidth;
        private int _CameraHeight;
        private int _ImgWidth;
        private int _ImgHeight;
        private LoginType _CameraLoginType;
        private Point _MouseHoverPoint1;
        private Point _MouseHoverPoint2;
        private float _WidthRatio1;
        private float _HeightRatio1;
        private float _WidthRatio2;
        private float _HeightRatio2;
        private string _URL;
        private string _ProId;
        private string _Data;
        #endregion

        #region Const
        PanasonicPLC PLC = new PanasonicPLC();
        ContorlsLibrary.UserControls.Sql.MySqlBase database;
        AutoResizeForm asc = new AutoResizeForm();
        private List<string> pathList = new List<string>();
        private object TimerLock1 = new object();
        private object TimerLock2 = new object();
        public Log log = null;
        bool breakflag;
        bool stopflag;
        bool runflag;
        bool E_light;
        bool AlarmFlag;
        bool S_Temp;
        bool E_Temp;
        bool InT;
        bool InL;
        bool Tfalg1;
        bool Tflag2;
        bool IsOK1;
        bool IsOK2;
        private Thread th_listen = null;
        //private Thread th_TL = null;
        //private Thread th_TR = null;
        private Thread th_Light = null;
        private Task tL = null;
        private Task tR = null;
        private static AutoResetEvent _mainEvent = new AutoResetEvent(false);
        private static AutoResetEvent _stopEvent = new AutoResetEvent(false);
        int Location = 0;
        int tmpLoc = 0;
        int Mode = 0;
        int OKCount1 = 0;
        int OKCount2 = 0;
        bool TEnd1;
        bool TEnd2;
        bool TFlag;
        bool LFlag;
        MVCamera mvCamera1;
        MVCamera mvCamera2;
        List<string> Cameralist;

        #endregion

        #region Priority
        public bool IsOpenFile
        {
            get
            {
                return _isOpenFile;
            }

            set
            {
                _isOpenFile = value;
            }
        }

        public string FileDirectory
        {
            get
            {
                return _FileDirectory;
            }

            set
            {
                _FileDirectory = value;
            }
        }

        public OWBCamera Camera1
        {
            get
            {
                return _Camera1;
            }

            set
            {
                _Camera1 = value;
            }
        }

        public OWBCamera Camera2
        {
            get
            {
                return _Camera2;
            }

            set
            {
                _Camera2 = value;
            }
        }

        public Thread TempAcqThread1
        {
            get
            {
                return _TempAcqThread1;
            }

            set
            {
                _TempAcqThread1 = value;
            }
        }

        public bool TempAcqDone1
        {
            get
            {
                return _TempAcqDone1;
            }

            set
            {
                _TempAcqDone1 = value;
            }
        }

        public Thread TempAcqThread2
        {
            get
            {
                return _TempAcqThread2;
            }

            set
            {
                _TempAcqThread2 = value;
            }
        }

        public bool TempAcqDone2
        {
            get
            {
                return _TempAcqDone2;
            }

            set
            {
                _TempAcqDone2 = value;
            }
        }

        public int CameraWidth
        {
            get
            {
                return _CameraWidth;
            }

            set
            {
                _CameraWidth = value;
                WidthRatio1 = (float)fotric1_camera.Width / _CameraWidth;
                WidthRatio2 = (float)fotric2_camera.Width / _CameraWidth;
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
                HeightRatio1 = (float)fotric1_camera.Height / _CameraHeight;
                HeightRatio2 = (float)fotric2_camera.Height / _CameraHeight;
            }
        }

        public int ImgHeight
        {
            get
            {
                return _ImgHeight;
            }

            set
            {
                _ImgHeight = value;
            }
        }

        public int ImgWidth
        {
            get
            {
                return _ImgWidth;
            }

            set
            {
                _ImgWidth = value;
            }
        }

        public LoginType CameraLoginType
        {
            get
            {
                return _CameraLoginType;
            }

            set
            {
                _CameraLoginType = value;
            }
        }

        public Point MouseHoverPoint1
        {
            get
            {
                return _MouseHoverPoint1;
            }

            set
            {
                _MouseHoverPoint1 = value;
            }
        }

        public Point MouseHoverPoint2
        {
            get
            {
                return _MouseHoverPoint2;
            }

            set
            {
                _MouseHoverPoint2 = value;
            }
        }

        private float WidthRatio1
        {
            get { return _WidthRatio1; }
            set { _WidthRatio1 = value; }
        }

        private float HeightRatio1
        {
            get { return _HeightRatio1; }
            set { _HeightRatio1 = value; }
        }

        public float WidthRatio2
        {
            get
            {
                return _WidthRatio2;
            }

            set
            {
                _WidthRatio2 = value;
            }
        }

        public float HeightRatio2
        {
            get
            {
                return _HeightRatio2;
            }

            set
            {
                _HeightRatio2 = value;
            }
        }

        public string ProId
        {
            get
            {
                return _ProId;
            }

            set
            {
                _ProId = value;
            }
        }

        public string URL
        {
            get
            {
                return _URL;
            }

            set
            {
                _URL = value;
            }
        }

        public string Data
        {
            get
            {
                return _Data;
            }

            set
            {
                _Data = value;
            }
        }
        #endregion

        public MainFrm()
        {
            mvCamera1 = new MVCamera();
            mvCamera2 = new MVCamera();
            Control.CheckForIllegalCrossThreadCalls = false;
            RefreshTemplate();
            log = new Log(System.AppDomain.CurrentDomain.BaseDirectory + "Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            database = new ContorlsLibrary.UserControls.Sql.MySqlBase("localhost", "3306", "root", "mysql123", "aa", "CX" + DateTime.Now.ToLongDateString().ToString());
            TempAcqDone1 = true;
            TempAcqThread1 = null;
            TempAcqDone2 = true;
            TempAcqThread2 = null;

            //th_TL = new Thread(LFunc);
            //th_TR = new Thread(RFunc);
            th_Light = new Thread(LightFunc);
            tL = new Task(() => LFunc());
            tR = new Task(() => RFunc());


            Tfalg1 = true;
            Tflag2 = true;
            TFlag = true;

            TEnd1 = false;
            TEnd2 = false;

            Camera1 = new OWBCamera();
            Camera2 = new OWBCamera();

            MouseHoverPoint1 = new Point(-1, -1);
            MouseHoverPoint2 = new Point(-1, -1);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrrentDomain_UnhandledException);

            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            asc.controllInitializeSize(this);
            MVCamera.EnumDevice(ref Cameralist);
            PLC.MewtocolSet("COM3", 115200, 8, 1, Parity.None);    //设置串口参数
                                                                   // PLC.MewtocolClose();                                //先关闭相应串口
            short s = PLC.MewtocolOpen();                            //再打开PLC串口通讯
            if (s == 0)
            {
                th_listen = new Thread(Listen);
                th_listen.IsBackground = true;
                th_listen.Start();

                WriteLog("PLC设备连接成功");
            }
            else
            {
                WriteLog("PLC设备连接失败");
            }
            DirectoryInfo info = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "\\Product\\");
            DirectoryInfo[] dirs = info.GetDirectories();
            if (dirs.Length > 0)
            {
                foreach (var item in dirs)
                {
                    cbxPro.Items.Add(item);
                }
                cbxPro.SelectedItem = cbxPro.Items[0];
                ProId = System.AppDomain.CurrentDomain.BaseDirectory + "Product\\" + cbxPro.Text;
            }
        }

        private void Listen()
        {
            while (true)
            {
                if (_stopEvent.WaitOne(2000))
                {
                    break;
                }
                if (TEnd1 && TEnd2)
                {
                    if (!IsOK1 || !IsOK2)
                    {
                        PLC.WriteSinglePoint("R134", true);
                        lblstatus.Text = "测温暂停...";
                        DialogResult res = MessageBox.Show("区域温度超阈值，是否中断！", "告警信息", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.Cancel)
                        {
                            PLC.WriteSinglePoint("R130", true);
                            PLC.WriteSinglePoint("R320", true);
                            lblstatus.Text = "温度检测中...";
                        }
                    }
                    else
                    {
                        PLC.WriteSinglePoint("R130", true);
                        PLC.WriteSinglePoint("R320", true);
                        lblstatus.Text = "温度检测中...";
                    }
                    TEnd1 = false;
                    TEnd2 = false;
                }

                if (!TFlag)
                {
                    _mainEvent.WaitOne();
                }
                AlarmFlag = PLC.ReadSinglePoint("RA");
                stopflag = PLC.ReadSinglePoint("R1");
                runflag = PLC.ReadSinglePoint("R0");
                breakflag = PLC.ReadSinglePoint("R2");
                S_Temp = PLC.ReadSinglePoint("R322");
                E_Temp = PLC.ReadSinglePoint("R323");
                E_light = PLC.ReadSinglePoint("R71");
                InT = PLC.ReadSinglePoint("R321");
                InL = PLC.ReadSinglePoint("R70");

                if (E_Temp)
                {
                    Tfalg1 = false;
                    Tflag2 = false;
                    lblstatus.Text = "测温完成";
                }
                if (S_Temp)
                {
                    Location = PLC.ReadSingleData("D2");
                }

                if (stopflag)
                {
                    Mode = 0;
                }
                else if (runflag)
                {
                    Mode = 1;
                }
                else if (breakflag)
                {
                    Mode = 2;
                }
            }
        }

        private void RFunc()
        {
            while (Tfalg1)
            {
                //Sleep(300);
                if (tmpLoc != Location)
                {
                    tmpLoc = Location;
                    lblWLocate1.Text = tmpLoc.ToString();
                    double threshold = 0;
                    List<MarkerItem> markers = Camera1.GetMarkerItems();
                    DelMarkers(markers, Camera1);
                    List<Regions> regions = new List<Regions>();
                    xml_serializer_manager manager = new xml_serializer_manager();
                    string path = ProId + "\\0\\";
                    float widthratio = 0;
                    float heightratio = 0;
                    if (!File.Exists(path + tmpLoc.ToString() + ".xml"))
                    {
                        MessageBox.Show("请检查温区文件是否存在", "提示消息");
                        Tfalg1 = false;
                    }
                    else
                    {
                        regions = (List<Regions>)manager.deserialize_from_xml(path + tmpLoc.ToString() + ".xml", typeof(List<Regions>));
                        if (regions.Count > 0)
                        {
                            widthratio = regions[0].WidthRatio;
                            heightratio = regions[0].HeightRatio;
                        }
                        else
                        {
                            MessageBox.Show("测温区域加载失败，请检查温区文件", "提示消息");
                            log.Write2LogFile("测温区域加载失败，请检查温区文件");
                            return;
                        }
                        Func<Point, Point> tranFunc = (p) =>
                        {
                            return new Point((int)(p.X / widthratio), (int)(p.Y / heightratio));
                        };

                        foreach (var item in regions)
                        {
                            threshold = item.Threshold;
                            if (item.Point != null)
                            {
                                Point Spot = tranFunc(item.Point.Point);
                                OWBTypes.SpotMarker pointMarker = new OWBTypes.SpotMarker();
                                pointMarker.Api_visible = true;
                                pointMarker.Osd_visible = true;
                                pointMarker.Emission = (float)nudEmissivityValue.Value;
                                pointMarker.Distance = (float)nudDistanceValue.Value;
                                pointMarker.ReflectionTemp = (float)nudReflectedTemperatureValue.Value;

                                pointMarker.Point = new OWBTypes.Pos();
                                pointMarker.Point.X = Spot.X;
                                pointMarker.Point.Y = Spot.Y;
                                pointMarker.Label = item.ID.ToString();
                                if (Camera1.PutPoint(item.ID.ToString(), pointMarker))
                                {
                                    Camera1.PutAlarm(item.ID.ToString(), MarkerType.Spot, new OWBTypes.Alarm { Value = (float)(threshold) });
                                }
                                else
                                {
                                    MessageBox.Show("加载测温点失败");
                                    log.Write2LogFile("加载测温点失败");
                                }
                            }
                            if (item.Polygon.Count != 0)
                            {
                                List<Point> PointList = item.Polygon.Where(p => !p.IsTemp).Select(p => tranFunc(p.Point)).ToList();
                                OWBTypes.PolygonMarker regionMarker = new OWBTypes.PolygonMarker();
                                regionMarker.Api_visible = true;
                                regionMarker.Osd_visible = true;
                                regionMarker.Emission = (float)nudEmissivityValue.Value;
                                regionMarker.Distance = (float)nudDistanceValue.Value;
                                regionMarker.ReflectionTemp = (float)nudReflectedTemperatureValue.Value;
                                regionMarker.PointList = new OWBTypes.Pos[PointList.Count];
                                regionMarker.Label = item.ID.ToString();

                                for (int i = 0; i < PointList.Count; i++)
                                {
                                    OWBTypes.Pos point = new OWBTypes.Pos();
                                    point.X = PointList[i].X;
                                    point.Y = PointList[i].Y;
                                    regionMarker.PointList[i] = point;
                                }
                                int index = URL.LastIndexOf("/");
                                URL = URL.Substring(0, index + 1);
                                URL += item.ID.ToString();
                                if (PutAvg("192.168.1.100"))
                                {
                                    if (Camera1.PutRegion(item.ID.ToString(), regionMarker))
                                    {
                                        Camera1.PutAlarm(item.ID.ToString(), MarkerType.Region, new OWBTypes.Alarm { Value = (float)(threshold) });
                                    }
                                }

                                else
                                {
                                    MessageBox.Show("加载测温区域失败");
                                    log.Write2LogFile("加载测温区域失败");
                                }
                            }
                        }
                    }
                    if (Camera1.TagMap == null)
                    {
                        return;
                    }

                    Thread.Sleep(30);
                    foreach (KeyValuePair<string, object> kv in Camera1.TagMap)
                    {
                        if (kv.Key == "max" || kv.Key == "min")
                            continue;

                        Dictionary<string, object> valueMap = OWBJson.parse(kv.Value.ToString());
                        double Max_T = 0;
                        double Avg_T = 0;
                        double Min_T = 0;
                        if (valueMap.ContainsKey("max"))
                        {
                            string max = valueMap["max"].ToString();
                            Max_T = GetMaxMinT(max);
                            string avg = valueMap["avg"].ToString();
                            Avg_T = GetAvgT(avg);
                            string min = valueMap["min"].ToString();
                            Min_T = GetMaxMinT(min);
                        }
                        else
                        {
                            string max_t = valueMap["t"].ToString();
                            Max_T = Convert.ToDouble(max_t);
                        }

                        if (Max_T > threshold)
                        {
                            //Thread.Sleep(50);
                            OKCount1--;
                            if (cbxPow.Text == "100")
                            {
                                database.WriteNewData(Convert.ToInt32(cbxPro.Text), tmpLoc, ContorlsLibrary.UserControls.Table.PanValue.正, Convert.ToInt32(kv.Key), Max_T, Min_T, Avg_T);
                            }
                            lblStatus1.Text = "NG";
                            lblStatus1.ForeColor = Color.Red;
                            lblAlarmNo1.Text = kv.Key;
                            if (!Directory.Exists(path + "\\NG\\"))
                            {
                                Directory.CreateDirectory(path + "\\NG\\");
                            }
                            SnapPicture(Camera1, SnapType.OSD, path + "\\NG\\");
                            SnapPicture(Camera1, SnapType.T, path + "\\NG\\");
                            WriteLog(kv.Key + "区域温度超阈值");
                            log.Write2LogFile(kv.Key + "区域温度超阈值");
                        }
                        else
                        {
                            OKCount1++;
                        }
                    }
                    if (OKCount1 == Camera1.TagMap.Count)
                    {
                        IsOK1 = true;
                    }
                    else
                    {
                        IsOK1 = false;
                    }
                    TEnd1 = true;
                }
            }
        }

        private void LFunc()
        {
            while (Tflag2)
            {
                if (tmpLoc != Location)
                {
                    // MessageBox.Show("Test");
                    Sleep(200);
                    tmpLoc = Location;
                    lblWLocate2.Text = tmpLoc.ToString();
                    string path = ProId + "\\1\\";
                    double threshold = 0;
                    List<MarkerItem> markers = Camera2.GetMarkerItems();
                    DelMarkers(markers, Camera2);
                    List<Regions> regions = new List<Regions>();
                    xml_serializer_manager manager = new xml_serializer_manager();

                    float widthratio = 0;
                    float heightratio = 0;
                    if (!File.Exists(path + tmpLoc.ToString() + ".xml"))
                    {
                        MessageBox.Show("请检查温区文件是否存在", "提示消息");
                        Tflag2 = false;
                    }
                    else
                    {
                        regions = (List<Regions>)manager.deserialize_from_xml(path + tmpLoc.ToString() + ".xml", typeof(List<Regions>));
                        if (regions.Count > 0)
                        {
                            widthratio = regions[0].WidthRatio;
                            heightratio = regions[0].HeightRatio;
                        }
                        else
                        {
                            MessageBox.Show("测温区域加载失败，请检查温区文件", "提示消息");
                            log.Write2LogFile("测温区域加载失败，请检查温区文件");
                            return;
                        }
                        Func<Point, Point> tranFunc = (p) =>
                        {
                            return new Point((int)(p.X / widthratio), (int)(p.Y / heightratio));
                        };

                        foreach (var item in regions)
                        {
                            threshold = item.Threshold;
                            if (item.Point != null)
                            {
                                Point Spot = tranFunc(item.Point.Point);
                                OWBTypes.SpotMarker pointMarker = new OWBTypes.SpotMarker();
                                pointMarker.Api_visible = true;
                                pointMarker.Osd_visible = true;
                                pointMarker.Emission = (float)nudEmissivityValue.Value;
                                pointMarker.Distance = (float)nudDistanceValue.Value;
                                pointMarker.ReflectionTemp = (float)nudReflectedTemperatureValue.Value;
                                pointMarker.Point = new OWBTypes.Pos();
                                pointMarker.Point.X = Spot.X;
                                pointMarker.Point.Y = Spot.Y;
                                pointMarker.Label = item.ID.ToString();
                                if (Camera2.PutPoint(item.ID.ToString(), pointMarker))
                                {

                                    Camera2.PutAlarm(item.ID.ToString(), MarkerType.Spot, new OWBTypes.Alarm { Value = (float)(threshold) });

                                }
                                else
                                {
                                    MessageBox.Show("加载测温点失败");
                                    log.Write2LogFile("加载测温点失败");
                                }
                            }
                            if (item.Polygon.Count != 0)
                            {
                                List<Point> PointList = item.Polygon.Where(p => !p.IsTemp).Select(p => tranFunc(p.Point)).ToList();
                                OWBTypes.PolygonMarker regionMarker = new OWBTypes.PolygonMarker();
                                regionMarker.Api_visible = true;
                                regionMarker.Osd_visible = true;
                                regionMarker.Emission = (float)nudEmissivityValue.Value;
                                regionMarker.Distance = (float)nudDistanceValue.Value;
                                regionMarker.ReflectionTemp = (float)nudReflectedTemperatureValue.Value;
                                regionMarker.PointList = new OWBTypes.Pos[PointList.Count];
                                regionMarker.Label = item.ID.ToString();

                                for (int i = 0; i < PointList.Count; i++)
                                {
                                    OWBTypes.Pos point = new OWBTypes.Pos();
                                    point.X = PointList[i].X;
                                    point.Y = PointList[i].Y;
                                    regionMarker.PointList[i] = point;
                                }
                                int index = URL.LastIndexOf("/");
                                URL = URL.Substring(0, index + 1);
                                URL += item.ID.ToString();
                                if (PutAvg("192.168.1.200"))
                                {
                                    if (Camera2.PutRegion(item.ID.ToString(), regionMarker))
                                    {
                                        Camera2.PutAlarm(item.ID.ToString(), MarkerType.Region, new OWBTypes.Alarm { Value = (float)(threshold) });
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("加载测温区域失败");
                                    log.Write2LogFile("加载测温区域失败");
                                }
                            }
                        }
                    }
                    if (Camera2.TagMap == null)
                    {
                        return;
                    }
                    Thread.Sleep(30);
                    foreach (KeyValuePair<string, object> kv in Camera2.TagMap)
                    {
                        if (kv.Key == "max" || kv.Key == "min")
                            continue;

                        Dictionary<string, object> valueMap = OWBJson.parse(kv.Value.ToString());
                        double Max_T = 0;
                        double Avg_T = 0;
                        double Min_T = 0;
                        if (valueMap.ContainsKey("max"))
                        {
                            string max = valueMap["max"].ToString();
                            Max_T = GetMaxMinT(max);
                            string avg = valueMap["avg"].ToString();
                            Avg_T = GetAvgT(avg);
                            string min = valueMap["min"].ToString();
                            Min_T = GetMaxMinT(min);
                        }
                        else
                        {
                            string max_t = valueMap["t"].ToString();
                            Max_T = Convert.ToDouble(max_t);
                        }
                        if (Max_T > threshold)
                        {
                            //Thread.Sleep(50);
                            OKCount2--;
                            if (cbxPow.Text == "100")
                            {
                                database.WriteNewData(Convert.ToInt32(cbxPro.Text), tmpLoc, ContorlsLibrary.UserControls.Table.PanValue.反, Convert.ToInt32(kv.Key), Max_T, Min_T, Avg_T);
                            }
                            lblStatus2.Text = "NG";
                            lblStatus2.ForeColor = Color.Red;
                            lblAlarmNo2.Text = kv.Key;
                            if (!Directory.Exists(path + "\\NG\\"))
                            {
                                Directory.CreateDirectory(path + "\\NG\\");
                            }
                            SnapPicture(Camera2, SnapType.OSD, path + "\\NG\\");
                            SnapPicture(Camera2, SnapType.T, path + "\\NG\\");
                            WriteLog(kv.Key + "区域温度超阈值");
                            log.Write2LogFile(kv.Key + "区域温度超阈值");
                        }
                        else
                        {
                            OKCount2++;

                        }
                    }
                    if (OKCount2 == Camera2.TagMap.Count)
                    {
                        IsOK2 = true;
                    }
                    else
                    {
                        IsOK2 = false;
                    }
                    TEnd2 = true;
                }
            }
        }

        private void LightFunc()
        {
            if (!Directory.Exists(ProId + "\\0\\" + "Light\\"))
            {
                DirectoryInfo dir = Directory.CreateDirectory(ProId + "\\0\\" + "Light\\");
            }
            if (!Directory.Exists(ProId + "\\1\\" + "Light\\"))
            {
                DirectoryInfo dir = Directory.CreateDirectory(ProId + "\\1\\" + "Light\\");
            }
            bool LFlag = true;
            while (LFlag)
            {
                //Thread.Sleep(1);
                if (Mode == 0)
                {
                    btnLight.Text = "继续";
                    if (mvCamera1.m_bGrabbing)
                    {
                        StopGrab(mvCamera1);
                    }
                    if (mvCamera2.m_bGrabbing)
                    {
                        StopGrab(mvCamera2);
                    }
                    lblstatus.Text = "漏光检测暂停...";
                }
                else if (Mode == 1)
                {
                    btnLight.Text = "暂停";
                    //ccd开始采集
                    if (!mvCamera1.m_bGrabbing)
                    {
                        StartRecord(mvCamera1, ProId + "\\0\\" + "Light\\", CCD1_camera);
                    }

                    if (!mvCamera2.m_bGrabbing)
                    {
                        StartRecord(mvCamera2, ProId + "\\1\\" + "Light\\", CCD2_camera);
                    }
                    lblstatus.Text = "漏光检测中...";
                    if (E_light)
                    {
                        StopGrab(mvCamera1);
                        StopGrab(mvCamera2);
                        mvCamera1.CloseDevice();
                        mvCamera2.CloseDevice();
                        lblstatus.Text = "漏光检测完成...";
                        LFlag = false;
                    }
                }
                else if (Mode == 2)
                {
                    StopGrab(mvCamera1);
                    StopGrab(mvCamera2);
                    LFlag = false;
                }
            }
        }

        private void OpenCamera(MVCamera camera, int num, bool Camera)
        {
            camera.OpenDevice(Cameralist[num]);
        }

        private void StartRecord(MVCamera camera, string path, PictureBox pic)
        {
            camera.Record(pic.Handle, path);
        }

        private void StartGrab(MVCamera camera, PictureBox pic)
        {
            if (camera.IsOpen)
            {
                bool GrabFlag = camera.StartGrab(pic.Handle);
                if (!GrabFlag)
                {
                    WriteLog("采集失败");
                }
            }
            else
            {
                WriteLog("相机连接失败，无法采集");
            }
        }

        private void StopGrab(MVCamera camera)
        {
            camera.StopGrab();
        }

        private void MainFrm_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }

        private void OpenTSM_Click(object sender, EventArgs e)
        {
            if (IsOpenFile)
            {
                IsOpenFile = false;
                fotric1_camera.Image = null;
            }
            else
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "打开快照文件";
                ofd.Multiselect = false;
                ofd.Filter = "温度图快照文件（*.p7）|*.p7|JPG文件（*.jpg）|*.jpg";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileDirectory = ofd.FileName;
                    if (FileDirectory != null)
                    {
                        IsOpenFile = true;
                        DirectoryInfo dir = new DirectoryInfo(FileDirectory);
                        string filepath = dir.ToString().Substring(0, dir.ToString().LastIndexOf(@"\"));
                        DirectoryInfo newdir = new DirectoryInfo(filepath);
                        FileInfo[] file = newdir.GetFiles();
                        //JPG
                        if (FileDirectory.Contains(".JPG"))
                        {
                            foreach (FileInfo item in file)
                            {
                                string filename = item.FullName;
                                if (filename.EndsWith(".JPG"))
                                {
                                    pathList.Add(filename);
                                }
                            }
                            System.Drawing.Image res = System.Drawing.Image.FromFile(FileDirectory);
                            ImgShow Checkform = new ImgShow();
                            Checkform.GetImg(res);
                            Checkform.ShowDialog();
                        }
                        //p7
                        else
                        {
                            foreach (FileInfo item in file)
                            {
                                string filename = item.FullName;
                                if (filename.EndsWith(".p7"))
                                {
                                    pathList.Add(filename);
                                }
                            }
                            ImgShow Checkform = new ImgShow();

                            GetTvalues(FileDirectory);
                            Checkform.Tvalues = OWBGlobal.Camera.TValues;
                            Image img = OWBGlobal.Camera.ToImage("rainbow", OWBGlobal.Camera.TValues);
                            Checkform.GetImg(img);
                            Checkform.ShowDialog();

                        }
                    }
                }
            }
        }

        private void GetTvalues(string FileName)
        {
            using (FileStream fileStream = new FileStream(FileName, System.IO.FileMode.Open, FileAccess.Read))
            {
                int length = (int)fileStream.Length;
                byte[] buffer = new byte[length];
                fileStream.Read(buffer, 0, length);
                using (BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
                {
                    br.BaseStream.Position = 2;
                    int width = (int)IPAddress.HostToNetworkOrder(br.ReadInt16());
                    int height = (int)IPAddress.HostToNetworkOrder(br.ReadInt16());
                    int depth = br.ReadByte();
                    int type = br.ReadByte();
                    uint linesize = br.ReadUInt32();

                    br.BaseStream.Position = OWBCamera.HEADERLENGTH;
                    float[,] tValues = new float[width, height];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            int t_src = br.ReadInt16();
                            int t_interger = (t_src & 65528) >> 3;
                            float t_float = (float)(t_src & 7) / 8;
                            tValues[j, i] = t_interger + t_float;
                        }
                    }
                    OWBGlobal.Camera.TValues = tValues;

                }
            }
        }

        private string SnapPicture(OWBCamera camera, SnapType snapType, string path)
        {
            if (camera.IsConnected)
            {
                try
                {
                    if (Directory.Exists(path) == false)
                    {
                        Directory.CreateDirectory(path);
                    }
                    switch (snapType)
                    {
                        case SnapType.ISP:
                        case SnapType.OSD:
                            string fileName = Path.Combine(path, "IR_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp");
                            Bitmap bmp = new Bitmap(camera.Width, camera.Height);
                            byte[] buf = OWBGlobal.Camera.Snapshot(snapType);
                            bmp = camera.CreateBitmap(buf);
                            bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            return fileName;
                        case SnapType.T:
                            fileName = Path.Combine(path, "IR_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".p7");
                            buf = camera.Snapshot(snapType);
                            using (FileStream fileStream = new FileStream(fileName, System.IO.FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                fileStream.Write(buf, 0, buf.Length);
                                return fileName;
                            }
                    }
                }
                catch
                {

                }
            }
            return null;
        }

        private void Fotric1_ConnTSM_Click(object sender, EventArgs e)
        {
            if (Camera1.IsConnected)
            {
                Camera1.LogoutCamera();
                TempAcqDone1 = true;
                if (TempAcqThread1 != null)
                {
                    TempAcqThread1.Interrupt();
                    TempAcqThread1.Join();
                    TempAcqThread1 = null;
                }
                Fotric1_ConnTSM.Text = "连接";
                WriteLog("成像仪1断开连接");
                log.Write2LogFile("成像仪1断开连接");
            }
            else
            {
                if (Connect(Camera1, "192.168.1.100"))
                {
                    WriteLog("成像仪1连接成功");
                    log.Write2LogFile("成像仪1连接成功");
                    Fotric1_ConnTSM.Text = "断开";
                    TempAcqDone1 = false;
                    TempAcqThread1 = new Thread(new ParameterizedThreadStart(TempAcqCallback1));
                    TempAcqThread1.Priority = ThreadPriority.AboveNormal;
                    TempAcqThread1.IsBackground = true;
                    TempAcqThread1.Start(Camera1);
                    RefreshCamera(Camera1);

                }
            }
        }

        private void Fotric2_ConnTSM_Click(object sender, EventArgs e)
        {
            if (Camera2.IsConnected)
            {
                Camera2.LogoutCamera();
                TempAcqDone2 = true;
                if (TempAcqThread2 != null)
                {
                    TempAcqThread2.Interrupt();
                    TempAcqThread2.Join();
                    TempAcqThread2 = null;
                }
                Fotric2_ConnTSM.Text = "连接";
                WriteLog("成像仪2断开连接");
                log.Write2LogFile("成像仪2断开连接");
            }
            else
            {
                if (Connect(Camera2, "192.168.1.200"))
                {
                    WriteLog("成像仪2连接成功");
                    log.Write2LogFile("成像仪2连接成功");
                    Fotric2_ConnTSM.Text = "断开";
                    TempAcqDone2 = false;
                    TempAcqThread2 = new Thread(new ParameterizedThreadStart(TempAcqCallback2));
                    TempAcqThread2.Priority = ThreadPriority.AboveNormal;
                    TempAcqThread2.IsBackground = true;
                    TempAcqThread2.Start(Camera2);
                    RefreshCamera(Camera2);
                }
            }


        }

        public bool Connect(OWBCamera camera, string ip)
        {
            ConnectForm con = new ConnectForm();
            if (con.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                if (camera.LoginCamera(ip, string.Empty, string.Empty))
                {
                    camera.PostIspAF();
                    List<MarkerItem> markeritems = camera.GetMarkerItems();
                    DelMarkers(markeritems, camera);
                    if (camera.StartStream())
                    {
                        Cursor = Cursors.Default;
                        return true;
                    }
                }
                else
                {
                    WriteLog("成像仪连接失败");
                    return false;
                }
                Cursor = Cursors.Default;
            }
            return false;
        }

        private void DelMarkers(List<MarkerItem> markerItems, OWBCamera camera)
        {
            OWBGlobal.Camera = camera;
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

        private void TempAcqCallback1(object obj)
        {
            while (!TempAcqDone1)
            {
                Sleep(1);
                try
                {
                    OWBCamera camera = obj as OWBCamera;
                    RefreshTempAcq1();

                    if (CameraLoginType == LoginType.H264)
                    {
                        Image image = null;
                        camera.GetH264Frame(out image);
                        fotric1_camera.Image = image;
                    }

                }
                catch (Exception)
                {

                }
            }
        }

        private void TempAcqCallback2(object obj)
        {
            while (!TempAcqDone2)
            {
                Sleep(1);
                try
                {
                    OWBCamera camera = obj as OWBCamera;
                    RefreshTempAcq2();

                    if (CameraLoginType == LoginType.H264)
                    {
                        Image image = null;
                        camera.GetH264Frame(out image);
                        fotric2_camera.Image = image;
                    }

                }
                catch (Exception)
                {

                }
            }
        }

        private void RefreshCamera(OWBCamera camera)
        {
            if (camera.IsConnected)
            {
                CameraWidth = camera.Width;
                CameraHeight = camera.Height;

                cbPalette.Items.Clear();
                if (CameraLoginType == LoginType.RAW)
                {
                    int index = 0;
                    foreach (string paletteKey in OWBGlobal.Palettes.Keys)
                    {
                        cbPalette.Items.Add(paletteKey);
                        if (index == 0)
                        {
                            cbPalette.SelectedItem = paletteKey;
                        }
                        index++;
                    }
                }
                else
                {
                    Cursor = Cursors.WaitCursor;
                    string[] palettes = camera.GetPalettes();
                    if (palettes != null)
                    {
                        foreach (string paletteKey in palettes)
                        {
                            cbPalette.Items.Add(paletteKey);
                        }
                        string pltName = camera.GetCurrentPlt();
                        cbPalette.SelectedItem = pltName;
                    }
                    Cursor = Cursors.Default;
                }

                RefreshTempParameter(camera);
                //statusStrip1.Text = "热像仪已连接：" + OWBGlobal.Camera.IP + "  固件版本：" + string.Join(".", Array.ConvertAll<int, string>(OWBGlobal.Camera.FirmwareVersion, delegate (int v) { return v.ToString(); }));
            }
            else
            {


                //statusStrip1.Text = "热像仪未连接";
            }

        }

        private void RefreshTempAcq1()
        {
            if (Camera1.IsConnected)
            {

                string temp = Camera1.GetIspTItem(MouseHoverPoint1).ToString("F1") + " ℃";
                if (IsDisposed || !this.IsHandleCreated)
                    return;
                else
                {
                    lblCurrentTemp.BeginInvoke((MethodInvoker)delegate ()
                    {
                        lblCurrentTemp.Text = temp;
                    });
                }
            }
        }

        private void RefreshTempAcq2()
        {
            if (Camera2.IsConnected)
            {

                string temp = Camera2.GetIspTItem(MouseHoverPoint2).ToString("F1") + " ℃";
                if (IsDisposed || !this.IsHandleCreated)
                    return;
                else
                {
                    lblCurrentTemp1.BeginInvoke((MethodInvoker)delegate ()
                    {
                        lblCurrentTemp1.Text = temp;
                    });
                }
            }
        }

        private void fotric1_camera_MouseMove(object sender, MouseEventArgs e)
        {

            Point p = new Point();
            p.X = (int)((float)e.X / WidthRatio1);
            p.Y = (int)((float)e.Y / HeightRatio1);
            MouseHoverPoint1 = p;

        }

        private void fotric2_camera_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = new Point();
            p.X = (int)((float)e.X / WidthRatio2);
            p.Y = (int)((float)e.Y / HeightRatio2);
            MouseHoverPoint2 = p;

        }

        private void GetId(string id)
        {
            ProId = id;
        }

        private void TareaTSM_Click(object sender, EventArgs e)
        {

            ProductForm pro = new ProductForm();
            pro.getid = GetId;
            pro.ShowDialog();
            ROIFFrm roi = new ROIFFrm(Camera1);
            roi.ProID = ProId;
            roi.ShowDialog();
        }

        private void TArea2TSM_Click(object sender, EventArgs e)
        {

            ProductForm pro = new ProductForm();
            pro.getid = GetId;
            pro.ShowDialog();
            ROIFFrm roi = new ROIFFrm(Camera2);
            roi.ProID = ProId;
            roi.ShowDialog();
        }

        private void RefreshTempParameter(OWBCamera camera)
        {
            try
            {
                OWBTypes.InstrumentJconfig instrumentJconfig = camera.GetInstrumentJconfig();
                nudAtmosphericTemperatureValue.Value = (decimal)instrumentJconfig.Atmosphere_t;
                nudEmissivityValue.Value = (decimal)instrumentJconfig.Emission;
                nudDistanceValue.Value = (decimal)instrumentJconfig.Distance;
                nudRelativeHumidityValue.Value = (decimal)instrumentJconfig.RH;
                nudReflectedTemperatureValue.Value = (decimal)instrumentJconfig.Reflection_t;
                nudLensTValue.Value = (decimal)instrumentJconfig.Lens_t;
                nudLensTransValue.Value = (decimal)instrumentJconfig.Lens_transmission;
                nudOffsetValue.Value = (decimal)instrumentJconfig.Offset;

            }
            catch
            { }
        }

        private void RefreshTemplate()
        {
            try
            {
                List<OWBTypes.Template> templateList = new List<OWBTypes.Template>();
                JArray templateArray = ToJObject("MyTemplate.json");
                for (int i = 0; i < templateArray.Count; i++)
                {
                    OWBTypes.Template template = new OWBTypes.Template(templateArray[i]["name"].ToString(), templateArray[i]["method"].ToString(), templateArray[i]["url"].ToString(), templateArray[i]["data"].ToString());
                    URL = template.URL;
                    Data = Convert.ToString(template.Data);
                }
            }
            catch { }
        }

        private JArray ToJObject(string fileName)
        {
            try
            {
                using (System.IO.StreamReader file = System.IO.File.OpenText(fileName))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        return (JArray)JToken.ReadFrom(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private void WriteLog(string msg)
        {
            txt_log.AppendText(msg + " " + DateTime.Now.ToString() + "\r\n");
        }

        private void fotric1_camera_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int rx = (int)(MouseHoverPoint1.X * WidthRatio1);
            int ry = (int)(MouseHoverPoint1.Y * HeightRatio1);
            g.DrawLine(Pens.White, rx, ry - 4, rx, ry + 4);
            g.DrawLine(Pens.White, rx - 4, ry, rx + 4, ry);
        }

        private void fotric2_camera_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int rx = (int)(MouseHoverPoint2.X * WidthRatio2);
            int ry = (int)(MouseHoverPoint2.Y * HeightRatio2);
            g.DrawLine(Pens.White, rx, ry - 4, rx, ry + 4);
            g.DrawLine(Pens.White, rx - 4, ry, rx + 4, ry);
        }

        private void cbPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OWBGlobal.Camera.IsConnected)
            {
                if (CameraLoginType == LoginType.H264)
                {
                    OWBGlobal.Camera.PutCurrentPlt(cbPalette.Text);
                }
            }
        }

        private void fotric2_camera_MouseLeave(object sender, EventArgs e)
        {
            MouseHoverPoint2 = new Point(-1, -1);
        }

        private void fotric1_camera_MouseLeave(object sender, EventArgs e)
        {
            MouseHoverPoint1 = new Point(-1, -1);
        }

        private void frotric1_TSM_Click(object sender, EventArgs e)
        {
            if (Camera1.IsConnected)
            {
                ParamForm param = new ParamForm(Camera1);
                param.Text += "-Camera1";
                param.ShowDialog();
            }

        }

        private void fotric2__TSM_Click(object sender, EventArgs e)
        {
            if (Camera2.IsConnected)
            {
                ParamForm param = new ParamForm(Camera2);
                param.Text += "-Camera2";
                param.ShowDialog();
            }
        }

        private void DataSetTSM_Click(object sender, EventArgs e)
        {
            DataConnectForm dacon = new DataConnectForm();
            dacon.ShowDialog();
        }

        private void btnChangePro_Click(object sender, EventArgs e)
        {
            ProId = System.AppDomain.CurrentDomain.BaseDirectory + "\\Product\\" + cbxPro.Text;
        }

        private bool PutAvg(string ip)
        {
            const int PORT = 10080;
            //string ip = "192.168.1.100";
            byte[] buffer = new byte[1024];
            uint length = 0;
            int statusCode = 0;
            OWBString.StringToBytes(Data, Data.Length, Encoding.UTF8).CopyTo(buffer, 0);
            statusCode = RestSDK.Put(ip, (ushort)PORT, URL, ref buffer, ref length);
            if (statusCode == 201 || statusCode == 200)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private double GetMaxMinT(string value)
        {
            int index0 = value.IndexOf("t");
            int index1 = value.IndexOf("x");
            string tmp = value.Substring(index0 + 3, index1 - index0 - 9);
            return Convert.ToDouble(tmp);
        }

        private double GetAvgT(string value)
        {
            int index0 = value.IndexOf("t");
            int index1 = value.LastIndexOf("\r");
            string tmp = value.Substring(index0 + 3, index1 - index0 - 3);
            return Convert.ToDouble(tmp);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            lblstatus.Text = "设备复位回原点...";
            TFlag = false;
            Sleep(500);
            PLC.WriteSinglePoint("R131", true);
            Sleep(100);
            PLC.WriteSinglePoint("R131", false);
            PLC.WriteSinglePoint("R7", true);
            if (InL)
            {
                LFlag = false;
                StopGrab(mvCamera1);
                StopGrab(mvCamera2);
                th_Light.Interrupt();
            }
            if (InT)
            {
                Tfalg1 = false;
                Tflag2 = false;
                Location = 0;
                tmpLoc = 0;
                tL.Wait(1500);
                tR.Wait(1500);
            }
            TFlag = true;
            _mainEvent.Set();
            Sleep(500);
            if (Mode == 0)
            {
                lblstatus.Text = "设备复位回原点完成...";
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            lblstatus.Text = "检测中断...";
            log.Write2LogFile("检测中断。。。");
            TFlag = false;
            Sleep(500);
            PLC.WriteSinglePoint("R131", true);
            Sleep(500);
            PLC.WriteSinglePoint("R131", false);
            TFlag = true;
            _mainEvent.Set();
            Sleep(200);
            if (InT)
            {
                Tfalg1 = false;
                Tflag2 = false;
                tL.Wait(1500);
                tR.Wait(1500);
                if (Camera1.IsConnected)
                {
                    Camera1.LogoutCamera();
                    TempAcqDone1 = true;
                    if (TempAcqThread1 != null)
                    {
                        TempAcqThread1.Interrupt();
                        TempAcqThread1.Join();
                        TempAcqThread1 = null;
                    }
                    Fotric1_ConnTSM.Text = "连接";
                }

                if (Camera2.IsConnected)
                {
                    Camera2.LogoutCamera();
                    TempAcqDone2 = true;
                    if (TempAcqThread2 != null)
                    {
                        TempAcqThread2.Interrupt();
                        TempAcqThread2.Join();
                        TempAcqThread2 = null;
                    }
                    Fotric2_ConnTSM.Text = "连接";
                }
            }

            if (InL)
            {
                th_Light.Interrupt();
                th_Light.Join();
                StopGrab(mvCamera1);
                mvCamera1.CloseDevice();
                StopGrab(mvCamera2);
                mvCamera2.CloseDevice();
            }
        }

        private void btnLight_Click(object sender, EventArgs e)
        {

            if (AlarmFlag)
            {
                WriteLog("设备故障，请检查");
                return;
            }
            if (!mvCamera1.IsOpen)
            {
                mvCamera1.OpenDevice(Cameralist[0]);
            }
            if (!mvCamera2.IsOpen)
            {
                mvCamera2.OpenDevice(Cameralist[1]);
            }
            if (Mode == 0)
            {
                btnLight.Text = "暂停";
                TFlag = false;
                Sleep(300);
                PLC.WriteSinglePoint("R130", true);
                if (!InL)
                {
                    mvCamera1.StopGrab();
                    mvCamera2.StopGrab();
                    PLC.WriteSinglePoint("R50", true);
                    th_Light.Start();
                }
                TFlag = true;
                _mainEvent.Set();
            }
            if (Mode == 1)
            {
                //btnLight.Text = "继续";
                TFlag = false;
                Sleep(500);
                PLC.WriteSinglePoint("R134", true);
                TFlag = true;
                _mainEvent.Set();
            }

        }

        private void btnTemp_Click(object sender, EventArgs e)
        {
            Tfalg1 = true;
            Tflag2 = true;

            if (AlarmFlag)
            {
                WriteLog("设备故障，请检查");
                return;
            }
            if (Mode == 0)
            {
                TFlag = false;
                Sleep(500);
                PLC.WriteSinglePoint("R130", true);
                PLC.WriteSinglePoint("R320", true);
                lblstatus.Text = "温度检测中...";
                TFlag = true;
                _mainEvent.Set();
                return;
            }
            lblstatus.Text = "温度检测中...";
            //th_TR.Start();
            //th_TL.Start();
            tR.Start();
            tL.Start();

        }

        private void CCD1TSM_Click(object sender, EventArgs e)
        {

            if (mvCamera1.OpenDevice(Cameralist[0]))
            {
                WriteLog("相机1连接成功");
                log.Write2LogFile("CCD1连接成功");
                mvCamera1.StartGrab(CCD1_camera.Handle);
            }
            else
            {
                WriteLog("相机1连接失败");
                log.Write2LogFile("CCD1连接失败");
            }
        }

        private void CCD2TSM_Click(object sender, EventArgs e)
        {
            if (mvCamera2.OpenDevice(Cameralist[1]))
            {
                WriteLog("相机2连接成功");
                log.Write2LogFile("CCD2连接成功");
                mvCamera2.StartGrab(CCD2_camera.Handle);
            }
            else
            {
                WriteLog("相机2连接失败");
                log.Write2LogFile("CCD2连接失败");
            }
        }

        private void CCD1ParamTSM_Click(object sender, EventArgs e)
        {
            CCDParamForm cp = new CCDParamForm(mvCamera1);
            cp.ShowDialog();
        }

        private void CCD2ParamTSM_Click(object sender, EventArgs e)
        {
            CCDParamForm cp = new CCDParamForm(mvCamera2);
            cp.ShowDialog();
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.Write2LogFile("软件关闭");
            _stopEvent.Set();
            Tfalg1 = false;
            Tflag2 = false;
            LFlag = false;
            Process.GetCurrentProcess().Kill();

            if (th_listen != null)
            {
                th_listen.Interrupt();
                th_listen.Join();
                th_listen = null;
            }
            PLC.MewtocolClose();

            if (th_Light != null)
            {
                LFlag = false;
                th_Light.Join();
                th_Light.Interrupt();
                th_Light = null;
            }
            if (mvCamera1.IsOpen)
            {
                if (mvCamera1.m_bGrabbing)
                {
                    mvCamera1.StopGrab();
                }
                mvCamera1.CloseDevice();
            }
            if (mvCamera2.IsOpen)
            {
                if (mvCamera2.m_bGrabbing)
                {
                    mvCamera2.StopGrab();
                }
                mvCamera2.CloseDevice();
            }

            if (Camera1.IsConnected)
            {
                Camera1.LogoutCamera();
                TempAcqDone1 = true;
                if (TempAcqThread1 != null)
                {
                    TempAcqThread1.Interrupt();
                    TempAcqThread1.Join();
                    TempAcqThread1 = null;
                }

            }
            if (Camera2.IsConnected)
            {
                Camera2.LogoutCamera();
                TempAcqDone2 = true;
                if (TempAcqThread2 != null)
                {
                    TempAcqThread2.Interrupt();
                    TempAcqThread2.Join();
                    TempAcqThread2 = null;
                }
            }
            this.Dispose();
            this.Close();
        }

        private void MainFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        public static void Sleep(int millisecondsTimeout)
        {
            var dt = DateTime.Now.AddMilliseconds(millisecondsTimeout);

            do
            {
                Thread.Sleep(1);

                Application.DoEvents();
            } while (DateTime.Now <= dt);
        }

        private void CurrrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        { MessageBox.Show(e.ExceptionObject.ToString()); }

        private void CheckTsm_Click(object sender, EventArgs e)
        {
            mvCamera1.CloseDevice();
            CheckForm ch = new CheckForm(0);
            ch.Show();

        }

        private void CheckTSMp_Click(object sender, EventArgs e)
        {
            mvCamera2.CloseDevice();
            CheckForm ch = new CheckForm(1);
            ch.Show();

        }
    }
}
