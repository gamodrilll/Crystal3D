using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Timers;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Drawing.Imaging;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        DateTime dt = new DateTime(1970, 1, 1);

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        ClickData data = null;

        Process unityProcess;
        WinServer serv;

        private void activateWindow()
        {
            if (unityProcess!=null)
                SendMessage(unityProcess.MainWindowHandle, 0x0006, (IntPtr)2, (IntPtr)0);
        }


        public Form1()
        {
            InitializeComponent();
            serv = new WinServer();
            serv.onClick += Serv_onClick;
            serv.Continue += Continue;
            foreach (var i in Groups.allGroups)
                groupComboBox.Items.Add(i.name);
        }

        

        private void SetSizeChild()
        {
            Thread.Sleep(5000);
            MoveWindow(unityProcess.MainWindowHandle, -8, -31, UnityPanel.Width + 15, UnityPanel.Height + 38, true);
        }




        private void Form1_Load(object sender, EventArgs e)
        {
            ProcessStartInfo st = new ProcessStartInfo();
            checkForExistingProcess();
            st.FileName = @"HanticStructure3d.exe";
            st.WindowStyle = ProcessWindowStyle.Normal;
            unityProcess = Process.Start(st);
            IntPtr x = unityProcess.MainWindowHandle;
            while (unityProcess.MainWindowHandle == x) ;
            SetParent(unityProcess.MainWindowHandle, this.UnityPanel.Handle);
            Thread setSize = new Thread(SetSizeChild);
            setSize.Start();
            MoveWindow(unityProcess.MainWindowHandle, -8, -31, UnityPanel.Width + 17, UnityPanel.Height + 40, true);
            UnityPanel.Focus();
            
        }

        private void checkForExistingProcess()
        {
            Process[] pr = Process.GetProcessesByName("HanticStructure3d");
            foreach (var i in pr)
                i.Kill();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            MoveWindow(unityProcess.MainWindowHandle, -8, -31, UnityPanel.Width + 15, UnityPanel.Height + 38, true);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            unityProcess.CloseMainWindow();
            unityProcess.Kill();
        }

        private void UnityPanel_Resize(object sender, EventArgs e)
        {
           if(unityProcess != null)
                MoveWindow(unityProcess.MainWindowHandle, -8, -31, UnityPanel.Width + 15, UnityPanel.Height + 38, true);
        }

        private void sendMessageToUnity(string msg)
        {
            Int32 port = 2200;
            TcpClient client = new TcpClient("127.0.0.1", port);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);
            NetworkStream stream = client.GetStream();
            stream.WriteByte((byte)data.Length);
            stream.Write(data, 0, data.Length);
            stream.Close();
            client.Close();
            activateWindow();
        }


        private void sendComppoundToUnity(Compound c)
        {
            Int32 port = 2200;
            TcpClient client = new TcpClient("127.0.0.1", port);
      
            NetworkStream stream = client.GetStream();
            try
            {
                using (FileStream fstream = new FileStream("compound.xml", FileMode.OpenOrCreate))
                {
                    (new XmlSerializer(typeof(Compound))).Serialize(fstream, c);
                    fstream.Close();
                }
                stream.WriteByte(0);
            }
            catch
            {
                MessageBox.Show("Ошибка доступа! Перезапустите приложение!");
            }
            finally
            {
                stream.Close();
                client.Close();
                activateWindow();
            }
        }

        private void sendGroupToUnity(Group g)
        {
            Int32 port = 2200;
            TcpClient client = new TcpClient("127.0.0.1", port);

            NetworkStream stream = client.GetStream();
            using (FileStream fstream = new FileStream("group.xml", FileMode.OpenOrCreate))
            {
                (new XmlSerializer(typeof(Group))).Serialize(fstream,g);
                fstream.Close();
            }
            stream.WriteByte(255);
            stream.Close();
            client.Close();
            activateWindow();
        }

        private void XYButton_Click(object sender, EventArgs e)
        {
            sendMessageToUnity("XY"); 
        }

        private void XZButton_Click(object sender, EventArgs e)
        {
            sendMessageToUnity("XZ");
        }

        private void Button3D_Click(object sender, EventArgs e)
        {
            sendMessageToUnity("3D");
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            sendMessageToUnity("Reset");
        }



        private void compoundComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Wait();
            int compoundNumber = compoundComboBox.SelectedIndex;
            if (compoundNumber < 0)
                return;
            current = Compounds.allCompounds.Find((c) => (c.name == compoundComboBox.Text));
            sendComppoundToUnity(current);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
           
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            if (data != null)
            {
               
                data = null;
            }
            if (dt == new DateTime(1970, 1, 1))
                return;
            if (DateTime.Now.Subtract(dt).TotalSeconds > 30.0)
            {
                MessageBox.Show("Программа завершена аварийно. Перезапустите программу!");
            }
        }

        private void UnityPanel_MouseMove(object sender, MouseEventArgs e)
        {
            activateWindow();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        Bitmap bm;
        void SaveImage()
        {
            bm.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            SaveImage();
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var handle =UnityPanel.Handle;
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            Rectangle bounds = new Rectangle(rect.Left, rect.Top, UnityPanel.Width, UnityPanel.Height);
            bm = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(bm))
            {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }
            saveFileDialog1.ShowDialog();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void groupComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            Wait();
            int groupNumber = groupComboBox.SelectedIndex;
            compoundComboBox.Items.Clear();
            if (groupNumber < 0)
            {
                compoundComboBox.Enabled = true;
                return;
            }
            compoundComboBox.Enabled = true;
            foreach (var i in from comp in Compounds.allCompounds
                              where comp.@group == Groups.allGroups[groupNumber].name
                              select comp)
                compoundComboBox.Items.Add(i.name);
            sendGroupToUnity(Groups.allGroups.Find((g) => (g.name == groupComboBox.Text)));

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
            if (toolStripMenuItem1.Checked)
                return;
            toolStripMenuItem1.Checked = true;
            toolStripMenuItem2.Checked = false;
            sendMessageToUnity("One");
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (toolStripMenuItem2.Checked)
                return;
            toolStripMenuItem1.Checked = false;
            toolStripMenuItem2.Checked = true;
            sendMessageToUnity("All");
        }

        private void betaAnglButton_Click(object sender, EventArgs e)
        {
            try
            {
                unityProcess.Kill();
            }
            catch
            {

            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex >= 2)
                graphButton.Enabled = true;
            else
                graphButton.Enabled = false;
        }


        public void Wait()
        {
            dt = DateTime.Now;
            this.Cursor = Cursors.WaitCursor;
            Enabled = false;
        }

        public void Continue()
        {
            dt = new DateTime(1970, 1, 1);
            this.Invoke((Action)delegate () {
                this.Cursor = Cursors.Default;
                Enabled = true;
            });
            
        }
    }
}
