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


        public static byte[] MeiColoudReadBytes = { 0xAA, 0x00, 0x00, 0x44, 0x03 };

        #region 读

        #region Readbit
        //读取某个位的状态
        public static bool Readbit(Socket tcpClient, int ComNum, int ByteAddress, int bitnumber,
            Enums.StorageType storageType,
            out byte[] bitValue, int plcAddress = 2)
        {
            //var client = new SimpleTcpClient().Connect("127.0.0.1", 8910);
            //var replyMsg = client.WriteLineAndGetReply("Hello world!", TimeSpan.FromSeconds(3));
            bitValue = new byte[1];

            #region 字符串拼接

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(plcAddress);

            byte[] Rbyte = ppiAddress.Rbyte;

            int i, Rece = 0;
            byte fcs;
            int ReceivesByteSize = 35;
          //  byte[] Receives = new byte[ReceivesByteSize];
            //if (storageType == Enums.StorageType.T)
            //{
            //    Receives = new byte[ReceivesByteSize + 4];
            //}
            //if (storageType == Enums.StorageType.C)
            //{
            //    Receives = new byte[ReceivesByteSize + 2];
            //}
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

            #endregion
            
            //接收到的数据：AA 00 02 44 03 05 E5 A5 

            bool flag = false;
            byte[] Receives=   ReceiveReadByte(tcpClient, SendData, ppiAddress, ComNum);

            if (Receives!=null)
            {


                flag = true;

                bitValue[0] = Receives[Receives.Length - 4];

                receiveByte = ByteHelper.ByteToString(Receives);
            }
            
            return flag;
        }
        #endregion

        #region ReadBytes

        public static bool Readbytes(Socket tcpClient, int Address, Enums.StorageType storageType, out byte[] readValue, int ComNum, int byteCount = 1)
        {

            if (byteCount > 200 || byteCount == 0 || byteCount < 0)
            {
                readValue = new byte[] { 0 };
                return false;
            }
            int i, Rece = 0;
            byte fcs;
            //byte[] Receives = new byte[34 + byteCount];
            Address = Address * 8;
            PPIAddress ppiAddress = new PPIAddress();

            byte[] Rbyte = ppiAddress.Rbyte;

            Rbyte[22] = 0x02;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            Rbyte[24] = Convert.ToByte(byteCount);//一次读取的个数

            if (storageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)storageType;

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            Rbyte[28] = Convert.ToByte(Address / 0x10000);
            Rbyte[29] = Convert.ToByte((Address / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(Address & 0xff);//低位，如320H，结果为20;
            //Rbyte[28] = fbyte3;
            //Rbyte[29] = fbyte2;
            //Rbyte[30] = fbyte1;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);
            //  Rbyte[31] = fcs;
            
            Rbyte = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, Rbyte);

            bool flag = false;
          
            byte[] Receives = ReceiveReadByte(tcpClient, Rbyte, ppiAddress, ComNum);
            if (Receives != null)
            {
              
                flag = true;
                receiveByte = ByteHelper.ByteToString(Receives);

                // serialPort1.Close();
                readValue = new byte[byteCount];
                if (storageType == Enums.StorageType.T)
                {
                    for (int j = 0; j < byteCount; j++)
                    {
                        readValue[j] = Receives[31 + j];
                    }
                }
                else
                {
                    for (int j = 0; j < byteCount; j++)
                    {
                        readValue[j] = Receives[31 + j];

                    }

                }
                return flag;
            }
            
            readValue = new byte[] { 0 };
            return flag;

        }

        #endregion

        #region ReadWords

        public static bool ReadWords(Socket tcpClient,int Address, Enums.StorageType storageType, out byte[] WordValue,int ComNum ,int WordCount = 1)
        {
            if (WordCount > 128 || WordCount == 0 || WordCount < 0)
            {
                WordValue = new byte[] { 0 };
                return false;
            }
            int i, Rece = 0;
            byte fcs;
          
            PPIAddress ppiAddress = new PPIAddress();

            byte[] Rbyte = ppiAddress.Rbyte;

           
            Address = Address * 8;

            if (storageType == Enums.StorageType.C)
            {
                Rbyte[22] = 0x1e;
            }
            else
            {

                Rbyte[22] = 0x04;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            }

            Rbyte[24] = Convert.ToByte(WordCount);//一次读取的个数

            if (storageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)storageType;

            Rbyte[28] = Convert.ToByte(Address / 0x10000);
            Rbyte[29] = Convert.ToByte((Address / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(Address & 0xff);//低位，如320H，结果为20;
        
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);


            Rbyte = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, Rbyte);

            bool flag = false;

            byte[] Receives = ReceiveReadByte(tcpClient, Rbyte, ppiAddress, ComNum);

            if (Receives!=null)
            {
                flag = true;
                WordValue = new byte[WordCount * 2];

                for (int j = 0; j < WordValue.Length; j++)
                {
                    WordValue[WordValue.Length - 1 - j] = Receives[Receives.Length - 4 - j];
                }
                receiveByte = ByteHelper.ByteToString(Receives);
                return flag;
            }
            
            WordValue = new byte[] { 0 };
            return false;
        }





        #endregion

        #region ReadDoubleWord

        public static bool ReadDoubleWord(Socket tcpClient,int Address, Enums.StorageType storageType, out byte[] WordValue,int ComNum)
        {
            int i, Rece = 0;
            byte fcs;
          
            PPIAddress ppiAddress = new PPIAddress();

            byte[] Rbyte = ppiAddress.Rbyte;

            Address = Address * 8;
            Rbyte[22] = 0x06;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1  Word 06： Double Word
            Rbyte[24] = 0x01;//一次读取的个数

            if (storageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)storageType;

            Rbyte[28] = Convert.ToByte(Address / 0x10000);
            Rbyte[29] = Convert.ToByte((Address / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(Address & 0xff);//低位，如320H，结果为20;
         
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);

            Rbyte = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, Rbyte);

            bool flag = false;

            byte[] Receives = ReceiveReadByte(tcpClient, Rbyte, ppiAddress, ComNum);

            if (Receives!=null)
            {
                flag = true;
                WordValue = new byte[4] { Receives[31], Receives[32], Receives[33], Receives[34] };

                receiveByte = ByteHelper.ByteToString(Receives);
                return flag;
            }
            
            WordValue = new byte[] { 0 };
            return false;
        }

        #endregion



        public static byte[] ReceiveReadByte(Socket tcpClient,byte [] Rbyte,PPIAddress ppiAddress,int ComNum)
        {
            byte[] Receives=new byte[100];
            Rbyte = GetSendData(Rbyte);

            sendCmd = ByteHelper.ByteToString(Rbyte);


            bool flag = false;

            byte[] receivesAffirm = new byte[8];
            try
            {
                tcpClient.Send(Rbyte);

                int n = tcpClient.Receive(receivesAffirm);
                if (n > 0)
                {
                    if (receivesAffirm[5] == Convert.ToByte(ComNum) && receivesAffirm[6] == ppiAddress.confirm)
                    {
                        Rbyte = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, ppiAddress.Affirm);

                        Rbyte = GetSendData(Rbyte);

                        tcpClient.Send(Rbyte);
                    
                        int m = tcpClient.Receive(Receives);

                        if (m > 0)
                        {
                          int ReceiveDataCount = 0;
                            for (int i = Receives.Length-1; i>=0; i--)
                            {
                                if (Receives[i]!=0)
                                {
                                    ReceiveDataCount = i;
                                    break;
                                }
                            }
                            byte [] ReceivesResult=new byte[ReceiveDataCount + 1];
                            Array.Copy(Receives, 0, ReceivesResult, 0, ReceiveDataCount+1);
                            
                            return ReceivesResult;
                            
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

          
            return null;


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




        #endregion


        #region 写



        #region WriteBit
        public static bool WriteBit(Socket tcpClient, int ComNum, int ByteAddress, byte bitnumber, Enums.StorageType storageType,
        int WriteValue, int plcAddress = 2)
        {
          
            if (WriteValue > 255)
            {
                return false;
            } //最大写入值255

            int i, Rece = 0;
            byte fcs;
     
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
            
            return WriteData(tcpClient, SendData, ppiAddress, ComNum);
            
        }
        #endregion


        #region WriteByte

        public static bool Writebyte(Socket tcpClient, int ByteAddress, Enums.StorageType storageType, int WriteValue, int ComNum)
        {

            if (WriteValue > 255)
            {
                return false;
            }//最大写入值255
            int i, Rece = 0;
            byte fcs;


            bool flag = false;

            PPIAddress ppiAddress = new PPIAddress();

            byte[] Wbyte = ppiAddress.Wbyte;

            ByteAddress = ByteAddress * 8;
            Wbyte[22] = 0x02;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            Wbyte[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据
            if (storageType == Enums.StorageType.V)
            {
                Wbyte[26] = 0x01;
            }
            else
            {
                Wbyte[26] = 0x00;
            }
            Wbyte[27] = (byte)storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            Wbyte[28] = Convert.ToByte(ByteAddress / 0x10000);
            Wbyte[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Wbyte[30] = Convert.ToByte(ByteAddress & 0xff);//低位，如320H，结果为20;

            Wbyte[32] = 0x04;
            Wbyte[34] = 0x08;


            Wbyte[35] = Convert.ToByte(WriteValue);
            for (i = 4, fcs = 0; i < 36; i++)
            {
                fcs += Wbyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Wbyte[36] = Convert.ToByte(tt);


            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, Wbyte);

           
         return   WriteData(tcpClient, SendData, ppiAddress, ComNum);

        }

        #endregion



        #region WriteWord

        public static bool WriteWord(Socket tcpClient,int byteAddress, Enums.StorageType storageType, int writeValue,int ComNum)
        {

            //if (WriteValue > 255)
            //{
            //    return false;
            //}//最大写入值255
            int i, Rece = 0;
            byte fcs;
            
            byteAddress = byteAddress * 8;

            bool flag = false;

            PPIAddress ppiAddress = new PPIAddress();

            byte[] Wword = ppiAddress.Wword;

            Wword[22] = 0x04;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 =Word06： Double Word
            Wword[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据

            if (storageType == Enums.StorageType.V)
            {
                Wword[26] = 0x01;
            }
            else
            {
                Wword[26] = 0x00;
            }
            Wword[27] = (byte)storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            Wword[28] = Convert.ToByte(byteAddress / 0x10000);
            Wword[29] = Convert.ToByte((byteAddress / 0x100) & 0xff);//0x100 ->256;
            Wword[30] = Convert.ToByte(byteAddress & 0xff);//低位，如320H，结果为20;

            Wword[32] = 0x04;

            Wword[34] = 0x10;

            Wword[35] = Convert.ToByte(writeValue / 256);
            Wword[36] = Convert.ToByte(writeValue % 256);
            for (i = 4, fcs = 0; i < Wword.Length - 2; i++)
            {
                fcs += Wword[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Wword[37] = Convert.ToByte(tt);

            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, Wword);

            return WriteData(tcpClient, SendData, ppiAddress, ComNum);

        }

        #endregion

        #region WriteDoubleWord 
        public static bool WriteDoubleWord(Socket tcpClient,int byteAddress, Enums.StorageType storageType, long writeValue,int ComNum)
        {

            if (writeValue > uint.MaxValue)
            {
                return false;
            }//最大写入值0xffffffff,4,294,967,295
            int i, Rece = 0;
            byte fcs;

            bool flag = false;

            PPIAddress ppiAddress = new PPIAddress();

            byte[] WDword = ppiAddress.WDword;


            byteAddress = byteAddress * 8;
            WDword[22] = 0x06;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            WDword[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据
            if (storageType == Enums.StorageType.V)
            {
                WDword[26] = 0x01;
            }
            else
            {
                WDword[26] = 0x00;
            }
            
            WDword[27] = (byte)storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            WDword[28] = Convert.ToByte(byteAddress / 0x10000);
            WDword[29] = Convert.ToByte((byteAddress / 0x100) & 0xff);//0x100 ->256;
            WDword[30] = Convert.ToByte(byteAddress & 0xff);//低位，如320H，结果为20;

            WDword[32] = 0x04;
            WDword[34] = 0x20;

            WDword[35] = Convert.ToByte(writeValue / 0x1000000);
         
            WDword[36] = Convert.ToByte((writeValue / 0x10000) & 0xff);
            WDword[37] = Convert.ToByte((writeValue / 0x100) & 0xff);
            WDword[38] = Convert.ToByte(writeValue % 256);

            for (i = 4, fcs = 0; i < WDword.Length - 2; i++)
            {
                fcs += WDword[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            WDword[WDword.Length - 2] = Convert.ToByte(tt);


            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, WDword);

            return WriteData(tcpClient, SendData, ppiAddress, ComNum);
            
        }


        #endregion




        public static bool IsWriteSuccess(byte[] Receives, PPIAddress ppiAddress, int ComNum)
        {

            byte[] ReceivesCheck = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, ppiAddress.WriteReceivesCheck);

            //Wbit = new byte[] { 0 };

            ReceivesCheck = GetSendData(ReceivesCheck);

            int n = 0;

            for (int j = 0; j < ReceivesCheck.Length; j++)
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


        public static bool WriteData(Socket tcpClient, byte[] SendData, PPIAddress ppiAddress, int ComNum)
        {


            SendData = GetSendData(SendData);

            sendCmd = ByteHelper.ByteToString(SendData);

            bool flag = false;
            byte[] receivesAffirm = new byte[8];
            try
            {
                tcpClient.Send(SendData);

                int n = tcpClient.Receive(receivesAffirm);
                if (n > 0)
                {
                    if (receivesAffirm[5] == Convert.ToByte(ComNum) && receivesAffirm[6] == ppiAddress.confirm)
                    {
                        SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(ComNum) }, ppiAddress.Affirm);

                        //Wbit = new byte[] { 0 };

                        SendData = GetSendData(SendData);

                        tcpClient.Send(SendData);

                        string str = ByteHelper.ByteToString(SendData);

                        byte[] Receives = new byte[ppiAddress.WriteReceivesCheck.Length + 7];
                        int m = tcpClient.Receive(Receives);

                        if (m > 0)
                        {

                            flag = IsWriteSuccess(Receives, ppiAddress, ComNum);
                            receiveByte = ByteHelper.ByteToString(Receives);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }



            return flag;




        }


        #endregion

        #region 其他函数


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





        #endregion





    }
}
