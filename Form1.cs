using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Z_IPnet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }



        static bool Connect_state = false;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                int[] dstIP = new int[4];
                dstIP[0] = int.Parse(textBox1.Text);
                dstIP[1] = int.Parse(textBox2.Text);
                dstIP[2] = int.Parse(textBox3.Text);
                dstIP[3] = int.Parse(textBox4.Text);

                if(dstIP[0] == 127 || dstIP[0] > 223)
                {
                    dstIP[4] = 0;
                }
                else
                {
                    for(i = 1; i < 4; i++)
                    {
                        if(dstIP[i] > 255) dstIP[4] = 0;
                    }
                }
                if (Connect_state)
                {
                    Connect_state = false;
                    button1.Text = "连接";
                    label4.Text = "未连接";
                }
                else
                {
                    Connect_state = true;
                    button1.Text = "断开连接";
                    label4.Text = "正在连接";
                }
            }
            catch
            {
                MessageBox.Show("地址无效，请输入正确的IP地址");
                textBox1.Focus();
            }

            

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar != '\b')
            {
                if (e.KeyChar < '0' || e.KeyChar > '9')
                {
                    e.Handled = true;
                    if (e.KeyChar == '.' && textBox1.Text.Length > 0)
                    {
                        textBox2.Focus();
                    }
                }
                if (textBox1.Text.Length >= 2)
                {
                     textBox2.Focus();
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Right && textBox1.SelectionStart == textBox1.Text.Length)
            {
                textBox2.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (e.KeyChar < '0' || e.KeyChar > '9')
                {
                    e.Handled = true;
                    if (e.KeyChar == '.' && textBox2.Text.Length > 0)
                    {
                        textBox3.Focus();
                    }
                }
                if (textBox2.Text.Length >= 2)
                {
                    textBox3.Focus();
                }
            }
            else
            {
                if(textBox2.SelectionStart == 0)
                {
                    textBox1.Focus();
                }
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right && textBox2.SelectionStart == textBox2.Text.Length)
            {
                textBox3.Focus();
            }
            else if(e.KeyCode == Keys.Left && textBox2.SelectionStart == 0)
            {
                textBox1.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (e.KeyChar < '0' || e.KeyChar > '9')
                {
                    e.Handled = true;
                    if (e.KeyChar == '.' && textBox3.Text.Length > 0)
                    {
                        textBox4.Focus();
                    }
                }
                if (textBox3.Text.Length >= 2)
                {
                    textBox4.Focus();
                }
            }
            else
            {
                if (textBox3.SelectionStart == 0)
                {
                    textBox2.Focus();
                }
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right && textBox3.SelectionStart == textBox3.Text.Length)
            {
                textBox4.Focus();
            }
            else if (e.KeyCode == Keys.Left && textBox3.SelectionStart == 0)
            {
                textBox2.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (e.KeyChar < '0' || e.KeyChar > '9')
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (textBox4.SelectionStart == 0)
                {
                    textBox3.Focus();
                }
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && textBox4.SelectionStart == 0)
            {
                textBox3.Focus();
            }
            else if(e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}
