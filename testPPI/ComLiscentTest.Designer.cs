namespace testPPI
{
    partial class ComLiscentTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCom1 = new System.Windows.Forms.TabPage();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtSend1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRece1 = new System.Windows.Forms.TextBox();
            this.tabCom2 = new System.Windows.Forms.TabPage();
            this.tabCom3 = new System.Windows.Forms.TabPage();
            this.tabCom4 = new System.Windows.Forms.TabPage();
            this.tabCom5 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.btnListen = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRece2 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRece3 = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRece4 = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtRece5 = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabCom1.SuspendLayout();
            this.tabCom2.SuspendLayout();
            this.tabCom3.SuspendLayout();
            this.tabCom4.SuspendLayout();
            this.tabCom5.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(79, 60);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(122, 21);
            this.txtPort.TabIndex = 44;
            this.txtPort.Text = "5000";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(32, 63);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(41, 12);
            this.label17.TabIndex = 43;
            this.label17.Text = "端口号";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(79, 22);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(122, 21);
            this.txtIP.TabIndex = 42;
            this.txtIP.Text = "10.32.0.165";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 41;
            this.label8.Text = "IP地址";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(237, 22);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(89, 23);
            this.button2.TabIndex = 40;
            this.button2.Text = "连接智联宝";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabCom1);
            this.tabControl1.Controls.Add(this.tabCom2);
            this.tabControl1.Controls.Add(this.tabCom3);
            this.tabControl1.Controls.Add(this.tabCom4);
            this.tabControl1.Controls.Add(this.tabCom5);
            this.tabControl1.Location = new System.Drawing.Point(34, 87);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(632, 333);
            this.tabControl1.TabIndex = 45;
            // 
            // tabCom1
            // 
            this.tabCom1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.tabCom1.Controls.Add(this.btnSend);
            this.tabCom1.Controls.Add(this.txtSend1);
            this.tabCom1.Controls.Add(this.label1);
            this.tabCom1.Controls.Add(this.txtRece1);
            this.tabCom1.Location = new System.Drawing.Point(4, 22);
            this.tabCom1.Name = "tabCom1";
            this.tabCom1.Padding = new System.Windows.Forms.Padding(3);
            this.tabCom1.Size = new System.Drawing.Size(624, 307);
            this.tabCom1.TabIndex = 0;
            this.tabCom1.Text = "Com1";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(290, 26);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtSend1
            // 
            this.txtSend1.Location = new System.Drawing.Point(290, 59);
            this.txtSend1.Multiline = true;
            this.txtSend1.Name = "txtSend1";
            this.txtSend1.Size = new System.Drawing.Size(271, 203);
            this.txtSend1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "接收的数据";
            // 
            // txtRece1
            // 
            this.txtRece1.Location = new System.Drawing.Point(19, 59);
            this.txtRece1.Multiline = true;
            this.txtRece1.Name = "txtRece1";
            this.txtRece1.Size = new System.Drawing.Size(248, 203);
            this.txtRece1.TabIndex = 0;
            // 
            // tabCom2
            // 
            this.tabCom2.Controls.Add(this.button3);
            this.tabCom2.Controls.Add(this.textBox2);
            this.tabCom2.Controls.Add(this.label2);
            this.tabCom2.Controls.Add(this.txtRece2);
            this.tabCom2.Location = new System.Drawing.Point(4, 22);
            this.tabCom2.Name = "tabCom2";
            this.tabCom2.Padding = new System.Windows.Forms.Padding(3);
            this.tabCom2.Size = new System.Drawing.Size(624, 307);
            this.tabCom2.TabIndex = 1;
            this.tabCom2.Text = "Com2";
            this.tabCom2.UseVisualStyleBackColor = true;
            // 
            // tabCom3
            // 
            this.tabCom3.Controls.Add(this.button4);
            this.tabCom3.Controls.Add(this.textBox4);
            this.tabCom3.Controls.Add(this.label3);
            this.tabCom3.Controls.Add(this.txtRece3);
            this.tabCom3.Location = new System.Drawing.Point(4, 22);
            this.tabCom3.Name = "tabCom3";
            this.tabCom3.Size = new System.Drawing.Size(624, 307);
            this.tabCom3.TabIndex = 2;
            this.tabCom3.Text = "Com3";
            this.tabCom3.UseVisualStyleBackColor = true;
            // 
            // tabCom4
            // 
            this.tabCom4.Controls.Add(this.button5);
            this.tabCom4.Controls.Add(this.textBox6);
            this.tabCom4.Controls.Add(this.label4);
            this.tabCom4.Controls.Add(this.txtRece4);
            this.tabCom4.Location = new System.Drawing.Point(4, 22);
            this.tabCom4.Name = "tabCom4";
            this.tabCom4.Size = new System.Drawing.Size(624, 307);
            this.tabCom4.TabIndex = 3;
            this.tabCom4.Text = "Com4";
            this.tabCom4.UseVisualStyleBackColor = true;
            // 
            // tabCom5
            // 
            this.tabCom5.AutoScroll = true;
            this.tabCom5.Controls.Add(this.button6);
            this.tabCom5.Controls.Add(this.textBox8);
            this.tabCom5.Controls.Add(this.label5);
            this.tabCom5.Controls.Add(this.txtRece5);
            this.tabCom5.Location = new System.Drawing.Point(4, 22);
            this.tabCom5.Name = "tabCom5";
            this.tabCom5.Size = new System.Drawing.Size(624, 307);
            this.tabCom5.TabIndex = 4;
            this.tabCom5.Text = "Com5";
            this.tabCom5.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(452, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 23);
            this.button1.TabIndex = 46;
            this.button1.Text = "停止监听端口";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(343, 22);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(89, 23);
            this.btnListen.TabIndex = 47;
            this.btnListen.Text = "监听端口";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(312, 35);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "发送";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(312, 68);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(271, 203);
            this.textBox2.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "接收的数据";
            // 
            // txtRece2
            // 
            this.txtRece2.Location = new System.Drawing.Point(41, 68);
            this.txtRece2.Multiline = true;
            this.txtRece2.Name = "txtRece2";
            this.txtRece2.Size = new System.Drawing.Size(248, 203);
            this.txtRece2.TabIndex = 4;
        
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(312, 35);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 7;
            this.button4.Text = "发送";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(312, 68);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(271, 203);
            this.textBox4.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "接收的数据";
            // 
            // txtRece3
            // 
            this.txtRece3.Location = new System.Drawing.Point(41, 68);
            this.txtRece3.Multiline = true;
            this.txtRece3.Name = "txtRece3";
            this.txtRece3.Size = new System.Drawing.Size(248, 203);
            this.txtRece3.TabIndex = 4;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(312, 35);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 7;
            this.button5.Text = "发送";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(312, 68);
            this.textBox6.Multiline = true;
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(271, 203);
            this.textBox6.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(50, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "接收的数据";
            // 
            // txtRece4
            // 
            this.txtRece4.Location = new System.Drawing.Point(41, 68);
            this.txtRece4.Multiline = true;
            this.txtRece4.Name = "txtRece4";
            this.txtRece4.Size = new System.Drawing.Size(248, 203);
            this.txtRece4.TabIndex = 4;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(312, 35);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 7;
            this.button6.Text = "发送";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(312, 68);
            this.textBox8.Multiline = true;
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(271, 203);
            this.textBox8.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(50, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "接收的数据";
            // 
            // txtRece5
            // 
            this.txtRece5.Location = new System.Drawing.Point(41, 68);
            this.txtRece5.Multiline = true;
            this.txtRece5.Name = "txtRece5";
            this.txtRece5.Size = new System.Drawing.Size(248, 203);
            this.txtRece5.TabIndex = 4;
            // 
            // ComLiscentTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 473);
            this.Controls.Add(this.btnListen);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button2);
            this.Name = "ComLiscentTest";
            this.Text = "ComLiscentTest";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ComLiscentTest_FormClosed);
            this.Load += new System.EventHandler(this.ComLiscentTest_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabCom1.ResumeLayout(false);
            this.tabCom1.PerformLayout();
            this.tabCom2.ResumeLayout(false);
            this.tabCom2.PerformLayout();
            this.tabCom3.ResumeLayout(false);
            this.tabCom3.PerformLayout();
            this.tabCom4.ResumeLayout(false);
            this.tabCom4.PerformLayout();
            this.tabCom5.ResumeLayout(false);
            this.tabCom5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabCom1;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtSend1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRece1;
        private System.Windows.Forms.TabPage tabCom2;
        private System.Windows.Forms.TabPage tabCom3;
        private System.Windows.Forms.TabPage tabCom4;
        private System.Windows.Forms.TabPage tabCom5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRece2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRece3;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtRece4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtRece5;
    }
}