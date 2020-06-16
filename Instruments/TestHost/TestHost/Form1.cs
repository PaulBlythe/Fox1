using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TestHost
{
    public partial class Form1 : Form
    {
        public static Form1 Instance;

        private BackgroundWorker myWorker = new BackgroundWorker();
        public volatile bool running = true;
        delegate void StringArgReturningVoidDelegate(string text);
        Bitmap worldmap;

        UDPComms comms = new UDPComms();
        
        Bitmap fighter;
        Bitmap transport;
        Bitmap sam;
        Bitmap aaa;
        Bitmap naval;
        Bitmap heli;

        public Form1()
        {
            InitializeComponent();
            myWorker.DoWork += new DoWorkEventHandler(myWorker_DoWork);
            myWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myWorker_RunWorkerCompleted);
            myWorker.ProgressChanged += new ProgressChangedEventHandler(myWorker_ProgressChanged);
            myWorker.WorkerReportsProgress = true;
            myWorker.WorkerSupportsCancellation = true;
            Instance = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (myWorker.IsBusy)
            {
                running = false;
                myWorker.CancelAsync();//Issue a cancellation request to stop the background worker

                return;
            }
            button1.Enabled = false;        //Disable the Start button
            button2.Enabled = true;         //Enable the stop button
            running = true;
            myWorker.RunWorkerAsync();      //Call the background worker


        }

        protected void myWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker sendingWorker = (BackgroundWorker)sender;//Capture the BackgroundWorker that fired the event
            Log("Thread started " + DateTime.Now.ToShortTimeString());
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            UdpClient udpClient = new UdpClient(5150);
            while (!sendingWorker.CancellationPending)
            {
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                UDPComms.MessageType type;

                String res = comms.HandleMessage(RemoteIpEndPoint.Address.ToString(), returnData.ToString(), out type);
                switch(type)
                {
                    case UDPComms.MessageType.Connect:
                        AddConnection(res);
                        break;
                    case UDPComms.MessageType.Subscribe:
                        Log(RemoteIpEndPoint.Address.ToString() + " subscribed to stream " + res);
                        break;
                    default:
                        Log("Unkown message " + returnData.ToString());
                        break;
                }
            }
            e.Cancel = true;
        }

        protected void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Thread closed");
        }

        protected void myWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void AddData(String ip, String msg)
        {
            AddConnection(ip);
        }

        private void AddConnection(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true.  
            if (this.listBox1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(AddConnection);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBox1.Items.Add(text);
            }
        }

        private void Log(String ip)
        {
            LogText(ip);
        }

        private void LogText(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true.  
            if (this.listBox3.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(LogText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBox3.Items.Add(text);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            running = false;
            myWorker.CancelAsync();//Issue a cancellation request to stop the background worker

            button1.Enabled = true;
            button2.Enabled = false;
            Log("Thread stopped " + DateTime.Now.ToShortTimeString());

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            running = false;
            myWorker.CancelAsync();//Issue a cancellation request to stop the background worker
        }


        #region World code
        public enum AddTypes
        {
            FighterEmitting,
            Transport,
            NavalEmitting,
            Helicopter,
            SAMEmitting,
            AAA,
            Total
        }
        AddTypes addType = AddTypes.Total;
        public int Heading;
        public List<WorldObject> Objects = new List<WorldObject>();

         
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                addType = AddTypes.FighterEmitting;
            }
        }



        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                addType = AddTypes.Transport;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                addType = AddTypes.NavalEmitting;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                addType = AddTypes.Helicopter;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton6.Checked = false;
                addType = AddTypes.SAMEmitting;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                addType = AddTypes.AAA;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Heading = (int) numericUpDown1.Value;

            RedrawWorld();
        }

        private void RedrawWorld()
        {
            Graphics g = Graphics.FromImage(worldmap);
            g.Clear(Color.Black);

            int cx = 512;
            int cy = 512;
            int hy = (int)(512 * Math.Cos((2 * Math.PI * Heading) / 360));
            int hx = (int)(512 * Math.Sin((2 * Math.PI * Heading) / 360));

            g.DrawLine(Pens.GreenYellow, cx, cy, cx + hx, cy - hy);


            foreach (WorldObject wo in Objects)
            {
                Point p = new Point((int)(cx + wo.x), (int)(cy + wo.y));
                switch (wo.Type)
                {
                    case AddTypes.FighterEmitting:
                        p.X -= 30;
                        p.Y -= 11;
                        g.DrawImage(fighter, p);
                        break;

                    case AddTypes.Transport:
                        p.X -= 30;
                        p.Y -= 17;
                        g.DrawImage(transport, p);
                        break;

                    case AddTypes.SAMEmitting:
                        p.X -= 9;
                        p.Y -= 14;
                        g.DrawImage(sam, p);
                        break;

                    case AddTypes.AAA:
                        p.X -= 6;
                        p.Y -= 16;
                        g.DrawImage(aaa, p);
                        break;

                    case AddTypes.NavalEmitting:
                        p.X -= 24;
                        p.Y -= 14;
                        g.DrawImage(naval, p);
                        break;

                    case AddTypes.Helicopter:
                        p.X -= 30;
                        p.Y -= 15;
                        g.DrawImage(heli, p);
                        break;
                }
            }

            g.Dispose();

            pictureBox1.Invalidate();
            Application.DoEvents();

            comms.BroadcastMessage();
            Log("Broadcast state");

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (addType )
            {
                case AddTypes.Total:
                    {
                       
                    }
                    break;

                default:
                    {
                        WorldObject wo = new WorldObject();
                        wo.Type = addType;
                        wo.Emitting = true;
                        wo.x = e.X - 512;
                        wo.y = e.Y - 512;
                        Objects.Add(wo);

                    }
                    break;
            }
            RedrawWorld();
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            fighter = (Bitmap) Bitmap.FromFile("fighter.png");
            transport = (Bitmap)Bitmap.FromFile("transport.png");
            sam = (Bitmap)Bitmap.FromFile("SAM.png");
            aaa = (Bitmap)Bitmap.FromFile("aaa.png");
            naval = (Bitmap)Bitmap.FromFile("naval.png");
            heli = (Bitmap)Bitmap.FromFile("heli.png");

            worldmap = new Bitmap(1024, 1024);
            pictureBox1.Image = worldmap;
            pictureBox1.Invalidate();
            Application.DoEvents();
            RedrawWorld();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex>=0)
            {
                listBox2.Items.Clear();
                foreach (String s in comms.Handlers[listBox1.SelectedIndex].StreamIDs)
                {
                    listBox2.Items.Add(s);
                }
            }
        }
    }
}
