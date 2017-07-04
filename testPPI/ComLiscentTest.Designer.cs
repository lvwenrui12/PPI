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
            this.tabCom2 = new System.Windows.Forms.TabPage();
            this.tabCom3 = new System.Windows.Forms.TabPage();
            this.tabCom4 = new System.Windows.Forms.TabPage();
            this.tabCom5 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSend1 = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnListen = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabCom1.SuspendLayout();
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
            this.tabCom1.Controls.Add(this.textBox1);
            this.tabCom1.Location = new System.Drawing.Point(4, 22);
            this.tabCom1.Name = "tabCom1";
            this.tabCom1.Padding = new System.Windows.Forms.Padding(3);
            this.tabCom1.Size = new System.Drawing.Size(624, 307);
            this.tabCom1.TabIndex = 0;
            this.tabCom1.Text = "Com1";
            // 
            // tabCom2
            // 
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
            this.tabCom3.Location = new System.Drawing.Point(4, 22);
            this.tabCom3.Name = "tabCom3";
            this.tabCom3.Size = new System.Drawing.Size(624, 307);
            this.tabCom3.TabIndex = 2;
            this.tabCom3.Text = "Com3";
            this.tabCom3.UseVisualStyleBackColor = true;
            // 
            // tabCom4
            // 
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
            this.tabCom5.Location = new System.Drawing.Point(4, 22);
            this.tabCom5.Name = "tabCom5";
            this.tabCom5.Size = new System.Drawing.Size(624, 307);
            this.tabCom5.TabIndex = 4;
            this.tabCom5.Text = "Com5";
            this.tabCom5.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(19, 59);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(248, 203);
            this.textBox1.TabIndex = 0;
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
            // txtSend1
            // 
            this.txtSend1.Location = new System.Drawing.Point(290, 59);
            this.txtSend1.Multiline = true;
            this.txtSend1.Name = "txtSend1";
            this.txtSend1.Size = new System.Drawing.Size(271, 203);
            this.txtSend1.TabIndex = 2;
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
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabPage tabCom2;
        private System.Windows.Forms.TabPage tabCom3;
        private System.Windows.Forms.TabPage tabCom4;
        private System.Windows.Forms.TabPage tabCom5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnListen;
    }
}