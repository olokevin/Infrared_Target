using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//模块数据
public class ModuleData : INotifyPropertyChanged
{
    //逻辑对象
    public Module LogicGameObject;

    //时间戳
    public DateTime last_time;

    //在线状态
    private bool m_bOnline = true;
    public bool bOnline { get { return m_bOnline; } set { m_bOnline = value; SendTrigger("bOnline"); } }

    //模块
    private T_MODULE m_moduleProperty;
    public T_MODULE ModuleProperty { get { return m_moduleProperty; } set { m_moduleProperty = value; SendTrigger("ModuleType"); SendTrigger("ModuleId"); } }

    //类型和ID
    public byte ModuleType { get { return m_moduleProperty.type; } }
    public byte ModuleId { get { return m_moduleProperty.id; } }

    //loader版本
    private UInt32 m_loader_version;
    public UInt32 LoaderVersion { get { return m_loader_version; } set { m_loader_version = value; SendTrigger("LoaderVersion"); } }

    //App版本
    private UInt32 m_app_version;
    public UInt32 AppVersion { get { return m_app_version; } set { m_app_version = value; SendTrigger("AppVersion"); } }

    //设备ID
    private byte[] m_deviceId;
    public byte[] DeviceId { get { return m_deviceId; } set { m_deviceId = value; SendTrigger("DeviceId"); } }

    //升级的进度
    private double m_progressVal;
    public double ProgressVal { get { return m_progressVal; } set { m_progressVal = value; SendTrigger("ProgressVal"); } }

    ///App 版本与配置表是否匹配
    ///     1、该属性决定AppVsersion的颜色(默认黑色；当与配置表不匹配时，文字显示红色；如果配置表插槽没有对应的配置项，显示默认黑色)
    private bool m_appVersionMismatch;
    public bool AppVersionMismatch { get { return m_appVersionMismatch; } set { m_appVersionMismatch = value; SendTrigger("AppVersionMismatch"); } }

    ///模块升级状态
    ///     1、该属性决定状态Lable的颜色和内容（未升级：黑色"未升级"；等待升级：黑色"等待升级"；升级中：黑色"升级中"；升级完成：绿色"升级完成"；升级失败：红色"升级失败"）
    ///     2、该属性决定进度条的颜色（升级条默认是绿色，升级失败进度条为红色）
    private byte m_updateStatus;
    public byte UpdateStatus { get { return m_updateStatus; } set { m_updateStatus = value; SendTrigger("UpdateStatus"); } }

    ///配置表插槽是否配置对象项
    ///     1、该属性决定升级按钮是否Disable（若配置表插槽没有对应的配置项，则升级按钮Disable）
    ///     2、该属性决定AppVsersion的颜色（若配置表插槽没有对应的配置项，显示默认黑色）
    public bool m_bHasModuleConfiged;
    public bool BHasModuleConfiged
    {
        get { return m_bHasModuleConfiged; }
        set
        {
            m_bHasModuleConfiged = value;
            SendTrigger("BHasModuleConfiged");
        }
    }

    public ModuleData(Module _mod)
    {
        LogicGameObject = _mod;
        m_deviceId = new byte[12];
    }

    public event PropertyChangedEventHandler PropertyChanged;

    //触发器
    private void SendTrigger(string strProperty)
    {
        if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(strProperty));
    }
}
