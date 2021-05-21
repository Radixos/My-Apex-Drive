using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YourGameSystemsManager : GameSystemsManager
{
	protected override void AddAdditionalGameSystems()
	{
		// Add your own game systems here
		AddGameSystem(GameSystem.InstantiateFromAsset("GameManager"));
//		AddGameSystem(GameSystem.CreateInstance<>()); or
//		AddGameSystem(GameSystem.InstantiateFromAsset(""));
	}
}
