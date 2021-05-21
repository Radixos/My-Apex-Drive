using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Singleton<ScriptType> : MonoBehaviour where ScriptType : MonoBehaviour
{
	private static	ScriptType	m_instance				= null;
	private	static	object		m_lock					= new object();
	private	static	GameObject	m_globalsFolder			= null;

	private static	bool		m_missingErrorShown		= false;

	public static bool Exists 
	{ 
		get 
		{
			if (Application.isEditor && m_instance == null)
			{
				m_instance = FindObjectOfType<ScriptType>();
			}

			return m_instance != null;
		}
	}

	public static bool ValidateExists
	{
		get
		{
			if (m_instance != null)
			{
				return true;
			}
			else
			{
#if UNITY_EDITOR
				if (EditorApplication.isPlaying)
				{
					//try to re-validate
					Get();
				}
#endif
				return m_instance != null;
			}
		}
	}

	protected virtual void Awake()
	{

		//< an assert for an already existing instance would be nice here
		m_instance = this as ScriptType;
	}

	public virtual void OnValidate()
	{
		if (Application.isPlaying && gameObject.scene.isLoaded)
		{
			m_instance = this as ScriptType;
		}
	}

	public static ScriptType Instance
	{
		get
		{
            return Get();
		}

		protected set
		{
			m_instance = value;
		}
	}

    public static ScriptType Get()
    {
#if UNITY_EDITOR
		if(!EditorApplication.isPlaying)
		{
			System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(false);

			for (int stackIter = 0; stackIter < stackTrace.FrameCount; stackIter++)
			{
				System.Diagnostics.StackFrame stackFrame = stackTrace.GetFrame(stackIter);

				if(stackFrame.GetMethod().Name.Equals("OnValidate"))
				{
					Debug.LogError("TRYING TO ACCESS SINGLETON OF TYPE " + typeof(ScriptType).Name + " WHEN ENTERING OR LEAVING PLAY MODE, OR FROM VALIDATE FUNCTION. THIS IS USUALY BAD.");

					return null;
				}
			}
		}
#endif
        lock(m_lock)
		{
            if (m_instance == null)
            {
                if (m_globalsFolder == null)
                {
                    m_globalsFolder = GameObject.Find("Globals");

                    if (m_globalsFolder == null)
                    {
                        Debug.Log("Unable to find a Globals folder!");
                    }
                    //DontDestroyOnLoad(m_globalsFolder);
                }

                m_instance = (ScriptType)FindObjectOfType(typeof(ScriptType));

				if (m_instance == null && !m_missingErrorShown)
                {
					Debug.LogError("TRYING TO ACCESS SINGLETON(" + typeof(ScriptType).Name + ") THAT ISN'T IN THE SCENE! PLEASE ADD IT.");
					m_missingErrorShown = true;
				}
            }
			return m_instance;
		}
    }
}
