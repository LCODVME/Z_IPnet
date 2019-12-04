using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Z_IPnet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        const int CONNECT_REQUEST_CODE = 3000;
        const int CONNECT_RESOPNCE_CODE = 4000;
        const int READ_CONFIG_REQUEST_CODE = 3001;
        const int READ_CONFIG_RESPONCE_CODE = 4001;
        const int WRITE_CONFIG_REQUEST_CODE = 3002;
        const int WRITE_CONFIG_RESPONCE_CODE = 4002;
        const int DISCONNECT_CODE = 3003;
        const int REBOOT_CODE = 3004;
        const int HEART_BEAT_CODE = 3005;
        const int HEART_BEAT_RESPONCE = 4005;
        const int FACTORY_SETTING_CODE = 3006;
        
        UdpClient udp_client;
        Thread udpRcvThread;
        IPEndPoint remotePoint;
        IPAddress remoteIP;
        int remotePort = 5000;
        static int Connect_state = 0;  //0:未连接，1：连接中；，2：已连接
        static int heartBeatCnt = 0;
        System.Timers.Timer heartBeat = new System.Timers.Timer(3000);
        System.Timers.Timer retransmit = new System.Timers.Timer(2000);
        public delegate void delegateCall();
        delegateCall udpRetransmit;
        delegateCall udpRetransmit_old;
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Z_IPnet - V1.2.0";
            udp_client = new UdpClient();
            udpRcvThread = new Thread(receiveProcess);
            udpRcvThread.Start();
            /* create heart beat timer */
            heartBeat.Elapsed += new System.Timers.ElapsedEventHandler(heartBeatSend);
            heartBeat.AutoReset = true;
            heartBeat.Enabled = true;
            heartBeat.Stop();
            /* create retransmit timer */
            retransmit.Elapsed += new System.Timers.ElapsedEventHandler(retransmitTime);
            retransmit.AutoReset = true;
            retransmit.Enabled = true;
            retransmit.Stop();

        }

        private void heartBeatSend(object source, System.Timers.ElapsedEventArgs e)
        {
            heartBeatSend();
            
            if (++heartBeatCnt > 3)
            {
                heartBeat.Stop();
                Connect_state = 0;
                button1.Text = "连接";
                label4.Text = "未连接";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                MessageBox.Show("连接断开");
            }
        }
        static int retranCount = 0;
        private void retransmitTime(object source, System.Timers.ElapsedEventArgs e)
        {
            
            if (udpRetransmit != null)
            {
                udpRetransmit();
            }
            else
            {
                retransmit.Stop();
                retranCount = 0;
            }
            if(udpRetransmit_old == udpRetransmit && ++retranCount > 3)
            {
                udpRetransmit_old = null;
                udpRetransmit = null;
            }
            else if(udpRetransmit_old != udpRetransmit)
            {
                udpRetransmit_old = udpRetransmit;
                retranCount = 0;
            }
        }
        private void heartBeatSend()
        {
            byte[] sendBuf;
            JObject root = new JObject();
            root.Add("code", HEART_BEAT_CODE);
            sendBuf = Encoding.Default.GetBytes(root.ToString(Newtonsoft.Json.Formatting.None, null));
            udp_client.Send(sendBuf, sendBuf.Length, remotePoint);
        }
        private void connectRequest()
        {
            byte[] sendBuf;
            JObject root = new JObject();
            root.Add("code", CONNECT_REQUEST_CODE);
            sendBuf = Encoding.Default.GetBytes(root.ToString(Newtonsoft.Json.Formatting.None, null));
            udp_client.Send(sendBuf, sendBuf.Length, remotePoint);
            udpRetransmit = new delegateCall(connectRequest);
            retransmit.Start();
        }
        private void disConnect()
        {
            byte[] sendBuf;
            JObject root = new JObject();
            root.Add("code", DISCONNECT_CODE);
            sendBuf = Encoding.Default.GetBytes(root.ToString(Newtonsoft.Json.Formatting.None, null));
            udp_client.Send(sendBuf, sendBuf.Length, remotePoint);
        }
        private void readConfigRequest()
        {
            byte[] sendBuf;
            JObject root = new JObject();
            root.Add("code", READ_CONFIG_REQUEST_CODE);
            sendBuf = Encoding.Default.GetBytes(root.ToString(Newtonsoft.Json.Formatting.None, null));
            udp_client.Send(sendBuf, sendBuf.Length, remotePoint);
            udpRetransmit = new delegateCall(readConfigRequest);
            retransmit.Start();
        }
        private void writeConfigRequest()
        {
            try
            {
                byte[] sendBuf;
                string IP_Mac_Buf;
                JObject root = new JObject();
                JObject gateway = new JObject();
                JObject lora = new JObject();
                JObject net = new JObject();
                JObject server = new JObject();
                root.Add("code", WRITE_CONFIG_REQUEST_CODE);
                root.Add("gateway", gateway);
                root.Add("lora", lora);
                root.Add("net", net);
                root.Add("server", server);
                gateway.Add("sn", textBox25.Text);
                lora.Add("baseStationID", int.Parse(textBox20.Text));
                lora.Add("channel", int.Parse(comboBox1.Text));
                lora.Add("power", int.Parse(comboBox3.Text));
                lora.Add("speed", int.Parse(comboBox2.Text));
                IP_Mac_Buf = String.Format("{0}.{1}.{2}.{3}", textBox5.Text, textBox6.Text, textBox7.Text, textBox8.Text);
                net.Add("ip", IP_Mac_Buf);
                IP_Mac_Buf = String.Format("{0}.{1}.{2}.{3}", textBox10.Text, textBox11.Text, textBox12.Text, textBox13.Text);
                net.Add("subMask", IP_Mac_Buf);
                IP_Mac_Buf = String.Format("{0}.{1}.{2}.{3}", textBox14.Text, textBox15.Text, textBox16.Text, textBox17.Text);
                net.Add("gateway", IP_Mac_Buf);
                IP_Mac_Buf = formattingMac(textBox18.Text);
                net.Add("mac", IP_Mac_Buf);
                IP_Mac_Buf = String.Format("{0}.{1}.{2}.{3}", textBox21.Text, textBox22.Text, textBox23.Text, textBox24.Text);
                server.Add("ip", IP_Mac_Buf);
                server.Add("port", int.Parse(textBox26.Text));
                server.Add("user", textBox9.Text);
                server.Add("password", textBox19.Text);
                sendBuf = Encoding.Default.GetBytes(root.ToString(Newtonsoft.Json.Formatting.None, null));
                udp_client.Send(sendBuf, sendBuf.Length, remotePoint);
                udpRetransmit = new delegateCall(writeConfigRequest);
                retransmit.Start();
            }
            catch
            {
                MessageBox.Show("配置填写错误，请检查改正");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                
                string dstIP = String.Format("{0}.{1}.{2}.{3}", textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                remoteIP = IPAddress.Parse(dstIP);
                remotePoint = new IPEndPoint(remoteIP, remotePort);

                if (Connect_state > 0)
                {
                    disConnect();
                    heartBeat.Stop();
                    retransmit.Stop();
                    Connect_state = 0;
                    button1.Text = "连接";
                    label4.Text = "未连接";
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    textBox4.Enabled = true;
                }
                else
                {
                    button1.Text = "取消";
                    label4.Text = "正在连接";
                    connectRequest();
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    textBox4.Enabled = false;
                    Connect_state = 1;
                }
            }
            catch
            {
                MessageBox.Show("地址无效，请输入正确的IP地址");
                textBox1.Focus();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (Connect_state == 2)
            {
                readConfigRequest();
            }
            else
            {
                MessageBox.Show("未连接，请先连接设备");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (Connect_state == 2)
            {
                writeConfigRequest();
            }
            else
            {
                MessageBox.Show("未连接，请先连接设备");
            }
        }
        private string parseJsonMac(string mac)
        {
            char[] buf = mac.ToCharArray();
            char[] macBuf = new char[12];
            for(int i = 0,j = 0; i < buf.Length; i++)
            {
                if (buf[i] != ':')
                {
                    macBuf[j++] = buf[i];
                }
            }
            return new string(macBuf);
        }
        private string formattingMac(string mac)
        {
            char[] buf = mac.ToCharArray();
            char[] macBuf = new char[20];
            for (int i = 0, j = 0, ofs = 0; i < buf.Length; )
            {
                if(ofs == 2 && ofs < buf.Length)
                {
                    macBuf[j++] = ':';
                    ofs = 0;
                }
                else
                {
                    macBuf[j++] = buf[i++];
                    ofs++;
                }
            }
            return new string(macBuf);
        }

        private void receiveProcess(object obj)
        {
            IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
            //udpReceive = new UdpClient(remotePoint);
            string rcvData = null;
            while (true)
            {
                try
                {
                    byte[] bytRcv = udp_client.Receive(ref remotePoint);
                    rcvData = Encoding.Default.GetString(bytRcv, 0, bytRcv.Length);
                    if(rcvData != null)
                    {
                        JObject root = (JObject)JsonConvert.DeserializeObject(rcvData);
                        string code = root["code"].ToString();
                        switch(int.Parse(code))
                        {
                            case CONNECT_RESOPNCE_CODE:
                                {
                                    if (Connect_state != 1) break;
                                    button1.Text = "断开连接";
                                    label4.Text = "已连接";
                                    Connect_state = 2;  //连接状态为连接
                                    heartBeatCnt = 0;
                                    /* start heart beat timer */
                                    heartBeat.Start();
                                    /* stop retransmit timer */
                                    retransmit.Stop();
                                    udpRetransmit = null;
                                    string ip = root["ip"].ToString();
                                    remoteIP = IPAddress.Parse(ip);
                                    remotePoint = new IPEndPoint(remoteIP, remotePort);
                                    textBox4.Text = remoteIP.GetAddressBytes()[4].ToString();
                                }
                                break;
                            case READ_CONFIG_RESPONCE_CODE:
                                {
                                    retransmit.Stop();
                                    udpRetransmit = null;
                                    heartBeatCnt = 0;
                                    string sn = root["gateway"]["sn"].ToString();
                                    textBox25.Text = sn;
                                    string baseStationID = root["lora"]["baseStationID"].ToString();
                                    textBox20.Text = baseStationID;
                                    string channel = root["lora"]["channel"].ToString();
                                    comboBox1.Text = channel;
                                    string power = root["lora"]["power"].ToString();
                                    comboBox3.Text = power;
                                    string speed = root["lora"]["speed"].ToString();
                                    comboBox2.Text = speed;
                                    string n_ip = root["net"]["ip"].ToString();
                                    byte[] IPaddr = IPAddress.Parse(n_ip).GetAddressBytes();
                                    textBox5.Text = IPaddr[0].ToString();
                                    textBox6.Text = IPaddr[1].ToString();
                                    textBox7.Text = IPaddr[2].ToString();
                                    textBox8.Text = IPaddr[3].ToString();
                                    string subMask = root["net"]["subMask"].ToString();
                                    IPaddr = IPAddress.Parse(subMask).GetAddressBytes();
                                    textBox10.Text = IPaddr[0].ToString();
                                    textBox11.Text = IPaddr[1].ToString();
                                    textBox12.Text = IPaddr[2].ToString();
                                    textBox13.Text = IPaddr[3].ToString();
                                    string gateway = root["net"]["gateway"].ToString();
                                    IPaddr = IPAddress.Parse(gateway).GetAddressBytes();
                                    textBox14.Text = IPaddr[0].ToString();
                                    textBox15.Text = IPaddr[1].ToString();
                                    textBox16.Text = IPaddr[2].ToString();
                                    textBox17.Text = IPaddr[3].ToString();
                                    string mac = root["net"]["mac"].ToString();
                                    textBox18.Text = parseJsonMac(mac);
                                    string s_ip = root["server"]["ip"].ToString();
                                    IPaddr = IPAddress.Parse(s_ip).GetAddressBytes();
                                    textBox21.Text = IPaddr[0].ToString();
                                    textBox22.Text = IPaddr[1].ToString();
                                    textBox23.Text = IPaddr[2].ToString();
                                    textBox24.Text = IPaddr[3].ToString();
                                    string port = root["server"]["port"].ToString();
                                    textBox26.Text = port;
                                    string user = root["server"]["user"].ToString();
                                    textBox9.Text = user;
                                    string password = root["server"]["password"].ToString();
                                    textBox19.Text = password;
                                }
                                break;
                            case WRITE_CONFIG_RESPONCE_CODE:
                                retransmit.Stop();
                                udpRetransmit = null;
                                heartBeatCnt = 0;
                                break;
                            case HEART_BEAT_RESPONCE:
                                heartBeatCnt = 0;
                                break;
                        }
                        rcvData = null;
                    }
                }
                catch
                {
                   
                }
                
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                object o = this.GetType().GetField(this.ActiveControl.Name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                TextBox textboxName = (TextBox)o;
                char[] nameArray = this.ActiveControl.Name.ToCharArray();
                nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] + 1);
                string n = new string(nameArray);
                TextBox textboxNextName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                if (e.KeyChar != '\b')
                {
                    if (e.KeyChar < '0' || e.KeyChar > '9')
                    {
                        e.Handled = true;
                        if (e.KeyChar == '.' && textboxName.Text.Length > 0)
                        {
                            textboxNextName.Focus();
                        }
                    }
                    if (textboxName.Text.Length >= 2)
                    {
                        textboxNextName.Focus();
                    }
                }
            }
            catch { }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textboxName = (TextBox)sender;
            char[] nameArray = this.ActiveControl.Name.ToCharArray();
            nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] + 1);
            string n = new string(nameArray);
            TextBox textboxNextName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

            if (e.KeyCode == Keys.Right && textboxName.SelectionStart == textboxName.Text.Length)
            {
                textboxNextName.Focus();
            }
            else if (e.KeyCode == Keys.Enter && nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] < '4')
            {
                button1_Click(sender, e);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                TextBox textboxName = (TextBox)this.GetType().GetField(this.ActiveControl.Name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                char[] nameArray = this.ActiveControl.Name.ToCharArray();
                nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] + 1);
                string n = new string(nameArray);
                TextBox textboxNextName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] - 2);
                n = new string(nameArray);
                TextBox textboxLastName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                if (e.KeyChar != '\b')
                {
                    if (e.KeyChar < '0' || e.KeyChar > '9')
                    {
                        e.Handled = true;
                        if (e.KeyChar == '.' && textboxName.Text.Length > 0)
                        {
                            textboxNextName.Focus();
                        }
                    }
                    if (textboxName.Text.Length >= 2)
                    {
                        textboxNextName.Focus();
                    }
                }
                else
                {
                    if (textboxName.SelectionStart == 0)
                    {
                        textboxLastName.Focus();
                    }
                }
            }
            catch { }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textboxName = (TextBox)this.GetType().GetField(this.ActiveControl.Name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            char[] nameArray = this.ActiveControl.Name.ToCharArray();
            nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] + 1);
            string n = new string(nameArray);
            TextBox textboxNextName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] - 2);
            n = new string(nameArray);
            TextBox textboxLastName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

            if (e.KeyCode == Keys.Right && textboxName.SelectionStart == textboxName.Text.Length)
            {
                textboxNextName.Focus();
            }
            else if (e.KeyCode == Keys.Left && textboxName.SelectionStart == 0)
            {
                textboxLastName.Focus();
            }
            else if (e.KeyCode == Keys.Enter && nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] < '4')
            {
                button1_Click(sender, e);
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                TextBox textboxName = (TextBox)this.GetType().GetField(this.ActiveControl.Name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                char[] nameArray = this.ActiveControl.Name.ToCharArray();
                nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] + 1);
                string n = new string(nameArray);
                TextBox textboxNextName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] - 2);
                n = new string(nameArray);
                TextBox textboxLastName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                if (e.KeyChar != '\b')
                {
                    if (e.KeyChar < '0' || e.KeyChar > '9')
                    {
                        e.Handled = true;
                        if (e.KeyChar == '.' && textboxName.Text.Length > 0)
                        {
                            textboxNextName.Focus();
                        }
                    }
                    if (textboxName.Text.Length >= 2)
                    {
                        textboxNextName.Focus();
                    }
                }
                else
                {
                    if (textboxName.SelectionStart == 0)
                    {
                        textboxLastName.Focus();
                    }
                }
            }
            catch { }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textboxName = (TextBox)this.GetType().GetField(this.ActiveControl.Name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            char[] nameArray = this.ActiveControl.Name.ToCharArray();
            nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] + 1);
            string n = new string(nameArray);
            TextBox textboxNextName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] - 2);
            n = new string(nameArray);
            TextBox textboxLastName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

            if (e.KeyCode == Keys.Right && textboxName.SelectionStart == textboxName.Text.Length)
            {
                textboxNextName.Focus();
            }
            else if (e.KeyCode == Keys.Left && textboxName.SelectionStart == 0)
            {
                textboxLastName.Focus();
            }
            else if (e.KeyCode == Keys.Enter && nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] < '4')
            {
                button1_Click(sender, e);
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                TextBox textboxName = (TextBox)this.GetType().GetField(this.ActiveControl.Name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                char[] nameArray = this.ActiveControl.Name.ToCharArray();
                nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] - 1);
                string n = new string(nameArray);
                TextBox textboxLastName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                if (e.KeyChar != '\b')
                {
                    if (e.KeyChar < '0' || e.KeyChar > '9')
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    if (textboxName.SelectionStart == 0)
                    {
                        textboxLastName.Focus();
                    }
                }
            }
            catch { }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textboxName = (TextBox)sender;
            char[] nameArray = this.ActiveControl.Name.ToCharArray();
            nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] = (char)(nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] - 1);
            string n = new string(nameArray);
            TextBox textboxLastName = (TextBox)this.GetType().GetField(n, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

            if (e.KeyCode == Keys.Left && textboxName.SelectionStart == 0)
            {
                textboxLastName.Focus();
            }
            else if (e.KeyCode == Keys.Enter && nameArray[int.Parse(this.ActiveControl.Name.Length.ToString()) - 1] < '4')
            {
                button1_Click(sender, e);
            }

        }

        private void textBox9_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(textBox5.Text) > 0 && int.Parse(textBox5.Text) < 127)
                {
                    textBox10.Text = "255";
                    textBox11.Text = "0";
                    textBox12.Text = "0";
                    textBox13.Text = "0";
                }
                else if (int.Parse(textBox5.Text) > 128 && int.Parse(textBox5.Text) <= 191)
                {
                    textBox10.Text = "255";
                    textBox11.Text = "255";
                    textBox12.Text = "0";
                    textBox13.Text = "0";
                }
                else if (int.Parse(textBox5.Text) > 191 && int.Parse(textBox5.Text) <= 223)
                {
                    textBox10.Text = "255";
                    textBox11.Text = "255";
                    textBox12.Text = "255";
                    textBox13.Text = "0";
                }
                else
                {
                    MessageBox.Show("IP地址错误，请重新输入！");
                }
            }
            catch
            {
                
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox textboxName = (TextBox)sender;
            try
            {
                if (int.Parse(textboxName.Text) == 127 || int.Parse(textboxName.Text) > 223)
                {
                    MessageBox.Show("IP地址错误，请重新输入！");
                    textboxName.Text = "";
                }
            }
            catch
            {
                textboxName.Focus();
            }
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox textboxName = (TextBox)sender;
            try
            {
                if (int.Parse(textboxName.Text) > 255)
                {
                    MessageBox.Show("IP地址错误，请重新输入！");
                    textboxName.Text = "";
                }
            }
            catch
            {
                textboxName.Focus();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBox1.Text);
        }

        int[,] lora_rate = new int[6, 3] { { 7, 12, 1 }, 
                                           { 8, 7, 1 }, 
                                           { 9, 11, 1 }, 
                                           { 8, 8, 2 }, 
                                           { 9, 8, 2 }, 
                                           { 9, 7, 1 } };
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label37.Text = lora_rate[int.Parse(comboBox2.SelectedItem.ToString()), 0].ToString();
            label38.Text = lora_rate[int.Parse(comboBox2.SelectedItem.ToString()), 1].ToString();
            label40.Text = lora_rate[int.Parse(comboBox2.SelectedItem.ToString()), 2].ToString();
        }


        private void button4_MouseLeave(object sender, EventArgs e)
        {
            textBox19.PasswordChar = '*';
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            textBox19.PasswordChar = '\0';
        }

        private void textBox26_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void textBox18_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && 
                (e.KeyChar < 'a' || e.KeyChar > 'f') && 
                (e.KeyChar < 'A' || e.KeyChar > 'F') && 
                 e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void textBox25_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
