using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour 
{
	private bool bSpawned = false;
	public TileMap TM;

	// Use this for initialization
	void Start() 
	{
		if(!bSpawned)
		{
			//Get the loaded playerlist from resources
			PlayerList pList = PlayerList.GetPlayerList();
			if(pList != null)
			{
				GameObject SelectedModel = pList.GetRelevantModel(PlayerStats.playerClass);
				if(SelectedModel != null)
				{
					Vector3 NewPos = transform.position + transform.up * (transform.lossyScale.y * 0.5f);
					GameObject selectedPlayer = GameObject.Instantiate(SelectedModel, NewPos, transform.rotation) as GameObject;

					if(selectedPlayer.GetComponent<Unit>() != null)
					{
						selectedPlayer.GetComponent<Unit>().map = TM;
						float PositionOffset = TM.tileSize/2.0f;
						selectedPlayer.GetComponent<Unit>().SetSpawnLocation((int)((NewPos.x-PositionOffset)/TM.tileSize), (int)((NewPos.z-PositionOffset)/TM.tileSize));
						Unit.Player = selectedPlayer.GetComponent<Unit>();
					}//if

					bSpawned = true;
                    Debug.Log("Player spawned - Level: " + GameController.lvlCount);
				}//if
			}//if
		}//if
	}//Update
}//SpawnScript
