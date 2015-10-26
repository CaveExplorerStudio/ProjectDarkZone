using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Graph {

	public List<Coord> nodes;
	public List<Coord[]> links;
	public List<Coord>[,] linkArray;
	public int[,] array2D;

	public Tree tree;

	public float tileSize = 1.0f;

	public int height = 0;
	public int width = 0;


	public void SetTileSize(float size) {
		this.tileSize = size;
	}

	public void SetArray2D(int[,] array) {
		this.array2D = array;
	}
	
	public Graph() {

	}

	public void Init(int[,] _array2D, int valueOfNodes) {

		this.array2D = _array2D;
		this.width = array2D.GetLength(0);
		this.height = array2D.GetLength(1);

		linkArray = new List<Coord>[width,height];
		nodes = new List<Coord>();
		links = new List<Coord[]>();

		// Initiliaze linkArray for each node
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (array2D[x,y] == valueOfNodes) {
					this.linkArray[x,y] = new List<Coord>();
				}
			}
		}

		//Link nodes to neighboring nodes and keep track of nodes + links (AddNode and LinkNodes does this)
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				if (array2D[x,y] == valueOfNodes) {
					Coord currentNode = new Coord(x,y);
					AddNode(currentNode);

					List<Coord> adjacentNodes = GetAdjacentNodes(currentNode, valueOfNodes);

					foreach (Coord node in adjacentNodes) {
						//AddNode(node);

						LinkNodes(currentNode,node);
					}
				}
			}
		}
	}

	private void AddNode(Coord node) {
		//Adds node to nodes list
		nodes.Add(node);
	}

	private void LinkNodes(Coord node1, Coord node2) {
		//Links the two nodes in the linkArray and adds their link to the total links

		List<Coord> links1 = GetLinksFromNode(node1, this.linkArray);
		List<Coord> links2 = GetLinksFromNode(node2, this.linkArray);

		if (links1.Contains(node2) == false) {
			links1.Add (node2);
		}
		if (links2.Contains(node1) == false) {
			links2.Add (node1);
		}

		Coord[] newLink = new Coord[2];
		newLink[0] = node1;
		newLink[1] = node2;
		this.links.Add (newLink);
	}

	private List<Coord> GetLinksFromNode(Coord node, List<Coord>[,] _linkArray) {
		//Gets the links from the specified node in the linkArray
		return _linkArray[node.tileX,node.tileY];
	}

	public bool InRange(int x, int y, int[,] array2D) {
		int w = array2D.GetLength(0);
		int h = array2D.GetLength(1);
		return x >= 0 && x < w && y >= 0 && y < h;
	}

	private List<Coord> GetAdjacentNodes(Coord middleTile, int valueOfNodes) {
		// Returns the 4 nodes adjacent to "middleNode" in the 2D array
		// Used for linking nodes while making the grid graph.
		List<Coord> adjacentNodes = new List<Coord>();

		int x = middleTile.tileX;
		int y = middleTile.tileY;

		for (int x2 = 1; x2 >= -1; x2 -= 2) {
			int newX = x2 + x;
			if (InRange(newX,y,array2D) && array2D[newX,y] == valueOfNodes) {
				adjacentNodes.Add(new Coord(newX,y));
			}
		}
		for (int y2 = 1; y2 >= -1; y2 -= 2) {
			int newY = y2 + y;
			if (InRange(x,newY,array2D) && array2D[x,newY] == valueOfNodes) {
				adjacentNodes.Add(new Coord(x,newY));
			}
		}

		return adjacentNodes;
	}

	public float GetDistance(Coord tile1, Coord tile2) {
		//Return distance between tiles
		float distX = Mathf.Abs(tile1.tileX-tile2.tileX);
		float distY = Mathf.Abs(tile1.tileY-tile2.tileY);
		float distance = Mathf.Sqrt(Mathf.Pow(distY,2) + Mathf.Pow (distX,2));
		return distance;
	}

	public float GetDistanceSquared(Coord tile1, Coord tile2) {
		//Returns the distance squared.
		//This is faster because there is no need to square root distances when just comparing them.
		//This should only be used for comparisons, as it does not return the actual distance.
		float distX = Mathf.Abs(tile1.tileX-tile2.tileX);
		float distY = Mathf.Abs(tile1.tileY-tile2.tileY);
		return Mathf.Pow(distY,2) + Mathf.Pow (distX,2);
	}

	public void RecalculateNodesAndLinks() {
		// Recalculates nodes and links based on the current state of the linkArray.
		// Useful after many changes have been made to the linkArray
		links = new List<Coord[]>();
		nodes = new List<Coord>();

		List<Coord> checkedNodes = new List<Coord>();

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				if (this.linkArray[x,y] != null) { //The node exists at (x,y)
					Coord node = new Coord(x,y);
					nodes.Add (node);
					checkedNodes.Add (node);
					foreach (Coord linkedNode in this.linkArray[x,y]) {
						if (checkedNodes.Contains(linkedNode) == false) {
							Coord[] link = new Coord[2];
							link[0] = node;
							link[1] = linkedNode;
							links.Add(link);
						}
					}
				}
			}
		}
	}

	private List<List<Coord>> GetColumns() {

		List<List<Coord>> columns = new List<List<Coord>>();

		List<Coord> currentColumn = new List<Coord>();
		bool combiningNodesInColumn = false;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				if (linkArray[x,y] != null) {
					//Node exists
					if (combiningNodesInColumn) {
						currentColumn.Add (new Coord(x,y)); //Continuing to add to column
					}
					else {
						currentColumn = new List<Coord>(); //Starting new column
						currentColumn.Add (new Coord(x,y));
						combiningNodesInColumn = true;
					}
				}
				else {
					if (combiningNodesInColumn) { //Reached end of column
						combiningNodesInColumn = false;
						columns.Add (currentColumn);
					}
				}
			}
		}

		return columns;
	}

	private List<List<Coord>> GetRows() {
		
		List<List<Coord>> rows = new List<List<Coord>>();
		
		List<Coord> currentRow = new List<Coord>();
		bool combiningNodesInRow = false;
		
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				
				if (linkArray[x,y] != null) {
					//Node exists
					if (combiningNodesInRow) {
						currentRow.Add (new Coord(x,y)); //Continuing to add to column
					}
					else {
						currentRow = new List<Coord>(); //Starting new column
						currentRow.Add (new Coord(x,y));
						combiningNodesInRow = true;
					}
				}
				else {
					if (combiningNodesInRow) { //Reached end of column
						combiningNodesInRow = false;
						rows.Add (currentRow);
					}
				}
			}
		}
		
		return rows;
	}

	Coord GetMiddleNode(List<Coord> nodes) {
		//Returns the node in the center of a list (used for columns and rows)
		int middleNodeIndex = nodes.Count/2;
		return nodes[middleNodeIndex];
	}
	

	public void Refine() {
		// Removes repetative nodes in the grid graph
		// NOT finished (does not form a complete graph)
		List<Coord>[,] _linkArray = new List<Coord>[width,height];
		List<List<Coord>> columns = GetColumns();
		
		List<Coord> newNodes = new List<Coord>();
		
		foreach(List<Coord> column in columns) {
			Coord middleNode = GetMiddleNode(column);
			newNodes.Add (middleNode);
		}
		
		foreach(List<Coord> column in columns) {

			int linksToMake; //How many more columns it should link to
			if (IsEndColumn(column)) {
				linksToMake = 1;
			}
			else {
				linksToMake = 2;
			}
			
			Coord middleTile = GetMiddleNode(column);
			
			List<Coord> tempNodes = new List<Coord>(newNodes);
			List<Coord> closestTiles = new List<Coord>(); //The nodes that will be linked to

			while (closestTiles.Count < linksToMake) {
				Coord closestTile = tempNodes[0];
				float minDistance = GetDistanceSquared(tempNodes[0],middleTile);
				
				foreach (Coord tile in tempNodes) {
					float distance = GetDistanceSquared (tile, middleTile);
					if (middleTile.tileX != tile.tileX) {
						if (closestTiles.Count > 0) {
							if (Mathf.Sign(closestTiles[0].tileX - middleTile.tileX) != Mathf.Sign (tile.tileX - middleTile.tileX)) { //Makes sure that the two new tiles are on opposite sides of the column
								if (distance <= minDistance) {
									minDistance = distance;
									closestTile = tile;
								}
							}
						}
						else {
							if (distance <= minDistance) {
								minDistance = distance;
								closestTile = tile;
							}
						}
					}
				}
				closestTiles.Add (closestTile);
				if (tempNodes.Count > 1) {
					tempNodes.Remove(closestTile);
				}
			}

			foreach (Coord tile in closestTiles) {
				//Link middle tile to closest tiles
				LinkNodesInLinkArray(tile,middleTile,_linkArray);
			}
		}
		linkArray = _linkArray;
		RecalculateNodesAndLinks();
	}

	private void LinkNodesInLinkArray(Coord node1, Coord node2, List<Coord>[,] _linkArray) {
		//Adds each node to the other's linkList in the linkArray
		//If the node doesn't have a linkList then it initializes one
		if (node1.tileX != node2.tileX || node1.tileY != node2.tileY) {
			
			if (_linkArray[node2.tileX,node2.tileY] == null) {
				_linkArray[node2.tileX,node2.tileY] = new List<Coord>();
			}
			if (_linkArray[node1.tileX,node1.tileY] == null) {
				_linkArray[node1.tileX,node1.tileY] = new List<Coord>();
			}
			if (_linkArray[node2.tileX,node2.tileY].Contains(node1) == false) {
				_linkArray[node2.tileX,node2.tileY].Add (node1);
			}
			if (_linkArray[node1.tileX,node1.tileY].Contains(node2) == false) {
				_linkArray[node1.tileX,node1.tileY].Add (node2);
			}
		}
	}

	public void MakeConnectedGraph() {
		//NOT finsihed
		Coord startNode = nodes[0];

		List<Coord> nodesToCheck = new List<Coord>();
		nodesToCheck.Add (startNode);

		List<Coord> checkedNodes = new List<Coord>();
		List<Coord> unconnectedNodes = new List<Coord>();
	}

	bool IsEndColumn(List<Coord> column) {
		//Returns true if the column has all wall tiles on one or both sides.
		bool rightIsFilled = true;
		bool leftIsFilled = true;
		
		foreach (Coord tile in column) {
			if (array2D[tile.tileX+1,tile.tileY] == 0) {
				rightIsFilled = false;
			}
			if (array2D[tile.tileX-1,tile.tileY] == 0) {
				leftIsFilled = false;
			}
		}
		
		if (!leftIsFilled && !rightIsFilled) {
			return false;
		}
		else {
			return true;
		}
	}

	bool IsEndRow(List<Coord> row) {
		//Returns true if the row has all wall tiles on above or below it.
		bool topIsFilled = true;
		bool bottomIsFilled = true;
		
		foreach (Coord tile in row) {
			if (array2D[tile.tileX,tile.tileY+1] == 0) {
				topIsFilled = false;
			}
			if (array2D[tile.tileX,tile.tileY-1] == 0) {
				bottomIsFilled = false;
			}
		}
		
		if (!bottomIsFilled && !topIsFilled) {
			return false;
		}
		else {
			return true;
		}
	}

	public List<Coord> GetEndpointsFromArray() {
		// 1.) Iterates through all continuous rows and columns of nodes in the graph
		// 2.) Determines which ones are next to a wall (end row/column)
		// 3.) Finds the middle of the end rows/columns and returns these as the endpoints

		List<List<Coord>> rows = GetRows();
		List<List<Coord>> columns = GetColumns();

		List<Coord> endpoints = new List<Coord>();

		foreach (List<Coord> row in rows) {
			if (IsEndRow(row)) {
				endpoints.Add (GetMiddleNode(row));
			}
		}

		foreach (List<Coord> column in columns) {
			if (IsEndColumn(column)) {
				endpoints.Add (GetMiddleNode(column));
			}
		}

		return endpoints;
	}

	public List<Coord> GetFileteredEndpointsFromArray() {
		
		List<List<Coord>> rows = GetRows();
		List<List<Coord>> columns = GetColumns();
		
		List<Coord> endpoints = new List<Coord>();
		
		foreach (List<Coord> row in rows) {
			if (IsEndRow(row)) {
				Coord middleNode = GetMiddleNode(row);
				if (array2D[middleNode.tileX,middleNode.tileY-1] != 0) {
					endpoints.Add (middleNode);
				}
			}
		}
		
		foreach (List<Coord> column in columns) {
			if (IsEndColumn(column)) {
				endpoints.Add (GetMiddleNode(column));
			}
		}

		return endpoints;
	}

	
	public List<Coord> GetEndpointsFromGraph() {
		//Returns the nodes in the linkArray with only 1 link
		List<Coord> endpoints = new List<Coord>();
		
		foreach (Coord node in nodes) {
			if (linkArray[node.tileX,node.tileY].Count == 1) {
				endpoints.Add(node);
			}
		}
		return endpoints;
	}

	public List<Coord> MergeEndpoints(List<Coord> endpoints, int finalAmount, List<Coord> floorTiles) {

		float mergeRadius = 0.0f;
		List<Coord> mergedNodes = new List<Coord>();
		List<Coord> mergedEndpoints = new List<Coord>();


		do {
			mergeRadius ++;
			mergedNodes = new List<Coord>();
			mergedEndpoints = new List<Coord>();

			foreach(Coord node in endpoints) {
	
				if (mergedNodes.Contains(node) == false) {
					mergedNodes.Add (node);
					List<Coord> nearbyeNodes = new List<Coord>();
					
					foreach(Coord otherNode in endpoints) { 
						
						if (node.tileX != otherNode.tileX && node.tileY != otherNode.tileY && mergedNodes.Contains(otherNode) == false) {
							if (GetDistance(node,otherNode) <= mergeRadius) {
								mergedNodes.Add(otherNode);
								nearbyeNodes.Add (otherNode);
							}
						}
					}
					if (nearbyeNodes.Count > 0) {
						Coord mergedEndpoint;
						bool didContainFloorTile = false;
						foreach(Coord nearbyeNode in nearbyeNodes) {
							Coord tempNode = nearbyeNode;
							tempNode.tileY -= 1;
							if (floorTiles.Contains(tempNode)) {
								mergedEndpoint = nearbyeNode;
								mergedEndpoints.Add (mergedEndpoint);
								didContainFloorTile = true;

//								Debug.Log ("Setting FloorTile During Merge");
								break;
							}
						}
						if (!didContainFloorTile) {
							mergedEndpoint = nearbyeNodes[UnityEngine.Random.Range(0, nearbyeNodes.Count)];
							mergedEndpoints.Add (mergedEndpoint);
						}
//						Coord mergedEndpoint = nearbyeNodes[UnityEngine.Random.Range(0, nearbyeNodes.Count)];
//						mergedEndpoints.Add (mergedEndpoint);
					}
					else {
						mergedEndpoints.Add(node);
					}
				}
			}
		} while (mergedEndpoints.Count > finalAmount);

		//Old:
//		float mergeRadius = 10.0f;
//		List<Coord> mergedNodes = new List<Coord>();
//		List<Coord> mergedEndpoints = new List<Coord>();
//
//		foreach(Coord node in endpoints) {
//
//			if (mergedNodes.Contains(node) == false) {
//				mergedNodes.Add (node);
//				List<Coord> nearbyeNodes = new List<Coord>();
//				
//				foreach(Coord otherNode in endpoints) { 
//					
//					if (node.tileX != otherNode.tileX && node.tileY != otherNode.tileY && mergedNodes.Contains(otherNode) == false) {
//						if (GetDistance(node,otherNode) <= mergeRadius) {
//							mergedNodes.Add(otherNode);
//							nearbyeNodes.Add (otherNode);
//						}
//					}
//				}
//				if (nearbyeNodes.Count > 0) {
//					Coord mergedEndpoint = nearbyeNodes[UnityEngine.Random.Range(0, nearbyeNodes.Count)];
//					mergedEndpoints.Add (mergedEndpoint);
//				}
//				else {
//					mergedEndpoints.Add(node);
//				}
//			}
//		}

		return mergedEndpoints;

	}

	public static bool CoordsAreEqual(Coord coord1, Coord coord2) {
		//Returns true if the coords have the same x and y values
		return (coord1.tileX == coord2.tileX) && (coord1.tileY == coord2.tileY);
	}

	//Speed Test

	private List<Coord>[,] DisconnectNode(Coord node, List<Coord>[,] _linkArray) {

		foreach (Coord linkedNode in _linkArray[node.tileX,node.tileY]) {
			List<Coord> nodeLinks = _linkArray[linkedNode.tileX,linkedNode.tileY];
			nodeLinks.Remove(node);
			_linkArray[linkedNode.tileX,linkedNode.tileY] = nodeLinks;
		}

		_linkArray[node.tileX,node.tileY] = new List<Coord>();

		return _linkArray;
	}


	public void MakeTreeFromGraph(Coord rootNode) {

		//List<Coord> connectedNodes = new List<Coord>(this.nodes);

	//	List<Coord> unconnectedNodes = new List<Coord>(this.nodes);
		//connectedNodes.Add(rootNode);
		Queue<Coord> nodesToConnect = new Queue<Coord>();
		nodesToConnect.Enqueue(rootNode);

		List<Coord>[,] tempLinkArray = new List<Coord>[this.linkArray.GetLength(0),this.linkArray.GetLength(1)];
		Array.Copy(this.linkArray,tempLinkArray,this.linkArray.GetLength(0)*this.linkArray.GetLength(1));

		Tree newTree = new Tree(rootNode);
		//Coord lastParent = rootNode;

		while (nodesToConnect.Count > 0) {

			Coord node = nodesToConnect.Dequeue();
			List<Coord> linkedNodes = GetLinksFromNode(node,tempLinkArray);
			tempLinkArray = DisconnectNode(node,tempLinkArray);
//			List<Coord> unconnectedLinkedNodes = new List<Coord>();
//			foreach(Coord linkedNode in linkedNodes) {
//				if (unconnectedNodes.Contains(linkedNode)) {
//					unconnectedLinkedNodes.Add (linkedNode);
//				}
//			}
			foreach (Coord linkedNode in linkedNodes) {
				if (nodesToConnect.Contains(linkedNode) == false) {
					newTree.AddNode(node,linkedNode);
					nodesToConnect.Enqueue(linkedNode);
				}

//				foreach (Coord unconnectedNode in unconnectedNodes) {
//					if (CoordsAreEqual(unconnectedNode,linkedNode)) {
//						unconnectedNodes.Remove(unconnectedNode);
//						break;
//					}
//				}
			}
		}

		this.tree = newTree;
		Debug.Log ("Graph Nodes: " + nodes.Count.ToString());
		Debug.Log ("Tree Nodes:  " + tree.nodes.Count.ToString());
	}


//	public void MakeTreeFromGraph(Coord rootNode) {
//
//		List<Coord> unconnectedNodes = new List<Coord>(this.nodes);
//		Queue<Coord> nodesToConnect = new Queue<Coord>();
//		nodesToConnect.Enqueue(rootNode);
//
//		Tree newTree = new Tree(rootNode);
//		Coord lastParent = rootNode;
//
//		while (unconnectedNodes.Count > 0) {
//
//			Coord node = nodesToConnect.Dequeue();
//			List<Coord> linkedNodes = GetLinksFromNode(node,this.linkArray);
//			List<Coord> unconnectedLinkedNodes = new List<Coord>();
//			foreach(Coord linkedNode in linkedNodes) {
//				if (unconnectedNodes.Contains(linkedNode)) {
//					unconnectedLinkedNodes.Add (linkedNode);
//				}
//			}
//			foreach (Coord linkedNode in unconnectedLinkedNodes) {
//				newTree.AddNode(node,linkedNode);
//				nodesToConnect.Enqueue(linkedNode);
//				foreach (Coord unconnectedNode in unconnectedNodes) {
//					if (CoordsAreEqual(unconnectedNode,linkedNode)) {
//						unconnectedNodes.Remove(unconnectedNode);
//						break;
//					}
//				}
//			}
//		}
//
//		this.tree = newTree;
//		Debug.Log ("Graph Nodes: " + nodes.Count.ToString());
//		Debug.Log ("Making Tree, Unconnected Count = " + unconnectedNodes.Count.ToString());
//	}



	public Vector2 GetPositionInScene(Coord tile) {
		Vector2 relativePos = new Vector2(tile.tileX * tileSize - (width/2)*tileSize + tileSize/2, tile.tileY*tileSize- (width/2)*tileSize + tileSize/2);
		return relativePos;
	}
	
	public Vector2 GetPositionInSceneFloat(float x, float y) {
		Vector2 relativePos = new Vector2(x * tileSize - (width/2)*tileSize + tileSize/2, y*tileSize- (width/2)*tileSize + tileSize/2);
		return relativePos;
	}

	private void DrawLink(Coord[] link) {
		Vector2 startPos = GetPositionInScene(link[0]);
		Vector2 endPos = GetPositionInScene(link[1]);
		Debug.DrawLine (startPos, endPos, Color.cyan, 1);
	}

	private void DrawLinkAt(float x1, float y1, float x2, float y2) {
		Vector2 startPos = new Vector2(x1,y1);
		Vector2 endPos = new Vector2(x2,y2);
		Debug.DrawLine (startPos, endPos, Color.cyan, 1);
		Debug.Log ("Drawing Line");
	}
	
	public void DrawNode(Coord node) {
		float x = (float)node.tileX;
		float y = (float)node.tileY;
		DrawNodeAt(x,y);
	}
	
	public void DrawNodeAt(float x, float y) {
		
		List<float[]> squareVertices = new List<float[]>();
		float radius = 0.25f;
		
		for (float x2 = radius; x2 >= -radius; x2 -= radius*2) {
			for (float y2 = radius; y2 >= -radius; y2 -= radius*2) {
				float[] fArray = new float[2];
				fArray[0] = x + x2;
				fArray[1] = y + y2;
				squareVertices.Add (fArray);
			}
		}
		
		//Swap
		float[] temp = squareVertices[2];
		squareVertices[2] = squareVertices[3];
		squareVertices[3] = temp;
		
		for (int i = 0; i < squareVertices.Count; i++) {
			if (i < squareVertices.Count-1) {
				Debug.DrawLine(GetPositionInSceneFloat(squareVertices[i][0],squareVertices[i][1]),GetPositionInSceneFloat(squareVertices[i+1][0],squareVertices[i+1][1]), Color.red);
			}
			else {
				Debug.DrawLine(GetPositionInSceneFloat(squareVertices[i][0],squareVertices[i][1]),GetPositionInSceneFloat(squareVertices[0][0],squareVertices[0][1]),Color.red);
			}
		}
	}

	public void OverlayGraph() {
		
		foreach (Coord node in this.nodes) {
			DrawNode(node);
		}
		
		foreach (Coord[] link in this.links) {
			DrawLink(link);
		}
		Debug.Log ("Graph Displayed");
		foreach (Coord node in this.nodes) {
			int linkCount = linkArray[node.tileX,node.tileY].Count;
		}
	}

	public void DisplayTree() {
		List<List<TreeNode>> levels = tree.GetLevels();

		Debug.Log ("Levels: " + levels.Count.ToString());

		int currentLevel = 0;
		int currentNode = 0;
		int xoffset = 2;
		int yoffset = -2;
		
		foreach (List<TreeNode> level in levels) {
			currentNode = 0;
			foreach (TreeNode node in level) {
				float xPos = currentNode*xoffset;
				float yPos = currentLevel*yoffset;
				DrawNodeAt(xPos,yPos);
				if (CoordsAreEqual(node.node,tree.rootNode.node)==false) {
					int parentIndexInLevel = -1;

					for (int i = 0; i < levels[currentLevel-1].Count;i++) {
						List<Coord> children = levels[currentLevel-1][i].children;
						for (int c = 0; c < children.Count;c++) {
							if (CoordsAreEqual(children[c],node.node)) {
								parentIndexInLevel = i;
								break;
							} 
						}
					}

					float newXPos = xPos - width*this.tileSize/2+this.tileSize/2;
					float newYPos = yPos - height*this.tileSize/2 + this.tileSize/2;
					float parentXPos = (parentIndexInLevel*xoffset) - width*this.tileSize/2+this.tileSize/2;
					float parentYPos = (currentLevel*yoffset - yoffset) - height*this.tileSize/2 + this.tileSize/2;

					DrawLinkAt(newXPos,newYPos,parentXPos,parentYPos);
				}
				currentNode ++;
			}
			currentLevel ++;
		}
	}

	public void ShowTreeNodeAndLinks(Coord node) {
		DrawNode(node);
		List<Coord> children = tree.GetTreeNodeFromCoord(node).children;
		foreach(Coord child in children) {
			Coord[] link = new Coord[2];
			link[0] = node;
			link[1] = child;
			DrawLink(link);
		}
	}

	public void OverlayTree() {
		//Shows the tree with the nodes in there actual positions
		Queue<Coord> nodesToDraw = new Queue<Coord>();
		foreach (TreeNode tNode in tree.nodes) {
			ShowTreeNodeAndLinks(tNode.node);

		}
		Debug.Log ("Tree Drawn\nNodes: " + tree.nodes.Count.ToString());
	}


}



public struct TreeNode {
	
	public Coord parent;
	public Coord node;
	public List<Coord> children;
	
	public void SetAll(Coord _parent, Coord _node, List<Coord> _children) {
		this.parent = _parent;
		this.node = _node;
		this.children = new List<Coord>();
		this.children = _children;
	}
	
	public void SetRoot(Coord _node, List<Coord> _children) {
		this.node = _node;
		this.children = new List<Coord>();
		this.children = _children;
	}
	
	public void AddChild(Coord child) {
		if (this.children.Contains(child) == false) {
			this.children.Add(child);
		}
	}
}

public struct Tree
{
	
	public TreeNode rootNode;
	public List<TreeNode> nodes;
	
	public Tree(Coord root) {
		this.rootNode = new TreeNode();
		this.rootNode.SetRoot(root,new List<Coord>());
		this.nodes = new List<TreeNode>();
		this.nodes.Add(rootNode);
	}
	
	public void AddNode(Coord parent, Coord node) {
		TreeNode parentNode = this.nodes[GetIndexInNodes(parent)];
		parentNode.AddChild(node);
		TreeNode newNode = new TreeNode();
		newNode.SetAll(parent,node,new List<Coord>());
		this.nodes.Add(newNode);
	}
	
	private int GetIndexInNodes(Coord node) {
		for (int i = 0; i < this.nodes.Count; i++) {
			if (Graph.CoordsAreEqual(nodes[i].node,node)) {
				return i;
			}
		}
		Debug.Log ("Couldn't find node in nodes.\nNode: X = " + node.tileX.ToString() + " Y = " + node.tileY.ToString());
		return -1;
	}
	
	public Coord GetParent(Coord child) {
		TreeNode childNode = this.nodes[GetIndexInNodes(child)];
		return childNode.parent;
	}

	public TreeNode GetParentTreeNode(Coord child) {
		TreeNode childNode = GetTreeNodeFromCoord(child);
		return GetTreeNodeFromCoord(childNode.parent);
	}

	public TreeNode GetTreeNodeFromCoord(Coord node) {
		return this.nodes[GetIndexInNodes(node)];
	}
	
	public List<Coord> GetChain(Coord start, Coord end) {
		//Starts from the bottom and works its way up the tree through parent nodes until reaching the start node.
		
		TreeNode endNode = GetTreeNodeFromCoord(end);
		TreeNode currentParent =  GetTreeNodeFromCoord(GetParentTreeNode(end).parent);
		List<Coord> chain = new List<Coord>();
		chain.Add(endNode.node);
		while (Graph.CoordsAreEqual(currentParent.node,start) == false) {
			chain.Add(currentParent.node);
			currentParent = GetParentTreeNode(currentParent.node);
		}
		chain.Add (currentParent.node);

		return chain;
	}

	public int Distance(Coord start, Coord end) {
//		Debug.Log ("Chain Start: " + start.ToString());
//		Debug.Log ("Chain End: " + end.ToString());

		return GetChain(start,end).Count;
	}

	public List<List<TreeNode>> GetLevels() {
		List<List<TreeNode>> levels = new List<List<TreeNode>>();
		List<TreeNode> checkedNodes = new List<TreeNode>();
//		nodesToCheck.Add (rootNode);
		levels.Add (new List<TreeNode>());
		levels[0].Add (this.rootNode);
		int currentLevel = 0;

		while (levels[currentLevel].Count > 0) {
			levels.Add (new List<TreeNode>());
			foreach (TreeNode node in levels[currentLevel]) {
				if (checkedNodes.Contains(node) == false) {
					checkedNodes.Add(node);
					foreach (Coord child in node.children) {
						TreeNode childTreeNode = GetTreeNodeFromCoord(child);
						levels[currentLevel+1].Add (childTreeNode);
					}
				}
			}
			currentLevel ++;
		}
		
//		while (true) {
//			if (levels[currentLevel].Count > 0) {
//				levels.Add (new List<TreeNode>());
//				foreach (TreeNode node in levels[currentLevel]) {
//					foreach (Coord child in node.children) {
//						TreeNode childTreeNode = GetTreeNodeFromCoord(child);
//						levels[currentLevel+1].Add (childTreeNode);
//					}
//				}
//				currentLevel ++;
//			}
//			else {
//				break;
//			}
//		}
		return levels;
	}
}


