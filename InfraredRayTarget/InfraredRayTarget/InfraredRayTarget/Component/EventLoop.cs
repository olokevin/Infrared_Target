using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class EventLoop
{
	public object m_lock = null;
	private const int queueMax = 1000;
	private Queue<BaseEvent> m_queEvents = null;
	private Dictionary<int, Delegate> m_dicDels = null;

	public EventLoop()
	{
		m_lock = new object();
		m_queEvents = new Queue<BaseEvent>();
		m_dicDels = new Dictionary<int, Delegate>();
	}

	public void AddEvent(int id, Delegate del)
	{
		if (m_dicDels.ContainsKey(id))
		{
			m_dicDels[id] = Delegate.Combine(m_dicDels[id], del);
		}
		else
		{
			m_dicDels.Add(id, del);
		}
	}

	public void RemoveEvent(int evId, Delegate del)
	{
		if (m_dicDels.ContainsKey(evId))
		{
			m_dicDels[evId] = Delegate.Remove(m_dicDels[evId], del);
		}
	}

	public void ClearEvent(int evId)
	{
		if (m_dicDels.ContainsKey(evId))
		{
			m_dicDels.Remove(evId);
		}
	}

	public void PostEvent(BaseEvent ev)
	{
		lock (m_lock)
		{
			if (m_queEvents.Count > queueMax) Debug.LogWarning("Queue is full");
			m_queEvents.Enqueue(ev);
		}
	}

	public BaseEvent GetEvent()
	{
		BaseEvent ev = null;
		lock (m_lock)
		{
			if (m_queEvents.Count > 0) ev = m_queEvents.Dequeue();
		}
		return ev;
	}

	public void SendEvent(BaseEvent ev)
	{
		try
		{
			if (m_dicDels.ContainsKey(ev.id))
			{
				m_dicDels[ev.id].DynamicInvoke(ev);
			}
			else
			{
                Debug.LogWarning("Function not found EventID : " + ev.id);
			}
		}
		catch (Exception e)
		{
            Debug.LogError("Error:{0}\nStackTrace:{1}", e.Message, e.StackTrace);
		}
	}

	public void Run()
	{
		BaseEvent ev = null;
		while (true)
		{
			ev = GetEvent();
			if (ev == null)
			{
				Thread.Sleep(10);
				continue;
			}
			if (ev.id == 0) break;
			SendEvent(ev);
		}
	}

    public void RunInMainThread()
    {
        while (true)
        {
            BaseEvent ev = GetEvent();
            if (ev == null) break;
            if (ev.id == 0) break;
            SendEvent(ev);
        }
    }
}
