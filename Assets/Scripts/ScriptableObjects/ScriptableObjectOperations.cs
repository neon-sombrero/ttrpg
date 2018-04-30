using UnityEngine;
using UnityEditor;

public class ScriptableObjectOperations
{
	[MenuItem("Assets/Create/Custom/Player List")]
	public static void CreatePlayerList ()
	{
		ScriptableObjectUtility.CreateAsset<PlayerList> ();
	}

	[MenuItem("Assets/Create/Custom/Enemy List")]
	public static void CreateEnemyList ()
	{
		ScriptableObjectUtility.CreateAsset<EnemyList> ();
	}
}