using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TflmsW
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show(System.AppDomain.CurrentDomain.BaseDirectory);

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
            }

            // һ������
            if (File.Exists(appPath + "/closedoor"))
            {
                DeleteFile(appPath + "/closedoor");
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
    }
}
