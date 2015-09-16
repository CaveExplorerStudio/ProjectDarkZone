using UnityEngine;
using System.Collections;
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

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(map, 1);
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
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height) //Ensure that the neighbors are in the maps, so as to catch errors of edge cases
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
