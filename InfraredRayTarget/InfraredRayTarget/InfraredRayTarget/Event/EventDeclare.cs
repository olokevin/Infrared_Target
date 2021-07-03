using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class BaseEvent
{
    public readonly int id;
    public BaseEvent(int _id) { id = _id; }
}

//===================================== 系统事件 =====================================

public class ThreadQuitEvent : BaseEvent
{
    public ThreadQuitEvent() : base(EventDef.Quit) { }
}

public class SendProtoEvent : BaseEvent
{
    public SendProtoEvent() : base(EventDef.SendProto) { }
    public byte[] data;
}

public class RecvProtoEvent : BaseEvent
{
    public RecvProtoEvent(int _protoid, byte[] _proto) : base(_protoid) { proto = _proto; }
    public byte[] proto;
}

public class MessageEvent : BaseEvent
{
    public MessageEvent(LogType _type, string _message, params object[] _args) : base(EventDef.Message)
    {
        type = _type;
        message = string.Format(_message, _args);
        time = DateTime.Now.ToString("HH:mm:ss");
    }

    public string time;
    public string user;
    public string message;
    public LogType type;
}

public class SelectCusomGoalEvent : BaseEvent
{
    public SelectCusomGoalEvent() : base(EventDef.SelectCustomGoal) { }
}

//===================================== 串口事件 =====================================

public class OpenPortEvent : BaseEvent
{
    public OpenPortEvent(string _portName, SerialPortParam _info) : base(EventDef.OpenPort)
    {
        portName = _portName;
        param = _info;
    }
    public OpenPortEvent(string _portName) : base(EventDef.OpenPort)
    {
        portName = _portName;
        param = new SerialPortParam();
        param.baudRate = 115200;
        param.dataBits = 8;
        param.parity = System.IO.Ports.Parity.None;
        param.stopBits = System.IO.Ports.StopBits.One;
        param.readTimeout = 1000;
        param.writeTimeout = 1000;
    }

    public string portName;
    public SerialPortParam param;
}

public class ClosePortEvent : BaseEvent
{
    public ClosePortEvent(string _portname) : base(EventDef.ClosePort) { portName = _portname; }
    public string portName;
}

public class PortStateChangeEvent : BaseEvent
{
    public PortStateChangeEvent(string _name, SerialState _state) : base(EventDef.PortStateChange) { state = _state; portname = _name; }
    public string portname;
    public SerialState state;
}

public class UpdatePortsEvent : BaseEvent
{
    public UpdatePortsEvent() : base(EventDef.UpdatePorts) { }
    public List<string> listAdd;
    public List<string> listDel;
    public List<string> listRetain;
    public List<string> listWorking;
}

//===================================== 其他事件 =====================================

public class DownloadConfigEvent : BaseEvent
{
    public DownloadConfigEvent() : base(EventDef.DownloadConfig) { }
}

public class DownloadConfigFinishEvent : BaseEvent
{
    public DownloadConfigFinishEvent() : base(EventDef.DownloadConfigFinish) { }
}

public class StartUpdateEvnet : BaseEvent
{
    public StartUpdateEvnet() : base(EventDef.StartUpdate) { }
    public string path;
}

public class TargetEvent : BaseEvent
{
    public TargetEvent(float _x, float _y, float _r) : base(EventDef.TargetProto) { x = _x; y = _y; r = _r; }
    public float x;
    public float y;
    public float r;
}

public class LoadDataEvent : BaseEvent
{
    public LoadDataEvent(string _path) : base(EventDef.LoadData) { path = _path; }
    public string path;
}
