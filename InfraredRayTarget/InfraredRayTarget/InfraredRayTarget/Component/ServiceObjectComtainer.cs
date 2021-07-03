using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ServiceObjectContainer
{
	private int m_capacity = 64;
	private GameObject[] m_array;
	private Stack<int> m_indexStack;

	public ServiceObjectContainer()
	{
		m_indexStack = new Stack<int>();
		m_array = new GameObject[m_capacity];
		for (int i = 0; i < m_capacity; ++i)
		{
			m_indexStack.Push(i);
		}
	}

	public IEnumerable<GameObject> Iterator { get => GetIterator(); }
	public IEnumerable<GameObject> GetIterator()
	{
		int i = 0;
		while (i < m_array.Length)
		{
			if (m_array[i] != null)
			{
				yield return m_array[i];
			}
			++i;
		}
	}

	public GameObject GetGameObject(int id)
	{
		return m_array[id];
	}

	public int Add(GameObject obj)
	{
		if (m_indexStack.Count == 0) Expand();
		int index = m_indexStack.Pop();
		m_array[index] = obj;
		return index;
	}

	public void RemoveAt(int index)
	{
		m_array[index] = null;
		m_indexStack.Push(index);
	}

	private void Expand()
	{
		Debug.LogWarning("GameObjectContainer Expand");
		GameObject[] tempArr = new GameObject[m_capacity * 2];
		for (int i = 0; i < m_capacity; ++i)
		{
			tempArr[i] = m_array[i];
		}
		for (int i = 0; i < m_capacity; ++i)
		{
			m_indexStack.Push(i + m_capacity);
		}
		m_array = tempArr;
		m_capacity = m_capacity * 2;
	}
}
