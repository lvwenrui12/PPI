using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace testPPI
{
    /// <summary>
    /// 设备走modbus协议驱动
    /// </summary>
    public class ZlbDeviceDrive
    {

        public delegate void ReadCallback(string tagId, string value);

        private string _ip;
        private int _port;
        private TcpClient tcpClient;
        public static byte[] MeiColoudReadBytes = { 0xAA, 0x00, 0x01, 0x44, 0x03 };
        private IPEndPoint _ipEndPoint;

        private ManualResetEvent readManualResetEvent=new ManualResetEvent(false);

        private StringBuilder  ReadBuffer=new StringBuilder(1024);

        //public  ZlbDeviceDrive(string ip, int port)
        //{
        //    this._ip = ip;
        //    this._port = port;

        //    this._ipEndPoint  = new IPEndPoint(IPAddress.Parse(this._ip), this._port);
        //    this.tcpClient.Connect(this._ipEndPoint);
        //}


        public ZlbDeviceDrive(TcpClient tcpClient)
        {

            this.tcpClient = tcpClient;

        }


        #region 监听socket端口

        private void ListenSocke()
        {

            Thread thrListener = new Thread(new ThreadStart(Listen));
            thrListener.Start();




        }

        private void Listen()
        {
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(this._ipEndPoint);

            //不断监听端口
            while (true)
            {
                listener.Listen(0);
                Socket socket = listener.Accept();
                NetworkStream ntwStream = new NetworkStream(socket);
                StreamReader ReaderStrm = new StreamReader(ntwStream);
                //Invoke(new PrintRecvMssgDelegate(PrintRecvMssg),
                //    new object[] { strmReader.ReadToEnd() });
                string readStr = ReaderStrm.ReadToEnd();

                lock (ReadBuffer)
                {
                    this.ReadBuffer.Append(readStr);


                }
                socket.Close();

                //StringBuilder 
            }

            //程序的listener一直不关闭
            //listener.Close();
        }










        #endregion


        #region 写串口

        public void WriteCom(int ComNum, byte[] Content)
        {
            byte[] SendData = MergerArray(new []{ Convert.ToByte(ComNum)}, Content);

            SendData = GetSendData(SendData);

            if (tcpClient!=null)
            {
                if (tcpClient.Connected)
                {
                    tcpClient.Client.Send(SendData);
                }
            }
            
        

            
        }



        #endregion
       
        
        #region 发送字符串拼接
        public static byte[] GetSendData(byte[] data)
       
        {

            byte[] SendData = new byte[] { };
            MeiColoudReadBytes[1] = Convert.ToByte(data.Length / 256);
            MeiColoudReadBytes[2] = Convert.ToByte(data.Length % 256);

            SendData = MergerArray(MeiColoudReadBytes, data);

            byte CheckSum = 0;

            SendData = MergerArray(SendData, new byte[] { CheckSum });

            CheckSum = GetCheckSum(SendData);

            SendData[SendData.Length - 1] = CheckSum;

            return SendData;

        } 
        #endregion

        #region 校验
        /// <summary>
        /// 根据指令获取对应的校验和，采用异或校验的方式进行
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static byte GetCheckSum(byte[] bytes)
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

        #region byte数组合并
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
