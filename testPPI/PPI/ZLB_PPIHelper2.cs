using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;

using SimpleTCP;
using testPPI.Common;
using WpfModBusTest;

namespace testPPI.PPI
{
    public class ZLB_PPIHelper2
    {

        //2017-5 吕文瑞

        public byte[] receiveByte;

        public static TcpClient tcpClient = new TcpClient();
        // public static SimpleTcpClient tcpClient = new SimpleTcpClient();
        
        public static PPIAddress pAddress = new PPIAddress();

        
        public static byte[] MeiColoudReadBytes = MeiColoudAddress.MeiColoudReadBytes;

        static readonly MeiCloudHelper meiCloudHelper = new MeiCloudHelper();

        #region 读

        //读取某个位的状态
        public  bool Readbit(int ComNum, int ByteAddress, int bitnumber, Enums.StorageType storageType,
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



            ByteAddress = ByteAddress * 8 + bitnumber;


            byte[] SendData = pAddress.Rbyte;

            SendData[22] = 0x01; //Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            if (storageType == Enums.StorageType.T)
            {
                SendData[22] = 0x1F;
            }
            if (storageType == Enums.StorageType.C)
            {
                SendData[22] = 0x1E;
            }

            SendData[24] = 0x01; // Byte 24 为数据个数：这里是 01，一次读一个数据

            if (storageType == Enums.StorageType.V)
            {
                SendData[26] = 0x01;
            }
            else
            {
                SendData[26] = 0x00;
            }
            SendData[27] = (byte)storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20
        
          SendData[28] = Convert.ToByte(ByteAddress / 0x10000);
          SendData[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff); //0x100 ->256;
            SendData[30] = Convert.ToByte(ByteAddress & 0xff); //低位，如320H，结果为20;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += SendData[i];
            }

            int tt = Convert.ToInt32(fcs) % 256; //添加的代码 mod 256

            SendData[31] = Convert.ToByte(tt);


            SendData = GetSendData(SendData);


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
                if (rece[0] == pAddress.confirm)
                {

                    byte[] sendAffirm = GetSendData(pAddress.Affirm);

                    tcpClient.Client.Send(sendAffirm);
                    //  tcpClient.Write(sendAffirm);
                    meiCloudHelper.Read(ComNum, Receives, 0, Receives.Length);

                    bitValue = new byte[1];
                    bitValue[0] = Receives[Receives.Length - 3];
                    return true;
                }
                else
                {
                    bitValue = new byte[] { 0 };
                    return false;
                }
            }
            else
            {
                bitValue = new byte[] { 0 };
                return false;
            }
        }

        public static byte[] GetSendData(byte[] data)
        {

            byte[] SendData = new byte[] { };
            MeiColoudReadBytes[1] = Convert.ToByte(data.Length / 256);
            MeiColoudReadBytes[2] = Convert.ToByte(data.Length % 256);

            SendData = ByteHelper.MergerArray(MeiColoudReadBytes, data);

            byte CheckSum = 0;

            SendData = ByteHelper.MergerArray(SendData, new byte[] { CheckSum });

            CheckSum = MeiCloudHelper.GetCheckSum(SendData);

            SendData[SendData.Length - 1] = CheckSum;
            
            return SendData;



        }




        #endregion


        #region 写





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

         
            byte[] SendData = pAddress.Wbit;

            ByteAddress = ByteAddress * 8 + bitnumber;
            SendData[22] = 0x01; 

            if (storageType == Enums.StorageType.T)
            {
                SendData[22] = 0x1F;
            }
            if (storageType == Enums.StorageType.C)
            {
                SendData[22] = 0x1E;
            }

            SendData[24] = 0x01; // Byte 24 为数据个数：这里是 01，一次读一个数据

            if (storageType == Enums.StorageType.V)
            {
                SendData[26] = 0x01;
            }
            else
            {
                SendData[26] = 0x00;
            }
            SendData[27] = (byte)storageType;
            SendData[28] = Convert.ToByte(ByteAddress / 0x10000);
            SendData[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff); //0x100 ->256;
            SendData[30] = Convert.ToByte(ByteAddress & 0xff); //低位，如320H，结果为20;
            SendData[35] = Convert.ToByte(WriteValue);
            for (i = 4, fcs = 0; i < 36; i++)
            {
                fcs += SendData[i];
            }

            int tt = Convert.ToInt32(fcs) % 256; //添加的代码 mod 256

            SendData[36] = Convert.ToByte(tt);

            SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, SendData);

            SendData = GetSendData(SendData);

      
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
                        for (int j = 0; j < length; j++)
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
                if (rece[0] == pAddress.confirm)
                {

                    byte[] sendAffirm = GetSendData(pAddress.Affirm);

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
            byte[] ReceivesCheck = pAddress.WriteReceivesCheck;
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
