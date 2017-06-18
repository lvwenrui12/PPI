using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
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

        public static string receiveByte;

        public static string sendCmd;



        private static String response = String.Empty;
        public static byte[] ResponseByte;

        private static AutoResetEvent sendDone = new AutoResetEvent(false);
        private static AutoResetEvent receiveDone = new AutoResetEvent(false);

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

     

        #region 读

        //读取某个位的状态
        public static bool Readbit(Socket tcpClient,int ComNum, int ByteAddress, int bitnumber, Enums.StorageType storageType,
            out byte[] bitValue, int plcAddress=2)
        {


            #region 字符串拼接
            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(plcAddress);

            byte[] Rbyte = ppiAddress.Rbyte;

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
            ByteAddress = ByteAddress * 8 + bitnumber;
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
            Rbyte[27] = (byte)storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            Rbyte[28] = Convert.ToByte(ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff); //0x100 ->256;
            Rbyte[30] = Convert.ToByte(ByteAddress & 0xff); //低位，如320H，结果为20;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256; //添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);

            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, Rbyte);

            //Wbit = new byte[] { 0 };

            SendData = GetSendData(SendData);

            sendCmd = ByteHelper.ByteToString(SendData); 
            #endregion


            //接收到的数据：AA 00 02 44 03 05 E5 A5 

            bool flag = false;
         
            byte []receivesAffirm=new byte[8];

            tcpClient.Send(SendData);

            StateObject state = new StateObject();
            state.workSocket = tcpClient;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            tcpClient.BeginReceive(receivesAffirm, 0, receivesAffirm.Length, 0, new AsyncCallback(ReceiveCallback), state);

           

            if (sw.Elapsed > TimeSpan.FromSeconds(3))
            {
                receiveDone.Set();
            }

            receiveDone.WaitOne();
            sw.Stop();

            if (receivesAffirm[5]==Convert.ToByte(ComNum)&& receivesAffirm[6]== ppiAddress.confirm)
            {
                tcpClient.Send(ppiAddress.Affirm);

              
                tcpClient.BeginReceive(Receives, 0, Receives.Length, 0, new AsyncCallback(ReceiveCallback), null);

                Stopwatch sw2 = new Stopwatch();
                sw2.Start();

                if (sw2.Elapsed > TimeSpan.FromSeconds(1))
                {
                    receiveDone.Set();
                }
                sw2.Stop();
                receiveDone.WaitOne();
            }
            
            bitValue = Receives;
          
            return flag;
        }

        #region Socket 函数
        public static void myReadCallBack(IAsyncResult iar)
        {
            NetworkStream ns = (NetworkStream)iar.AsyncState;
            byte[] read = new byte[1024];
            // String data = "";
            int recv;

            recv = ns.EndRead(iar);
            //  data = String.Concat(data, Encoding.ASCII.GetString(read, 0, recv));

            //接收到的消息长度可能大于缓冲区总大小，反复循环直到读完为止
            while (ns.DataAvailable)
            {
                ns.BeginRead(read, 0, read.Length, new AsyncCallback(myReadCallBack), ns);
            }
            //打印
            // Console.WriteLine("您收到的信息是" + data);

        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.     
                StateObject state = new StateObject();
                state.workSocket = client;
                // Begin receiving the data from the remote device.     
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket     
                // from the asynchronous state object.     
                
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                
                // Read data from the remote device.     
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.     

                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    // Get the rest of the data.     
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.     
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.     
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private static void Send(Socket client, byte[] byteData)
        {
              
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.     
                Socket client = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.     
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                // Signal that all bytes have been sent.     
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion



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




        #endregion


        #region 写

    

        public static bool WriteBit(TcpClient tcpClient,int ComNum, int ByteAddress, byte bitnumber, Enums.StorageType storageType,
            int WriteValue,int plcAddress=2)
        {
            #region byte数组
            if (WriteValue > 255)
            {
                return false;
            } //最大写入值255

            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[24];

            bool flag = false;

            PPIAddress ppiAddress = new PPIAddress();

            ppiAddress.DAddress = Convert.ToByte(plcAddress);

            byte[] Wbit = ppiAddress.Wbit;

            ByteAddress = ByteAddress * 8 + bitnumber;
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
            Wbit[27] = (byte)storageType;
            Wbit[28] = Convert.ToByte(ByteAddress / 0x10000);
            Wbit[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff); //0x100 ->256;
            Wbit[30] = Convert.ToByte(ByteAddress & 0xff); //低位，如320H，结果为20;
            Wbit[35] = Convert.ToByte(WriteValue);
            for (i = 4, fcs = 0; i < 36; i++)
            {
                fcs += Wbit[i];
            }

            int tt = Convert.ToInt32(fcs) % 256; //添加的代码 mod 256

            Wbit[36] = Convert.ToByte(tt);

            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, Wbit);

            //Wbit = new byte[] { 0 };

            SendData = GetSendData(SendData); 
            #endregion

            tcpClient.Client.Send(SendData);

             sendCmd = ByteHelper.ByteToString(SendData);

            byte[] rece = new byte[100];
        tcpClient.Client.BeginReceive(rece, 0, rece.Length, SocketFlags.None,
                new AsyncCallback(ReceiveCallback), null);


            receiveDone.WaitOne();
            byte[] receivesAffirm = new byte[8];

            if (receivesAffirm[5] == Convert.ToByte(ComNum) && receivesAffirm[6] == ppiAddress.confirm)
            {
                tcpClient.Client.Send(ppiAddress.Affirm);


                tcpClient.Client.BeginReceive(Receives, 0, Receives.Length, 0, new AsyncCallback(ReceiveCallback), null);

                receiveDone.WaitOne();
            }

            if (Receives[0]==ppiAddress.confirm)
            {
                flag = true;
            }
            else
            {
                int n = 0;
                for (int j = 0; j < Receives.Length; j++)
                {
                    if (Receives[j]!=ppiAddress.WriteReceivesCheck[i])
                    {
                        n++;
                        break;
                    }

                }
                if (n==0)
                {
                    flag = true;
                }
            }

            return flag;

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
