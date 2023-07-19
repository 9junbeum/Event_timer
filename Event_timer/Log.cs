using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Event_timer
{
    public class Log
    {
        private static readonly Log instance = new Log();
        private static readonly string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly string logFilePath = Path.Combine(logDirectory, "Event_Timer_Log.txt");

        private Log()
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        public static Log Instance
        {
            get { return instance; }
        }

        public void WriteLog(string message)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine($"[{DateTime.Now}] {message}");
                }
            }
            catch (Exception ex)
            {
                // 로그 파일 기록 중 예외가 발생한 경우에 대한 예외 처리 로직
                Console.WriteLine("로그 파일 기록 중 예외 발생: " + ex.Message);
            }
        }
    }
}
