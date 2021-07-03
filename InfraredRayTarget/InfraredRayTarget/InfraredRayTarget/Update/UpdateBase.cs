using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public enum ModuleUpgradeStatus
{
    NotUpgrade = 0,
    WaitUpgrade,
    UpgradeProcessing,
    UpgradeSuccess,
    UpgradeFail
}

//配置表项
public class UpdateConfigItem
{
    public byte moduleType;
    public string name;
    public string version;
    public string filePath;
    public bool encrypt;
}

//升级文件项（固件路径 MD5等）
public class FirmwareContent
{
    public byte moduleType;
    public string filePath;
    public byte[] fileDataMd5;
    public byte[] fileData;
    public UInt32 encrypt;

    public IMAGE_HEADER fileheader;   //加密的文件头
    public IMAGE_TAIL filetailor;   //加密的文件尾
}

//升级状态
public enum UpdateModuleStatus
{
    STEP_INIT = 0,
    STEP_WAITING,
    STEP_REQ_HARDWARECODE,
    STEP_SEND_HEADER,
    STEP_REQ_UPDATE,
    STEP_REQ_DATA,
    STEP_REQ_END,
    STEP_COMPLETED
}

//升级结果
public enum UpdateResult
{
    //成功
    Success,

    //没有配置文件
    Failed_NoConfigFile,

    //未知失败
    Failed_Unknow,

    //协议返回retCode错误
    Failed_RetCodeError,

    //用户取消
    Failed_UserCancel,

    //文件路径出错
    Failed_FilePathError,

    //文件打开失败
    Failed_OpenFileError,

    //端口出错（无法发送，或端口不存在）
    Failed_PortError,

    //发送超时
    Failed_TimeOut,

    //校验失败
    Failed_CheckSumError,

    //升级未完成，结果无效
    Invalid
}

public class UpdateRobotItem
{
    //public Robot robot;
    //public bool bUpdating;
    //public List<Module> listPrepare;
    //public List<Module> listUpdating;
    //public List<Module> listEnds;
}

//硬件码
public class HardWareCode
{
    public byte moduleType;
    public byte subtype;
    public byte mcu;
    public byte mcuVersion;
}

//设备名称字典(不唯一)
public class RbModuleType
{
    public const string MOTOR = "motor";                  //C620
    public const string MOTOR_C610 = "motorC610";         //C610
    public const string UWB = "uwb";
    public const string UWB_REF = "uwb_ref";
    public const string MAINBOARD = "mainboard";
    public const string STRIKE = "strike";
    public const string BLOODBAR = "bloodbar";
    public const string SPEEDDETECT = "speedDetect";
    public const string RFID = "RFID";
    public const string CAMERA = "camera";
    public const string VIDEOTRANSFER = "videotransfer";
    public const string WIRELESS = "wireless";
    public const string ROBOTCONTROL = "robotcontrol";
    public const string BLOODPOOL = "bloodpool";
    public const string TRAP = "trap";
    public const string AIRCRAFTTRIGGER = "aircrafttrigger";
    public const string POWERBORDER = "powerboard";
    public const string PC = "pc";


    //设备名称字典(不唯一)
    public static Dictionary<byte, string> dic = new Dictionary<byte, string>()
    {
        {0x01,  MAINBOARD},
        {0x02,  STRIKE},
        {0x03,  SPEEDDETECT},
        {0x04,  RFID},
        {0x05,  BLOODBAR},
        {0x06,  CAMERA},
        {0x07,  VIDEOTRANSFER},
        {0x08,  WIRELESS},
        {0x09,  MOTOR},
        {0x0A,  ROBOTCONTROL},
        {0x0B,  BLOODPOOL},
        {0x0C,  TRAP },
        {0x0D,  AIRCRAFTTRIGGER },
        {0x0E,  UWB },
        {0x0F,  POWERBORDER },
        {0x80,  PC}
    };

    public static Dictionary<byte, string> dic_dynamicResKey = new Dictionary<byte, string>()
    {
        {0x01,  "MainModule_FH"},
        {0x02,  "ArmorModule_FH"},
        {0x03,  "SpeedometerModule_FH"},
        {0x04,  "Rfid_FH"},
        {0x05,  "LightbarModule_FH"},
        {0x06,  "VideoModule_FH"},
        {0x07,  VIDEOTRANSFER},
        {0x08,  "WIFI_FH"},
        {0x09,  "Motor_FH"},
        {0x0A,  ROBOTCONTROL},
        {0x0B,  BLOODPOOL},
        {0x0C,  TRAP },
        {0x0D,  AIRCRAFTTRIGGER },
        {0x0E,  "UWB_FH" },
        {0x0F,  "PowerPanel_FH" },
        {0x80,  PC}
    };
}
