using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public sealed class EnemyContainer
{
	public GameObject EnemyModel;
	public EnemyClass Class;
}

[System.Serializable]
public sealed class EnemyList : ScriptableObject
{
	public List<EnemyContainer> EnemyPrefabs = new List<EnemyContainer>();

	// TODO - Still think this needs a look at
	// Should be just an 'object' with the class and its prefab
	// Shouldn't have to go and look for its prefab

	/* public GameObject GetRelevantModel(EnemyClass DesiredClass)
	{
		Debug.Log("Getting relevent enemy model...");
		//If the list has a size and isn't null
		if(EnemyPrefabs != null && EnemyPrefabs.Count > 0)
		{
			Debug.Log("Enemy Prefabs exist");
			Debug.Log("Num of prefabs: " + EnemyPrefabs.Count);

			// Pull the prefab from the List
			EnemyClass filteredClass = EnemyPrefabs.Class.Find(DesiredClass);
			if(filteredClass.length() != 0) {
				return filteredClass.EnemyModel;
			}
			Debug.Log("Couldn't find enemy prefab for: " + DesiredClass);
		}

		//Otherwise, done goofed
		return null;
	} */

	public GameObject GetRelevantModel(EnemyClass DesiredClass)
	{
		//If the list has a size and isn't null
		if(EnemyPrefabs != null && EnemyPrefabs.Count > 0)
		{
			//For each pref
			for(int i = 0; i < EnemyPrefabs.Count; i++)
			{
				//
				if(EnemyPrefabs[i] != null)
				{
					//If it matches
					if(EnemyPrefabs[i].Class == DesiredClass)
					{
						//And exists
						if(EnemyPrefabs[i].EnemyModel != null)
						{
							//Return the model
							return EnemyPrefabs[i].EnemyModel;
						}
					}
				}
			}
		}

		//Otherwise, done goofed
		return null;
	}

	public static EnemyList GetEnemyList()
	{
		return (EnemyList)Resources.Load("EnemyList", typeof(EnemyList));
	}
}
