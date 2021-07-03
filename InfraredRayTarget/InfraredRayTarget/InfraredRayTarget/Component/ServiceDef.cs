using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;

public class ServiceID
{
    public const int None = 0;
    public const int UI = 1;
    public const int Net = 2;
    public const int SerialPort = 3;
    public const int UpdateFirmware = 4;
}

public class UIObject : GameObject
{
	public UIObject() : base(ServiceID.UI) { }
}

public class SerialPortObject : GameObject
{
	public SerialPortObject() : base(ServiceID.SerialPort) { }
}

public class NetObject : GameObject
{
	public NetObject() : base(ServiceID.Net) { }
}

public class UpdateObject : GameObject
{
    public UpdateObject() : base(ServiceID.UpdateFirmware) { }
}
