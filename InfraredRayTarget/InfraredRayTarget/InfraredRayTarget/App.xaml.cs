using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace InfraredRayTarget
{
    public partial class App : Application
    {
        private EventLoop m_mainEvloop;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CrashReportManager.InitCrashReport();
            Process process = SingleProcess.GetRunningProcess();
            if (process != null)
            {
                SingleProcess.SetHandleRunning(process);
                Process.GetCurrentProcess().Kill();
                return;
            }
#if DEBUG
            Debug.CreateConsole();
            Debug.LogSuc("Start Console Success");
#endif
            ServiceManager serviceMgr = ServiceManager.Instance;
            serviceMgr.Init();
            serviceMgr.Run();
            m_mainEvloop = serviceMgr.m_serviceArr[ServiceID.UI].EventLoop;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();
            Dispatcher.Hooks.DispatcherInactive += OnRunUIEventLoop;
        }

        private void OnRunUIEventLoop(object sender, EventArgs e)
        {
            m_mainEvloop.RunInMainThread();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceManager.Instance.ExitAllThread();
#if DEBUG
            Debug.ReleaseConsole();
#endif
            base.OnExit(e);
        }
    }
}
