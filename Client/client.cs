using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class client : Form
    {
       
        public  TcpClient clientsk;
        NetworkStream stm;
        byte[] bb;
      
        public client()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are You Sure To Disconnect ?", "Disconnect", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                String message = textBox5.Text;

                stm = clientsk.GetStream();
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] b = asen.GetBytes("#DIS" + message);
                Console.WriteLine("disconnecting Transmitting....");
                stm.Write(b, 0, b.Length);
                
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                clientsk = new TcpClient();
                Console.WriteLine("Connecting.....");
                String ip = "127.0.0.1";
                String port = "8080";
                clientsk.Connect(IPAddress.Parse(ip), int.Parse(port)); ;
                Console.WriteLine("Connected");
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] b = asen.GetBytes(textBox5.Text);
                Console.WriteLine("...Transmitting....");
                stm = clientsk.GetStream();
                stm.Write(b, 0, b.Length);
                stm.Flush();

                byte[] bb = new byte[10050];
                int em = stm.Read(bb, 0, bb.Length);
                if (em > 0){
                    String returndata = "";
                    for(int i = 0; i<= em; i++)
                    {
                        returndata += Convert.ToChar(bb[i]);
                    }
                    setMessage(returndata);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error..... " + ex.StackTrace);

            }
            new Thread(getemailcontent).Start();
        }

        private void setMessage(String text)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(setMessage), new object[] { text });
                return;
            }
            textBox4.Text += text + Environment.NewLine;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
           
            
            sendEmail(textBox1.Text + "#" + textBox2.Text + ">" + sendertb.Text);
            textBox4.Text += ("\r\n"+"You : "+"\r\n"+"Subject : " + textBox2.Text +"\r\n"+"To : "+textBox1.Text+ "\r\n" + "Message : " + sendertb.Text+"\r\n");
            sendertb.Clear();
            textBox2.Clear();
            textBox1.Clear();


        }

        private void sendEmail(String message)
        {

            try
            {
                
                
                    
                    NetworkStream stream = clientsk.GetStream();
                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] b = asen.GetBytes("$SIN"+ message);
                    Console.WriteLine("sending email Transmitting....");
                    stream.Write(b, 0, b.Length);
                    stream.Flush();

                   
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }

        private void sendallEmail(String message)
        {

            try
            {
                    NetworkStream stream = clientsk.GetStream();
                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] b = asen.GetBytes("$MUL" + message);
                    Console.WriteLine("sendall Transmitting....");
                    stream.Write(b, 0, b.Length);
                    stream.Flush();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }

        private void getemailcontent()
        {
            try
            {
                bb = new byte[1025];
                stm= clientsk.GetStream();
                int byteRead = stm.Read(bb, 0, bb.Length);
                if (byteRead > 0)
                {
                    String text = "";
                    for (int x = 0; x < byteRead; x++)
                    {
                        text += Convert.ToChar(bb[x]);
                    }

                    if (text.IndexOf("$SIN") == 0)
                    {
                        string data = text.Substring(4).Replace("\0", "");
                        string email = data.Split('%')[0];
                        string data2 = data.Substring(email.Length + 1);
                        string subject = data2.Split('$')[0];
                        string msg = data2.Substring(subject.Length + 1);
                        setMessage("\r\n"+"From : " + email);
                        setMessage("Subject : "+subject);
                        setMessage("Message :" + msg);

                    }

                    if (text.IndexOf("$MUL") == 0)
                    {
                        string data = text.Substring(4).Replace("\0", "");
                        string sender = data.Split('$')[0];
                        string data2 = data.Substring(sender.Length + 1);
                        string subject = data2.Split('@')[0];
                        string msg = data2.Substring(subject.Length + 1);
                        setMessage("\r\n" + "From : " + sender);
                        setMessage("Subject : " + subject);
                        setMessage("Message :" + msg);
                    }

                    
                    if (text.IndexOf("D%") == 0)
                    {
                        setMessage("Disconncted");
                        Application.Exit();
                    }

                    if (text.IndexOf("%ST") == 0)
                    {
                        setMessage("\r\n" + "....Server has Stopped....");
                    }



                }
                
                    
               
            }
            catch (Exception ex)

            {
                MessageBox.Show(ex.StackTrace);
            }

            new Thread(getemailcontent).Start();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void client_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            sendallEmail(textBox5.Text+"$"+textBox2.Text+"@"+sendertb.Text);
            textBox4.Text += ("\r\n"+"You : "+"\r\n"+"To all :"+"\r\n"+"Subject : "+textBox2.Text+"\r\n"+"Message : "+sendertb.Text+"\r\n");
            String name = "datax";
            sendertb.Clear();
            textBox2.Clear();
            textBox1.Clear();

        }
    }
}


