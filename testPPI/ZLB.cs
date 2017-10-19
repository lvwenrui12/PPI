using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using testPPI.Common;
using testPPI.PPI;

namespace testPPI
{
    public partial class ZLB : Form
    {


        public ZLB()
        {
            InitializeComponent();
        }





        ZLB_PPIHelper ppiHelper = new ZLB_PPIHelper();


        // Create a TCP/IP socket.     
        private Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        private TcpClient tcpClient = new TcpClient();

        private void Form1_Load(object sender, EventArgs e)
        {
           
            foreach (string s in Enum.GetNames(typeof(Enums.StorageType)))
            {
                comStore.Items.Add(s);
            }

            foreach (string s in Enum.GetNames(typeof(Enums.VarType)))
            {
                comRead.Items.Add(s);
            }

            foreach (string s in Enum.GetNames(typeof(Enums.VarType)))
            {
                comWrite.Items.Add(s);
            }

        

            comStore.SelectedIndex = 0;

            comWrite.SelectedIndex = 0;

            comRead.SelectedIndex = 0;

            client.ReceiveTimeout = 1000;


        }




   

        private void button18_Click(object sender, EventArgs e)
        {
            //ZLB_PPIHelper.serialPort1.Close();
            //this.Close();
        }



        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            
            PPIReadWritePara readResult = new PPIReadWritePara();
            ZLB_PPIHelper zlbPPI = new ZLB_PPIHelper();

            if (tcpClient.Connected)
            {
                PPIReadWritePara para = new PPIReadWritePara();
                para.StorageType = (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text);
                para.TcpClient =tcpClient;
                para.ByteAddress = Int32.Parse(txtAddress.Text);
                para.Bitnumber = int.Parse(txtBit.Text);
                para.PlcAddress = int.Parse(txtPLC.Text);
                para.ComNum = int.Parse(txtComNum.Text);

                para.ReadCount = int.Parse(txtReadCount.Text) > 0 ? int.Parse(txtReadCount.Text) : 1;
                para.WriteValue = int.Parse(txtWriteValue.Text);
             if (para.StorageType == Enums.StorageType.T)
                {
                     readResult = zlbPPI.TReadDword(para);
                    
                    txtSendCmd.Text = ZLB_PPIHelper.sendCmd;

                }
                else
                {

                    #region switch
                    switch (comRead.Text)
                    {
                        case "Bit":

                            readResult = zlbPPI.Readbit(para);
                        
                            txtSendCmd.Text = ZLB_PPIHelper.sendCmd;
                            break;
                        case "Byte":
                            readResult = zlbPPI.Readbytes(para);
                            txtSendCmd.Text = (ZLB_PPIHelper.sendCmd);
                            break;
                        case "Word":
                            readResult = zlbPPI.ReadWords(para);
                            txtSendCmd.Text = (ZLB_PPIHelper.sendCmd);
                      
                            txtSendCmd.Text = ZLB_PPIHelper.sendCmd;
                            break;

                        case "DWord":

                            readResult = zlbPPI.ReadDoubleWord(para);
                            txtSendCmd.Text = (ZLB_PPIHelper.sendCmd);
                         
                            txtSendCmd.Text = ZLB_PPIHelper.sendCmd;
                            break;
                        default:

                            break;

                    }
                    #endregion

                }
            }
            else
            {
                MessageBox.Show("连接断开");
            }

            if (readResult.IsSuceess)
            {
                txtReceive1.Text = ZLB_PPIHelper.receiveByte;
                txtValue.Text = ByteHelper.ByteToString(readResult.ReadValue);
            }
            else
            {
                txtReceive1.Text = "读取失败";

            }

        }



        private void btnWrite_Click(object sender, EventArgs e)
        {

            int wValue = 0;

            bool flag = false;
        ZLB_PPIHelper zlbPPI = new ZLB_PPIHelper();
            if (txtWriteValue.Text.Length == 0)
            {
                MessageBox.Show("请输数值");
            }
            else
            {
                


                if ((Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text) == Enums.StorageType.C || (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text) == Enums.StorageType.T)
                {
                    MessageBox.Show("T，C寄存器等不能用写命令写入");
                    return;
                }

                if (client.Connected)
                {
                    PPIReadWritePara para = new PPIReadWritePara();
                    para.StorageType = (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text);
                    para.TcpClient = tcpClient;
                    para.ByteAddress = Int32.Parse(txtAddress.Text);
                    para.Bitnumber = int.Parse(txtBit.Text);
                    para.PlcAddress = int.Parse(txtPLC.Text);
                    para.ComNum = int.Parse(txtComNum.Text);

                    para.ReadCount = int.Parse(txtReadCount.Text) > 0 ? int.Parse(txtReadCount.Text) : 1;
                    para.WriteValue = int.Parse(txtWriteValue.Text);




                    if (int.TryParse(txtWriteValue.Text, out wValue))
                    {
                        if ((Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text) == Enums.StorageType.T)
                        {

                            if (zlbPPI.TwriteDWord(para))
                            {
                                txtSendCmd.Text = (ZLB_PPIHelper.sendCmd);
                                flag = true;
                            }
                           
                          
                            
                        }
                        else if ((Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text) == Enums.StorageType.C)
                        {
                            if (zlbPPI.CWriteWord(para))
                            {
                                flag = true;
                                txtSendCmd.Text = (ZLB_PPIHelper.sendCmd);
                            }
                        }
                        else
                        {
                            #region switch
                            switch (comWrite.Text)
                            {
                                case "Bit":
                                    
                                    if (ppiHelper.WriteBit(para))
                                        {
                                        flag = true;

                                        txtSendCmd.Text = (ZLB_PPIHelper.sendCmd);
                                    }
                                    break;
                                case "Byte":

                                    if (ppiHelper.Writebyte(para))
                                   
                                    {
                                        flag = true;
                                        txtSendCmd.Text = (ZLB_PPIHelper.sendCmd);
                                    }
                                    break;
                                case "Word":
                                    if (ppiHelper.WriteWord(para))
                                    {
                                        txtSendCmd.Text = (ZLB_PPIHelper.sendCmd);
                                        flag = true;
                                    }
                                    break;
                                case "DWord":
                                    if (ppiHelper.WriteDoubleWord(para))
                                   
                                    {
                                        flag = true;

                                        txtSendCmd.Text = (ZLB_PPIHelper.sendCmd);

                                    }
                                    break;

                                default:

                                    break;


                            }

                            #endregion
                        }
                        if (flag)
                        {
                            txtReceive1.Text = (ZLB_PPIHelper.receiveByte);
                            }
                        else
                        {
                            txtReceive1.Text = "写入失败";
                        }

                    }
                    else
                    {
                        MessageBox.Show("请输入正确数值");
                    }
                }
                else
                {
                    MessageBox.Show("连接失败");
                }

            }

        }

        private void txtAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtBit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtWriteValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            //if (ZLB_PPIHelper.PLCStop())
            //{
            //    btnPLCRun.Enabled = true;
            //    btnStop.Enabled = false;
            //}

        }

        private void btnPLCRun_Click(object sender, EventArgs e)
        {
            //if (ZLB_PPIHelper.PLCRun())
            //{
            //    btnStop.Enabled = true;
            //    btnPLCRun.Enabled = false;
            //}


        }

        private void comStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comStore.Text == "T")
            {
                comRead.Items.RemoveAt(1);
                comRead.Items.RemoveAt(1);

                comWrite.Items.RemoveAt(0);
                comWrite.Items.RemoveAt(1);
                comWrite.Items.RemoveAt(0);
                //  comWrite.Items.RemoveAt(2);
                comRead.SelectedIndex = 0;
                comWrite.SelectedIndex = 0;
            }
            else if (comStore.Text == "AQ")
            {
                comRead.Items.RemoveAt(3);
                comRead.Items.RemoveAt(0);
                comRead.Items.RemoveAt(0);

                comWrite.Items.RemoveAt(3);
                comWrite.Items.RemoveAt(0);
                comWrite.Items.RemoveAt(0);
                //  comWrite.Items.RemoveAt(2);
                comRead.SelectedIndex = 0;
                comWrite.SelectedIndex = 0;
            }

            else
            {
                comRead.Items.Clear();
                comWrite.Items.Clear();
                foreach (string s in Enum.GetNames(typeof(Enums.VarType)))
                {
                    comRead.Items.Add(s);
                }

                foreach (string s in Enum.GetNames(typeof(Enums.VarType)))
                {
                    comWrite.Items.Add(s);
                }
                comRead.SelectedIndex = 0;
                comWrite.SelectedIndex = 0;
            }
        }




        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                //IPAddress ipAddress = IPAddress.Parse(txtIP.Text);
                //IPEndPoint remoteEP = new IPEndPoint(ipAddress, int.Parse(txtPort.Text));

                //client.Connect(remoteEP);

               tcpClient.Connect(txtIP.Text, int.Parse(txtPort.Text));

                //ZLB_PPIHelper.tcpClient.Connect(IPAddress.Parse(txtIP.Text), int.Parse(txtPort.Text));
             
            

            }
            catch (SocketException ex)
            {
                MessageBox.Show("智联宝连接失败:" + ex.Message);
            }



        }



        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            int length = serialPort1.BytesToRead;
            byte[] data = new byte[length];
            if (length > 0)
            {
                //byte[] data = new byte[length];
                serialPort1.Read(data, 0, length);
                serialPort1.DiscardInBuffer();
                ////如果接收到的数据长度为1，值为E5H (229)，则为确认码，返回读写确认命令
                //if (data[0] == (byte)229 && data.Length == 1)
                //{
                //    //收到数据
                //    byte[] dataSend = new byte[6];
                //    dataSend[0] = 16;
                //    //站号
                //    dataSend[1] = 0x02;
                //    dataSend[2] = 0x00;
                //    dataSend[3] = 0x5C;
                //    dataSend[4] = (byte)(dataSend[3] + dataSend[1]);
                //    dataSend[5] = 0x16;
                //    serialPort1.DiscardOutBuffer();
                //    serialPort1.DiscardInBuffer();
                //    Thread.Sleep(100);
                //    serialPort1.Write(dataSend, 0, dataSend.Length);
                //    Thread.Sleep(100);
                //}
            }
            Invoke
           (new EventHandler
             (delegate
             {
                 this.txtReceive1.Text += BitConverter.ToString(data);
             }
             )
            );

        }

        private void btnClose_Click(object sender, EventArgs e)
        {

            client.Close();
        }

        private void txtPort_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtIP_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void txtComNum_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
