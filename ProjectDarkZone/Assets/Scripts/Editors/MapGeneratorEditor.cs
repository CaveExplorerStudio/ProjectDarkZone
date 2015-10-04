using UnityEngine;
using System.Collections;
using UnityEditor;

public class MapGeneratorEditor : EditorWindow {

	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(MapGeneratorEditor));
	}

	//Foldouts
	bool showThresholds = false;

	//Spacing
	int buttonStartHeight = 210;
	int buttonSpacing = 50;

	//Parameters
	int randomFillPercent = 50;
	int width = 100;
	int height = 100;
	bool useRandomSeed = true;
	string seed = "0";
	int wallThresholdSize = 50;
	int roomThresholdSize = 50;
	int tileAmount = 1;
	float batSpawnRate = 1.0f;



	void OnGUI()
	{	
		GUILayout.Label ("Map Generator", EditorStyles.boldLabel);

		randomFillPercent = EditorGUILayout.IntSlider ("Fill %", randomFillPercent, 1, 100);
		width = EditorGUILayout.IntSlider ("Width", width, 2, 1000);
		height = EditorGUILayout.IntSlider ("Height", height, 2, 1000);

		tileAmount = EditorGUILayout.IntSlider ("Texture Tiling", tileAmount, 1, 1000);
		useRandomSeed = EditorGUILayout.Toggle("Use Random Seed", useRandomSeed);
		if (!useRandomSeed) {
			seed = EditorGUILayout.TextField ("Seed", seed);
		}

		batSpawnRate = EditorGUILayout.Slider ("Bat Spawn Rate", batSpawnRate, 0.0f, 100.0f);

		showThresholds = EditorGUILayout.Foldout(showThresholds, "Thresholds");
		if (showThresholds) {
			wallThresholdSize = EditorGUILayout.IntSlider ("Wall", wallThresholdSize, 1, 1000);
			roomThresholdSize = EditorGUILayout.IntSlider ("Room", roomThresholdSize, 1, 1000);
		}
		
		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight, Screen.width*0.9f, 40), "Clear")) {
			MeshGenerator meshGenScript = GameObject.Find ("Map Generator").GetComponent<MeshGenerator>();
			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			meshGenScript.DestroyMeshAndCollider();
			mapGenScript.DestroyScenery();
		}
		
		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing, Screen.width*0.9f, 40), "New Map")) {
			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			mapGenScript.setNewParameters(randomFillPercent, useRandomSeed, width, height, seed, tileAmount, wallThresholdSize, roomThresholdSize, batSpawnRate);
			mapGenScript.DestroyScenery();
			mapGenScript.GenerateMap();
		}

		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing*2, Screen.width*0.9f, 40), "Add Scenery")) {
			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			mapGenScript.AddScenery();
		}

		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing*3, Screen.width*0.9f, 40), "Move Player to Spawn")) {
			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			mapGenScript.SpawnPlayer();
		}

		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing*4, Screen.width*0.9f, 80), "Do All The Things!")) {
			MeshGenerator meshGenScript = GameObject.Find ("Map Generator").GetComponent<MeshGenerator>();
			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			meshGenScript.DestroyMeshAndCollider();
			mapGenScript.DestroyScenery();
			mapGenScript.setNewParameters(randomFillPercent, useRandomSeed, width, height, seed, tileAmount, wallThresholdSize, roomThresholdSize, batSpawnRate);
			mapGenScript.GenerateMap();
			mapGenScript.AddScenery();
			mapGenScript.SpawnPlayer();
		}

		if (GUI.Button (new Rect (Screen.width/20, buttonStartHeight + buttonSpacing*6, Screen.width*0.9f, 40), "Make Graph (Unfinished)")) {
			MapGenerator mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
			mapGenScript.GenerateGraph();
			mapGenScript.ShowGraph();
		}




	}


	void OnInspectorUpdate() {
		Repaint();
	}















}
