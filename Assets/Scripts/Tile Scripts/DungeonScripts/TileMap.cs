using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavRequest
{
	public int TargetX;
	public int TargetY;
	public int SourceX;
	public int SourceY;
	public Unit Requester;
	public System.Action<List<Node>, bool> FinishCallBack;

	public NavRequest(int TarX, int TarY, int SouX, int SouY, Unit Req, System.Action<List<Node>, bool> FinCB)
	{
		TargetX = TarX;
		TargetY = TarY;
		SourceX = SouX;
		SourceY = SouY;
		Requester = Req;
		FinishCallBack = FinCB;
	}
}

public class TileMap : MonoBehaviour 
{	
	public int size_x = 60;
	public int size_z = 30;
	public float tileSize = 5.0f;
	
	public Texture2D terrainTiles;
	public int tileResolution;
	
	public TileType[] tileTypes;

	int[,] tiles;

	Node[,] graph;

	[HideInInspector]
	public List<GameObject> CreateObjects = new List<GameObject>();

	#region Nav Queue
	//Queue of all waiting nav requests
	private List<NavRequest> QueuedNavs = new List<NavRequest>();
	private bool bIsWorking = false;
	#endregion

	// Use this for initialization
	void Start () 
	{
		Rebuild();
		StartCoroutine("HandleQueue");
	}//Start

	public void Rebuild()
	{
		DestroyPreviousObjects();
		BuildTexture();
	}

	void BuildTexture() 
	{
		DTileMap map = new DTileMap(size_x, size_z, this);
		tiles = map.GetMapData();
		GeneratePathfindingGraph();
	}//BuildTexture

	private void DestroyPreviousObjects()
	{
		for(int i = 0; i < CreateObjects.Count; i++)
		{
			if(CreateObjects[i] != null)
			{
				GameObject.DestroyImmediate(CreateObjects[i]);
			}//if
		}//for
		CreateObjects.Clear();
		CreateObjects = new List<GameObject>();
	}//DestroyPreviousObjects

	/*
	 * Taken from the tile movement project,
	 * remove any code below as well as  
	 * Node, Unit and Clickable tile to go back 26/07/2015 14:22
	 */

	void GeneratePathfindingGraph() 
	{
		// Initialize the array
		graph = new Node[size_x,size_z];
		
		// Initialize a Node for each spot in the array
		for(int x=0; x < size_x; x++) 
		{
			for(int z =0;  z< size_z; z++) 
			{
				graph[x,z] = new Node();
				graph[x,z].x = x;
				graph[x,z].z = z;
			}//for
		}//for
		
		// Now that all the nodes exist, calculate their neighbours
		for(int x=0; x < size_x; x++) 
		{
			for(int z=0; z < size_z; z++) 
			{
				//This is the 8 way connection version, allows diagonal
				
				//Try left
				if(x > 0)
				{	
					if(z > 0)
					{
						graph[x,z].neighbours.Add( graph[x-1, z-1] );
					}
					if(z < size_z-1)
					{
						graph[x,z].neighbours.Add( graph[x-1, z+1] );
					}
					graph[x,z].neighbours.Add( graph[x-1, z] );
				}
				
				//Try Right
				if(x < size_x-1)
				{	
					if(z > 0)
					{
						graph[x,z].neighbours.Add( graph[x+1, z-1] );
					}
					if(z < size_z-1)
					{
						graph[x,z].neighbours.Add( graph[x+1, z+1] );
					}
					graph[x,z].neighbours.Add( graph[x+1, z] );
				}
				
				//Try straight up and down
				if(z > 0)
				{
					graph[x,z].neighbours.Add( graph[x, z-1] );
				}
				if(z < size_z-1)
				{
					graph[x,z].neighbours.Add( graph[x, z+1] );
				}
				// This also works with 6-way hexes and 8-way tiles and n-way variable areas (like EU4)
			}//for
		}//for
	}//GeneratePathFindingGraph

	public float CostToEnterTile(int sourceX, int sourceZ, int targetX, int targetZ)
	{
		TileType tt = tileTypes[tiles[targetX,targetZ]];
		
		if(UnitCanEnterTile(targetX, targetZ) == false)
		{
			return Mathf.Infinity;
		}//if
		
		float cost = tt.moveCost;
		
		if(sourceX != targetX && sourceZ != targetZ)
		{
			//We are moving diagonally. Fudge the cost for tie breaking
			//Purely cosmetic
			cost += 0.001f;
		}//if
		
		return cost;
		
	}//CostToEnterTile

	public Vector3 TileCoordToWorldCoord(int x, int z) 
	{
		z = z - size_z;
		return new Vector3((x*tileSize) + (tileSize/2.0f), tileSize*1.5f, (z*tileSize) + (tileSize/2.0f));
	}//TileCoordToWorldCoord
	
	public bool UnitCanEnterTile(int x, int z)
	{
		//We could test the unit's walk over type againts various 
		//terrain flags to see if it can enter a tile.
		return tileTypes[tiles[x,z]].isWalkable;
	}

	#region Nav Queue Logic
	public void RequestNav(int TargetX, int TargetY, int SourceX, int SourceY, Unit Requester, System.Action<List<Node>, bool> FinishCallBack) 
	{
		bool AlreadyExists = false;

		//For each member of the queue
		for(int i = 0; i < QueuedNavs.Count; i++)
		{
			//If the owner matches
			if(QueuedNavs[i].Requester == Requester)
			{
				//Change its stuff
				AlreadyExists = true;
				QueuedNavs[i].TargetX = TargetX;
				QueuedNavs[i].TargetY = TargetY;
				QueuedNavs[i].SourceX = SourceX;
				QueuedNavs[i].SourceY = SourceY;
			}
		}

		//If it doesn't already exist, add it to the end
		if(!AlreadyExists)
		{
			QueuedNavs.Add(new NavRequest(TargetX, TargetY, SourceX, SourceY, Requester, FinishCallBack));
		}
	}

	//THIS HANDLES OUR QUEUED NAV REQUESTS
	private IEnumerator HandleQueue()
	{
		while(true)
		{
			if(QueuedNavs.Count > 0 && !bIsWorking)
			{
				//Take the front waiting part of the queue
				NavRequest CurRequest = QueuedNavs[0];
				QueuedNavs.RemoveAt(0);

				//Passes our request to the system
				StartCoroutine(GeneratePathTo(CurRequest.TargetX, CurRequest.TargetY, CurRequest.SourceX, CurRequest.SourceY, CurRequest.Requester, CurRequest.FinishCallBack));
			}
			yield return null;
		}
	}

	private IEnumerator GeneratePathTo(int TargetX, int TargetY, int SourceX, int SourceY, Unit Requester, System.Action<List<Node>, bool> FinishCallBack) 
	{
		//We are working
		bIsWorking = true;
		TargetY = size_z + TargetY;

		if(Requester != null)
		{
			if(UnitCanEnterTile(TargetX,TargetY) == false)
			{
				//We probably clicked on a mountain or something, so just quit out.
				Debug.Log("FUCK");
				FinishCallBack(null, false);
				bIsWorking = false;
				yield break;
			}
						
			Dictionary<Node, float> dist = new Dictionary<Node, float>();
			Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
			
			// Setup the "Q" -- the list of nodes we haven't checked yet.
			List<Node> unvisited = new List<Node>();

			Node source = graph[SourceX,SourceY];
			Node target = graph[TargetX,TargetY];

			dist[source] = 0;
			prev[source] = null;
			
			// Initialize everything to have INFINITY distance, since
			// we don't know any better right now. Also, it's possible
			// that some nodes CAN'T be reached from the source,
			// which would make INFINITY a reasonable value
			foreach(Node v in graph)
			{
				if(v != source) 
				{
					dist[v] = Mathf.Infinity;
					prev[v] = null;
				}//if
				
				unvisited.Add(v);
			}//foreach
			
			while(unvisited.Count > 0) 
			{
				// "u" is going to be the unvisited node with the smallest distance.
				Node u = null;
				
				foreach(Node possibleU in unvisited) 
				{
					if(u == null || dist[possibleU] < dist[u]) 
					{
						u = possibleU;
					}//if
				}//foreach
				
				if(u == target) 
				{
					break;	// Exit the while loop!
				}//if
				
				unvisited.Remove(u);
				
				foreach(Node v in u.neighbours) 
				{
					//float alt = dist[u] + u.DistanceTo(v);
					float alt = dist[u] + CostToEnterTile(u.x, u.z, v.x, v.z);
					if( alt < dist[v] ) 
					{
						dist[v] = alt;
						prev[v] = u;
					}//if
				}//foreach

				//Give it a chance to breathe
				if(unvisited.Count % 60 == 0)
				{
					yield return new WaitForEndOfFrame();
				}
			}//while
			
			// If we get there, the either we found the shortest route
			// to our target, or there is no route at ALL to our target.
			
			if(prev[target] == null) 
			{
				// No route between our target and the source
				FinishCallBack(null, false);
				bIsWorking = false;
				yield break;
			}//if
			
			List<Node> currentPath = new List<Node>();
			
			Node curr = target;
			
			// Step through the "prev" chain and add it to our path
			while(curr != null) 
			{
				currentPath.Add(curr);
				curr = prev[curr];
			}//while
			
			// Right now, currentPath describes a route from out target to our source
			// So we need to invert it!
			
			currentPath.Reverse();

			//Tell it we've finished
			FinishCallBack(currentPath, true);
		}

		yield return new WaitForEndOfFrame();
		bIsWorking = false;
	}//GeneratePathTo
	#endregion
}//TileMap
