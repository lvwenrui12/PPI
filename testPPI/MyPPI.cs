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

            if (comportName.Items.Count>0)
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
            bool flag = false;

            byte[] readValues = new byte[] { };

            if ((Enums.StorageType) Enum.Parse(typeof(Enums.StorageType), comStore.Text) == Enums.StorageType.T)
            {

                if (PPIHelper.TReadDword(Int32.Parse(txtAddress.Text),out readValues ))
                {
                    flag = true;
                }
                txtSendCmd.Text = ByteToString(PPIHelper.TReadByte);

            }
            else
            {
                #region switch
                switch (comRead.Text)
                {
                    case "Bit":



                        if (PPIHelper.Readbit(Int32.Parse(txtAddress.Text), Int32.Parse(txtBit.Text),

                            (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text),
                            out readValues))
                        {
                            flag = true;


                        }
                        txtSendCmd.Text = ByteToString(PPIHelper.Rbyte);

                        break;
                    case "Byte":
                        if (txtReadCount.Text.Length == 0)
                        {
                            if (
                             PPIHelper.Readbytes(Int32.Parse(txtAddress.Text),

                            (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text),
                            out readValues))
                            {
                                flag = true;

                            }
                            ;
                        }
                        else
                        {
                            if (PPIHelper.Readbytes(Int32.Parse(txtAddress.Text),

                           (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text),
                         out readValues, Int32.Parse(txtReadCount.Text)))
                            {
                                flag = true;

                            }
                            ;
                        }
                        txtSendCmd.Text = ByteToString(PPIHelper.Rbyte);
                        break;


                    case "Word":
                        if (txtReadCount.Text.Length == 0)
                        {
                            if (PPIHelper.ReadWords(Int32.Parse(txtAddress.Text),

                           (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text),
                          out readValues))
                            {
                                flag = true;
                            }
                            ;
                        }
                        else
                        {
                            if (PPIHelper.ReadWords(Int32.Parse(txtAddress.Text),

                          (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text),
                          out readValues, Int32.Parse(txtReadCount.Text)))
                            {
                                flag = true;
                            }

                        }

                        txtSendCmd.Text = ByteToString(PPIHelper.Rbyte);
                        break;

                    case "DWord":

                        if (PPIHelper.ReadDWord(Int32.Parse(txtAddress.Text),

                       (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text),
                       out readValues))
                        {
                            flag = true;
                        }
                        txtSendCmd.Text = ByteToString(PPIHelper.Rbyte);



                        break;

                    default:

                        break;


                }
                #endregion
            }




            if (flag)
            {

              

                txtReceive.Text = ByteToString(PPIHelper.receiveByte);

                txtValue.Text = ByteToString(readValues);
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
            bool flag = false;
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

                if (int.TryParse(txtWriteValue.Text, out wValue))
                {
                    if ((Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text)==Enums.StorageType.T)
                    {
                        if (PPIHelper.TwriteDWord(Int32.Parse(txtAddress.Text), wValue))
                        {
                            flag = true;
                            txtSendCmd.Text = ByteToString(PPIHelper.TWritebyte); 

                        }
                    }
                    else if ((Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text) == Enums.StorageType.C)
                    {
                        if (PPIHelper.CWriteWord(Int32.Parse(txtAddress.Text), wValue))
                        {
                            flag = true;
                            txtSendCmd.Text = ByteToString(PPIHelper.CwriteWordByte);

                        }
                    }
                    else
                    {
                        #region switch
                        switch (comWrite.Text)
                        {
                            case "Bit":

                                if (PPIHelper.WriteBit(Int32.Parse(txtAddress.Text), Convert.ToByte(txtBit.Text),

                                    (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text),
                                    wValue))
                                {
                                    flag = true;

                                    txtSendCmd.Text = ByteToString(PPIHelper.Wbit);
                                }


                                break;
                            case "Byte":

                                if (PPIHelper.Writebyte(Int32.Parse(txtAddress.Text),

                                   (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text),
                                wValue))
                                {
                                    flag = true;
                                    txtSendCmd.Text = ByteToString(PPIHelper.Wbyte);
                                }

                                break;


                            case "Word":

                                if (PPIHelper.WriteWord(Int32.Parse(txtAddress.Text),

                               (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text),
                             wValue))
                                {

                                    txtSendCmd.Text = ByteToString(PPIHelper.Wword);
                                    flag = true;
                                }


                                break;
                            case "DWord":

                                if (PPIHelper.WriteDWord(Int32.Parse(txtAddress.Text),

                               (Enums.StorageType)Enum.Parse(typeof(Enums.StorageType), comStore.Text),
                               wValue))
                                {
                                    flag = true;

                                    txtSendCmd.Text = ByteToString(PPIHelper.WDword);

                                }
                                break;

                            default:

                                break;


                        }

                        #endregion
                    }





                    if (flag)
                    {
                        txtReceive.Text = ByteToString(PPIHelper.receiveByte);

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
            if (PPIHelper.PLCStop())
            {
                btnPLCRun.Enabled = true;
                btnStop.Enabled = false;
            }

        }

        private void btnPLCRun_Click(object sender, EventArgs e)
        {
            if (PPIHelper.PLCRun())
            {
                btnStop.Enabled = true;
                btnPLCRun.Enabled = false;
            }


        }

        private void comStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comStore.Text=="T")
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
