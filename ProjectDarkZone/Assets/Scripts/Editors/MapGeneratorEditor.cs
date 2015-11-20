using UnityEngine;
using System.Collections;
using UnityEditor;

public class MapGeneratorEditor : EditorWindow {
	
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(MapGeneratorEditor));
	}
	
	public MapGeneratorEditor() {
		this.ResetParametersToDefault();
	}
	
	//Foldouts
	bool showParameters = true;
	bool showThresholds = false;
	bool showGraphOptions = false;
	bool showStepByStep = false;
	
	//Button
	float buttonHeight = 50.0f;
	float buttonHeightSmall = 30.0f;
	
	
	//Parameters - SET DEFUALTS IN "ResetParametersToDefault" FUNCTION, NOT HERE. These will be over overridden.
	int randomFillPercent;
	int width;
	int height;
	bool useRandomSeed;
	string seed;
	int wallThresholdSize;
	int roomThresholdSize;
	int tileAmount;
	float batSpawnRate;
	bool spawnBats;
	bool placeGems;
	bool regenerateMapOnLaunch;
	
	
	void ResetParametersToDefault() { //Put the default parameters here
		this.randomFillPercent = 50;
		this.width = 150;
		this.height = 150;
		this.useRandomSeed = true;
		this.seed = "CS196";
		this.wallThresholdSize = 50;
		this.roomThresholdSize = 50;
		this.tileAmount = 1;
		this.batSpawnRate = 1.0f;
		this.spawnBats = true;
		this.placeGems = false;
		this.regenerateMapOnLaunch = true;
	}
	
	
	void OnGUI()
	{	
		GUILayout.Label ("Map Generator", EditorStyles.boldLabel);
		
		//Buttons
		
		if (GUILayout.Button ("Clear", GUILayout.Height(buttonHeight))) {
			MeshGenerator meshGenScript = GameObject.Find ("Map Generator").GetComponent<MeshGenerator>();
			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			mapGenScript.CreateNecessaryGameObjects();
			meshGenScript.DestroyMeshAndCollider();
			mapGenScript.Clear();
		}
		
		if (GUILayout.Button ( "New", GUILayout.Height(buttonHeight))) {
			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			mapGenScript.CreateNecessaryGameObjects();
			mapGenScript.setNewParameters(randomFillPercent, useRandomSeed, width, height, seed, tileAmount, wallThresholdSize, roomThresholdSize, batSpawnRate, regenerateMapOnLaunch);
			mapGenScript.Clear();
			mapGenScript.GenerateMap();
			mapGenScript.GetTileTypes();
			if (this.spawnBats) {
				mapGenScript.AddBats();
			}
			mapGenScript.SetPlayerSpawn();
			mapGenScript.AddExit();
			if (this.placeGems) {
				mapGenScript.GenerateGraph();
				//				mapGenScript.GetTileTypes();

				mapGenScript.SpawnPlayer();
				mapGenScript.MakeTreeGraph();
				mapGenScript.SetItemSpawnPoints();
				mapGenScript.GrabItemSpawnPoints();
				mapGenScript.PlaceItemsAndGems();
			}
			else {
				mapGenScript.SpawnPlayer();
			}
			mapGenScript.AddScenery();
			//			mapGenScript.SpawnPlayer();
		}
		
		
		showParameters = EditorGUILayout.Foldout(showParameters, "Parameters");
		
		if (showParameters) {
			
			if (GUILayout.Button ("Reset To Default", GUILayout.Height(buttonHeightSmall))) {
				ResetParametersToDefault();
			}
			
			
			
			randomFillPercent = EditorGUILayout.IntSlider ("Fill %", randomFillPercent, 1, 100);
			width = EditorGUILayout.IntSlider ("Width", width, 2, 1000);
			height = EditorGUILayout.IntSlider ("Height", height, 2, 1000);
			
			tileAmount = EditorGUILayout.IntSlider ("Texture Tiling", tileAmount, 1, 1000);
			
			batSpawnRate = EditorGUILayout.Slider ("Bat Spawn Rate", batSpawnRate, 0.0f, 100.0f);
			
			wallThresholdSize = EditorGUILayout.IntSlider ("Wall", wallThresholdSize, 1, 1000);
			roomThresholdSize = EditorGUILayout.IntSlider ("Room", roomThresholdSize, 1, 1000);
			
			spawnBats = EditorGUILayout.Toggle("Spawn Bats", spawnBats);
			placeGems = EditorGUILayout.Toggle("Place Gems (Slower)", placeGems);
			
			regenerateMapOnLaunch = EditorGUILayout.Toggle("Regenerate Map On Launch", regenerateMapOnLaunch);
			
			if (spawnBats) {
				regenerateMapOnLaunch = true;
			}
			
			useRandomSeed = EditorGUILayout.Toggle("Use Random Seed", useRandomSeed);
			if (regenerateMapOnLaunch) {
				useRandomSeed = false;
			}
			if (!useRandomSeed) {
				if (GUILayout.Button ("Randmize Seed (Date)", GUILayout.Height(20), GUILayout.Width(140))) {
					seed = System.DateTime.Now.Ticks.ToString();
				}
				seed = EditorGUILayout.TextField ("     Seed", seed);
			}
			
			
			
			//			showThresholds = EditorGUILayout.Foldout(showThresholds, "Thresholds");
			//			if (showThresholds) {
			//				wallThresholdSize = EditorGUILayout.IntSlider ("Wall", wallThresholdSize, 1, 1000);
			//				roomThresholdSize = EditorGUILayout.IntSlider ("Room", roomThresholdSize, 1, 1000);
			//			}
		}
		
		
		//Buttons
		
		showStepByStep = EditorGUILayout.Foldout(showStepByStep, "Go Step-by-Step");
		if (showStepByStep) {
			
			if (GUILayout.Button ("Clear", GUILayout.Height(buttonHeightSmall))) {
				MeshGenerator meshGenScript = GameObject.Find ("Map Generator").GetComponent<MeshGenerator>();
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				meshGenScript.DestroyMeshAndCollider();
				mapGenScript.Clear ();
			}
			
			if (GUILayout.Button ( "Blank Map", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.setNewParameters(randomFillPercent, useRandomSeed, width, height, seed, tileAmount, wallThresholdSize, roomThresholdSize, batSpawnRate, regenerateMapOnLaunch);
				mapGenScript.Clear();
				mapGenScript.GenerateMap();
			}
			
			if (GUILayout.Button ("Calculate Tile Types", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.GetTileTypes();
				mapGenScript.ShowTileTypes();
			}
			
			if (GUILayout.Button ("Add Scenery", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.AddScenery();
			}
			
			if (GUILayout.Button ("Move Player to Spawn", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.GetTileTypes();
				mapGenScript.SpawnPlayer();
			}
			
			if (GUILayout.Button ("Add Bats", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.AddBats();
			}
			
			//			if (GUILayout.Button ("Find All Endpoints", GUILayout.Height(buttonHeightSmall))) {
			//				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			//
			//				mapGenScript.GenerateGraph();
			//				mapGenScript.RefineGraph();
			//				mapGenScript.ShowAllEndpoints();
			//			}
			//
			//			if (GUILayout.Button ("Find Filtered Endpoints", GUILayout.Height(buttonHeightSmall))) {
			//				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			//				mapGenScript.ShowFilteredEndpoints();
			//			}
			
			if (GUILayout.Button ("Place Gems", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.GenerateGraph();
				mapGenScript.GetTileTypes();
				mapGenScript.SetPlayerSpawn();
				mapGenScript.MakeTreeGraph();
				mapGenScript.PlaceItemsAndGems();
			}
		}
		
		showGraphOptions = EditorGUILayout.Foldout(showGraphOptions, "Graph");
		if (showGraphOptions) {
			
			if (GUILayout.Button ("Make And Show Grid Graph", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.GenerateGraph();
				mapGenScript.ShowGraph();
			}
			
			if (GUILayout.Button ("Make And Show Refined Graph (WIP)", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.GenerateGraph();
				mapGenScript.RefineGraph();
				mapGenScript.ShowGraph();
			}
			
			//			if (GUILayout.Button ("Place Items at Endpoints", GUILayout.Height(buttonHeightSmall))) {
			//				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			//				mapGenScript.GenerateGraph();
			//				mapGenScript.RefineGraph();
			//				mapGenScript.PlaceItemsAtEndpoints();
			//			}
			
			if (GUILayout.Button ("Make Tree", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.GenerateGraph();
				mapGenScript.GetTileTypes();
				mapGenScript.MakeTreeGraph();
			}
			
			if (GUILayout.Button ("Overlay Tree", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.OverlayTree();
			}
			
			if (GUILayout.Button ("Display Tree", GUILayout.Height(buttonHeightSmall))) {
				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
				mapGenScript.DisplayTree();
				
			}
			
			//			if (GUILayout.Button ("Find Furthest Endpoint", GUILayout.Height(buttonHeightSmall))) {
			//				MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			//				mapGenScript.ShowFurthestEndpointFromPlayer();
			//			}
			
			
		}
		
		
		//		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight, Screen.width*0.9f, 40), "Clear")) {
		//			MeshGenerator meshGenScript = GameObject.Find ("Map Generator").GetComponent<MeshGenerator>();
		//			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		//			meshGenScript.DestroyMeshAndCollider();
		//			mapGenScript.DestroyScenery();
		//		}
		//		
		//		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing, Screen.width*0.9f, 40), "New Map")) {
		//			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		//			mapGenScript.setNewParameters(randomFillPercent, useRandomSeed, width, height, seed, tileAmount, wallThresholdSize, roomThresholdSize, batSpawnRate);
		//			mapGenScript.DestroyScenery();
		//			mapGenScript.GenerateMap();
		//		}
		//
		//		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing*2, Screen.width*0.9f, 40), "Add Scenery")) {
		//			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		//			mapGenScript.AddScenery();
		//		}
		//
		//		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing*3, Screen.width*0.9f, 40), "Move Player to Spawn")) {
		//			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		//			mapGenScript.SpawnPlayer();
		//		}
		//
		//		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing*4, Screen.width*0.9f, 80), "Do All The Things!")) {
		//			MeshGenerator meshGenScript = GameObject.Find ("Map Generator").GetComponent<MeshGenerator>();
		//			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		//			meshGenScript.DestroyMeshAndCollider();
		//			mapGenScript.DestroyScenery();
		//			mapGenScript.setNewParameters(randomFillPercent, useRandomSeed, width, height, seed, tileAmount, wallThresholdSize, roomThresholdSize, batSpawnRate);
		//			mapGenScript.GenerateMap();
		//			mapGenScript.AddScenery();
		//			mapGenScript.SpawnPlayer();
		//		}
		//
		//		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing*6, Screen.width*0.9f, 40), "Make Grid Graph")) {
		//			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		//			mapGenScript.GenerateGraph();
		//			mapGenScript.ShowGraph();
		//		}
		//
		//		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing*7, Screen.width*0.9f, 40), "Make Refined Graph")) {
		//			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		//			mapGenScript.GenerateGraph();
		//			mapGenScript.RefineGraph();
		//			mapGenScript.ShowGraph();
		//		}
		//
		//		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing*8, Screen.width*0.9f, 40), "Place Items at Endpoints")) {
		//			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		//			mapGenScript.GenerateGraph();
		//			mapGenScript.RefineGraph();
		//			//mapGenScript.ShowGraph();
		//			mapGenScript.PlaceItemsAtEndpoints();
		//
		//		}
		
		
		
		
	}
	
	
	void OnInspectorUpdate() {
		Repaint();
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}
