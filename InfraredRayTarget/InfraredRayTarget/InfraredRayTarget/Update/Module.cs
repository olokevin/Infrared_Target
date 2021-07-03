using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

public class Module
{
    //数据中心
    public DataManager m_data = DataManager.Instance;

    //模块数据
    public ModuleData moduleData;

    //升级组件
    public UpdateModule updateModule;

    //定时器
    private DispatcherTimer timer = new DispatcherTimer();

    public Module()
    {
  
        InitModuleData();
    }

    //初始化模块数据
    public void InitModuleData()
    {
        moduleData = new ModuleData(this);
       
    }

    //更新模块数据
    public void UpdateModuleData(T_HEADER header, ACK_QEURYINFO info)
    {
        if (moduleData.AppVersion != info.app_version) moduleData.AppVersion = info.app_version;

        //更新Loader Version
        if (moduleData.LoaderVersion != info.loader_version)
        {
            Debug.Log("Loader Version 检测更新，从{0}更新至{1}", Utility.MakeLongVersionStr(moduleData.LoaderVersion), Utility.MakeLongVersionStr(info.loader_version));
            moduleData.LoaderVersion = info.loader_version;
        }
        //if (moduleData.ModuleType != header.sender.type || moduleData.ModuleId != header.sender.id) moduleData.ModuleProperty = header.sender;
        if (!Utility.ByteArrayEqual(moduleData.DeviceId, info.deviceId)) Array.Copy(info.deviceId, moduleData.DeviceId, 12);

        //更新模块版本是否匹配
        string curAppVersion = Utility.MakeLongVersionStr(moduleData.AppVersion);

        string configAppVersion = "";

        //未配置
        if (string.IsNullOrEmpty(configAppVersion))
        {
            //不作处理...
            moduleData.AppVersionMismatch = false;
        }

        //已配置，则进行版本比较
        else
        {
            if (Utility.MakeLongVersionInt(curAppVersion) != Utility.MakeLongVersionInt(configAppVersion))
            {
                moduleData.AppVersionMismatch = true;
            }
            else moduleData.AppVersionMismatch = false;
        }

        //更新模块时间戳        
        moduleData.last_time = DateTime.Now;
    }

    //发起模块升级
    public void StartUpdate()
    {
        //填充数据
        FirmwareContent enc = FillUpdateData();

        //异常处理
        if (enc == null)
        {
            return;
        }

        //开始升级
        if (updateModule == null) updateModule = new UpdateModule();

        //设置升级所需数据
        updateModule.SetUpdateInfo(this, enc);

        //发起升级
        updateModule.StartUpdate();
    }

    //读取固件 - 填充升级包数据
    public FirmwareContent FillUpdateData()
    {
        //填充数据
        FirmwareContent fillData = new FirmwareContent();
        string strPath = "";
        Debug.LogWarning(strPath);
        if (!File.Exists(strPath)) return null;

        //调试模式（.bin）：需要额外加密为.encx或.pack
        if (strPath.EndsWith(".bin"))
        {
            byte[] binData = File.ReadAllBytes(strPath);
            EnCrypt enCrypt = new EnCrypt();
            byte[] TotalFirmwareArray = enCrypt.Encrypt(binData, strPath);

            //解析固件头并填充
            fillData = FirmwareCryptUtility.ParseEncxOrPack(TotalFirmwareArray, strPath);
        }

        //标准模式(.encx或.pack)
        else
        {
            fillData = FirmwareCryptUtility.ParseEncxOrPack(strPath);
        }

        return fillData;
    }

    //取消升级
    public void CancelUpdate()
    {
        //开始升级
        if (updateModule != null)
        {
            updateModule.CancelUpdate();
        }

        //参数设置
        moduleData.UpdateStatus = (byte)ModuleUpgradeStatus.NotUpgrade;
        moduleData.ProgressVal = 0;
    }

    //销毁自己
    public void OnDestroy()
    {
        //销毁数据
        moduleData = null;

        //升级组件的销毁
        if (updateModule != null)
        {
            updateModule.DestroySelf();
            updateModule = null;
        }

    }
}
