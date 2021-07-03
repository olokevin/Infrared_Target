using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class TargetProto
{
    public UInt16 x;
    public UInt16 y;
    public byte type;//0x01小弹丸 0x02大弹丸

}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class TargetFault
{
    public byte axle;//0x01为x 0x02为y
    public byte index;//故障灯珠 第index颗（从各轴原点数）
    public byte innerdata1;//保留
    public byte innerdata2;//保留
    public byte innerdata3;//保留
}



//升级Ack错误码类型
public enum error_status_e
{
    BL_ERROR_NONE,
    BL_ERROR_FIRMWARE_NOT_EXIST,
    BL_ERROR_FIRMWARE_RECV_TIMEOUT,
    BL_ERROR_FIRMWARE_CHECK_FAILED,
    BL_ERROR_FLASH_OPERTE_FAILED,
    BL_ERROR_FRAME_UNKNOWN,
    BL_ERROR_FIRMWARE_SIZE_INVALID,
    BL_ERROR_NOKNOWN,
    BL_ERROR_WRITE_ADDR_ATYPISM,
    BL_ERROR_HARDWARE_CODE_FAILED,
    BL_ERROR_SUCCESS_CHECK = 0xFF,
}


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct T_ROBOT
{
    public byte type;
    public byte id;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct T_MODULE
{
    public byte type;
    public byte id;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct T_HEADER
{
    public byte flag;
    public UInt16 length;
    public byte seq_num;
    public byte crc8;
    public UInt16 cmd_Id;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct T_TAIL
{
    public UInt16 checkSum;
}

//CMD_ID_QUERY_DEVICE_INFO = 0x80,        //读取设备信息
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct ACK_QEURYINFO
{
    public UInt32 loader_version;
    public UInt32 app_version;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] deviceId;
    public UInt32 reserved;
}

//CMD_ID_UPGRADE_START = 0x81,            //请求升级
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct REQ_UPDATE_START
{
    public UInt32 length;              //固件长度（bin原始文件长度）
    public UInt32 encrypt_type;        //加密类型，0 无加密，1 AES128
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] deviceId;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct ACK_UPDATE_START
{
    public byte retCode;               //返回码，返回0表示已准备成功，可以开始传输数据，非零表示错误代码
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] deviceId;
}

//CMD_ID_UPGRADE_DATA = 0x82,             //升级数据
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct REQ_UPDATE_DATA
{
    public UInt32 sequence;        //序号，从0开始
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public byte[] data;            //升级数据
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] deviceId;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct ACK_UPDATE_DATA
{
    public byte retCode;           //返回值，非0表示错误
    public UInt32 sequence;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] deviceId;
}

//CMD_ID_UPGRADE_END = 0x83,              //升级成功
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct REQ_UPDATE_END
{
    public byte verify_type;       //校验类型，0无校验，1 MD5校验
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] verify_data;     //校验数据，MD5
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] deviceId;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct ACK_UPDATE_END
{
    public byte recCode;           //返回码， 0成功，1校验错误
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] deviceId;
}

//CMD_ID_CONTROL_ROBOT = 0x02				//找车命令
//主命令是0x02号，opt_code为5号命令，其他字段为0
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct CONTROL_CMD_T
{
    public UInt16 opt_code;     //命令码
    public UInt16 data;         //命令参数
    public byte cmdNum;			//命令序号
}

//新的升级协议加密包头
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
public struct IMAGE_HEADER
{
    public UInt32 image_format_version;
    public UInt32 app_version;
    public UInt32 app_startup_address;
    public UInt32 app_param_address;
    public UInt32 image_type;
    public UInt32 image_write_addr;
    public UInt32 image_size;
    public UInt32 hardware_code;
    #region _HARDWARE_CODE_DETAIL_

    public byte ModuleType
    {
        get
        {
            return (byte)(hardware_code >> 24);
        }
        set
        {
            hardware_code = (hardware_code & 0x0FFF) | (((UInt32)TargetPeripheral) << 24);
        }
    }
    public byte ModuleSubType
    {
        get
        {
            return (byte)(hardware_code >> 16);
        }
        set
        {
            hardware_code = (hardware_code & 0xF0FF) | (((UInt32)TargetMcu) << 16);
        }
    }
    public byte TargetMcu
    {
        get
        {
            return (byte)(hardware_code >> 8);
        }
        set
        {
            hardware_code = (hardware_code & 0xFF0F) | (((UInt32)ModuleSubType) << 8);
        }
    }
    public byte TargetPeripheral
    {
        get
        {
            return (byte)(hardware_code);
        }
        set
        {
            hardware_code = (hardware_code & 0xFFF0) | (((UInt32)TargetPeripheral));
        }
    }
    #endregion

    public UInt32 security_information;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public UInt32[] reserved1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] image_plaintext_digest;
    public UInt16 reserved2;
    public UInt16 crc16;
}

//新的升级协议加密包尾
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct IMAGE_TAIL
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] enc1_md5;
}

//命令码是0x0085
//固件码查询
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct HEADERWARE_CODE_GET
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] device_id;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct HEADERWARE_CODE_ACK
{
    public UInt32 hw_code;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] device_id;
}

//命令码是0x0086
//固件头发送
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEND_IMAGE_HEADER
{
    public IMAGE_HEADER image_header;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] device_id;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEND_IMAGE_HEADER_ACK
{
    public byte ack;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] device_id;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct config_slot_item
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] config_data;

}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct data_slot_item
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] slot_data;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct config_slot_t
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public config_slot_item[] slot;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct data_slot_t
{
    public byte index;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public data_slot_item[] slot;
}
