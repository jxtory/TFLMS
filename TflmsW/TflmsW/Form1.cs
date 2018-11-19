using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
// Tflms����
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace TflmsW
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // ��������
        public String fileCmdContent = "";
        // �ļ��б�
        public String[] fileList = null;
        // ͼ����Դ
        public List<Image> pictures = new List<Image>();
        // ����λ��
        public int PicPos = 0;


        // LED ��ʾ�����ؿ���
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll ")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childe, string strclass, string FrmText);

        // �¼�����
        const int WM_SETTEXT = 0x000C;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int WM_CLOSE = 0x0010;

        IntPtr hwnd = FindWindow(null, "����������ʾϵͳ");
        // �ķ�֮�����Ž�����ϵͳ
        //Create Standalone SDK class dynamicly.
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();

        const int NOGroupIndex = 101; // Normally Open
        const int NCGroupIndex = 102; // Normally Close        

        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        // �ö�����
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow(); //��ñ�����ľ��
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//���ô˴���Ϊ�����
        //�������,�������
        public IntPtr Handle1;
        //�ڴ�����ص�ʱ���������ֵ,������ǰ����ľ����������

        /// <summary>
        /// ��ȡ�������ʱ��
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
        /// <summary>
        /// ��ȡ�������ʱ��
        /// </summary>
        /// <param name="plii"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        /// <summary>
        /// �������״̬�ļ���������״̬��
        /// </summary>
        /// <param name="bShow">״̬</param>
        /// <returns>״̬����</returns>
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public static extern int ShowCursor(bool bShow);

        //���״̬������
        int iCount = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            // ��ʼ�����
            InitCheck();

            // ���
            Handle1 = this.Handle;
            //string AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;

            bIsConnected = axCZKEM1.Connect_Net("192.168.10.10", Convert.ToInt32("4370"));

            if (bIsConnected == true)
            {
                iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                //MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
                File.WriteAllText("doorError.txt", "����û�����ӳɹ�����������ǣ�" + idwErrorCode.ToString());
            }
            Cursor = Cursors.Default;

            // ��ӳ��
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.VideoPlayer.Visible = false;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            string appPath = @System.AppDomain.CurrentDomain.BaseDirectory;
            // �˳�����
            if (File.Exists(appPath + "/app_quit"))
            {
                DeleteFile(appPath + "/app_quit");
                System.Environment.Exit(0);
            }

            // ����Teamviewer����
            IntPtr tmw = new IntPtr();
            tmw = FindWindow(null, "����Ự");
            if (tmw != new IntPtr())
            {
                IntPtr okbtn = FindWindowEx(tmw, IntPtr.Zero, null, "ȷ��");
                SetForegroundWindow(okbtn);
                SendMessage(okbtn, WM_LBUTTONDOWN, IntPtr.Zero, null);
                SendMessage(okbtn, WM_LBUTTONUP, IntPtr.Zero, null);
            }

            // ������ӳ
            if (File.Exists(appPath + "/led_play"))
            {
                DeleteFile(appPath + "/led_play");
                // ȡ��һ��
                this.BackgroundImage = null;
                this.BackColor = Color.FromArgb(255, 0, 0, 51);
                this.BackColor = Color.Black;
                this.VideoPlayer.Ctlcontrols.stop();
                this.VideoPlayer.Visible = false;
                this.VideoPlayer.URL = "";
                this.Timer3.Enabled = false;
                this.PicPos = 0;
                pictures = new List<Image>();
                fileList = null;

                ReadIt(appPath + "/playfile");
                String playFile = fileCmdContent;
                String fileType = fileCmdContent.Split('.')[1];
                if(fileType == "mp4")
                {
                    this.VideoPlayer.uiMode = "None";
                    this.VideoPlayer.Visible = true;
                    this.VideoPlayer.Location = new Point(0, 0);
                    this.VideoPlayer.Size = this.Size;
                    this.VideoPlayer.Height = this.Height - (this.Height - this.ClientRectangle.Height);
                    this.VideoPlayer.URL = @appPath + "/tqupload/" + fileCmdContent;
                    this.VideoPlayer.settings.setMode("loop", true);
                    this.VideoPlayer.Ctlcontrols.play();
                }
                else
                {
                    this.BackgroundImage = Image.FromFile(appPath + "/tqupload/" + fileCmdContent);
                }
            }

            // ������ӳ
            if (File.Exists(appPath + "/led_carousel"))
            {
                DeleteFile(appPath + "/led_carousel");
                // ȡ��һ��
                this.BackgroundImage = null;
                this.BackColor = Color.FromArgb(255, 0, 0, 51);
                this.BackColor = Color.Black;
                this.VideoPlayer.Ctlcontrols.stop();
                this.VideoPlayer.Visible = false;
                this.VideoPlayer.URL = "";
                this.Timer3.Enabled = false;
                this.PicPos = 0;
                pictures = new List<Image>();
                fileList = null;

                ReadIt(appPath + "/playfiles");
                fileList = fileCmdContent.Split(',');
                for (int i = 0; i < fileList.Length; i++)
                {
                    Image subPlay = Image.FromFile(appPath + "/tqupload/" + fileList[i]);
                    pictures.Add(subPlay);
                }

                this.PicPos = 0;
                this.BackgroundImage = pictures[0];
                Timer3.Enabled = true;
            }

            // ȡ����ӳ
            if (File.Exists(appPath + "/led_stop"))
            {
                DeleteFile(appPath + "/led_stop");
                this.BackgroundImage = null;
                this.BackColor = Color.FromArgb(255, 0, 0, 51);
                this.VideoPlayer.Ctlcontrols.stop();
                this.VideoPlayer.Visible = false;
                this.VideoPlayer.URL = "";
                this.Timer3.Enabled = false;
                this.PicPos = 0;
                pictures = new List<Image>();
                fileList = null;

                //Graphics g = this.CreateGraphics();
                //Font font = new Font("����", 24, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline, GraphicsUnit.Point);//�������������壬24���ֺţ�FontStyle�ļ������ֵ���ʽ������GraphicsUnit���ֵĶ�����λ�����Կ�һ���ϱߵı�
                //g.DrawString("�ķ�֮����IT����", font, Brushes.Brown, new Point(0, this.Height / 2));//�������壬font���ϱ߶�������壬Brushes.Brown����ɫ��Point���ִ��ĸ�λ�ÿ�ʼ����
                //g.Dispose();

            }

            // ���ڸ�λ
            if (File.Exists(appPath + "/app_max"))
            {
                DeleteFile(appPath + "/app_max");
                this.TopMost = true;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                this.Left = 0;
                this.Top = 0;
                this.WindowState = FormWindowState.Maximized;
                this.Activate();
                this.Show();

                SetForegroundWindow(Handle1);

            }

            // ��Ļ����
            if (File.Exists(appPath + "/led_open"))
            {
                DeleteFile(appPath + "/led_open");
                IntPtr subWindow = FindWindowEx(hwnd, IntPtr.Zero, null, "");
                IntPtr itbtn = FindWindowEx(subWindow, IntPtr.Zero, null, "����");
                SetForegroundWindow(itbtn);
                SendMessage(itbtn, WM_LBUTTONDOWN, IntPtr.Zero, null);
                SendMessage(itbtn, WM_LBUTTONUP, IntPtr.Zero, null);

            }

            // ��Ļ�ػ�
            if (File.Exists(appPath + "/led_close"))
            {
                DeleteFile(appPath + "/led_close");
                IntPtr subWindow = FindWindowEx(hwnd, IntPtr.Zero, null, "");
                IntPtr itbtn = FindWindowEx(subWindow, IntPtr.Zero, null, "�ػ�");
                SetForegroundWindow(itbtn);
                SendMessage(itbtn, WM_LBUTTONDOWN, IntPtr.Zero, null);
                SendMessage(itbtn, WM_LBUTTONUP, IntPtr.Zero, null);

            }

            // һ������
            if (File.Exists(appPath + "/opendoor"))
            {
                DeleteFile(appPath + "/opendoor");
                if (bIsConnected == false)
                {
                    return;
                }

                int idwErrorCode = 0;
                int iDelay = Convert.ToInt32("-1");//time to delay

                Cursor = Cursors.WaitCursor;
                if (axCZKEM1.ACUnlock(iMachineNumber, iDelay))
                {
                    //MessageBox.Show("ACUnlock, Dalay Seconds:" +iDelay.ToString(), "Success");
                }
                else
                {
                    axCZKEM1.GetLastError(ref idwErrorCode);
                    MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                }
                Cursor = Cursors.Default;
            }

            // ����15��
            if (File.Exists(appPath + "/opendoor15"))
            {
                DeleteFile(appPath + "/opendoor15");
                if (bIsConnected == false)
                {
                    return;
                }

                int idwErrorCode = 0;
                int iDelay = Convert.ToInt32("15");//time to delay

                Cursor = Cursors.WaitCursor;
                if (axCZKEM1.ACUnlock(iMachineNumber, iDelay))
                {
                    //MessageBox.Show("ACUnlock, Dalay Seconds:" +iDelay.ToString(), "Success");
                }
                else
                {
                    axCZKEM1.GetLastError(ref idwErrorCode);
                    MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                }
                Cursor = Cursors.Default;
            }

            // һ������
            if (File.Exists(appPath + "/closedoor"))
            {
                DeleteFile(appPath + "/closedoor");
                if (bIsConnected == false)
                {
                    return;
                }

                int idwErrorCode = 0;
                int iDelay = Convert.ToInt32("0");//time to delay

                Cursor = Cursors.WaitCursor;
                if (axCZKEM1.ACUnlock(iMachineNumber, iDelay))
                {
                    //MessageBox.Show("ACUnlock, Dalay Seconds:" +iDelay.ToString(), "Success");
                }
                else
                {
                    axCZKEM1.GetLastError(ref idwErrorCode);
                    MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                }
                Cursor = Cursors.Default;

            }

        }

        // ��ʼ�����
        public void InitCheck()
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.Process[] pros = System.Diagnostics.Process.GetProcessesByName(process.ProcessName);
            for (int i = 0; i < pros.Length; i++)
            {
                if (process.Id != pros[i].Id) //ɸѡ����ǰ�Ľ���
                {
                    System.Environment.Exit(0);
                }
            }

            DeleteFiles("app_quit");
            DeleteFiles("app_max");
            DeleteFiles("led_open");
            DeleteFiles("led_close");
            DeleteFiles("led_play");
            DeleteFiles("led_stop");
            DeleteFiles("led_carousel");
            DeleteFiles("opendoor");
            DeleteFiles("opendoor15");
            DeleteFiles("closedoor");

            // Timer
            Timer1.Enabled = true;
        }

        public void DeleteFiles(string path)
        {
            string appPath = @System.AppDomain.CurrentDomain.BaseDirectory;

            if (File.Exists(appPath + "/" + path))
            {
                DeleteFile(appPath + "/" + path);
            }

        }

        public void DeleteFile(string path)
        {
            // 2������·���ַ����ж����ļ������ļ���
            FileAttributes attr = File.GetAttributes(path);
            // 3�����ݾ������ͽ���ɾ��
            if (attr == FileAttributes.Directory)
            {
                // 3.1��ɾ���ļ���
                Directory.Delete(path, true);
            }
            else
            {
                // 3.2��ɾ���ļ�
                File.Delete(path);
            }
            File.Delete(path);

        }

        //// ���ͼ�����
        //public void AddPictureBox(int left, int top)
        //{
        //    PictureBox pic = new PictureBox();
        //    this.Controls.Add(pic);
        //    pic.Left = left;
        //    pic.Top = top;
        //    pic.Width = this.Width;
        //    pic.Height = this.Height;
        //    //pic.Image = Image.FromFile(@"E:\\86d6277f9e2f070891004d53e424b899a901f258.jpg");
        //    pic.SizeMode = PictureBoxSizeMode.StretchImage;
        //    //pic.Load("http://e.hiphotos.baidu.com/image/pic/item/9345d688d43f8794906df240df1b0ef41ad53ac9.jpg");
        //    pic.Load("http://led.tqcen.com/1.jpg");
        //}

        /// <summary>
        /// ��ȡ����ʱ��
        /// </summary>
        /// <returns></returns>
        public long GetIdleTick()
        {
            LASTINPUTINFO vLastInputInfo = new LASTINPUTINFO();
            vLastInputInfo.cbSize = Marshal.SizeOf(vLastInputInfo);
            if (!GetLastInputInfo(ref vLastInputInfo)) return 0;
            return Environment.TickCount - (long)vLastInputInfo.dwTime;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // �ö���ȡ���ö�
            if (e.KeyCode == Keys.Q)
            {
                this.TopMost = !this.TopMost;
            }

            // ����߿�
            if (e.KeyCode == Keys.B)
            {
                if (this.FormBorderStyle == FormBorderStyle.Sizable)
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                }
                else
                {
                    this.Width = 800;
                    this.Height = 600;
                    this.Left = 0;
                    this.Top = 0;
                    this.TopMost = false;
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                }
            }

            // �������
            if (e.KeyCode == Keys.M)
            {
                if (this.FormBorderStyle == FormBorderStyle.None)
                {
                    this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                    this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                    this.Left = 0;
                    this.Top = 0;
                }
            }

        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            //���״̬������>=0����������ɼ���<0���ɼ���������ֱ����api����Ӱ����ı�
            long i = GetIdleTick();
            if (i > 5000)
            {
                while (iCount >= 0)
                {
                    iCount = ShowCursor(false);
                }
            }
            else
            {
                while (iCount < 0)
                {
                    iCount = ShowCursor(true);
                }
            }
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            this.PicPos++;
            if (this.PicPos >= fileList.Length)
            {
                this.PicPos = 0;
            }
            this.BackgroundImage = pictures[this.PicPos];

        }

        // �ļ�ȡ��
        public void ReadIt(String filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    fileCmdContent = File.ReadAllText(filePath);
                    byte[] mybyte = Encoding.UTF8.GetBytes(fileCmdContent);
                    fileCmdContent = Encoding.UTF8.GetString(mybyte);
                }
                else
                {
                    //MessageBox.Show("�ļ�������");
                    File.WriteAllText("app_log.txt", "�ļ�������");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                File.WriteAllText("app_log.txt", ex.Message);
            }
        }

        private void VideoPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if ((int)VideoPlayer.playState == 3)
            {
                VideoPlayer.fullScreen = true;
            }
        }
    }
}
