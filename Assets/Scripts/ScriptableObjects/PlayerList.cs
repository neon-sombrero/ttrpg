using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public sealed class PlayerClassContainer
{
	public GameObject PlayerModel;
	public PlayerStats.PlayerClass Class = PlayerStats.PlayerClass.None;
}

[System.Serializable]
public sealed class PlayerList : ScriptableObject 
{
	public List<PlayerClassContainer> PlayerPrefabs = new List<PlayerClassContainer>();

	public GameObject GetRelevantModel(PlayerStats.PlayerClass DesiredClass)
	{
		//If the list has a size and isn't null
		if(PlayerPrefabs != null && PlayerPrefabs.Count > 0)
		{
			//For each pref
			for(int i = 0; i < PlayerPrefabs.Count; i++)
			{
				//If it has a class (This shits important...)
				if(PlayerPrefabs[i] != null)
				{
					//If it matches
					if(PlayerPrefabs[i].Class == DesiredClass)
					{
						//And exists
						if(PlayerPrefabs[i].PlayerModel != null)
						{
							//Return the model
							return PlayerPrefabs[i].PlayerModel;
						}
					}
				}
			}
		}

		//Otherwise, done goofed
		return null;
	}

	public static PlayerList GetPlayerList()
	{
		return (PlayerList)Resources.Load("PlayerList", typeof(PlayerList));
	}
}