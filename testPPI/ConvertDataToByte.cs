using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace testPPI
{
    class ConvertDataToByte
    {


        #region 写
        /// <summary>
        /// 写一个Byte数组
        /// </summary>
        /// <param name="stationNO">站号</param>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte[] WriteVB(int stationNO, int address, int value)
        {
            byte[] data = new byte[38];

            //开始标记符
            data[0] = 0x68;
            data[1] = 0x20;
            data[2] = 0x20;
            data[3] = 0x68;

            //站号
            data[4] = (byte)stationNO;
            data[5] = 0x00;

            //功能码，写入
            data[6] = 0x7C;

            //
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x0E;
            data[15] = 0x00;
            data[16] = 0x05;
            data[17] = 0x05;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;
            data[21] = 0x10;

            //长度
            //01：1 Bit	
            //02：1 Byte 
            //04：1 Word 
            //06：Double Word
            data[22] = 0x02;



            data[23] = 0x00;

            //个数
            data[24] = 0x01;

            data[25] = 0x00;


            //存储器类型，01：V存储器  00：其它
            data[26] = 0x01;

            //存储器类型
            //04：S	05：SM 		06：AI		07：AQ		1E: C
            //81：I	82：Q		83：M		84：V		1F: T
            data[27] = 0x84;


            //地址，偏移量
            data[28] = 0x00;
            data[29] = Convert.ToByte(address * 8 / 256);
            data[30] = Convert.ToByte(address * 8 % 256);


            data[31] = 0x00;
            //如果写入的是位数据这一字节为03，其它则为04
            data[32] = 0x04;
            data[33] = 0x00;

            //位数
            //01: 1 Bit	08: 1 Byte	10H: 1 Word  20H: 1 Double Word   
            data[34] = 0x08;

            //值
            data[35] = (byte)value;

            //效验和
            int j = 0;
            for (int i = 4; i <= 35; i++)
                j = j + data[i];
            data[36] = Convert.ToByte(j % 256);


            data[37] = 0x16;
            return data;
        }



        /// <summary>
        /// 写一个Byte数组
        /// </summary>
        /// <param name="stationNO">站号</param>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte[] WriteQ(int stationNO, double address, int value)
        {
            byte[] data = new byte[38];

            //开始标记符
            data[0] = 0x68;
            data[1] = 0x20;
            data[2] = 0x20;
            data[3] = 0x68;

            //站号
            data[4] = (byte)stationNO;
            data[5] = 0x00;

            //功能码，写入
            data[6] = 0x7C;

            //
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x0E;
            data[15] = 0x00;
            data[16] = 0x05;
            data[17] = 0x05;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;
            data[21] = 0x10;

            //长度
            //01：1 Bit	
            //02：1 Byte 
            //04：1 Word 
            //06：Double Word
            data[22] = 0x02;



            data[23] = 0x00;

            //个数
            data[24] = 0x01;

            data[25] = 0x00;


            //存储器类型，01：V存储器  00：其它
            data[26] = 0x00;

            //存储器类型
            //04：S	05：SM 		06：AI		07：AQ		1E: C
            //81：I	82：Q		83：M		84：V		1F: T
            data[27] = 0x82;


            //地址，偏移量
            data[28] = 0x00;
            data[29] = Convert.ToByte(address * 8 / 256);
            data[30] = Convert.ToByte(address * 8 % 256);


            data[31] = 0x00;
            //如果写入的是位数据这一字节为03，其它则为04
            data[32] = 0x04;
            data[33] = 0x00;

            //位数
            //01: 1 Bit	08: 1 Byte	10H: 1 Word  20H: 1 Double Word   
            data[34] = 0x08;

            //值
            data[35] = (byte)value;

            //效验和
            int j = 0;
            for (int i = 4; i <= 35; i++)
                j = j + data[i];
            data[36] = Convert.ToByte(j % 256);


            data[37] = 0x16;
            return data;
        }

        /// <summary>
        /// 写一个VW数据
        /// </summary>
        /// <param name="paddr"></param>
        /// <param name="waddr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] WriteVW(int stationNO, int address, int value)
        {
            byte[] data = new byte[39];

            //开始
            data[0] = 0x68;
            data[1] = 0x21;
            data[2] = 0x21;
            data[3] = 0x68;

            //站号
            data[4] = (byte)stationNO;
            data[5] = 0x00;

            //功能码
            data[6] = 0x7C;


            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x0E;
            data[15] = 0x00;
            data[16] = 0x06;
            data[17] = 0x05;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;
            data[21] = 0x10;
            data[22] = 0x04;
            data[23] = 0x00;
            data[24] = 0x01;
            data[25] = 0x00;
            data[26] = 0x01;
            data[27] = 0x84;
            data[28] = 0x00;


            //地址
            data[29] = Convert.ToByte(address * 8 / 256);
            data[30] = Convert.ToByte(address * 8 % 256);

            data[31] = 0x00;
            data[32] = 0x04;
            data[33] = 0x00;
            data[34] = 0x10;

            //写值
            data[35] = Convert.ToByte(value / 256);
            data[36] = Convert.ToByte(value % 256);

            //效验码
            int j = 0;
            for (int i = 4; i <= 36; i++)
                j = j + data[i];
            data[37] = Convert.ToByte(j % 256);

            data[38] = 0x16;

            return data;
        }




        /// <summary>
        /// 写一个Byte数组 字
        /// </summary>
        /// <param name="stationNO">站号</param>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte[] WriteQ_w(int stationNO, int address, int value1, int value2)
        {
            byte[] data = new byte[39];

            //开始标记符
            data[0] = 0x68;
            data[1] = 0x21;
            data[2] = 0x21;
            data[3] = 0x68;

            //站号
            data[4] = (byte)stationNO;
            data[5] = 0x00;

            //功能码，写入
            data[6] = 0x7C;

            //
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x0E;
            data[15] = 0x00;
            data[16] = 0x05;
            data[17] = 0x05;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;
            data[21] = 0x10;

            //长度
            //01：1 Bit	
            //02：1 Byte 
            //04：1 Word 
            //06：Double Word
            data[22] = 0x04;



            data[23] = 0x00;

            //个数
            data[24] = 0x01;

            data[25] = 0x00;


            //存储器类型，01：V存储器  00：其它
            data[26] = 0x00;

            //存储器类型
            //04：S	05：SM 		06：AI		07：AQ		1E: C
            //81：I	82：Q		83：M		84：V		1F: T
            data[27] = 0x82;


            //地址，偏移量
            data[28] = 0x00;
            data[29] = Convert.ToByte(address * 8 / 256);
            data[30] = Convert.ToByte(address * 8 % 256);


            data[31] = 0x00;
            //如果写入的是位数据这一字节为03，其它则为04
            data[32] = 0x04;
            data[33] = 0x00;

            //位数
            //01: 1 Bit	08: 1 Byte	10H: 1 Word  20H: 1 Double Word   
            data[34] = 0x10;

            //值
            data[35] = Convert.ToByte(value1);
            data[36] = Convert.ToByte(value2);


            //效验和
            int j = 0;
            for (int i = 4; i <= 36; i++)
                j = j + data[i];
            data[37] = Convert.ToByte(j % 256);


            data[38] = 0x16;
            return data;
        }
        #endregion

        #region 读
        /// 发送数据函数定义
        /// </summary>
        /// <param name="paddr">站号</param>
        /// <param name="length">长度</param>
        /// <param name="saddr">地址</param>
        public static byte[] ReadI(byte stationNO, int length, double address)
        {
            //对于一次读取一个数据，读命令都是 33 个字节。前面的0~21 字节是相同的， 为 ：
            //68 1B 1B 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10


            //对于一次读多个数据的情况，前 21Byte 与上面相似，只
            //是长度 LE、LEr 及 Byte 14 不同：
            //Byte 14 数据块占位字节，它指明数据块占用的字节数。与数
            //据块数量有关，长度 = 4 + 数据块数 * 10,如：一条数据时为 4 +
            //     10 = 0E(H)。

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

            byte[] data = new byte[33];



            //68 1B 1B 68 02 00 6C 32 01 00 00 00 00 00 0E 00 00 04 01 12 0A 10 02 00 01 00 00 81 00 00 00 64 16

            data[0] = 0x68;
            data[1] = 0x1B;
            data[2] = 0x1B;
            data[3] = 0x68;
            data[4] = stationNO;
            data[5] = 0x00;

            data[6] = 0x6C;
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;

            data[12] = 0x00;

            data[13] = 0x00;
            data[14] = 0x0E;
            data[15] = 0x00;
            data[16] = 0x00;
            data[17] = 0x04;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;
            data[21] = 0x10;
            //以上0-21字节相同



            data[22] = 0x02;//Byte 22 为读取数据的长度,01： 1 Bit 02： 1 Byte 04： 1 Word 06： Double Word
            data[23] = 0x00;
            data[24] = Convert.ToByte(length);
            data[25] = 0x00;
            data[26] = 0x00;
            data[27] = 0x81;
            data[28] = 0x00;
            data[29] = Convert.ToByte(address * 8 / 256);
            data[30] = Convert.ToByte(address * 8 % 256);
            int j = 0;
            for (int i = 4; i <= 30; i++)
            {
                j = j + Convert.ToInt32(data[i]);
            }
            data[31] = Convert.ToByte(j % 256);//校验
            data[32] = 0x16;
            return data;
        }

        /// 发送数据函数定义
        /// </summary>
        /// <param name="paddr">站号</param>
        /// <param name="length">长度</param>
        /// <param name="saddr">地址</param>
        public static byte[] ReadVB(byte stationNO, int length, int address)
        {
            byte[] data = new byte[33];



            //68 1B 1B 68 02 00 7C 32 01 00 00 00 08 00 0E 00 00 04 01 12 0A 10 02 00 06 00 01 84 00 1F 40 E4 16 


            data[0] = 0x68;
            data[1] = 0x1B;
            data[2] = 0x1B;
            data[3] = 0x68;
            data[4] = stationNO;
            data[5] = 0x00;

            //以前是6C ，现在改7C
            data[6] = 0x7C;
            data[7] = 0x32;
            data[8] = 0x01;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;

            //这里是08
            data[12] = 0x08;

            data[13] = 0x00;
            data[14] = 0x0E;
            data[15] = 0x00;
            data[16] = 0x00;
            data[17] = 0x04;
            data[18] = 0x01;
            data[19] = 0x12;
            data[20] = 0x0A;
            data[21] = 0x10;
            data[22] = 0x02;
            data[23] = 0x00;
            data[24] = Convert.ToByte(length);
            data[25] = 0x00;
            data[26] = 0x01;
            data[27] = 0x84;
            data[28] = 0x00;
            data[29] = Convert.ToByte(address * 8 / 256);
            data[30] = Convert.ToByte(address * 8 % 256);
            int j = 0;
            for (int i = 4; i <= 30; i++)
            {
                j = j + Convert.ToInt32(data[i]);
            }
            data[31] = Convert.ToByte(j % 256);
            data[32] = 0x16;
            return data;
        }

        #endregion

      


        public static byte[] ClosePLC()
        {
            byte[] data = new byte[39];
            //68 1D 1D 68 02 00 6C 32 01 00 00 00 00 00 10 00 00 29 00 00 00 00 00 09 50 5F 50 52 4F 47 52 41 4D AA 16
            return data;
        }


        #region 自己的代码
        //---------- 以下为自己添加的测试代码------------
        public static byte[] MergerArray(byte[] First, byte[] Second)
        {
            byte[] result = new byte[First.Length + Second.Length];
            First.CopyTo(result, 0);
            Second.CopyTo(result, First.Length);
            return result;
        }

    
        #endregion


    }
}
