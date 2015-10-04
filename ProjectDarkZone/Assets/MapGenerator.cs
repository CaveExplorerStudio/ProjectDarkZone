using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour {

    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed; 

    [Range(0,100)]
    public int randomFillPercent; //Deterimines amount of wall versus blank space

    int[,] map;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for(int i = 0; i < 5; i++) //Could consider modifying algorithm slightly as the i count goes up
        {
            SmoothMap(); //Iteration count can be modified to produce different looking caves.
        }

        ProcessMap();

        int borderSize = 1;
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2]; //Creates boarder around map (MIGHT NEED TO CHANGE TO CREATE MANY MAPS)

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
        meshGen.GenerateMesh(borderedMap, 1); //(Originally used "map" instead of boardered map)
    }

    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);
        int wallThresholdSize = 50; //MODIFY ME CAPTAIN

        foreach(List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize) //Remove all regions of walls with less than 50 coordinate tiles
            {
                foreach (Coord tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);
        int roomThresholdSize = 50; //MODIFY ME CAPTAIN
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize) //Remove all regions of walls with less than 50 coordinate tiles
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

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
            seed = Time.time.ToString();

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

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
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

    void OnDrawGizmos()
    {
        /*
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white; //If it is a wall, color it black, or a space, color it white. Visual debug code
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
        */
    }



}
