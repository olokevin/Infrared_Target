using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

public class UpdateModule
{
    //数据中心
    private DataManager m_data = DataManager.Instance;

    //模块信息
    public Module m_module = null;

    //固件内容
    public FirmwareContent m_updateFileItem = null;

    //升级管理定时器
    private DispatcherTimer m_timer = new DispatcherTimer();

    private UpdateModuleStatus m_curStep;
    private int m_retryCountReqHardwareCode = 0;
    private int m_retryCountSendHeader = 0;
    private int m_retryCountReqUpdateStart = 0;
    private int m_retryCountReqData = 0;
    private int m_retryCountReqEnd = 0;
    private int m_curSeq = 0;
    public float updateProgress = 0;
    private int currentStep = 0;
    private bool firstStep = false;

    UpdateResult m_result;

    T_ROBOT robot;
    T_MODULE module;

    //================================= 测试数据 ====================================
    private float m_falshEraseTime;             //flash擦除时间(收到硬件码Ack截止)
    private float m_headerSendTime;             //固件头发送时间(收到固件头Ack截止)
    private float m_updateReqTime;              //升级请求时间(收到升级请求Ack截止)
    private List<float> m_listEachPackTimes = new List<float>();
    private int m_realPackSendNum = 0;                  //数据包发送数量
    private float m_averageDataTime = 0;        //平均每包发送时间
    private float m_totalUpdateTime = 0;        //全部升级时间

    private DateTime t_start;
    private DateTime t_past;

    private void Reset()
    {
        m_retryCountReqHardwareCode = 0;
        m_retryCountSendHeader = 0;
        m_retryCountReqUpdateStart = 0;
        m_retryCountReqData = 0;
        m_retryCountReqEnd = 0;
        m_curSeq = 0;
        updateProgress = 0;
    }

    //注入
    public void SetUpdateInfo(Module _moduleItem, FirmwareContent _fileItem)
    {
        m_module = _moduleItem;
        m_updateFileItem = _fileItem;
        robot.id = 0xff;
        robot.type = 0xff;
        module.id = 0xff;
        module.type = 0xff;
    }

    //开始升级(总)
    public void StartUpdate()
    {
        //异常处理: 文件不存在
        if (m_updateFileItem != null && !File.Exists(m_updateFileItem.filePath))
        {
            Debug.LogError("固件路径出错");
            ExitUpdateModule(UpdateResult.Failed_FilePathError);
            return;
        }

        //测试时间插入
        t_start = DateTime.Now;

        Reset();
        m_curStep = UpdateModuleStatus.STEP_REQ_HARDWARECODE;

        t_past = DateTime.Now;
        ReqHardwareCode();
        SendFirmwareHeader();
        ReqUpdateStart();

        //发起升级请求
        m_timer.Tick += new EventHandler(Timer_Tick);
        SetTimeInterval(0.2f);
        m_timer.Start();
    }

    //查询硬件码
    public void ReqHardwareCode()
    {
        ModuleData _moduleItem = m_module.moduleData;
        try
        {
            HEADERWARE_CODE_GET data = new HEADERWARE_CODE_GET();
            data.device_id = DataManager.Instance.deviceId;
            //Array.Copy(_moduleItem.DeviceId, 0, data.device_id, 0, 12);
            byte[] dataArry = Utility.StructToBytes(data); 
            Debug.Log("查询硬件码的Receiver模块类型:{0},ID:{1}", module.type, module.id);
            ProtoManager.SendProto(EventDef.CMD_ID_QUERY_HARDWARE_CODE,
                    0, dataArry, (ushort)dataArry.Length, robot, module);
            Utility.PostEvent(new MessageEvent(LogType.Warning, "升级 - 开始查询硬件码..."));
        }
        catch (Exception excption)
        {
            Debug.LogError(excption.Message);
        }
    }

    //查询硬件码 - Ack
    public void OnQueryHardwareCode(T_HEADER header, HEADERWARE_CODE_ACK ack)
    {
        //逻辑
        if (m_curStep == UpdateModuleStatus.STEP_REQ_HARDWARECODE)
        {
            m_curStep = UpdateModuleStatus.STEP_SEND_HEADER;

            uint ackCode = ack.hw_code;
            uint localCode = m_updateFileItem.fileheader.hardware_code;
            uint tmpLocalCode = localCode & 0x00FF0000;

            //通配固件特殊处理 - 如果本地硬件码的ID段位是0xFF,则此段位通配
            if (tmpLocalCode == 0x00FF0000)
            {
                localCode = localCode | 0x00FF0000;
                ackCode = ackCode | 0x00FF0000;
            }

            if (localCode != ackCode)
            {
                Utility.PostEvent(new MessageEvent(LogType.Error, "硬件码校验失败,返回的硬件码： 0x{0:X}, 拒绝升级", ack.hw_code));
                ExitUpdateModule(UpdateResult.Failed_RetCodeError);
                return;
            }

            //LOG
            Utility.PostEvent(new MessageEvent(LogType.Suc, "硬件码校验成功"));

            //得到Flash擦除时间
            m_falshEraseTime = (float)(DateTime.Now - t_past).TotalMilliseconds;
            t_past = DateTime.Now;

            //发送固件头
            SendFirmwareHeader();
        }
    }

    //发送固件头
    public void SendFirmwareHeader()
    {
        ModuleData _moduleItem = m_module.moduleData;

        try
        {
            //填充 - DeviceID
            SEND_IMAGE_HEADER data = new SEND_IMAGE_HEADER();
            data.device_id = DataManager.Instance.deviceId;
            //Array.Copy(_moduleItem.DeviceId, 0, data.device_id, 0, 12);

            //填充 - 固件头
            data.image_header = m_updateFileItem.fileheader;

            //LOG
            Debug.LogWarning("固件头SecurityInfo = {0}", data.image_header.security_information);

            byte[] dataArry = ProtoManager.StructToBytes(data);
            ProtoManager.SendProto(EventDef.CMD_ID_SEND_IMAGEHEADER,
                    0, dataArry, (ushort)dataArry.Length, robot, module);

            Debug.Log("Send - 固件头");
        }
        catch (Exception excption)
        {
            Debug.LogError(excption.Message);
        }
    }

    //发送固件头 - Ack
    public void OnSendImageHeader(T_HEADER header, SEND_IMAGE_HEADER_ACK ack)
    {
        Debug.Log("ACK - 发送固件头");

        //逻辑
        if (m_curStep == UpdateModuleStatus.STEP_SEND_HEADER)
        {
            m_curStep = UpdateModuleStatus.STEP_REQ_UPDATE;

            //校验异常处理
            if (ack.ack != 0)
            {
                Utility.PostEvent(new MessageEvent(LogType.Error, "固件包ACK错误: 0x{0:X}, 拒绝升级", ack.ack));
                ExitUpdateModule(UpdateResult.Failed_RetCodeError);
                return;
            }

            //得到固件头发送时间
            m_headerSendTime = (float)(DateTime.Now - t_past).TotalMilliseconds;
            t_past = DateTime.Now;

            //逻辑
            ReqUpdateStart();
            SetTimeInterval(0.2f);
        }
    }

    //请求升级
    private void ReqUpdateStart()
    {
        REQ_UPDATE_START req = new REQ_UPDATE_START();
        req.deviceId = DataManager.Instance.deviceId;
        req.encrypt_type = m_updateFileItem.encrypt;

        Debug.Log("加密类型:{0}", req.encrypt_type);
        req.length = (uint)m_updateFileItem.fileData.Length;

        byte[] senddata = ProtoManager.StructToBytes(req);
        ProtoManager.SendProto(EventDef.CMD_ID_UPGRADE_START, 0, senddata, (ushort)senddata.Length,
            robot, module);

        Debug.Log("Send - 请求升级");
    }

    //请求升级-ACK
    public void OnUpdateStartAck(T_HEADER header, ACK_UPDATE_START ack)
    {
        Debug.Log("Ack - 请求升级");

        //逻辑
        if (m_curStep == UpdateModuleStatus.STEP_REQ_UPDATE)
        {
            m_curStep = UpdateModuleStatus.STEP_REQ_DATA;
            if (ack.retCode != 0)
            {
                Utility.PostEvent(new MessageEvent(LogType.Error, "请求升级码错误：0x{0:X}", ack.retCode));
                ExitUpdateModule(UpdateResult.Failed_RetCodeError);
                return;
            }
            SetTimeInterval(0.5f);
            m_module.moduleData.UpdateStatus = (byte)ModuleUpgradeStatus.UpgradeProcessing;
            m_module.moduleData.ProgressVal = 0;

            //得到升级请求发送时间
            m_updateReqTime = (float)(DateTime.Now - t_past).TotalMilliseconds;
            t_past = DateTime.Now;

            ReqUpdateData();
        }
    }

    //设置定时器间隔
    private void SetTimeInterval(float v)
    {
        m_timer.Interval = TimeSpan.FromSeconds(v);
    }

    //发送数据
    private void ReqUpdateData()
    {
        REQ_UPDATE_DATA req = new REQ_UPDATE_DATA();
        req.data = new byte[256];
        req.deviceId = DataManager.Instance.deviceId;
        req.sequence = (uint)GetCurSeq();

        //calc data len
        int len = 0;
        if ((req.sequence + 1) * 256 <= m_updateFileItem.fileData.Length)
        {
            len = 256;
        }
        else
        {
            len = m_updateFileItem.fileData.Length - (int)req.sequence * 256;
        }
        if (len == 0)
        {
            return;
        }
        Array.Copy(m_updateFileItem.fileData, req.sequence * 256, req.data, 0, len);

        //send
        byte[] senddata = ProtoManager.StructToBytes(req);
        ProtoManager.SendProto(EventDef.CMD_ID_UPGRADE_DATA, 0, senddata, (ushort)senddata.Length, robot, module);

        m_realPackSendNum++;
        Debug.Log("Send - 升级数据");
    }

    //发送数据-ACK
    public void OnUpdateDataAck(T_HEADER header, ACK_UPDATE_DATA ack)
    {
        Debug.Log("Ack - 升级数据");

        //测试代码注入
        m_listEachPackTimes.Add((float)(DateTime.Now - t_past).TotalMilliseconds);
        t_past = DateTime.Now;

        //更新模块的时间戳
        m_module.moduleData.last_time = t_past;

        //逻辑
        if (m_curStep == UpdateModuleStatus.STEP_REQ_DATA &&
            ack.retCode == 0 &&
            ack.sequence == m_curSeq)
        {
            m_timer.Stop();
            m_timer.Start();
            ++m_curSeq;
            updateProgress = GetUpdatePercent();
            if (IsFileEnd())
            {
                m_curStep = UpdateModuleStatus.STEP_REQ_END;
                SetTimeInterval(0.2f);
                ReqUpdateEnd();
            }
            else
            {
                ReqUpdateData();
            }
        }

        else
        {
            //升级数据失败过滤并打印LOG
            //1、升级状态不对
            if (m_curStep != UpdateModuleStatus.STEP_REQ_DATA)
            {
                Utility.PostEvent(new MessageEvent(LogType.Error, "升级数据--状态机错误"));
            }

            //2、返回错误码 - 有错误码，直接取消升级
            if (ack.retCode != 0)
            {
                Utility.PostEvent(new MessageEvent(LogType.Error, "升级数据包(ack)--返回错误码 => 0x{0:X}", ack.retCode));
                ExitUpdateModule(UpdateResult.Failed_RetCodeError);
            }

            if (ack.sequence != m_curSeq)
            {
                Utility.PostEvent(new MessageEvent(LogType.Error, "收到升级数据包序号错误, 收到Ack序号：{0}，期望序号：{1}", ack.sequence, m_curSeq));
            }

            //升级过程中断电导致此错误，直接取消升级
            if (ack.retCode == (byte)error_status_e.BL_ERROR_FLASH_OPERTE_FAILED)
            {
                Utility.PostEvent(new MessageEvent(LogType.Error, "升级数据--BL_ERROR_FLASH_OPERTE_FAILED，升级取消"));
                //ExitUpdateModule(UpdateResult.Failed_RetCodeError);
            }
        }

        //升级进度设置
        if (m_module.moduleData.UpdateStatus == (byte)ModuleUpgradeStatus.UpgradeProcessing)
            m_module.moduleData.ProgressVal = updateProgress;

        int temp = (int)(updateProgress * 100);
        if (firstStep == false)
        {
            firstStep = true;
            Utility.PostEvent(new MessageEvent(LogType.Suc, "升级 - 当前进度：0%"));
        }
        if (currentStep != temp && temp % 10 == 0)
        {
            currentStep = temp;
            Utility.PostEvent(new MessageEvent(LogType.Suc, "升级 - 当前进度：{0}%", currentStep));
        }

        //不等于0的情况自动超时重发
    }

    //升级完成
    private void ReqUpdateEnd()
    {
        REQ_UPDATE_END req = new REQ_UPDATE_END();
        req.deviceId = DataManager.Instance.deviceId;
        req.verify_data = new byte[16];
        Array.Copy(m_updateFileItem.fileDataMd5, req.verify_data, 16);
        req.verify_type = 1;

        //send
        byte[] senddata = ProtoManager.StructToBytes(req);
        ProtoManager.SendProto(EventDef.CMD_ID_UPGRADE_END, 0, senddata, (ushort)senddata.Length, robot, module);
    }

    //升级完成-ACK
    public void OnUpdateEndAck(T_HEADER header, ACK_UPDATE_END ack)
    {
        Utility.PostEvent(new MessageEvent(LogType.Suc, "Send - 升级结束"));

        //逻辑
        if (m_curStep == UpdateModuleStatus.STEP_REQ_END)
        {
            m_timer.Stop();
            m_timer.Start();
            if (ack.recCode == 0)
            {
                Utility.PostEvent(new MessageEvent(LogType.Suc, "Send - 升级成功"));

                ExitUpdateModule(UpdateResult.Success);
            }
            else
            {
                Utility.PostEvent(new MessageEvent(LogType.Suc, "升级失败: 0x{0:X}", ack.recCode));
                ExitUpdateModule(UpdateResult.Failed_CheckSumError);
            }
        }
    }

    //获取当前发送SeqNum
    private int GetCurSeq()
    {
        return m_curSeq;
    }

    //获取当前升级百分比
    public float GetUpdatePercent()
    {
        int seqCount = 0;
        seqCount = m_updateFileItem.fileData.Length / 256;
        if (m_updateFileItem.fileData.Length % 256 != 0)
        {
            ++seqCount;
        }
        return m_curSeq / (float)seqCount;
    }

    //取消升级
    public void CancelUpdate()
    {
        if (m_curStep == UpdateModuleStatus.STEP_COMPLETED)
        {
            return;
        }
        ExitUpdateModule(UpdateResult.Failed_UserCancel);
    }

    //取消升级
    public void CancelUpdateWithoutReport()
    {
        if (m_curStep == UpdateModuleStatus.STEP_COMPLETED)
        {
            return;
        }

        Debug.Log("取消升级");
    }

    //是否已是文件尾
    private bool IsFileEnd()
    {
        if (m_curSeq * 256 >= m_updateFileItem.fileData.Length)
        {
            return true;
        }
        return false;
    }

    //退出该升级模块
    private void ExitUpdateModule(UpdateResult result)
    {
        m_curStep = UpdateModuleStatus.STEP_COMPLETED;
        m_timer.Stop();

        m_result = result;
        if (result != UpdateResult.Success)
        {
            m_module.moduleData.UpdateStatus = (byte)ModuleUpgradeStatus.UpgradeFail;
        }

        if (result == UpdateResult.Success)
        {
            m_module.moduleData.UpdateStatus = (byte)ModuleUpgradeStatus.UpgradeSuccess;
        }
        else if (result == UpdateResult.Failed_UserCancel)
        {
            m_module.moduleData.UpdateStatus = (byte)ModuleUpgradeStatus.NotUpgrade;
        }
        else if (result == UpdateResult.Failed_FilePathError)
        {
            m_module.moduleData.UpdateStatus = (byte)ModuleUpgradeStatus.UpgradeFail;
        }
        else if (result == UpdateResult.Failed_RetCodeError)
        {
            m_module.moduleData.UpdateStatus = (byte)ModuleUpgradeStatus.UpgradeFail;
        }

        //UI设置
        m_module.moduleData.ProgressVal = 0;

        //不管升级有没有成功，先清除此标记，具体匹不匹配由下次查询结果在设置
        m_module.moduleData.AppVersionMismatch = false;

        //打印测试时间
        PrintTimeTestLog();



        //销毁自己
        DestroySelf();
    }

    //升级请求Timer
    private void Timer_Tick(object sender, EventArgs args)
    {
        if (m_curStep == UpdateModuleStatus.STEP_REQ_HARDWARECODE)
        {
            ++m_retryCountReqHardwareCode;
            if (m_retryCountReqHardwareCode <= 50)
            {
                Debug.LogError("Retry ReqHardwareCode: 第{0}次", m_retryCountReqHardwareCode);
                ReqHardwareCode();
            }
            else
            {
                OnTimeOut();
            }

        }
        else if (m_curStep == UpdateModuleStatus.STEP_SEND_HEADER)
        {
            ++m_retryCountSendHeader;
            if (m_retryCountSendHeader <= 50)
            {
                Debug.LogError("Retry SendHeader: 第{0}次", m_retryCountSendHeader);
                SendFirmwareHeader();
            }
            else
            {
                OnTimeOut();
            }
        }
        else if (m_curStep == UpdateModuleStatus.STEP_REQ_UPDATE)
        {
            ++m_retryCountReqUpdateStart;
            if (m_retryCountReqUpdateStart <= 50)
            {
                Debug.LogError("Retry ReqUpdateStart");
                ReqUpdateStart();
            }
            else
            {
                OnTimeOut();
            }
        }
        else if (m_curStep == UpdateModuleStatus.STEP_REQ_DATA)
        {
            ++m_retryCountReqData;
            if (m_retryCountReqData <= 50)
            {
                Debug.LogError("Retry 发送升级数据:第[{0}]次", m_retryCountReqData);
                ReqUpdateData();
            }
            else
            {
                OnTimeOut();
            }
        }
        else if (m_curStep == UpdateModuleStatus.STEP_REQ_END)
        {
            ++m_retryCountReqEnd;
            if (m_retryCountReqEnd <= 50)
            {
                Debug.LogError("Retry ReqUpdateEnd");
                ReqUpdateEnd();
            }
            else
            {
                OnTimeOut();
            }
        }
        else //Completed
        {
        }
    }

    //请求已超时
    private void OnTimeOut()
    {
        ExitUpdateModule(UpdateResult.Failed_TimeOut);
        Debug.LogError("请求超时");
    }

    //销毁自身
    public void DestroySelf()
    {
        m_timer.Stop();
        m_timer = null;
        m_module.updateModule = null;
    }

    //打印测试信息
    private void PrintTimeTestLog()
    {
        //总时间
        m_totalUpdateTime = (float)(DateTime.Now - t_start).TotalMilliseconds;

        //总升级数据部分时间
        float allDataUpdTime = 0.0f;

        //单包最大时间
        float max = 0.0f;

        for (int i = 0; i < m_listEachPackTimes.Count; ++i)
        {
            allDataUpdTime += m_listEachPackTimes[i];
            if (max < m_listEachPackTimes[i]) max = m_listEachPackTimes[i];
        }

        //每包平均时间
        m_averageDataTime = allDataUpdTime / m_listEachPackTimes.Count;

        Debug.LogWarning("=============================== 升级过程延时报表 ==============================");
        Debug.LogWarning("Flash擦除: {0}ms", m_falshEraseTime);
        Debug.LogWarning("固件头发送:{0}ms", m_headerSendTime);
        Debug.LogWarning("升级请求发送:{0}ms", m_updateReqTime);

        Debug.LogWarning("\n升级数据部分延迟如下：");
        Debug.LogWarning("升级数据总延迟:{0}ms", allDataUpdTime);
        Debug.LogWarning("升级数据单包平均延迟:{0}ms", m_averageDataTime);
        Debug.LogWarning("升级数据单包最大延迟:{0}ms", max);
        Debug.LogWarning("上位机共发数据包:{0}次", m_realPackSendNum);
        Debug.LogWarning("收到数据包Ack:{0}次\n", m_listEachPackTimes.Count);
        Debug.LogWarning("上位机重传:{0}次\n", m_realPackSendNum - m_listEachPackTimes.Count);

        Debug.LogWarning("此次升级总耗时:{0}ms, 合{1}s", m_totalUpdateTime, m_totalUpdateTime / 1000);
        Debug.LogWarning("===============================================================================");
    }
}
