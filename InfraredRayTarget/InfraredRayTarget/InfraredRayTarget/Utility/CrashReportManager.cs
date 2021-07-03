using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CrashReportManager
{
    private const string CCrashFolder = "./CrashReport";

    public static void InitCrashReport()
    {
        if (!Directory.Exists(CCrashFolder))
        {
            Directory.CreateDirectory(CCrashFolder);
        }
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = e.ExceptionObject as Exception;
        DateTime date = DateTime.Now;
        string strDateTime = string.Format("{0:D4}-{1:D2}-{2:D2}-{3:D2}-{4:D2}-{5:D2}", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        string fileName = string.Format("{0}/CrashLog{1}.txt", CCrashFolder, strDateTime);
        FileStream stream = File.Open(fileName, FileMode.Create);
        StreamWriter writer = new StreamWriter(stream);
        writer.WriteLine("{0}", ex.GetType());
        writer.WriteLine(ex.Message);
        writer.WriteLine(ex.StackTrace);
        writer.Close();
        stream.Close();
    }
}
