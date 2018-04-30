using UnityEngine;
using System.Collections;

[System.Serializable]

public enum EnemyClass
{
	None,
	EnemyFighter,
	EnemyRanged,
	EnemyMage
}

public enum EnemyMoveState
{
	None,
	Stationary,
	Patrol,
	Wander
}

public class EnemyControl : MonoBehaviour
{
	public static EnemyClass enemyClass;
	public static EnemyMoveState enemyMoveState;

	public static void Randomise() {
		// Debug.Log("Randomising enemy class...");
		// Randomise the enemy's Class
		int randomNum = Random.Range(0, 3);
		switch (randomNum) {
			case 0:
				// Debug.Log("Setting enemy class to [Fighter]");
				enemyClass = EnemyClass.EnemyFighter;
				break;

			case 1:
				// Debug.Log("Setting enemy class to [Ranged]");
				enemyClass = EnemyClass.EnemyRanged;
				break;

			case 2:
				// Debug.Log("Setting enemy class to [Mage]");
				enemyClass = EnemyClass.EnemyMage;
				break;

			default:
				// Something went wrong
				// Debug.Log("Could not set an enemy class");
				// Debug.Log("Setting enemy class to [None]");
				enemyClass = EnemyClass.None;
				break;
		}

		// Randomise the int again
		randomNum = Random.Range(0,3);

		// Debug.Log("Randomising enemy move state...");
		// Randomise the enemy's movement state
		switch(randomNum) {
			case 0:
				// Debug.Log("Setting enemy move state to [Stationary]");
				enemyMoveState = EnemyMoveState.Stationary;
				break;

			case 1:
				// Debug.Log("Setting enemy move state to [Patrol]");
				enemyMoveState = EnemyMoveState.Patrol;
				break;

			case 2:
				// Debug.Log("Setting enemy move state to [Wander]");
				enemyMoveState = EnemyMoveState.Wander;
				break;

			default:
				// Something went wrong
				// Debug.Log("Could not set an enemy move state");
				// Debug.Log("Setting enemy move state to [None]");
				enemyMoveState = EnemyMoveState.None;
				break;
		}
	}
}
