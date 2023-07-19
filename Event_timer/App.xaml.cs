using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Event_timer
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 전역 예외 처리기 등록
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        // 전역 예외 처리기 메서드
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                // 로그 기록
                Log.Instance.WriteLog("Unhandled Exception: " + exception.Message);
            }
        }
    }
}
