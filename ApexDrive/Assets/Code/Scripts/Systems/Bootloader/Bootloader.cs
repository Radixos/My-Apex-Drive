using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootloader
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Bootload()
	{
		GameSystemsManager.Create<YourGameSystemsManager>();
	}
}
