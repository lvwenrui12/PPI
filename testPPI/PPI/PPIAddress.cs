using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testPPI.Common;

namespace testPPI.PPI
{
  public  class PPIAddress
    {

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


        // PLC会返回 E5，意思：已经收到
        //那么这时上位机再次发送指令 10 02 00 5C 5E 16 意思：请执行命令。

        //再次发送指令 10 02 00 5C 5E 16 意思：请执行命令,其中，SD为起始符，始终为10H；DA为读指令的目的地址；SA源地址；FCS校验和，等于DA+SA+FC的数据和；ED始终为16H
        //SD DA  SA FC  FCS  ED
        //10 02	 00	5C	5E	 16
        public byte[] Affirm = { 0x10, 0x02, 0x00, 0x5c, 0x5e, 0x16 };

        public  byte confirm = 0xE5;
        

        #region 目标地址 源地址

        private byte _DAddress = 0x0c;

        public byte DAddress
        {
            get { return _DAddress; }
            set
            {
                _DAddress = value;
                Rbyte[4] = _DAddress;
                Wbyte[4] = _DAddress;
                Wbit[4] = _DAddress;
                Wword[4] = _DAddress;
                Affirm[1] = _DAddress;

                WriteReceivesCheck[5] = _DAddress;

                int i;
                byte fcs;
                for (i = 4, fcs = 0; i < WriteReceivesCheck.Length-2; i++)
                {
                    fcs += WriteReceivesCheck[i];
                }
                string str = ByteHelper.ByteToString(WriteReceivesCheck);

                int tt = Convert.ToInt32(fcs) % 256; //添加的代码 mod 256

                WriteReceivesCheck[WriteReceivesCheck.Length-2] = Convert.ToByte(tt);

               
              
                for (i = 1, fcs = 0; i < 4; i++)
                {
                    fcs += Affirm[i];
                }
                tt = Convert.ToInt32(fcs)%256; //添加的代码 mod 256
                Affirm[4] = Convert.ToByte(tt);
            }
        }//PLC地址，DA，默认情况下，PLC的地址为02H


        private byte _SAddress = 0x00;
        public byte SAddress
        {
            get { return _SAddress; }
            set
            {
                _SAddress = value;
                Rbyte[5] = _SAddress;
                Wbyte[5] = _SAddress;

                Wbit[5] = _SAddress;
                Wword[5] = _SAddress;
                Affirm[2] = _SAddress;

                int i;
                byte fcs;
                for (i = 1, fcs = 0; i < 4; i++)
                {
                    fcs += Affirm[i];
                }
                int tt = Convert.ToInt32(fcs) % 256; //添加的代码 mod 256
                Affirm[4] = Convert.ToByte(tt);
            }
        } //上位机地址 SA，//SA源地址，默认情况下，PC机地址为00H，HMI设备的地址为01H

        #endregion

        

        public byte[] Rbyte =
       {
            0x68, 0x1B, 0x1B, //LE为从DA到DU的数据长度，以字节计，如读一个数据时始终为1BH
            0x68, 0x02, 0x00, 0x6C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x04, 0x01, 0x12, 0x0A,
            0x10, 0x02, //byte[22]02代表读取数据长度为byte，即8个bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, //byte[24]读取的个数
            0x00, 0x01, //存储类型01:V,其他的0
            0x84, 0x00, 0x03, 0x20, 0x8b, 0x16
        };


        public  byte[] TReadByte =
           {
                0x68, 0x1b, 0x1b, 0x68, 0x02, 0x00, 0x6c, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0e,
                0x00, 0x00, 0x04, 0x01, 0x12, 0x0a, 0x10, 0x1f, 0x00, 0x01, 0x00, 0x00, 0x1f, 0x00, 0x00, 0xff, 0x1e,
                0x16
            };


        #region 预定义写字符串

        public  byte[] Wbit = { 0x68, 0x20, 0x20, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x05, 0x05, 0x01, 0x12, 0x0A, 0x10,//前面0-21位
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


        public  byte[] Wbyte = { 0x68, 0x20, 0x20, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x05, 0x05, 0x01, 0x12, 0x0A, 0x10,//前面0-21位
            0x02,////byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, 0x00, 0x01, 0x84,//存储器类型
            0x00, 0x03, 0x20,////byte[28],byte[29],byte[30]偏移量
            0x00, 0x04,//byte[32]数据形式03 bit 其他04
            0x00, 0x08,//数据位数01 bit 10:word 08 byte 20: Double Word
            0x10,//value
            0xbd, 0x16 };


        public  byte[] Wword = { 0x68, 0x21, 0x21, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x06, 0x05, 0x01, 0x12, 0x0A, 0x10,
            0x04,//byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, 0x00, 0x01, 0x84,//存储器类型
            0x00, 0x03, 0x20,//byte[28],byte[29],byte[30]偏移量
            0x00, 0x04,//byte[32]数据形式03 bit 其他04
            0x00, 0x10,//数据位数01 bit 10:word 08 byte 20: Double Word
            0xab, 0xcd,//value
            0x30, 0x16 };

        public  byte[] WDword = { 0x68, 0x23, 0x23, 0x68, 0x02, 0x00, 0x7C, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x08, 0x05, 0x01, 0x12, 0x0A, 0x10,
            0x06,//byte[22],bit, 01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            0x00, 0x01, 0x00, 0x01, 0x84,//存储器类型
            0x00, 0x03, 0x20,//byte[28],byte[29],byte[30]偏移量
            0x00, 0x04,//byte[32]数据形式03 bit 其他04
            0x00, 0x20,//数据位数01 bit 10:word 08 byte 20: Double Word
            0xab, 0xcd,//value
            0xab, 0xcd,//value
            0x30, 0x16 };

        #endregion


       public byte[] WriteReceivesCheck = { 0x68, 0x12, 0x12, 0x68, 0x00, 0x02, 0x08, 0x32, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x05, 0x01, 0xFF, 0x47, 0x16 };

      

        public  byte[] TWritebyte = { 0x68, 0x24, 0x24, 0x68, 0x02, 0x00, 0x6c, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0e, 0x00, 0x09, 0x05, 0x01, 0x12, 0x0a, 0x10, 0x1f, 0x00, 0x01, 0x00, 0x00, 0x1f, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x28, 0x00, 0x00, 0x00, 0x01, 0x01, 0x57, 0x16 };

        public  byte[] CwriteWordByte =
              {
                0x68, 0x22, 0x22, 0x68, 0x02, 0x00, 0x6c, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x0e, 0x00, 0x07, 0x05, 0x01, 0x12, 0x0a, 0x10, 0x1e, 0x00, 0x01, 0x00, 0x00, 0x1e, 0x00, 0x00, 0x00,
                0x00, 0x04, 0x00, 0x18, 0x00, 0x01, 0x01, 0x43, 0x16
            };

    

    }
}
