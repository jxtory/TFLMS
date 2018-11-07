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

namespace TflmsW
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // �ķ�֮�����Ž�����ϵͳ
        //Create Standalone SDK class dynamicly.
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();

        const int NOGroupIndex = 101; // Normally Open
        const int NCGroupIndex = 102; // Normally Close        

        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.


        private void Form1_Load(object sender, EventArgs e)
        {
            // ��ʼ�����
            InitCheck();
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
                if (axCZKEM1.ACUnlock(iMachineNumber,iDelay))
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
                if (axCZKEM1.ACUnlock(iMachineNumber,iDelay))
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
                if (axCZKEM1.ACUnlock(iMachineNumber,iDelay))
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
            DeleteFiles("app_quit");
            DeleteFiles("opendoor");
            DeleteFiles("opendoor15");
            DeleteFiles("closedoor");
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

        public void AddPictureBox(int left, int top)
        {
            PictureBox pic = new PictureBox();
            this.Controls.Add(pic);
            pic.Left = left;
            pic.Top = top;
            pic.Image = Image.FromFile(@"E:\\86d6277f9e2f070891004d53e424b899a901f258.jpg");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // �ö���ȡ���ö�
            if(e.KeyCode == Keys.Q)
            {
                this.TopMost = !this.TopMost;
            }

            // ����߿�
            if(e.KeyCode == Keys.B)
            {
                if(this.FormBorderStyle == FormBorderStyle.Sizable)
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                } else
                {
                    this.Width = 800;
                    this.Height = 600;
                    this.Left = 0;
                    this.Top = 0;
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                }
            }

            // �������
            if(e.KeyCode == Keys.M)
            {
                if(this.FormBorderStyle == FormBorderStyle.None)
                {
                    this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                    this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                    this.Left = 0;
                    this.Top = 0;
                }
            }

            // ����ͼƬ
            if(e.KeyCode == Keys.T)
            {
                AddPictureBox(0, 0);
            }
        }
    }
}
