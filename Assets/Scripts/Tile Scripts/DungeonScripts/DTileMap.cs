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

        MakeWalls();
        MakePortals(spx, spy, epx, epy);
        MakePrefabs(size_x, size_y, TTRef);
        GameController.mapIsDone = true;
    }//DTileMap

    // Function to spawn prefabs over the 2d map
    void MakePrefabs(int size_x, int size_y, TileMap TTRef) {
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
        int ssx = 9;                    //Spawn room Size X
        int ssy = 9;                    //Spawn room Size Y
        int ss = Random.Range(0, 4);    //Spawn Site

        DRoom sr = new DRoom();
        //Chooses between four different spawn sites
        if (ss == 0) {
            sr.left = 1;
            sr.top = 1;
            //Make the spawn point
            spx = sr.left + 4;          //4 is the mid point of the room's x size
            spy = sr.top + 4;           //4 is the mid point of the room's y size
        }
        if (ss == 1) {
            sr.left = 1;
            sr.top = 20;
            //Make the spawn point
            spx = sr.left + 4;          //4 is the mid point of the room's x size
            spy = sr.top + 4;           //4 is the mid point of the room's y size
        }
        if (ss == 2) {
            sr.left = 50;
            sr.top = 1;
            //Make the spawn point
            spx = sr.left + 4;          //4is the mid point of the room's x size
            spy = sr.top + 4;           //4is the mid point of the room's y size
        }
        if (ss == 3) {
            sr.left = 50;
            sr.top = 20;
            //Make the spawn point
            spx = sr.left + 4;          //4 is the mid point of the room's x size
            spy = sr.top + 4;           //4 is the mid point of the room's y size
        }
        sr.width = ssx;
        sr.height = ssy;
        rooms.Add(sr);

        //Make a exit room that always appears in one of the corners of the map
        //Should be in a list or library of rooms
        int esx = 7;                    //Spawn Size X
        int esy = 7;                    //Spawn Size Y
        int es = Random.Range(0, 2);    //Spawn Site

        DRoom er = new DRoom();
        //Chooses between two different spawn sites
        if (sr.left == 1) {
            er.left = 52;

            if (es == 1) {
                er.top = 1;
                //Make the exit point
                epx = er.left + 3;          //3 is the mid point of the room's x size
                epy = er.top + 3;           //3 is the mid point of the room's y size
            } else {
                er.top = 22;
                //Make the exit point
                epx = er.left + 3;          //3 is the mid point of the room's x size
                epy = er.top + 3;           //3 is the mid point of the room's y size
            }
        }
        if (sr.left == 50) {
            er.left = 3;

            if (es == 1) {
                er.top = 1;
                //Make the exit point
                epx = er.left + 3;          //3 is the mid point of the room's x size
                epy = er.top + 3;           //3 is the mid point of the room's y size
            } else {
                er.top = 22;
                //Make the exit point
                epx = er.left + 3;          //3 is the mid point of the room's x size
                epy = er.top + 3;           //3 is the mid point of the room's y size
            }
        }

        er.width = esx;
        er.height = esy;
        rooms.Add(er);
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

    void MakePortals(int x, int y, int i, int j) {
        map_data[x, y] = 5;
        map_data[i, j] = 6;
    }//MakePortals

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

    /// <summary>
    /// Makes the corridor.
    /// </summary>
    /// <param name="r1">R1.</param>
    /// <param name="r2">R2.</param>
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

    /// <summary>
    /// Makes the walls.
    /// </summary>
    void MakeWalls() {
        for (int x = 0; x < size_x; x++) {
            for (int y = 0; y < size_y; y++) {
                if (map_data[x, y] == 3 && HasAdjacentFloor(x, y)) {
                    map_data[x, y] = 2;
                }//if
            }//for
        }//for

        for (int x = 0; x < size_x; x++) {
            for (int y = 0; y < size_y; y++) {
                if (map_data[x, y] == 2 && strayWall(x, y) == true) {
                    map_data[x, y] = 1;
                }//if
            }//if
        }//if

        for (int x = 0; x < size_x; x++) {
            for (int y = 0; y < size_y; y++) {
                if (map_data[x, y] == 2 && isPillar(x, y) == true) {
                    map_data[x, y] = 4;
                }//if
            }//if
        }//if

        for (int x = 0; x < size_x; x++) {
            for (int y = 0; y < size_y; y++) {
                if (map_data[x, y] == 1 && isDoor(x, y) == true && HasAdjacentDoor(x, y) == false) {
                    map_data[x, y] = 7;
                }//if
            }//if
        }//if

    }//MakeWalls

    bool HasAdjacentFloor(int x, int y) {
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

    bool HasAdjacentDoor(int x, int y) {
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
    }//HasAdjacentFloor

    //Turn stray wall tiles into floor tiles
    bool strayWall(int x, int y) {
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

    bool isPillar(int x, int y) {
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

    bool isDoor(int x, int y) {
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
    }//isPillar
}
