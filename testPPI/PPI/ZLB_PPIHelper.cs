using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using AsynchronousSocket;
using SimpleTCP;
using testPPI.Common;
using WpfModBusTest;

namespace testPPI.PPI
{
    public class ZLB_PPIHelper
    {

        //2017-5 吕文瑞

        public static byte[] receiveByte;

        public static TcpClient tcpClient = new TcpClient();
        // public static SimpleTcpClient tcpClient = new SimpleTcpClient();

            public  static  ClientSocket tp=new ClientSocket();
        public static System.IO.Ports.SerialPort serialPort1 = new SerialPort();

        public ZLB_PPIHelper()
        {


        }


        #region 预定义字符串

        // PLC会返回 E5，意思：已经收到
        //那么这时上位机再次发送指令 10 02 00 5C 5E 16 意思：请执行命令。

        //再次发送指令 10 02 00 5C 5E 16 意思：请执行命令,其中，SD为起始符，始终为10H；DA为读指令的目的地址；SA源地址；FCS校验和，等于DA+SA+FC的数据和；ED始终为16H
        //SD DA  SA FC  FCS  ED
        //10 02	 00	5C	5E	 16
        private static byte[] Affirm = {0x10, 0x02, 0x00, 0x5c, 0x5e, 0x16};



        private static byte DAddress { get; set; } = 0x02; //PLC地址，DA，默认情况下，PLC的地址为02H
        private static byte SAddress { get; set; } = 0x00; //上位机地址 SA，//SA源地址，默认情况下，PC机地址为00H，HMI设备的地址为01H



        public static byte[] Rbyte =
        {
            0x68, 0x1B, 0x1B, //LE为从DA到DU的数据长度，以字节计，如读一个数据时始终为1BH
            0x68, 0x02, 0x00, 0x6C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x04, 0x01, 0x12, 0x0A,
            0x10, 0x02, //byte[22]02代表读取数据长度为byte，即8个bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, //byte[24]读取的个数
            0x00, 0x01, //存储类型01:V,其他的0
            0x84, 0x00, 0x03, 0x20, 0x8b, 0x16
        };


        #endregion


        #region 目标地址 源地址

        public byte DestinationAddress
        {
            get { return DAddress; }
            set
            {
                DAddress = value;
                Rbyte[4] = DAddress;
                Wbyte[4] = DAddress;
                Wbit[4] = DAddress;
                Wword[4] = DAddress;
                Affirm[1] = DAddress;
                int i;
                byte fcs;
                for (i = 1, fcs = 0; i < 4; i++)
                {
                    fcs += Affirm[i];
                }
                int tt = Convert.ToInt32(fcs)%256; //添加的代码 mod 256
                Affirm[4] = Convert.ToByte(tt);
            }
        }

        public byte SourceAddress
        {
            get { return SAddress; }
            set
            {
                SAddress = value;
                Rbyte[5] = SAddress;
                Wbyte[5] = SAddress;

                Wbit[5] = SAddress;
                Wword[5] = SAddress;
                Affirm[2] = SAddress;

                int i;
                byte fcs;
                for (i = 1, fcs = 0; i < 4; i++)
                {
                    fcs += Affirm[i];
                }
                int tt = Convert.ToInt32(fcs)%256; //添加的代码 mod 256
                Affirm[4] = Convert.ToByte(tt);
            }
        }

        #endregion


        public static byte confirm = 0xE5;

        ////数据部长度
        //data[1] = 0x00;
        //    data[2] = 0x09;
        //    //指令部
        //    data[3] = 0x44;
        //    data[4] = 0x03;
        //    //数据部
        //    //端口-05
        //    data[5] = 0x05;


        public static byte[] MeiColoudReadBytes = {0xAA, 0x00, 0x00, 0x44, 0x03};

        static readonly MeiCloudHelper meiCloudHelper = new MeiCloudHelper();

        #region 读

        //读取某个位的状态
        public static bool Readbit(int ComNum, int ByteAddress, int bitnumber, Enums.StorageType storageType,
            out byte[] bitValue)
        {

            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[28];
            if (storageType == Enums.StorageType.T)
            {
                Receives = new byte[32];
            }
            if (storageType == Enums.StorageType.C)
            {
                Receives = new byte[30];
            }

            MeiColoudReadBytes[6] = Convert.ToByte(ComNum); // 端口号



            ByteAddress = ByteAddress*8 + bitnumber;
            Rbyte[22] = 0x01; //Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            if (storageType == Enums.StorageType.T)
            {
                Rbyte[22] = 0x1F;
            }
            if (storageType == Enums.StorageType.C)
            {
                Rbyte[22] = 0x1E;
            }

            Rbyte[24] = 0x01; // Byte 24 为数据个数：这里是 01，一次读一个数据

            if (storageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte) storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            Rbyte[28] = Convert.ToByte(ByteAddress/0x10000);
            Rbyte[29] = Convert.ToByte((ByteAddress/0x100) & 0xff); //0x100 ->256;
            Rbyte[30] = Convert.ToByte(ByteAddress & 0xff); //低位，如320H，结果为20;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }

            int tt = Convert.ToInt32(fcs)%256; //添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);


            byte[] SendData = GetSendData(Rbyte);


            NetworkStream streamToServer = tcpClient.GetStream();
            streamToServer.Write(SendData, 0, SendData.Length);
            //tcpClient.Client.Send(SendData);

            byte[] buffer = new byte[200];
            int mm;

            lock (streamToServer)

            {

                mm = streamToServer.Read(buffer, 0, buffer.Length);

            }


            byte[] rece = new byte[1];

            if (meiCloudHelper.Read(ComNum, rece, 0, rece.Length) > 0)
            {
                if (rece[0] == confirm)
                {

                    byte[] sendAffirm = GetSendData(Affirm);

                    tcpClient.Client.Send(sendAffirm);
                    //  tcpClient.Write(sendAffirm);
                    meiCloudHelper.Read(ComNum, Receives, 0, Receives.Length);

                    bitValue = new byte[1];
                    bitValue[0] = Receives[Receives.Length - 3];
                    return true;
                }
                else
                {
                    bitValue = new byte[] {0};
                    return false;
                }
            }
            else
            {
                bitValue = new byte[] {0};
                return false;
            }
        }

        public static byte[] GetSendData(byte[] data)
        {

            byte[] SendData = new byte[] {};
            MeiColoudReadBytes[1] = Convert.ToByte(data.Length/256);
            MeiColoudReadBytes[2] = Convert.ToByte(data.Length%256);

            SendData = ByteHelper.MergerArray(MeiColoudReadBytes, data);

            byte CheckSum = 0;

            SendData = ByteHelper.MergerArray(SendData, new byte[] {CheckSum});

            CheckSum = MeiCloudHelper.GetCheckSum(SendData);

            SendData[SendData.Length - 1] = CheckSum;



            return SendData;



        }





        //读取一个字节存储单元的数据,长度29





        //读取一个字存储单元的数据




        public static byte[] TReadByte =
        {
            0x68, 0x1b, 0x1b, 0x68, 0x02, 0x00, 0x6c, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0e,
            0x00, 0x00, 0x04, 0x01, 0x12, 0x0a, 0x10, 0x1f, 0x00, 0x01, 0x00, 0x00, 0x1f, 0x00, 0x00, 0xff, 0x1e,
            0x16
        };




        #endregion


        #region 写

        //一次写一个 Double Word 类型的数据。写命令是 40 个字节，其余为 38 个字节
        //写一个 Double Word 类型的数据，前面的 0~21 字节为 ：68 23 23 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10
        //写一个其他类型的数据，前面的 0~21 字节为 ：68 21 21 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10
        //  68 21 21 68 02 00 6C 32 01 00 00 00 00 00 0E 00 06 05 01 12 0A 10
        //从 22 字节开始根据写入数据的值和位置不同而变化


        #region 预定义写字符串

        public static byte[] Wbit =
        {
            0x68, 0x20, 0x20, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x05, 0x05,
            0x01, 0x12, 0x0A, 0x10, //前面0-21位
            0x01, //byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00,
            0x01, //byte[24]数据个数
            0x00,
            0x01, 0x84, //存储器类型
            0x00, 0x00, 0x00, //byte[28],byte[29],byte[30]偏移量
            0x00, 0x03, //数据形式03 bit 其他04
            0x00, 0x01, //数据位数
            0x01, //值
            0x80, 0x16
        }; // 38 个字节


        public static byte[] Wbyte =
        {
            0x68, 0x20, 0x20, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x05, 0x05,
            0x01, 0x12, 0x0A, 0x10, //前面0-21位
            0x02, ////byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, 0x00, 0x01, 0x84, //存储器类型
            0x00, 0x03, 0x20, ////byte[28],byte[29],byte[30]偏移量
            0x00, 0x04, //byte[32]数据形式03 bit 其他04
            0x00, 0x08, //数据位数01 bit 10:word 08 byte 20: Double Word
            0x10, //value
            0xbd, 0x16
        };


        public static byte[] Wword =
        {
            0x68, 0x21, 0x21, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x06, 0x05,
            0x01, 0x12, 0x0A, 0x10,
            0x04, //byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, 0x00, 0x01, 0x84, //存储器类型
            0x00, 0x03, 0x20, //byte[28],byte[29],byte[30]偏移量
            0x00, 0x04, //byte[32]数据形式03 bit 其他04
            0x00, 0x10, //数据位数01 bit 10:word 08 byte 20: Double Word
            0xab, 0xcd, //value
            0x30, 0x16
        };

        public static byte[] WDword =
        {
            0x68, 0x23, 0x23, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x08, 0x05,
            0x01, 0x12, 0x0A, 0x10,
            0x06, //byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, 0x00, 0x01, 0x84, //存储器类型
            0x00, 0x03, 0x20, //byte[28],byte[29],byte[30]偏移量
            0x00, 0x04, //byte[32]数据形式03 bit 其他04
            0x00, 0x20, //数据位数01 bit 10:word 08 byte 20: Double Word
            0xab, 0xcd, //value
            0xab, 0xcd, //value
            0x30, 0x16
        };

        #endregion

        public static bool WriteBit(int ComNum, int ByteAddress, byte bitnumber, Enums.StorageType storageType,
            int WriteValue)
        {
            if (WriteValue > 255)
            {
                return false;
            } //最大写入值255

            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[24];

            //位写操作返回都是字符串为：68 12 12 68 00 02 08 32 03 00 00 00 00 00 02 00 01 00 00 05 01 FF 47 16


            ByteAddress = ByteAddress*8 + bitnumber;
            Wbit[22] = 0x01; //Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word

            if (storageType == Enums.StorageType.T)
            {
                Wbit[22] = 0x1F;
            }
            if (storageType == Enums.StorageType.C)
            {
                Wbit[22] = 0x1E;
            }

            Wbit[24] = 0x01; // Byte 24 为数据个数：这里是 01，一次读一个数据

            if (storageType == Enums.StorageType.V)
            {
                Wbit[26] = 0x01;
            }
            else
            {
                Wbit[26] = 0x00;
            }
            Wbit[27] = (byte) storageType;
            Wbit[28] = Convert.ToByte(ByteAddress/0x10000);
            Wbit[29] = Convert.ToByte((ByteAddress/0x100) & 0xff); //0x100 ->256;
            Wbit[30] = Convert.ToByte(ByteAddress & 0xff); //低位，如320H，结果为20;
            Wbit[35] = Convert.ToByte(WriteValue);
            for (i = 4, fcs = 0; i < 36; i++)
            {
                fcs += Wbit[i];
            }

            int tt = Convert.ToInt32(fcs)%256; //添加的代码 mod 256

            Wbit[36] = Convert.ToByte(tt);

            byte[] SendData = ByteHelper.MergerArray(new byte[] {Convert.ToByte(ComNum)}, Wbit);

            //Wbit = new byte[] { 0 };

            SendData = GetSendData(SendData);

            string strSend = ByteHelper.ByteToString(SendData);
            //写Q0
            //AA 00 27 44 03 05 68 20 20 68 02 00 7C 32 01 00 00 00 00 00 0E 00 05 05 01 12 0A 10 01 00 01 00 00 82 00 00 00 00 03 00 01 01 7F 16 C7

            //  var replyMsg = tcpClient.WriteLineAndGetReply(strSend, TimeSpan.FromSeconds(2));

            //   byte[] rece = new byte[100];

            //  NetworkStream streamToServer = tcpClient.GetStream();
            //  streamToServer.Write(SendData, 0, SendData.Length);


            //  tp.SendBytes(SendData);


            //streamToServer.BeginWrite(SendData, 0, SendData.Length, new AsyncCallback(SendCallback), stream);//异步发送数据


            // byte[] buffer = new byte[200];
            //int mm;

            //lock (streamToServer)

            //{
            //    //try
            //    //{
            //    //    mm = streamToServer.Read(buffer, 0, buffer.Length);
            //    //}
            //    //catch (Exception ex)
            //    //{

            //    //    throw ex;
            //    //}
            //    try
            //    {

            //        streamToServer.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), stream);//异步接受服务器回报的字符串

            //    }
            //    catch { }
            //    string strResponse = Encoding.ASCII.GetString(result).Trim();//从服务器接受到的字符串

            //}



            // }

            //03 串口，
            //网口发送：AA 00 27 44 03 03 68 20 20 68 02 00 7C 32 01 00 00 00 00 00 0E 00 05 05 01 12 0A 10 01 00 01 00 00 82 00 00 00 00 03 00 01 01 7F 16 C1


            //05串口
            //AA 00 27 44 03 05 68 20 20 68 02 00 7C 32 01 00 00 00 00 00 0E 00 05 05 01 12 0A 10 01 00 01 00 00 04 00 00 00 00 03 00 01 01 01 16 3F



            tcpClient.Client.Send(SendData);

            string send = ByteHelper.ByteToString(SendData);

            Thread.Sleep(100);

            byte[] rece = new byte[100];
        tcpClient.Client.BeginReceive(rece, 0, rece.Length, SocketFlags.None,
                asyncResult =>
                {
                    int length = tcpClient.Client.EndReceive(asyncResult);
        string strRec = Encoding.UTF8.GetString(rece);
        string sssss = strRec;
        StringBuilder recBuffer16 = new StringBuilder();
                    for (int j = 0; j<length; j++)
                    {
                        recBuffer16.AppendFormat("{0:X2}" + " ", rece[j]); //X2表示十六进制格式（大写），域宽2位，不足的左边填0。
                    }

    //receiveByte = recBuffer16.ToString();

}, null);






            //if (meiCloudHelper.Read(ComNum, rece, 0, rece.Length) > 0)
            //{
            //    if (rece[0] == confirm)
            //    {

            //        byte[] sendAffirm = GetSendData(Affirm);

            //        tcpClient.Client.Send(sendAffirm);

            //        meiCloudHelper.Read(ComNum, Receives, 0, Receives.Length);
            //        if (Receives.Length > 0)
            //        {
            //            return IsWriteSuccess(Receives);
            //        }
            //        else
            //        {
            //            return false;
            //        }


            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //else
            //{

            //    return false;
            //}

            //if (replyMsg.MessageString== "AA 00 01 41 03 F2 B1")
            //{
            //    return false;
            //}
            //else
            //{
                if (meiCloudHelper.Read(ComNum, rece, 0, rece.Length) > 0)
                {
                    if (rece[0] == confirm)
                    {

                        byte[] sendAffirm = GetSendData(Affirm);

                         tcpClient.Client.Send(sendAffirm);
                       // tcpClient.Write(sendAffirm);

                        meiCloudHelper.Read(ComNum, Receives, 0, Receives.Length);
                        if (Receives.Length > 0)
                        {
                            return IsWriteSuccess(Receives);
                        }
                        else
                        {
                            return false;
                        }


                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {

                    return false;
                }
        //    }


          


        }


        public static bool IsWriteSuccess(byte[] Receives)
        {
            byte[] ReceivesCheck = { 0x68, 0x12, 0x12, 0x68, 0x00, 0x02, 0x08, 0x32, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x05, 0x01, 0xFF, 0x47, 0x16 };
            int n = 0;

            for (int j = 0; j < 24; j++)
            {
                if (Receives[j] != ReceivesCheck[j])
                {
                    n++;
                    break;
                }
            }
            if (n == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }






        public static string ByteToString(byte[] bytes)

        {

            StringBuilder strBuilder = new StringBuilder();

            foreach (byte bt in bytes)

            {

                strBuilder.AppendFormat("{0:X2} ", bt);

            }

            return strBuilder.ToString();

        }



        public static byte[] TWritebyte = { 0x68, 0x24, 0x24, 0x68, 0x02, 0x00, 0x6c, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0e, 0x00, 0x09, 0x05, 0x01, 0x12, 0x0a, 0x10, 0x1f, 0x00, 0x01, 0x00, 0x00, 0x1f, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x28, 0x00, 0x00, 0x00, 0x01, 0x01, 0x57, 0x16 };

        public static byte[] CwriteWordByte =
                {
                0x68, 0x22, 0x22, 0x68, 0x02, 0x00, 0x6c, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x0e, 0x00, 0x07, 0x05, 0x01, 0x12, 0x0a, 0x10, 0x1e, 0x00, 0x01, 0x00, 0x00, 0x1e, 0x00, 0x00, 0x00,
                0x00, 0x04, 0x00, 0x18, 0x00, 0x01, 0x01, 0x43, 0x16
            };


        #endregion


        public static void PortTimeout(int byteLength, TimeSpan ts1, SerialPort port)
        {


            while (port.BytesToRead < byteLength)
            {
                System.Threading.Thread.Sleep(10);

                TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                TimeSpan ts = ts2.Subtract(ts1).Duration();
                //如果接收时间超出设定时间，则直接退出
                if (ts.TotalMilliseconds > 5000)
                {
                    break;

                }
            }

        }



    }
}
