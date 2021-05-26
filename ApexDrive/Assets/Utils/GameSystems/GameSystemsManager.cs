using UnityEngine;
using System.Collections.Generic;

public class GameSystemsManager : Singleton<GameSystemsManager>
{
	public static T Create<T>() where T : GameSystemsManager
	{
		GameObject mainSystemObject = new GameObject("Systems");
		Object.DontDestroyOnLoad(mainSystemObject);
		return mainSystemObject.AddComponent<T>();
	}

	[SerializeField]
	List<GameSystem> m_GameSystems;

	protected virtual void AddAdditionalGameSystems()
	{

	}

	protected void AddGameSystem(GameSystem system)
	{
		m_GameSystems.Add(system);
	}

	public T GetGameSystem<T>() where T : GameSystem
	{
		if (m_GameSystems != null)
		{
			for (int i = 0; i < m_GameSystems.Count; i++)
			{
				if(m_GameSystems[i] is T)
				{
					return (T)m_GameSystems[i];
				}
			}
		}

		return null;
	}

	protected override sealed void Awake()
	{
		base.Awake();

		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

		m_GameSystems = new List<GameSystem>();

		AddAdditionalGameSystems();

		for (int i = 0; i < m_GameSystems.Count; i++)
		{
			m_GameSystems[i].Init();
		}
	}

	public override sealed void OnValidate()
	{
		base.OnValidate();

		if (Application.isPlaying && gameObject.scene.isLoaded)
		{
			if (m_GameSystems != null)
			{
				for (int i = 0; i < m_GameSystems.Count; i++)
				{
					m_GameSystems[i].RecoverAfterRecompile();
				}
			}
		}
	}

	private void Update()
	{
		if(m_GameSystems != null)
		{
			for(int i = 0; i < m_GameSystems.Count; i++)
			{
				m_GameSystems[i].Update();
			}
		}
	}

	private void OnDestroy()
	{
		if(m_GameSystems != null)
		{
			for(int i = 0; i < m_GameSystems.Count; i++)
			{
				m_GameSystems[i].Destroy();
			}
		}
	}

	private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
	{
		if (m_GameSystems != null)
		{
			for (int i = 0; i < m_GameSystems.Count; i++)
			{
				m_GameSystems[i].OnSceneLoaded(scene, loadingMode);
			}
		}
	}
}
