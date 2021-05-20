using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemPrefab : GameSystem
{
	public GameObject m_SystemPrefab = null;

	[SerializeField]
	[HideInInspector]
	private GameObject m_PrefabInstance = null;

	public override void Init()
	{
		if(m_SystemPrefab != null)
		{
			m_PrefabInstance = GameObject.Instantiate<GameObject>(m_SystemPrefab);
			if(GameSystemsManager.Exists)
			{
				m_PrefabInstance.transform.SetParent(GameSystemsManager.Instance.transform);
			}
		}	
		base.Init();
	}

	public override void Destroy()
	{
		if(m_PrefabInstance != null)
		{
			Destroy(m_PrefabInstance);
		}

		base.Destroy();
	}
}
