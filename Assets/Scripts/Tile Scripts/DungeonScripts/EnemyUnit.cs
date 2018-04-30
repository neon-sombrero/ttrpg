using UnityEngine;
using System.Collections.Generic;

public class EnemyUnit : MonoBehaviour
{
	public int tileX;
	public int tileZ;
	public TileMap map;

	public List<Node> currentPath = null;
	private bool bIsInCombat = false;

	int moveSpeed = 2;

	void Start()
	{

	}

	void Update()
	{
		if(map != null)
		{
			if(currentPath != null)
			{
				int currNode = 0;
				while(currNode < currentPath.Count-1)
				{
					int TmpNodeZ = currentPath[currNode].z;
					Vector3 start = map.TileCoordToWorldCoord((int)(currentPath[currNode].x), (int)(TmpNodeZ));
					int TmpNodeZEnd = currentPath[currNode+1].z;
					Vector3 end = map.TileCoordToWorldCoord((int)(currentPath[currNode+1].x), (int)(TmpNodeZEnd));

					Debug.DrawLine (start, end, Color.red);

					currNode++;
				}//while
			}//if

			//Smoothly animate towards target tile
			transform.position = Vector3.Lerp(transform.position, map.TileCoordToWorldCoord((int)(tileX), (int)(tileZ)), 5f * Time.deltaTime);

			if(bIsInCombat)
			{
				MoveNextTile(moveSpeed);
			}
			else
			{
				MoveNextTile();
			}
		}
	}//Update

	public void GivePath(List<Node> NewPath, bool Successful)
	{
		if(Successful)
		{
			currentPath = NewPath;
		}
	}

	/// <summary>
	/// Sets the spawn location, by taking the location of the
	/// spawn prefab and copying it over
	/// </summary>
	public void SetSpawnLocation(int spx, int spy)
	{
		//GridSize Scaling
		tileX = spx;
		tileZ = spy;
		tileZ = map.size_z + tileZ;
		//Z is now grid aligned
	}

	public Vector2 GetUnscaledPosition()
	{
		return new Vector2(tileX, tileZ);
	}

	public void MoveNextTile(int MoveSpeed = 10000)
	{
		float remainingMovement = MoveSpeed;

		while(remainingMovement > 0)
		{
			if(currentPath == null)
			{
				return;
			}

			//Get cost from current tile to next tile
			remainingMovement -= map.CostToEnterTile(currentPath[0].x, currentPath[0].z, currentPath[1].x, currentPath[1].z);

			//Move us to the next tile in the sequence
			tileX = currentPath[1].x;
			tileZ = currentPath[1].z;

			//If we are within a specific range
			if(Vector3.Distance(map.TileCoordToWorldCoord(tileX, tileZ), transform.position) < 1.0f)
			{
				//Remove old 'current' tile
				currentPath.RemoveAt(0);
			}

			if(currentPath.Count == 1)
			{
				//We only have one tile left in the path, and that tile must be our ultimate
				//destination, and we are standing on it.
				//So let's just clear our pathfinding info.
				currentPath = null;
			}
		}
	}

}//Unit
