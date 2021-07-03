using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class BaseService
{
    public BaseService(int serviceId)
    {
        m_serviceId = serviceId;
        m_eventLoop = new EventLoop();
        m_updateLoop = new UpdateLoop();
        m_serviceMgr = ServiceManager.Instance;
    }

    protected string m_serviceName = "BaseService";
    public string ServiceName { get => m_serviceName; set => m_serviceName = value; }

    protected int m_serviceId = 0;
    public int ServiceId { get => m_serviceId; set => m_serviceId = value; }

    public Thread m_thread = null;
    protected EventLoop m_eventLoop = null;
    protected UpdateLoop m_updateLoop = null;
    protected ServiceManager m_serviceMgr = null;
    public EventLoop EventLoop { get { return m_eventLoop; } }

    protected virtual void Awake() { }
    protected virtual void Start() { }
    public virtual void Init() { }
    public virtual void OnExitService() { }

    public void WorkProc()
    {
        Debug.LogWarning("{0} Thread Start Working", GetType().Name);
        Awake();
        Start();
        Run();
        OnExitService();
        Debug.LogWarning("{0} Thread Exit", GetType().Name);
    }

    public virtual void Run()
    {
        m_eventLoop.Run();
    }

    public void AddEvent(int id, Delegate del)
    {
        m_eventLoop.AddEvent(id, del);
    }

    public int AddObject(GameObject obj)
    {
        return m_updateLoop.RegisterGameObject(obj);
    }

    public void RemoveEvent(int id, Delegate del)
    {
        m_eventLoop.RemoveEvent(id, del);
    }

    public void RemoveAllEvent(int id)
    {
        m_eventLoop.ClearEvent(id);
    }

    public void SendEvent(BaseEvent ev)
    {
        m_eventLoop.SendEvent(ev);
    }

    public void PostEvent(BaseEvent ev)
    {
        m_eventLoop.PostEvent(ev);
    }

    public void PostEvent(int serviceId, BaseEvent ev)
    {
        if (serviceId == m_serviceId) m_eventLoop.PostEvent(ev);
        else m_serviceMgr.m_serviceArr[serviceId].PostEvent(serviceId, ev);
    }
}
