using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public static class Debug
{
    [DllImport("kernel32.dll")]
    private static extern Boolean AllocConsole();
    [DllImport("kernel32.dll")]
    private static extern Boolean FreeConsole();

    private enum MSG_COLOR { GRAY, GREEN, YELLOW, RED };

    public static void CreateConsole()
    {
#if DEBUG
        AllocConsole();
#endif
    }

    public static void ReleaseConsole()
    {
#if DEBUG
        FreeConsole();
#endif
    }

    public static void Log()
    {
#if DEBUG
        Console.WriteLine();
#endif
    }

    public static void Log(string msg, params object[] args)
    {
#if DEBUG
        WriteMessage(MSG_COLOR.GRAY, msg, args);
#endif
    }

    public static void LogSuc(string msg, params object[] args)
    {
#if DEBUG
        WriteMessage(MSG_COLOR.GREEN, msg, args);
#endif
    }

    public static void LogWarning(string msg, params object[] args)
    {
#if DEBUG
        WriteMessage(MSG_COLOR.YELLOW, msg, args);
#endif
    }

    public static void LogError(string msg, params object[] args)
    {
#if DEBUG
        WriteMessage(MSG_COLOR.RED, msg, args);
#endif
    }

    private static void WriteMessage(MSG_COLOR color, string msg, params object[] args)
    {
        switch (color)
        {
            case MSG_COLOR.GRAY:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            case MSG_COLOR.GREEN:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case MSG_COLOR.YELLOW:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case MSG_COLOR.RED:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
        }
        Console.WriteLine(msg, args);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}
