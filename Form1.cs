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
            this.Text = "Z_IPnet - V1.2.0";
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
                if(textboxName.SelectionStart == 0)
                {
                    textboxLastName.Focus();
                }
            }
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
    }
}
