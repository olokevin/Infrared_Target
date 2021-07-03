using InfraredRayTarget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

public class DataManager
{
    private static DataManager m_ins;
    public static DataManager Instance
    {
        get
        {
            if (m_ins == null) m_ins = new DataManager();
            return m_ins;
        }
    }

    private DataManager()
    {
        historyPath = new List<string>();
        portInfoDic = new Dictionary<string, SetSerialPort>();
        portParamDic = new Dictionary<string, SerialPortParam>();
        customGoalDic = new Dictionary<string, CustomGoalInfo>();
        InitCusomGoal();
        currentGameType = GameType.Target;
        currentShowBulletsType = BulletAttribute.All;
        currentFirmwareState = FirmwareState.Offline;
        lightbead_x = new bool[65];
        lightbead_y = new bool[41];
    }

    private void InitCusomGoal()
    {
        //CustomGoalInfo hitMouse = new CustomGoalInfo();
        //hitMouse.name = "打地鼠";
        //hitMouse.image.source = new BitmapImage(new Uri("pack://application:,,,/Images/connect.png"));
        //hitMouse.scale.type = CustomScaleType.Fixed;
        //hitMouse.scale.fixValue = 1;
        //hitMouse.alive.createSpan = 2000;
        //hitMouse.alive.createCount = 2;
        //hitMouse.alive.aliveTime = 2;
        //hitMouse.alive.destoryWhenHit = true;
        //customGoalDic.Add(hitMouse.name, hitMouse);
    }

    public Dictionary<string, SetSerialPort> portInfoDic;
    public Dictionary<string, SerialPortParam> portParamDic;
    public Dictionary<string, CustomGoalInfo> customGoalDic;
    public CustomGoalInfo currentCustomGoalInfo;
    public List<string> historyPath;

    public GameType currentGameType;                     //当前游戏模式        靶子 or 装甲板
    public ArmorType currentArmorType;                   //当前装甲板          小 or 大
    public BulletAttribute currentShowBulletsType;       //当前显示子弹        全部 or 命中 or 未命中
    public FirmwareState currentFirmwareState;           //当前固件状态        在线 or 错误 or 离线

    public string firmware_version;
    public bool[] lightbead_x;
    public bool[] lightbead_y;

    public byte[] deviceId;
}
