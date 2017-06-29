using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading;
using testPPI.PPI;

namespace testPPI
{
    public partial class MyPPI : Form
    {


        public MyPPI()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            //自动获取COM口名称
            foreach (string com in System.IO.Ports.SerialPort.GetPortNames())
            {
                this.comportName.Items.Add(com);
            }

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

            if (comportName.Items.Count > 0)
            {
                comportName.SelectedIndex = 0;
            }

            comStore.SelectedIndex = 0;

            comWrite.SelectedIndex = 0;

            comRead.SelectedIndex = 0;

            
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (PPIHelper.serialPort1.IsOpen)
                {
                    PPIHelper.serialPort1.Close();
                }
                else
                {
                    // 设置端口参数
                    PPIHelper.serialPort1.BaudRate = int.Parse(this.comboBox2.Text);
                    PPIHelper.serialPort1.DataBits = int.Parse(this.comboBox3.Text);
                    PPIHelper.serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), this.comboBox4.Text);
                    PPIHelper.serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), this.comboBox5.Text);
                    PPIHelper.serialPort1.PortName = this.comportName.Text;
                    //comport.Encoding = Encoding.ASCII;
                    PPIHelper.serialPort1.ReadTimeout = 500;
                    //打开端口
                    PPIHelper.serialPort1.Open();
                }
                this.groupBox1.Enabled = !PPIHelper.serialPort1.IsOpen;
                //txtsend.Enabled = btnsend.Enabled = comport.IsOpen;

                if (PPIHelper.serialPort1.IsOpen)
                {
                    this.button1.Text = "关闭端口";
                }
                else
                {
                    this.button1.Text = "打开端口";
                }
                //if (this.serialPort1.IsOpen) txtsend.Focus();
            }
            catch (Exception er)
            {
                MessageBox.Show("端口打开失败！" + er.Message, "提示");
            }

        }

        private void button18_Click(object sender, EventArgs e)
        {
            PPIHelper.serialPort1.Close();
            this.Close();
        }



        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnRead_Click(object sender, EventArgs e)
        {


            if (!PPIHelper.serialPort1.IsOpen)
            {
                MessageBox.Show("请设置串口参数后打开", "警告");
                return;
            }
       
         

            PPIReadWritePara para=new PPIReadWritePara();
            PPIReadWritePara readResult=new PPIReadWritePara();

            para.ByteAddress = int.Parse(txtAddress.Text);
            para.Bitnumber = Int32.Parse(txtBit.Text);
            para.StorageType= (Enums.StorageType)
            Enum.Parse(typeof(Enums.StorageType), comStore.Text);
            para.PlcAddress = int.Parse(txtPLC.Text);


            if (para.StorageType == Enums.StorageType.T)
            {

                readResult= PPIHelper.TReadDword(para);
             
                txtSendCmd.Text = (PPIHelper.sendCmd);

            }
            else
            {
                #region switch
                switch (comRead.Text)
                {
                    case "Bit":

                        readResult = PPIHelper.Readbit(para);
                        
                        txtSendCmd.Text = (PPIHelper.sendCmd);

                        break;
                    case "Byte":

                        readResult = PPIHelper.Readbytes(para);
                        txtSendCmd.Text = (PPIHelper.sendCmd);
                        break;
                        
                    case "Word":
                       
                readResult = PPIHelper.ReadWords(para);
                txtSendCmd.Text = (PPIHelper.sendCmd);
                        break;

                    case "DWord":
                        readResult = PPIHelper.ReadDWord(para);

                        txtSendCmd.Text = (PPIHelper.sendCmd);
                        
                        break;

                    default:

                        break;


                }
                #endregion
            }
            if (readResult.IsSuceess)
            {
                
                txtReceive.Text = (PPIHelper.receiveByte);

                txtValue.Text = ByteToString(readResult.ReadValue);
            }
            else
            {

                txtReceive.Text = "读取失败";
                
            }

        }

        public static string ByteToString(byte[] bytes)

        {

            StringBuilder strBuilder = new StringBuilder();

            foreach (byte bt in bytes)

            {

                strBuilder.AppendFormat("{0:X2} ", bt);

            }

            return strBuilder.ToString();

        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
        
            if (!PPIHelper.serialPort1.IsOpen)
            {
                MessageBox.Show("请设置串口参数后打开", "警告");
                return;
            }

            int wValue = 0;
        
            if (txtWriteValue.Text.Length == 0)
            {
                MessageBox.Show("请输数值");
            }
            else
            {
                //if ((Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text)==Enums.StorageType.C|| (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text)==Enums.StorageType.T)
                //{
                //    MessageBox.Show("T，C寄存器等不能用写命令写入");
                //    return;

                //}

                PPIReadWritePara para = new PPIReadWritePara();
                bool flag = false;

                para.ByteAddress = int.Parse(txtAddress.Text);
                para.Bitnumber = Int32.Parse(txtBit.Text);
                para.StorageType = (Enums.StorageType)
                Enum.Parse(typeof(Enums.StorageType), comStore.Text);
                para.PlcAddress = int.Parse(txtPLC.Text);
             


                if (int.TryParse(txtWriteValue.Text, out wValue))
                {
                    para.WriteValue = int.Parse(txtWriteValue.Text);
                    if ((Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text) == Enums.StorageType.T)
                    {
                        if (PPIHelper.TwriteDWord(para))
                        {
                          txtSendCmd.Text = (PPIHelper.sendCmd);
                            flag = true;
                        }
                    }
                    else if ((Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text) == Enums.StorageType.C)
                    {
                        if (PPIHelper.CWriteWord(para))
                        {
                         
                            txtSendCmd.Text = PPIHelper.sendCmd;
                            flag = true;
                        }
                    }
                    else
                    {
                        #region switch
                        switch (comWrite.Text)
                        {
                            case "Bit":

                                if (PPIHelper.WriteBit(para))
                                {
                                  txtSendCmd.Text = (PPIHelper.sendCmd);
                                    flag = true;
                                }
                                break;
                            case "Byte":

                                if (PPIHelper.Writebyte(para))
                                {
                                  
                                    txtSendCmd.Text = PPIHelper.sendCmd;
                                    flag = true;
                                }
                                break;
                                case "Word":

                                if (PPIHelper.WriteWord(para))
                                {

                                    txtSendCmd.Text = (PPIHelper.sendCmd);
                                    flag = true;

                                }


                                break;
                            case "DWord":

                                if (PPIHelper.WriteDWord(para))
                                {
                                  
                                    txtSendCmd.Text = PPIHelper.sendCmd;
                                    flag = true;

                                }
                                break;

                            default:

                                break;


                        }

                        #endregion
                    }





                    if (flag)
                    {
                        txtReceive.Text = PPIHelper.receiveByte;

                    }
                    else
                    {
                        txtReceive.Text = "写入失败";
                    }

                }
                else
                {
                    MessageBox.Show("请输入正确数值");
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
            //plc地址不是 02，验证失败

            PPIReadWritePara para = new PPIReadWritePara();

            para.PlcAddress = int.Parse(txtPLC.Text);



            if (PPIHelper.PLCStop(para))
            {
                btnPLCRun.Enabled = true;
                btnStop.Enabled = false;
            }

        }

        private void btnPLCRun_Click(object sender, EventArgs e)
        {
            //plc地址不是 02，验证失败

            PPIReadWritePara para = new PPIReadWritePara();
     
            para.PlcAddress = int.Parse(txtPLC.Text);

            if (PPIHelper.PLCRun(para))
            {
                btnStop.Enabled = true;
                btnPLCRun.Enabled = false;
            }


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


    }
}
