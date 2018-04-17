using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Threading;

namespace Chat_Client
{
    public partial class Form1 : Form
    {
        Socket client;
        byte[] data;
        IPEndPoint ipServer;
        int recv;
        Thread thr = null;
        string receivedData;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                button2.Enabled = false;
                //ipServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                int attemps = 0;
                while (!client.Connected)
                {
                    attemps++;
                    ipServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
                    client.Connect(ipServer);
                    //client.BeginConnect(ipServer, new AsyncCallback(Connected), client);
                }
                richTextBox1.Text += "Connected";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //private void Connected(IAsyncResult i)
        //{
        //    client = ((Socket)i.AsyncState);
        //    client.EndConnect(i);
        //    thr = new Thread(new ThreadStart(nhanDuLieu));
        //    thr.Start();
        //}
        private void button1_Click(object sender, EventArgs e)
        {
            data = new byte[1024];
            data = Encoding.UTF8.GetBytes(textBox2.Text);
            richTextBox1.Text+= "\nĐã gửi: "+(textBox2.Text);
            textBox2.Text = "";
            //client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), client);
            client.Send(data);

            byte[] bufRec = new byte[1024];
            int rec = client.Receive(bufRec);
            byte[] getData = new byte[rec];
            Array.Copy(bufRec, getData, rec);
            richTextBox1.Text += ("\nĐã nhận: " + Encoding.UTF8.GetString(getData));
           
        }
        //void nhanDuLieu()
        //{
        //    while (true)
        //    {
        //        if (client.Poll(1000000, SelectMode.SelectRead))
        //        {
        //            data = new byte[1024];
        //            client.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceivedData), client);
        //        }
        //    }
            
        //}
        //private void SendData(IAsyncResult iar)
        //{
        //    client = (Socket)iar.AsyncState;
        //    int sent = client.EndSend(iar);
            
        //}
        //private void ReceivedData(IAsyncResult iar)
        //{
        //    client = (Socket)iar.AsyncState;
        //    recv = client.EndReceive(iar);
        //    receivedData = Encoding.ASCII.GetString(data);
        //    richTextBox1.Invoke((MethodInvoker)delegate()
        //    {
        //        richTextBox1.Text += "\nĐã nhận: " + (receivedData);
        //    });
        //    //client.Close();
        //}

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
