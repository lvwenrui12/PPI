namespace testPPI
{
    partial class ZLB
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button18 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.comStore = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtReceive1 = new System.Windows.Forms.TextBox();
            this.txtSendCmd = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtBit = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtReadCount = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.comRead = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.comWrite = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtWriteValue = new System.Windows.Forms.TextBox();
            this.btnRead = new System.Windows.Forms.Button();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPLCRun = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtComNum = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.txtPLC = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtReceive2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtReceive3 = new System.Windows.Forms.TextBox();
            this.txtReceive4 = new System.Windows.Forms.TextBox();
            this.txtReceive5 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(629, 17);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(75, 23);
            this.button18.TabIndex = 1;
            this.button18.Text = "关闭程序";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "寄存器";
            // 
            // comStore
            // 
            this.comStore.FormattingEnabled = true;
            this.comStore.Location = new System.Drawing.Point(64, 17);
            this.comStore.Name = "comStore";
            this.comStore.Size = new System.Drawing.Size(77, 20);
            this.comStore.TabIndex = 3;
            this.comStore.Text = "Q";
            this.comStore.SelectedIndexChanged += new System.EventHandler(this.comStore_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "地址";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(64, 54);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(77, 21);
            this.txtAddress.TabIndex = 5;
            this.txtAddress.Text = "0";
            this.txtAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAddress_KeyPress);
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(146, 125);
            this.txtValue.Multiline = true;
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(173, 26);
            this.txtValue.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(419, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 9;
            this.label9.Text = "接收结果";
            // 
            // txtReceive1
            // 
            this.txtReceive1.Location = new System.Drawing.Point(477, 96);
            this.txtReceive1.Multiline = true;
            this.txtReceive1.Name = "txtReceive1";
            this.txtReceive1.Size = new System.Drawing.Size(323, 55);
            this.txtReceive1.TabIndex = 10;
            // 
            // txtSendCmd
            // 
            this.txtSendCmd.Location = new System.Drawing.Point(14, 414);
            this.txtSendCmd.Multiline = true;
            this.txtSendCmd.Name = "txtSendCmd";
            this.txtSendCmd.Size = new System.Drawing.Size(399, 49);
            this.txtSendCmd.TabIndex = 11;
            this.txtSendCmd.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 380);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 12;
            this.label10.Text = "发送的指令";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(155, 60);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 12);
            this.label11.TabIndex = 18;
            this.label11.Text = "位";
            // 
            // txtBit
            // 
            this.txtBit.Location = new System.Drawing.Point(242, 51);
            this.txtBit.Name = "txtBit";
            this.txtBit.Size = new System.Drawing.Size(77, 21);
            this.txtBit.TabIndex = 19;
            this.txtBit.Text = "0";
            this.txtBit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBit_KeyPress);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 97);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(77, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "连续读取个数";
            // 
            // txtReadCount
            // 
            this.txtReadCount.Location = new System.Drawing.Point(96, 93);
            this.txtReadCount.Name = "txtReadCount";
            this.txtReadCount.Size = new System.Drawing.Size(77, 21);
            this.txtReadCount.TabIndex = 20;
            this.txtReadCount.Text = "1";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(179, 97);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 21;
            this.label13.Text = "读取方式";
            // 
            // comRead
            // 
            this.comRead.FormattingEnabled = true;
            this.comRead.Location = new System.Drawing.Point(242, 89);
            this.comRead.Name = "comRead";
            this.comRead.Size = new System.Drawing.Size(77, 20);
            this.comRead.TabIndex = 22;
            this.comRead.Text = "bit";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(25, 31);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 12);
            this.label14.TabIndex = 23;
            this.label14.Text = "写方式";
            // 
            // comWrite
            // 
            this.comWrite.FormattingEnabled = true;
            this.comWrite.Location = new System.Drawing.Point(85, 28);
            this.comWrite.Name = "comWrite";
            this.comWrite.Size = new System.Drawing.Size(77, 20);
            this.comWrite.TabIndex = 24;
            this.comWrite.Text = "bit";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(17, 137);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(101, 12);
            this.label15.TabIndex = 25;
            this.label15.Text = "读取的值十六进制";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(25, 69);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(41, 12);
            this.label16.TabIndex = 27;
            this.label16.Text = "写的值";
            // 
            // txtWriteValue
            // 
            this.txtWriteValue.Location = new System.Drawing.Point(85, 60);
            this.txtWriteValue.Name = "txtWriteValue";
            this.txtWriteValue.Size = new System.Drawing.Size(205, 21);
            this.txtWriteValue.TabIndex = 28;
            this.txtWriteValue.Text = "0";
            this.txtWriteValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtWriteValue_KeyPress);
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(325, 17);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(73, 27);
            this.btnRead.TabIndex = 29;
            this.btnRead.Text = "读";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(306, 20);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(73, 27);
            this.btnWrite.TabIndex = 30;
            this.btnWrite.Text = "写";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(527, 17);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 31;
            this.btnStop.Text = "PLC Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPLCRun
            // 
            this.btnPLCRun.Location = new System.Drawing.Point(433, 17);
            this.btnPLCRun.Name = "btnPLCRun";
            this.btnPLCRun.Size = new System.Drawing.Size(75, 23);
            this.btnPLCRun.TabIndex = 32;
            this.btnPLCRun.Text = "PLC Run";
            this.btnPLCRun.UseVisualStyleBackColor = true;
            this.btnPLCRun.Click += new System.EventHandler(this.btnPLCRun_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(210, 53);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(79, 23);
            this.button2.TabIndex = 33;
            this.button2.Text = "连接ZLB";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(69, 23);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(122, 21);
            this.txtIP.TabIndex = 35;
            this.txtIP.Text = "10.32.0.165";
            this.txtIP.TextChanged += new System.EventHandler(this.txtIP_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 34;
            this.label8.Text = "IP地址";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(69, 53);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(122, 21);
            this.txtPort.TabIndex = 37;
            this.txtPort.Text = "5000";
            this.txtPort.TextChanged += new System.EventHandler(this.txtPort_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 53);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(41, 12);
            this.label17.TabIndex = 36;
            this.label17.Text = "端口号";
            this.label17.Click += new System.EventHandler(this.label17_Click);
            // 
            // txtComNum
            // 
            this.txtComNum.Location = new System.Drawing.Point(271, 20);
            this.txtComNum.Name = "txtComNum";
            this.txtComNum.Size = new System.Drawing.Size(122, 21);
            this.txtComNum.TabIndex = 39;
            this.txtComNum.Text = "5";
            this.txtComNum.TextChanged += new System.EventHandler(this.txtComNum_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(197, 28);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(53, 12);
            this.label18.TabIndex = 38;
            this.label18.Text = "串口口号";
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // txtPLC
            // 
            this.txtPLC.Location = new System.Drawing.Point(242, 17);
            this.txtPLC.Name = "txtPLC";
            this.txtPLC.Size = new System.Drawing.Size(77, 21);
            this.txtPLC.TabIndex = 41;
            this.txtPLC.Text = "12";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(147, 21);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(89, 12);
            this.label19.TabIndex = 40;
            this.label19.Text = "PLC地址 十进制";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(298, 53);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(74, 23);
            this.btnClose.TabIndex = 42;
            this.btnClose.Text = "关闭连接";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtReceive2
            // 
            this.txtReceive2.Location = new System.Drawing.Point(477, 163);
            this.txtReceive2.Multiline = true;
            this.txtReceive2.Name = "txtReceive2";
            this.txtReceive2.Size = new System.Drawing.Size(323, 55);
            this.txtReceive2.TabIndex = 43;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(419, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 44;
            this.label1.Text = "串口一";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(419, 189);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 45;
            this.label2.Text = "串口二";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(419, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 46;
            this.label3.Text = "串口三";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(419, 392);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 47;
            this.label4.Text = "串口五";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(419, 315);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 48;
            this.label5.Text = "串口四";
            // 
            // txtReceive3
            // 
            this.txtReceive3.Location = new System.Drawing.Point(477, 233);
            this.txtReceive3.Multiline = true;
            this.txtReceive3.Name = "txtReceive3";
            this.txtReceive3.Size = new System.Drawing.Size(323, 55);
            this.txtReceive3.TabIndex = 49;
            // 
            // txtReceive4
            // 
            this.txtReceive4.Location = new System.Drawing.Point(477, 301);
            this.txtReceive4.Multiline = true;
            this.txtReceive4.Name = "txtReceive4";
            this.txtReceive4.Size = new System.Drawing.Size(323, 55);
            this.txtReceive4.TabIndex = 50;
            // 
            // txtReceive5
            // 
            this.txtReceive5.Location = new System.Drawing.Point(477, 366);
            this.txtReceive5.Multiline = true;
            this.txtReceive5.Name = "txtReceive5";
            this.txtReceive5.Size = new System.Drawing.Size(323, 55);
            this.txtReceive5.TabIndex = 51;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtIP);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.txtComNum);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Location = new System.Drawing.Point(11, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(402, 89);
            this.groupBox1.TabIndex = 52;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "连接";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.comStore);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtAddress);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtBit);
            this.groupBox2.Controls.Add(this.txtPLC);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.txtReadCount);
            this.groupBox2.Controls.Add(this.comRead);
            this.groupBox2.Controls.Add(this.btnRead);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.txtValue);
            this.groupBox2.Location = new System.Drawing.Point(10, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(403, 157);
            this.groupBox2.TabIndex = 53;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "读";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comWrite);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.txtWriteValue);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.btnWrite);
            this.groupBox3.Location = new System.Drawing.Point(11, 268);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(402, 100);
            this.groupBox3.TabIndex = 54;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "写";
            // 
            // ZLB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 494);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtReceive5);
            this.Controls.Add(this.txtReceive4);
            this.Controls.Add(this.txtReceive3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtReceive2);
            this.Controls.Add(this.btnPLCRun);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtSendCmd);
            this.Controls.Add(this.txtReceive1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.button18);
            this.Name = "ZLB";
            this.Text = "PPI协议测试";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comStore;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtReceive1;
        private System.Windows.Forms.TextBox txtSendCmd;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtBit;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtReadCount;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox comRead;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox comWrite;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtWriteValue;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPLCRun;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtComNum;
        private System.Windows.Forms.Label label18;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TextBox txtPLC;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtReceive2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtReceive3;
        private System.Windows.Forms.TextBox txtReceive4;
        private System.Windows.Forms.TextBox txtReceive5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

