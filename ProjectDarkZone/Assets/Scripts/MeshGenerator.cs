using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour {

    public SquareGrid squareGrid;
    public MeshFilter walls;
    public MeshFilter cave;

	[HideInInspector]
	public int tileAmount = 1;

    public bool is2D;

    List<Vector3> vertices;
    List<int> triangles;

    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>(); //Given a index representing a vertice, it will return all triangles that contain that vertice (purpose)
    List<List<int>> outlines = new List<List<int>>(); //Going to have multiple outlines, each defined by a list of ints for vertices. Thus, a list, in a list. 
    HashSet<int> checkedVertices = new HashSet<int>(); //Quicker to do "contains" checks on HashSet's versus lists, so to make sure we don't check a vertice twice, this will be useful.

    public void GenerateMesh(int[,] map, float squareSize)
    {
        outlines.Clear(); //Reset these variables with each new mesh
        checkedVertices.Clear();
        triangleDictionary.Clear();

        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

//        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
//        {
//            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
//            {
//                TriangulateSquare(squareGrid.squares[x, y]);
//            }
//        }

		for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
		{
			for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
			{
				TriangulateSquare(squareGrid.squares[x, y]);
			}
		}

        Mesh mesh = new Mesh();
        cave.mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {

			float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].x)*tileAmount;
			float percentY = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].z)*tileAmount;

            uvs[i] = new Vector2(percentX, percentY);
        }
        mesh.uv = uvs;

        if (is2D)
        {
            Generate2DColliders();
        }
        else
        {
            CreateWallMesh();
        }

    }

    void CreateWallMesh()
    {
        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 5; //Could change for 2D?

        foreach (List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count -1; i++)
            {
                int startIndex = wallVertices.Count;
                wallVertices.Add(vertices[outline[i]]); //Left vertex
                wallVertices.Add(vertices[outline[i+1]]); //Right vertex
                wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); //Bottom left vertex
                wallVertices.Add(vertices[outline[i + 1]] - Vector3.up * wallHeight); //Bottom right vertex

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3); //^^First triangle
                                                    //Both have anti-clockwise winding
                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0); //^^Second Triangle
            }
        }
        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        walls.mesh = wallMesh;

        MeshCollider wallCollider = walls.gameObject.AddComponent<MeshCollider>();
        wallCollider.sharedMesh = wallMesh;
    }

	public void Destroy2DColliders() {
		EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();
		for (int i = 0; i < currentColliders.Length; i++)
		{
			if (Application.isPlaying) {
				Destroy(currentColliders[i]);
				Debug.Log ("Destroying Old 2D Collider");
			}
			else if (Application.isEditor) {
				DestroyImmediate(currentColliders[i]);
			}
		}
	}
	
	void Generate2DColliders()
    {
//        EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();
//        for (int i = 0; i < currentColliders.Length; i++)
//        {
//			if (Application.isPlaying) {
//				Destroy(currentColliders[i]);
//				Debug.Log ("Destroying Old 2D Collider");
//			}
//			else if (Application.isEditor) {
//				DestroyImmediate(currentColliders[i]);
//			}
//        }

		Destroy2DColliders();

        CalculateMeshOutlines();

        foreach (List<int> outline in outlines)
        {
            EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            Vector2[] edgePoints = new Vector2[outline.Count];

            for (int i = 0; i < outline.Count; i++)
            {
                edgePoints[i] = new Vector2(vertices[outline[i]].x, vertices[outline[i]].z);
            }
            edgeCollider.points = edgePoints;
        }
    }

    void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft); //If all four are activated, it can't be an edge, so don't check the vertices for edges.
                checkedVertices.Add(square.topLeft.vertexIndex); 
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;

        }
    }

    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]); //All create different triangles to fill in the mesh

    }

    void AssignVertices(Node[] points)
    {
        for(int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count; //Each will incrementally be assigned a new number for a vertex, depending on how many are in the list.
                vertices.Add(points[i].position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle); //Add the triangle to each of the vertex lists of triangles
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }

    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
    {
        if (triangleDictionary.ContainsKey(vertexIndexKey)) //If the dictionary already has the vertex key, store the triangle there. 
        {
            triangleDictionary[vertexIndexKey].Add(triangle);
        }
        else //If not, create a list of triangles to then store at that vertex key
        {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            triangleDictionary.Add(vertexIndexKey, triangleList);
        }
    }

    void CalculateMeshOutlines() //Go through all vertices and check if it is outline, and if it, follow outline until meets up, and then add to the outline list.
    {
        for (int vertexIndex = 0; vertexIndex< vertices.Count; vertexIndex++)
        {
            if (!checkedVertices.Contains(vertexIndex))
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);

                if (newOutlineVertex != -1) // If there IS a connected outline vertex, add it to the checked HashSet, then create the new outline to add to the outlines.
                {
                    checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vertexIndex);
                }
            }
        }
    }

    void FollowOutline(int vertexIndex, int outlineIndex) //This will follow the trace of the outline, and add the vertices to the checked vertices
    {
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex); 

        if (nextVertexIndex != -1)
        {
            FollowOutline(nextVertexIndex, outlineIndex); //Calls itself to continue the trace, assuming it hasn't reached its tail
        }
    }

    int GetConnectedOutlineVertex(int vertexIndex) //Get a list of triangles at that index to determine what next index to go to.
    {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];

            for (int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];
                if (IsOutlineEdge(vertexIndex, vertexB) && vertexB != vertexIndex && !checkedVertices.Contains(vertexB)) //Don't want to check it against itself
                {
                    return vertexB; //Will return a new vertex that is an outline edge with the first.
                }
            }
        }
        return -1;
    }

    bool IsOutlineEdge(int vertexA, int vertexB) //If vertex A and vertex B share ONLY 1 triangle, then the edge between them must be an outline edge
    {
        List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
        int sharedTriangleCount = 0;

        for (int i = 0; i < trianglesContainingVertexA.Count; i++)
        {
            if (trianglesContainingVertexA[i].Contains(vertexB)) //ERROR
            {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1)
                {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }

	public void DestroyMeshAndCollider() {
		Destroy2DColliders();
		cave.mesh = null;

	}

    //For reference only, early visualization of map.
    /*
    void OnDrawGizmos() //Simply the visualization of the map
    { 
        if (squareGrid != null)
        {
            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
                    Gizmos.color = (squareGrid.squares[x, y].topLeft.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].topLeft.position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].topRight.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].topRight.position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].bottomLeft.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].bottomLeft.position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].bottomRight.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].bottomRight.position, Vector3.one * .4f);

                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(squareGrid.squares[x, y].centerTop.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centerRight.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centerBottom.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centerLeft.position, Vector3.one * .15f);
                }
            }
        } 
    }
    */

    struct Triangle 
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        int[] vertices;

        public Triangle (int a, int b, int c)
        {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;

            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        public int this[int i] //Indexer allows retrieval of vertices using array notation. Nifty, right?
        {
            get
            {
                return vertices[i];
            }
        }

        public bool Contains(int vertexIndex) //Easy way of determining if the triangle contains a certain vertex, good for IsOutlineEdge method
        {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapWidth / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);

                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }
    }

    public class Square //The square of nodes used for the marching square algorithm, defined by the nodes and control nodes
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;
        public int configuration; //Remember, 16 configuration of control node on/off (2^4, you know, as each one is on or off)

        public Square (ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft) //Defines a new square for marching squares, consisting of the control nodes and nodes
        {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;

            centerTop = topLeft.right;
            centerRight = bottomRight.above;
            centerBottom = bottomLeft.right;
            centerLeft = bottomLeft.above;

            if (topLeft.active) //First node, starting top left and then going clockwise
                configuration += 8;
            if (topRight.active) //Second node
                configuration += 4;
            if (bottomRight.active) //Third node
                configuration += 2;
            if (bottomLeft.active) //Fourth node
                configuration += 1;
        }
    }

    public class Node //Creates a new node, or item in our map grid (representing position, at least)
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos)
        {
            position = _pos;
        }
    }

    public class ControlNode : Node //Defines control nodes which inherit from nodes
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
        {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}
