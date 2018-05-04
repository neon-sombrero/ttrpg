using UnityEngine;
using System.Collections.Generic;

public class DTileMap : DungeonRoom {
    Utils utilsInstance = new Utils();

    int size_x;
    int size_y;

    int[,] map_data;

    public int spawnPointX, spawnPointY, exitPointX, exitPointY;

    List<DRoom> rooms = new List<DRoom>();

    public int[,] GetMapData() {
      return map_data;
    }

    public DTileMap(int sizeX, int sizeY, TileMap TTRef) {
        GameController.mapIsDone = false;
        this.size_x = sizeX;
        this.size_y = sizeY;

        map_data = new int[size_x, size_y];

        // Set all tiles to the Stone (floor) tile
        for (int x = 0; x < size_x; x++) {
            for (int y = 0; y < size_y; y++) {
                map_data[x, y] = 3;
            }//for
        }//for

        // Create the spawn and exit rooms
        MakeSpawnRoom();

        int maxFails = 50;

        int randomRoomCountTotal = Random.Range(8, 16);
        while (rooms.Count <= randomRoomCountTotal) {
          // Randomise the sizes of each room
          int roomSizeX = Random.Range(6, 15);
          int roomSizeY = Random.Range(6, 11);

          DRoom room = new DRoom();
          room.left = Random.Range(0, size_x - roomSizeX);
          room.top = Random.Range(0, size_y - roomSizeY);
          room.width = roomSizeX;
          room.height = roomSizeY;

          if (!RoomCollides(room)) {
              rooms.Add(room);
          }//if
          else {
              maxFails--;
              if (maxFails <= 0)
                  break;
          }//else
        }//while

        foreach (DRoom r2 in rooms) {
            MakeRoom(r2);
        }//foreach

        //Allow the system to build rooms and set up connections
        for (int i = 0; i < rooms.Count; i++) {
            if (true)//!rooms[i].isConnected)
            {
                int j = Random.Range(1, rooms.Count);
                MakeCorridor(rooms[i], rooms[(i + j) % rooms.Count]);
            }//if
        }//for

        // Create functions pulled in from Utils
        utilsInstance.MakeWalls(map_data, size_x, size_y);
        utilsInstance.MakePortals(map_data, spawnPointX, spawnPointY, exitPointX, exitPointY);
        utilsInstance.MakePrefabs(map_data, size_x, size_y, TTRef);

        GameController.mapIsDone = true;
    }//DTileMap

    bool RoomCollides(DRoom r) {
        foreach (DRoom r2 in rooms) {
            if (r.CollidesWith(r2)) {
                return true;
            }//if
        }//foreach

        return false;
    }//RoomCollides

    public int GetTileAt(int x, int y) {
        return map_data[x, y];
    }//GetTileAt

    // Until I find out what the craic is
    // void BuildSpawnRoom(int spawnSizeX, int spawnSizeY) {
    //   // Possible room sizes
    //   int[] sizes = new int[] {5,7,9,11};
    //
    //   // Randomise both sizes
    //   spawnSizeX = sizes[Random.Range(0, 4)];  // Spawn room Size X
    //   spawnSizeY = sizes[Random.Range(0, 4)];  // Spawn room Size Y
    //   Debug.Log("Building a spawn room - size x: " + spawnSizeX + " & size y: " + spawnSizeY);
    // }

    void CalculateRoomSpawnPoint(int sizeX, int sizeY, int left, int top) {
      // Calculate the mid point of each size and round down
      int midX = Mathf.FloorToInt(sizeX/2);
      int midY = Mathf.FloorToInt(sizeY/2);

      // Add the mid point to left and top
      spawnPointX = left + midX;
      spawnPointY = top + midY;
      // Debug.Log("Building a spawn point - x: " + spawnPointX + " & y: " + spawnPointY);
    }

    void CalculateExitRoomSpawnPoint(int sizeX, int sizeY, int left, int top) {
      // Calculate the mid point of each size and round down
      int midX = Mathf.FloorToInt(sizeX/2);
      int midY = Mathf.FloorToInt(sizeY/2);

      // Add the mid point to left and top
      exitPointX = left + midX;
      exitPointY = top + midY;
      // Debug.Log("Building a exit point - x: " + exitPointX + " & y: " + exitPointY);
    }

    /// <summary>
    /// Chooses a corner of the map randomly, then spawns a room there. Based on the
    /// position of the Spawn room, an exit room is made on the opposite side in a different corner.
    /// </summary>
    void MakeSpawnRoom() {
        //Make a spawn room that always appears in one of the corners of the map
        //Should be in a list or library of rooms

        // Calculate where the room should go based on how big it is
        // The map is 30x60 so we cannot exceed 29 on the x, or 59 on the Y
        // Or else we will mess up the nice walls around the outside

        // TODO - Find out why this doesn't work as expected
        // Randomise Spawn Room size
        // Default sizes to 9
        // int spawnSizeX = 9;
        // int spawnSizeY = 9;
        // BuildSpawnRoom(spawnSizeX, spawnSizeY);

        // Possible room sizes
        int[] sizes = new int[] {5,7,9,11};

        // Randomise both sizes
        int spawnSizeX = sizes[Random.Range(0, 4)];  // Spawn room Size X
        int spawnSizeY = 9; // Default to 9
        if (spawnSizeX == 11) {
          spawnSizeY = sizes[Random.Range(0, 3)];   // Spawn room Size Y
        } else {
          spawnSizeY = sizes[Random.Range(0, 4)];   // Spawn room Size Y
        }
        int spawnSite = Random.Range(0, 4);         //Spawn Site

        // Debug.Log("Using spawn sizes - size x: " + spawnSizeX + " & size y: " + spawnSizeY);

        DRoom spawnRoom = new DRoom();
        //Chooses between four different spawn sites
        if (spawnSite == 0) {
            spawnRoom.left = 1;
            spawnRoom.top = 1;
            // Debug.Log("spawnRoom.left: " + spawnRoom.left + " spawnRoom.top: " + spawnRoom.top);
            //Make the spawn point
            CalculateRoomSpawnPoint(spawnSizeX, spawnSizeY, spawnRoom.left, spawnRoom.top);
        }
        if (spawnSite == 1) {
            spawnRoom.left = 1;
            spawnRoom.top = (30-(spawnSizeY+1));
            // Debug.Log("spawnRoom.left: " + spawnRoom.left + " spawnRoom.top: " + spawnRoom.top);
            //Make the spawn point
            CalculateRoomSpawnPoint(spawnSizeX, spawnSizeY, spawnRoom.left, spawnRoom.top);
        }
        if (spawnSite == 2) {
            spawnRoom.left = (60-(spawnSizeX+1));
            spawnRoom.top = 1;
            // Debug.Log("spawnRoom.left: " + spawnRoom.left + " spawnRoom.top: " + spawnRoom.top);
            //Make the spawn point
            CalculateRoomSpawnPoint(spawnSizeX, spawnSizeY, spawnRoom.left, spawnRoom.top);
        }
        if (spawnSite == 3) {
            spawnRoom.left = (60-(spawnSizeX+1));
            spawnRoom.top = (30-(spawnSizeY+1));
            // Debug.Log("spawnRoom.left: " + spawnRoom.left + " spawnRoom.top: " + spawnRoom.top);
            //Make the spawn point
            CalculateRoomSpawnPoint(spawnSizeX, spawnSizeY, spawnRoom.left, spawnRoom.top);
        }
        spawnRoom.width = spawnSizeX;
        spawnRoom.height = spawnSizeY;
        rooms.Add(spawnRoom);

        //Make a exit room that always appears in one of the corners of the map
        //Should be in a list or library of rooms
        int exitSizeX = sizes[Random.Range(0, 3)];  // Spawn room Size X
        int exitSizeY = sizes[Random.Range(0, 3)];  // Spawn room Size Y
        int es = Random.Range(0, 2);                // Spawn Site

        // Debug.Log("Using exit sizes - size x: " + exitSizeX + " & size y: " + exitSizeY);

        DRoom exitRoom = new DRoom();
        //Chooses between two different spawn sites
        if ((spawnSite == 0) || (spawnSite == 1)) {
            exitRoom.left = (60-(exitSizeX+1));

            if (es == 1) {
                exitRoom.top = 1;
                //Make the exit point
                CalculateExitRoomSpawnPoint(exitSizeX, exitSizeY, exitRoom.left, exitRoom.top);
                // Debug.Log("exitRoom.left: " + exitRoom.left + " exitRoom.top: " + exitRoom.top);
            } else {
                exitRoom.top = (30-(exitSizeY+3));
                //Make the exit point
                CalculateExitRoomSpawnPoint(exitSizeX, exitSizeY, exitRoom.left, exitRoom.top);
                // Debug.Log("exitRoom.left: " + exitRoom.left + " exitRoom.top: " + exitRoom.top);
            }
        } else {
            exitRoom.left = 3;

            if (es == 1) {
                exitRoom.top = 1;
                //Make the exit point
                CalculateExitRoomSpawnPoint(exitSizeX, exitSizeY, exitRoom.left, exitRoom.top);
                // Debug.Log("exitRoom.left: " + exitRoom.left + " exitRoom.top: " + exitRoom.top);
            } else {
                exitRoom.top = (30-(exitSizeY+3));
                //Make the exit point
                CalculateExitRoomSpawnPoint(exitSizeX, exitSizeY, exitRoom.left, exitRoom.top);
                // Debug.Log("exitRoom.left: " + exitRoom.left + " exitRoom.top: " + exitRoom.top);
            }
        }

        exitRoom.width = exitSizeX;
        exitRoom.height = exitSizeY;
        rooms.Add(exitRoom);
    }//MakeSpawnRoom

    void MakeRoom(DRoom r) {
        if (r == rooms[0]) {
            for (int x = 0; x < r.width; x++) {
                for (int y = 0; y < r.height; y++) {
                    if (x == 0 || x == r.width - 1 || y == 0 || y == r.height - 1) {
                        map_data[r.left + x, r.top + y] = 2;
                    }//for
                    else {
                        map_data[r.left + x, r.top + y] = 8;
                    }//else
                }//for
            }//for
        }//if
        else {
            for (int x = 0; x < r.width; x++) {
                for (int y = 0; y < r.height; y++) {
                    if (x == 0 || x == r.width - 1 || y == 0 || y == r.height - 1) {
                        map_data[r.left + x, r.top + y] = 2;
                    }//for
                    else {
                        map_data[r.left + x, r.top + y] = 1;
                    }//else
                }//for
            }//for
        }//else
    }//MakeRoom

    // Makes the corridor.
    void MakeCorridor(DRoom r1, DRoom r2) {
        int x = r1.center_x;
        int y = r1.center_y;

        while (x != r2.center_x) {
            map_data[x, y] = 1;

            x += x < r2.center_x ? 1 : -1;
        }//while

        while (y != r2.center_y) {
            map_data[x, y] = 1;

            y += y < r2.center_y ? 1 : -1;
        }//while

        r1.isConnected = true;
        r2.isConnected = true;

        r1.AddConnection(r2);
        r2.AddConnection(r1);
    }//MakeCorridor
}
