using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EventNotifier : INotifier
{
	private BaseService m_service = null;
    private List<DelegateRef> m_list = new List<DelegateRef>();

    public EventNotifier(int serviceId)
	{
		m_service = ServiceManager.GetService(serviceId);
	}

	public void AddEvent(int id, Delegate del)
	{
		if (m_service == null) return;
		m_service.AddEvent(id, del);
        m_list.Add(new DelegateRef(id, del));
    }

    public void PostEvent(BaseEvent ev)
    {
        if (m_service == null) return;
        m_service.PostEvent(m_service.ServiceId, ev);
    }

    public void PostEvent(int serviceId, BaseEvent ev)
    {
        if (m_service == null) return;
        m_service.PostEvent(serviceId, ev);
    }

    public void SendEvent(BaseEvent ev)
    {
        if (m_service == null) return;
        m_service.SendEvent(ev);
    }

    public void RemoveEvent(int evtId, Delegate del)
    {
        for (int i = 0; i < m_list.Count; ++i)
        {
            if (m_list[i].id == evtId && m_list[i].del == del)
            {
                m_list.RemoveAt(i);
                break;
            }
        }
        m_service.RemoveEvent(evtId, del);
    }

    public void Clear()
	{
        for (int i = 0; i < m_list.Count; ++i)
        {
            m_service.RemoveEvent(m_list[i].id, m_list[i].del);
        }
        m_list.Clear();
    }
}
