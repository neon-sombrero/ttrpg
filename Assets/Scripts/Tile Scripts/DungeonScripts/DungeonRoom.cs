using UnityEngine;
using System.Collections.Generic;

public class DungeonRoom {
  public class DRoom {
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
}//DungeonRoom
