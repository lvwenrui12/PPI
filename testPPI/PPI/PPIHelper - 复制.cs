using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace testPPI.PPI
{
    public class PPIHelper
    {

        //2017-5 吕文瑞

        //LE为从DA到DU的数据长度，以字节计，如读一个数据时始终为1BH；
        //LEr始终等于LE；

        //对于一次读取一个数据，读命令都是 33 个字节。前面的0~21 字节是相同的， 为 ：
        //68 1B 1B 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10


        //对于一次读多个数据的情况，前 21Byte 与上面相似，只是长度 LE、LEr 及 Byte 14 不同：
        //Byte 14 数据块占位字节，它指明数据块占用的字节数。与数据块数量有关，长度 = 4 + 数据块数 * 10,如：一条数据时为 4 + 10 = 0E(H)。



        #region 协议读格式
        //0    1   2   3   4   5   6   7   8   9   10  11  12  13  14  15  16  17  18  19  20  21  22  23  24  25  26  27  28  29  30  31  32
        //SD  LE  LER SD  DA   SA  FC  CC----------CC  GU------------------GU  DU--------------------------------------------------DU  FSC DE
        //起  长  长  起  PLC  上  功  协  远  冗  冗  协  单  参  参  数  数   4  变              读      数      存      偏          校  终
        //始  度  度  始  地   位  能  议  程  余  余  议  元  数  数  据  据  读  量              取      据      储      移          验  止
        //符      重  符  址   机  码  识  控  识  识  数  参  长  长  长  长   5  地              长      个      器      量          码  符
        //        复           地      别  制  别  别  据  考  度  度  度  度  写  址              度      数      类                      
        //                     址                                                  数                              型                                                                            
        //68  1B  1B  68  02   00  6C  32  01  00  00  00  00  00  0E  00  00  04  01  12  0A  10  02  00  08  00  00  03  00  05  E0 D2  16

        #endregion

        //private System.IO.Ports.SerialPort serialPort1 = new SerialPort("COM2", 19200, Parity.Even, 8, StopBits.One);
        public static  System.IO.Ports.SerialPort serialPort1=new SerialPort();
        public PPIHelper()
        {

        


        }


        #region 预定义字符串
        // PLC会返回 E5，意思：已经收到
        //那么这时上位机再次发送指令 10 02 00 5C 5E 16 意思：请执行命令。

        //再次发送指令 10 02 00 5C 5E 16 意思：请执行命令,其中，SD为起始符，始终为10H；DA为读指令的目的地址；SA源地址；FCS校验和，等于DA+SA+FC的数据和；ED始终为16H
        //SD DA  SA FC  FCS  ED
        //10 02	 00	5C	5E	 16
        private byte[] Affirm = { 0x10, 0x02, 0x00, 0x5c, 0x5e, 0x16 };



        private byte DAddress { get; set; } = 0x02;//PLC地址，DA，默认情况下，PLC的地址为02H
        private byte SAddress { get; set; } = 0x00;//上位机地址 SA，//SA源地址，默认情况下，PC机地址为00H，HMI设备的地址为01H

        private byte[] Rbyte = { 0x68, 0x1B, 0x1B,//LE为从DA到DU的数据长度，以字节计，如读一个数据时始终为1BH
            0x68, 0x02, 0x00, 0x6C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x04, 0x01, 0x12, 0x0A, 0x10, 0x02,//byte[22]02代表读取数据长度为byte，即8个bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01,//byte[24]读取的个数
            0x00, 0x01,//存储类型01:V,其他的0
            0x84, 0x00, 0x03, 0x20, 0x8b, 0x16 };


        #endregion

        //[Browsable(true)]
        //[MonitoringDescription("DestinationAddress")]
        //[DefaultValue(2)]
        public byte DestinationAddress
        {
            get { return this.DAddress; }
            set
            {
                this.DAddress = value;
                this.Rbyte[4] = this.DAddress;
                this.Wbyte[4] = this.DAddress;
                this.Wbit[4] = this.DAddress;
                this.Wword[4] = this.DAddress;
                this.Affirm[1] = this.DAddress;
            }
        }

        //[Browsable(true)]
        //[MonitoringDescription("SourceAddress")]
        //[DefaultValue(0)]
        public byte SourceAddress
        {
            get { return this.SAddress; }
            set
            {
                this.SAddress = value;
                this.Rbyte[5] = this.SAddress;
                this.Wbyte[5] = this.SAddress;

                this.Wbit[5] = this.SAddress;
                this.Wword[5] = this.SAddress;
                this.Affirm[2] = this.SAddress;
            }
        }


        #region 读

        //读取某个位的状态
        public bool Readbit(int ByteAddress, int bitnumber, Enums.StorageType storageType, Enums.StorageTypeIsV isV, out int bitValue)
        {
            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[28];

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }

            ByteAddress = ByteAddress * 8 + bitnumber;
            this.Rbyte[22] = 0x01;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Rbyte[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据

            this.Rbyte[26] = (byte)isV;
            this.Rbyte[27] = (byte)storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            this.Rbyte[28] = Convert.ToByte(ByteAddress / 0x10000);
            this.Rbyte[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff);//0x100 ->256;
            this.Rbyte[30] = Convert.ToByte(ByteAddress & 0xff);//低位，如320H，结果为20;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += this.Rbyte[i];


            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.Rbyte[31] = Convert.ToByte(tt);

            //再次发送指令 10 02 00 5C 5E 16 意思：请执行命令,其中，SD为起始符，始终为10H；DA为读指令的目的地址；SA源地址；FCS校验和，等于DA+SA+FC的数据和；ED始终为16H
            //SD DA  SA FC  FCS  ED
            //10 02	 00	5C	5E	 16
            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Rbyte, 0, 33);
            while (this.serialPort1.BytesToRead == 0) {; }
            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
                while (this.serialPort1.BytesToRead < 27)//返回的数据比在串口助手少第一个数据 0x68
                {
                    System.Threading.Thread.Sleep(100);

                    TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan ts = ts2.Subtract(ts1).Duration();
                    //如果接收时间超出设定时间，则直接退出
                    if (ts.TotalMilliseconds > 5000)
                    {
                        bitValue = 0;
                        return false;
                    }

                }// 长度为27

                this.serialPort1.Read(Receives, 0, 28);

                bitValue = (int)Receives[24];//
                return true;
            }
            else
            {
                bitValue = 0;
                return false;
            }
        }


        public bool Readbits(int ByteAddress, int bitnumber,int bitCount, Enums.StorageType storageType, Enums.StorageTypeIsV isV, out int[] bitValue)
        {
            if (bitCount>255||bitCount==0||bitCount<0)
            {

                bitValue =new int[] {0};
                return false;
            }

            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[27+bitCount];

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }

            ByteAddress = ByteAddress * 8 + bitnumber;
            this.Rbyte[22] = 0x01;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Rbyte[24] = Convert.ToByte(bitCount);// Byte 24 为数据个数：这里是 01，一次读一个数据

            this.Rbyte[26] = (byte)isV;
            this.Rbyte[27] = (byte)storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            this.Rbyte[28] = Convert.ToByte(ByteAddress / 0x10000);
            this.Rbyte[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff);//0x100 ->256;
            this.Rbyte[30] = Convert.ToByte(ByteAddress & 0xff);//低位，如320H，结果为20;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += this.Rbyte[i];
                
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.Rbyte[31] = Convert.ToByte(tt);

            //再次发送指令 10 02 00 5C 5E 16 意思：请执行命令,其中，SD为起始符，始终为10H；DA为读指令的目的地址；SA源地址；FCS校验和，等于DA+SA+FC的数据和；ED始终为16H
            //SD DA  SA FC  FCS  ED
            //10 02	 00	5C	5E	 16
            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Rbyte, 0, Rbyte.Length);
            while (this.serialPort1.BytesToRead == 0) {; }
            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
                while (this.serialPort1.BytesToRead < Receives.Length-1)//返回的数据比在串口助手少第一个数据 0x68
                {
                    System.Threading.Thread.Sleep(100);

                    TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan ts = ts2.Subtract(ts1).Duration();
                    //如果接收时间超出设定时间，则直接退出
                    if (ts.TotalMilliseconds > 5000)
                    {
                        bitValue = new int[] { 0 };
                     
                        return false;
                    }

                }// 长度为27

                this.serialPort1.Read(Receives, 0, Receives.Length);

                bitValue=new int[bitCount];
                for (int j = 0; j < bitCount; j++)
                {
                    bitValue[j] = (int) Receives[24 + j];
                }
             
                return true;
            }
            else
            {
                bitValue = new int[] { 0 };
                return false;
            }
        }//读取失败


        //读取一个字节存储单元的数据,长度29
        public bool Readbyte(int Address, Enums.StorageType storageType, Enums.StorageTypeIsV isV, out int readValue)
        {
            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[29];

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }
            Address = Address * 8;

            this.Rbyte[22] = 0x02;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Rbyte[24] = 0x01;//一次读取的个数

            this.Rbyte[26] = (byte)isV;
            this.Rbyte[27] = (byte)storageType;

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            this.Rbyte[28] = Convert.ToByte(Address / 0x10000);
            this.Rbyte[29] = Convert.ToByte((Address / 0x100) & 0xff);//0x100 ->256;
            this.Rbyte[30] = Convert.ToByte(Address & 0xff);//低位，如320H，结果为20;
            //this.Rbyte[28] = this.fbyte3;
            //this.Rbyte[29] = this.fbyte2;
            //this.Rbyte[30] = this.fbyte1;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += this.Rbyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.Rbyte[31] = Convert.ToByte(tt);
            //  this.Rbyte[31] = fcs;

            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }

            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);
            // this.Affirm[4] = fcs;

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Rbyte, 0, Rbyte.Length);
            while (this.serialPort1.BytesToRead == 0) {; }
            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                Thread.Sleep(100);
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
                while (this.serialPort1.BytesToRead < 27)//返回的数据比在串口助手少第一个数据 0x68
                {
                    System.Threading.Thread.Sleep(100);
                    TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan ts = ts2.Subtract(ts1).Duration();
                    //如果接收时间超出设定时间，则直接退出
                    if (ts.TotalMilliseconds > 5000)
                    {
                        readValue = 0;
                        return false;
                    }

                }// 长度为27

                this.serialPort1.Read(Receives, 0, 28);
                this.serialPort1.Close();

                readValue = (int)Receives[25];
                return true;
            }
            else
            {
                readValue = 0;
                return false;
            }
        }

        public bool Readbytes(int Address,int byteCount, Enums.StorageType storageType, Enums.StorageTypeIsV isV, out byte [] readValue)
        {

            if (byteCount>222||byteCount==0||byteCount<0)
            {
                readValue=new byte[] {0};
                return false;
            }
            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[27+byteCount];

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }
            Address = Address * 8;

            this.Rbyte[22] = 0x02;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Rbyte[24] = Convert.ToByte(byteCount);//一次读取的个数

            this.Rbyte[26] = (byte)isV;
            this.Rbyte[27] = (byte)storageType;

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            this.Rbyte[28] = Convert.ToByte(Address / 0x10000);
            this.Rbyte[29] = Convert.ToByte((Address / 0x100) & 0xff);//0x100 ->256;
            this.Rbyte[30] = Convert.ToByte(Address & 0xff);//低位，如320H，结果为20;
            //this.Rbyte[28] = this.fbyte3;
            //this.Rbyte[29] = this.fbyte2;
            //this.Rbyte[30] = this.fbyte1;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += this.Rbyte[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.Rbyte[31] = Convert.ToByte(tt);
            //  this.Rbyte[31] = fcs;

            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }

            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);
            // this.Affirm[4] = fcs;

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Rbyte, 0, Rbyte.Length);
            while (this.serialPort1.BytesToRead == 0) {; }
            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                Thread.Sleep(100);
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
                while (this.serialPort1.BytesToRead < 27)//返回的数据比在串口助手少第一个数据 0x68
                {
                    System.Threading.Thread.Sleep(100);
                    TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan ts = ts2.Subtract(ts1).Duration();
                    //如果接收时间超出设定时间，则直接退出
                    if (ts.TotalMilliseconds > 5000)
                    {
                        readValue = new byte[] {0};
                        return false;
                    }

                }

                this.serialPort1.Read(Receives, 0, Receives.Length);
                this.serialPort1.Close();
                readValue=new byte[byteCount];
                for (int j = 0; j < byteCount; j++)
                {
                    readValue[j] = Receives[25 + j];
                }
               
                return true;
            }
            else
            {
                readValue = new byte[] {0};
                return false;
            }
        }//测试没有读到数据，还没有成功




        //读取一个字存储单元的数据
        public bool ReadWord(int Address, Enums.StorageType storageType, Enums.StorageTypeIsV isV,out byte[] WordValue)
        {
            int i,  Rece = 0;
            byte fcs;
            byte[] Receives = new byte[30];

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }
            Address = Address * 8;
            this.Rbyte[22] = 0x04;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Rbyte[24] = 0x01;//一次读取的个数

            this.Rbyte[26] = (byte)isV;
            this.Rbyte[27] = (byte)storageType;

            this.Rbyte[28] = Convert.ToByte(Address / 0x10000);
            this.Rbyte[29] = Convert.ToByte((Address / 0x100) & 0xff);//0x100 ->256;
            this.Rbyte[30] = Convert.ToByte(Address & 0xff);//低位，如320H，结果为20;
            //this.Rbyte[28] = this.fbyte3;
            //this.Rbyte[29] = this.fbyte2;
            //this.Rbyte[30] = this.fbyte1;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += this.Rbyte[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.Rbyte[31] = Convert.ToByte(tt);

            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);
            // this.Affirm[4] = fcs;

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Rbyte, 0, Rbyte.Length);
            Thread.Sleep(200);
            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                //for (i = 0; i < 10000; i++)
                //    for (int j = 0; j < 1000; j++) ;
                //  while (this.serialPort1.BytesToRead < 29) {; }
                Thread.Sleep(200);

                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
                while (this.serialPort1.BytesToRead <29)//返回的数据比在串口助手少第一个数据 0x68
                {
                    System.Threading.Thread.Sleep(100);
                    TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan ts = ts2.Subtract(ts1).Duration();
                    //如果接收时间超出设定时间，则直接退出
                    if (ts.TotalMilliseconds > 5000)
                    {
                        WordValue =new byte[] {0};
                        return false;
                    }

                }// 长度为30

                WordValue=new byte[2];
                this.serialPort1.Read(Receives, 0, 30);
                WordValue[0] = Receives[25];
                WordValue[1] = Receives[26];
                //WordValue = (int)(Receives[25] * 256 + Receives[26]);
                return true;
            }
            this.serialPort1.Close();

            WordValue =new byte[] {0};
            return false;
        }


        public bool ReadWords(int Address,int WordCount, Enums.StorageType storageType, Enums.StorageTypeIsV isV, out byte [] WordValue)
        {
            if (WordCount>255||WordCount==0||WordCount<0)
            {
                WordValue=new byte[] {0};
                return false;
            }
            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[28+ WordCount*2];

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }
            Address = Address * 8;
            this.Rbyte[22] = 0x04;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Rbyte[24] = Convert.ToByte(WordCount);//一次读取的个数

            this.Rbyte[26] = (byte)isV;
            this.Rbyte[27] = (byte)storageType;

            this.Rbyte[28] = Convert.ToByte(Address / 0x10000);
            this.Rbyte[29] = Convert.ToByte((Address / 0x100) & 0xff);//0x100 ->256;
            this.Rbyte[30] = Convert.ToByte(Address & 0xff);//低位，如320H，结果为20;
            //this.Rbyte[28] = this.fbyte3;
            //this.Rbyte[29] = this.fbyte2;
            //this.Rbyte[30] = this.fbyte1;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += this.Rbyte[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.Rbyte[31] = Convert.ToByte(tt);

            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);
            // this.Affirm[4] = fcs;

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Rbyte, 0, Rbyte.Length);
            Thread.Sleep(200);
            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                //for (i = 0; i < 10000; i++)
                //    for (int j = 0; j < 1000; j++) ;
                //  while (this.serialPort1.BytesToRead < 29) {; }
                Thread.Sleep(200);

                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
                while (this.serialPort1.BytesToRead < 27+WordCount*2)//返回的数据比在串口助手少第一个数据 0x68
                {
                    System.Threading.Thread.Sleep(100);
                    TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan ts = ts2.Subtract(ts1).Duration();
                    //如果接收时间超出设定时间，则直接退出
                    if (ts.TotalMilliseconds > 5000)
                    {
                      WordValue=new byte[] {0};
                        return false;
                    }

                }// 长度为30

                WordValue = new byte[WordCount*2];
                this.serialPort1.Read(Receives, 0, 30);
                for (int j = 0; j < WordValue.Length; j++)
                {
                    WordValue[j] = Receives[25 + j];
                }


                //WordValue = (int)(Receives[25] * 256 + Receives[26]);
                return true;
            }
            this.serialPort1.Close();

            WordValue = new byte[] { 0 };
            return false;
        }


        public bool ReadDWord(int Address, Enums.StorageType storageType, Enums.StorageTypeIsV isV, out long WordValue)
        {
            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[30];

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }
            Address = Address * 8;
            this.Rbyte[22] = 0x06;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Rbyte[24] = 0x01;//一次读取的个数

            this.Rbyte[26] = (byte)isV;
            this.Rbyte[27] = (byte)storageType;

            this.Rbyte[28] = Convert.ToByte(Address / 0x10000);
            this.Rbyte[29] = Convert.ToByte((Address / 0x100) & 0xff);//0x100 ->256;
            this.Rbyte[30] = Convert.ToByte(Address & 0xff);//低位，如320H，结果为20;
            //this.Rbyte[28] = this.fbyte3;
            //this.Rbyte[29] = this.fbyte2;
            //this.Rbyte[30] = this.fbyte1;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += this.Rbyte[i];
            }
            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.Rbyte[31] = Convert.ToByte(tt);

            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);
            // this.Affirm[4] = fcs;

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Rbyte, 0, Rbyte.Length);
            Thread.Sleep(200);
            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                //for (i = 0; i < 10000; i++)
                //    for (int j = 0; j < 1000; j++) ;
                //  while (this.serialPort1.BytesToRead < 29) {; }
                Thread.Sleep(200);

                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
                while (this.serialPort1.BytesToRead < 30)//返回的数据比在串口助手少第一个数据 0x68
                {
                    System.Threading.Thread.Sleep(100);
                    TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan ts = ts2.Subtract(ts1).Duration();
                    //如果接收时间超出设定时间，则直接退出
                    if (ts.TotalMilliseconds > 5000)
                    {
                        WordValue = 0;
                        return false;
                    }

                }// 长度为30


                this.serialPort1.Read(Receives, 0, 30);
                long d1 = Convert.ToInt32(Receives[25]);//用long类型避免出现负数
                long c1 = d1 * 16777216;
                long c2 = Receives[26] * 0x10000;
                long c3 = Receives[27] * 256;
                long c4 = Receives[28];
                 WordValue = c1 + c2 + c3 + c4;
                return true;
            }
            this.serialPort1.Close();

            WordValue = 0;
            return false;
        }

        #endregion
        #region 写
        //一次写一个 Double Word 类型的数据。写命令是 40 个字节，其余为 38 个字节
        //写一个 Double Word 类型的数据，前面的 0~21 字节为 ：68 23 23 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10
        //写一个其他类型的数据，前面的 0~21 字节为 ：68 21 21 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10
        //  68 21 21 68 02 00 6C 32 01 00 00 00 00 00 0E 00 06 05 01 12 0A 10
        //从 22 字节开始根据写入数据的值和位置不同而变化


        #region 预定义写字符串

        private byte[] Wbit = { 0x68, 0x20, 0x20, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x05, 0x05, 0x01, 0x12, 0x0A, 0x10,//前面0-21位
            0x01,//byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00,
            0x01,//byte[24]数据个数
            0x00,
             0x01, 0x84,//存储器类型
            0x00, 0x00, 0x00,//byte[28],byte[29],byte[30]偏移量
            0x00, 0x03,//数据形式03 bit 其他04
            0x00, 0x01,//数据位数
            0x01,//值
            0x80, 0x16 };// 38 个字节


        private byte[] Wbyte = { 0x68, 0x20, 0x20, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x05, 0x05, 0x01, 0x12, 0x0A, 0x10,//前面0-21位
            0x02,////byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, 0x00, 0x01, 0x84,//存储器类型
            0x00, 0x03, 0x20,////byte[28],byte[29],byte[30]偏移量
            0x00, 0x04,//byte[32]数据形式03 bit 其他04
            0x00, 0x08,//数据位数01 bit 10:word 08 byte 20: Double Word
            0x10,//value
            0xbd, 0x16 };


        private byte[] Wword = { 0x68, 0x21, 0x21, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x06, 0x05, 0x01, 0x12, 0x0A, 0x10,
            0x04,//byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, 0x00, 0x01, 0x84,//存储器类型
            0x00, 0x03, 0x20,//byte[28],byte[29],byte[30]偏移量
            0x00, 0x04,//byte[32]数据形式03 bit 其他04
            0x00, 0x10,//数据位数01 bit 10:word 08 byte 20: Double Word
            0xab, 0xcd,//value
            0x30, 0x16 };

        private byte[] WDword = { 0x68, 0x23, 0x23, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x06, 0x05, 0x01, 0x12, 0x0A, 0x10,
            0x06,//byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, 0x00, 0x01, 0x84,//存储器类型
            0x00, 0x03, 0x20,//byte[28],byte[29],byte[30]偏移量
            0x00, 0x04,//byte[32]数据形式03 bit 其他04
            0x00, 0x20,//数据位数01 bit 10:word 08 byte 20: Double Word
            0xab, 0xcd,//value
            0xab, 0xcd,//value
            0x30, 0x16 };

        #endregion




        public bool WriteBit(int ByteAddress, byte bitnumber, Enums.StorageType storageType, Enums.StorageTypeIsV isV, int WriteValue)
        {
            if (WriteValue > 255)
            {
                return false;
            }//最大写入值255

            int i, Rece = 0;
            byte fcs;

            //位写操作返回都是字符串为：68 12 12 68 00 02 08 32 03 00 00 00 00 00 02 00 01 00 00 05 01 FF 47 16
            byte[] ReceivesCheck = { 0x68, 0x12, 0x12, 0x68, 0x00, 0x02, 0x08, 0x32, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x05, 0x01, 0xFF, 0x47, 0x16 };

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }

            ByteAddress = ByteAddress * 8 + bitnumber;
            this.Wbit[22] = 0x01;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Wbit[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据

            this.Wbit[26] = (byte)isV;
            this.Wbit[27] = (byte)storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            this.Wbit[28] = Convert.ToByte(ByteAddress / 0x10000);
            this.Wbit[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff);//0x100 ->256;
            this.Wbit[30] = Convert.ToByte(ByteAddress & 0xff);//低位，如320H，结果为20;

            this.Wbit[35] = Convert.ToByte(WriteValue);
            for (i = 4, fcs = 0; i < 36; i++)
            {
                fcs += this.Wbit[i];


            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.Wbit[36] = Convert.ToByte(tt);

            //再次发送指令 10 02 00 5C 5E 16 意思：请执行命令,其中，SD为起始符，始终为10H；DA为读指令的目的地址；SA源地址；FCS校验和，等于DA+SA+FC的数据和；ED始终为16H
            //SD DA  SA FC  FCS  ED
            //10 02	 00	5C	5E	 16
            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Wbit, 0, this.Wbit.Length);
            //while (this.serialPort1.BytesToRead == 0) {; }

            Thread.Sleep(100);

            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);

                PortTimeout(24, ts1, this.serialPort1);

                byte[] Receives = new byte[24];
                this.serialPort1.Read(Receives, 0, 24);

                if (Receives == ReceivesCheck)
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

        

        public bool Writebyte(int ByteAddress, Enums.StorageType storageType, Enums.StorageTypeIsV isV, int WriteValue)
        {

            if (WriteValue > 255)
            {
                return false;
            }//最大写入值255
            int i, Rece = 0;
            byte fcs;

            //位写操作返回都是字符串为：68 12 12 68 00 02 08 32 03 00 00 00 00 00 02 00 01 00 00 05 01 FF 47 16
            byte[] ReceivesCheck = { 0x68, 0x12, 0x12, 0x68, 0x00, 0x02, 0x08, 0x32, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x05, 0x01, 0xFF, 0x47, 0x16 };

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }

            ByteAddress = ByteAddress * 8;
            this.Wbyte[22] = 0x02;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Wbyte[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据

            this.Wbyte[26] = (byte)isV;
            this.Wbyte[27] = (byte)storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            this.Wbyte[28] = Convert.ToByte(ByteAddress / 0x10000);
            this.Wbyte[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff);//0x100 ->256;
            this.Wbyte[30] = Convert.ToByte(ByteAddress & 0xff);//低位，如320H，结果为20;

            this.Wbyte[32] = 0x04;
            this.Wbyte[34] = 0x08;


            this.Wbyte[35] = Convert.ToByte(WriteValue);
            for (i = 4, fcs = 0; i < 36; i++)
            {
                fcs += this.Wbyte[i];


            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.Wbyte[36] = Convert.ToByte(tt);

            //再次发送指令 10 02 00 5C 5E 16 意思：请执行命令,其中，SD为起始符，始终为10H；DA为读指令的目的地址；SA源地址；FCS校验和，等于DA+SA+FC的数据和；ED始终为16H
            //SD DA  SA FC  FCS  ED
            //10 02	 00	5C	5E	 16
            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Wbyte, 0, this.Wbyte.Length);
            //while (this.serialPort1.BytesToRead == 0) {; }

            Thread.Sleep(100);

            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);//返回的数据比在串口助手少第一个数据 0x68

                PortTimeout(24, ts1, this.serialPort1);

                byte[] Receives = new byte[24];
                this.serialPort1.Read(Receives, 0, 24);

                int n = 0;
                for (int j = 0; j < 24; j++)
                {
                    if (Receives[j] != ReceivesCheck[j])
                    {
                        n++;
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


        public bool WriteWord(int byteAddress, Enums.StorageType storageType, Enums.StorageTypeIsV isV, int writeValue)
        {

            //if (WriteValue > 255)
            //{
            //    return false;
            //}//最大写入值255
            int i, Rece = 0;
            byte fcs;

            //位写操作返回都是字符串为：68 12 12 68 00 02 08 32 03 00 00 00 00 00 02 00 01 00 00 05 01 FF 47 16
            byte[] ReceivesCheck = { 0x68, 0x12, 0x12, 0x68, 0x00, 0x02, 0x08, 0x32, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x05, 0x01, 0xFF, 0x47, 0x16 };

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }

            byteAddress = byteAddress * 8;
            this.Wword[22] = 0x04;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Wword[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据

            this.Wword[26] = (byte)isV;
            this.Wword[27] = (byte)storageType;

            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            this.Wword[28] = Convert.ToByte(byteAddress / 0x10000);
            this.Wword[29] = Convert.ToByte((byteAddress / 0x100) & 0xff);//0x100 ->256;
            this.Wword[30] = Convert.ToByte(byteAddress & 0xff);//低位，如320H，结果为20;

            this.Wword[32] = 0x04;
            this.Wword[34] = 0x10;


            this.Wword[35] = Convert.ToByte(writeValue / 256);
            this.Wword[36] = Convert.ToByte(writeValue % 256);
            for (i = 4, fcs = 0; i < Wword.Length-2; i++)
            {
                fcs += this.Wword[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.Wword[36] = Convert.ToByte(tt);

            //再次发送指令 10 02 00 5C 5E 16 意思：请执行命令,其中，SD为起始符，始终为10H；DA为读指令的目的地址；SA源地址；FCS校验和，等于DA+SA+FC的数据和；ED始终为16H
            //SD DA  SA FC  FCS  ED
            //10 02	 00	5C	5E	 16
            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Wword, 0, this.Wword.Length);
            //while (this.serialPort1.BytesToRead == 0) {; }

            Thread.Sleep(100);

            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);//返回的数据比在串口助手少第一个数据 0x68

                PortTimeout(ReceivesCheck.Length, ts1, this.serialPort1);

                byte[] Receives = new byte[ReceivesCheck.Length];
                this.serialPort1.Read(Receives, 0, 24);

                int n = 0;
                for (int j = 0; j < 24; j++)
                {
                    if (Receives[j] != ReceivesCheck[j])
                    {
                        n++;
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

        public bool WriteDWord(int byteAddress, Enums.StorageType storageType, Enums.StorageTypeIsV isV, long writeValue)
        {

            if (writeValue > uint.MaxValue)
            {
                return false;
            }//最大写入值0xffffffff,4,294,967,295
            int i, Rece = 0;
            byte fcs;

            //位写操作返回都是字符串为：68 12 12 68 00 02 08 32 03 00 00 00 00 00 02 00 01 00 00 05 01 FF 47 16
            byte[] ReceivesCheck = { 0x68, 0x12, 0x12, 0x68, 0x00, 0x02, 0x08, 0x32, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x05, 0x01, 0xFF, 0x47, 0x16 };

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }

            byteAddress = byteAddress * 8;
            this.WDword[22] = 0x06;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.WDword[24] = 0x01;// Byte 24 为数据个数：这里是 01，一次读一个数据
               
            this.WDword[26] = (byte)isV;
            this.WDword[27] = (byte)storageType;
        
            //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
            //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[11] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[10] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[9] = Convert.ToByte(address / 0x10000);
            this.WDword[28] = Convert.ToByte(byteAddress / 0x10000);
            this.WDword[29] = Convert.ToByte((byteAddress / 0x100) & 0xff);//0x100 ->256;
            this.WDword[30] = Convert.ToByte(byteAddress & 0xff);//低位，如320H，结果为20;
               
            this.WDword[32] = 0x04;
            this.WDword[34] = 0x20;
            this.WDword[35] = Convert.ToByte(writeValue / 0x1000000);
            this.WDword[36] = Convert.ToByte((writeValue/0x10000) & 0xffff);
            this.WDword[37] = Convert.ToByte((writeValue / 0x100)&0xff);
            this.WDword[38] = Convert.ToByte(writeValue % 256);
         
            for (i = 4, fcs = 0; i < Wword.Length - 2; i++)
            {
                fcs += this.WDword[i];
            }

            int tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256

            this.WDword[Wword.Length-2] = Convert.ToByte(tt);

            //再次发送指令 10 02 00 5C 5E 16 意思：请执行命令,其中，SD为起始符，始终为10H；DA为读指令的目的地址；SA源地址；FCS校验和，等于DA+SA+FC的数据和；ED始终为16H
            //SD DA  SA FC  FCS  ED
            //10 02	 00	5C	5E	 16
            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            tt = Convert.ToInt32(fcs) % 256;//添加的代码 mod 256
            this.Affirm[4] = Convert.ToByte(tt);

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.WDword, 0, this.WDword.Length);
            //while (this.serialPort1.BytesToRead == 0) {; }

            Thread.Sleep(100);

            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);//返回的数据比在串口助手少第一个数据 0x68

                PortTimeout(ReceivesCheck.Length, ts1, this.serialPort1);

                byte[] Receives = new byte[ReceivesCheck.Length];
                this.serialPort1.Read(Receives, 0, 24);

                int n = 0;
                for (int j = 0; j < 24; j++)
                {
                    if (Receives[j] != ReceivesCheck[j])
                    {
                        n++;
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





        #endregion


        public void PortTimeout(int byteLength, TimeSpan ts1, SerialPort port)
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
