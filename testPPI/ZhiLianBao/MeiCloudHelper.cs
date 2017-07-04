using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;


namespace testPPI
{
    public class MeiCloudHelper:IMeiCloud
    {
        #region 变量

        private TcpClient tcpClient = new TcpClient();

        private  Queue<byte> QueueCom1 = new Queue<byte>();
        private  Queue<byte> QueueCom2 = new Queue<byte>(); 
        private  Queue<byte> QueueCom3 = new Queue<byte>(); 
        private  Queue<byte> QueueCom4 = new Queue<byte>();
        private  Queue<byte> QueueCom5 = new Queue<byte>();

        string strReceive = "";

        #endregion

        public long ReadTimeOut
        {
            get
            {
                return tcpClient.ReceiveTimeout;
                //throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public long WriteTimeOut
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Connect(System.Net.IPAddress ipAddress, int port)
        {
            tcpClient.Connect(ipAddress, port);
        }

        public void Disconnect()
        {
            tcpClient.Client.Disconnect(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="buffer"></param>
        /// <param name="offset">buffer存放的开始位置</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public  int Read(int comPort, byte[] buffer, int offset, int size)
        {
            int len = size;
            switch(comPort)
            {
                case 1:
                    if (QueueCom1.Count ==0)
                    {
                        len = 0;
                    }
                    else if(QueueCom1.Count<size)
                    {
                        len = QueueCom1.Count;
                        for (int i = 0; i < len; i++)
                        {
                            buffer[offset+i]= QueueCom1.Dequeue();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < size; i++)
                        {
                            buffer[offset + i] = QueueCom1.Dequeue();
                        }
                    }
                    break;
                case 2:
                    if (QueueCom2.Count == 0)
                    {
                        len = 0;
                    }
                    else if (QueueCom2.Count < size)
                    {
                        len = QueueCom2.Count;
                        for (int i = 0; i < len; i++)
                        {
                            buffer[offset + i] = QueueCom2.Dequeue();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < size; i++)
                        {
                            buffer[offset + i] = QueueCom2.Dequeue();
                        }
                    }
                    break;
                case 3:
                    if (QueueCom3.Count == 0)
                    {
                        len = 0;
                    }
                    else if (QueueCom3.Count < size)
                    {
                        len = QueueCom3.Count;
                        for (int i = 0; i < len; i++)
                        {
                            buffer[offset + i] = QueueCom3.Dequeue();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < size; i++)
                        {
                            buffer[offset + i] = QueueCom3.Dequeue();
                        }
                    }
                    break;
                case 4:
                    if (QueueCom4.Count == 0)
                    {
                        len = 0;
                    }
                    else if (QueueCom4.Count < size)
                    {
                        len = QueueCom4.Count;
                        for (int i = 0; i < len; i++)
                        {
                            buffer[offset + i] = QueueCom4.Dequeue();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < size; i++)
                        {
                            buffer[offset + i] = QueueCom4.Dequeue();
                        }
                    }
                    break;
                case 5:
                    if (QueueCom5.Count == 0)
                    {
                        len = 0;
                    }
                    else if (QueueCom5.Count < size)
                    {
                        len = QueueCom5.Count;
                        for (int i = 0; i < len; i++)
                        {
                            buffer[offset + i] = QueueCom5.Dequeue();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < size; i++)
                        {
                            buffer[offset + i] = QueueCom5.Dequeue();
                        }
                    }
                    break;
                default:
                    break;
            }
            return len;
        }

        public string Read()
        {
            return strReceive;
        }

        /// <summary>
        /// 下发命令
        /// </summary>
        /// <param name="comPort">智联宝与设备连接的端口</param>
        /// <param name="buffer">保存命令的数组</param>
        /// <param name="offset">数组中命令开始的位置</param>
        /// <param name="size">命令的长度</param>
        /// <returns>下发成功：0，下发失败：-1</returns>
        public int Write(int comPort, byte[] buffer, int offset, int size)
        {
            try
            {
                if (buffer == null || buffer.Length == 0 || offset < 0 || size <= 0)
                {
                    return -1;
                }
                //判断从offse开始size个字节是否已经越界
                int len = offset + size;
                if (buffer.Length < len)
                {
                    return -1;
                }
                if (tcpClient.Connected)
                {
                    //crc校验的数据
                    byte[] crcData=new byte[size];
                    //data指令当前写入的位置
                    int currentIndex = 6;
                    byte[] data = new byte[size+9];//9是协议头和尾的长度
                    data[0] = 0xAA;
                    //数据部长度
                    data[1] = 0x00;
                    data[2] = 0x09;
                    //指令部
                    data[3] = 0x44;
                    data[4] = 0x03;
                    //数据部
                    //端口-05
                    data[5] = (byte)comPort;
                    //modbus指令
                    for (int i = 0; i < size;i++ )
                    {
                        data[6 + i] = buffer[offset + i];
                        crcData[i] = buffer[offset + i];
                        currentIndex++;
                    }
                    //crc校验
                    //byte[] test = new byte[6] { 0x0c, 0x05, 0x00, 0x01, 0xFF, 0x00 };
                    byte[] resCRC = CRC16_C(crcData);
                    data[currentIndex++] = resCRC[0];
                    data[currentIndex++] = resCRC[1];
                    //校验和
                    data[currentIndex++] = GetCheckSum(data);
                   
                    tcpClient.Client.Send(data);

                    byte[] recData = new byte[1024];
                    tcpClient.Client.BeginReceive(recData, 0, recData.Length, SocketFlags.None,
                        asyncResult =>
                        {
                            int length = tcpClient.Client.EndReceive(asyncResult);
                            //string strRec = Encoding.UTF8.GetString(recData);AA 00 0A 44 03 05 0C 03 04 0C 0C 0D 0D 20 F5 
                            //string sssss = strRec;
                            //所属com
                            int com = recData[5];
                            //数据长度
                            int lenght = recData[8];
                            switch(com)
                            {
                                case 1:
                                    for (int i = 0; i < lenght; i++)
                                    {
                                        QueueCom1.Enqueue(recData[9 + i]);
                                    }
                                    break;
                                case 2:
                                    for (int i = 0; i < lenght; i++)
                                    {
                                        QueueCom2.Enqueue(recData[9 + i]);
                                    }
                                    break;
                                case 3:
                                    for (int i = 0; i < lenght; i++)
                                    {
                                        QueueCom3.Enqueue(recData[9 + i]);
                                    }
                                    break;
                                case 4:
                                    for (int i = 0; i < lenght; i++)
                                    {
                                        QueueCom4.Enqueue(recData[9 + i]);
                                    }
                                    break;
                                case 5:
                                    for (int i = 0; i < lenght; i++)
                                    {
                                        QueueCom5.Enqueue(recData[9 + i]);
                                    }
                                    break;
                                default :
                                    break;
                            }
                            StringBuilder recBuffer16 = new StringBuilder();

                            for (int i = 0; i < length; i++)
                            {
                                recBuffer16.AppendFormat("{0:X2}" + " ", recData[i]);//X2表示十六进制格式（大写），域宽2位，不足的左边填0。
                            }
                            strReceive += recBuffer16.ToString();
                        }
            , null);  
                    return 0;
                }
                return -1;
            }
            catch(Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// 计算CRC校验码
        /// Cyclic Redundancy Check 循环冗余校验码
        /// 是数据通信领域中最常用的一种差错校验码
        /// 特征是信息字段和校验字段的长度可以任意选定
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CRC16_C(byte[] data)
        {
            byte num = 0xff;
            byte num2 = 0xff;

            byte num3 = 1;
            byte num4 = 160;
            byte[] buffer = data;

            for (int i = 0; i < buffer.Length; i++)
            {
                //位异或运算
                num = (byte)(num ^ buffer[i]);

                for (int j = 0; j <= 7; j++)
                {
                    byte num5 = num2;
                    byte num6 = num;

                    //位右移运算
                    num2 = (byte)(num2 >> 1);
                    num = (byte)(num >> 1);

                    //位与运算
                    if ((num5 & 1) == 1)
                    {
                        //位或运算
                        num = (byte)(num | 0x80);
                    }
                    if ((num6 & 1) == 1)
                    {
                        num2 = (byte)(num2 ^ num4);
                        num = (byte)(num ^ num3);
                    }
                }
            }
            return new byte[] { num, num2 };
        }

        /// <summary>
        /// 根据指令获取对应的校验和，采用异或校验的方式进行
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public   static byte GetCheckSum(byte[] bytes)
        {
            //从第二位开始进行校验和校验
            byte checkSumValue = bytes[1];
            for (int i = 2; i < bytes.Length - 1; i++)
            {
                checkSumValue = Convert.ToByte(bytes[i] ^ checkSumValue);
            }

            return checkSumValue;
        }
    }
}
