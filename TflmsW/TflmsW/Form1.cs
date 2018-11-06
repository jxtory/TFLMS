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
            // 退出程序
            if (File.Exists(appPath + "/app_quit"))
            {
                DeleteFile(appPath + "/app_quit");
                System.Environment.Exit(0);
            }

            // 一键开门
            if (File.Exists(appPath + "/opendoor"))
            {
                DeleteFile(appPath + "/opendoor");
            }

            // 一键关门
            if (File.Exists(appPath + "/closedoor"))
            {
                DeleteFile(appPath + "/closedoor");
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
    }
}
