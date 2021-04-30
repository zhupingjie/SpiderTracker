using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SpiderCore.Util
{
    public class CmdUtil
    {
        public static void RunCmd(string exe, string command)
        {
            //Process p = new Process();
            //p.StartInfo.Verb = "runas";
            //p.StartInfo.FileName = "cmd.exe";
            //p.StartInfo.UseShellExecute = false;
            //p.StartInfo.RedirectStandardInput = true;
            //p.StartInfo.RedirectStandardOutput = true;
            //p.StartInfo.RedirectStandardError = true;
            //p.StartInfo.CreateNoWindow = true;
            //p.Start();
            //p.StandardInput.WriteLine(command);
            //p.StandardInput.WriteLine("exit");
            //return p.StandardError.ReadToEnd();

            Process p = new Process();//建立外部调用线程
            p.StartInfo.FileName = exe;//要调用外部程序的绝对路径
            p.StartInfo.Arguments = command;
            p.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动线程(一定为FALSE,详细的请看MSDN)
            p.StartInfo.RedirectStandardError = true;//把外部程序错误输出写到StandardError流中(这个一定要注意,FFMPEG的所有输出信息,都为错误输出流,用StandardOutput是捕获不到任何消息的...这是我耗费了2个多月得出来的经验...mencoder就是用standardOutput来捕获的)
            p.StartInfo.CreateNoWindow = true;//不创建进程窗口
            //p.ErrorDataReceived += new DataReceivedEventHandler(Output);//外部程序(这里是FFMPEG)输出流时候产生的事件,这里是把流的处理过程转移到下面的方法中,详细请查阅MSDN
            p.Start();//启动线程
            p.BeginErrorReadLine();//开始异步读取
            p.WaitForExit();//阻塞等待进程结束
            p.Close();//关闭进程
            p.Dispose();//释放资源
        }

        //public static void RunCmd2(string exe, string args)
        //{
        //    using (Process myPro = new Process())
        //    {
        //        ProcessStartInfo psi = new ProcessStartInfo(exe, args);
        //        myPro.StartInfo = psi;
        //        myPro.Start();
        //        myPro.WaitForExit();
        //    }
        //}

        public static string DeleteShareUserConn(string path)
        {
            Process proc = new Process();
            string dosLine = @"net use " + path + " /del";
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg)) return null;

                return $"{dosLine} -> {errormsg}";
            }
            catch(Exception ex)
            {
                return $"{dosLine} -> {ex.Message}";
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
        }

        public static void OpenShareDir(string path, string userName, string passWord)
        {
            string dosLine = @"net use " + path + " /User:" + userName + " " + passWord + " /PERSISTENT:YES";
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput=true;
                proc.StartInfo.RedirectStandardError=true;
                proc.StartInfo.CreateNoWindow=true;
                proc.Start();
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (!string.IsNullOrEmpty(errormsg))
                {
                    throw new Exception($"{errormsg}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{dosLine} -> {ex.Message}");
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
        }
    }
}
