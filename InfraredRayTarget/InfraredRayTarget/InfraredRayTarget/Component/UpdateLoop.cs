using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UpdateLoop
{
	private ServiceObjectContainer m_container = null;

	public UpdateLoop()
	{
		m_container = new ServiceObjectContainer();
	}

	public GameObject GetObject(int objId)
	{
		return m_container.GetGameObject(objId);
	}

	public int RegisterGameObject(GameObject obj)
	{
		return m_container.Add(obj);
	}

	public void RemoveGameObject(GameObject obj)
	{
		m_container.RemoveAt(obj.InnerID);
	}

	public void Update()
	{
		var colletion = m_container.GetIterator();
		foreach (GameObject cur in colletion)
		{
			if (cur.started == false)
			{
				cur.Start();
				cur.started = true;
			}
			if (cur.destroyed == false && cur.enable)
			{
				cur.Update();
			}
		}
	}

	public void Destroy()
	{
		var colletion = m_container.GetIterator();
		foreach (GameObject cur in colletion)
		{
			cur.OnDestroy();
		}
	}
}
