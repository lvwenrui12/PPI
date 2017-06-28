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

        public static byte[] ResponseByte;

    
        ////数据部长度
        //data[1] = 0x00;data[2] = 0x09;
        //指令部 data[3] = 0x44;data[4] = 0x03;
        //数据部 
        //端口-05 data[5] = 0x05;

        public static byte[] MeiColoudReadBytes = { 0xAA, 0x00, 0x00, 0x44, 0x03 };

        #region 读

        #region Readbit
        //读取某个位的状态
        //public  bool Readbit(Socket tcpClient, int ComNum, int ByteAddress, int bitnumber,
        //    Enums.StorageType storageType,
        //    out byte[] bitValue, int plcAddress = 2)

        public PPIReadWritePara Readbit(PPIReadWritePara readPara)
        {
            readPara.ReadValue = new byte[1];
            #region 字符串拼接

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(readPara.PlcAddress);

            byte[] Rbyte = ppiAddress.Rbyte;

            int i = 0;
            byte fcs;
      
            readPara.ByteAddress = readPara.ByteAddress * 8 + readPara.Bitnumber;
            Rbyte[22] = 0x01; //Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            if (readPara.StorageType == Enums.StorageType.T)
            {
                Rbyte[22] = 0x1F;
            }
            if (readPara.StorageType == Enums.StorageType.C)
            {
                Rbyte[22] = 0x1E;
            }

            Rbyte[24] = 0x01; // Byte 24 为数据个数：这里是 01，一次读一个数据

            if (readPara.StorageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)readPara.StorageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            Rbyte[28] = Convert.ToByte(readPara.ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((readPara.ByteAddress / 0x100) & 0xff); //0x100 ->256;
            Rbyte[30] = Convert.ToByte(readPara.ByteAddress & 0xff); //低位，如320H，结果为20;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256; //添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);

            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(readPara.ComNum) }, Rbyte);

            //Wbit = new byte[] { 0 };

            #endregion

            //接收到的数据：AA 00 02 44 03 05 E5 A5 


            byte[] Receives = ReceiveReadByte(readPara.TcpClient, SendData, ppiAddress, readPara.ComNum);

            if (Receives != null)
            {
                readPara.IsSuceess = true;

                readPara.ReadValue[0] = Receives[Receives.Length - 4];

                receiveByte = ByteHelper.ByteToString(Receives);
            }

            return readPara;
        }
        #endregion

        #region ReadBytes

        //public  bool Readbytes(Socket tcpClient, int Address, Enums.StorageType storageType, out byte[] readValue, int ComNum, int byteCount = 1)

        public PPIReadWritePara Readbytes(PPIReadWritePara readPara)
        {
            if (readPara.ReadCount > 200 || readPara.ReadCount == 0 || readPara.ReadCount < 0)
            {
                readPara.ReadValue = new byte[] { 0 };
                return readPara;
            }
            int i = 0;
            byte fcs;
            //byte[] Receives = new byte[34 + byteCount];
            readPara.ByteAddress = readPara.ByteAddress * 8;
            PPIAddress ppiAddress = new PPIAddress();

            ppiAddress.DAddress = Convert.ToByte(readPara.PlcAddress);
            byte[] Rbyte = ppiAddress.Rbyte;
            Rbyte[22] = 0x02;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            Rbyte[24] = Convert.ToByte(readPara.ReadCount);//一次读取的个数
            if (readPara.StorageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)readPara.StorageType;

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            Rbyte[28] = Convert.ToByte(readPara.ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((readPara.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(readPara.ByteAddress & 0xff);//低位，如320H，结果为20;

            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);
            //  Rbyte[31] = fcs;

            Rbyte = ByteHelper.MergerArray(new byte[] { Convert.ToByte(readPara.ComNum) }, Rbyte);

            byte[] Receives = ReceiveReadByte(readPara.TcpClient, Rbyte, ppiAddress, readPara.ComNum);
            if (Receives != null)
            {
                readPara.IsSuceess = true;
                receiveByte = ByteHelper.ByteToString(Receives);

                readPara.ReadValue = new byte[readPara.ReadCount];
                if (readPara.StorageType == Enums.StorageType.T)
                {
                    for (int j = 0; j < readPara.ReadCount; j++)
                    {
                        readPara.ReadValue[j] = Receives[31 + j];
                    }
                }
                else
                {
                    for (int j = 0; j < readPara.ReadCount; j++)
                    {
                        readPara.ReadValue[j] = Receives[31 + j];

                    }
                }
                return readPara;
            }

            readPara.ReadValue = new byte[] { 0 };
            return readPara;

        }

        #endregion

        #region ReadWords

        //public  bool ReadWords(Socket tcpClient, int Address, Enums.StorageType storageType, out byte[] WordValue, int ComNum, int WordCount = 1)

        public PPIReadWritePara ReadWords(PPIReadWritePara readPara)
        {
            if (readPara.ReadCount > 128)
            {
                readPara.ReadValue = new byte[] { 0 };
                return readPara;
            }
            int i = 0;
            byte fcs;

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(readPara.PlcAddress);
            byte[] Rbyte = ppiAddress.Rbyte;
            readPara.ByteAddress = readPara.ByteAddress * 8;
            if (readPara.StorageType == Enums.StorageType.C)
            {
                Rbyte[22] = 0x1e;
            }
            else
            {
                Rbyte[22] = 0x04;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            }

            Rbyte[24] = Convert.ToByte(readPara.ReadCount);//一次读取的个数

            if (readPara.StorageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)readPara.StorageType;

            Rbyte[28] = Convert.ToByte(readPara.ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((readPara.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(readPara.ByteAddress & 0xff);//低位，如320H，结果为20;

            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);

            Rbyte = ByteHelper.MergerArray(new byte[] { Convert.ToByte(readPara.ComNum) }, Rbyte);


            byte[] Receives = ReceiveReadByte(readPara.TcpClient, Rbyte, ppiAddress, readPara.ComNum);

            if (Receives != null)
            {
                readPara.IsSuceess = true;
                readPara.ReadValue = new byte[readPara.ReadCount * 2];

                for (int j = 0; j < readPara.ReadValue.Length; j++)
                {
                    readPara.ReadValue[readPara.ReadValue.Length - 1 - j] = Receives[Receives.Length - 4 - j];
                }
                receiveByte = ByteHelper.ByteToString(Receives);
                return readPara;
            }

            readPara.ReadValue = new byte[] { 0 };
            return readPara;
        }





        #endregion

        #region ReadDoubleWord

        //public bool ReadDoubleWord(Socket tcpClient, int Address, Enums.StorageType storageType, out byte[] WordValue, int ComNum)
        public PPIReadWritePara ReadDoubleWord(PPIReadWritePara readPara)
        {
            int i = 0;
            byte fcs;

            PPIAddress ppiAddress = new PPIAddress();

            ppiAddress.DAddress = Convert.ToByte(readPara.PlcAddress);
            byte[] Rbyte = ppiAddress.Rbyte;

            readPara.ByteAddress = readPara.ByteAddress * 8;
            Rbyte[22] = 0x06;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1  Word 06： Double Word
            Rbyte[24] = 0x01;//一次读取的个数

            if (readPara.StorageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)readPara.StorageType;

            Rbyte[28] = Convert.ToByte(readPara.ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((readPara.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(readPara.ByteAddress & 0xff);//低位，如320H，结果为20;

            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);

            Rbyte = ByteHelper.MergerArray(new byte[] { Convert.ToByte(readPara.ComNum) }, Rbyte);



            byte[] Receives = ReceiveReadByte(readPara.TcpClient, Rbyte, ppiAddress, readPara.ComNum);

            if (Receives != null)
            {
                readPara.IsSuceess = false;
                readPara.ReadValue = new byte[4] { Receives[31], Receives[32], Receives[33], Receives[34] };

                receiveByte = ByteHelper.ByteToString(Receives);
                return readPara;
            }

            readPara.ReadValue = new byte[] { 0 };
            return readPara;
        }

        #endregion


        #region TReadWord
        //public bool TReadDword(Socket tcpClient, int Address, out byte[] value, int ComNum)
        public PPIReadWritePara TReadDword(PPIReadWritePara readPara)
        {
            int i = 0;
            byte fcs;

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(readPara.PlcAddress);
            byte[] TReadByte = ppiAddress.TReadByte;


            readPara.ByteAddress = readPara.ByteAddress * 8;
            TReadByte[28] = Convert.ToByte(readPara.ByteAddress / 0x10000);
            TReadByte[29] = Convert.ToByte((readPara.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            TReadByte[30] = Convert.ToByte(readPara.ByteAddress & 0xff);//低位，如320H，结果为20;

            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += TReadByte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            TReadByte[31] = Convert.ToByte(tt);

            TReadByte = ByteHelper.MergerArray(new byte[] { Convert.ToByte(readPara.ComNum) }, TReadByte);

          
            byte[] Receives = ReceiveReadByte(readPara.TcpClient, TReadByte, ppiAddress, readPara.ComNum);

            if (Receives != null)
            {
                readPara.IsSuceess = true;
                readPara.ReadValue = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    readPara.ReadValue[j] = Receives[32 + j];
                }
                receiveByte = ByteHelper.ByteToString(Receives);
                return readPara;
            }
            readPara.ReadValue = new byte[] { 0 };
            return readPara;

        }

        #endregion


        public byte[] ReceiveReadByte(Socket tcpClient, byte[] Rbyte, PPIAddress ppiAddress, int ComNum)
        {
            byte[] Receives = new byte[100];
            Rbyte = GetSendData(Rbyte);

            sendCmd = ByteHelper.ByteToString(Rbyte);
            
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
                            for (int i = Receives.Length - 1; i >= 0; i--)
                            {
                                if (Receives[i] != 0)
                                {
                                    ReceiveDataCount = i;
                                    break;
                                }
                            }
                            byte[] ReceivesResult = new byte[ReceiveDataCount + 1];
                            Array.Copy(Receives, 0, ReceivesResult, 0, ReceiveDataCount + 1);

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


        #endregion


        #region 写



        #region WriteBit

        public bool WriteBit(PPIReadWritePara writePara)
        {

            if (writePara.WriteValue > 255)
            {
                return false;
            } //最大写入值255

            int i= 0;
            byte fcs;
            
            PPIAddress ppiAddress = new PPIAddress();

            ppiAddress.DAddress = Convert.ToByte(writePara.PlcAddress);

            byte[] Wbit = ppiAddress.Wbit;

            writePara.ByteAddress = writePara.ByteAddress * 8 + writePara.Bitnumber;
            Wbit[22] = 0x01; //Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word

            if (writePara.StorageType == Enums.StorageType.T)
            {
                Wbit[22] = 0x1F;
            }
            if (writePara.StorageType == Enums.StorageType.C)
            {
                Wbit[22] = 0x1E;
            }

            Wbit[24] = 0x01; // Byte 24 为数据个数：这里是 01，一次读一个数据

            if (writePara.StorageType == Enums.StorageType.V)
            {
                Wbit[26] = 0x01;
            }
            else
            {
                Wbit[26] = 0x00;
            }
            Wbit[27] = (byte)writePara.StorageType;
            Wbit[28] = Convert.ToByte(writePara.ByteAddress / 0x10000);
            Wbit[29] = Convert.ToByte((writePara.ByteAddress / 0x100) & 0xff); //0x100 ->256;
            Wbit[30] = Convert.ToByte(writePara.ByteAddress & 0xff); //低位，如320H，结果为20;
            Wbit[35] = Convert.ToByte(writePara.WriteValue);
            for (i = 4, fcs = 0; i < 36; i++)
            {
                fcs += Wbit[i];
            }

            int tt = Convert.ToInt32(fcs) % 256; //添加的代码 mod 256

            Wbit[36] = Convert.ToByte(tt);

            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(writePara.ComNum) }, Wbit);

            return WriteData(writePara.TcpClient, SendData, ppiAddress, writePara.ComNum);

        }
        #endregion


        #region WriteByte
        //public bool Writebyte(Socket tcpClient, int ByteAddress, Enums.StorageType storageType, int WriteValue, int ComNum)

        public bool Writebyte(PPIReadWritePara para)
        {

            if (para.WriteValue > 255)
            {
                return false;
            }//最大写入值255
            int i = 0;
            byte fcs;
            
            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] Wbyte = ppiAddress.Wbyte;

            para.ByteAddress = para.ByteAddress * 8;
            Wbyte[22] = 0x02;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            Wbyte[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据
            if (para.StorageType == Enums.StorageType.V)
            {
                Wbyte[26] = 0x01;
            }
            else
            {
                Wbyte[26] = 0x00;
            }
            Wbyte[27] = (byte)para.StorageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            Wbyte[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            Wbyte[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Wbyte[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            Wbyte[32] = 0x04;
            Wbyte[34] = 0x08;


            Wbyte[35] = Convert.ToByte(para.WriteValue);
            for (i = 4, fcs = 0; i < 36; i++)
            {
                fcs += Wbyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Wbyte[36] = Convert.ToByte(tt);


            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(para.ComNum) }, Wbyte);


            return WriteData(para.TcpClient, SendData, ppiAddress, para.ComNum);

        }

        #endregion



        #region WriteWord

        public bool WriteWord(PPIReadWritePara writePara)
        {

            int i = 0;
            byte fcs;

            writePara.ByteAddress = writePara.ByteAddress * 8;
            
            PPIAddress ppiAddress = new PPIAddress();

            byte[] Wword = ppiAddress.Wword;

            Wword[22] = 0x04;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 =Word06： Double Word
            Wword[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据

            if (writePara.StorageType == Enums.StorageType.V)
            {
                Wword[26] = 0x01;
            }
            else
            {
                Wword[26] = 0x00;
            }
            Wword[27] = (byte)writePara.StorageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            Wword[28] = Convert.ToByte(writePara.ByteAddress / 0x10000);
            Wword[29] = Convert.ToByte((writePara.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Wword[30] = Convert.ToByte(writePara.ByteAddress & 0xff);//低位，如320H，结果为20;

            Wword[32] = 0x04;

            Wword[34] = 0x10;

            Wword[35] = Convert.ToByte(writePara.WriteValue / 256);
            Wword[36] = Convert.ToByte(writePara.WriteValue % 256);
            for (i = 4, fcs = 0; i < Wword.Length - 2; i++)
            {
                fcs += Wword[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Wword[37] = Convert.ToByte(tt);

            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(writePara.ComNum) }, Wword);

            return WriteData(writePara.TcpClient, SendData, ppiAddress, writePara.ComNum);

        }

        #endregion

        #region WriteDoubleWord 
        public bool WriteDoubleWord(PPIReadWritePara writePara)
        {

            //if (writePara.WriteValue > uint.MaxValue)
            //{
            //    return false;
            //}//最大写入值0xffffffff,4,294,967,295
            int i = 0;
            byte fcs;

       

            PPIAddress ppiAddress = new PPIAddress();

            byte[] WDword = ppiAddress.WDword;


            writePara.ByteAddress = writePara.ByteAddress * 8;
            WDword[22] = 0x06;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            WDword[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据
            if (writePara.StorageType == Enums.StorageType.V)
            {
                WDword[26] = 0x01;
            }
            else
            {
                WDword[26] = 0x00;
            }

            WDword[27] = (byte)writePara.StorageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            WDword[28] = Convert.ToByte(writePara.ByteAddress / 0x10000);
            WDword[29] = Convert.ToByte((writePara.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            WDword[30] = Convert.ToByte(writePara.ByteAddress & 0xff);//低位，如320H，结果为20;

            WDword[32] = 0x04;
            WDword[34] = 0x20;

            WDword[35] = Convert.ToByte(writePara.WriteValue / 0x1000000);

            WDword[36] = Convert.ToByte((writePara.WriteValue / 0x10000) & 0xff);
            WDword[37] = Convert.ToByte((writePara.WriteValue / 0x100) & 0xff);
            WDword[38] = Convert.ToByte(writePara.WriteValue % 256);

            for (i = 4, fcs = 0; i < WDword.Length - 2; i++)
            {
                fcs += WDword[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            WDword[WDword.Length - 2] = Convert.ToByte(tt);


            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(writePara.ComNum) }, WDword);

            return WriteData(writePara.TcpClient, SendData, ppiAddress, writePara.ComNum);

        }


        #endregion

        #region CWriteWord

        //public bool CWriteWord(Socket tcpClient, int byteAddress, int writeValue, int ComNum)
              public bool CWriteWord(PPIReadWritePara para)

        {
            if (para.WriteValue > 65536)
            {
                return false;
            }//最大写入值0xfffff,4,294,967,295
            int i = 0;
            byte fcs;

            PPIAddress ppiAddress = new PPIAddress();

            byte[] CwriteWordByte = ppiAddress.CwriteWordByte;
            para.ByteAddress = para.ByteAddress * 8;
            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            CwriteWordByte[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            CwriteWordByte[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            CwriteWordByte[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            CwriteWordByte[35] = Convert.ToByte((para.WriteValue / 0x10000) & 0xff);
            CwriteWordByte[36] = Convert.ToByte((para.WriteValue / 0x100) & 0xff);
            CwriteWordByte[37] = Convert.ToByte(para.WriteValue % 256);

            for (i = 4, fcs = 0; i < CwriteWordByte.Length - 2; i++)
            {
                fcs += CwriteWordByte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            CwriteWordByte[CwriteWordByte.Length - 2] = Convert.ToByte(tt);
            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(para.ComNum) }, CwriteWordByte);
            return WriteData(para.TcpClient, SendData, ppiAddress, para.ComNum);
        }

        #endregion


        public bool TwriteDWord(PPIReadWritePara writePara)
        {


            //if (writeValue > uint.MaxValue)
            //{
            //    return false;
            //}//最大写入值0xffffffff,4,294,967,295
            int i = 0;
            byte fcs;

            PPIAddress ppiAddress = new PPIAddress();

            byte[] TWritebyte = ppiAddress.TWritebyte;


            writePara.ByteAddress = writePara.ByteAddress * 8;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            TWritebyte[28] = Convert.ToByte(writePara.ByteAddress / 0x10000);
            TWritebyte[29] = Convert.ToByte((writePara.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            TWritebyte[30] = Convert.ToByte(writePara.ByteAddress & 0xff);//低位，如320H，结果为20;

            TWritebyte[36] = Convert.ToByte(writePara.WriteValue / 0x1000000);
            TWritebyte[37] = Convert.ToByte((writePara.WriteValue / 0x10000) & 0xff);
            TWritebyte[38] = Convert.ToByte((writePara.WriteValue / 0x100) & 0xff);
            TWritebyte[39] = Convert.ToByte(writePara.WriteValue % 256);

            for (i = 4, fcs = 0; i < TWritebyte.Length - 2; i++)
            {
                fcs += TWritebyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            TWritebyte[TWritebyte.Length - 2] = Convert.ToByte(tt);


            byte[] SendData = ByteHelper.MergerArray(new byte[] { Convert.ToByte(writePara.ComNum) }, TWritebyte);

            return WriteData(writePara.TcpClient, SendData, ppiAddress, writePara.ComNum);



        }

        public bool IsWriteSuccess(byte[] Receives, PPIAddress ppiAddress, int ComNum)
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


        public bool WriteData(Socket tcpClient, byte[] SendData, PPIAddress ppiAddress, int ComNum)
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

            CheckSum = GetCheckSum(SendData);

            SendData[SendData.Length - 1] = CheckSum;

            return SendData;

        }

        /// <summary>
        /// 根据指令获取对应的校验和，采用异或校验的方式进行
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte GetCheckSum(byte[] bytes)
        {
            //从第二位开始进行校验和校验
            byte checkSumValue = bytes[1];
            for (int i = 2; i < bytes.Length - 1; i++)
            {
                checkSumValue = Convert.ToByte(bytes[i] ^ checkSumValue);
            }

            return checkSumValue;
        }

        #endregion





    }
}
