using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Easy
{
    public partial class Form1 : Form
    {
        static IPAddress ipAd = IPAddress.Parse("127.0.0.1");
        static int PortNumber = 5001;
        TcpListener ServerListener = new TcpListener(ipAd, PortNumber);
        TcpClient clientSocket = default(TcpClient);
        delegate void SetTextCallback(string text);
        NetworkStream networkStream = null;
        byte[] first;
        byte[] second;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thread2 = new Thread(StartServer);
            thread2.Start();

            treeView1.Nodes.Add("S1F1 - Are you there");
            treeView1.Nodes[0].Nodes.Add("R - Are you there request");
            treeView1.Nodes[0].Nodes.Add("D - On line data");
            treeView1.Nodes[0].Nodes[1].Nodes.Add("L");
            treeView1.Nodes[0].Nodes[1].Nodes[0].Nodes.Add("A");
            treeView1.Nodes[0].Nodes[1].Nodes[0].Nodes.Add("A");

            treeView1.Nodes.Add("S1F3 - Selected equipment status");
            treeView1.Nodes[1].Nodes.Add("SSR - Selected equipment status request");
            treeView1.Nodes[1].Nodes[0].Nodes.Add("L");
            treeView1.Nodes[1].Nodes[0].Nodes[0].Nodes.Add("U4");
            treeView1.Nodes[1].Nodes.Add("SSD - Selected equipment status data");
            treeView1.Nodes[1].Nodes[1].Nodes.Add("L");
            treeView1.Nodes[1].Nodes[1].Nodes[0].Nodes.Add("V");

            treeView1.Nodes.Add("S1F5 - Formatted status");
            treeView1.Nodes[2].Nodes.Add("FSR - Formatted status request");
            treeView1.Nodes[2].Nodes[0].Nodes.Add("B");
            treeView1.Nodes[2].Nodes.Add("FSD - Formatted status data");
        }

        private void StartServer()
        {
            ServerListener.Start();
            clientSocket = ServerListener.AcceptTcpClient();
            SetText("Connected to Client");

            while (true)
            {
                try
                {
                    networkStream = clientSocket.GetStream();
                    byte[] readBytes = new byte[clientSocket.ReceiveBufferSize];
                    int bytesread = networkStream.Read(readBytes, 0, clientSocket.ReceiveBufferSize);

                    byte[] bytesreceived = readBytes.Take(bytesread).ToArray();

                    if (bytesreceived.Length == 14)
                    {
                        if (bytesreceived[9] == 5)
                        {
                            SetText("Linktest Request  \n  " + BitConverter.ToString(bytesreceived));
                            processlinktest(bytesreceived);
                        }
                        else if (bytesreceived[9] == 1)
                        {
                            SetText("Select Request  \n  " + BitConverter.ToString(bytesreceived));
                            processselect(bytesreceived);
                        }
                    }

                    else if (bytesreceived.Length > 4)
                    {
                        second = bytesreceived;
                        SetText("S1F2: " + BitConverter.ToString(bytesreceived));
                        //Combine(first, second);
                    }
                }

                catch (Exception ex)
                {
                    SetText(ex.Message);
                }
            }
        }

        private void processlinktest(byte[] bytesreceived)
        {
            if (bytesreceived.Length > 10)
            {
                byte[] sendBytes = new byte[] { 0, 0, 0, 10, 255, 255, 0, 0, 0, 6, 0, 0, 0, 2 };
                networkStream.Write(sendBytes, 0, sendBytes.Length);
                SetText("Linktest Response " + BitConverter.ToString(sendBytes));
            }
        }

        private void processselect(byte[] bytesreceived)
        {
            if (bytesreceived.Length > 10)
            {
                byte[] sendBytes = new byte[] { 0, 0, 0, 10, 255, 255, 0, 0, 0, 2, 0, 0, 0, 1 };
                networkStream.Write(sendBytes, 0, sendBytes.Length);
                SetText("Select Response \n" + BitConverter.ToString(sendBytes));
            }
        }

        public void Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            processsecs(ret);
        }

        private void processsecs(byte[] ret)
        {
            throw new NotImplementedException();
        }

        private void SetText(string text)
        {
            if (this.listBox1.InvokeRequired)
            {

                listBox1.Invoke(new SetTextCallback(SetText), text);
            }

            else
            {
                this.listBox1.Items.Add(text);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);

                if (treeView1.SelectedNode != null)
                {
                    contextMenuStrip1.Show(treeView1, e.Location);
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {   
            try
            {
                byte[] sends1f1 = new byte[] { 00, 00, 00, 10, 00, 01, 129, 01, 00, 00, 00, 00, 00, 01 };
                networkStream.Write(sends1f1, 0, sends1f1.Length);
                T3.Start();
                SetText("S1F1: " + BitConverter.ToString(sends1f1));
            }
            catch (Exception ex)
            {
                SetText(ex.Message);
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void T3_Tick(object sender, EventArgs e)
        {
            int x = 0;
            x++;
            if (x == 5)
            {
                SetText("no reply");
            }
        }
    }
}
