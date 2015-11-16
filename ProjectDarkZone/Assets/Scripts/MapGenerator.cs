using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour {
	
	int[,] map;
	
	GameObject player;
	
	
	public bool regenerateMapOnLaunch = true; //This regenerates the data about the cave so that it can be used while the game is playing
	
	// Base Map Parameters
	public int width;
	public int height;
	public string seed;
	public bool useRandomSeed; 
	
	[Range(0,100)]
	public int randomFillPercent; //Deterimines amount of wall versus blank space
	
	public int wallThresholdSize = 50; //Smallest number of tiles for a wall
	public int roomThresholdSize = 50; //Smallest number of tiles for a room
	
	public float tileSize = 1.0f; //Conversion from Coord units to Unity Scene units.
	
	
	// Graph (used for analyzing map)
	public Graph graph;
	
	
	// Tiles types (based on their surrounding tiles)
	public List<Coord> ceilingTiles;
	public List<Coord> floorTiles;
	public List<Coord> rightWallTiles;
	public List<Coord> leftWallTiles;
	public List<Coord> filledWallTiles;
	public List<Coord> openTiles; // 3x3 of empty tiles
	public List<Coord> emptyTiles; // 1x1 of empty tiles
	
	//Scenery - Prefabs to place at the different tile types
	public GameObject ceilingScenery;
	public GameObject floorScenery;
	public GameObject rightWallScenery;
	public GameObject leftWallScenery;
	
	//Other Prefabs
	public GameObject exitPrefab;
	public GameObject[] gemPrefabs;
	public GameObject[] itemPrefabs;
	public GameObject playerPrefab;
	public GameObject spiderWed;
	public GameObject bat;
	
	//Spawn Rates
	public float spiderWebSpawnRate = 5.0f;	
	public float batSpawnRate = 1.0f;
	
	//Important Positions in Map
	public Coord playerSpawn;
	
	//Children of Map Generator
	public GameObject background;
	//Use to organize other game objects:
	public GameObject scenery;
	public GameObject entities;
	public GameObject bats;
	public GameObject gems;
	public GameObject items;
	public GameObject torches;
	public GameObject rocks;
	
	void Start()
	{	
		if (this.regenerateMapOnLaunch) {
			GenerateMap();
			GetTileTypes();
		}
		
	}
	
	public void CreateNecessaryGameObjects() {
		
		if (scenery == null) {
			this.scenery = FindOrCreateGameObject("Scenery",this.gameObject);
		}
		if (entities == null) {
			this.entities = FindOrCreateGameObject("Entities",this.gameObject);
		}
		if (bats == null) {
			this.bats = FindOrCreateGameObject("Bats",this.entities);
		}
		if (gems == null) {
			this.gems = FindOrCreateGameObject("Gems",this.gameObject);
		}
		if (items == null) {
			this.items = FindOrCreateGameObject("Items",this.gameObject);
		}
		if (rocks == null) {
			this.rocks = FindOrCreateGameObject("Rocks",this.gameObject);
		}
		if (torches == null) {
			this.torches = FindOrCreateGameObject("Torches",this.gameObject);
		}
		
		FindOrCreatePlayer();
	}
	
	GameObject FindOrCreateGameObject(string name, GameObject parent) {
		GameObject gameObject = GameObject.Find (name);
		if (gameObject == null) {
			gameObject = new GameObject(name);
			gameObject.transform.parent = parent.transform;
		}
		return gameObject;
	}
	
	void FindOrCreatePlayer() {
		GameObject _player = GameObject.Find ("Player");
		if (_player == null) {
			this.player = Instantiate(this.playerPrefab, GetTilePositionInScene(new Coord(0,0)), Quaternion.identity) as GameObject;
			this.player.name = "Player";
		}
		else {
			this.player = _player;
		}
	}
	
	public void SetPlayerSpawn() {
		Coord spawnPoint = GetTileClosestToTile(new Coord(0,height-1), floorTiles);
		spawnPoint.tileY ++;
		this.playerSpawn = spawnPoint;
		this.AddObjectAt(this.playerSpawn,this.exitPrefab,this.gameObject);
		GameObject exit = GameObject.Find ("Exit(Clone)");
		exit.name = "Exit";
		exit.transform.position = new Vector3(exit.transform.position.x,exit.transform.position.y,0.0f);
	}
	
	void ResizeBackground() {
		
		//TODO: Fix the offset when the width and height are different.
		
		Vector3 backgroundScale = new Vector3(this.width/10.0f,1.0f,this.height/10.0f);
		background.transform.localScale = backgroundScale;
		
		if (width > height) {
			float yOffset = -width/4;
			background.transform.position = new Vector3(0f,yOffset,0.1f);
		}
		else if (height > width) {
			float xOffset = height/4;
			background.transform.position = new Vector3(0f, xOffset,0.1f);
		}
		else {
			background.transform.position = new Vector3(0.0f,0.0f,0.1f);
		}
	}
	
	public void GenerateMap()
	{
		ResizeBackground();
		
		map = new int[width, height];
		RandomFillMap();
		
		for(int i = 0; i < 5; i++) //Could consider modifying algorithm slightly as the i count goes up
		{
			SmoothMap(); //Iteration count can be modified to produce different looking caves.
		}
		
		ProcessMap();
		
		int borderSize = 1;
		int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2]; //Creates border around map (MIGHT NEED TO CHANGE TO CREATE MANY MAPS)
		
		for (int x = 0; x < borderedMap.GetLength(0); x++) //Creates a boarder, might need MODIFICATION
		{
			for (int y = 0; y < borderedMap.GetLength(1); y++)
			{
				if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
				{
					borderedMap[x, y] = map[x - borderSize, y - borderSize];
				}
				else
				{
					borderedMap[x, y] = 1;
				}
			}
		}
		
		
		MeshGenerator meshGen = GetComponent<MeshGenerator>();
		meshGen.GenerateMesh(borderedMap, tileSize); //(Originally used "map" instead of bordered map)
	}
	
	void ProcessMap()
	{
		List<List<Coord>> wallRegions = GetRegions(1);
		
		foreach(List<Coord> wallRegion in wallRegions)
		{
			if (wallRegion.Count < wallThresholdSize) //Remove all regions of walls with less than wallThreshold coordinate tiles
			{
				foreach (Coord tile in wallRegion)
				{
					map[tile.tileX, tile.tileY] = 0;
				}
			}
		}
		
		List<List<Coord>> roomRegions = GetRegions(0);
		//int roomThresholdSize = 50; //MODIFY ME CAPTAIN
		List<Room> survivingRooms = new List<Room>();
		
		foreach (List<Coord> roomRegion in roomRegions)
		{
			if (roomRegion.Count < roomThresholdSize) //Remove all regions of walls with less than roomThreshold coordinate tiles
			{
				foreach (Coord tile in roomRegion)
				{
					map[tile.tileX, tile.tileY] = 1;
				}
			}
			else
			{
				survivingRooms.Add(new Room(roomRegion, map)); //Create list of rooms that are actually in the map
			}
		}
		
		survivingRooms.Sort();
		survivingRooms[0].isMainRoom = survivingRooms[0].isAccessibleFromMainRoom = true;
		
		ConnectClosestRooms(survivingRooms);
	}
	
	void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
	{
		List<Room> roomListA = new List<Room>(); //We want to connect all rooms in list A to some room in list B
		List<Room> roomListB = new List<Room>();
		
		if (forceAccessibilityFromMainRoom)
		{
			foreach(Room room in allRooms)
			{
				if (room.isAccessibleFromMainRoom)
				{
					roomListB.Add(room);
				}
				else
				{
					roomListA.Add(room);
				}
			}
		}
		else
		{
			roomListA = allRooms;
			roomListB = allRooms;
		}
		
		int bestDistance = 0;
		Coord bestTileA = new Coord();
		Coord bestTileB = new Coord();
		Room bestRoomA = new Room();
		Room bestRoomB = new Room();
		bool possibleConnectionFound = false;
		
		foreach (Room roomA in roomListA)
		{
			if (!forceAccessibilityFromMainRoom)
			{
				possibleConnectionFound = false; //We only want to consider ALL connections when no rooms are connected. Once they are connected, we need to consider shortest path between larger rooms
				if (roomA.connectedRooms.Count > 0)
				{
					continue;
				}
			}
			foreach(Room roomB in roomListB)
			{
				if (roomA == roomB || roomA.IsConnected(roomB))
				{
					continue;
				}
				
				for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
				{
					for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
					{
						Coord tileA = roomA.edgeTiles[tileIndexA];
						Coord tileB = roomB.edgeTiles[tileIndexB];
						int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2)); //Check distance between edges. Note, don't use sqrt
						
						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
						{
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}
			
			if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
			{
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}
		
		if (possibleConnectionFound && forceAccessibilityFromMainRoom)
		{
			CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms(allRooms, true);
		}
		
		if (!forceAccessibilityFromMainRoom)
		{
			ConnectClosestRooms(allRooms, true);
		}
	}
	
	void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
	{
		Room.ConnectRooms(roomA, roomB);
		Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100); //Visualize connection
		
		List<Coord> line = GetLine(tileA, tileB);
		foreach (Coord c in line)
		{
			DrawCircle(c, 1); //MODIFY ME, CAPTIAN! Changes passage connection width ********************************
		}
	}
	
	void DrawCircle(Coord c, int r)
	{
		for(int x = -r; x <= r; x++)
		{
			for (int y = -r; y <= r; y++)
			{
				if (x*x + y*y <= r * r)
				{
					int drawX = c.tileX + x;
					int drawY = c.tileY + y;
					
					if (IsInMapRange(drawX, drawY))
					{
						map[drawX, drawY] = 0;
					}
				}
			}
		}
	}
	
	List<Coord> GetLine(Coord from, Coord to) //See video for math...there's a lot of it.
	{
		List<Coord> line = new List<Coord>();
		
		int x = from.tileX;
		int y = from.tileY;
		
		int dx = to.tileX - from.tileX;
		int dy = to.tileY - from.tileY;
		
		bool inverted = false;
		int step = Math.Sign(dx);
		int gradientStep = Math.Sign(dy);
		
		int longest = Mathf.Abs(dx);
		int shortest = Mathf.Abs(dy);
		
		if (longest < shortest)
		{
			inverted = true;
			longest = Mathf.Abs(dy);
			shortest = Mathf.Abs(dx);
			
			step = Math.Sign(dy);
			gradientStep = Math.Sign(dx);
		}
		
		int gradientAccumulation = longest / 2;
		for (int i = 0; i < longest; i++)
		{
			line.Add(new Coord(x, y));
			
			if (inverted)
			{
				y += step;
			}
			else
			{
				x += step;
			}
			
			gradientAccumulation += shortest;
			
			if (gradientAccumulation >= longest)
			{
				if (inverted)
				{
					x += gradientStep;
				}
				else
				{
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}
		
		return line;
	}
	
	Vector3 CoordToWorldPoint(Coord tile)
	{
		return new Vector3((-width / 2) + .5f + tile.tileX, 2, (-height / 2) + .5f + tile.tileY - 28); //YOU ARE MY PROBLEM YOU A$$HOLE
	}
	
	List<List<Coord>> GetRegions(int tileType)
	{
		List<List<Coord>> regions = new List<List<Coord>>();
		int[,] mapFlags = new int[width, height]; //If checked tile or not.
		
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (mapFlags[x,y] == 0 && map[x,y] == tileType)
				{
					List<Coord> newRegion = GetRegionTiles(x, y); //Get a new region of tiles
					regions.Add(newRegion);
					foreach (Coord tile in newRegion)
					{
						mapFlags[tile.tileX, tile.tileY] = 1; //Mark each tile as "looked at"
					}
				}
			}
		}
		return regions;
	}
	
	
	List<Coord> GetRegionTiles(int startX, int startY) //Get the tiles that make up a certain region/open space on the map.
	{
		List<Coord> tiles = new List<Coord>();
		int[,] mapFlags = new int[width, height];
		int tileType = map[startX, startY];
		
		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(new Coord(startX, startY));
		mapFlags[startX, startY] = 1;
		
		while (queue.Count > 0) //Check all tiles next to, above, and below for wall vs room
		{
			Coord tile = queue.Dequeue();
			tiles.Add(tile);
			
			for (int x = tile.tileX -1; x <= tile.tileX + 1; x++) // Check side to side
			{
				for (int y = tile.tileY -1; y <= tile.tileY + 1; y++) //Check up and down
				{
					if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
					{
						if (mapFlags[x, y] == 0 && map[x, y] == tileType)
						{
							mapFlags[x, y] = 1;
							queue.Enqueue(new Coord(x, y));
						}
					}
				}
			}
			
		}
		return tiles;
	}
	
	bool IsInMapRange(int x, int y)
	{
		return x >= 0 && x < width && y >= 0 && y < height;
	}
	
	//    void Update() //Uncomment for new map upon click
	//    {
	//        if (Input.GetMouseButtonDown(0))
	//        {
	//            GenerateMap();
	//        }
	//    }
	
	void RandomFillMap()
	{
		if (useRandomSeed)
			seed = System.DateTime.Now.Ticks.ToString();
		
		System.Random pseudoRandom = new System.Random(seed.GetHashCode());
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (x == 0 || x ==width -1 || y == 0 || y == height - 1)
				{
					map[x, y] = 1; //We want all of the edges of the maps to be walls, so, this makes that happen.
				}
				else
				{
					map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0; // If the pseudo-random number is less than fill %, add a wall. Greater, then leave as blank tile.
				}
				
			}
		}
	}
	
	void SmoothMap() //Iterative smoothing for the created map
	{
		
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				int neighborWallTiles = GetSurroundingWallCount(x, y);
				
				if (neighborWallTiles > 4)
				{
					map[x, y] = 1;
				}
				else if (neighborWallTiles < 4) //Either 4 can be varied or made <= to create different caves, ones that are less organic
				{
					map[x, y] = 0;
				}
			}
		}
		
	}
	
	int GetSurroundingWallCount(int gridX, int gridY) //Searching grid can be changed for larger and smaller grids for fine tuning
	{
		int wallCount = 0;
		
		for(int neighborX = gridX - 1; neighborX <= gridX+1; neighborX++)
		{
			for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++) //Iterate through a 3x3 grid of neighbors
			{
				if (IsInMapRange(neighborX, neighborY)) //Ensure that the neighbors are in the maps, so as to catch errors of edge cases
				{
					if (neighborX != gridX || neighborY != gridY)
					{
						wallCount += map[neighborX, neighborY]; //Walls are 1, so this will count walls
					}
				}
				else
				{
					wallCount++; //Encourage growth of walls around the outside walls.
				}
			}
		}
		
		return wallCount;
	}
	
	class Room : IComparable<Room>
	{
		public List<Coord> tiles;
		public List<Coord> edgeTiles;
		public List<Room> connectedRooms;
		public int roomSize; //Master room defined by room with the biggest size. That's why we must have a compareTo, for the ProcessMap() method.
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;
		
		public Room()
		{
			
		}
		
		public Room(List<Coord> roomTiles, int[,] map)
		{
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();
			
			edgeTiles = new List<Coord>();
			foreach (Coord tile in tiles) //If any of the tiles has a boarder tile that's a wall, it's an edge tile
			{
				for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
				{
					for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
					{
						if (x == tile.tileX || y == tile.tileY) //Don't consider diagonals
						{
							//Debug.Log ("Tile: X = " + x.ToString() + "   Y = " + y.ToString());
							if (map[x,y] == 1)
							{
								edgeTiles.Add(tile);
							}
						}
					}
				}
			}
		}
		
		public void SetAccessibleFromMainRoom()
		{
			if (!isAccessibleFromMainRoom)
			{
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms)
				{
					connectedRoom.SetAccessibleFromMainRoom();
				}
			}
		}
		
		public static void ConnectRooms(Room roomA, Room roomB)
		{
			if (roomA.isAccessibleFromMainRoom)
			{
				roomB.SetAccessibleFromMainRoom();
			}
			else if (roomB.isAccessibleFromMainRoom)
			{
				roomA.SetAccessibleFromMainRoom();
			}
			roomA.connectedRooms.Add(roomB);
			roomB.connectedRooms.Add(roomA);
		}
		
		public bool IsConnected(Room otherRoom)
		{
			return connectedRooms.Contains(otherRoom);
		}
		
		public int CompareTo(Room otherRoom)
		{
			return otherRoom.roomSize.CompareTo(roomSize);
		}
	}
	
	//	void OnDrawGizmos()
	//	{
	//        
	//        if (map != null)
	//        {
	//            for (int x = 0; x < width; x++)
	//            {
	//                for (int y = 0; y < height; y++)
	//                {
	//                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white; //If it is a wall, color it black, or a space, color it white. Visual debug code
	//					Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y + .5f, 0.0f);
	//					Gizmos.DrawCube(pos, Vector3.one);
	//                }
	//            }
	//        }
	//
	//    }
	
	public void Clear() {
		DestroyChildren(scenery);
		DestroyChildren(bats);
		DestroyChildren(items);
		DestroyChildren(gems);

		GameObject exit = GameObject.Find ("Exit");
		if (exit != null) {
			DestroyImmediate(exit);
		}
	}
	
	public void DestroyChildren(GameObject parent) {
		//Destroys the children game objects of the given game object
		
		while (parent.transform.childCount > 0) {
			GameObject child = parent.transform.GetChild(0).gameObject;
			if (Application.isPlaying) {
				Destroy(child,0.1f);
			}
			else {
				DestroyImmediate(child);
			}
		}
	}
	
	public void SpawnPlayer() {
		SetPlayerSpawn();
		//Adjust the tile coordinates to Unity coordinates
		float x = playerSpawn.tileX * tileSize - (width/2)*tileSize + tileSize/2;
		float y = playerSpawn.tileY*tileSize- (width/2)*tileSize + tileSize/2+ tileSize/2;
		player.transform.position = new Vector2(x,y);
	}
	
	public void AddBats() {
		//Add bats randomly in tiles of type "opentile" based on batSpawnRate
		GetTileTypes();
		int batCount = 0;
		foreach (Coord tile in openTiles) {
			if (UnityEngine.Random.Range(0.0f,100.0f) <= batSpawnRate) {
				AddObjectAt(tile,bat,this.bats);
				batCount ++;
			}
		}
		Debug.Log ("Bats Added: " + batCount.ToString());
	}
	
	public void AddScenery() {
		
		DestroyChildren(this.scenery); //Clear previous scenery
		GetTileTypes();
		
		List<Coord> allFlatTiles = new List<Coord>();
		allFlatTiles.AddRange(floorTiles);
		allFlatTiles.AddRange(ceilingTiles);
		allFlatTiles.AddRange(rightWallTiles);
		allFlatTiles.AddRange(leftWallTiles);
		
		int markerCount = 0;
		
		foreach (Coord tile in floorTiles) {
			markerCount ++;
			AddObjectAt(tile,floorScenery,scenery);
		}
		
		foreach (Coord tile in ceilingTiles) {
			markerCount ++;
			AddObjectAt(tile,ceilingScenery,scenery);
		}
		
		foreach (Coord tile in rightWallTiles) {
			markerCount ++;
			AddObjectAt(tile,rightWallScenery,scenery);
		}
		
		foreach (Coord tile in leftWallTiles) {
			markerCount ++;
			AddObjectAt(tile,leftWallScenery,scenery);
		}
		
		Debug.Log ("Markers Added: " + markerCount);
		
	}
	
	public Vector2 GetTilePositionInScene(Coord tile) {
		Vector2 relativePos = new Vector2(tile.tileX * tileSize - (width/2)*tileSize + tileSize/2, tile.tileY*tileSize- (width/2)*tileSize + tileSize/2);
		return relativePos;
	}
	
	public bool IsInWall(Vector2 position) {
		Coord tile = GetTileFromScenePosition(position);
		if (this.map == null) {
			Debug.Log ("Map is Null");
		}
		if (this.map != null && IsInMapRange(tile.tileX,tile.tileY) && this.map[tile.tileX,tile.tileY] == 1) {
			return true;
		}
		else if (IsInMapRange(tile.tileX,tile.tileY) == false) {
			return true;
		}
		else {
			return false;
		}
	}
	
	public Vector2 GetEscapeRoute(Vector2 position) {
		Coord tile = GetTileFromScenePosition(position);
		Coord escapeTile = GetClosestEmptyTile(tile);
		return GetTilePositionInScene(escapeTile);
	}
	
	public Coord GetTileFromScenePosition(Vector2 position) {
		int x = Mathf.RoundToInt((position.x - tileSize/2 + (width/2)*tileSize)/tileSize);
		int y = Mathf.RoundToInt((position.y + (width/2)*tileSize - tileSize/2)/tileSize);
		Coord newTile = new Coord(x,y);
		//Debug.Log ("New Tile: " + newTile.ToString());
		return newTile;
	}
	
	
	
	public Coord GetClosestEmptyTile(Coord tile) {
		
		Coord closestTile = this.emptyTiles[0];
		float minDistance = Distance(closestTile, tile);
		foreach (Coord emptyTile in emptyTiles) {
			float distance = Distance (tile,emptyTile);
			if (distance < minDistance) {
				minDistance = distance;
				closestTile = emptyTile;
			}
		}
		
		return closestTile;
	}

	public Coord GetClosestWallTile(Coord tile) {
		
		Coord closestTile = this.filledWallTiles[0];
		float minDistance = Distance(closestTile, tile);
		foreach (Coord filledWallTile in filledWallTiles) {
			float distance = Distance (tile,filledWallTile);
			if (distance < minDistance) {
				minDistance = distance;
				closestTile = filledWallTile;
			}
		}
		return closestTile;
	}
	
	public Vector2 GetPositionInSceneFloat(float x, float y) {
		Vector2 relativePos = new Vector2(x * tileSize - (width/2)*tileSize + tileSize/2, y*tileSize- (width/2)*tileSize + tileSize/2);
		return relativePos;
	}
	
	public void AddObjectAt(Coord tile, GameObject obj, GameObject parent) {
		Vector2 relativePosition = GetTilePositionInScene(tile);
		float x = relativePosition.x;
		float y = relativePosition.y;
		GameObject tileMarker = Instantiate(obj, new Vector3(x,y,0.0f), Quaternion.identity) as GameObject;
		
		tileMarker.transform.parent = parent.transform; //Set as child parent object
		
		tileMarker.transform.position = new Vector3(tileMarker.transform.position.x,tileMarker.transform.position.y,-1.0f);
	}
	
	public void DrawSquareAt(Coord tile, float radius, Color color) {
		List<float[]> squareVertices = new List<float[]>();
		
		for (float x2 = radius; x2 >= -radius; x2 -= radius*2) {
			for (float y2 = radius; y2 >= -radius; y2 -= radius*2) {
				float[] floatArray = new float[2];
				floatArray[0] = tile.tileX + x2;
				floatArray[1] = tile.tileY + y2;
				squareVertices.Add (floatArray);
			}
		}
		
		//Swap
		float[] temp = squareVertices[2];
		squareVertices[2] = squareVertices[3];
		squareVertices[3] = temp;
		
		for (int i = 0; i < squareVertices.Count; i++) {
			if (i < squareVertices.Count-1) {
				Debug.DrawLine(GetPositionInSceneFloat(squareVertices[i][0],squareVertices[i][1]),GetPositionInSceneFloat(squareVertices[i+1][0],squareVertices[i+1][1]), color);
			}
			else {
				Debug.DrawLine(GetPositionInSceneFloat(squareVertices[i][0],squareVertices[i][1]),GetPositionInSceneFloat(squareVertices[0][0],squareVertices[0][1]), color);
			}
		}
	}
	
	public void ShowTileTypes () {
		
		GetTileTypes();
		float radius = 0.5f;
		
		foreach (Coord tile in floorTiles) {
			DrawSquareAt(tile,radius,Color.cyan);
		}
		
		foreach (Coord tile in ceilingTiles) {
			DrawSquareAt(tile,radius,Color.magenta);
		}
		
		foreach (Coord tile in rightWallTiles) {
			DrawSquareAt(tile,radius,Color.yellow);
		}
		
		foreach (Coord tile in leftWallTiles) {
			DrawSquareAt(tile,radius,Color.green);
		}
		
		foreach (Coord tile in openTiles) {
			DrawSquareAt(tile,radius,Color.blue);
		}
	}
	
	public void GetTileTypes() {
		
		floorTiles = new List<Coord>();
		ceilingTiles = new List<Coord>();
		rightWallTiles = new List<Coord>();
		leftWallTiles = new List<Coord>();
		filledWallTiles = new List<Coord>();
		openTiles = new List<Coord>();
		emptyTiles = new List<Coord>();
		
		for (int x = 1; x < width-1; x++)
		{
			for (int y = 1; y < height-1; y++)
			{
				Coord tile = new Coord(x,y);
				
				int configuration = GetTileConfiguration(tile);
				
				switch(configuration) {
				case 1008:
					floorTiles.Add(tile);
					break;
				case 126:
					ceilingTiles.Add(tile);
					break;
				case 876:
					rightWallTiles.Add(tile);
					break;
				case 438:
					leftWallTiles.Add(tile);
					break;
				case 0:
					openTiles.Add (tile);
					break;
				default:
					break;
				}

				if((configuration & 32) > 0) //all other tiles with the middle bit
					filledWallTiles.Add (tile);

				if (this.map[x,y] == 0) {
					this.emptyTiles.Add(tile);
				}
			}
		}
	}
	
	int GetTileConfiguration(Coord tile) {
		//Returns a single int corresponding to which tiles are walls (1) and empty (0)
		
		int bitCount = 1;
		int tileNumber = 0;
		
		for (int dy = 1; dy >= -1; dy--) {
			for (int dx = -1; dx <= 1; dx++) {
				int x = tile.tileX + dx;
				int y = tile.tileY + dy;
				if (IsInMapRange(x,y)) {
					if (map[x,y] == 1) { 
						tileNumber += (int)Mathf.Pow(2,bitCount);
					}
				}
				bitCount ++;
			}
		}
		
		return tileNumber;
	}
	
	
	Coord GetHeighestHTileInList(List<Coord> tiles) {
		//Returns the tile with the greatest Y component.
		Coord maxTile = tiles[0];
		
		foreach(Coord tile in tiles) {
			if (tile.tileY > maxTile.tileY) {
				maxTile = tile;
			}
		}
		
		return maxTile;
	}
	
	public Vector2 GetLandingSpot(Vector2 currentPosition) {
		Coord currentTile = GetTileFromScenePosition(currentPosition);
		Coord closestTile = GetTileClosestToTile(currentTile,ceilingTiles);
		closestTile.tileY --;
		return GetTilePositionInScene(closestTile);
	}
	
	Coord GetTileClosestToTile(Coord tile, List<Coord> tiles) {
		//Finds the closest tile in the list "tiles" to another tile.
		//This is being used it spawn the player near the upper left corner.
		
		Coord closestTile = tiles[0];
		float closestDistance = Distance(tile,closestTile);
		
		foreach (Coord _tile in tiles) {
			float distance = Distance(tile,_tile);
			if (distance < closestDistance) {
				closestTile = _tile;
				closestDistance = distance;
			}
		}
		return closestTile;
	}
	
	public float Distance(Coord tile1, Coord tile2) {
		//Return distance between tiles
		float dx = Mathf.Abs(tile1.tileX-tile2.tileX);
		float dy = Mathf.Abs(tile1.tileY-tile2.tileY);
		float distance = Mathf.Sqrt(Mathf.Pow(dy,2) + Mathf.Pow (dx,2));
		return distance;
	}
	
	public void setNewParameters(int _randomFillPercent,bool _useRandomSeed, int _width, int _height, string _seed, int _tileAmount, int _wallThresholdSize, int _roomThresholdSize, float _batSpawnRate, bool _regenerateMapOnLaunch) {
		//This is used to update the parameters from the editor window.
		this.randomFillPercent = _randomFillPercent;
		this.useRandomSeed = _useRandomSeed;
		this.width = _width;
		this.height = _height;
		this.seed = _seed;
		this.wallThresholdSize = _wallThresholdSize;
		this.roomThresholdSize = _roomThresholdSize;
		this.batSpawnRate  =_batSpawnRate;
		this.regenerateMapOnLaunch = _regenerateMapOnLaunch;
		
		MeshGenerator meshGen = GetComponent<MeshGenerator>();
		meshGen.tileAmount = _tileAmount;
	}
	
	public List<Coord> GetOpenTileCluster(Coord origin, int tileAmount) {
		
		List<Coord> cluster = new List<Coord>();
		List<Coord> uncheckedTiles = new List<Coord>();
		uncheckedTiles.Add(origin);
		
		while (cluster.Count < tileAmount && uncheckedTiles.Count > 0) {
			Coord currentTile = uncheckedTiles[UnityEngine.Random.Range(0,uncheckedTiles.Count-1)];
			
			int[] xValues = {-1,1,0,0};
			int[] yValues = {0,0,-1,1};
			
			for (int i = 0;i<4;i++) {
				int x = xValues[i] + currentTile.tileX;
				int y = yValues[i] + currentTile.tileY;
				Coord adjacentTile = new Coord(x,y);
				if (IsInMapRange(x,y) && openTiles.Contains(adjacentTile) && cluster.Contains(adjacentTile) == false) {
					uncheckedTiles.Add (adjacentTile);
				}
			}
			
			cluster.Add (currentTile);
			uncheckedTiles.Remove(currentTile);
		}
		
		return cluster;
	}
	
	//Graph Stuff (not finished)
	
	public void GenerateGraph() {
		graph = new Graph();
		graph.Init(map, 0);
		SetPlayerSpawn();
	}
	
	public void RefineGraph() {
		graph.Refine();
	}
	
	public void ShowGraph() {
		graph.OverlayGraph();
		
	}
	
	public void MakeTreeGraph() {
		GenerateGraph();
		graph.MakeTreeFromGraph(playerSpawn);
		Debug.Log ("Made Tree Graph");
	}
	
	public void OverlayTree() {
		graph.OverlayTree();
	}
	
	public void DisplayTreeGraph() {
		graph.MakeTreeFromGraph(playerSpawn);
	}
	
	public void DisplayTree() {
		graph.DisplayTree();
	}
	
	//	public void ShowFurthestEndpointFromPlayer() {
	//		GenerateGraph();
	//		
	//		graph.MakeTreeFromGraph(playerSpawn);
	//		
	//		List<Coord> filteredEndPoints = graph.GetFileteredEndpointsFromArray(100,this.floorTiles);
	//		Coord furthestEndPoint = filteredEndPoints[0];
	//		int maxDistance = graph.tree.Distance(playerSpawn,furthestEndPoint);
	//		foreach (Coord endpoint in filteredEndPoints) {
	//			int distance = graph.tree.Distance(playerSpawn,endpoint);
	//			if (distance > maxDistance) {
	//				maxDistance = distance;
	//				furthestEndPoint = endpoint;
	//			}
	//		}
	//		Debug.Log ("Max Distance From Player: " + maxDistance.ToString());
	//		DrawSquareAt(furthestEndPoint, 0.8f,Color.green);
	//	}
	
	public void ShowAllEndpoints() {
		GenerateGraph();
		List<Coord> endpoints = graph.GetEndpointsFromArray();
		foreach(Coord endpoint in endpoints) {
			DrawSquareAt(endpoint, 0.75f,Color.red);
		}
		
		Debug.Log ("Endpoints: " + endpoints.Count.ToString());
	}
	
	//	public void ShowFilteredEndpoints() {
	//		GenerateGraph();
	//		List<Coord> endpoints = graph.GetFileteredEndpointsFromArray(20,this.floorTiles);
	//		foreach(Coord endpoint in endpoints) {
	//			DrawSquareAt(endpoint, 0.8f,Color.white);
	//		}
	//		Debug.Log ("Endpoints: " + endpoints.Count.ToString());
	//	}
	
	
	//	public void PlaceItemsAtEndpoints() {
	//		GenerateGraph();
	//		List<Coord> endpoints = graph.GetFileteredEndpointsFromArray(20,this.floorTiles);
	//		foreach(Coord endpoint in endpoints) {
	//			AddObjectAt(endpoint,ceilingScenery,scenery);
	//		}
	//	}
	
	public void PlaceItemsAndGems() {
		List<Coord> endpoints = graph.GetFileteredEndpointsFromArray();
		Coord[] sortedPoints = SortEndpointsByDistance(endpoints);
		
		List<Coord> gemPositions = GetSpacedOutEndpoints(this.gemPrefabs.Length,sortedPoints);
		for (int i = 0; i<this.gemPrefabs.Length;i++) {
			if (i < gemPositions.Count) {
				AddObjectAt(gemPositions[i],this.gemPrefabs[i],this.gems);
			}
		}
	}
	
	List<Coord> GetSpacedOutEndpoints(int amount, Coord[] sortedEndpoints) {
		int interval = Mathf.RoundToInt(sortedEndpoints.Length / (amount-1));
		List<Coord> newEndpoints = new List<Coord>();
		for (int i = 0; i < amount; i++) {
			int newIndex = interval*i;
			if (newIndex > 0) {
				newIndex --;
			}
			//			Debug.Log ("New Index: " + newIndex.ToString());
			newEndpoints.Add (sortedEndpoints[newIndex]);
		}
		return newEndpoints;
	}
	
	//	public void PlaceGems() {
	//		List<Coord> endpoints = graph.GetFileteredEndpointsFromArray(gemPrefabs.Length,this.floorTiles);
	//		Coord[] sortedPoints = SortEndpointsByDistance(endpoints);
	//		for (int i = 0; i<this.gemPrefabs.Length;i++) {
	//			if (i < sortedPoints.Length) {
	//				AddObjectAt(sortedPoints[i],this.gemPrefabs[i],this.gems);
	//			}
	//		}
	//	}
	
	public Coord[] SortEndpointsByDistance(List<Coord> endpoints) {
		Coord[] sortedEndPoints = new Coord[endpoints.Count];
		for (int i = 0;i<endpoints.Count;i++) {
			sortedEndPoints[i] = endpoints[i];
		}
		float[] distances = new float[sortedEndPoints.Length];
		for (int i = 0; i < sortedEndPoints.Length; i++) {
			distances[i] = graph.tree.Distance(this.playerSpawn,sortedEndPoints[i]);
		}
		Array.Sort(distances,sortedEndPoints);
		return sortedEndPoints;
	}
	
}


public struct Coord
{
	public int tileX;
	public int tileY;
	
	public Coord(int x, int y)
	{
		tileX = x;
		tileY = y;
	}
	
	public string ToString() {
		string str = "(X: " + tileX.ToString() + ", Y: " + tileY.ToString() + ")";
		return str;
	}
}
