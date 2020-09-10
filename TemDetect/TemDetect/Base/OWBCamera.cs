using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using SDK;
using System.Security.Cryptography;

namespace Base
{
    public struct TemperatureRange
    {
        public const double Epsilon = 1E-6;
        #region Fields
        public float RangeMax;
        public float RangeMin;
        public object Tag;
        public static TemperatureRange Default = new TemperatureRange(-20.0F, 150F);
        #endregion

        #region Constructors
        public TemperatureRange(float rangeMin, float rangeMax)
        {
            RangeMin = rangeMin;
            RangeMax = rangeMax;
            Tag = null;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return RangeMin.ToString() + "~" + RangeMax.ToString();
        }

        public static bool operator ==(TemperatureRange t1, TemperatureRange t2)
        {
            return Math.Abs(t1.RangeMin - t2.RangeMin) < Epsilon && Math.Abs(t1.RangeMax - t2.RangeMax) < Epsilon;
        }

        public static bool operator !=(TemperatureRange t1, TemperatureRange t2)
        {
            return !(t1 == t2);
        }

        public override bool Equals(object obj)
        {
            if (obj is TemperatureRange)
            {
                return this == (TemperatureRange)obj;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return RangeMin.GetHashCode() + RangeMax.GetHashCode();
        }
        #endregion
    };
    public class OWBCamera
    {
        #region Const
        public static int HEADERLENGTH = 16;
        public const int LUT_SIZE = 65536;
        public const float ZERO_T = 273.15F;
        private const int PORT = 10080;
        private const string URL_ADMIN_INFO = "/admin/info";
        private const string URL_ADMIN_REBOOT = "/admin/reboot";
        private const string URL_SENSOR_DIMENSION = "/sensor/dimension";
        private const string URL_STREAM_VIDEO_RAW = "/stream/video/raw";
        private const string URL_STREAM_VIDEO_PRI = "/stream/video/pri";
        private const string URL_STREAM_VIDEO_SUB = "/stream/video/sub";
        private const string URL_OSD_TRAY_PRESETS = "/isp/t-ray/presets";
        private const string URL_CAPTURE_MODES = "/capture/modes";
        private const string URL_CAPTURE_MODE = "/capture/mode";
        private const string URL_CAPTURE = "/capture/{0}";
        private const string URL_ISP_TRAY_CUSTOMS = "/isp/t-ray/customs";
        private const string URL_OSD_TRAY_PLT = "/isp/t-ray/plt";
        private const string URL_OSD_LAYOUT_CBAR = "/osd/layout/cbar";
        private const string URL_OSD_LAYOUT_TITLE = "/osd/layout/title";
        private const string URL_OSD_LAYOUT_TIME = "/osd/layout/time";
        private const string URL_OSD_LAYOUT_UNIT = "/osd/layout/unit";
        private const string URL_OSD_LAYOUT_EMISSIVITY = "/osd/layout/emissivity";
        private const string URL_ISP_T = @"/isp/t?x={0}&y={1}";
        private const string URL_ISP_SNAPSHOT = "/isp/snapshot";
        private const string URL_T_SNAPSHOT = "/isp/t-snapshot";
        private const string URL_OSD_SNAPSHOT = "/osd/snapshot";
        private const string URL_ISP_AF = "/isp/af";
        private const string URL_PERI = "/peri/ptz/af";
        private const string URL_ISP_INSTRUMENT_OBJECTS_REGION = "/isp/instrument/objects/regions/{0}";
        private const string URL_ISP_INSTRUMENT_JCONFIG = "/isp/instrument/jconfig";
        private const string URL_ADMIN_USERS = "/admin/users";
        private const string URL_ADMIN_USER_OPERATE = "/user/{0}";
        private const string URL_ADMIN_BOOT_ID = "/admin/boot-id";
        private const string URL_PERI_FOCUS = @"/peri/focus?op={0}&step={1}";
        private const string URL_SENSOR_LENS = "/sensor/lens";
        private const string URL_SENSOR_T_RANGE = "/sensor/t-range";
        private const string URL_SENSOR_J_CONFIG = "/sensor/jconfig";
        private const string URL_SENSOR_LUTS = @"/sensor/luts";
        private const string URL_SENSOR_CURRENT_LUT_INDEX = @"/sensor/lut";
        private const string URL_SENSOR_LUT_TABLE = @"/sensor/luts/{0}?list";
        private const string URL_ISP_INSTRUMENT_OBJECTS_SPOTS = @"/isp/instrument/objects/points";
        private const string URL_ISP_INSTRUMENT_OBJECTS_REGIONS = @"/isp/instrument/objects/regions";
        private const string URL_ISP_INSTRUMENT_OBJECTS_LINES = @"/isp/instrument/objects/lines";
        private const string URL_ISP_INSTRUMENT_OBJECTS_SPOTITEM = @"/isp/instrument/objects/points/{0}";
        private const string URL_ISP_INSTRUMENT_OBJECTS_REGIONITEM = @"/isp/instrument/objects/regions/{0}";
        private const string URL_ISP_INSTRUMENT_OBJECTS_LINEITEM = @"/isp/instrument/objects/lines/{0}";
        private const string URL_STREAM_TAG_VALUES = "/stream/tag/values";
        private const string URL_TAG = "/tag/values";

        protected object CameraLock = new object();
        protected object CameraRawLock = new object();
        protected object CameraH264Lock = new object();
        #endregion

        #region Fields
        private IntPtr _HStream;
        private IntPtr _Decoder;
        private IntPtr _HCb;

        private bool _IsConnected;
        private bool _IsPlay;

        private string _IP;
        private string _UserName;
        private string _Password;
        private Dictionary<string, object> _TagMap;

        private byte[] _EncryptPassword;

        private string _BootID;

        private int[] _FirmwareVersion;
        private int _Width;
        private int _Height;
        private string _VideoMode;
        private int _CurrentBitrate;

        private string[] _Lens;
        private int _LensIndex;
        private TemperatureRange[] _TRanges;
        private int _TRangeIndex;
        private Dictionary<int, List<int>> _LensTRangesMap;

        private Image _ChannelImage;
        private ushort[,] _RawValues;
        private float[,] _TValues;
        private float[] _LUT;
        private int _From;
        private int _To;

        private StreamSDK.streamsdk_cb_grabber _GrabberCallback;
        private StreamSDK.streamsdk_cb_image_grabber _ImageGrabberCallback;
        private StreamSDK.streamsdk_cb_grabber _StreamGrabberCallback;
        #endregion

        #region Constructors
        public OWBCamera()
        {
            InitProperties();

            IP = string.Empty;
            UserName = string.Empty;
            Password = string.Empty;

            VideoMode = string.Empty;
            CurrentBitrate = 0;
            GrabberCallback = GrabberRaw;
            ImageGrabberCallback = GrabberH264;
            StreamGrabberCallback = GrabberStream;
        }
        #endregion

        #region Properties

        public IntPtr HStream
        {
            get { return _HStream; }
            private set { _HStream = value; }
        }

        public IntPtr Decoder
        {
            get { return _Decoder; }
            set { _Decoder = value; }
        }

        public bool IsConnected
        {
            get { return _IsConnected; }
            private set { _IsConnected = value; }
        }

        public bool IsPlay
        {
            get { return _IsPlay; }
            private set { _IsPlay = value; }
        }

        public string IP
        {
            get { return _IP; }
            private set { _IP = value; }
        }

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private byte[] EncryptPassword
        {
            get { return _EncryptPassword; }
            set { _EncryptPassword = value; }
        }

        private string BootID
        {
            get { return _BootID; }
            set { _BootID = value; }
        }

        public int[] FirmwareVersion
        {
            get { return _FirmwareVersion; }
            set { _FirmwareVersion = value; }
        }

        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        public string VideoMode
        {
            get { return _VideoMode; }
            set { _VideoMode = value; }
        }

        public int CurrentBitrate
        {
            get { return _CurrentBitrate; }
            set { _CurrentBitrate = value; }
        }

        public string[] Lens
        {
            get { return _Lens; }
            set { _Lens = value; }
        }

        public int LensIndex
        {
            get { return _LensIndex; }
            set
            {
                if (_LensIndex != value)
                {
                    bool found = false;
                    List<int> tRanges = LensTRangesMap[value];
                    for (int i = 0; i < tRanges.Count; i++)
                    {
                        if (tRanges[i] == TRangeIndex)
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        OWBTypes.SensorJConfig tmpValue = new OWBTypes.SensorJConfig();
                        tmpValue.Selected_Lens = value + 1;
                        tmpValue.Selected_T_Range = TRangeIndex + 1;
                        if (PutSensorJConfig(tmpValue))
                        {
                            _LensIndex = value;
                        }
                    }
                    else
                    {
                        OWBTypes.SensorJConfig tmpValue = new OWBTypes.SensorJConfig();
                        tmpValue.Selected_Lens = value + 1;
                        tmpValue.Selected_T_Range = tRanges[0] + 1;
                        if (PutSensorJConfig(tmpValue))
                        {
                            _LensIndex = value;
                            _TRangeIndex = tRanges[0];
                        }
                    }
                }
            }
        }

        public TemperatureRange[] TRanges
        {
            get { return _TRanges; }
            set { _TRanges = value; }
        }

        public int TRangeIndex
        {
            get { return _TRangeIndex; }
            set
            {
                if (_TRangeIndex != value)
                {
                    OWBTypes.SensorJConfig tmpValue = new OWBTypes.SensorJConfig();
                    tmpValue.Selected_Lens = LensIndex + 1;
                    tmpValue.Selected_T_Range = value + 1;
                    if (PutSensorJConfig(tmpValue))
                    {
                        _TRangeIndex = value;
                    }
                }
            }
        }

        private Dictionary<int, List<int>> LensTRangesMap
        {
            get { return _LensTRangesMap; }
            set { _LensTRangesMap = value; }
        }

        public Image ChannelImage
        {
            get { return _ChannelImage; }
            set { _ChannelImage = value; }
        }

        private ushort[,] RawValues
        {
            get { return _RawValues; }
            set { _RawValues = value; }
        }

        public float[,] TValues
        {
            get { return _TValues; }
            set { _TValues = value; }
        }

        private float[] LUT
        {
            get { return _LUT; }
            set { _LUT = value; }
        }


        private int From
        {
            get { return _From; }
            set { _From = value; }
        }

        private int To
        {
            get { return _To; }
            set { _To = value; }
        }

        private StreamSDK.streamsdk_cb_grabber GrabberCallback
        {
            get { return _GrabberCallback; }
            set { _GrabberCallback = value; }
        }

        public StreamSDK.streamsdk_cb_image_grabber ImageGrabberCallback
        {
            get { return _ImageGrabberCallback; }
            set { _ImageGrabberCallback = value; }
        }

        public IntPtr HCb
        {
            get
            {
                return _HCb;
            }

            set
            {
                _HCb = value;
            }
        }

        public StreamSDK.streamsdk_cb_grabber StreamGrabberCallback
        {
            get
            {
                return _StreamGrabberCallback;
            }

            set
            {
                _StreamGrabberCallback = value;
            }
        }

        public Dictionary<string, object> TagMap
        {
            get
            {
                return _TagMap;
            }

            set
            {
                _TagMap = value;
            }
        }
        #endregion

        #region Methods

        private void InitProperties()
        {
            HStream = IntPtr.Zero;
            Decoder = IntPtr.Zero;
            HCb = IntPtr.Zero;

            IsConnected = false;
            IsPlay = false;

            BootID = string.Empty;

            Width = 80;
            Height = 80;

            VideoMode = StreamSDK.URL_STREAM_VIDEO_RAW;
        }

        public bool LoginCamera(string ipAddress, string userName, string password)
        {
            IP = ipAddress;
            UserName = userName;
            Password = password;

            if (IsConnected)
            {
                LogoutCamera();
            }
            InitProperties();

            byte[] buf = new byte[1024];
            byte[] passwordBytes = new byte[0];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_ADMIN_INFO, ref buf, ref length)))
            {
                return false;
            }
            try
            {
                OWBTypes.AdminInfo adminInfo = OWBJson.parse<OWBTypes.AdminInfo>(OWBString.BytesToString(buf, Encoding.UTF8));
                FirmwareVersion = adminInfo.Device_FW.Version.Clone() as int[];
            }
            catch
            {
                FirmwareVersion = new int[4];
            }

            BootID = GetAdminBootID();
            EncryptPassword = Encoding.UTF8.GetBytes(Password);

            RestSDK.Rest_set_authroization(userName, EncryptPassword);

            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_SENSOR_DIMENSION, ref buf, ref length)))
            {
                return false;
            }
            try
            {
                OWBTypes.SensorDimension sensorDimension = OWBJson.parse<OWBTypes.SensorDimension>(OWBString.BytesToString(buf, Encoding.UTF8));
                Width = sensorDimension.W;
                Height = sensorDimension.H;
                IsConnected = true;
                RefreshCameraInfo();
                
            }
            catch
            {
                return false;
            }
            if (CreateStream())
            {
                IsConnected = true;
                return true;
            }
            return false;
        }

        public void LogoutCamera()
        {
            StopStream();
            IsConnected = false;
        }

        private void RefreshCameraInfo()
        {
            OWBTypes.SensorLens[] sensorLens = GetSensorLens();
            Lens = new string[sensorLens.Length - 1];
            for (int i = 1; i < sensorLens.Length; i++)
            {
                Lens[i - 1] = sensorLens[i].Model;
            }
            OWBTypes.SensorT_Range[] sensorT_Ranges = GetSensorT_Range();
            TRanges = new TemperatureRange[sensorT_Ranges.Length - 1];
            for (int i = 1; i < sensorT_Ranges.Length; i++)
            {
                TRanges[i - 1] = new TemperatureRange(sensorT_Ranges[i].Low, sensorT_Ranges[i].High);
                TRanges[i - 1].Tag = i - 1;
            }
            OWBTypes.SensorLUT[] sensorLUTs = GetSensorLUTs();
            LensTRangesMap = new Dictionary<int, List<int>>();
            for (int i = 1; i < sensorLUTs.Length; i++)
            {
                int lensIndex = sensorLUTs[i].Lens - 1;
                int temperatureRangeIndex = sensorLUTs[i].T_Range - 1;
                if (!LensTRangesMap.ContainsKey(lensIndex))
                {
                    LensTRangesMap[lensIndex] = new List<int>();
                }

                LensTRangesMap[lensIndex].Add(temperatureRangeIndex);
            }
            OWBTypes.SensorJConfig sensorJConfig = GetSensorJConfig();
            LensIndex = sensorJConfig.Selected_Lens - 1;
            TRangeIndex = sensorJConfig.Selected_T_Range - 1;
            UpdateFactoryLUT();
        }

        private bool CreateStream()
        {
            StreamSDK.streamsdk_set_thread_pool_size(4);
            IntPtr hStream = IntPtr.Zero;
            if (StreamSDK.streamsdk_create_stream(IP, (ushort)(PORT + 1), ref hStream) != StreamSDK.STREAMSDK_EC_OK)
            {
                return false;
            }
            HStream = hStream;
            IntPtr hcb = IntPtr.Zero;
            if (StreamSDK.streamsdk_create_stream(IP, (ushort)(PORT + 1), ref hcb) != StreamSDK.STREAMSDK_EC_OK)
            {
                return false;
            }
            HCb = hcb;
            return true;
        }


        public bool StartStream()
        {
            int max_packet_size = 0;
            #region 视频流
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_STREAM_VIDEO_PRI, ref buf, ref length)))
            {
                return false;
            }
            try
            {
                OWBTypes.StreamVideoPri streamVideoPri = OWBJson.parse<OWBTypes.StreamVideoPri>(OWBString.BytesToString(buf, Encoding.UTF8));
                max_packet_size = streamVideoPri.Max_Packet_Size;
            }
            catch
            {
                return false;
            }

            if (StreamSDK.streamsdk_start_stream(HStream, StreamSDK.URL_STREAM_VIDEO_PRI, max_packet_size, null, IntPtr.Zero) != StreamSDK.STREAMSDK_EC_OK)
            {
                return false;
            }

            IntPtr dec = IntPtr.Zero;
            StreamSDK.streamsdk_st_decoder_param dp = new StreamSDK.streamsdk_st_decoder_param();
            dp.dec_w = Width;
            dp.dec_h = Height;
            dp.pix_type = (int)StreamSDK.streamsdk_enum_pix_type.STREAMSDK_PIX_BGR;
            StreamSDK.streamsdk_create_h264_decoder(HStream, ref dp, ref dec);
            Decoder = dec;
            StreamSDK.streamsdk_start_h264_decoder(Decoder, ImageGrabberCallback, IntPtr.Zero);
            #endregion

            #region 数据流
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_STREAM_TAG_VALUES, ref buf, ref length)))
            {
                return false;
            }
            try
            {
                OWBTypes.MaxPackageSize maxPackageSize = OWBJson.parse<OWBTypes.MaxPackageSize>(OWBString.BytesToString(buf, Encoding.UTF8));
                max_packet_size = maxPackageSize.Max_Packet_Size;
            }
            catch
            {
                return false;
            }
            #endregion
            if (StreamSDK.streamsdk_start_stream(HCb, URL_TAG, max_packet_size, null, IntPtr.Zero) != StreamSDK.STREAMSDK_EC_OK)
            {
                return false;
            }
            StreamSDK.streamsdk_set_stream_grabber(HCb, StreamGrabberCallback, IntPtr.Zero);
            return true;
        }
        public bool StartStream(LoginType loginType, StreamType streamType)
        {
            if (CreateStream())
            {
                if (loginType == LoginType.RAW)
                {
                    VideoMode = StreamSDK.URL_STREAM_VIDEO_RAW;
                }
                else
                {
                    if (streamType == StreamType.PRI)
                    {
                        VideoMode = StreamSDK.URL_STREAM_VIDEO_PRI;
                    }
                    else
                    {
                        VideoMode = StreamSDK.URL_STREAM_VIDEO_SUB;
                    }
                }
                if (StartStream())
                {
                    IsPlay = true;
                    return true;
                }
            }
            return false;
        }

        //private bool StartStream()
        //{
        //    int max_packet_size = 0;

        //    byte[] buf = new byte[1024];
        //    uint length = 0;
        //    switch (VideoMode)
        //    {
        //        case StreamSDK.URL_STREAM_VIDEO_RAW:
        //            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_STREAM_VIDEO_RAW, ref buf, ref length)))
        //            {
        //                return false;
        //            }
        //            try
        //            {
        //                OWBTypes.StreamVideoRaw streamVideoRaw = OWBJson.parse<OWBTypes.StreamVideoRaw>(OWBString.BytesToString(buf, Encoding.UTF8));
        //                max_packet_size = streamVideoRaw.Max_Packet_Size;
        //            }
        //            catch
        //            {
        //                return false;
        //            }
        //            break;
        //        case StreamSDK.URL_STREAM_VIDEO_PRI:
        //            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_STREAM_VIDEO_PRI, ref buf, ref length)))
        //            {
        //                return false;
        //            }
        //            try
        //            {
        //                OWBTypes.StreamVideoPri streamVideoPri = OWBJson.parse<OWBTypes.StreamVideoPri>(OWBString.BytesToString(buf, Encoding.UTF8));
        //                max_packet_size = streamVideoPri.Max_Packet_Size;
        //                CurrentBitrate = streamVideoPri.Bitrate;
        //            }
        //            catch
        //            {
        //                return false;
        //            }
        //            break;
        //        case StreamSDK.URL_STREAM_VIDEO_SUB:
        //            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_STREAM_VIDEO_SUB, ref buf, ref length)))
        //            {
        //                return false;
        //            }
        //            try
        //            {
        //                OWBTypes.StreamVideoSub streamVideoSub = OWBJson.parse<OWBTypes.StreamVideoSub>(OWBString.BytesToString(buf, Encoding.UTF8));
        //                max_packet_size = streamVideoSub.Max_Packet_Size;
        //                CurrentBitrate = streamVideoSub.Bitrate;
        //            }
        //            catch
        //            {
        //                return false;
        //            }
        //            break;
        //    }

        //    if (StreamSDK.streamsdk_start_stream(HStream, VideoMode, max_packet_size, null, IntPtr.Zero) != StreamSDK.STREAMSDK_EC_OK)
        //    {
        //        return false;
        //    }

        //    if (VideoMode == StreamSDK.URL_STREAM_VIDEO_RAW)
        //    {
        //        StreamSDK.streamsdk_set_stream_grabber(HStream, GrabberCallback, IntPtr.Zero);
        //    }
        //    else
        //    {
        //        IntPtr dec = IntPtr.Zero;
        //        StreamSDK.streamsdk_st_decoder_param dp = new StreamSDK.streamsdk_st_decoder_param();
        //        dp.dec_w = Width;
        //        dp.dec_h = Height;
        //        dp.pix_type = (int)StreamSDK.streamsdk_enum_pix_type.STREAMSDK_PIX_BGR;
        //        StreamSDK.streamsdk_create_h264_decoder(HStream, ref dp, ref dec);
        //        Decoder = dec;
        //        StreamSDK.streamsdk_start_h264_decoder(Decoder, ImageGrabberCallback, IntPtr.Zero);
        //    }
        //    return true;
        //}

        public void StopStream()
        {
            if (Decoder != IntPtr.Zero)
            {
                StreamSDK.streamsdk_stop_h264_decoder(Decoder);
                StreamSDK.streamsdk_destroy_h264_decoder(Decoder);
                Decoder = IntPtr.Zero;
            }
            if (HStream != IntPtr.Zero)
            {
                StreamSDK.streamsdk_stop_stream(HStream);
                StreamSDK.streamsdk_destroy_stream(HStream);
                HStream = IntPtr.Zero;
            }
            if (HCb != IntPtr.Zero)
            {
                StreamSDK.streamsdk_stop_stream(HCb);
                StreamSDK.streamsdk_destroy_stream(HCb);
                HCb = IntPtr.Zero;
            }
            IsPlay = false;
        }

        public void UpdateFactoryLUT()
        {
            lock (CameraLock)
            {
                int index = 0;
                byte[] buf = new byte[1024];
                byte[] passwordBytes = new byte[0];
                uint length = 0;
                if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_SENSOR_CURRENT_LUT_INDEX, ref buf, ref length)))
                {
                    index = OWBJson.parse<int>(OWBString.BytesToString(buf, Encoding.UTF8));
                    OWBTypes.SensorLUTItem[] sensorLUTs = null;
                    if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, string.Format(URL_SENSOR_LUT_TABLE, index), ref buf, ref length)))
                    {
                        sensorLUTs = OWBJson.parse<OWBTypes.SensorLUTItem[]>(OWBString.BytesToString(buf, Encoding.UTF8));
                    }
                    LUT = new float[LUT_SIZE];
                    int from = sensorLUTs[0].R;
                    int to = sensorLUTs[0].R;
                    for (int i = 0; i < sensorLUTs.Length - 1 && sensorLUTs[i].R < sensorLUTs[i + 1].R; i++)
                    {
                        int tmpFrom = sensorLUTs[i].R;
                        int tmpTo = sensorLUTs[i + 1].R;
                        to = tmpTo;
                        float fromTemp = sensorLUTs[i].T;
                        float toTemp = sensorLUTs[i + 1].T;
                        for (int j = tmpFrom; j <= tmpTo; j++)
                        {
                            LUT[j] = fromTemp + (toTemp - fromTemp) * (j - tmpFrom) / (tmpTo - tmpFrom);
                        }
                    }

                    for (int i = 0; i < from; i++)
                    {
                        LUT[i] = LUT[from];
                    }
                    for (int i = to + 1; i < LUT_SIZE; i++)
                    {
                        LUT[i] = LUT[to];
                    }
                    From = from;
                    To = to;
                   
                }
            }
        }

        private void GrabberStream(int error, ref StreamSDK.streamsdk_st_buffer buf, IntPtr user_data)
        {
            byte[] buffer = new byte[buf.buf_size];
            unsafe
            {
                byte* dPtr = (byte*)(buf.buf_ptr);
                for (int i = 0; i < buf.buf_size; i++)
                {
                    buffer[i] = dPtr[i];
                }
            }
            try
            {
                Dictionary<string, object> tagValueMap = OWBJson.parse(OWBString.BytesToString(buffer, Encoding.UTF8));
                TagMap = tagValueMap;
            }
            catch
            { }
        }

        public OWBTypes.InstrumentJconfig GetInstrumentJconfig()
        {
            OWBTypes.InstrumentJconfig instrumentJconfig = new OWBTypes.InstrumentJconfig();
            byte[] buf = new byte[1024];
            uint length = 0;
            try
            {
                if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_ISP_INSTRUMENT_JCONFIG, ref buf, ref length)))
                {
                    return instrumentJconfig;
                }
                instrumentJconfig = OWBJson.parse<OWBTypes.InstrumentJconfig>(OWBString.BytesToString(buf, Encoding.UTF8));
            }
            catch
            { }
            return instrumentJconfig;
        }

        public bool PutInstrumentJconfig(OWBTypes.InstrumentJconfig obj)
        {
            string objStr = OWBJson.stringify(obj);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_ISP_INSTRUMENT_JCONFIG, ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        //public void RefreshStream(LoginType loginType, StreamType streamType)
        //{
        //    if (IsConnected)
        //    {
        //        StopStream();
        //        StartStream(loginType, streamType);
        //    }
        //}

        private void GrabberRaw(int error, ref StreamSDK.streamsdk_st_buffer buffer, IntPtr user_data)
        {
            if (error != StreamSDK.STREAMSDK_EC_OK)
            {
                RawValues = null;
                return;
            }

            int width = Width;
            int height = Height;
            if (buffer.buf_size != 2 * width * height)
            {
                return;
            }

            try
            {
                ushort[,] values = new ushort[width, height];
                unsafe
                {
                    ushort* sPtr = (ushort*)(buffer.buf_ptr);
                    int i = 0;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            values[x, y] = sPtr[i++];
                        }
                    }
                }
                RawValues = values;
            }
            catch { }
        }

        public bool GetRawFrame(out ushort[,] values)
        {
            lock (CameraRawLock)
            {
                values = null;

                if (RawValues != null && RawValues.GetLength(0) == Width && RawValues.GetLength(1) == Height)
                {
                    values = RawValues;
                    return true;
                }
                return false;
            }
        }

        public bool GetTemperatureFrame(out float[,] values)
        {
            lock (CameraRawLock)
            {
                values = null;

                if (RawValues != null && RawValues.GetLength(0) == Width && RawValues.GetLength(1) == Height)
                {
                    values = new float[Width, Height];
                    for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Height; j++)
                        {
                            if (RawValues[i, j] < From)
                            {
                                values[i, j] = LUT[From];
                            }
                            else if (RawValues[i, j] > To)
                            {
                                values[i, j] = LUT[To];
                            }
                            else
                            {
                                values[i, j] = LUT[RawValues[i, j]];
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        private void GrabberH264(int error, ref StreamSDK.streamsdk_st_image image, IntPtr user_data)
        {
            if (error != StreamSDK.STREAMSDK_EC_OK)
            {
                return;
            }

            if (image.img_h != Height || image.img_w != Width)
            {
                return;
            }

            try
            {
                using (Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb))
                {
                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                    int stride = bmpData.Stride;
                    int offset = stride - Width * 3;

                    unsafe
                    {
                        byte* dPtr = (byte*)(bmpData.Scan0);
                        byte* sPtr = (byte*)image.img_ptr;
                        int i = 0;
                        for (int y = 0; y < Height; y++)
                        {
                            for (int x = 0; x < Width; x++)
                            {
                                dPtr[0] = sPtr[i++];
                                dPtr[1] = sPtr[i++];
                                dPtr[2] = sPtr[i++];
                                dPtr += 3;
                            }
                            dPtr += offset;
                        }
                    }
                    bitmap.UnlockBits(bmpData);
                    ChannelImage = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
                }
            }
            catch
            {

            }
        }

        public bool GetH264Frame(out Image image)
        {
            lock (CameraH264Lock)
            {
                image = ChannelImage;
                return true;
            }
        }

        public bool PutStreamVideoPri(int value)
        {
            OWBTypes.StreamVideoPri_BitRate obj = new OWBTypes.StreamVideoPri_BitRate();
            obj.BitRate = value;
            string objStr = OWBJson.stringify(obj);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_STREAM_VIDEO_PRI, ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public bool PutStreamVideoSub(int value)
        {
            OWBTypes.StreamVideoSub_BitRate obj = new OWBTypes.StreamVideoSub_BitRate();
            obj.BitRate = value;
            string objStr = OWBJson.stringify(obj);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_STREAM_VIDEO_SUB, ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        private string GetAdminBootID()
        {
            string bootID = string.Empty;
            byte[] buf = new byte[1024];
            byte[] passwordBytes = new byte[0];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_ADMIN_BOOT_ID, ref buf, ref length)))
            {
                return bootID;
            }
            bootID = OWBJson.parse<string>(OWBString.BytesToString(buf, Encoding.UTF8));
            return bootID;
        }

        public bool PostReBoot()
        {
            bool flag = false;
            byte[] buf = new byte[1024];
            byte[] passwordBytes = new byte[0];
            uint length = 0;
            if (OWBTypes.CheckStatus(RestSDK.Post(IP, (ushort)PORT, URL_ADMIN_REBOOT, ref buf, ref length)))
            {
                flag = true;
            }
            flag = false;
            BootID = GetAdminBootID();
            return flag;
        }

        public Dictionary<string, object> GetUserList()
        {
            Dictionary<string, object> userList = new Dictionary<string, object>();
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_ADMIN_USERS, ref buf, ref length)))
            {
                return userList;
            }
            try
            {
                userList = OWBJson.parse(OWBString.BytesToString(buf, Encoding.UTF8));
            }
            catch
            {

            }
            return userList;
        }

        public bool UpdateUser(string userName, string password, string userGroup)
        {
            OWBTypes.UserProperty obj = new OWBTypes.UserProperty();
            obj.UserGroup = userGroup;
            obj.Password = password;
            string objStr = OWBJson.stringify(obj);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, string.Format(URL_ADMIN_USER_OPERATE, userName), ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public bool DeleteUser(string userName)
        {
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Delete(IP, (ushort)PORT, string.Format(URL_ADMIN_USER_OPERATE, userName), ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public string[] GetPalettes()
        {
            string[] palettes = null;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_TRAY_PRESETS, ref buf, ref length)))
            {
                return palettes;
            }
            try
            {
                string[] paletteDefaults = new string[0];
                string[] paletteCustoms = new string[0];
                paletteDefaults = OWBJson.parse<string[]>(OWBString.BytesToString(buf, Encoding.UTF8));
                if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_ISP_TRAY_CUSTOMS, ref buf, ref length)))
                {
                    paletteCustoms = OWBJson.parse<string[]>(OWBString.BytesToString(buf, Encoding.UTF8));
                }
                palettes = new string[paletteDefaults.Length + paletteCustoms.Length];
                for (int i = 0; i < paletteDefaults.Length; i++)
                {
                    palettes[i] = paletteDefaults[i];
                }
                for (int i = 0; i < paletteCustoms.Length; i++)
                {
                    palettes[paletteDefaults.Length + i] = paletteCustoms[i];
                }
            }
            catch
            {

            }
            return palettes;
        }

        public string GetCurrentPlt()
        {
            string pltName = string.Empty;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_TRAY_PLT, ref buf, ref length)))
            {
                return pltName;
            }
            try
            {
                OWBTypes.OsdTrayPlt plt = OWBJson.parse<OWBTypes.OsdTrayPlt>(OWBString.BytesToString(buf, Encoding.UTF8));
                pltName = plt.Name;
            }
            catch
            {

            }
            return pltName;
        }

        public bool PutCurrentPlt(string plt)
        {
            OWBTypes.OsdTrayPlt obj = new OWBTypes.OsdTrayPlt();
            obj.Name = plt;
            obj.Inverse = false;
            string objStr = OWBJson.stringify(obj);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_OSD_TRAY_PLT, ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public string[] GetCaptureModes()
        {
            string[] captureModes = null;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_CAPTURE_MODES, ref buf, ref length)))
            {
                return captureModes;
            }
            try
            {
                captureModes = OWBJson.parse<string[]>(OWBString.BytesToString(buf, Encoding.UTF8));
            }
            catch
            {

            }
            return captureModes;
        }

        public string GetCurrentCaptureMode()
        {
            string captureMode = string.Empty;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_CAPTURE_MODE, ref buf, ref length)))
            {
                return captureMode;
            }
            try
            {
                captureMode = OWBJson.parse<string>(OWBString.BytesToString(buf, Encoding.UTF8));
            }
            catch
            {

            }
            return captureMode;
        }

        public bool PutCurrentCaptureMode(string mode)
        {
            byte[] buf = new byte[1024];
            string objStr = OWBJson.stringify(mode);
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_CAPTURE_MODE, ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public bool GetPltVisible()
        {
            bool visible = false;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_LAYOUT_CBAR, ref buf, ref length)))
            {
                return visible;
            }
            try
            {
                OWBTypes.OSDProperty osdCbar = OWBJson.parse<OWBTypes.OSDProperty>(OWBString.BytesToString(buf, Encoding.UTF8));
                visible = osdCbar.visible;
            }
            catch
            {

            }
            return visible;
        }

        public bool PutPltVisible(bool visible)
        {
            byte[] buf = new byte[1024];
            uint length = 0;
            if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_LAYOUT_CBAR, ref buf, ref length)))
            {
                OWBTypes.OSDProperty osdCbar = OWBJson.parse<OWBTypes.OSDProperty>(OWBString.BytesToString(buf, Encoding.UTF8));
                osdCbar.visible = visible;
                string objStr = OWBJson.stringify(osdCbar);
                OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
                if (OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_OSD_LAYOUT_CBAR, ref buf, ref length)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool GetTitleVisible()
        {
            bool visible = false;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_LAYOUT_TITLE, ref buf, ref length)))
            {
                return visible;
            }
            try
            {
                OWBTypes.OSDProperty osdTitle = OWBJson.parse<OWBTypes.OSDProperty>(OWBString.BytesToString(buf, Encoding.UTF8));
                visible = osdTitle.visible;
            }
            catch
            {

            }
            return visible;
        }

        public bool PutTitleVisible(bool visible)
        {
            byte[] buf = new byte[1024];
            uint length = 0;
            if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_LAYOUT_TITLE, ref buf, ref length)))
            {
                OWBTypes.OSDProperty osdTitle = OWBJson.parse<OWBTypes.OSDProperty>(OWBString.BytesToString(buf, Encoding.UTF8));
                osdTitle.visible = visible;
                string objStr = OWBJson.stringify(osdTitle);
                OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
                if (OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_OSD_LAYOUT_TITLE, ref buf, ref length)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool DelPoint(string markerName)
        {
            byte[] buf = new byte[0];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Delete(IP, (ushort)PORT, string.Format(URL_ISP_INSTRUMENT_OBJECTS_SPOTITEM, markerName), ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public bool DelRegion(string markerName)
        {
            byte[] buf = new byte[0];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Delete(IP, (ushort)PORT, string.Format(URL_ISP_INSTRUMENT_OBJECTS_REGIONITEM, markerName), ref buf, ref length)))
            {
                return false;
            }
            return true;
        }


        public bool DelLine(string markerName)
        {
            byte[] buf = new byte[0];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Delete(IP, (ushort)PORT, string.Format(URL_ISP_INSTRUMENT_OBJECTS_LINEITEM, markerName), ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public List<MarkerItem> GetMarkerItems()
        {
            List<MarkerItem> markerItemList = new List<MarkerItem>();
            byte[] buf = new byte[1024];
            uint length = 0;
            if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_ISP_INSTRUMENT_OBJECTS_SPOTS, ref buf, ref length)))
            {
                try
                {
                    string[] tempLabels = OWBJson.parse<string[]>(OWBString.BytesToString(buf, Encoding.UTF8));
                    for (int i = 0; i < tempLabels.Length; i++)
                    {
                        markerItemList.Add(new MarkerItem(tempLabels[i], MarkerType.Spot));
                    }
                }
                catch { }
            }
            if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_ISP_INSTRUMENT_OBJECTS_REGIONS, ref buf, ref length)))
            {
                try
                {
                    string[] tempLabels = OWBJson.parse<string[]>(OWBString.BytesToString(buf, Encoding.UTF8));
                    for (int i = 0; i < tempLabels.Length; i++)
                    {
                        markerItemList.Add(new MarkerItem(tempLabels[i], MarkerType.Region));
                    }
                }
                catch { }
            }
            if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_ISP_INSTRUMENT_OBJECTS_LINES, ref buf, ref length)))
            {
                try
                {
                    string[] tempLabels = OWBJson.parse<string[]>(OWBString.BytesToString(buf, Encoding.UTF8));
                    for (int i = 0; i < tempLabels.Length; i++)
                    {
                        markerItemList.Add(new MarkerItem(tempLabels[i], MarkerType.Line));
                    }
                }
                catch { }
            }
            return markerItemList;
        }

        public bool GetTimeVisible()
        {
            bool visible = false;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_LAYOUT_TIME, ref buf, ref length)))
            {
                return visible;
            }
            try
            {
                OWBTypes.OSDProperty osdTime = OWBJson.parse<OWBTypes.OSDProperty>(OWBString.BytesToString(buf, Encoding.UTF8));
                visible = osdTime.visible;
            }
            catch
            {

            }
            return visible;
        }

        public bool PutTimeVisible(bool visible)
        {
            byte[] buf = new byte[1024];
            uint length = 0;
            if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_LAYOUT_TIME, ref buf, ref length)))
            {
                OWBTypes.OSDProperty osdTime = OWBJson.parse<OWBTypes.OSDProperty>(OWBString.BytesToString(buf, Encoding.UTF8));
                osdTime.visible = visible;
                string objStr = OWBJson.stringify(osdTime);
                OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
                if (OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_OSD_LAYOUT_TIME, ref buf, ref length)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool GetUnitVisible()
        {
            bool visible = false;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_LAYOUT_UNIT, ref buf, ref length)))
            {
                return visible;
            }
            try
            {
                OWBTypes.OSDProperty osdUnit = OWBJson.parse<OWBTypes.OSDProperty>(OWBString.BytesToString(buf, Encoding.UTF8));
                visible = osdUnit.visible;
            }
            catch
            {

            }
            return visible;
        }

        public bool PutUnitVisible(bool visible)
        {
            byte[] buf = new byte[1024];
            uint length = 0;
            if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_LAYOUT_UNIT, ref buf, ref length)))
            {
                OWBTypes.OSDProperty osdUnit = OWBJson.parse<OWBTypes.OSDProperty>(OWBString.BytesToString(buf, Encoding.UTF8));
                osdUnit.visible = visible;
                string objStr = OWBJson.stringify(osdUnit);
                OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
                if (OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_OSD_LAYOUT_UNIT, ref buf, ref length)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool GetEmissivityVisible()
        {
            bool visible = false;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_LAYOUT_EMISSIVITY, ref buf, ref length)))
            {
                return visible;
            }
            try
            {
                OWBTypes.OSDProperty osdEmissivity = OWBJson.parse<OWBTypes.OSDProperty>(OWBString.BytesToString(buf, Encoding.UTF8));
                visible = osdEmissivity.visible;
            }
            catch
            {

            }
            return visible;
        }

        public bool PutEmissivityVisible(bool visible)
        {
            byte[] buf = new byte[1024];
            uint length = 0;
            if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_OSD_LAYOUT_EMISSIVITY, ref buf, ref length)))
            {
                OWBTypes.OSDProperty osdEmissivity = OWBJson.parse<OWBTypes.OSDProperty>(OWBString.BytesToString(buf, Encoding.UTF8));
                osdEmissivity.visible = visible;
                string objStr = OWBJson.stringify(osdEmissivity);
                OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
                if (OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_OSD_LAYOUT_EMISSIVITY, ref buf, ref length)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool GetMaxMinVisible()
        {
            bool visible = false;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_ISP_INSTRUMENT_OBJECTS_REGIONS, ref buf, ref length)))
            {
                return visible;
            }
            try
            {
                OWBTypes.PolygonMarker regionMarker = OWBJson.parse<OWBTypes.PolygonMarker>(OWBString.BytesToString(buf, Encoding.UTF8));
                visible = regionMarker.Osd_visible;
            }
            catch
            {

            }
            return visible;
        }

        public bool PutMaxMinVisible(bool visible)
        {
            OWBTypes.PolygonMarker regionMarker = new OWBTypes.PolygonMarker();
            regionMarker.PointList = new OWBTypes.Pos[4];
            regionMarker.PointList[0] = new OWBTypes.Pos();
            regionMarker.PointList[0].X = 10;
            regionMarker.PointList[0].Y = 10;
            regionMarker.PointList[1] = new OWBTypes.Pos();
            regionMarker.PointList[1].X = Width - 10;
            regionMarker.PointList[1].Y = 10;
            regionMarker.PointList[2] = new OWBTypes.Pos();
            regionMarker.PointList[2].X = Width - 10;
            regionMarker.PointList[2].Y = Height - 10;
            regionMarker.PointList[3] = new OWBTypes.Pos();
            regionMarker.PointList[3].X = 10;
            regionMarker.PointList[3].Y = Height - 10;
            regionMarker.Osd_visible = visible;
            regionMarker.Emission = 0.97F;
            regionMarker.Distance = 1;
            regionMarker.ReflectionTemp = 20.0F;
            regionMarker.Label = "区域";

            string objStr = OWBJson.stringify(regionMarker);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, string.Format(URL_ISP_INSTRUMENT_OBJECTS_REGIONS, "r1"), ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public float GetIspTItem(Point p)
        {
            float temperature = 0;
            byte[] buf = new byte[1024];
            uint length = 0;
            try
            {
                if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, string.Format(URL_ISP_T, p.X, p.Y), ref buf, ref length)))
                {
                    return temperature;
                }
                OWBTypes.IspTItem ispTItem = OWBJson.parse<OWBTypes.IspTItem>(OWBString.BytesToString(buf, Encoding.UTF8));
                temperature = ispTItem.T;
            }
            catch
            { }
            return temperature;
        }

        public void PostIspAF()
        {
            byte[] buf = new byte[0];
            uint length = 0;
            RestSDK.Post(IP, (ushort)PORT, URL_ISP_AF, ref buf, ref length);
        }

        public bool PostIspAFXY(OWBTypes.Pos pos)
        {
            string objStr = OWBJson.stringify(pos);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Post(IP, (ushort)PORT, URL_ISP_AF, ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public void PostNearFocus()
        {
            byte[] buf = new byte[0];
            uint length = 0;
            RestSDK.Post(IP, (ushort)PORT, string.Format(URL_PERI_FOCUS, "near", 10), ref buf, ref length);
        }

        public void PostFarFocus()
        {
            byte[] buf = new byte[0];
            uint length = 0;
            RestSDK.Post(IP, (ushort)PORT, string.Format(URL_PERI_FOCUS, "far", 10), ref buf, ref length);
        }

        public OWBTypes.SensorLens[] GetSensorLens()
        {
            OWBTypes.SensorLens[] sensorLens = null;
            byte[] buf = new byte[1024];
            uint length = 0;
            try
            {
                if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_SENSOR_LENS, ref buf, ref length)))
                {
                    return sensorLens;
                }
                sensorLens = OWBJson.parse<OWBTypes.SensorLens[]>(OWBString.BytesToString(buf, Encoding.UTF8));
            }
            catch
            { }
            return sensorLens;
        }

        public OWBTypes.SensorT_Range[] GetSensorT_Range()
        {
            OWBTypes.SensorT_Range[] sensorT_Range = null;
            byte[] buf = new byte[1024];
            uint length = 0;
            try
            {
                if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_SENSOR_T_RANGE, ref buf, ref length)))
                {
                    return sensorT_Range;
                }
                sensorT_Range = OWBJson.parse<OWBTypes.SensorT_Range[]>(OWBString.BytesToString(buf, Encoding.UTF8));
            }
            catch
            { }
            return sensorT_Range;
        }

        public OWBTypes.SensorLUT[] GetSensorLUTs()
        {
            OWBTypes.SensorLUT[] sensorLUTs = null;
            byte[] buf = new byte[1024];
            uint length = 0;
            try
            {
                if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_SENSOR_LUTS, ref buf, ref length)))
                {
                    return sensorLUTs;
                }
                sensorLUTs = OWBJson.parse<OWBTypes.SensorLUT[]>(OWBString.BytesToString(buf, Encoding.UTF8));
            }
            catch
            { }
            return sensorLUTs;
        }

        public OWBTypes.SensorJConfig GetSensorJConfig()
        {
            OWBTypes.SensorJConfig sensorJConfig = null;
            byte[] buf = new byte[1024];
            uint length = 0;
            try
            {
                if (!OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, URL_SENSOR_J_CONFIG, ref buf, ref length)))
                {
                    return sensorJConfig;
                }
                sensorJConfig = OWBJson.parse<OWBTypes.SensorJConfig>(OWBString.BytesToString(buf, Encoding.UTF8));
            }
            catch
            { }
            return sensorJConfig;
        }

        public bool PutSensorJConfig(OWBTypes.SensorJConfig sensorJConfig)
        {
            string objStr = OWBJson.stringify(sensorJConfig);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, URL_SENSOR_J_CONFIG, ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public bool PutPoint(string markerName, OWBTypes.SpotMarker pointMarker)
        {
            string objStr = OWBJson.serialize(pointMarker);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, string.Format(URL_ISP_INSTRUMENT_OBJECTS_SPOTITEM, markerName), ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public bool PutRegion(string markerName, OWBTypes.PolygonMarker regionMarker)
        {
            string objStr = OWBJson.serialize(regionMarker);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, string.Format(URL_ISP_INSTRUMENT_OBJECTS_REGIONITEM, markerName), ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public bool PutAlarm(string markerName, MarkerType markerType, OWBTypes.Alarm alarm)
        {
            string url = string.Empty;
            switch (markerType)
            {
                case MarkerType.Spot:
                    url = string.Format(URL_ISP_INSTRUMENT_OBJECTS_SPOTITEM, markerName);
                    break;
                case MarkerType.Region:
                    url = string.Format(URL_ISP_INSTRUMENT_OBJECTS_REGIONITEM, markerName);
                    break;
                case MarkerType.Line:
                    url = string.Format(URL_ISP_INSTRUMENT_OBJECTS_LINEITEM, markerName);
                    break;
            }
            string objStr = OWBJson.stringify(alarm);
            byte[] buf = new byte[1024];
            OWBString.StringToBytes(objStr, objStr.Length, Encoding.UTF8).CopyTo(buf, 0);
            uint length = 0;
            if (!OWBTypes.CheckStatus(RestSDK.Put(IP, (ushort)PORT, url, ref buf, ref length)))
            {
                return false;
            }
            return true;
        }

        public byte[] Snapshot()
        {
            string url = URL_OSD_SNAPSHOT;
            byte[] buf = new byte[1024];
            uint length = 0;
            if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, url, ref buf, ref length)))
            {
                OWBTypes.FileCache fileCache = OWBJson.parse<OWBTypes.FileCache>(OWBString.BytesToString(buf, Encoding.UTF8));
                if (fileCache != null)
                {
                    buf = new byte[1024000];
                    if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, fileCache.Path, ref buf, ref length)))
                    {
                        byte[] refBuf = new byte[length + HEADERLENGTH];
                        Array.Copy(buf, refBuf, length + HEADERLENGTH);
                        return refBuf;
                    }
                }
            }
            return null;
        }

        public byte[] Snapshot(SnapType snapType)
        {
            string url = string.Empty;
            switch (snapType)
            {
                case SnapType.ISP:
                    url = URL_ISP_SNAPSHOT;
                    break;
                case SnapType.OSD:
                    url = URL_OSD_SNAPSHOT;
                    break;
                case SnapType.T:
                    url = URL_T_SNAPSHOT;
                    break;
            }
            byte[] buf = new byte[1024];
            uint length = 0;
            if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, url, ref buf, ref length)))
            {
                OWBTypes.FileCache fileCache = OWBJson.parse<OWBTypes.FileCache>(OWBString.BytesToString(buf, Encoding.UTF8));
                buf = new byte[1024000];
                if (OWBTypes.CheckStatus(RestSDK.Get(IP, (ushort)PORT, fileCache.Path, ref buf, ref length)))
                {
                    byte[] refBuf = new byte[length];
                    Array.Copy(buf, refBuf, length);
                    return refBuf;
                }
            }
            return null;
        }

        public Bitmap CreateBitmap(byte[] buffer)
        {
            using (BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
            {
                try
                {
                    br.BaseStream.Position = 2;
                    int width = (int)IPAddress.HostToNetworkOrder(br.ReadInt16());
                    int height = (int)IPAddress.HostToNetworkOrder(br.ReadInt16());
                    int depth = br.ReadByte();
                    int type = br.ReadByte();
                    uint linesize = br.ReadUInt32();
                    switch (type)
                    {
                        case 0:
                            br.BaseStream.Position = 16;
                            ushort[,] raw = new ushort[width, height];
                            for (int i = 0; i < height; i++)
                            {
                                for (int j = 0; j < width; j++)
                                {
                                    raw[j, i] = br.ReadUInt16();
                                }
                            }
                            return (Bitmap)ToImage("grey", raw);
                        case 1:
                            br.BaseStream.Position = 16;
                            int framesize = height * width * 3 / 2;
                            int imgsize = width * height;
                            byte[] rgb = new byte[3 * imgsize];
                            return ConvertYUV2RGB(br.ReadBytes(framesize), rgb, width, height);
                    }
                }
                catch
                {

                }
            }
            return null;
        }

        static double[,] YUV2RGB_CONVERT_MATRIX = new double[3, 3] { { 1, 0, 1.4022 }, { 1, -0.3456, -0.7145 }, { 1, 1.771, 0 } };
        private Bitmap ConvertYUV2RGB(byte[] yuvFrame, byte[] rgbFrame, int width, int height)
        {
            int uIndex = width * height;
            int vIndex = uIndex + ((width * height) >> 2);
            int gIndex = width * height;
            int bIndex = gIndex * 2;

            int temp = 0;

            Bitmap bm = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    temp = (int)(yuvFrame[y * width + x] + (yuvFrame[vIndex + (y / 2) * (width / 2) + x / 2] - 128) * YUV2RGB_CONVERT_MATRIX[0, 2]);
                    rgbFrame[y * width + x] = (byte)(temp < 0 ? 0 : (temp > 255 ? 255 : temp));

                    temp = (int)(yuvFrame[y * width + x] + (yuvFrame[uIndex + (y / 2) * (width / 2) + x / 2] - 128) * YUV2RGB_CONVERT_MATRIX[1, 1] + (yuvFrame[vIndex + (y / 2) * (width / 2) + x / 2] - 128) * YUV2RGB_CONVERT_MATRIX[1, 2]);
                    rgbFrame[gIndex + y * width + x] = (byte)(temp < 0 ? 0 : (temp > 255 ? 255 : temp));

                    temp = (int)(yuvFrame[y * width + x] + (yuvFrame[uIndex + (y / 2) * (width / 2) + x / 2] - 128) * YUV2RGB_CONVERT_MATRIX[2, 1]);
                    rgbFrame[bIndex + y * width + x] = (byte)(temp < 0 ? 0 : (temp > 255 ? 255 : temp));
                    Color c = Color.FromArgb(rgbFrame[y * width + x], rgbFrame[gIndex + y * width + x], rgbFrame[bIndex + y * width + x]);
                    bm.SetPixel(x, y, c);
                }
            }
            return bm;
        }

        public Image ToImage(string paletteKey, ushort[,] ADValues)
        {
            if (ADValues == null)
            {
                return null;
            }

            int width = ADValues.GetLength(0);
            int height = ADValues.GetLength(1);
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int stride = bmpData.Stride;
            int offset = stride - width * 3;
            unsafe
            {
                byte* dPtr = (byte*)(bmpData.Scan0);

                ushort min = ushort.MaxValue;
                ushort max = ushort.MinValue;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (ADValues[x, y] > max)
                        {
                            max = ADValues[x, y];
                        }
                        if (ADValues[x, y] < min)
                        {
                            min = ADValues[x, y];
                        }
                    }
                }
                int span = max - min;
                if (span < 1)
                {
                    span = 1;
                }

                if (!OWBGlobal.Palettes.ContainsKey(paletteKey))
                {
                    return null;
                }
                Color[] palette = OWBGlobal.Palettes[paletteKey];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int diff = ADValues[x, y] - min;
                        if (diff < 0)
                        {
                            diff = 0;
                        }
                        if (diff > span)
                        {
                            diff = span;
                        }
                        byte index = Convert.ToByte(diff * (palette.Length - 1) / span);

                        if (index < 0)
                        {
                            index = 0;
                        }
                        if (index > palette.Length - 1)
                        {
                            index = Convert.ToByte(palette.Length - 1);
                        }

                        dPtr[0] = palette[index].B;
                        dPtr[1] = palette[index].G;
                        dPtr[2] = palette[index].R;
                        dPtr += 3;
                    }
                    dPtr += offset;
                }
            }
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }

        public Image ToImage(string paletteKey, float[,] tValues)
        {
            try
            {
                if (tValues == null)
                {
                    return null;
                }

                int width = tValues.GetLength(0);
                int height = tValues.GetLength(1);
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                int stride = bmpData.Stride;
                int offset = stride - width * 3;
                unsafe
                {
                    byte* dPtr = (byte*)(bmpData.Scan0);

                    float min = float.MaxValue;
                    float max = float.MinValue;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (tValues[x, y] > max)
                            {
                                max = tValues[x, y];
                            }
                            if (tValues[x, y] < min)
                            {
                                min = tValues[x, y];
                            }
                        }
                    }
                    float span = max - min;
                    if (span < 1)
                    {
                        span = 1;
                    }

                    if (!OWBGlobal.Palettes.ContainsKey(paletteKey))
                    {
                        return null;
                    }
                    Color[] palette = OWBGlobal.Palettes[paletteKey];
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            float diff = tValues[x, y] - min;
                            if (diff < 0)
                            {
                                diff = 0;
                            }
                            if (diff > span)
                            {
                                diff = span;
                            }
                            byte index = Convert.ToByte(diff * (palette.Length - 1) / span);

                            if (index < 0)
                            {
                                index = 0;
                            }
                            if (index > palette.Length - 1)
                            {
                                index = Convert.ToByte(palette.Length - 1);
                            }

                            dPtr[0] = palette[index].B;
                            dPtr[1] = palette[index].G;
                            dPtr[2] = palette[index].R;
                            dPtr += 3;
                        }
                        dPtr += offset;
                    }
                }
                bitmap.UnlockBits(bmpData);
                return bitmap;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
