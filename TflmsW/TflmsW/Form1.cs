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
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
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

        // ���ͼ�����
        public void AddPictureBox(int left, int top)
        {
            PictureBox pic = new PictureBox();
            this.Controls.Add(pic);
            pic.Left = left;
            pic.Top = top;
            pic.Width = this.Width;
            pic.Height = this.Height;
            //pic.Image = Image.FromFile(@"E:\\86d6277f9e2f070891004d53e424b899a901f258.jpg");
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
            //pic.Load("http://e.hiphotos.baidu.com/image/pic/item/9345d688d43f8794906df240df1b0ef41ad53ac9.jpg");
            pic.Load("http://led.tqcen.com/1.jpg");
        }

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

            // ����ͼƬ
            if (e.KeyCode == Keys.T)
            {
                //AddPictureBox(0, 0);
            }

            // ����ͼƬ
            if (e.KeyCode == Keys.C)
            {
                this.Controls.Clear();
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
    }
}
