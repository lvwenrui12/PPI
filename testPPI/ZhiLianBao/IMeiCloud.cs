using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace testPPI
{
    public interface IMeiCloud
    {/// <summary>
        /// 读超时
        /// </summary>
        long ReadTimeOut { get; set; }

        /// <summary>
        /// 写超时
        /// </summary>
        long WriteTimeOut { get; set; }


        /// <summary>
        /// 连接智联宝
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="port">端口号</param>
        void Connect(IPAddress ipAddress, int port);

        /// <summary>
        /// 断开智联宝
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 从指定端口读取数据
        /// </summary>
        /// <param name="comPort">COM端口</param>
        /// <param name="buffer">类型 System.Byte 的数组，它是内存中用于存储从 指定COM口 读取的数据的位置。</param>
        /// <param name="offset">buffer 中开始将数据存储到的位置。</param>
        /// <param name="size"> 要从 指定COM口 中读取的字节数。</param>
        /// <returns></returns>
        int Read(int comPort, byte[] buffer, int offset, int size);

        /// <summary>
        /// 将数据写入指定端口
        /// </summary>
        /// <param name="comPort">COM端口</param>
        /// <param name="buffer">类型 System.Byte 的数组，该数组包含要写入 指定COM口 的数据。</param>
        /// <param name="offset">buffer 中开始写入数据的位置。</param>
        /// <param name="size">要写入 指定COM口 的字节数。</param>
        /// <returns></returns>
        int Write(int comPort, byte[] buffer, int offset, int size);
    }
}
