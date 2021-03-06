using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
// Tflms引用
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

        // 控制内容
        public String fileCmdContent = "";
        // 文件列表
        public String[] fileList = null;
        // 图像资源
        public List<Image> pictures = new List<Image>();
        // 滚动位置
        public int PicPos = 0;


        // LED 显示器开关控制
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll ")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childe, string strclass, string FrmText);

        // 事件常量
        const int WM_SETTEXT = 0x000C;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int WM_CLOSE = 0x0010;

        IntPtr hwnd = FindWindow(null, "大屏控制显示系统");
        // 四分之三·门禁管理系统
        //Create Standalone SDK class dynamicly.
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();

        const int NOGroupIndex = 101; // Normally Open
        const int NCGroupIndex = 102; // Normally Close        

        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        // 置顶功能
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow(); //获得本窗体的句柄
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体
        //定义变量,句柄类型
        public IntPtr Handle1;
        //在窗体加载的时候给变量赋值,即将当前窗体的句柄赋给变量

        /// <summary>
        /// 获取鼠标闲置时间
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
        /// 获取鼠标闲置时间
        /// </summary>
        /// <param name="plii"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        /// <summary>
        /// 设置鼠标状态的计数器（非状态）
        /// </summary>
        /// <param name="bShow">状态</param>
        /// <returns>状态技术</returns>
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public static extern int ShowCursor(bool bShow);

        //鼠标状态计数器
        int iCount = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            // 初始化检查
            InitCheck();

            // 句柄
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
                File.WriteAllText("doorError.txt", "大门没有连接成功，错误代码是：" + idwErrorCode.ToString());
            }
            Cursor = Cursors.Default;

            // 放映器
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.VideoPlayer.Visible = false;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            string appPath = @System.AppDomain.CurrentDomain.BaseDirectory;
            // 退出程序
            if (File.Exists(appPath + "/app_quit"))
            {
                DeleteFile(appPath + "/app_quit");
                System.Environment.Exit(0);
            }

            // 消除Teamviewer窗口
            IntPtr tmw = new IntPtr();
            tmw = FindWindow(null, "发起会话");
            if (tmw != new IntPtr())
            {
                IntPtr okbtn = FindWindowEx(tmw, IntPtr.Zero, null, "确定");
                SetForegroundWindow(okbtn);
                SendMessage(okbtn, WM_LBUTTONDOWN, IntPtr.Zero, null);
                SendMessage(okbtn, WM_LBUTTONUP, IntPtr.Zero, null);
            }

            // 启动放映
            if (File.Exists(appPath + "/led_play"))
            {
                DeleteFile(appPath + "/led_play");
                // 取消一遍
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

            // 滚动放映
            if (File.Exists(appPath + "/led_carousel"))
            {
                DeleteFile(appPath + "/led_carousel");
                // 取消一遍
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

            // 取消放映
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
                //Font font = new Font("宋体", 24, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline, GraphicsUnit.Point);//其中宋体是字体，24是字号，FontStyle的几个是字的样式，最后的GraphicsUnit是字的度量单位（可以看一下上边的表）
                //g.DrawString("四分之三·IT中心", font, Brushes.Brown, new Point(0, this.Height / 2));//绘制字体，font是上边定义的字体，Brushes.Brown是颜色，Point是字从哪个位置开始绘制
                //g.Dispose();

            }

            // 窗口复位
            if (File.Exists(appPath + "/app_max"))
            {
                DeleteFile(appPath + "/app_max");
                SetForegroundWindow(Handle1);
                this.FormBorderStyle = FormBorderStyle.None;
                // this.WindowState = FormWindowState.Maximized;
                this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                this.Left = 0;
                this.Top = 0;
                this.Show();
                this.Activate();
                this.TopMost = true;


            }

            // 屏幕开机
            if (File.Exists(appPath + "/led_open"))
            {
                DeleteFile(appPath + "/led_open");
                IntPtr subWindow = FindWindowEx(hwnd, IntPtr.Zero, null, "");
                IntPtr itbtn = FindWindowEx(subWindow, IntPtr.Zero, null, "开机");
                SetForegroundWindow(itbtn);
                SendMessage(itbtn, WM_LBUTTONDOWN, IntPtr.Zero, null);
                SendMessage(itbtn, WM_LBUTTONUP, IntPtr.Zero, null);

            }

            // 屏幕关机
            if (File.Exists(appPath + "/led_close"))
            {
                DeleteFile(appPath + "/led_close");
                IntPtr subWindow = FindWindowEx(hwnd, IntPtr.Zero, null, "");
                IntPtr itbtn = FindWindowEx(subWindow, IntPtr.Zero, null, "关机");
                SetForegroundWindow(itbtn);
                SendMessage(itbtn, WM_LBUTTONDOWN, IntPtr.Zero, null);
                SendMessage(itbtn, WM_LBUTTONUP, IntPtr.Zero, null);

            }

            // 一键开门
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

            // 开门15秒
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

            // 一键关门
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

        // 初始化检查
        public void InitCheck()
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.Process[] pros = System.Diagnostics.Process.GetProcessesByName(process.ProcessName);
            for (int i = 0; i < pros.Length; i++)
            {
                if (process.Id != pros[i].Id) //筛选掉当前的进程
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
            // 2、根据路径字符串判断是文件还是文件夹
            FileAttributes attr = File.GetAttributes(path);
            // 3、根据具体类型进行删除
            if (attr == FileAttributes.Directory)
            {
                // 3.1、删除文件夹
                Directory.Delete(path, true);
            }
            else
            {
                // 3.2、删除文件
                File.Delete(path);
            }
            File.Delete(path);

        }

        //// 添加图像测试
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
        /// 获取闲置时间
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
            // 置顶和取消置顶
            if (e.KeyCode == Keys.Q)
            {
                this.TopMost = !this.TopMost;
            }

            // 窗体边框
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

            // 窗体最大化
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
            //鼠标状态计数器>=0的情况下鼠标可见，<0不可见，并不是直接受api函数影响而改变
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

        // 文件取得
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
                    //MessageBox.Show("文件不存在");
                    File.WriteAllText("app_log.txt", "文件不存在");
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
