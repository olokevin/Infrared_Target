using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SingleProcess
{
    [DllImport("User32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
    [DllImport("User32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    public static Process GetRunningProcess()
    {
        Process current = Process.GetCurrentProcess();
        Process[] processes = Process.GetProcessesByName(current.ProcessName);
        foreach (Process cur in processes)
        {
            if (cur.Id != current.Id)
            {
                if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName) return cur;
            }
        }
        return null;
    }

    public static void SetHandleRunning(Process pro)
    {
        ShowWindowAsync(pro.MainWindowHandle, 1);
        SetForegroundWindow(pro.MainWindowHandle);
    }
}
