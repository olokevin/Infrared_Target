using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Utility
{
    public static void PostEvent(BaseEvent ev)
    {
        ServiceManager.Instance.m_serviceArr[ServiceID.UI].PostEvent(ev);
    }

    public static void PostEvent(int serviceId, BaseEvent ev)
    {
        ServiceManager.Instance.m_serviceArr[serviceId].PostEvent(ev);
    }

    public static string GetComNameWithNumber(string p)
    {
        string pattern = @"\(COM[1-9][0-9]?\)$";
        if (Regex.IsMatch(p, pattern, RegexOptions.IgnoreCase))
        {
            p = Regex.Match(p, @"COM[1-9][0-9]?", RegexOptions.IgnoreCase).Value;
        }
        return p;
    }

    public static int GetSerialPortNumFromString(string p)
    {
        int numLen = 0;
        int index = p.Length - 2;
        while (p[index] >= '0' && p[index] <= '9')
        {
            --index;
            ++numLen;
        }

        string strNum = p.Substring(index + 1, numLen);
        int num = Convert.ToInt32(strNum);
        return num;
    }

    public static int GetSerialPortNumFromCOMx(string p)
    {
        int numLen = p.Length - 3;
        int index = 3;

        string strNum = p.Substring(index, numLen);
        int num = Convert.ToInt32(strNum);
        return num;
    }

    public static object BytesToStruct(byte[] bytes, int startIndex, Type type)
    {

        int size = Marshal.SizeOf(type);
        if (size > bytes.Length - startIndex) return null;
        IntPtr structPtr = Marshal.AllocHGlobal(size);
        Marshal.Copy(bytes, startIndex, structPtr, size);
        object obj = Marshal.PtrToStructure(structPtr, type);
        Marshal.FreeHGlobal(structPtr);
        return obj;
    }

    public static Byte[] StructToBytes(object structObj)
    {
        int size = Marshal.SizeOf(structObj);
        byte[] data = new byte[size];

        IntPtr localPtr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(structObj, localPtr, false);
        Marshal.Copy(localPtr, data, 0, size);
        Marshal.FreeHGlobal(localPtr);
        return data;
    }

    public static byte[] GetMd5(byte[] data, int length)
    {
        MD5 md5 = MD5.Create();
        byte[] res = md5.ComputeHash(data, 0, length);
        return res;
    }

    public static string MakeLongVersionStr(UInt32 ver)
    {
        string res = String.Format("{0}.{1}.{2}.{3}", (((ver) >> 24) & 0xFF), (((ver) >> 16) & 0xFF), (((ver) >> 8) & 0xFF), ((ver) & 0xFF));
        return res;
    }

    public static UInt32 MakeLongVersionInt(string arg)
    {
        UInt32 res = 0;
        string[] arrystr = arg.Split('.');
        foreach (var item in arrystr)
        {
            res <<= 8;
            UInt32 tmp = Convert.ToUInt32(item);
            res |= tmp;
        }
        return res;
    }

    //获取硬件码
    public static HardWareCode GetHardwareCode(UInt32 data)
    {
        HardWareCode code = new HardWareCode();
        code.moduleType = (byte)((data & 0xFF000000) >> 24);
        code.subtype = (byte)((data & 0x00FF0000) >> 16);
        code.mcu = (byte)((data & 0x0000FF00) >> 8);
        code.mcuVersion = (byte)((data & 0x000000FF));
        return code;
    }

    public static bool ByteArrayEqual(byte[] t1, byte[] t2)
    {
        if (t1 == null || t2 == null) return false;
        if (t1.Length != t2.Length) return false;
        for (int i = 0; i < t1.Length; ++i)
        {
            if (t1[i] != t2[i]) return false;
        }
        return true;
    }
}
