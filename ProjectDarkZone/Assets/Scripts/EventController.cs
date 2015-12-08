using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventController : MonoBehaviour {
	
	int gemsCollected = 0;
	int sanity = 100; //Replace once a dynamic value is created somewhere in the project.

	bool enableNumberTesting = false;
	
	//Prefabs
	public GameObject rock1;
	public GameObject batSwarm;

	public Transform rockSpawnPoint;

	//GameObjects
	GameObject camera;
	GameObject player;
	GameObject mapGenerator;
	GameObject audioDelegate;
	GameObject exit;
	
	//Scripts
	CameraController cameraController;
	PlayerController playerController;
	TorchPlacer torchPlacer;
	MapGenerator mapGenScript;
	AudioController audioController;
	
	
	// Use this for initialization
	void Start () {
		this.camera = GameObject.Find("Main Camera");
		this.player = GameObject.Find ("Player");
		this.mapGenerator = GameObject.Find ("Map Generator");
		this.audioDelegate = GameObject.Find ("Audio Delegate");
		this.exit = GameObject.Find ("Exit");
//		this.rockSpawnPoint = GameObject.Find ("Rock Spawn").transform;
		
		this.cameraController = camera.GetComponent<CameraController>();
		this.playerController = player.GetComponent<PlayerController>();
		this.torchPlacer = player.GetComponent<TorchPlacer>();
		this.mapGenScript = mapGenerator.GetComponent<MapGenerator>();
		this.audioController = audioDelegate.GetComponent<AudioController>();
	}

	bool didRecieveGemUpdate = false;

	// Update is called once per frame
	void Update () {
		if (didRecieveGemUpdate == false && PlayerController.heldGem != null) {
			didRecieveGemUpdate = true;
			gemsCollected = 1;
			audioController.ChangeAmbient();
		}


		DoEvents();

		if (enableNumberTesting) {
			CheckTests();
		}


	}

	bool hasShaken = false;
	bool hasDoneSecondShake = false;
	bool hasSweepedDim = false;
	bool hasSweepedOut = false;
	bool hasDoneRockSlide = false;


	void DoEvents() {

		float distFromExit = Vector2.Distance(exit.transform.position,player.transform.position);

		if (hasSweepedDim == false && distFromExit > 13.0f) {
			TorchSweepDim(40.0f,0.6f,0.05f);
			hasSweepedDim = true;
		}

		if (gemsCollected > 0) {
			if (hasShaken == false) {
				hasShaken = true;
				ShakeCamera(0.5f,5.0f);
			}
			else if (hasSweepedOut == false) {
				hasSweepedOut = true;
				TorchSweepOut(40.0f);
			}
			else if (hasDoneRockSlide == false && distFromExit < 30.0f) {
				hasDoneRockSlide = true;
				CauseCaveIn(5,rockSpawnPoint.position);
			}
			else if (hasDoneSecondShake == false && distFromExit < 15.0f) {
				hasDoneSecondShake = true;
				ShakeCamera(0.5f,5.0f);
			}
		}
	}

	void CheckTests() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Debug.Log("Event: Shaking Camera");
			ShakeCamera(0.5f,5.0f);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			Debug.Log("Event: Dimming Torches");
			DimAllTorches(0.1f,2.0f,0.0f);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			Debug.Log("Event: Blowing Out Player Torch");
			BlowOutPlayerTorch();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4)) {
			Debug.Log ("Event: Blowing Out Placed Torches");
			BlowOutPlacedTorches();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5)) {
			Debug.Log ("Event: Flickering Torches");
			FlickerPlacedTorches(0.50f);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6)) {
			Debug.Log ("Event: Rock Slide");
			CauseCaveIn(8);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7)) {
			Debug.Log ("Event: Torch Sweep Dim");
			TorchSweepDim(40.0f,0.6f,0.05f);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8)) {
			Debug.Log ("Event: Torch Sweep Out");
			TorchSweepOut(40.0f);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9)) {
			Debug.Log ("Event: Torch Sweep Pop");
			TorchSweepPop(40.0f);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha0)) {
			Debug.Log ("Event: Bat Swarm");
			SpawnBatSwarm();
		}
		else if (Input.GetKeyDown(KeyCode.Minus)) {
			Debug.Log ("Event: Combinatation");
			ShakeCamera(0.5f,5.0f);
			FlickerPlacedTorches(5.0f);
		}
	}
	
	//Events
	
	public void ShakeCamera(float intensity, float duration) {
		//Intensity: 0.0 - 1.0
		//Duration in seconds
		cameraController.BeginCameraShake(intensity,duration);
	}
	
	public void DimAllTorches(float dimPercent, float duration, float delay) {
		//DimPercent: 0.01-1.00
		//Duration in seconds
		torchPlacer.DimAllTorches(dimPercent,duration,delay);
	}
	
	public void BlowOutPlayerTorch() {
		//Blows out the torch being carried by the player
		GameObject playerTorch = GameObject.Find ("Player Torch");
		if (playerTorch != null) {
			TorchController torchController = playerTorch.GetComponent<TorchController>();
			torchController.BlowOut();
		}
	}
	
	public void BlowOutPlacedTorches() {
		foreach (Transform torch in torchPlacer.torches.transform) {
			TorchController torchController = torch.gameObject.GetComponent<TorchController>();
			torchController.BlowOut();
		}
	}
	
	public void FlickerPlacedTorches(float duration) {
		foreach (Transform torch in torchPlacer.torches.transform) {
			float offset = UnityEngine.Random.value/10;
			TorchController torchController = torch.gameObject.GetComponent<TorchController>();
			torchController.BeginFlicker(offset,offset+duration);
		}
	}
	
	public void CauseCaveIn(int rockAmount) {
		Coord playerPosition = mapGenScript.GetTileFromScenePosition(player.transform.position);
		List<Coord> openTiles = new List<Coord>(mapGenScript.openTiles);
		List<float> distances = new List<float>();
		foreach (Coord tile in mapGenScript.openTiles) {
			float distance = mapGenScript.Distance(playerPosition,tile);
			if (cameraController.IsInView(mapGenScript.GetTilePositionInScene(tile)) == false && tile.tileY > playerPosition.tileY) {
				distances.Add (distance);
			}
			else {
				openTiles.Remove(tile);
			}
		}
		
		int minDistIndex = distances.IndexOf(Mathf.Min (distances.ToArray()));
		Coord origin = openTiles[minDistIndex];
		
		List<Coord> rockSpawnTiles = mapGenScript.GetOpenTileCluster(origin,rockAmount);
		
		foreach(Coord spawnTile in rockSpawnTiles) {
			float randomScale = UnityEngine.Random.Range(10,60)/100.0f;
			this.rock1.transform.localScale = new Vector3(randomScale,randomScale,1.0f);
			mapGenScript.AddObjectAt(spawnTile,this.rock1,mapGenScript.rocks);
		}
	}

	public void CauseCaveIn(int rockAmount, Vector3 originV3) {

		Coord origin = mapGenScript.GetTileFromScenePosition(originV3);

		List<Coord> rockSpawnTiles = mapGenScript.GetOpenTileCluster(origin,rockAmount);
		
		foreach(Coord spawnTile in rockSpawnTiles) {
			float randomScale = UnityEngine.Random.Range(10,60)/100.0f;
			this.rock1.transform.localScale = new Vector3(randomScale,randomScale,1.0f);
			mapGenScript.AddObjectAt(spawnTile,this.rock1,mapGenScript.rocks);
		}
	}
	
	public void TorchSweepDim(float speed, float dimDuration, float dimPercentage) {
		
		GameObject[] torches = GameObject.FindGameObjectsWithTag("Torch");
		if (torches.Length > 0) {
			Array.Sort(torches,CompareXPos);
			float minX = torches[0].transform.position.x;
			foreach (GameObject torch in torches) {
				TorchController torchController = torch.GetComponent<TorchController>();
				float delay = (torch.transform.position.x - minX)/speed;
				torchController.BeginDim(dimPercentage,dimDuration,delay);
			}
			
			audioController.PlayBewareSound();
		}
	}
	
	private int CompareXPos(GameObject a, GameObject b) {
		return Math.Sign(a.transform.position.x - b.transform.position.x);
	}
	
	public void TorchSweepOut(float speed) {
		
		GameObject[] torches = GameObject.FindGameObjectsWithTag("Torch");

		Array.Sort(torches,CompareXPos);
		if (torches.Length > 0) {
			float minX = torches[0].transform.position.x;
			foreach (GameObject torch in torches) {
				TorchController torchController = torch.GetComponent<TorchController>();
				float delay = (torch.transform.position.x - minX)/speed;
				torchController.BeginBlowOut(delay);
			}
			
			
			audioController.PlayBewareSound();
		}
	}
	
	public void TorchSweepPop(float speed) {
		
		GameObject[] torches = GameObject.FindGameObjectsWithTag("Torch");
		Array.Sort(torches,CompareXPos);
		float minX = torches[0].transform.position.x;
		foreach (GameObject torch in torches) {
			TorchController torchController = torch.GetComponent<TorchController>();
			float delay = (torch.transform.position.x - minX)/speed;
			torchController.PopOffWall();
		}
		
		
		audioController.PlayBewareSound();
	}
	
	
	public void SpawnBatSwarm() {
		Coord playerPosition = mapGenScript.GetTileFromScenePosition(player.transform.position);
		List<Coord> openTiles = new List<Coord>(mapGenScript.openTiles);
		List<float> distances = new List<float>();
		foreach (Coord tile in mapGenScript.openTiles) {
			float distance = mapGenScript.Distance(playerPosition,tile);
			if (cameraController.IsInView(mapGenScript.GetTilePositionInScene(tile)) == false && tile.tileY > playerPosition.tileY) {
				distances.Add (distance);
			}
			else {
				openTiles.Remove(tile);
			}
		}
		
		int minDistIndex = distances.IndexOf(Mathf.Min (distances.ToArray()));
		Coord origin = openTiles[minDistIndex];
		mapGenScript.AddObjectAt(origin,batSwarm,mapGenerator);
	}
	
	
	
}
