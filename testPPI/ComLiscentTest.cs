﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using testPPI.Common;

namespace testPPI
{
    public partial class ComLiscentTest : Form
    {
        private TcpClient tcpClient = new TcpClient();

        private ManualResetEvent ListenMaualReset = new ManualResetEvent(false);

        private Thread listenThread;

        private Thread ComsuThread;

        private Queue readQueue = new Queue(5);

        //WorkQueue<List<byte>> quere = new WorkQueue<List<byte>>();
        WorkQueue<byte[]> quere = new WorkQueue<byte[]>();


        public ComLiscentTest()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                //IPAddress ipAddress = IPAddress.Parse(txtIP.Text);
                //IPEndPoint remoteEP = new IPEndPoint(ipAddress, int.Parse(txtPort.Text));

                //client.Connect(remoteEP);

                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(txtIP.Text), int.Parse(txtPort.Text));

                tcpClient.Connect(txtIP.Text, int.Parse(txtPort.Text));

                //ZLB_PPIHelper.tcpClient.Connect(IPAddress.Parse(txtIP.Text), int.Parse(txtPort.Text));



            }
            catch (SocketException ex)
            {
                MessageBox.Show("智联宝连接失败:" + ex.Message);
            }


        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            ZlbDeviceDrive zlbDeviceDrive = new ZlbDeviceDrive();

            byte[] SendData = new byte[1024];

            if (string.IsNullOrEmpty(txtSend1.Text))
            {
                MessageBox.Show("请输入发送的数据");
                return;

            }
            SendData = ByteHelper.StringToByte(txtSend1.Text);


            zlbDeviceDrive.WriteCom(this.tabControl1.SelectedIndex + 1, SendData);

        }

        private void button1_Click(object sender, EventArgs e)
        {

            ListenMaualReset.Reset();

        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            listenThread.Start();

            ComsuThread.Start();

            ListenMaualReset.Set();

        }

        private void ComLiscentTest_FormClosed(object sender, FormClosedEventArgs e)
        {

            listenThread.Abort();

            ComsuThread.Abort();

        }

        private void ComsuData()
        {
            try
            {
                while (true)
                {



                    if (!quere.IsEmpty)
                    {
                       byte[]receiceData= this.quere.TryDequeueBox();
                        string ReceStr = ByteHelper.ByteToString(receiceData);


                        if (this.textBox1.InvokeRequired)//等待异步 
                        {
                            Action<string> actionDelegate = (x) => { this.textBox1.Text = x.ToString(); };
                            // 或者
                            // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                            this.textBox1.Invoke(actionDelegate, ReceStr);
                            
                        }
                        else
                        {
                            this.textBox1.Text = ReceStr;

                        }




                    }



                }


            }
            catch (Exception)
            {
                
                throw;
            }



        }

        private void ReadComData()
        {

            try
            {

                while (true)
                {
                    ListenMaualReset.WaitOne();

                    NetworkStream netStream = tcpClient.GetStream();
                    if (netStream.CanRead)
                    {
                        // Reads NetworkStream into a byte buffer.
                        byte[] bytes = new byte[tcpClient.ReceiveBufferSize];

                        // Read can return anything from 0 to numBytesToRead. 
                        // This method blocks until at least one byte is read.
                        netStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

                        // Returns the data received from the host to the console.
                        string returndata = Encoding.UTF8.GetString(bytes);

                        if (quere.MaxSize != quere.Count)
                        {
                            quere.EnqueueBox(bytes);
                        }
                        else
                        {
                            MessageBox.Show("接收数据处理不过来");
                           break;
                        }
                        
                    }
                
                    netStream.Close();
                    
                }

            }
            catch (Exception)
            {
                
                throw;
            }


        }


        private void ComLiscentTest_Load(object sender, EventArgs e)
        {

            tcpClient.ReceiveBufferSize = 1024;
            listenThread =new Thread(() => ReadComData());

            ComsuThread=new Thread(()=> ComsuData());
        }
    }
}