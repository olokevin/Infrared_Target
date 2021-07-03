using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SerialPortManager : SerialPortObject
{
    private float m_checkTiming = 0;
    private List<string> m_existingPortList;
    private Dictionary<string, SerialPortEntity> m_portDic;

    public override void Start()
    {
        m_existingPortList = new List<string>();
        m_portDic = new Dictionary<string, SerialPortEntity>();
        AddEvent(EventDef.OpenPort, new Action<OpenPortEvent>(OnOpenPort));
        AddEvent(EventDef.ClosePort, new Action<ClosePortEvent>(OnClosePort));
        AddEvent(EventDef.SendProto, new Action<SendProtoEvent>(OnSendProto));
    }

    public override void Update()
    {
        m_checkTiming += SerialPortService.deltaTime;
        if (m_checkTiming >= 0.5f)
        {
            m_checkTiming -= 0.5f;
            CheckDeviceNodes();
        }
    }

    private void CheckDeviceNodes()
    {
        string[] winPorts = HardWareCheck.GetStringSerialPorts();
        List<string> addList = new List<string>();
        List<string> removeList = new List<string>();
        List<string> extstingList = new List<string>();
        List<string> workingList = new List<string>();
        List<SerialPortEntity> doCloseList = new List<SerialPortEntity>();

        for (int i = 0; i < winPorts.Length; ++i)
        {
            if (m_existingPortList.Contains(winPorts[i]))
            {
                extstingList.Add(winPorts[i]);
                m_existingPortList.Remove(winPorts[i]);
            }
            else
            {
                addList.Add(winPorts[i]);
            }
        }

        for (int i = 0; i < m_existingPortList.Count; ++i)
        {
            removeList.Add(m_existingPortList[i]);
        }

        for (int i = 0; i < removeList.Count; ++i)
        {
            if (m_portDic.ContainsKey(removeList[i]))
            {
                string sortName = Utility.GetComNameWithNumber(removeList[i]);
                doCloseList.Add(m_portDic[removeList[i]]);
                SerialPortEntity portEntity = m_portDic[removeList[i]];
                if (portEntity.opened) PostEvent(ServiceID.UI, new MessageEvent(LogType.Error, "{0}串口断开", sortName));
                m_portDic.Remove(removeList[i]);
            }
        }

        foreach (var cur in m_portDic)
        {
            if (cur.Value.IsOpen())
            {
                workingList.Add(cur.Key);
            }
        }

        List<string> list = new List<string>();
        for (int i = 0; i < addList.Count; ++i) list.Add(addList[i]);
        for (int i = 0; i < extstingList.Count; ++i) list.Add(extstingList[i]);
        m_existingPortList = list;

        UpdatePortsEvent ev = new UpdatePortsEvent();
        ev.listAdd = addList;
        ev.listDel = removeList;
        ev.listRetain = extstingList;
        ev.listWorking = workingList;
        PostEvent(ServiceID.UI, ev);
    }

    public void OnOpenPort(OpenPortEvent ev)
    {
        string portname = ev.portName;
        if (m_portDic.ContainsKey(portname))
        {
            PostEvent(ServiceID.UI, new PortStateChangeEvent(portname, SerialState.Open));
            return;
        }

        string portcomName = Utility.GetComNameWithNumber(portname);
        int portNumber = Utility.GetSerialPortNumFromCOMx(portcomName);
        List<string> ports = new List<string>(HardWareCheck.GetStringSerialPorts());
        if (ports.Contains(portname))
        {
            SerialPortEntity serialport = new SerialPortEntity();
            bool bSuc = serialport.OpenPort(portcomName, ev.param);
            if (bSuc)
            {
                m_portDic.Add(portname, serialport);
                PostEvent(ServiceID.UI, new PortStateChangeEvent(portname, SerialState.Open));
            }
        }
    }

    public void OnClosePort(ClosePortEvent ev)
    {
        string portname = ev.portName;
        if (!m_portDic.ContainsKey(portname))
        {
            PostEvent(ServiceID.UI, new PortStateChangeEvent(portname, SerialState.Close));
            return;
        }
        SerialPortEntity serialport = m_portDic[portname];
        serialport.ClosePort();
        if (m_portDic.ContainsKey(portname)) m_portDic.Remove(portname);
        PostEvent(ServiceID.UI, new PortStateChangeEvent(portname, SerialState.Close));
    }

    private void OnSendProto(SendProtoEvent ev)
    {
        foreach(var port in m_portDic)
        {
            port.Value.SendData(ev.data);
        }
    }
}
