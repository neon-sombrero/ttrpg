using UnityEngine;
using System.Collections.Generic;

// Function to spawn prefabs over the 2d map
public class Utils {
  public void MakeWalls(int[,] map_data, int size_x, int size_y) {
      for (int x = 0; x < size_x; x++) {
          for (int y = 0; y < size_y; y++) {
              if (map_data[x, y] == 3 && HasAdjacentFloor(map_data, size_x, size_y, x, y)) {
                  map_data[x, y] = 2;
              }//if
          }//for
      }//for

      for (int x = 0; x < size_x; x++) {
          for (int y = 0; y < size_y; y++) {
              if (map_data[x, y] == 2 && strayWall(map_data, size_x, size_y,  x, y) == true) {
                  map_data[x, y] = 1;
              }//if
          }//if
      }//if

      for (int x = 0; x < size_x; x++) {
          for (int y = 0; y < size_y; y++) {
              if (map_data[x, y] == 2 && isPillar(map_data, size_x, size_y,  x, y) == true) {
                  map_data[x, y] = 4;
              }//if
          }//if
      }//if

      for (int x = 0; x < size_x; x++) {
          for (int y = 0; y < size_y; y++) {
              if (map_data[x, y] == 1 && isDoor(map_data, size_x, size_y,  x, y) == true && HasAdjacentDoor(map_data, size_x, size_y,  x, y) == false) {
                  map_data[x, y] = 7;
              }//if
          }//if
      }//if
  }//MakeWalls

  public void MakePortals(int[,] map_data, int x, int y, int i, int j) {
      map_data[x, y] = 5;
      map_data[i, j] = 6;
  }//MakePortals

  public void MakePrefabs(int[,] map_data, int size_x, int size_y, TileMap TTRef) {
      GameObject Container = new GameObject("Map");

      List<GameObject> floorTiles = new List<GameObject>();

      float PositionOffset = TTRef.tileSize / 2.0f;
      float ZPosOffset = TTRef.tileSize * size_y;

      for (int x = 0; x < size_x; x++) {
          for (int y = 0; y < size_y; y++) {
              TileType tt = TTRef.tileTypes[map_data[x, y]]; //map_data will equal 0,1,2 or 3

              //If not empty blue
              if (map_data[x, y] != 3) {
                  //Spawns the prefabs ontop of the map, the walls are 1f higher than the rest.
                  if (map_data[x, y] == 2 || map_data[x, y] == 4) {
                      Vector3 WorldPosition = new Vector3((x * TTRef.tileSize) + PositionOffset, 2 * TTRef.tileSize, ((y * TTRef.tileSize) + PositionOffset) - ZPosOffset);

                      GameObject go = (GameObject)GameObject.Instantiate(tt.tileVisualPrefab, WorldPosition, Quaternion.identity);
                      go.transform.localScale *= TTRef.tileSize;
                      go.transform.parent = Container.transform;
                      TTRef.CreateObjects.Add(go);
                  }//if
                  else {
                      Vector3 WorldPosition = new Vector3((x * TTRef.tileSize) + PositionOffset, 1 * TTRef.tileSize, ((y * TTRef.tileSize) + PositionOffset) - ZPosOffset);

                      GameObject go = (GameObject)GameObject.Instantiate(tt.tileVisualPrefab, WorldPosition, Quaternion.identity);
                      go.transform.localScale *= TTRef.tileSize;
                      go.transform.parent = Container.transform;
                      TTRef.CreateObjects.Add(go);

                      if (go.GetComponent<ClickableTile>() == null) {
                          go.AddComponent<ClickableTile>();
                      }

                      if (go.GetComponent<ClickableTile>() != null) {
                          ClickableTile ct = go.GetComponent<ClickableTile>();
                          ct.tileX = WorldPosition.x;
                          ct.tileZ = WorldPosition.z;
                          ct.map = TTRef;
                      }

                      if (go.GetComponent<SpawnScript>() != null) {
                          go.GetComponent<SpawnScript>().TM = TTRef;
                      }

                      //If the map data is a floor tile and doesn't lay in the spawn room, add the gameObject to the floorTiles list.
                      if (map_data[x, y] == 1 && map_data[x, y] != 8) {
                          floorTiles.Add(go);
                      }//if

                  }//else
              }//if
          }//for
      }//for
      TTRef.CreateObjects.Add(Container);

      //Shuffle floorTiles list and randomly add an ememy spawn script to some.
      Shuffle(floorTiles);
  }//MakePrefabs

  void Shuffle(List<GameObject> list) {
      // Loops through array
      for (int i = list.Count - 1; i > 0; i--) {
          // Randomize a number between 0 and i (so that the range decreases each time)
          int rnd = Random.Range(0, i);

          // Save the value of the current i, otherwise it'll overright when we swap the values
          GameObject temp = list[i];

          // Swap the new and old values
          list[i] = list[rnd];
          list[rnd] = temp;
      }//for

      //Change this to scale with the level
      int MaxEnemies = GameController.lvlCount + 4;
      MaxEnemies = Mathf.Clamp(MaxEnemies, 1, 20);

      int numOfEnemies = 0;

      for (int x = 0; x < list.Count; x++) {
          if ((Random.Range(0, 11) > 9) && (numOfEnemies < MaxEnemies) && (list[x].GetComponent<EnemySpawnScript>() == null)) {
              list[x].AddComponent<EnemySpawnScript>();
              numOfEnemies++;
          }
      }
      Debug.Log("Number of enemies on map: " + numOfEnemies);
  }//Shuffle

  public bool HasAdjacentFloor(int[,] map_data, int size_x, int size_y, int x, int y) {
      if (x > 0 && map_data[x - 1, y] == 1)
          return true;
      if (x < size_x - 1 && map_data[x + 1, y] == 1)
          return true;
      if (y > 0 && map_data[x, y - 1] == 1)
          return true;
      if (y < size_y - 1 && map_data[x, y + 1] == 1)
          return true;

      if (x > 0 && y > 0 && map_data[x - 1, y - 1] == 1)
          return true;
      if (x < size_x - 1 && y > 0 && map_data[x + 1, y - 1] == 1)
          return true;

      if (x > 0 && y < size_y - 1 && map_data[x - 1, y + 1] == 1)
          return true;
      if (x < size_x - 1 && y < size_y - 1 && map_data[x + 1, y + 1] == 1)
          return true;

      return false;
  }//HasAdjacentFloor

  public bool HasAdjacentDoor(int[,] map_data, int size_x, int size_y, int x, int y) {
      if (x > 0 && map_data[x - 1, y] == 7) {
          return true;
      }
      if (x < size_x - 1 && map_data[x + 1, y] == 7) {
          return true;
      }
      if (y > 0 && map_data[x, y - 1] == 7) {
          return true;
      }
      if (y < size_y - 1 && map_data[x, y + 1] == 7) {
          return true;
      }

      return false;
  }//HasAdjacentDoor

  //Turn stray wall tiles into floor tiles
  public bool strayWall(int[,] map_data, int size_x, int size_y, int x, int y) {
      if (x > 0 && map_data[x - 1, y] != 2) {

          if (x < size_x - 1 && map_data[x + 1, y] != 2) {

              if (y > 0 && map_data[x, y - 1] != 2) {
                  if (y < size_y - 1 && map_data[x, y + 1] != 2) {
                      return true;
                  }//if
              }//if
          }//if
      }//if
      return false;
  }//StrayWall

  public bool isPillar(int[,] map_data, int size_x, int size_y, int x, int y) {
      //Number of walls on left and right
      int NumX = 0;
      //Number if walls on front and back
      int NumY = 0;

      int NumDiags = 0;

      //Check Left is in range
      if (x - 1 >= 0) {
          //It's in range
          if (map_data[x - 1, y] == 2 || map_data[x - 1, y] == 4) {
              NumX++;
          }
      }

      //Check Right is in range
      if (x + 1 < size_x) {
          //It's in range
          if (map_data[x + 1, y] == 2 || map_data[x + 1, y] == 4) {
              NumX++;
          }
      }

      //Check Top is in range
      if (y - 1 >= 0) {
          //It's in range
          if (map_data[x, y - 1] == 2 || map_data[x, y - 1] == 4) {
              NumY++;
          }
      }

      //Check Bottom is in range
      if (y + 1 < size_y) {
          //It's in range
          if (map_data[x, y + 1] == 2 || map_data[x, y + 1] == 4) {
              NumY++;
          }
      }

      //Check TopLeft
      if (x - 1 >= 0 && y - 1 >= 0) {
          //It's in range
          if (map_data[x - 1, y - 1] != 2 && map_data[x - 1, y - 1] != 4) {
              NumDiags++;
          }
      }

      //Check TopRight
      if (x + 1 < size_x && y - 1 >= 0) {
          //It's in range
          if (map_data[x + 1, y - 1] != 2 && map_data[x + 1, y - 1] != 4) {
              NumDiags++;
          }
      }

      //Check BottomLeft
      if (x - 1 >= 0 && y + 1 < size_y) {
          //It's in range
          if (map_data[x - 1, y + 1] != 2 && map_data[x - 1, y + 1] != 4) {
              NumDiags++;
          }
      }

      //Check BottomRight
      if (x + 1 < size_x && y + 1 < size_y) {
          //It's in range
          if (map_data[x + 1, y + 1] != 2 && map_data[x + 1, y + 1] != 4) {
              NumDiags++;
          }
      }

      //T Junc
      if (NumX == 2 && NumY == 1 && (NumDiags >= 3)) {
          return true;
      }
      //TJunc
      if (NumX == 1 && NumY == 2 && (NumDiags >= 3)) {
          return true;
      }
      //Corner or straight
      if (NumX == 1 && NumY == 1) {
          return true;
      }

      if (NumX == 1 && NumY == 0) {
          return true;
      }

      if (NumX == 0 && NumY == 1) {
          return true;
      }

      return false;
  }//isPillar

  public bool isDoor(int[,] map_data, int size_x, int size_y, int x, int y) {
      //Number of pillars on left and right
      int NumX = 0;
      //Number if pillars on front and back
      int NumY = 0;

      //Check Left is in range
      if (x - 1 >= 0) {
          //It's in range
          if (map_data[x - 1, y] == 4) {
              NumX++;
          }
      }

      //Check Right is in range
      if (x + 1 < size_x) {
          //It's in range
          if (map_data[x + 1, y] == 4) {
              NumX++;
          }
      }

      //Check Top is in range
      if (y - 1 >= 0) {
          //It's in range
          if (map_data[x, y - 1] == 4) {
              NumY++;
          }
      }

      //Check Bottom is in range
      if (y + 1 < size_y) {
          //It's in range
          if (map_data[x, y + 1] == 4) {
              NumY++;
          }
      }

      //Straight Horizontal
      if (NumX == 2 && NumY == 0) {
          return true;
      }

      //Straight Vertical
      if (NumX == 0 && NumY == 2) {
          return true;
      }

      return false;
  }//isDoor
}
