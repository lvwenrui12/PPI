using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using MeiCloud.ZLB.ZlbDrive.PPI;
using testPPI.Common;

namespace testPPI.PPI
{
    public class PPIHelper
    {

        //2017-5 吕文瑞




        public static string receiveByte;

        public static string sendCmd;



        public static System.IO.Ports.SerialPort serialPort1 = new SerialPort();




        #region 读

        //读取某个位的状态
        //public static bool Readbit(int ByteAddress, int bitnumber, Enums.StorageType storageType, out byte[] bitValue, int plcAdd)

        public static PPIReadWritePara Readbit(PPIReadWritePara para)
        {

            para.ReadValue = new byte[1];

            int i = 0;
            byte fcs;
           

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }


            PPIAddress ppiAddress = new PPIAddress();

            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] Rbyte = ppiAddress.Rbyte;
            para.ByteAddress = para.ByteAddress * 8 + para.Bitnumber;
            Rbyte[22] = 0x01;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            if (para.StorageType == Enums.StorageType.T)
            {
                Rbyte[22] = 0x1F;
            }
            if (para.StorageType == Enums.StorageType.C)
            {
                Rbyte[22] = 0x1E;
            }

            Rbyte[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据

            if (para.StorageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)para.StorageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            Rbyte[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];

            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);

            byte[] Receives = ReceiveReadByte(serialPort1, Rbyte, ppiAddress);

            if (Receives != null)
            {
               
                receiveByte = ByteHelper.ByteToString(Receives);

                if (Receives.Length>=3)
                {
                    para.ReadValue[0] = Receives[Receives.Length - 3];
                    para.IsSuceess = true;
                }
              
            }

            return para;
        }


        public static bool Readbits(int ByteAddress, int bitnumber, Enums.StorageType storageType, out byte[] bitValue, int plcAdd, int bitCount = 1)//测试失败
        {
            if (bitCount > 255 || bitCount == 0 || bitCount < 0)
            {

                bitValue = new byte[] { 0 };
                return false;
            }

            int i = 0;
            byte fcs;
            //byte[] Receives = new byte[27 + bitCount];

            //if (storageType == Enums.StorageType.T)
            //{
            //    Receives = new byte[31 + bitCount];
            //}

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(plcAdd);
            ByteAddress = ByteAddress * 8 + bitnumber;
            byte[] Rbyte = ppiAddress.Rbyte;
            Rbyte[22] = 0x01;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            if (storageType == Enums.StorageType.T)
            {
                Rbyte[22] = 0x1F;
            }

            Rbyte[24] = Convert.ToByte(bitCount);// Byte 24 为数据个数：这里是 01，一次读一个数据

            if (storageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)storageType;

            Rbyte[28] = Convert.ToByte(ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(ByteAddress & 0xff);//低位，如320H，结果为20;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];

            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);


            byte[] Receives = ReceiveReadByte(serialPort1, Rbyte, ppiAddress);

            if (Receives != null)
            {
                receiveByte = ByteHelper.ByteToString(Receives);
                bitValue = new byte[bitCount];
                for (int j = 0; j < bitCount; j++)
                {
                    bitValue[j] = Receives[24 + j];
                }

                return true;
            }

            else
            {
                bitValue = new byte[] { 0 };
                return false;
            }

        }//读取失败


        //读取一个字节存储单元的数据,长度29
        public static bool Readbyte(int Address, Enums.StorageType storageType, out int readValue, int plcAdd)
        {
            int i = 0;
            byte fcs;
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }
            Address = Address * 8;

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(plcAdd);
            byte[] Rbyte = ppiAddress.Rbyte;

            Rbyte[22] = 0x02;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
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
            //  Rbyte[31] = fcs;

            byte[] Receives = ReceiveReadByte(serialPort1, Rbyte, ppiAddress);

            if (Receives != null)
            {
                receiveByte = ByteHelper.ByteToString(Receives);
                readValue = (int)Receives[25];

                return true;
            }

            else
            {
                readValue = 0;
                return false;
            }


        }

        //public static bool Readbytes(int Address, Enums.StorageType storageType, out byte[] readValue, int plcAdd, int byteCount = 1)

        public static PPIReadWritePara Readbytes(PPIReadWritePara para)
        {

            if (para.ReadCount > 200)
            {
                para.ReadValue = new byte[] { 0 };
                return para;
            }
            int i = 0;
            byte fcs;

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }
            para.ByteAddress = para.ByteAddress * 8;

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] Rbyte = ppiAddress.Rbyte;
            Rbyte[22] = 0x02;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            Rbyte[24] = Convert.ToByte(para.ReadCount);//一次读取的个数

            if (para.StorageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)para.StorageType;


            Rbyte[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);
            //  Rbyte[31] = fcs;

            byte[] Receives = ReceiveReadByte(serialPort1, Rbyte, ppiAddress);

            if (Receives != null)
            {
                receiveByte = ByteHelper.ByteToString(Receives);
                para.ReadValue = new byte[para.ReadCount];
                if (para.StorageType == Enums.StorageType.T)
                {
                    for (int j = 0; j < para.ReadCount; j++)
                    {
                        para.ReadValue[j] = Receives[24 + j];//等待测试
                    }
                }
                else
                {
                    for (int j = 0; j < para.ReadCount; j++)
                    {
                        para.ReadValue[j] = Receives[25 + j];
                    }

                }
                para.IsSuceess = true;

            }

            return para;

        }


        //读取一个字存储单元的数据
        //public static bool ReadWord(int Address, Enums.StorageType storageType, out byte[] WordValue, int plcAdd)
        public static PPIReadWritePara ReadWord(PPIReadWritePara para)

        {
            int i = 0;
            byte fcs;

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }
            para.ByteAddress = para.ByteAddress * 8;

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] Rbyte = ppiAddress.Rbyte;
            Rbyte[22] = 0x04;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            Rbyte[24] = 0x01;//一次读取的个数

            if (para.StorageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)para.StorageType;

            Rbyte[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);

            byte[] Receives = ReceiveReadByte(serialPort1, Rbyte, ppiAddress);

            if (Receives != null)
            {
                receiveByte = ByteHelper.ByteToString(Receives);
                para.ReadValue = new byte[2];
                serialPort1.Read(Receives, 0, 30);
                para.ReadValue[0] = Receives[25];
                para.ReadValue[1] = Receives[26];
                receiveByte = ByteHelper.ByteToString(Receives);
                para.IsSuceess = true;

            }

            return para;
        }



        //public static bool ReadWords(int Address, Enums.StorageType storageType, out byte[] WordValue, int plcAdd, int WordCount = 1)
        public static PPIReadWritePara ReadWords(PPIReadWritePara para)
        {
            if (para.ReadCount > 128)
            {
                para.ReadValue = new byte[] { 0 };
                return para;
            }
            int i = 0;
            byte fcs;

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }
            para.ByteAddress = para.ByteAddress * 8;

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] Rbyte = ppiAddress.Rbyte;
            if (para.StorageType == Enums.StorageType.C)
            {
                Rbyte[22] = 0x1e;
            }
            else
            {

                Rbyte[22] = 0x04;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            }

            Rbyte[24] = Convert.ToByte(para.ReadCount);//一次读取的个数

            if (para.StorageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)para.StorageType;

            Rbyte[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);

            byte[] Receives = ReceiveReadByte(serialPort1, Rbyte, ppiAddress);

            if (Receives != null)
            {

                para.ReadValue = new byte[para.ReadCount * 2];

                for (int j = 0; j < para.ReadValue.Length; j++)
                {
                    para.ReadValue[para.ReadValue.Length - 1 - j] = Receives[Receives.Length - 3 - j];
                }

                receiveByte = ByteHelper.ByteToString(Receives);
                para.IsSuceess = true;

            }

            return para;

        }

        //public static PPIReadWritePara ReadDWord(int Address, Enums.StorageType storageType, out byte[] WordValue, int plcAdd)
        public static PPIReadWritePara ReadDWord(PPIReadWritePara para)
        {
            int i = 0;
            byte fcs;

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] Rbyte = ppiAddress.Rbyte;
            para.ByteAddress = para.ByteAddress * 8;
            Rbyte[22] = 0x06;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1  Word 06： Double Word
            Rbyte[24] = 0x01;//一次读取的个数

            if (para.StorageType == Enums.StorageType.V)
            {
                Rbyte[26] = 0x01;
            }
            else
            {
                Rbyte[26] = 0x00;
            }
            Rbyte[27] = (byte)para.StorageType;

            Rbyte[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            Rbyte[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Rbyte[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += Rbyte[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Rbyte[31] = Convert.ToByte(tt);

            byte[] Receives = ReceiveReadByte(serialPort1, Rbyte, ppiAddress);

            if (Receives != null)
            {
                para.ReadValue = new byte[4] { Receives[25], Receives[26], Receives[27], Receives[28] };

                receiveByte = ByteHelper.ByteToString(Receives);
                para.IsSuceess = true;

            }


            return para;
        }

        //public static bool TReadDword(int Address, out byte[] value, int plcAdd)

        public static PPIReadWritePara TReadDword(PPIReadWritePara para)
        {

            int i = 0;
            byte fcs;

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }
            para.ByteAddress = para.ByteAddress * 8;

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] TReadByte = ppiAddress.TReadByte;
            TReadByte[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            TReadByte[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            TReadByte[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += TReadByte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            TReadByte[31] = Convert.ToByte(tt);
            //  Rbyte[31] = fcs;

            byte[] Receives = ReceiveReadByte(serialPort1, TReadByte, ppiAddress);

            if (Receives != null)
            {
                para.ReadValue = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    para.ReadValue[j] = Receives[26 + j];
                }

                receiveByte = ByteHelper.ByteToString(Receives);
                para.IsSuceess = true;

            }

            return para;

        }


        public static byte[] ReceiveReadByte(SerialPort sPort, byte[] sendData, PPIAddress ppiAddress)
        {
            byte[] Receives = new byte[100];
            sPort.DiscardInBuffer();
            sPort.DiscardOutBuffer();
            sPort.Write(sendData, 0, sendData.Length);

            sendCmd = ByteHelper.ByteToString(sendData);
            ReadTimeOut(sPort, 1, 1);


            if (sPort.BytesToRead > 0)
            {
                if (0xe5 == sPort.ReadByte())
                {
                    sPort.DiscardInBuffer();
                    sPort.DiscardOutBuffer();

                    sPort.Write(ppiAddress.Affirm, 0, ppiAddress.Affirm.Length);
                }
                ReadTimeOut(sPort, 1, 20);
                sPort.Read(Receives, 0, Receives.Length);


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

                receiveByte = ByteHelper.ByteToString(ReceivesResult);
                return ReceivesResult;

            }

            return null;


        }

        public static void  ReadTimeOut(SerialPort sPort,int seconds,int byteLenth)
        {
            while (sPort.BytesToRead < byteLenth)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                if (sw.Elapsed > TimeSpan.FromSeconds(seconds))
                {
                    break;
                }
                sw.Stop();

            }
            
        }


        #endregion


        #region 写
        //一次写一个 Double Word 类型的数据。写命令是 40 个字节，其余为 38 个字节
        //写一个 Double Word 类型的数据，前面的 0~21 字节为 ：68 23 23 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10
        //写一个其他类型的数据，前面的 0~21 字节为 ：68 21 21 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10
        //  68 21 21 68 02 00 6C 32 01 00 00 00 00 00 0E 00 06 05 01 12 0A 10
        //从 22 字节开始根据写入数据的值和位置不同而变化

        //public static bool WriteBit(int ByteAddress, byte bitnumber, Enums.StorageType storageType, int WriteValue, int plcAdd)
        public static bool WriteBit(PPIReadWritePara para)
        {
            if (para.WriteValue > 255)
            {
                return false;
            }//最大写入值255

            int i = 0;
            byte fcs;


            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }

            para.ByteAddress = para.ByteAddress * 8 + para.Bitnumber;

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] Wbit = ppiAddress.Wbit;
            Wbit[22] = 0x01;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word

            if (para.StorageType == Enums.StorageType.T)
            {
                Wbit[22] = 0x1F;
            }
            if (para.StorageType == Enums.StorageType.C)
            {
                Wbit[22] = 0x1E;
            }

            Wbit[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据

            if (para.StorageType == Enums.StorageType.V)
            {
                Wbit[26] = 0x01;
            }
            else
            {
                Wbit[26] = 0x00;
            }
            Wbit[27] = (byte)para.StorageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            Wbit[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            Wbit[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Wbit[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            Wbit[35] = Convert.ToByte(para.WriteValue);
            for (i = 4, fcs = 0; i < 36; i++)
            {
                fcs += Wbit[i];
                
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Wbit[36] = Convert.ToByte(tt);

            return IsWriteSuccess(Wbit, serialPort1, ppiAddress);
           
        }



        public static bool Writebyte(PPIReadWritePara para)
        {

            if (para.WriteValue > 255)
            {
                return false;
            }//最大写入值255
            int i = 0;
            byte fcs;



            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }

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

            return IsWriteSuccess(Wbyte, serialPort1, ppiAddress);

        }



        public static bool WriteWord(PPIReadWritePara para)
        {

            //if (WriteValue > 255)
            //{
            //    return false;
            //}//最大写入值255
            int i = 0;
            byte fcs;



            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }

            para.ByteAddress = para.ByteAddress * 8;

            PPIAddress ppiAddress = new PPIAddress();

            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] Wword = ppiAddress.Wword;

            Wword[22] = 0x04;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 =Word06： Double Word
            Wword[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据

            if (para.StorageType == Enums.StorageType.V)
            {
                Wword[26] = 0x01;
            }
            else
            {
                Wword[26] = 0x00;
            }
            Wword[27] = (byte)para.StorageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            Wword[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            Wword[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            Wword[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            Wword[32] = 0x04;

            Wword[34] = 0x10;

            Wword[35] = Convert.ToByte(para.WriteValue / 256);
            Wword[36] = Convert.ToByte(para.WriteValue % 256);
            for (i = 4, fcs = 0; i < Wword.Length - 2; i++)
            {
                fcs += Wword[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            Wword[37] = Convert.ToByte(tt);

            return IsWriteSuccess(Wword, serialPort1, ppiAddress);

        }

    
        public static bool WriteDWord(PPIReadWritePara para)
        {

            //if (writeValue > uint.MaxValue)
            //{
            //    return false;
            //}//最大写入值0xffffffff,4,294,967,295
            int i = 0;
            byte fcs;
            
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }
            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] WDword = ppiAddress.WDword;

            para.ByteAddress = para.ByteAddress * 8;
            WDword[22] = 0x06;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            WDword[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据


            if (para.StorageType == Enums.StorageType.V)
            {
                WDword[26] = 0x01;
            }
            else
            {
                WDword[26] = 0x00;
            }


            WDword[27] = (byte)para.StorageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            WDword[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            WDword[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            WDword[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            WDword[32] = 0x04;
            WDword[34] = 0x20;

            WDword[35] = Convert.ToByte(para.WriteValue / 0x1000000);
            // long hh = (writeValue/0x10000) & 0xff;


            WDword[36] = Convert.ToByte((para.WriteValue / 0x10000) & 0xff);
            WDword[37] = Convert.ToByte((para.WriteValue / 0x100) & 0xff);
            WDword[38] = Convert.ToByte(para.WriteValue % 256);

            for (i = 4, fcs = 0; i < WDword.Length - 2; i++)
            {
                fcs += WDword[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            WDword[WDword.Length - 2] = Convert.ToByte(tt);


            return IsWriteSuccess(WDword, serialPort1, ppiAddress);

        }




        public static bool TwriteDWord(PPIReadWritePara para)
        {
            //if (writeValue > uint.MaxValue)
            //{
            //    return false;
            //}//最大写入值0xffffffff,4,294,967,295
            int i = 0;
            byte fcs;


            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }

            para.ByteAddress = para.ByteAddress * 8;

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] TWritebyte = ppiAddress.TWritebyte;

            TWritebyte[28] = Convert.ToByte(para.ByteAddress / 0x10000);
            TWritebyte[29] = Convert.ToByte((para.ByteAddress / 0x100) & 0xff);//0x100 ->256;
            TWritebyte[30] = Convert.ToByte(para.ByteAddress & 0xff);//低位，如320H，结果为20;

            TWritebyte[36] = Convert.ToByte(para.WriteValue / 0x1000000);
            TWritebyte[37] = Convert.ToByte((para.WriteValue / 0x10000) & 0xff);
            TWritebyte[38] = Convert.ToByte((para.WriteValue / 0x100) & 0xff);
            TWritebyte[39] = Convert.ToByte(para.WriteValue % 256);

            for (i = 4, fcs = 0; i < TWritebyte.Length - 2; i++)
            {
                fcs += TWritebyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            TWritebyte[TWritebyte.Length - 2] = Convert.ToByte(tt);


            return IsWriteSuccess(TWritebyte, serialPort1, ppiAddress);

        }



        public static bool CWriteWord(PPIReadWritePara para)
        {

            if (para.WriteValue > 65536)
            {
                return false;
            }//最大写入值0xfffff,4,294,967,295
            int i = 0;
            byte fcs;

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }

            para.ByteAddress = para.ByteAddress * 8;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);

            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            byte[] CwriteWordByte = ppiAddress.CwriteWordByte;
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

            return IsWriteSuccess(CwriteWordByte, serialPort1, ppiAddress);

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

        public static bool PLCStop(PPIReadWritePara para)
        {

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();

            }
            PPIAddress ppiAddress = new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);

            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();

            serialPort1.Write(ppiAddress.StopBytesyte, 0, ppiAddress.StopBytesyte.Length);
            //Thread.Sleep(10);
            string str = ByteHelper.ByteToString(ppiAddress.StopBytesyte);

            if (serialPort1.ReadByte() == 0xE5)
            {
                return true;
            }
            else
            {
                return false;
            }


        }


        public static bool PLCRun(PPIReadWritePara para)
        {

            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();

            }

           PPIAddress ppiAddress=new PPIAddress();
            ppiAddress.DAddress = Convert.ToByte(para.PlcAddress);
            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();

            serialPort1.Write(ppiAddress.RunBytes,0, ppiAddress.RunBytes.Length);
            string str = ByteHelper.ByteToString(ppiAddress.RunBytes);

            Thread.Sleep(100);
            int recei = serialPort1.ReadByte();
            if (recei == 0xE5)
            {
                return true;
            }
            else
            {
                return false;
            }


        }


        public static bool IsWriteSuccess(byte[] SendData, SerialPort port, PPIAddress ppiAddress)
        {

            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();

            serialPort1.Write(SendData, 0, SendData.Length);

            sendCmd = ByteHelper.ByteToString(SendData);
            Thread.Sleep(100);

            int Rece = serialPort1.ReadByte();

            if (Rece == 0xE5)
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();

                port.Write(ppiAddress.Affirm, 0, ppiAddress.Affirm.Length);
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);

                PortTimeout(ppiAddress.WriteReceivesCheck.Length, ts1, port);

                byte[] Receives = new byte[ppiAddress.WriteReceivesCheck.Length];
                port.Read(Receives, 0, Receives.Length);
                receiveByte =ByteHelper.ByteToString(Receives);
                string strcheck = ByteHelper.ByteToString(ppiAddress.WriteReceivesCheck);

                int n = 0;
                for (int j = 0; j < 24; j++)
                {
                    if (Receives[j] != ppiAddress.WriteReceivesCheck[j])
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
            else
            {

                return false;
            }

        }


    }
}
