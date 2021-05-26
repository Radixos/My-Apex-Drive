using UnityEngine;

public abstract class GameSystem : ScriptableObject
{
	public virtual void Init() { }
	public virtual void RecoverAfterRecompile() { }
	public virtual void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode) { }
	public virtual void Update() { }
	public virtual void Destroy()
	{
		ScriptableObject.Destroy(this);
	}

	public static GameSystem InstantiateFromAsset(string path)
	{
		if (path != "")
		{
			GameSystem system = Resources.Load(path) as GameSystem;
			if(system != null)
			{
				return Instantiate<GameSystem>(system);
			}
		}

		return null;
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem("Assets/Create/Game System File From Selected Script")]
	static void CreateGameSystemFileFromSelectedScript()
	{
		UnityEditor.MonoScript script = UnityEditor.Selection.objects[0] as UnityEditor.MonoScript;

		string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save location", script.name, "asset", "", "Assets/Data/Resources/GameSystems/");
		if (string.IsNullOrEmpty(path))
			return;

		string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);

		GameSystem gameSystem = ScriptableObject.CreateInstance(script.GetClass()) as GameSystem;
		UnityEditor.AssetDatabase.CreateAsset(gameSystem, assetPathAndName);
		UnityEditor.AssetDatabase.SaveAssets();
		UnityEditor.EditorUtility.FocusProjectWindow();
		UnityEditor.Selection.activeObject = gameSystem;
	}

	[UnityEditor.MenuItem("Assets/Create/Game System File From Selected Script", true)]
	static bool CreateGameSystemFileFromSelectedScript_Validator()
	{
		if(UnityEditor.Selection.objects != null && UnityEditor.Selection.objects.Length == 1 && UnityEditor.Selection.objects[0] is UnityEditor.MonoScript
			&& (UnityEditor.Selection.objects[0] as UnityEditor.MonoScript).GetClass().IsSubclassOf(typeof(GameSystem)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
#endif
}
