using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UpdateMnager : UpdateObject
{
    private static UpdateMnager ins;
    public static UpdateMnager Instance
    {
        get
        {
            if (ins == null) ins = new UpdateMnager();
            return ins;
        }
    }

    private HttpDownloader m_loader;
    private UpdateModule m_updater;

    public override void Start()
    {
        m_loader = new HttpDownloader();
        AddEvent(EventDef.DownloadConfig, new Action<DownloadConfigEvent>(OnStartDownloadConfig));
        AddEvent(EventDef.StartUpdate, new Action<StartUpdateEvnet>(OnStartUpdate));
        AddEvent(EventDef.CMD_ID_QUERY_HARDWARE_CODE, new Action<RecvProtoEvent>(OnQueryHardwareCode));
        AddEvent(EventDef.CMD_ID_SEND_IMAGEHEADER, new Action<RecvProtoEvent>(OnSendImageHeader));
        AddEvent(EventDef.CMD_ID_UPGRADE_START, new Action<RecvProtoEvent>(OnUpdateStartAck));
        AddEvent(EventDef.CMD_ID_UPGRADE_DATA, new Action<RecvProtoEvent>(OnUpgradeData));
        AddEvent(EventDef.CMD_ID_UPGRADE_END, new Action<RecvProtoEvent>(OnUpdateEndAck));
    }

    private void OnStartUpdate(StartUpdateEvnet ev)
    {
        if (!File.Exists(ev.path)) return;
        FirmwareContent enc = FirmwareCryptUtility.ParseEncxOrPack(ev.path);
        if (enc == null) return;
        m_updater = new UpdateModule();
        m_updater.SetUpdateInfo(new Module(), enc);
        m_updater.StartUpdate();
    }

    private void OnStartDownloadConfig(DownloadConfigEvent obj)
    {
        m_loader.Download();
    }

    public override void Update()
    {
        
    }

    //协议回调 - 查询硬件码ACK
    private void OnQueryHardwareCode(RecvProtoEvent ev)
    {
        T_HEADER header = new T_HEADER();
        HEADERWARE_CODE_ACK body = (HEADERWARE_CODE_ACK)ProtoManager.DecodeProto(ev.proto, typeof(HEADERWARE_CODE_ACK), ref header);
        m_updater.OnQueryHardwareCode(header, body);
    }

    private void OnSendImageHeader(RecvProtoEvent ev)
    {
        T_HEADER header = new T_HEADER();
        SEND_IMAGE_HEADER_ACK body = (SEND_IMAGE_HEADER_ACK)ProtoManager.DecodeProto(ev.proto, typeof(SEND_IMAGE_HEADER_ACK), ref header);
        m_updater.OnSendImageHeader(header, body);
    }

    private void OnUpdateStartAck(RecvProtoEvent ev)
    {
        T_HEADER header = new T_HEADER();
        ACK_UPDATE_START body = (ACK_UPDATE_START)ProtoManager.DecodeProto(ev.proto, typeof(ACK_UPDATE_START), ref header);
        m_updater.OnUpdateStartAck(header, body);
    }

    private void OnUpgradeData(RecvProtoEvent ev)
    {
        T_HEADER header = new T_HEADER();
        ACK_UPDATE_DATA body = (ACK_UPDATE_DATA)ProtoManager.DecodeProto(ev.proto, typeof(ACK_UPDATE_DATA), ref header);
        m_updater.OnUpdateDataAck(header, body);
    }

    private void OnUpdateEndAck(RecvProtoEvent ev)
    {
        T_HEADER header = new T_HEADER();
        ACK_UPDATE_END body = (ACK_UPDATE_END)ProtoManager.DecodeProto(ev.proto, typeof(ACK_UPDATE_END), ref header);
        m_updater.OnUpdateEndAck(header, body);
    }
}
