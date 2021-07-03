using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameObject
{
	private int m_innerID;
	public bool enable;
	public bool started;
	public bool destroyed;
	private BaseService m_service;
	public int InnerID { get => m_innerID; }

	public GameObject(int serviceId)
	{
		enable = true;
		started = false;
		destroyed = false;
		m_service = ServiceManager.GetService(serviceId);
		m_innerID = m_service.AddObject(this);
	}

	public virtual void Start() { }
	public virtual void Update() { }
	public virtual void OnDestroy() { }

	public static void Destroy(GameObject obj)
	{
		if (obj == null) return;
		if (obj.destroyed) return;
		obj.OnDestroy();
		obj.destroyed = true;
	}

	public static void SetActive(GameObject obj, bool active)
	{
		obj.enable = active;
	}

	public void AddEvent(int id, Delegate del)
	{
		m_service.AddEvent(id, del);
	}

	public void RemoveEvent(int id, Delegate del)
	{
		m_service.RemoveEvent(id, del);
	}

	public void RemoveAllEvent(int id)
	{
		m_service.RemoveAllEvent(id);
	}

	public void SendEvent(BaseEvent ev)
	{
		m_service.SendEvent(ev);
	}

	public void PostEvent(int svcid, BaseEvent ev)
	{
		m_service.PostEvent(svcid, ev);
	}

	public static bool operator ==(GameObject obj1, GameObject obj2)
	{
		return CompareBase(obj1, obj2);
	}

	public static bool operator !=(GameObject obj1, GameObject obj2)
	{
		return !CompareBase(obj1, obj2);
	}

	private static bool CompareBase(GameObject obj1, GameObject obj2)
	{
		bool obj1_exist = !ReferenceEquals(obj1, null);
		bool obj2_exist = !ReferenceEquals(obj2, null);
		if (obj1_exist && obj1.destroyed) obj1_exist = false;
		if (obj2_exist && obj2.destroyed) obj2_exist = false;
		if ((obj1_exist != obj2_exist) || !obj1_exist) return obj1_exist == obj2_exist;
		else return ReferenceEquals(obj1, obj2);
	}
}
