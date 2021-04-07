using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SpiderTracker.Imp
{
    public class LogUtil
    {
        private static Mutex mutex = new Mutex();
        public static void Debug(string message)
        {
            Logger(false, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}[{ThreadId()}]>{message}");
        }

        public static void Info(string message)
        {
            Logger(false, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}[{ThreadId()}]>{message}");
        }

        public static void Test(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Logger(false, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}[{ThreadId()}]>{message}");
        }

        public static void Error(string message)
        {
            Logger(true, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}[{ThreadId()}]>{message}");
        }

        public static void Error(Exception ex, string message = null)
        {
            if (ex.InnerException != null) ex = ex.InnerException;
            if (message == null) message = ex.Message;

            var messages = new string[] {
                $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}[{ThreadId()}]>{message}",
                $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}[{ThreadId()}]>{ex.StackTrace}" };

            Logger(true, messages);
        }

        static void Logger(bool error, params string[] messages)
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

            foreach (var message in messages)
            {
                if (error)
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            try
            {
                mutex.WaitOne();
                var logFile = Path.Combine(logPath, $"{(error ? "err" : "log")}{DateTime.Now.ToString("yyyyMMdd")}.log");
                File.AppendAllLines(logFile, messages);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        static string ThreadId()
        {
            return $"Thd:{Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(4, '0')}";
        }
    }
}
