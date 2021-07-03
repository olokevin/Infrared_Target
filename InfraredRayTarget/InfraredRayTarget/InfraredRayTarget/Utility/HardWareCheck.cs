using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Text.RegularExpressions;

//功能：查询串口设备（直接使用SetupApi）
//特点：查询速度极快，大约20ms
//配置：在工程设置里，取消优先编译32位，否则API调用失败

public class DeviceNodes
{
    public string deviceName;
    public string portName;

    public DeviceNodes(string _devicename, string _portname)
    {
        deviceName = _devicename;
        portName = _portname;
    }
}

public class HardWareCheck
{
    internal const int SPDRP_DEVICEDESC = 0x00000000;
    internal const int SPDRP_CAPABILITIES = 0x0000000F;
    internal const int SPDRP_CLASS = 0x00000007;
    internal const int SPDRP_CLASSGUID = 0x00000008;
    internal const int SPDRP_FRIENDLYNAME = 0x0000000C;
    internal const int SPDRP_HARDWAREID = (0x00000001);  // HardwareID (R/W)


    public const int DIGCF_ALLCLASSES = (0x00000004);
    public const int DIGCF_PRESENT = (0x00000002);

    public const int DIGCF_DEFAULT = 0x00000001;  // only valid with DIGCF_DEVICEINTERFACE
    public const int DIGCF_PROFILE = 0x00000008;
    public const int DIGCF_DEVICEINTERFACE = 0x00000010;

    public static Guid deviceguid = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");//"4d36e978-e325-11ce-bfc1-08002be10318"


    public const int INVALID_HANDLE_VALUE = -1;
    public const int MAX_DEV_LEN = 512;
    public const int DEVICE_NOTIFY_WINDOW_HANDLE = (0x00000000);
    public const int DEVICE_NOTIFY_SERVICE_HANDLE = (0x00000001);
    public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = (0x00000004);

    public const int WM_DEVICECHANGE = (0x0219);
    public const int DIF_PROPERTYCHANGE = (0x00000012);
    public const int DICS_FLAG_GLOBAL = (0x00000001);
    public const int DICS_FLAG_CONFIGSPECIFIC = (0x00000002);
    public const int DICS_ENABLE = (0x00000001);
    public const int DICS_DISABLE = (0x00000002);

    // Win32 constants
    public const int BROADCAST_QUERY_DENY = 0x424D5144;

    public const int DBT_DEVICEARRIVAL = 0x8000; // system detected a new device

    public const int DBT_DEVICEQUERYREMOVE = 0x8001;   // Preparing to remove (any program can disable the removal)
    public const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // removed 
    public const int DBT_DEVTYP_VOLUME = 0x00000002; // drive type is logical volume
    public const int DBT_DEVTYP_PORT = 0x00000005;
    public const int DBT_DEVTYP_DEVICEINTERFACE = (0x00000005);
    public const int DBT_DEVNODES_CHANGED = (0x0007);
    public const int DBT_DEVTYP_HANDLE = 6;
    public const int DBT_CONFIGCHANGECANCELED = 0x0019;
    public const int DBT_CONFIGCHANGED = 0x0018;

    public const int DBT_CUSTOMEVENT = 0x8006;
    public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
    public const int DBT_DEVICEREMOVEPENDING = 0x8003;
    public const int DBT_DEVICETYPESPECIFIC = 0x8005;
    public const int DBT_QUERYCHANGECONFIG = 0x0017;
    public const int DBT_USERDEFINED = 0xFFFF;


    // 注册设备或者设备类型，在指定的窗口返回相关的信息
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, DEV_BROADCAST_DEVICEINTERFACE NotificationFilter, UInt32 Flags);

    // 通过名柄，关闭指定设备的信息。
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

    // 获取一个指定类别或全部类别的所有已安装设备的信息
    [DllImport("setupapi.dll", SetLastError = true)]
    // public static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, string Enumerator, IntPtr hParent, UInt32 nFlags);
    public static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, UInt32 iEnumerator, IntPtr hParent, UInt32 nFlags);

    // 销毁一个设备信息集合，并且释放所有关联的内存
    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern int SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);

    // 枚举指定设备信息集合的成员，并将数据放在SP_DEVINFO_DATA中
    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern bool SetupDiEnumDeviceInfo(IntPtr lpInfoSet, UInt32 dwIndex, SP_DEVINFO_DATA devInfoData);

    // 获取指定设备的属性
    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr lpInfoSet, SP_DEVINFO_DATA DeviceInfoData, UInt32 Property, UInt32 PropertyRegDataType, StringBuilder PropertyBuffer, UInt32 PropertyBufferSize, IntPtr RequiredSize);

    // 停用设备
    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, IntPtr ClassInstallParams, int ClassInstallParamsSize);

    // 启用设备
    [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
    public static extern Boolean SetupDiCallClassInstaller(UInt32 InstallFunction, IntPtr DeviceInfoSet, IntPtr DeviceInfoData);

    // RegisterDeviceNotification所需参数
    [StructLayout(LayoutKind.Sequential)]
    public struct DEV_BROADCAST_HANDLE
    {
        public int dbch_size;
        public int dbch_devicetype;
        public int dbch_reserved;
        public IntPtr dbch_handle;
        public IntPtr dbch_hdevnotify;
        public Guid dbch_eventguid;
        public long dbch_nameoffset;
        public byte dbch_data;
        public byte dbch_data1;
    }

    // 设备类，lParam为一个指向DEV_BROADCAST_DEVICEINTERFACE数据结构的指针
    [StructLayout(LayoutKind.Sequential)]
    public class DEV_BROADCAST_DEVICEINTERFACE
    {
        public int dbcc_size;
        public int dbcc_devicetype;
        public int dbcc_reserved;
        public Guid classguid;
        public string name;
    }

    // 端口设备（串口或者并口），lParam为一指向DEV_BROADCAST_PORT数据结构的指针
    [StructLayout(LayoutKind.Sequential)]
    public class DEV_BROADCAST_PORT
    {
        public int dbcp_size;
        public int dbcp_devicetype;
        public int dbcp_reserved;
        public byte dbcp_name;
    }

    // 文件系统处理，lParam为一个指向DEV_BROADCAST_HANDLE数据结构的指针
    [StructLayout(LayoutKind.Sequential)]
    public class DEV_BROADCAST_HDR
    {
        public int dbch_size;
        public int dbch_devicetype;
        public int dbch_reserved;
    }

    // 逻辑驱动器，lParam为一指向DEV_BROADCAST_VOLUME数据结构的指针
    [StructLayout(LayoutKind.Sequential)]
    public class DEV_BROADCAST_VOLUME
    {
        public int dbcv_size;
        public int dbcv_devicetype;
        public int dbcv_reserved;
        public int dbcv_unitmask;
        public short dbcv_flags;
    }

    // 设备信息数据
    [StructLayout(LayoutKind.Sequential)]
    public class SP_DEVINFO_DATA
    {
        public int cbSize;
        public Guid classGuid;
        public int devInst;
        public ulong reserved;
    }

    // 属性变更参数
    [StructLayout(LayoutKind.Sequential)]
    public class SP_PROPCHANGE_PARAMS
    {
        public SP_CLASSINSTALL_HEADER ClassInstallHeader = new SP_CLASSINSTALL_HEADER();
        public int StateChange;
        public int Scope;
        public int HwProfile;
    }

    // 设备安装
    [StructLayout(LayoutKind.Sequential)]
    public class SP_DEVINSTALL_PARAMS
    {
        public int cbSize;
        public int Flags;
        public int FlagsEx;
        public IntPtr hwndParent;
        public IntPtr InstallMsgHandler;
        public IntPtr InstallMsgHandlerContext;
        public IntPtr FileQueue;
        public IntPtr ClassInstallReserved;
        public int Reserved;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string DriverPath;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SP_CLASSINSTALL_HEADER
    {
        public int cbSize;
        public int InstallFunction;
    }

    public string[] GetDeviceProperty(string portname)
    {
        List<string> HWList = new List<string>();
        try
        {
            Guid myGUID = System.Guid.Empty;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref myGUID, 0, IntPtr.Zero, DIGCF_ALLCLASSES | DIGCF_PRESENT);
            if (hDevInfo.ToInt32() == INVALID_HANDLE_VALUE)
            {
                throw new Exception("Invalid Handle");
            }
            SP_DEVINFO_DATA DeviceInfoData;
            DeviceInfoData = new SP_DEVINFO_DATA();
            DeviceInfoData.cbSize = 28;//(16,4,4,4)
            DeviceInfoData.devInst = 0;
            DeviceInfoData.classGuid = System.Guid.Empty;
            DeviceInfoData.reserved = 0;
            UInt32 i;
            StringBuilder property = new StringBuilder("");
            property.Capacity = MAX_DEV_LEN;
            for (i = 0; SetupDiEnumDeviceInfo(hDevInfo, i, DeviceInfoData); i++)
            {
                SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, (uint)SPDRP_CLASS, 0, property, (uint)property.Capacity, IntPtr.Zero);
                if (property.ToString().ToLower() != "ports")
                {
                    continue;
                }

                SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, (uint)SPDRP_FRIENDLYNAME, 0, property, (uint)property.Capacity, IntPtr.Zero);
                if (!property.ToString().ToLower().Contains(portname.ToLower()))
                    continue;//找到对应于portname的设备
                HWList.Add(property.ToString());
                SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, (uint)SPDRP_HARDWAREID, 0, property, (uint)property.Capacity, IntPtr.Zero);
                HWList.Add(property.ToString());
                break;
            }
            SetupDiDestroyDeviceInfoList(hDevInfo);

        }
        catch (Exception ex)
        {
            throw new Exception("枚举设备列表出错", ex);
        }
        return HWList.ToArray();
    }

    public static List<DeviceNodes> GetAllList()
    {
        List<DeviceNodes> HWList = new List<DeviceNodes>();
        IntPtr hDevInfo = IntPtr.Zero;
        try
        {
            Guid myGUID = System.Guid.Empty;
            hDevInfo = SetupDiGetClassDevs(ref myGUID, 0, IntPtr.Zero, DIGCF_ALLCLASSES | DIGCF_PRESENT);
            if (hDevInfo.ToInt32() == INVALID_HANDLE_VALUE)
            {
                throw new Exception("Invalid Handle");
            }
            SP_DEVINFO_DATA DeviceInfoData;
            DeviceInfoData = new SP_DEVINFO_DATA();
            DeviceInfoData.cbSize = 28;//(16,4,4,4)
            DeviceInfoData.devInst = 0;
            DeviceInfoData.classGuid = System.Guid.Empty;
            DeviceInfoData.reserved = 0;
            UInt32 i;
            StringBuilder DeviceName = new StringBuilder("");
            DeviceName.Capacity = MAX_DEV_LEN;

            StringBuilder HardWare = new StringBuilder("");
            HardWare.Capacity = MAX_DEV_LEN;

            for (i = 0; SetupDiEnumDeviceInfo(hDevInfo, i, DeviceInfoData); i++)
            {
                //获取friendlyname
                if (!SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, SPDRP_FRIENDLYNAME, 0, DeviceName, MAX_DEV_LEN, IntPtr.Zero))
                {
                    continue;
                }

                //获取设备pid和vid号
                if (!SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, SPDRP_HARDWAREID, 0, HardWare, MAX_DEV_LEN, IntPtr.Zero))
                {
                    continue;
                }

                if (HardWare.ToString().Contains("VID_FFF0") && HardWare.ToString().Contains("PID_0008"))   //UWB
                {
                    int num = Utility.GetSerialPortNumFromString(DeviceName.ToString());
                    HWList.Add(new DeviceNodes("uwb", string.Format("COM{0}", num)));
                }
                else if (HardWare.ToString().Contains("VID_0403") && HardWare.ToString().Contains("PID_6015"))  //电调
                {
                    int num = Utility.GetSerialPortNumFromString(DeviceName.ToString());
                    HWList.Add(new DeviceNodes("motor", string.Format("COM{0}", num)));
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("枚举设备列表出错", ex);
        }

        finally
        {
            if (hDevInfo != IntPtr.Zero)
                SetupDiDestroyDeviceInfoList(hDevInfo);
        }
        return HWList;
    }

    public static List<int> GetSerialPorts()
    {
        List<int> HWList = new List<int>();
        IntPtr hDevInfo = IntPtr.Zero;
        StringBuilder sb = new StringBuilder();
        try
        {
            Guid myGUID = System.Guid.Empty;
            hDevInfo = SetupDiGetClassDevs(ref myGUID, 0, IntPtr.Zero, DIGCF_ALLCLASSES | DIGCF_PRESENT);
            if (hDevInfo.ToInt64() == INVALID_HANDLE_VALUE)
            {
                throw new Exception("Invalid Handle");
            }
            SP_DEVINFO_DATA DeviceInfoData;
            DeviceInfoData = new SP_DEVINFO_DATA();

            /*************************************************
            * 分析原因： 是32位和64位系统差异造成。
            *
            * 解决办法：判断是否为64位系统。
            *************************************************/
            if (Environment.Is64BitOperatingSystem)
                DeviceInfoData.cbSize = 32;//(16,4,4,4)
            else
                DeviceInfoData.cbSize = 28;

            DeviceInfoData.devInst = 0;
            DeviceInfoData.classGuid = System.Guid.Empty;
            DeviceInfoData.reserved = 0;
            UInt32 i;
            StringBuilder DeviceName = new StringBuilder("");
            DeviceName.Capacity = MAX_DEV_LEN;

            StringBuilder Property = new StringBuilder("");
            Property.Capacity = MAX_DEV_LEN;

            for (i = 0; SetupDiEnumDeviceInfo(hDevInfo, i, DeviceInfoData); i++)
            {
                //判断是否是串口类型
                SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, SPDRP_CLASS, 0, Property, MAX_DEV_LEN, IntPtr.Zero);
                if (Property.ToString().ToLower() != "ports")
                    continue;

                //获取friendlyname
                if (!SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, SPDRP_FRIENDLYNAME, 0, DeviceName, MAX_DEV_LEN, IntPtr.Zero))
                {
                    continue;
                }

                //获取COM口
                string friendlyName = DeviceName.ToString();
                string pattern = @"\(COM[1-9][0-9]?\)$";//friendlyName一般形式为以(COMn)结尾，n为1-99
                if (Regex.IsMatch(friendlyName, pattern, RegexOptions.IgnoreCase))
                {
                    friendlyName = Regex.Match(friendlyName, pattern, RegexOptions.IgnoreCase).Value;
                    int num = Utility.GetSerialPortNumFromString(DeviceName.ToString());
                    HWList.Add(num);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("枚举设备列表出错 serialports: ", ex);
        }

        finally
        {
            if (hDevInfo != IntPtr.Zero)
                SetupDiDestroyDeviceInfoList(hDevInfo);
        }
        return HWList;
    }

    public static string[] GetStringSerialPorts()
    {
        List<string> HWList = new List<string>();
        IntPtr hDevInfo = IntPtr.Zero;
        StringBuilder sb = new StringBuilder();
        try
        {
            Guid myGUID = System.Guid.Empty;
            hDevInfo = SetupDiGetClassDevs(ref myGUID, 0, IntPtr.Zero, DIGCF_ALLCLASSES | DIGCF_PRESENT);
            if (hDevInfo.ToInt64() == INVALID_HANDLE_VALUE)
            {
                throw new Exception("Invalid Handle");
            }
            SP_DEVINFO_DATA DeviceInfoData;
            DeviceInfoData = new SP_DEVINFO_DATA();

            /*************************************************
            * 分析原因： 是32位和64位系统差异造成。
            *
            * 解决办法：判断是否为64位系统。
            *************************************************/
            if (Environment.Is64BitOperatingSystem)
                DeviceInfoData.cbSize = 32;//(16,4,4,4)
            else
                DeviceInfoData.cbSize = 28;

            DeviceInfoData.devInst = 0;
            DeviceInfoData.classGuid = System.Guid.Empty;
            DeviceInfoData.reserved = 0;
            UInt32 i;
            StringBuilder DeviceName = new StringBuilder("");
            DeviceName.Capacity = MAX_DEV_LEN;

            StringBuilder Property = new StringBuilder("");
            Property.Capacity = MAX_DEV_LEN;

            for (i = 0; SetupDiEnumDeviceInfo(hDevInfo, i, DeviceInfoData); i++)
            {
                //判断是否是串口类型
                SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, SPDRP_CLASS, 0, Property, MAX_DEV_LEN, IntPtr.Zero);
                if (Property.ToString().ToLower() != "ports")
                    continue;

                //获取friendlyname
                if (!SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, SPDRP_FRIENDLYNAME, 0, DeviceName, MAX_DEV_LEN, IntPtr.Zero))
                {
                    continue;
                }

                //获取COM口
                string friendlyName = DeviceName.ToString();
                string pattern = @"\(COM[1-9][0-9]?\)$";//friendlyName一般形式为以(COMn)结尾，n为1-99
                //if (Regex.IsMatch(friendlyName, pattern, RegexOptions.IgnoreCase))
                //{
                //    friendlyName = Regex.Match(friendlyName, @"COM[1-9][0-9]?", RegexOptions.IgnoreCase).Value;
                //    HWList.Add(friendlyName);
                //}
                HWList.Add(friendlyName);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("枚举设备列表出错 serialports: ", ex);
        }

        finally
        {
            if (hDevInfo != IntPtr.Zero)
                SetupDiDestroyDeviceInfoList(hDevInfo);
        }
        return HWList.ToArray();
    }

    public static string[] GetStringSerialPortsOnlyCOM()
    {
        List<string> HWList = new List<string>();
        IntPtr hDevInfo = IntPtr.Zero;
        StringBuilder sb = new StringBuilder();
        try
        {
            Guid myGUID = System.Guid.Empty;
            hDevInfo = SetupDiGetClassDevs(ref myGUID, 0, IntPtr.Zero, DIGCF_ALLCLASSES | DIGCF_PRESENT);
            if (hDevInfo.ToInt64() == INVALID_HANDLE_VALUE)
            {
                throw new Exception("Invalid Handle");
            }
            SP_DEVINFO_DATA DeviceInfoData;
            DeviceInfoData = new SP_DEVINFO_DATA();

            /*************************************************
            * 分析原因： 是32位和64位系统差异造成。
            *
            * 解决办法：判断是否为64位系统。
            *************************************************/
            if (Environment.Is64BitOperatingSystem)
                DeviceInfoData.cbSize = 32;//(16,4,4,4)
            else
                DeviceInfoData.cbSize = 28;

            DeviceInfoData.devInst = 0;
            DeviceInfoData.classGuid = System.Guid.Empty;
            DeviceInfoData.reserved = 0;
            UInt32 i;
            StringBuilder DeviceName = new StringBuilder("");
            DeviceName.Capacity = MAX_DEV_LEN;

            StringBuilder Property = new StringBuilder("");
            Property.Capacity = MAX_DEV_LEN;

            for (i = 0; SetupDiEnumDeviceInfo(hDevInfo, i, DeviceInfoData); i++)
            {
                //判断是否是串口类型
                SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, SPDRP_CLASS, 0, Property, MAX_DEV_LEN, IntPtr.Zero);
                if (Property.ToString().ToLower() != "ports")
                    continue;

                //获取friendlyname
                if (!SetupDiGetDeviceRegistryProperty(hDevInfo, DeviceInfoData, SPDRP_FRIENDLYNAME, 0, DeviceName, MAX_DEV_LEN, IntPtr.Zero))
                {
                    continue;
                }

                //获取COM口
                string friendlyName = DeviceName.ToString();
                string pattern = @"\(COM[1-9][0-9]?\)$";//friendlyName一般形式为以(COMn)结尾，n为1-99
                if (Regex.IsMatch(friendlyName, pattern, RegexOptions.IgnoreCase))
                {
                    friendlyName = Regex.Match(friendlyName, @"COM[1-9][0-9]?", RegexOptions.IgnoreCase).Value;
                    HWList.Add(friendlyName);
                }
                HWList.Add(friendlyName);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("枚举设备列表出错 serialports: ", ex);
        }

        finally
        {
            if (hDevInfo != IntPtr.Zero)
                SetupDiDestroyDeviceInfoList(hDevInfo);
        }
        return HWList.ToArray();
    }

    // 允许一个窗口或者服务接收所有硬件的通知
    // 注:目前还没有找到一个比较好的方法来处理如果通知服务。
    public static bool AllowNotifications(IntPtr callback)
    {
        try
        {
            DEV_BROADCAST_DEVICEINTERFACE dbdi = new DEV_BROADCAST_DEVICEINTERFACE();
            dbdi.dbcc_size = Marshal.SizeOf(dbdi);
            dbdi.dbcc_reserved = 0;
            dbdi.dbcc_devicetype = DBT_DEVTYP_PORT;
            dbdi.classguid = deviceguid;

            RegisterDeviceNotification(callback, dbdi, DEVICE_NOTIFY_ALL_INTERFACE_CLASSES | DEVICE_NOTIFY_WINDOW_HANDLE);
            return true;
        }
        catch (Exception ex)
        {
            string err = ex.Message;
            return false;
        }
    }

    public bool AllowUnNotifications(IntPtr callback, bool UseWindowHandle)
    {
        try
        {
            UnregisterDeviceNotification(callback);
            return true;
        }
        catch (Exception ex)
        {
            string err = ex.Message;
            return false;
        }
    }
}
