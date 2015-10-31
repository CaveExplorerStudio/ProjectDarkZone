using UnityEngine;
using System.Collections;

public class EventController : MonoBehaviour {

	int gemsCollected = 0;
	int sanity = 100; //Replace once a dynamic value is created somewhere in the project.


	//GameObjects
	GameObject camera;
	GameObject player;

	//Scripts
	CameraController cameraController;
	PlayerController playerController;
	TorchPlacer torchPlacer;


	// Use this for initialization
	void Start () {
		this.camera = GameObject.Find("Main Camera");
		this.player = GameObject.Find ("Player");

		this.cameraController = camera.GetComponent<CameraController>();
		this.playerController = player.GetComponent<PlayerController>();
		this.torchPlacer = player.GetComponent<TorchPlacer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Debug.Log("Event: Shaking Camera");
			ShakeCamera(0.5f,5.0f);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			Debug.Log("Event: Dimming Torches");
			DimAllTorches(0.5f,2.0f);
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
		else if (Input.GetKeyDown(KeyCode.Alpha0)) {
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

	public void DimAllTorches(float dimPercent, float duration) {
		//DimPercent: 0.01-1.00
		//Duration in seconds
		torchPlacer.DimAllTorches(dimPercent,duration);
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



}
