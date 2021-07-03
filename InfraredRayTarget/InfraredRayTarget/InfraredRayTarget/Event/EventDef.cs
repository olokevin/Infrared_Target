using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class EventDef
{
    //================================== 系统【0-99】 ==================================

    public const int Quit = 0;
    public const int Message = 8;
    public const int ChangePoint = 2;
    public const int MotorEvent = 3;
    public const int SaveData = 4;
    public const int ClearMotorItem = 5;
    public const int SelectCustomGoal = 6;

    //================================== 升级 ==================================
    public const int DownloadConfig = 50;
    public const int DownloadConfigFinish = 51;
    public const int StartUpdate = 52;


    //================================== 协议【100-199】 ================================

    public const int SendProto = 100;
    public const int TargetProto = 0x0003;
    public const int TargetFaultProto = 102;

    //================================== 界面 【200-299】================================

    public const int UpdateMotorData = 200;
    public const int UpdateMotorUI = 201;
    public const int ClearMotorMessage = 202;
    public const int LoadData = 203;

    //================================== 串口【300-399】==================================

    public const int UpdatePorts = 300;
    public const int PortStateChange = 301;
    public const int OpenPort = 302;
    public const int ClosePort = 303;

    //=================================== 升级协议事件 ========================================
    public const int CMD_ID_QUERY_DEVICE_INFO = 0x080;        //查询信息
    public const int CMD_ID_UPGRADE_START = 0x81;            //请求升级
    public const int CMD_ID_UPGRADE_DATA = 0x82;             //升级数据
    public const int CMD_ID_UPGRADE_END = 0x83;              //升级成功
    public const int CMD_ID_QUERY_HARDWARE_CODE = 0x85;      //查询硬件码
    public const int CMD_ID_SEND_IMAGEHEADER = 0x86;         //发送固件头
}
