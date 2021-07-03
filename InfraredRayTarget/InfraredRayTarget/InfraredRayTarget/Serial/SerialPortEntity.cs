using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SerialPortEntity : SerialPortObject
{
    private SerialPort m_serialPort = null;
    private byte[] m_cache = null;
    private int m_len = 0;
    public bool opened = false;

    public override void Start()
    {
        m_cache = new byte[10240];
    }

    public bool IsOpen()
    {
        if (m_serialPort == null) return false;
        else return m_serialPort.IsOpen;
    }

    public bool OpenPort(string _portname, SerialPortParam _param)
    {
        if (m_serialPort != null) ClosePort();

        try
        {
            string[] portNames = HardWareCheck.GetStringSerialPortsOnlyCOM();
            if (!portNames.Contains(_portname)) return false;

            m_serialPort = new SerialPort(_portname);
            m_serialPort.BaudRate = _param.baudRate;
            m_serialPort.DataBits = _param.dataBits;
            m_serialPort.Parity = _param.parity;
            m_serialPort.StopBits = _param.stopBits;
            m_serialPort.ReadTimeout = _param.readTimeout;
            m_serialPort.WriteTimeout = _param.writeTimeout;

            m_serialPort.DataReceived += OnPortDataReceived;
            m_serialPort.Open();
            opened = true;
            PostEvent(ServiceID.UI, new MessageEvent(LogType.Suc, "打开{0}串口成功", _portname));
            return true;
        }
        catch (Exception ex)
        {
            PostEvent(ServiceID.UI, new MessageEvent(LogType.Error, "打开{0}串口失败，原因：{1}", _portname, ex.Message));
            ClosePort();
            return false;
        }
    }

    public void SendData(byte[] bytes)
    {
        try
        {
            m_serialPort.Write(bytes, 0, bytes.Length);
        }
        catch (Exception ex)
        {
            PostEvent(ServiceID.UI, new MessageEvent(LogType.Error, "串口{0}:发送协议失败，原因：{1}", m_serialPort.PortName, ex.Message));
        }
    }

    //接收数据
    private void OnPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            if (m_serialPort == null) return;
            if (!m_serialPort.IsOpen) return;
            byte[] bytes = new byte[1024];
            int readLength = m_serialPort.Read(bytes, 0, 1024);
            if (readLength == 0) return;
            Array.Copy(bytes, 0, m_cache, m_len, readLength);
            m_len += readLength;
            SearchPackage();
        }
        catch (Exception ex)
        {
            if (ex.GetType() == typeof(TimeoutException)) return;
            if (ex.Message.EndsWith("has timed-out")) return;

            m_len = 0;
            ClosePort();
            PostEvent(ServiceID.UI, new MessageEvent( LogType.Error, "{0}串口发生异常，原因：{1}", m_serialPort.PortName, ex.Message));
        }
    }

    //关闭串口
    public void ClosePort()
    {
        if (m_serialPort == null) return;
        try
        {
            if (m_serialPort.IsOpen)
            {
                m_serialPort.Close();
                m_serialPort.Dispose();
            }
            PostEvent(ServiceID.UI, new MessageEvent(LogType.Warning, "{0}串口关闭", m_serialPort.PortName));
        }
        catch (Exception ex)
        {
            PostEvent(ServiceID.UI, new MessageEvent(LogType.Error, "{0}串口异常，原因：{1}", m_serialPort.PortName, ex.Message));
        }
        m_serialPort = null;
    }

    private void SearchPackage()
    {
        int headerlen = Marshal.SizeOf(typeof(T_HEADER));
        while (true)
        {
            if (m_len < 0) throw new Exception("SearchPackage m_index < 0");
            if (m_len == 0) return;
            if (m_len < headerlen + 2) return;
            if (m_cache[0] != 0xA5)
            {
                RelocateHeader();
                continue;
            }

            T_HEADER tHeader = new T_HEADER();
            if (!SearchHeader(ref tHeader))
            {
                Debug.LogError("协议包头校验失败...");
                RelocateHeader();
                continue;
            }
            if (m_len < headerlen + 2 + tHeader.length) return;

            //整包校验
            int packagelen = headerlen + 2 + tHeader.length;
            ushort checksum = CRCCheck.GetCRC16(m_cache, headerlen + tHeader.length);
            if ((ushort)(checksum & 0x00ff) == m_cache[headerlen + tHeader.length] &&
                (ushort)((checksum >> 8) & 0xff) == m_cache[headerlen + tHeader.length + 1])
            {
                byte[] byPackage = new byte[packagelen];
                Array.Copy(m_cache, 0, byPackage, 0, packagelen);

                RecvProtoEvent ev = new RecvProtoEvent(tHeader.cmd_Id, byPackage);
                ev.proto = byPackage;
                if (tHeader.cmd_Id >= EventDef.CMD_ID_UPGRADE_START && 
                    tHeader.cmd_Id <= EventDef.CMD_ID_SEND_IMAGEHEADER)
                {
                    PostEvent(ServiceID.UpdateFirmware, ev);
                }
                else
                {
                    PostEvent(ServiceID.UI, ev);
                }
                //Debug.Log("收到cmd_id:0x{0:X}的协议", tHeader.cmd_Id);
            }

            //打印错误日志
            else
            {
                Debug.LogError("协议整包校验失败...");
            }
            Array.Copy(m_cache, packagelen, m_cache, 0, m_len - packagelen);
            m_len = m_len - packagelen;
        }
    }

    private void RelocateHeader()
    {
        int i = 1;
        for (; i < m_len; ++i)
        {
            if (m_cache[i] == 0xA5)
            {
                break;
            }
        }

        Array.Copy(m_cache, i, m_cache, 0, m_len - i);
        m_len = m_len - i;
    }

    private bool SearchHeader(ref T_HEADER header)
    {
        int headerlen = Marshal.SizeOf(typeof(T_HEADER));
        if (m_cache[0] != 0xA5) return false;
        byte checkHeader = CRCCheck.GetCRC8(m_cache, headerlen - 3);
        if (checkHeader == m_cache[headerlen - 3])
        {
            header = (T_HEADER)ProtoManager.BytesToStruct(m_cache, 0, typeof(T_HEADER));
            return true;
        }
        return false;
    }
}
