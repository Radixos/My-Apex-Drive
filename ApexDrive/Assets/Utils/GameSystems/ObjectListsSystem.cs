using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectListsSystem : GameSystem
{
	[System.Serializable]
	private class MonoBehaviourList
	{
		public List<MonoBehaviour> List;
	}

	private Dictionary<System.Type, List<MonoBehaviour>> m_AllLists;
	[SerializeField]
	private List<MonoBehaviourList> m_SerializedList = new List<MonoBehaviourList>();

	public static List<MonoBehaviour> GetObjectList<T>() where T : MonoBehaviour
	{
		if (GameSystemsManager.Exists)
		{
			ObjectListsSystem objectListSystem = GameSystemsManager.Instance.GetGameSystem<ObjectListsSystem>();
			if (objectListSystem != null)
			{
				if (objectListSystem.m_AllLists == null)
				{
					objectListSystem.RecoverAfterRecompile();
				}
				return objectListSystem.GetList<T>();
			}
		}
		return null;
	}

	public static List<MonoBehaviour> AddObjectToList<T>(T obj) where T : MonoBehaviour
	{
		if (GameSystemsManager.Exists)
		{
			ObjectListsSystem objectListSystem = GameSystemsManager.Instance.GetGameSystem<ObjectListsSystem>();
			if (objectListSystem)
			{
				if (objectListSystem.m_AllLists == null)
				{
					objectListSystem.RecoverAfterRecompile();
				}

				List<MonoBehaviour> list = objectListSystem.GetList<T>();
				list.Add(obj);
				return list;
			}
		}
		return null;
	}

	public static List<MonoBehaviour> RemoveObjectFromList<T>(T obj) where T : MonoBehaviour
	{
		if (GameSystemsManager.Exists)
		{
			ObjectListsSystem objectListSystem = GameSystemsManager.Instance.GetGameSystem<ObjectListsSystem>();
			if (objectListSystem)
			{
				if (objectListSystem.m_AllLists == null)
				{
					objectListSystem.RecoverAfterRecompile();
				}
				List<MonoBehaviour> list = objectListSystem.GetList<T>();
				list.Remove(obj);
				return list;
			}
		}
		return null;
	}

	public override void Init()
	{
		m_AllLists = new Dictionary<System.Type, List<MonoBehaviour>>();
	}

	public override void RecoverAfterRecompile()
	{
		m_AllLists = new Dictionary<System.Type, List<MonoBehaviour>>();
		for(int i = 0; i < m_SerializedList.Count; i++)
		{
			if(m_SerializedList[i].List.Count > 0)
			{
				m_AllLists.Add(m_SerializedList[i].List[0].GetType(), m_SerializedList[i].List);
			}
		}
		base.RecoverAfterRecompile();
	}

	private List<MonoBehaviour> GetOrCreateList<T>()
	{
		List<MonoBehaviour> newList = null;
		if(!m_AllLists.TryGetValue(typeof(T), out newList))
		{
			newList = new List<MonoBehaviour>();
			m_AllLists.Add(typeof(T), newList);
			m_SerializedList.Add(new MonoBehaviourList() { List = newList });
		}
		return newList;
	}

	public override void Destroy()
	{
		Dictionary<System.Type, List<MonoBehaviour>>.Enumerator enumerator = m_AllLists.GetEnumerator();
		while(enumerator.MoveNext())
		{
			enumerator.Current.Value.Clear();
		}
		m_AllLists.Clear();
	}

	public List<MonoBehaviour> GetList<T>() where T : MonoBehaviour
	{
		return GetOrCreateList<T>();
	}

	public void Add<T>(T obj) where T : MonoBehaviour
	{
		List<MonoBehaviour> list = GetOrCreateList<T>();		
		Debug.Assert(!list.Contains(obj), "Tried adding Object already in ObjectList, Type: " + typeof(T).ToString() + ", from GameObject: " + obj.gameObject != null ? obj.gameObject.name : "*", obj.gameObject);
		list.Add(obj);
	}

	public void Remove<T>(T obj) where T : MonoBehaviour
	{
		List<MonoBehaviour> list = GetOrCreateList<T>();
		Debug.Assert(list.Contains(obj), "Tried removing Object not in ObjectList, Type: " + typeof(T).ToString() + ", from GameObject: " + obj.gameObject != null ? obj.gameObject.name : "*", obj.gameObject);
		list.Remove(obj);		
	}
}
