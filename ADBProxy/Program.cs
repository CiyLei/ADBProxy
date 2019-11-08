using System;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace ADBProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string p = args[0];
                string[] arr = p.Split(":".ToCharArray());
                if (arr.Length > 1)
                {
                    string adbShell = HttpUtility.UrlDecode(arr[1].Trim('/'), System.Text.Encoding.GetEncoding(936));
                    string[] shells = adbShell.Split(';');
                    for (int i = 0; i < shells.Length; i++)
                    {
                        string shell = DecodeBase64("utf-8", shells[i]);
                        //Console.WriteLine(shell);
                        ExecuteInCmd(System.AppDomain.CurrentDomain.BaseDirectory + "adb.exe " + shell);
                    }
                    //Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory + "adb.exe shell " + adbShell);
                }
            }
            //Console.Read();
        }

        ///解码
        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }

        /// <summary>
        /// 执行内部命令（cmd.exe 中的命令）
        /// </summary>
        /// <param name="cmdline">命令行</param>
        /// <returns>执行结果</returns>
        public static string ExecuteInCmd(string cmdline)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine(cmdline + "&exit");

                //获取cmd窗口的输出信息  
                string output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();
                process.Close();

                return output;
            }
        }
    }
}
