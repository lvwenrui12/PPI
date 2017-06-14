using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testPPI.PPI
{
  public  class MeiColoudAddress
    {
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
    }
}
