using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace testPPI
{
    class PPIReferrence
    {
        /// <summary>
        /// 该类需要的变量声明
        /// </summary>
        private System.IO.Ports.SerialPort serialPort1 = new SerialPort("COM2", 19200, Parity.Even, 8, StopBits.One);
        private byte DAddress = 0x02, SAddress = 0x00;
        private byte fbyte1 = 0x00, fbyte2 = 0x00, fbyte3 = 0x00, fbyte4 = 0x00;
        private string mystring, str1, str2, str3, str4;

        // PLC会返回 E5，意思：已经收到
        //那么这时上位机再次发送指令 10 02 00 5C 5E 16 意思：请执行命令。
        private byte[] Affirm = { 0x10, 0x02, 0x00, 0x5c, 0x5e, 0x16 };

        private byte[] Rbyte = { 0x68, 0x1B,//为从DA到DU的数据长度，以字节计，如读一个数据时始终为1BH,也就是byte[4]到byte[30]的长度 30-4+1=27
            0x1B, 0x68, 0x02, 0x00, 0x6C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x04, 0x01, 0x12, 0x0A, 0x10, 0x02,//02代表读取数据长度为byte，即8个bit
            0x00, 0x08, 0x00, 0x00, 0x03, 0x00, 0x05, 0xE0, 0xD2, 0x16 };

        private byte[] Rbyte2 = { 0x68, 0x1B,//为从DA到DU的数据长度，以字节计，如读一个数据时始终为1BH,也就是byte[4]到byte[30]的长度 30-4+1=27
            0x1B, 0x68, 0x02, 0x00, 0x6C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x04, 0x01, 0x12, 0x0A, 0x10, 0x02,//02代表读取数据长度为byte，即8个bit
            0x00, 0x01, 0x00, 0x01, 0x84, 0x00, 0x03, 0x20, 0x8b, 0x16 };

        private byte[] Wbyte = { 0x68, 0x20, 0x20, 0x68, 0x02, 0x00, 0x6C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x05, 0x05, 0x01, 0x12, 0x0A, 0x10, 0x02, 0x00, 0x01, 0x00, 0x01, 0x84, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x08, 0x01, 0xCF, 0x16 };
        private byte[] Wword = { 0x68, 0x21, 0x21, 0x68, 0x02, 0x00, 0x6C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x06, 0x05, 0x01, 0x12, 0x0A, 0x10, 0x04, 0x00, 0x01, 0x00, 0x01, 0x84, 0x00, 0x03, 0x20, 0x00, 0x04, 0x00, 0x10, 0x00, 0x10, 0xB9, 0x16 };

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
                this.Wword[5] = this.SAddress;
                this.Affirm[2] = this.SAddress;
            }
        }

        public string PortName
        {
            get { return serialPort1.PortName; }
            set { serialPort1.PortName = value; }
        }
        public int BaudRate
        {
            get { return serialPort1.BaudRate; }
            set { serialPort1.BaudRate = value; }
        }

        public static byte Hex2Ten(string hex)
        {
            int ten = 0;
            for (int i = 0, j = hex.Length - 1; i < hex.Length; i++)
            {
                ten += HexChar2Value(hex.Substring(i, 1)) * ((int)Math.Pow(16, j));
                j--;
            }
            return Convert.ToByte(ten);
        }

        public static int HexChar2Value(string hexChar)
        {
            switch (hexChar)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    return Convert.ToInt32(hexChar);
                case "a":
                case "A":
                    return 10;
                case "b":
                case "B":
                    return 11;
                case "c":
                case "C":
                    return 12;
                case "d":
                case "D":
                    return 13;
                case "e":
                case "E":
                    return 14;
                case "f":
                case "F":
                    return 15;
                default:
                    return 0;
            }
        }

        public void AddressConvert(int num)
        {
            //该功能与一下代码一致，除了太大的数以外，太大的数，改功能代码没有问题，但是以下代码有问题
            //this.Rbyte[28] = Convert.ToByte(ByteAddress / 0x10000);
            //this.Rbyte[29] = Convert.ToByte((ByteAddress / 0x100) & 0xff);//0x100 ->256;
            //this.Rbyte[30] = Convert.ToByte(ByteAddress & 0xff);//地位，如320H，结果为20;
            this.mystring = Convert.ToString(num, 16);//转换成16进制字符串
            switch (this.mystring.Length)
            {
                case 1:
                    this.str1 = this.mystring;
                    this.fbyte4 = 0x00;
                    this.fbyte3 = 0x00;
                    this.fbyte2 = 0x00;
                    this.fbyte1 = Hex2Ten(this.str1);
                    break;
                case 2:
                    this.str2 = this.mystring;
                    this.fbyte4 = 0x00;
                    this.fbyte3 = 0x00;
                    this.fbyte2 = 0x00;
                    this.fbyte1 = Hex2Ten(this.str2);
                    break;
                case 3:
                    this.fbyte4 = 0x00;
                    this.fbyte3 = 0x00;
                    this.str1 = this.mystring.Substring(0, 1);
                    this.fbyte2 = Hex2Ten(this.str1);

                    this.str2 = this.mystring.Substring(1, 2);
                    this.fbyte1 = Hex2Ten(this.str2);
                    break;
                case 4:
                    this.fbyte4 = 0x00;
                    this.fbyte3 = 0x00;
                    this.str1 = this.mystring.Substring(0, 2);
                    this.fbyte2 = Hex2Ten(this.str1);

                    this.str2 = this.mystring.Substring(2, 2);
                    this.fbyte1 = Hex2Ten(this.str2);
                    break;
                case 5:
                    this.fbyte4 = 0x00;
                    this.str1 = this.mystring.Substring(0, 1);
                    this.fbyte3 = Hex2Ten(this.str1);

                    this.str2 = this.mystring.Substring(1, 2);
                    this.fbyte2 = Hex2Ten(this.str2);

                    this.str3 = this.mystring.Substring(3, 2);
                    this.fbyte1 = Hex2Ten(this.str3);
                    break;
                case 6:
                    this.fbyte4 = 0x00;
                    this.str1 = this.mystring.Substring(0, 2);
                    this.fbyte3 = Hex2Ten(this.str1);

                    this.str2 = this.mystring.Substring(2, 2);
                    this.fbyte2 = Hex2Ten(this.str2);

                    this.str3 = this.mystring.Substring(4, 2);
                    this.fbyte1 = Hex2Ten(this.str3);
                    break;
                case 7:
                    this.str1 = this.mystring.Substring(0, 1);
                    this.fbyte4 = Hex2Ten(this.str1);

                    this.str2 = this.mystring.Substring(1, 2);
                    this.fbyte3 = Hex2Ten(this.str2);

                    this.str3 = this.mystring.Substring(3, 2);
                    this.fbyte2 = Hex2Ten(this.str3);

                    this.str4 = this.mystring.Substring(5, 2);
                    this.fbyte1 = Hex2Ten(this.str4);
                    break;

                //int范围是 -2,147,483,648 到 2,147,483,647，所以转换成 2147483647转换成十六进制 7FFFFFFF,刚好最多能到达 8位的十六进制
                case 8:
                    this.str1 = this.mystring.Substring(0, 2);
                    this.fbyte4 = Hex2Ten(this.str1);

                    this.str2 = this.mystring.Substring(2, 2);
                    this.fbyte3 = Hex2Ten(this.str2);

                    this.str3 = this.mystring.Substring(4, 2);
                    this.fbyte2 = Hex2Ten(this.str3);

                    this.str4 = this.mystring.Substring(6, 2);
                    this.fbyte1 = Hex2Ten(this.str4);
                    break;
                default: break;
            }
        }


        //读取某个位的状态
        public bool Readbit(int ByteAddress, byte bitnumber)//如 I1.2，则 ByteAddress=1，bitnumber=2
        {
            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[28];

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }

            AddressConvert(ByteAddress * 8 + bitnumber);
            this.Rbyte[22] = 0x01;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            this.Rbyte[24] = 0x01;//Byte 24 以字节为单位，连续读取的字节数。如读2个VD则Byte24 =64 bit = 8*8bit,所以byte[24]=0x08
             //偏移量,byte 28,29,30 存储器偏移量指针 （存储器地址 *8 ）：
             //  如 VB100，存储器地址为 100，偏移量指针为 800，转换成 16 
            //进制就是 320H，则 Byte 28~29 这三个字节就是： 00 03 20

            // buffer[30] = Convert.ToByte(address & 0xff);//地位，如320H，结果为20
            //buffer[29] = Convert.ToByte((address / 0x100) & 0xff);//0x100 ->256
            //buffer[28] = Convert.ToByte(address / 0x10000);
            this.Rbyte[28] = this.fbyte3;
            this.Rbyte[29] = this.fbyte2;
            this.Rbyte[30] = this.fbyte1;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += this.Rbyte[i];//结果fcs为十进制，需要转换成十六进制
            }
            this.Rbyte[31] = fcs;

            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            this.Affirm[4] = fcs;

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
                while (this.serialPort1.BytesToRead < 28) {; }
                this.serialPort1.Read(Receives, 0, 28);
            }
            return Convert.ToBoolean(Receives[25]);
        }



        //读取一个字节存储单元的数据
        public byte Readbyte(int Address)
        {
            int i, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[28];

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }
            AddressConvert(Address * 8);
            this.Rbyte[22] = 0x02;
            this.Rbyte[24] = 0x01;
            this.Rbyte[28] = this.fbyte3;
            this.Rbyte[29] = this.fbyte2;
            this.Rbyte[30] = this.fbyte1;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += this.Rbyte[i];
            }
            this.Rbyte[31] = fcs;

            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            this.Affirm[4] = fcs;

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
                while (this.serialPort1.BytesToRead < 28) {; }
                this.serialPort1.Read(Receives, 0, 28);
            }
            this.serialPort1.Close();
            return Receives[25];
        }

        //读取一个字存储单元的数据
        public int ReadWord(int Address)
        {
            int i, Result = 0, Rece = 0;
            byte fcs;
            byte[] Receives = new byte[29];

            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.Open();
            }

            AddressConvert(Address * 8);
            this.Rbyte[22] = 0x04;
            this.Rbyte[24] = 0x01;
            this.Rbyte[28] = this.fbyte3;
            this.Rbyte[29] = this.fbyte2;
            this.Rbyte[30] = this.fbyte1;
            for (i = 4, fcs = 0; i < 31; i++)
            {
                fcs += this.Rbyte[i];
            }
            this.Rbyte[31] = fcs;

            for (i = 1, fcs = 0; i < 4; i++)
            {
                fcs += this.Affirm[i];
            }
            this.Affirm[4] = fcs;

            this.serialPort1.DiscardInBuffer();
            this.serialPort1.DiscardOutBuffer();

            this.serialPort1.Write(this.Rbyte, 0, 33);
            while (this.serialPort1.BytesToRead < 1) {; }
            //for (i = 0; i < 10000; i++)
            //    for (int j = 0; j < 1000; j++) ;
            Rece = this.serialPort1.ReadByte();
            if (Rece == 0xE5)
            {
                this.serialPort1.DiscardInBuffer();
                this.serialPort1.DiscardOutBuffer();

                this.serialPort1.Write(this.Affirm, 0, 6);
                //for (i = 0; i < 10000; i++)
                //    for (int j = 0; j < 1000; j++) ;
                while (this.serialPort1.BytesToRead < 29) {; }
                this.serialPort1.Read(Receives, 0, 29);
                Result = (int)(Receives[25] * 256 + Receives[26]);
            }
            this.serialPort1.Close();
            return Result;
        }

    }



}
