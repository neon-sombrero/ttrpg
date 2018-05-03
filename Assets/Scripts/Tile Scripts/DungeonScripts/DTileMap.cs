using UnityEngine;
using System.Collections.Generic;

public class DTileMap {
    protected class DRoom {
        public int left;
        public int top;
        public int width;
        public int height;

        public bool isConnected = false;
        public List<DRoom> Connections = new List<DRoom>();

        public int right {
            get { return left + width - 1; }
        }

        public int bottom {
            get { return top + height - 1; }
        }

        public int center_x {
            get { return left + width / 2; }
        }

        public int center_y {
            get { return top + height / 2; }
        }

        public bool CollidesWith(DRoom other) {
            if (left > other.right - 1)
                return false;

            if (top > other.bottom - 1)
                return false;

            if (right < other.left + 1)
                return false;

            if (bottom < other.top + 1)
                return false;

            return true;
        }//CollidesWidth

        public void AddConnection(DRoom OtherRoom) {
            if (!Connections.Contains(OtherRoom) && OtherRoom != this) {
                Connections.Add(OtherRoom);
            }
        }//AddConnection

        public static bool CheckConnections(DRoom StartRoom, DRoom TargetRoom) {
            //If we are looking at the same room then we dont have an issue
            if (StartRoom == TargetRoom) {
                //So return that we have a connection
                return true;
            }

            //A list of all rooms currently searched
            List<DRoom> SearchedRooms = new List<DRoom>();

            //Add the room we started in
            SearchedRooms.Add(StartRoom);

            //For each connection
            foreach (DRoom Connection in StartRoom.Connections) {
                //Search through the rooms
                if (SearchRooms(ref SearchedRooms, Connection, TargetRoom)) {
                    //And if it was successful, then we have found our room
                    return true;
                }
            }

            //Otherwise no route was found
            return false;
        }//Check Connections

        private static bool SearchRooms(ref List<DRoom> SearchedRooms, DRoom CurrentRoom, DRoom TargetRoom) {
            //Add the room we are currently looking at
            if (!SearchedRooms.Contains(CurrentRoom)) {
                SearchedRooms.Add(CurrentRoom);
                Debug.Log("Room Added");
            }

            //For each further connection
            foreach (DRoom Connection in CurrentRoom.Connections) {
                //If this is the target room
                if (Connection == TargetRoom) {
                    //Then fall back through the loop
                    return true;
                } else {
                    //If this connection hasn't been searched yet
                    if (!SearchedRooms.Contains(Connection)) {
                        Debug.Log("Searching Connection");
                        //Continue down the chain
                        return SearchRooms(ref SearchedRooms, Connection, TargetRoom);
                    }
                }
            }
            return false;
        }//SearchRooms

    }//DRoom

    Utils utilsInstance = new Utils();

    int size_x;
    int size_y;

    int[,] map_data;

    public int spx, spy, epx, epy;

    List<DRoom> rooms = new List<DRoom>();

    public int[,] GetMapData() {
      return map_data;
    }

    public DTileMap(int size_x, int size_y, TileMap TTRef) {
        GameController.mapIsDone = false;
        this.size_x = size_x;
        this.size_y = size_y;

        map_data = new int[size_x, size_y];

        for (int x = 0; x < size_x; x++) {
            for (int y = 0; y < size_y; y++) {
                map_data[x, y] = 3;
            }//for
        }//for

        rooms = new List<DRoom>();
        MakeSpawnRoom();

        int maxFails = 50;

        while (rooms.Count < 10) {
            int rsx = Random.Range(8, 14);
            int rsy = Random.Range(8, 10);

            DRoom r = new DRoom();
            r.left = Random.Range(0, size_x - rsx);
            r.top = Random.Range(0, size_y - rsy);
            r.width = rsx;
            r.height = rsy;

            if (!RoomCollides(r)) {
                rooms.Add(r);
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
        utilsInstance.MakePortals(map_data, spx, spy, epx, epy);
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

    void CalculateSpawnRoomPosition(DRoom spawnRoom, int sizeX, int sizeY){
      // Calculate where the room should go based on how big it is
      // The map is 30x60 so we cannot exceed 29 on the x, or 59 on the Y
      // Or else we will mess up the nice walls around the outside

    }

    void CalculateRoomSpawnPoint(int sizeX, int sizeY, int left, int top) {
      // Calculate the mid point of each size and round down
      int midX = Mathf.FloorToInt(sizeX/2);
      int midY = Mathf.FloorToInt(sizeY/2);

      // Add the mid point to left and top
      spx = left + midX;
      spy = top + midY;
      Debug.Log("Building a spawn point - x: " + spx + " & y: " + spy);
    }

    /// <summary>
    /// Chooses a corner of the map randomly,
    /// then spawns a room there. Based on the
    /// position of the Spawn room, an exit room
    /// is made on the opposite side in a different
    /// corner.
    /// </summary>
    void MakeSpawnRoom() {
        //Make a spawn room that always appears in one of the corners of the map
        //Should be in a list or library of rooms

        // Could go further by having the spawn room sizes randomise,
        // then half it and round up for the mid points

        // Thinking of having odd nums in an array [5,7,9,11]
        // randomise the index of each to make sure the spawn room is always
        // an odd num (and so has a mid point)

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
          spawnSizeY = sizes[Random.Range(0, 3)];  // Spawn room Size Y
        } else {
          spawnSizeY = sizes[Random.Range(0, 4)];  // Spawn room Size Y
        }
        int spawnSite = Random.Range(0, 4);    //Spawn Site

        Debug.Log("Using spawn sizes - size x: " + spawnSizeX + " & size y: " + spawnSizeY);

        DRoom spawnRoom = new DRoom();
        //Chooses between four different spawn sites
        if (spawnSite == 0) {
            spawnRoom.left = 1;
            spawnRoom.top = 1;
            Debug.Log("spawnRoom.left: " + spawnRoom.left + " spawnRoom.top: " + spawnRoom.top);
            //Make the spawn point
            CalculateRoomSpawnPoint(spawnSizeX, spawnSizeY, spawnRoom.left, spawnRoom.top);
        }
        if (spawnSite == 1) {
            spawnRoom.left = 1;
            spawnRoom.top = (30-(spawnSizeY+1));
            Debug.Log("spawnRoom.left: " + spawnRoom.left + " spawnRoom.top: " + spawnRoom.top);
            //Make the spawn point
            CalculateRoomSpawnPoint(spawnSizeX, spawnSizeY, spawnRoom.left, spawnRoom.top);
        }
        if (spawnSite == 2) {
            spawnRoom.left = (60-(spawnSizeX+1));
            spawnRoom.top = 1;
            Debug.Log("spawnRoom.left: " + spawnRoom.left + " spawnRoom.top: " + spawnRoom.top);
            //Make the spawn point
            CalculateRoomSpawnPoint(spawnSizeX, spawnSizeY, spawnRoom.left, spawnRoom.top);
        }
        if (spawnSite == 3) {
            spawnRoom.left = (60-(spawnSizeX+1));
            spawnRoom.top = (30-(spawnSizeY+1));
            Debug.Log("spawnRoom.left: " + spawnRoom.left + " spawnRoom.top: " + spawnRoom.top);
            //Make the spawn point
            CalculateRoomSpawnPoint(spawnSizeX, spawnSizeY, spawnRoom.left, spawnRoom.top);
        }
        spawnRoom.width = spawnSizeX;
        spawnRoom.height = spawnSizeY;
        rooms.Add(spawnRoom);

        //Make a exit room that always appears in one of the corners of the map
        //Should be in a list or library of rooms
        int exitSizeX = 7;                    //Spawn Size X
        int exitSizeY = 7;                    //Spawn Size Y
        int es = Random.Range(0, 2);    //Spawn Site

        DRoom exitRoom = new DRoom();
        //Chooses between two different spawn sites
        if ((spawnSite == 0) || (spawnSite == 1)) {
            exitRoom.left = 52;

            if (es == 1) {
                exitRoom.top = 1;
                //Make the exit point
                epx = exitRoom.left + 3;          //3 is the mid point of the room's x size
                epy = exitRoom.top + 3;           //3 is the mid point of the room's y size
            } else {
                exitRoom.top = 22;
                //Make the exit point
                epx = exitRoom.left + 3;          //3 is the mid point of the room's x size
                epy = exitRoom.top + 3;           //3 is the mid point of the room's y size
            }
        } else {
            exitRoom.left = 3;

            if (es == 1) {
                exitRoom.top = 1;
                //Make the exit point
                epx = exitRoom.left + 3;          //3 is the mid point of the room's x size
                epy = exitRoom.top + 3;           //3 is the mid point of the room's y size
            } else {
                exitRoom.top = 22;
                //Make the exit point
                epx = exitRoom.left + 3;          //3 is the mid point of the room's x size
                epy = exitRoom.top + 3;           //3 is the mid point of the room's y size
            }
        }

        exitRoom.width = exitSizeX;
        exitRoom.height = exitSizeY;
        rooms.Add(exitRoom);
    }//MakeSpawnRoom

    public bool InSpawnRoom(Vector3 WorldPos, Vector3 CentrePos) {
        for (int i = -4; i < 4; i++) {
            for (int j = -4; j < 4; j++) {
                CentrePos = CentrePos + new Vector3(i, 0, j);

                if (WorldPos.x == CentrePos.x && WorldPos.y == CentrePos.y) {
                    return true;
                }//if
            }//for
        }//for

        return false;
    }//InSpawnRoom

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
