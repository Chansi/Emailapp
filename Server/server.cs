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
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.IO;

namespace Server
{
    public partial class server : Form
    {
        TcpListener clientListner;
        Socket socket;
        List<Socket> clientlist = new List<Socket>();
        Hashtable cList = new Hashtable();

        public server()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(StartServer);
            t1.Start();

        }

        public void StartServer()
        {
            try
            {
                String ip = "127.0.0.1";
                String port = "8080";
                clientListner = new TcpListener(IPAddress.Parse(ip), int.Parse(port));
                clientListner.Start();
                setMessage(".....Server started..../");
                setMessage(".....Waiting for clients..../");
                new Thread(connectClient).Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }


        }

        private void setMessage(String text)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(setMessage), new object[] { text });
                return;
            }
            textboxserver.Text += text+Environment.NewLine;
        }

        private void connectClient()
        {

            socket = clientListner.AcceptSocket();
            setMessage("Connetion establish from  " + socket.RemoteEndPoint);
            clientlist.Add(socket);
            byte[] takebyte = new byte[10050];
            int data = socket.Receive(takebyte);
            String email = "";
            for (int i = 0; i <= data; i++)
            {
                email += Convert.ToChar(takebyte[i]);
            }
            setMessage(email+"Connected.."+"/r/n");

            if (!cList.ContainsKey(email))
            {
                ASCIIEncoding asen = new ASCIIEncoding();
                socket.Send(asen.GetBytes("Welcome to email " + email));
               

                cList.Add(email, socket);
               
                Thread tt = new Thread(() => getEmailContent(socket));
                tt.Start();
            }
            new Thread(connectClient).Start();
        }


    

        private void getEmailContent(Socket s)
        {
           
            try
            {
                string message = "";
                byte[] bytefromx = new byte[1024];
                int data = s.Receive(bytefromx);
                for (int i = 0; i <= data; i++)
                {
                    message += (Convert.ToChar(bytefromx[i]));
                }
                setMessage(message);
                if (message.IndexOf("$SIN") == 0) {
                    string data1 = message.Substring(4);
                    string remail = data1.Split('#')[0];
                    string data2 = data1.Substring(remail.Length + 1);
                    string subject = data2.Split('>')[0];
                    string msg = data2.Substring(subject.Length + 1);
                    string senderemail = "";
                    foreach (string email in cList.Keys)
                    {
                        if (s.Equals(cList[email]))
                        {
                            senderemail = email;
                            break;
                        }
                    }
                    senderemail = senderemail.Replace("\0", "");
                    remail = remail.Replace("\0", "");

                    foreach (string email in cList.Keys)
                    {
                        
                        if (remail.Replace("\0", "").Equals(email.Replace("\0", "")))
                        {
                            string sendMsg = "$SIN" + senderemail + "%" + subject + "$" + msg.Replace("\0", "");
                            Socket receiverS = (Socket)cList[email];
                            ASCIIEncoding ase = new ASCIIEncoding();
                            byte[] broadcastBytes = ase.GetBytes(sendMsg);
                            receiverS.Send(broadcastBytes);
                            break;

                        }
                    }

                }

                else if (message.IndexOf("$MUL") == 0)
                {
                    string data1 = message.Substring(4);
                    string sender = data1.Split('$')[0];
                    string data2 = data1.Substring(sender.Length + 1);
                    string subject = data2.Split('@')[0];
                    string msg = data2.Substring(subject.Length + 1);

                    foreach (Socket sk in clientlist ) {
                        new Thread(()=> {
                            if (sk != s)
                            {
                                ASCIIEncoding ase = new ASCIIEncoding();
                                byte[] broadcastBytes = ase.GetBytes(message);
                                sk.Send(broadcastBytes);
                            }
                        }).Start();
                       

                       
                        
                    }

                }

                else if(message.IndexOf("#DIS") == 0)
                {
                    string name = message.Substring(4);
                    foreach(Socket skt in clientlist)
                    {
                        if (s.Equals( skt))
                        {
                            string dataset = "D%";
                            ASCIIEncoding ase = new ASCIIEncoding();
                            byte[] broadcastBytes = ase.GetBytes(dataset);
                            s.Send(broadcastBytes);
                            cList.Remove(name);
                            clientlist.Remove(s);
                            break;
                        }

                    }

                }


                Thread tt = new Thread(() => getEmailContent(s));
                tt.Start();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

          
           
        }

  

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textboxserver_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure To Stop the server ?", "Disconnect", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string message = "%ST";
                foreach (Socket sk in clientlist)
                {
                    new Thread(() =>
                    {

                        {
                            ASCIIEncoding ase = new ASCIIEncoding();
                            byte[] broadcastBytes = ase.GetBytes(message);
                            sk.Send(broadcastBytes);
                         
                        }

                    }).Start();
  
                }

                clientlist.Clear();
                cList.Clear();
                Application.Exit();
            }


            
                
         
        }
    }
}
