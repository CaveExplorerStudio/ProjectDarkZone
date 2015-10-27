using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	
	// The camera will automatically follow the player, but stop when the view touches the edge of the cave.
	// Automatically adjusts to any screen size.
	
	Transform player;
	PlayerController playerController;
	
	public MapGenerator mapGenScript;
	
	Vector2 maxPosition;
	
	float xMax;
	float xMin;
	float yMax;
	float yMin;
	
	public float viewOffset = 5.0f;
	public float panInterval = 0.2f;
	
	bool shouldPanRight;
	bool shouldPanLeft;
	bool shouldShakeCamera = false;
	
	float cameraShakeStartTime;
	float cameraShakeDuration;
	
	
	AudioSource audioSource;
	public AudioClip shakeSound;
	
	
	void Start () {
		
		this.audioSource = this.GetComponent<AudioSource>();
		
		if (player == null) {
			player = GameObject.Find ("Player").transform;
		}
		
		this.playerController = player.GetComponent<PlayerController>();
		
		MoveToPlayer();
		
		float vertExtent = Camera.main.orthographicSize;    
		float horzExtent = vertExtent * Screen.width / Screen.height;
		
		mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		maxPosition = mapGenScript.GetTilePositionInScene(new Coord(mapGenScript.width-1,mapGenScript.height-1));
		float mapX = maxPosition.x;
		float mapY = maxPosition.y;
		
		xMax = mapX - horzExtent;
		xMin = horzExtent - mapX;
		yMax = mapY - vertExtent;
		yMin = vertExtent - mapY;
	}
	
	void MoveToPlayer() {
		this.transform.position = new Vector3(player.position.x,player.position.y,this.transform.position.z);
	}
	
	void Update() {
		if (Input.GetKeyDown(KeyCode.S)) {
			BeginCameraShake(5.0f);
		}
		
		if (audioSource.isPlaying == false) {
			audioSource.Play ();
		}
	}
	
	void FixedUpdate () {
		MoveCamera();
		
		if (shouldShakeCamera) {
			ShakeCamera();
		}
	}
	
	void ShakeCamera() {
		float delay = 2.0f;
		if (Time.time - cameraShakeStartTime >= cameraShakeDuration) {
			shouldShakeCamera = false;
			return;
		}
		else if (Time.time - cameraShakeStartTime >= delay) {
			
			float randX = (UnityEngine.Random.value - 0.5f)/2;
			float randY = (UnityEngine.Random.value - 0.5f)/2;
			this.transform.position = new Vector3(transform.position.x + randX, transform.position.y + randY, transform.position.z);
		}
	}
	
	public void BeginCameraShake(float duration) {
		shouldShakeCamera = true;
		cameraShakeStartTime = Time.time;
		cameraShakeDuration = duration;
		this.audioSource.PlayOneShot(this.shakeSound);
		//		TorchPlacer torchPlacer = this.player.gameObject.GetComponent<TorchPlacer>();
		//		torchPlacer.DimAllTorches(duration);
	}
	
	//	void SetViewOffset() {
	//		if (playerController.facingRight) {
	//			if (this.viewOffset < 0) {
	//				this.viewOffset = -viewOffset;
	//			}
	//		}
	//		else {
	//			if (this.viewOffset > 0) {
	//				this.viewOffset = -viewOffset;
	//			}
	//		}
	//	}
	
	void SetPanDirection() {
		
		if ((player.position.x + viewOffset-this.transform.position.x) > panInterval && playerController.facingRight) {
			shouldPanRight = true;
			shouldPanLeft = false;
		}
		else if ((player.position.x - viewOffset - this.transform.position.x) < panInterval && !playerController.facingRight) {
			shouldPanLeft = true;
			shouldPanRight = false;
		}
		else {
			shouldPanRight = false;
			shouldPanLeft = false;
		}
	}
	
	void Pan() {
		if (shouldPanRight) {
			Vector3 pos = this.transform.position;
			pos.x += this.panInterval;
			this.transform.position = pos;
		}
		else if (shouldPanLeft) {
			Vector3 pos = this.transform.position;
			pos.x -= this.panInterval;
			this.transform.position = pos;
		}
	}
	
	void MoveCamera() {
		SetPanDirection();
		Pan ();
		
		float newX = this.transform.position.x;
		float newY = player.position.y;
		
		newX = Mathf.Clamp(newX,xMin,xMax);
		newY = Mathf.Clamp(newY,yMin,yMax);
		
		this.transform.position = new Vector3(newX,newY,this.transform.position.z);
		
		//		Debug.Log ("ShouldPanRight: " + shouldPanRight.ToString() + "\nShouldPanLeft: " + shouldPanLeft.ToString());
		
	}
	
	
}
