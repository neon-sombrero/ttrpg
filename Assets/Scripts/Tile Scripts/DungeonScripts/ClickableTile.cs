using UnityEngine;
using System.Collections;

public class ClickableTile : MonoBehaviour 
{

	public float tileX;
	public float tileZ;
	public TileMap map;

	void OnMouseUp() 
	{
		float PositionOffset = map.tileSize/2.0f;
		Vector2 TilePos = Unit.Player.GetUnscaledPosition();

		map.RequestNav((int)((tileX-PositionOffset)/map.tileSize), (int)((tileZ-PositionOffset)/map.tileSize), (int)TilePos.x, (int)TilePos.y, Unit.Player, Unit.Player.GivePath);
	}//OnMouseUp

}
