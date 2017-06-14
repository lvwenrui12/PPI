using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace testPPI.PPI
{
    public class Enums
    {

        public enum VarType
        {
            /// <summary>
            /// S7 Bit variable type (bool)
            /// </summary>
            Bit,

            /// <summary>
            /// S7 Byte variable type (8 bits)
            /// </summary>
            Byte,

            /// <summary>
            /// S7 Word variable type (16 bits, 2 bytes)
            /// </summary>
            Word,

            /// <summary>
            /// S7 DWord variable type (32 bits, 4 bytes)
            /// </summary>
            DWord,

            ///// <summary>
            ///// S7 Int variable type (16 bits, 2 bytes)   
            ///// </summary>
            //Int,

            ///// <summary>
            ///// DInt variable type (32 bits, 4 bytes)
            ///// </summary>
            //DInt,

            ///// <summary>
            ///// Real variable type (32 bits, 4 bytes)
            ///// </summary>
            //Real,

            ///// <summary>
            ///// String variable type (variable)
            ///// </summary>
            //String,

            ///// <summary>
            ///// Timer variable type
            ///// </summary>
            //Timer,

            ///// <summary>
            ///// Counter variable type
            ///// </summary>
            //Counter
        }



        public enum StorageType
        {
            //byte[27]

            //04:	S		05:	SM		06:	AI	
            //07:	AQ		1E:	C		81:	I	
            //82:	Q		83:	M		84:	V	
            //1F:	T		03:	系统存储区


            //数据存储区有：I,Q,V,M,S,SM,
            //L,局部存储器
            //T,C,AI,AQ,
            //AC,累加器
            //HC，高速计数器,Write Only
            //Timer Current Values  T00000-T65535  DWord, Long  Read/Write
            //Timer Status Bits  T00000-T65535  Boolean** Read Only


            //Counter Current Values  C00000-C65535  Word, Short  Read/Write


            //Counter Status Bits  C00000-C65535  Boolean**  Read Only
            //High Speed Counters  HC00000-HC65535  DWord, Long  Read Only


            //Analog Inputs  AI00000-AI65534***  Word, Short  Read Only


            //Analog Outputs  AQ00000-AQ65534***  Word, Short  Write Only

            S = 0x04,//顺序控制继电器区（S）

            C = 0x1E,//计数器存储器区（C）
            T = 0x1F,//时间继电器,只读

            I = 0x81,//输入
            Q = 0x82,//输出
            M = 0x83,//位存储器（M）区,PLC执行程序过程中，可能会用到一些标志位，这些标志位也需要用存储器来寄存
            V = 0x84,//变量存储器，存放全局变量、操作中的中间结果
            SM = 0x05,//特殊存储器区（SM）,SM0.0一直为“1”状态，SM0.1仅在执行用户程序的第一个扫描周期为“1”状态。SM0.4和SM0.5分别提供周期为1min和1s的时钟脉冲
            AQ = 0x07,//模拟量输出映像寄存器(AQ)
            AI = 0x06,//模拟量输入映像寄存器(AI)

        }
        public enum StorageTypeIsV
        {

            V = 0x01,
            Other = 0x00,

        }




    }
}
