using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TempDetect
{
    /// <summary>
    /// 发送串口命令，返回相应值的松下PLC类
    /// </summary>
    public class PanasonicPLC
    {
        SerialPort MewtocolSP = new SerialPort();
        MewtocolClass MC = new MewtocolClass();
        //public PanasonicPLC()
        //{
        //  SerialPort MewtocolSP = new SerialPort();
        //}
        /// <summary>
        /// 设置串口连接的各个参数
        /// </summary>
        ///  <param name="COMx">串口号 "COM0-COM16"</param>
        /// <param name="baudRate">波特率 9600，19200，38400，57600，115200</param>
        /// <param name="dataBits">数据位 7（7位），8（8位）</param>
        /// <param name="stopBits">停止位 1（1位），2（2位）</param>
        /// <param name="parity">校验 Partity.ODD(奇校验)，Partity.EVEN(偶校验)，Partity.NONE(无校验) ）</param>
        /// <returns></returns>
        public void MewtocolSet(string COMx, int baudRate, int dataBits, int stopBits, Parity parity)
        {
            MewtocolSP.PortName = COMx;            //串口号
            MewtocolSP.BaudRate = baudRate;       //波特率
            MewtocolSP.DataBits = dataBits;       //数据位 8位
            if (stopBits == 2)
            {
                MewtocolSP.StopBits = StopBits.Two;  //停止位 1位
            }
            //else if (stopBits == 1.5)
            //{
            //    BasicClass.MewtocolSP.StopBits = StopBits.OnePointFive;  //停止位 1.5位
            //}
            else
            {
                MewtocolSP.StopBits = StopBits.One;       //所有其他设置停止位都为1位
            }
            MewtocolSP.Parity = parity;      //校验位 无校验 Parity.Odd
        }

        /// <summary>
        /// 获取一个值，指示串口是否处于打开状态
        /// </summary>
        /// <returns>是或否</returns>
        public bool IsOpen()
        {
            return MewtocolSP.IsOpen;
        }
        /// <summary>
        /// 打开PLC操作对象的串口连接
        /// </summary>
        /// <returns></returns>
        public short MewtocolOpen()
        {
            if (!MewtocolSP.IsOpen)
            {
                try
                {
                    MewtocolSP.Open();     //打开串口
                    return 0;
                }
                catch (System.Exception ex)
                {
                    //MessageBox.Show("错误:" + "串口设置不正确,请重新确认", "串口设置错误");
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 关闭PLC操作对象的串口连接
        /// </summary>
        /// <returns></returns>
        public short MewtocolClose()
        {
            try
            {
                MewtocolSP.Close();
                return 0;
            }
            catch
            {
                return -1;
            }
        }
        public string send, receive;
        /// <summary>
        /// 读取上一条通讯指令发出的字符串
        /// </summary>
        /// <returns></returns>
        public string getSendStr()
        {
            return send;
        }
        /// <summary>
        /// 读取上一条通讯指令收到的字符串
        /// </summary>
        /// <returns></returns>
        public string getReceiveStr()
        {
            return receive;
        }
        public string cmd(string str)
        {
            MewtocolSP.WriteLine(str);
            send = str;               //可以添加控件显示发送的数据

            //读取返回数据
            DateTime dt = DateTime.Now;
            Thread.Sleep(10);
            while (MewtocolSP.BytesToRead == 0)
            {
                Thread.Sleep(5);
                if (DateTime.Now.Subtract(dt).TotalMilliseconds > 1000) //如果1秒后仍然无数据返回，则视为超时
                {
                    //throw new Exception("主版无响应");
                    // MessageBox.Show("通讯连接断开");
                    return "00";
                }
            }
            Thread.Sleep(15); //20毫秒内数据接收完毕，可根据实际情况调整
            byte[] receivedData = new byte[MewtocolSP.BytesToRead];        //创建接收字节数组
            MewtocolSP.Read(receivedData, 0, receivedData.Length);         //读取数据
            MewtocolSP.DiscardInBuffer();                                  //清除缓存区数据
            string re = System.Text.Encoding.Default.GetString(receivedData) + "\r\n";
            receive = re;
            return re;
        }
        /// <summary>
        ///  读取单个触点
        /// </summary>
        /// <param name="Address">触点地址</param>
        public bool ReadSinglePoint(string address)
        {
            Thread.Sleep(50);
            string receiveData = cmd(MC.RCS(address));
            string s = receiveData.Substring(6, 1);            
            if (s == "1") return true;
            else if (s == "0") return false;
            else
            {
                // MessageBox.Show("通讯返回错误");
            }

            return false;
        }
        /// <summary>
        /// 写入单个触点
        /// </summary>
        /// <param name="address">触点地址</param>
        /// <param name="value">要写入的值true/false</param>
        /// <returns></returns>
        public void WriteSinglePoint(string address, bool value)
        {
            Thread.Sleep(50);
            string receiveData = cmd(MC.WCS(address, value));
       
        }
        /// <summary>
        /// 按字单位读取触点值
        /// </summary>
        /// <param name="address">要读取的字单位地址</param>
        /// <returns></returns>
        public short ReadWordPoint(string address)
        {
            int n = 1;
            short[] returnValue = new short[n];
            string receiveData = cmd(MC.RCC(address, n));
            if (receiveData[3].ToString() == "$")
            {
                string registerData = receiveData.Substring(6, 4 * n);
                byte[] byteData = SoftBasic.HexStringToBytes(registerData);

                for (int i = 0; i < n; i++)
                {
                    returnValue[i] = (short)(byteData[i * 2 + 1] << 8 | byteData[2 * i] & 0xff);  //得到short数组 ：由byte数组转为PC低位在前的存储方式               
                }
            }
            else
            {
                //MessageBox.Show("通讯返回出错");
            }
            short returnData = returnValue[0];
            return returnData;
        }
        /// <summary>
        /// 按字单位读取多个字单位的触点值
        /// </summary>
        /// <param name="address">字单位 起始点</param>
        /// <param name="n">读取字数</param>
        /// <returns></returns>
        public short[] ReadWordsPoint(string address, int n)
        {
            short[] returnValue = new short[n];
            string receiveData = cmd(MC.RCC(address, n));
            if (receiveData[3].ToString() == "$")
            {
                string registerData = receiveData.Substring(6, 4 * n);
                byte[] byteData = SoftBasic.HexStringToBytes(registerData);

                for (int i = 0; i < n; i++)
                {
                    returnValue[i] = (short)(byteData[i * 2 + 1] << 8 | byteData[2 * i] & 0xff);  //得到short数组 ：由byte数组转为PC低位在前的存储方式               
                }
            }
            else
            {
                //MessageBox.Show("通讯返回出错");
            }
            return returnValue;

        }
        /// <summary>
        /// 按字单位写入多字的触点值
        /// </summary>
        /// <param name="address">字单位 起始点</param>
        /// <param name="n">写入字数</param>
        /// <param name="inData">写入字数组</param>
        public void WriteWordsPoint(string address, int n, short[] inData)
        {
            string receiveData = cmd(MC.WCC(address, n, inData));
        }
        /// <summary>
        /// 按字单位写入单字的触点值
        /// </summary>
        /// <param name="address">写入点字单位地址</param>
        /// <param name="inValue">写入值（short型）</param>
        public void WriteSingleWordPoint(string address, short inValue)
        {
            short[] inData = new short[1];
            inData[0] = inValue;
            string receiveData = cmd(MC.WCC(address, 1, inData));

        }
        /// <summary>
        /// 读取连续多个数据寄存器值
        /// </summary>
        /// <param name="address">数据寄存器地址</param>
        /// <param name="n">读取字个数</param>
        /// <returns></returns>
        public short[] ReadWordDatas(string address, int n)
        {
            short[] returnValue = new short[n];
            string receiveData = cmd(MC.RD(address, n));
            if (receiveData[3].ToString() == "$")
            {
                string registerData = receiveData.Substring(6, 4 * n);
                byte[] byteData = SoftBasic.HexStringToBytes(registerData);

                for (int i = 0; i < n; i++)
                {
                    returnValue[i] = (short)(byteData[i * 2 + 1] << 8 | byteData[2 * i] & 0xff);  //得到short数组 ：由byte数组转为PC低位在前的存储方式               
                }
            }
            else
            {
                // MessageBox.Show("通讯返回出错");
            }
            return returnValue;
        }
        /// <summary>
        /// 读取单个数据寄存器（16位）
        /// </summary>
        /// <param name="address">数据寄存器地址</param>
        /// <returns></returns>
        public short ReadSingleData(string address)
        {
            int n = 1;
            short[] returnValue = new short[n];

            string receiveData = cmd(MC.RD(address, n));
            if (receiveData[3].ToString() == "$")
            {
                string registerData = receiveData.Substring(6, 4 * n);
                byte[] byteData = SoftBasic.HexStringToBytes(registerData);

                for (int i = 0; i < n; i++)
                {
                    returnValue[i] = (short)(byteData[i * 2 + 1] << 8 | byteData[2 * i] & 0xff);  //得到short数组 ：由byte数组转为PC低位在前的存储方式               
                }
            }
            else
            {
                // MessageBox.Show("通讯返回出错");
            }
            short returnData = returnValue[0];
            return returnData;
        }
        /// <summary>
        /// 读取双字数据寄存器
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <returns></returns>
        public int ReadDoubleData(string address)
        {
            int n = 2;
            short[] returnValue = new short[n];

            string receiveData = cmd(MC.RD(address, n));
            if (receiveData[3].ToString() == "$")
            {
                string registerData = receiveData.Substring(6, 4 * n);
                byte[] byteData = SoftBasic.HexStringToBytes(registerData);

                for (int i = 0; i < n; i++)
                {
                    returnValue[i] = (short)(byteData[i * 2 + 1] << 8 | byteData[2 * i] & 0xff);  //得到short数组 ：由byte数组转为PC低位在前的存储方式               
                }
            }
            else
            {
                // MessageBox.Show("通讯返回出错");
            }
            int returnDoubleData = returnValue[1] * 0x100 + returnValue[0];
            return returnDoubleData;
        }



    }
}
