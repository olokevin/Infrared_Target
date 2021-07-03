using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

public class SerialPortParam
{
    public int baudRate;
    public int dataBits;
    public Parity parity;
    public StopBits stopBits;
    public int readTimeout;
    public int writeTimeout;
}

public class FirmwareItem
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string Date { get; set; }
    public string Url { get; set; }
}

public enum DownloadCurrentState
{
    None,
    Unwork,
    Downloading,
    Finish,
    Failed,
    Abort,
}

public class GoalInfo
{
    public GameType type;
    public Colors color;
    public double radius;
}

public enum GameType
{
    Target,
    Armor,
    Custom,
}

public enum BulletAtArea
{
    Target,
    BigArmor,
    SmallArmor,
}

public enum FirmwareState
{
    Offline,
    Warning,
    Online,
}

public enum ArmorType
{
    Small,
    Big,
}

public enum CustomType
{
    Circle,
    Rectangle,
    Custom,
}

public enum LogType
{
    Info,
    Suc,
    Warning,
    Error,
}

public enum SerialState
{
    Open,
    Close,
}

public enum BulletType
{
    Golf,
    Bullet,
}

public enum BulletAttribute
{
    All,
    Hit,
    Miss,
}

public class AttributeInfo
{
    public const float BulletSize = 10f;
    public const float GolfSize = 50f;
    public static Color BulletColor = Colors.Gray;
    public static Color MiniCircleColor = Color.FromArgb(91, 255, 169, 00);
    public static Color AveCircleColor = Color.FromArgb(135, 101, 242, 216);
    public static Color highLightColor = Colors.Yellow;
    public static Color curBulletColor = Colors.Blue;
}

public class RobotData
{
    public byte freq;
    public float speed;
}

public class CircleData
{
    public string sx;
    public string sy;
    public string sr;
    public string sCount;

    public string jx;
    public string jy;
    public string jr;
    public string jCount;
}


