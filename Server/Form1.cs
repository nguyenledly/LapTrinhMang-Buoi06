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
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Threading;
using System.Threading;

namespace Server
{
    public partial class Form1 : Form
    {
        Socket server, client;
        List<Socket> clientSockets = new List<Socket>();
        byte[] data = new byte[1024];
        IPEndPoint ipClient;
        int recv;
        Thread thr = null;
        string receivedData = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            IPEndPoint ipServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipServer);
            server.Listen(5);
            server.BeginAccept(new AsyncCallback(CallAccept), server);

        }
        private void CallAccept(IAsyncResult i)
        {
            Socket socket = server.EndAccept(i);
            clientSockets.Add(socket);
            richTextBox1.Invoke((MethodInvoker)delegate ()
            {
                richTextBox1.Text += "\nClient connected";
            });
            
            socket.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
            server.BeginAccept(new AsyncCallback(CallAccept), null);
        }
        private void ReceiveCallBack(IAsyncResult iar)
        {
            Socket socket = (Socket)iar.AsyncState;
            int received = socket.EndReceive(iar);
            byte[] dataBuf = new byte[received];
            Array.Copy(data, dataBuf, received);

            string text = Encoding.UTF8.GetString(dataBuf);
            richTextBox1.Invoke((MethodInvoker)delegate ()
            {
                richTextBox1.Text += "\nĐã nhận: " + text;
            });
            string respone = DateTime.Now.ToLongTimeString();
            byte[] _data = Encoding.UTF8.GetBytes(respone);
            socket.BeginSend(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(SendCallBack),socket);
            socket.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);

        }
        private static void SendCallBack(IAsyncResult iar)
        {
            Socket socket = (Socket)iar.AsyncState;
            socket.EndSend(iar);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(textBox1.Text);
            richTextBox1.Text += "\nĐã gửi: "+ textBox1.Text;
            textBox1.Text = "";
            client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), client);
            
        }
        void nhanDuLieu()
        {
            while (true)
            {
                if (client.Poll(1000000, SelectMode.SelectRead))
                {

                    data = new byte[1024];
                    client.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceivedData), client);
                }
            }
            
            
        }
        private void SendData(IAsyncResult iar)
        {
            client = (Socket)iar.AsyncState;
            int sent = client.EndSend(iar);
           
        }
        private void ReceivedData(IAsyncResult iar)
        {
            client = (Socket)iar.AsyncState; 
            recv = client.EndReceive(iar);
            receivedData = Encoding.ASCII.GetString(data, 0, recv);
            richTextBox1.Invoke((MethodInvoker)delegate()
            {
                richTextBox1.Text += "\nĐã nhận: " + receivedData;
            });
            //client.Close();
        }
    }
}
