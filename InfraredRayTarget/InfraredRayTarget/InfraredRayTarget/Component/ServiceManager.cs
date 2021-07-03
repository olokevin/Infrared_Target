using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Text;
using System;

public class ServiceManager
{
    private ServiceManager()
    {
        m_serviceArr = new BaseService[MAX_SERVICE];
    }
    private static ServiceManager m_ins;
    public static ServiceManager Instance
    {
        get
        {
            if (m_ins == null) m_ins = new ServiceManager();
            return m_ins;
        }
    }

    public const int MAX_SERVICE = 10;
    public BaseService[] m_serviceArr = null;

    public static BaseService GetService(int serviceId)
    {
        if (serviceId < 0 || serviceId >= m_ins.m_serviceArr.Length) throw new Exception("Get Service IndexOutArrExcption");
        return m_ins.m_serviceArr[serviceId];
    }

    public void Init()
    {
        AddService(ServiceID.SerialPort, new SerialPortService());
        AddService(ServiceID.UI, new UIService());
        AddService(ServiceID.UpdateFirmware, new UpdateFirmwareService());

        for (int i = 0; i < m_serviceArr.Length; ++i)
        {
            if (m_serviceArr[i] != null)
            {
                m_serviceArr[i].Init();
            }
        }

        Debug.LogSuc("ServiceManager Init Success");
    }

    private void AddService(int serviceId, BaseService service)
    {
        m_serviceArr[serviceId] = service;
        if (service != null)
        {
            service.Init();
            service.ServiceName = service.GetType().Name;
        }
    }

    public void Run()
    {
        for (int i = 0; i < m_serviceArr.Length; ++i)
        {
            if (m_serviceArr[i] != null && m_serviceArr[i].ServiceId != ServiceID.UI)
            {
                Thread thread = new Thread(m_serviceArr[i].WorkProc);
                m_serviceArr[i].m_thread = thread;
                thread.IsBackground = true;
                thread.Start();
            }
        }

        Debug.LogSuc("All service run success");
    }

    public void ExitAllThread()
    {
        foreach (BaseService bs in m_serviceArr)
        {
            if (bs != null)
            {
                bs.PostEvent(bs.ServiceId, new ThreadQuitEvent());
            }
        }
    }
}
