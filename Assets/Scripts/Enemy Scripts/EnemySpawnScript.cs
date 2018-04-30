using UnityEngine;
using System.Collections;

public class EnemySpawnScript : MonoBehaviour
{
	public GameObject selectedEnemy;
	private TileMap TM;

	void Start()
	{
		if(GameController.mapIsDone == true)
		{
			TM = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
			//Get the loaded enemylist from resources
			EnemyList eList = EnemyList.GetEnemyList();

			// Randomise enemies
			EnemyControl.Randomise();
			if(eList != null)
			{
				GameObject SelectedModel = eList.GetRelevantModel(EnemyControl.enemyClass);
				if(SelectedModel != null)
				{
					Vector3 NewPos = transform.position + transform.up * (transform.lossyScale.y * 0.5f);
					selectedEnemy = GameObject.Instantiate(SelectedModel, NewPos, transform.rotation) as GameObject;

					if(selectedEnemy != null)
					{
						if(selectedEnemy.GetComponent<EnemyUnit>() != null)
						{
							selectedEnemy.GetComponent<EnemyUnit>().map = TM;
							float PositionOffset = TM.tileSize/2.0f;
							selectedEnemy.GetComponent<EnemyUnit>().SetSpawnLocation((int)((NewPos.x-PositionOffset)/TM.tileSize), (int)((NewPos.z-PositionOffset)/TM.tileSize));
						}//if
					}//if
				}//if
			}//if
		}//if
	}//Start
}//EnemySpawnScript
