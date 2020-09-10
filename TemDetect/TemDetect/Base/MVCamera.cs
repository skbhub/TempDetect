using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvCamCtrl.NET;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;

namespace TempDetect
{
    public struct DeviceParm { public float ExposureTime; public float Gain; public float ResultingFrameRate; public uint HearTime; }

    public class MVCamera
    {
        ~MVCamera()
        {
            StopGrab();
            CloseDevice();
        }
       static MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
       static List<string> CameraList = new List<string>();
        private MyCamera m_MyCamera = new MyCamera();
        public bool m_bGrabbing = false;
        public bool m_Recoding = false;
        public bool IsOpen;
        UInt32 m_nBufSizeForDriver = 0;
        // ch:用于从驱动获取图像的缓存 | en:Buffer for getting image from driver
        IntPtr m_BufForDriver;
        Task task = null; Task task1 = null;
        //MyCamera.MV_CC_RECORD_PARAM stRecordPar = new MyCamera.MV_CC_RECORD_PARAM();
        private static Object BufForDriverLock = new Object();

        // ch:用于保存图像的缓存 | en:Buffer for saving image
        UInt32 m_nBufSizeForSaveImage = 0;
        IntPtr m_BufForSaveImage;
        MyCamera.MV_FRAME_OUT_INFO_EX m_stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();


        /// <summary>
        /// 枚举相机设备
        /// </summary>
        /// <returns></returns>
        public static void EnumDevice(ref List<string> RCameraList)
        {
            System.GC.Collect();
            CameraList.Clear();
            m_stDeviceList.nDeviceNum = 0;
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_stDeviceList);
            if (nRet == MyCamera.MV_OK)
            {
                for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
                {
                    MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                    if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                        if (gigeInfo.chUserDefinedName != "")
                        {
                            CameraList.Add("GEV: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                        }
                        else
                        {
                            CameraList.Add("GEV: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                        }
                    }
                    else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                    {
                        MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                        if (usbInfo.chUserDefinedName != "")
                        {
                            CameraList.Add("U3V: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                        }
                        else
                        {
                            CameraList.Add("U3V: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                        }
                    }
                }
                RCameraList = CameraList;
            }
            else
            {
                RCameraList = null;
            }
        }
        /// <summary>
        /// 打开相机
        /// </summary>
        /// <param name="DeviceName">相机名称</param>
        /// <returns></returns>
        public bool OpenDevice(string DeviceName)
        {
            int deviceIndex = 0;
            if (CameraList.Count > 0)
            {
                for (int i = 0; i < CameraList.Count; i++)
                {
                    if (CameraList[i] == DeviceName)
                    {
                        deviceIndex = i;
                    }
                }
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[deviceIndex], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (m_MyCamera == null)
                {
                    m_MyCamera = new MyCamera();
                }
                int nRet = m_MyCamera.MV_CC_CreateDevice_NET(ref device);
                nRet = m_MyCamera.MV_CC_OpenDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    m_MyCamera.MV_CC_DestroyDevice_NET();
                    IsOpen = false;

                    return IsOpen;
                }
                if (MyCamera.MV_OK == nRet)
                {
                    // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                    if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        int nPacketSize = m_MyCamera.MV_CC_GetOptimalPacketSize_NET();
                        if (nPacketSize > 0)
                        {
                            nRet = m_MyCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                            if (nRet != MyCamera.MV_OK)
                            {
                                IsOpen = false;

                                return IsOpen;
                            }
                        }
                        else
                        {
                            IsOpen = false;

                            return IsOpen;

                        }
                        // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
                    }
                    m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                    m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                   m_MyCamera.MV_CC_SetBoolValue_NET("AcquisitionFrameRateEnable", true);
                    m_MyCamera.MV_CC_SetEnumValueByString_NET("PixelFormat", "BayerBG8");
                    IsOpen = true;

                    return IsOpen;

                }
                IsOpen = false;

                return IsOpen;
            }
            IsOpen = false;

            return IsOpen;
        }
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        public bool CloseDevice()
        {
            if (m_bGrabbing == true)
            {
                m_bGrabbing = false;
                task1.Wait();
                task.Wait();
            }
            if (m_BufForDriver != IntPtr.Zero)
            {
                Marshal.Release(m_BufForDriver);
            }
            if (m_BufForSaveImage != IntPtr.Zero)
            {
                Marshal.Release(m_BufForSaveImage);
            }
            try
            {
                // ch:关闭设备 | en:Close Device
                m_MyCamera.MV_CC_CloseDevice_NET();
                m_MyCamera.MV_CC_DestroyDevice_NET();
                IsOpen = false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 停止采集
        /// </summary>
        /// <returns></returns>
        public bool StopGrab()
        {
            try
            {
                m_bGrabbing = false;
                m_Recoding = false;
                if (task != null)
                {
                    task.Wait();
                }
                if (task1 != null)
                {
                    task1.Wait();
                }

                m_MyCamera.MV_CC_StopGrabbing_NET();
                m_MyCamera.MV_CC_StopRecord_NET();
                task = null;
                task1 = null;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void StopRecord()
        {
            m_Recoding = false;
            if (task1 != null)
            {
                task1.Wait();
            }
            m_MyCamera.MV_CC_StopRecord_NET();
            task1 = null;
        }
        /// <summary>
        /// 开始采集
        /// </summary>
        /// <returns></returns>
        public bool StartGrab(IntPtr PictureBoxHandle)
        {
            m_bGrabbing = true;
            task = Task.Run(() =>
            {
                ReceiveThreadProcess(PictureBoxHandle);
            });
            m_stFrameInfo.nFrameLen = 0;//取流之前先清除帧长度
            m_stFrameInfo.enPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
            int nRet = m_MyCamera.MV_CC_StartGrabbing_NET();//开始采集
            if (MyCamera.MV_OK != nRet)
            {
                StopGrab();
                return false;
            }
            return true;
        }
        /// <summary>
        /// 录制视频
        /// </summary>
        /// <param name="PictureBoxHandle"></param>
        /// <returns></returns>
        public bool Record(IntPtr PictureBoxHandle, string path)
        {

            if (path.Length > 0)
            {
                m_bGrabbing = true;
                task = Task.Run(() => {
                    ReceiveThreadProcess(PictureBoxHandle);
                });
                m_stFrameInfo.nFrameLen = 0;//取流之前先清除帧长度
                m_stFrameInfo.enPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
                if (SetRECODING(path))
                {
                    int nRet = m_MyCamera.MV_CC_StartGrabbing_NET();//开始采集
                    if (MyCamera.MV_OK != nRet)
                    {
                        StopGrab();
                        return false;
                    }
                    task1 = Task.Run(() => {
                        ReceiveImageWorkThread(m_MyCamera);
                    });
                }
                else
                {
                    StopGrab();
                    return false;
                }
                return true;
            }
            return false;
        }
        public void ReceiveImageWorkThread(MyCamera obj )
        {
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            m_MyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            uint g_nPayloadSize = stParam.nCurValue;
            int nRet = MyCamera.MV_OK;
            MyCamera device = obj as MyCamera;
            MyCamera.MV_FRAME_OUT_INFO_EX stImageInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
            IntPtr pData = Marshal.AllocHGlobal((int)g_nPayloadSize);
            if (pData == IntPtr.Zero)
            {
                return;
            }

            uint nDataSize = g_nPayloadSize;
            MyCamera.MV_CC_INPUT_FRAME_INFO stInputFrameInfo = new MyCamera.MV_CC_INPUT_FRAME_INFO();
    
            while (m_bGrabbing)
            {
                //Thread.Sleep(1);
                nRet = device.MV_CC_GetOneFrameTimeout_NET(pData, nDataSize, ref stImageInfo, 1000);

                if (nRet == MyCamera.MV_OK)
                {

                    stInputFrameInfo.pData = pData;
                    stInputFrameInfo.nDataLen = stImageInfo.nFrameLen;
                    nRet = device.MV_CC_InputOneFrame_NET(ref stInputFrameInfo);
                    if (MyCamera.MV_OK != nRet)
                    {
                        //Console.WriteLine("Input one frame failed: nRet {0:x8}", nRet);
                    }
                   
                }
                else
                {
                    // Console.WriteLine("No data:{0:x8}", nRet);
                }
            }
        }
        private void ReceiveThreadProcess(IntPtr PictureBoxHandle)
        {
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int nRet = m_MyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }

            UInt32 nPayloadSize = stParam.nCurValue;
            if (nPayloadSize > m_nBufSizeForDriver)
            {
                if (m_BufForDriver != IntPtr.Zero)
                {
                    Marshal.Release(m_BufForDriver);
                }
                m_nBufSizeForDriver = nPayloadSize;
                m_BufForDriver = Marshal.AllocHGlobal((Int32)m_nBufSizeForDriver);
            }

            if (m_BufForDriver == IntPtr.Zero)
            {
                return;
            }

            MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
            MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();

            while (m_bGrabbing)
            {
                Thread.Sleep(1);
                lock (BufForDriverLock)
                {
                    nRet = m_MyCamera.MV_CC_GetOneFrameTimeout_NET(m_BufForDriver, nPayloadSize, ref stFrameInfo, 1000);
                    if (nRet == MyCamera.MV_OK)
                    {
                        m_stFrameInfo = stFrameInfo;
                    }
                    if (!m_bGrabbing)
                    {
                        break;
                    }
                }

                if (nRet == MyCamera.MV_OK)
                {
                    if (RemoveCustomPixelFormats(stFrameInfo.enPixelType))
                    {
                        continue;
                    }
                    stDisplayInfo.hWnd = PictureBoxHandle;
                    stDisplayInfo.pData = m_BufForDriver;
                    stDisplayInfo.nDataLen = stFrameInfo.nFrameLen;
                    stDisplayInfo.nWidth = stFrameInfo.nWidth;
                    stDisplayInfo.nHeight = stFrameInfo.nHeight;
                    stDisplayInfo.enPixelType = stFrameInfo.enPixelType;
                    m_MyCamera.MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);
                }
            }
        }
        // ch:去除自定义的像素格式 | en:Remove custom pixel formats
        private bool RemoveCustomPixelFormats(MyCamera.MvGvspPixelType enPixelFormat)
        {
            Int32 nResult = ((int)enPixelFormat) & (unchecked((Int32)0x80000000));
            if (0x80000000 == nResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 打开或关闭触发模式
        /// </summary>
        /// <param name="key">开关</param>
        public void OpenOrCloseTrigger(bool key)
        {
            if (key)
            {
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
            }
            else
            {
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            }
        }
        /// <summary>
        /// 软件触发一次
        /// </summary>
        /// <returns></returns>
        public bool SoftwareTrigger()
        {
            m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
            int nRet = m_MyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取相机参数
        /// </summary>
        /// <returns></returns>
        public DeviceParm GetDeviceParm()
        {
            DeviceParm deviceParm = new DeviceParm();
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            MyCamera.MVCC_INTVALUE intParam = new MyCamera.MVCC_INTVALUE();
            int nRet = m_MyCamera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);//获取曝光时间
            if (MyCamera.MV_OK == nRet)
            { deviceParm.ExposureTime = stParam.fCurValue; }
            nRet = m_MyCamera.MV_CC_GetFloatValue_NET("Gain", ref stParam);//获取增益
            if (MyCamera.MV_OK == nRet)
            { deviceParm.Gain = stParam.fCurValue; }
            nRet = m_MyCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam);//获取FPS
            if (MyCamera.MV_OK == nRet)
            { deviceParm.ResultingFrameRate = stParam.fCurValue; }
            nRet = m_MyCamera.MV_CC_GetIntValue_NET("GevHeartbeatTimeout", ref intParam);//获取FPS
            if (MyCamera.MV_OK == nRet)
            { deviceParm.HearTime = intParam.nCurValue; }
            return deviceParm;
        }
        /// <summary>
        /// 设置参数，有一个设置失败返回false
        /// </summary>
        /// <param name="ExposureTime">曝光</param>
        /// <param name="Gain">增益</param>
        /// <param name="AcquisitionFrameRate">帧率</param>
        /// <returns></returns>
        public bool SetDeviceParm(float ExposureTime, float Gain, float AcquisitionFrameRate, uint HearTime)
        {
            bool State = false;
            m_MyCamera.MV_CC_SetEnumValue_NET("ExposureAuto", 0);
            int nRet = m_MyCamera.MV_CC_SetFloatValue_NET("ExposureTime", ExposureTime);
            if (nRet != MyCamera.MV_OK)
                State = false;
            else
                State = true;

            m_MyCamera.MV_CC_SetEnumValue_NET("GainAuto", 0);
            nRet = m_MyCamera.MV_CC_SetFloatValue_NET("Gain", Gain);
            if (nRet != MyCamera.MV_OK)
                State = false;
            else
                State = true;

            nRet = m_MyCamera.MV_CC_SetBoolValue_NET("AcquisitionFrameRateEnable", true);
            if (nRet != MyCamera.MV_OK)
                State = false;
            else
                State = true;

            nRet = m_MyCamera.MV_CC_SetFloatValue_NET("AcquisitionFrameRate", AcquisitionFrameRate);
            if (nRet != MyCamera.MV_OK)
                State = false;
            else
                State = true;
            if (SetHearBeatTime(HearTime))
                State = true;
            else
                State = false;

            return State;
        }
        private bool SetRECODING(string RecordingPath)
        {

            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int nRet = m_MyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            MyCamera.MV_CC_RECORD_PARAM stRecordPar = new MyCamera.MV_CC_RECORD_PARAM();
            nRet = m_MyCamera.MV_CC_GetIntValue_NET("Width", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }
            stRecordPar.nWidth = (ushort)stParam.nCurValue;

            nRet = m_MyCamera.MV_CC_GetIntValue_NET("Height", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Get Height failed: nRet {0:x8}", nRet);
                return false;
            }
            stRecordPar.nHeight = (ushort)stParam.nCurValue;

            MyCamera.MVCC_ENUMVALUE stEnumValue = new MyCamera.MVCC_ENUMVALUE();
            nRet = m_MyCamera.MV_CC_GetEnumValue_NET("PixelFormat", ref stEnumValue);
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }
            stRecordPar.enPixelType = (MyCamera.MvGvspPixelType)stEnumValue.nCurValue;

            MyCamera.MVCC_FLOATVALUE stFloatValue = new MyCamera.MVCC_FLOATVALUE();
            nRet = m_MyCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stFloatValue);
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Get Float value failed: nRet {0:x8}", nRet);
                return false;
            }
            // ch:帧率(大于1/16)fps | en:Frame Rate (>1/16)fps
            stRecordPar.fFrameRate = stFloatValue.fCurValue;
            // ch:码率kbps(128kbps-16Mbps) | en:Bitrate kbps(128kbps-16Mbps)
            stRecordPar.nBitRate = 1000;
            // ch:录像格式(仅支持AVI) | en:Record Format(AVI is only supported)
            stRecordPar.enRecordFmtType = MyCamera.MV_RECORD_FORMAT_TYPE.MV_FormatType_AVI;
            string path = DateTime.Now.ToLocalTime().ToString().Replace("/", "");
            path = path.Replace(":", "");
            stRecordPar.strFilePath = RecordingPath + path + ".avi";
            nRet = m_MyCamera.MV_CC_StartRecord_NET(ref stRecordPar);
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Start Record failed: nRet {0:x8}", nRet);
                return false;
            }
            return true;
            //}
            //return false;
        }
        public bool SaveBmp(string path)
        {
            if (false == m_bGrabbing)
            {
                return false;
            }

            if (RemoveCustomPixelFormats(m_stFrameInfo.enPixelType))
            {
                return false;
            }

            IntPtr pTemp = IntPtr.Zero;
            MyCamera.MvGvspPixelType enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
            if (m_stFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8 || m_stFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed)
            {
                pTemp = m_BufForDriver;
                enDstPixelType = m_stFrameInfo.enPixelType;
            }
            else
            {
                UInt32 nSaveImageNeedSize = 0;
                MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

                lock (BufForDriverLock)
                {
                    if (m_stFrameInfo.nFrameLen == 0)
                    {
                        return false;
                    }

                    if (IsMonoData(m_stFrameInfo.enPixelType))
                    {
                        enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                        nSaveImageNeedSize = (uint)m_stFrameInfo.nWidth * m_stFrameInfo.nHeight;
                    }
                    else if (IsColorData(m_stFrameInfo.enPixelType))
                    {
                        enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed;
                        nSaveImageNeedSize = (uint)m_stFrameInfo.nWidth * m_stFrameInfo.nHeight * 3;
                    }
                    else
                    {
                        return false;
                    }

                    if (m_nBufSizeForSaveImage < nSaveImageNeedSize)
                    {
                        if (m_BufForSaveImage != IntPtr.Zero)
                        {
                            Marshal.Release(m_BufForSaveImage);
                        }
                        m_nBufSizeForSaveImage = nSaveImageNeedSize;
                        m_BufForSaveImage = Marshal.AllocHGlobal((Int32)m_nBufSizeForSaveImage);
                    }

                    stConverPixelParam.nWidth = m_stFrameInfo.nWidth;
                    stConverPixelParam.nHeight = m_stFrameInfo.nHeight;
                    stConverPixelParam.pSrcData = m_BufForDriver;
                    stConverPixelParam.nSrcDataLen = m_stFrameInfo.nFrameLen;
                    stConverPixelParam.enSrcPixelType = m_stFrameInfo.enPixelType;
                    stConverPixelParam.enDstPixelType = enDstPixelType;
                    stConverPixelParam.pDstBuffer = m_BufForSaveImage;
                    stConverPixelParam.nDstBufferSize = m_nBufSizeForSaveImage;
                    int nRet = m_MyCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
                    if (MyCamera.MV_OK != nRet)
                    {
                        return false;
                    }
                    pTemp = m_BufForSaveImage;
                }
            }

            lock (BufForDriverLock)
            {
                if (enDstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                {
                    //************************Mono8 转 Bitmap*******************************
                    Bitmap bmp = new Bitmap(m_stFrameInfo.nWidth, m_stFrameInfo.nHeight, m_stFrameInfo.nWidth * 1, PixelFormat.Format8bppIndexed, pTemp);

                    ColorPalette cp = bmp.Palette;
                    // init palette
                    for (int i = 0; i < 256; i++)
                    {
                        cp.Entries[i] = Color.FromArgb(i, i, i);
                    }
                    // set palette back
                    bmp.Palette = cp;
                    bmp.Save(path, ImageFormat.Bmp);
                }
                else
                {
                    //*********************BGR8 转 Bitmap**************************
                    try
                    {
                        Bitmap bmp = new Bitmap(m_stFrameInfo.nWidth, m_stFrameInfo.nHeight, m_stFrameInfo.nWidth * 3, PixelFormat.Format24bppRgb, pTemp);
                        bmp.Save(path, ImageFormat.Bmp);
                    }
                    catch
                    {
                    }
                }
            }
            return true;
        }
        public bool Savejpg(string path)
        {
            if (false == m_bGrabbing)
            {
                return false;
            }

            if (RemoveCustomPixelFormats(m_stFrameInfo.enPixelType))
            {
                return false;
            }

            UInt32 nSaveImageNeedSize = m_nBufSizeForDriver * 3 + 2048;
            if (m_nBufSizeForSaveImage < nSaveImageNeedSize)
            {
                if (m_BufForSaveImage != IntPtr.Zero)
                {
                    Marshal.Release(m_BufForSaveImage);
                }
                m_nBufSizeForSaveImage = nSaveImageNeedSize;
                m_BufForSaveImage = Marshal.AllocHGlobal((Int32)m_nBufSizeForSaveImage);
            }

            MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();

            lock (BufForDriverLock)
            {
                if (m_stFrameInfo.nFrameLen == 0)
                {
                    return false;
                }
                IntPtr pJpgImage = m_BufForSaveImage;
                stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Jpeg;
                stSaveParam.enPixelType = m_stFrameInfo.enPixelType;
                stSaveParam.pData = m_BufForDriver;
                stSaveParam.nDataLen = m_stFrameInfo.nFrameLen;
                stSaveParam.nHeight = m_stFrameInfo.nHeight;
                stSaveParam.nWidth = m_stFrameInfo.nWidth;
                stSaveParam.pImageBuffer = pJpgImage;
                stSaveParam.nBufferSize = m_nBufSizeForSaveImage;
                stSaveParam.nJpgQuality = 80;
                int nRet = m_MyCamera.MV_CC_SaveImageEx_NET(ref stSaveParam);
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
            }

            Byte[] bArrSaveImage = new Byte[stSaveParam.nImageLen];
            Marshal.Copy(m_BufForSaveImage, bArrSaveImage, 0, (int)stSaveParam.nImageLen);
            try
            {
                FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);
                file.Write(bArrSaveImage, 0, (int)stSaveParam.nImageLen);
                file.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private Boolean IsColorData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }
        private Boolean IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;

                default:
                    return false;
            }
        }
        /// <summary>
        /// 设置心跳时间
        /// </summary>
        /// <param name="HeartBeatTime"></param>
        /// <returns></returns>
        private bool SetHearBeatTime(uint HeartBeatTime)
        {
            uint tempTime = HeartBeatTime > 500 ? HeartBeatTime : 500;
            int temp = m_MyCamera.MV_CC_SetIntValue_NET("GevHeartbeatTimeout", tempTime);
            if (MyCamera.MV_OK != temp)
            {
                return false;
            }
            return true;
        }
    }
}



