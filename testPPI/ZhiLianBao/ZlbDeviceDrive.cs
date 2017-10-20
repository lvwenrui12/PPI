using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using testPPI.Common;

namespace testPPI
{
    /// <summary>
    /// 设备走modbus协议驱动
    /// </summary>
    public class MCDrive : IDisposable
    {
        #region 变量
        //跟设备连接的tcp连接
        private TcpClient _tcpClient = null;


        private List<WorkQueue<byte[]>> quereCom = new List<WorkQueue<byte[]>>(5);

        public List<byte[]> receiceData = new List<byte[]>(5);
        

        //保存DI数据的队列
        private WorkQueue<byte[]> quereDI1 = new WorkQueue<byte[]>();
        private WorkQueue<byte[]> quereDI2 = new WorkQueue<byte[]>();
        private WorkQueue<byte[]> quereDI3 = new WorkQueue<byte[]>();
        private WorkQueue<byte[]> quereDI4 = new WorkQueue<byte[]>();

        private static byte[] MeiColoudReadBytes = { 0xAA, 0x00, 0x01, 0x44, 0x03 };

        /// <summary>
        /// 驱动与设备是否连接标志
        /// </summary>
        public bool Connected
        {
            get
            {
                if (_tcpClient != null)
                {
                    return _tcpClient.Connected;
                }
                return false;
            }
        }

        #endregion

        public TcpClient tcpClient
        {

            get { return this._tcpClient; }


        }

        /// <summary>
        /// 连接智联宝
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public MCDrive(string ip, int port)
        {
            try
            {
                if (IsCorrectIP(ip))
                {
                    _tcpClient = new TcpClient(ip, port);
                    if (_tcpClient.Connected)//开始监听接收网口数据
                    {
                        new Thread(() => ReadData()).Start();
                    }
                    else
                    {
                        MessageBox.Show("无法连接智联宝，请检查ip和端口是否正确，或者主机和智联宝是否处于同一网段。");
                    }
                }
                else
                {
                    MessageBox.Show("智联宝ip地址不正确");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("无法连接智联宝，请检查ip和端口是否正确，或者主机和智联宝是否处于同一网段。");
            }
        }

        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool IsCorrectIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 读DI
        /// </summary>
        /// <param name="terminalNum">DI端子号</param>
        /// <returns></returns>
        public byte[] ReadDI(int terminalNum)
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                if (terminalNum < 0 || terminalNum > 4)
                {
                    MessageBox.Show("terminalNum参数只能是1到8之前的整数");
                    return null;
                }
                byte[] data = new byte[7];
                data[0] = 0xAA;
                //数据部长度
                data[1] = 0x00;
                data[2] = 0x01;
                //指令部
                data[3] = 0x43;
                data[4] = 0x06;
                //数据部
                data[5] = (byte)terminalNum;
                //校验和
                data[6] = GetCheckSum(data);
                NetworkStream netStream = _tcpClient.GetStream();
                netStream.Write(data, 0, data.Length);
                //tcpClient.Client.Send(data);
                int readCount = 0;
                byte[] recData = null;
                if (terminalNum == 1)
                {
                    while (true)
                    {
                        if (quereDI1.Count > 0)
                        {
                            recData = quereDI1.TryDequeueBox();
                            break;
                        }
                        if (readCount == 3)//读4次读不到数据，超时
                        {
                            break;
                        }
                        readCount++;
                        Thread.Sleep(50);
                    }
                }
                else if (terminalNum == 2)
                {
                    while (true)
                    {
                        if (quereDI2.Count > 0)
                        {
                            recData = quereDI2.TryDequeueBox();
                            break;
                        }
                        if (readCount == 3)//读4次读不到数据，超时
                        {
                            break;
                        }
                        readCount++;
                        Thread.Sleep(50);
                    }
                }
                else if (terminalNum == 3)
                {
                    while (true)
                    {
                        if (quereDI3.Count > 0)
                        {
                            recData = quereDI3.TryDequeueBox();
                            break;
                        }
                        if (readCount == 3)//读4次读不到数据，超时
                        {
                            break;
                        }
                        readCount++;
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    while (true)
                    {
                        if (quereDI4.Count > 0)
                        {
                            recData = quereDI4.TryDequeueBox();
                            break;
                        }
                        if (readCount == 3)//读4次读不到数据，超时
                        {
                            break;
                        }
                        readCount++;
                        Thread.Sleep(50);
                    }
                }

                //byte[] recData = new byte[8];
                //读取返回
                //tcpClient.Client.Receive(recData, 0, 8, SocketFlags.None);
                return recData;
            }
            else
            {
                MessageBox.Show("智联宝设备未连接，请检查网线是否已连接或者ip、端口是否正确，或者主机与智联宝是否处于同一网段。");
                return null;
            }
        }

        /// <summary>
        /// 控制DO
        /// </summary>
        /// <param name="command">命令参数，1-开，2-关，3-闪</param>
        /// <param name="terminalNum">DO端子信号，从1到8的整数</param>
        public void WriteDO(int command, int terminalNum)
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                if (command < 1 || command > 3)
                {
                    MessageBox.Show("command参数只能是1或2或3");
                    return;
                }
                if (terminalNum < 0 || terminalNum > 8)
                {
                    MessageBox.Show("terminalNum参数只能是1到8之前的整数");
                    return;
                }
                byte[] data = new byte[7];
                data[0] = 0xAA;
                //数据部长度
                data[1] = 0x00;
                data[2] = 0x01;
                //指令部
                data[3] = 0x43;
                data[4] = (byte)command;
                //数据部
                data[5] = (byte)terminalNum;
                //校验和
                data[6] = 0x00;
                data[6] = GetCheckSum(data);

                //tcpClient.Client.Send(data);
                NetworkStream netStream = _tcpClient.GetStream();
                netStream.Write(data, 0, data.Length);

                //byte[] recData = new byte[8];
                //tcpClient.Client.Receive(recData, 0, 8, SocketFlags.None);
                //StringBuilder recBuffer16 = new StringBuilder();//定义16进制接收缓存
                //for (int i = 0; i < 8; i++)
                //{
                //    recBuffer16.AppendFormat("{0:X2}" + " ", recData[i]);//X2表示十六进制格式（大写），域宽2位，不足的左边填0。
                //}
            }
            else
            {
                MessageBox.Show("智联宝设备未连接，请检查网线是否已连接或者ip、端口是否正确，或者主机与智联宝是否处于同一网段。");
            }
        }

        /// <summary>
        /// 输出IO复位
        /// </summary>
        public void ResetOutIO()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                byte[] data = new byte[6];
                data[0] = 0xAA;
                //数据部长度
                data[1] = 0x00;
                data[2] = 0x00;
                //指令部
                data[3] = 0x43;
                data[4] = 0x04;
                data[5] = 0x47;

                _tcpClient.Client.Send(data);

            }
            else
            {
                MessageBox.Show("智联宝设备未连接，请检查网线是否已连接或者ip、端口是否正确，或者主机与智联宝是否处于同一网段。");
            }

        }

        /// <summary>
        /// 计数复位
        /// </summary>
        public void ResetCount()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                byte[] data = new byte[6];
                data[0] = 0xAA;
                //数据部长度
                data[1] = 0x00;
                data[2] = 0x00;
                //指令部
                data[3] = 0x43;
                data[4] = 0x05;
                data[5] = 0x46;

                _tcpClient.Client.Send(data);

            }
            else
            {
                MessageBox.Show("智联宝设备未连接，请检查网线是否已连接或者ip、端口是否正确，或者主机与智联宝是否处于同一网段。");
            }
        }

        /// <summary>
        /// 根据指令获取对应的校验和，采用异或校验的方式进行
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private byte GetCheckSum(byte[] bytes)
        {
            //从第二位开始进行校验和校验
            byte checkSumValue = bytes[1];
            for (int i = 2; i < bytes.Length - 1; i++)
            {
                checkSumValue = Convert.ToByte(bytes[i] ^ checkSumValue);
            }

            return checkSumValue;
        }


        private ManualResetEvent _ListenMaualReset = new ManualResetEvent(false);

        public ManualResetEvent ListenMaualReset
        {

            get { return this._ListenMaualReset; }
            set { this._ListenMaualReset = value; }
        }

        bool readding = true;
        /// <summary>
        /// 从网口读接收数据
        /// </summary>
        private void ReadData()
        {
            try
            {
                NetworkStream netStream = _tcpClient.GetStream();
                while (readding)
                {
                    // _ListenMaualReset.WaitOne();

                    if (netStream.CanRead)
                    {
                        // Reads NetworkStream into a byte buffer.
                        byte[] bytes = new byte[_tcpClient.ReceiveBufferSize];

                        // Read can return anything from 0 to numBytesToRead. 
                        // This method blocks until at least one byte is read.
                        netStream.Read(bytes, 0, _tcpClient.ReceiveBufferSize);

                        // Returns the data received from the host to the console.
                        //string returndata = Encoding.UTF8.GetString(bytes);

                        if (bytes != null && bytes.Length > 5)
                        {
                            if (bytes[3] == 67 && bytes[4] == 6)//返回的是DI的数据
                            {
                                if (bytes[5] == 1)
                                {
                                    quereDI1.EnqueueBox(bytes);
                                }
                                else if (bytes[5] == 2)
                                {
                                    quereDI2.EnqueueBox(bytes);
                                }
                                else if (bytes[5] == 3)
                                {
                                    quereDI3.EnqueueBox(bytes);
                                }
                                else
                                {
                                    quereDI4.EnqueueBox(bytes);
                                }
                            }
                            else if (bytes[3] == 68 && bytes[4] == 3)//返回的是串口的数据
                            {

                                if (bytes.Length > 5)
                                {
                                    for (int i = 0; i < quereCom.Count; i++)
                                    {
                                        if (bytes[5] == (i + 1))
                                        {
                                            if (quereCom[i].MaxSize != quereCom[i].Count)
                                            {
                                                quereCom[i].EnqueueBox(bytes);

                                            }
                                            else
                                            {
                                                //如果大于限定的缓存数，就把前面的删除
                                                quereCom[i].TryDequeueBox();
                                                MessageBox.Show("接收数据处理不过来");
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                netStream.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }


        }



        #region 串口写
        public void WriteCom(int ComNum, byte[] Content)
        {
            byte[] SendData = ByteHelper.MergerArray(new[] { Convert.ToByte(ComNum) }, Content);

            SendData = ComGetSendData(SendData);

            if (_tcpClient != null)
            {
                if (_tcpClient.Connected)
                {
                    _tcpClient.Client.Send(SendData);
                }
            }

        }
        #endregion

        #region 串口读

        public byte[] ReadCom(int ComNubm)
        {
            if (ComNubm > quereCom.Count - 1)
            {
                return null;
            }


            if (!quereCom[ComNubm].IsEmpty)
            {
                lock (this)
                {
                    receiceData[ComNubm - 1] = quereCom[ComNubm].TryDequeueBox();
                }
              

                if (receiceData[ComNubm - 1].Length > 7)
                {
                    List<byte> bytesList = new List<byte>(receiceData[ComNubm - 1]);

                    #region 去掉末尾的0
                    for (int j = bytesList.Count - 1; j >= 0; j--)
                    {
                        if (bytesList[j] == 0)
                        {
                            bytesList.RemoveAt(j);
                        }
                        else
                        {
                            break;
                        }
                    }
                    #endregion

                    receiceData[ComNubm - 1] = bytesList.ToArray();
                    var result = receiceData[ComNubm - 1].Skip(6).Take(receiceData[ComNubm - 1].Length - 7);
                    return result.ToArray();
                }

            }
            return null;
        }

        #endregion

        #region 串口发送字符串拼接
        private byte[] ComGetSendData(byte[] data)
        {

            byte[] SendData = new byte[] { };
            MeiColoudReadBytes[1] = Convert.ToByte(data.Length / 256);//有效数据长度

            MeiColoudReadBytes[2] = Convert.ToByte(data.Length % 256);//有效数据长度

            SendData = ByteHelper.MergerArray(MeiColoudReadBytes, data);//格式 ：长度+数据

            byte CheckSum = 0;

            SendData = ByteHelper.MergerArray(SendData, new byte[] { CheckSum });//合并校验位

            CheckSum = GetCheckSum(SendData);

            SendData[SendData.Length - 1] = CheckSum;

            return SendData;

        }

        #endregion


        #region 清除缓存

        public void ClearAllCache()
        {
            quereCom = new List<WorkQueue<byte[]>>();
            quereDI1 = new WorkQueue<byte[]>();
            quereDI2 = new WorkQueue<byte[]>();
            quereDI3 = new WorkQueue<byte[]>();
            quereDI4 = new WorkQueue<byte[]>();

        }



        #endregion

        public void Dispose()
        {
            readding = false;
            this.tcpClient.Close();
        }
    }
}
