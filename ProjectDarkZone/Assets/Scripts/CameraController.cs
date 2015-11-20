using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	
	// The camera will automatically follow the player, but stop when the view touches the edge of the cave.
	// Automatically adjusts to any screen size.
	
	Transform player;
	Rigidbody2D playerRigidBody;
	PlayerController playerController;
	
	public MapGenerator mapGenScript;
	
	Vector2 maxPosition;
	
	float xMax;
	float xMin;
	float yMax;
	float yMin;
	
	public float viewOffset = 5.0f;
	private float panInterval;
	private float maxPanInterval = 0.25f;
	private float minPanInterval = 0.03f;
	
	bool shouldPanRight;
	bool shouldPanLeft;
	bool shouldShakeCamera = false;
	
	float cameraShakeStartTime;
	float cameraShakeDuration;
	float cameraShakeIntensity;
	
	
	private GameObject audioDelegate;
	private AudioController audioController;
	
	
	void Start () {
		
		//		this.audioSource = this.GetComponent<AudioSource>();
		
		if (player == null) {
			player = GameObject.Find ("Player").transform;
		}
		
		this.playerRigidBody = player.GetComponent<Rigidbody2D>();
		this.playerController = player.GetComponent<PlayerController>();
		this.audioDelegate = GameObject.Find ("Audio Delegate");
		this.audioController = audioDelegate.GetComponent<AudioController>();
		
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
		if (Input.GetKeyDown(KeyCode.P)) {
			Debug.Log("Beginning Camera Shake");
			BeginCameraShake(0.50f,5.0f);
		}
		
	}
	
	void FixedUpdate () {
		//		MoveCamera();
		//		
		//		if (shouldShakeCamera) {
		//			ShakeCamera();
		//		}
	}
	
	void LateUpdate() {
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
			
			float randX = (UnityEngine.Random.value*cameraShakeIntensity*2)-cameraShakeIntensity;
			float randY = (UnityEngine.Random.value*cameraShakeIntensity*2)-cameraShakeIntensity;
			this.transform.position = new Vector3(transform.position.x + randX, transform.position.y + randY, transform.position.z);
		}
	}
	
	public void BeginCameraShake(float intensity, float duration) {
		shouldShakeCamera = true;
		cameraShakeStartTime = Time.time;
		cameraShakeDuration = duration;
		cameraShakeIntensity = intensity;
		this.audioController.PlayShakeSound();
	}
	
	void SetPanDirection() {
		
		if ((player.position.x + viewOffset-this.transform.position.x) > 0 && playerController.facingRight) {
			shouldPanRight = true;
			shouldPanLeft = false;
			float temp = (player.position.x + viewOffset-this.transform.position.x);
			this.panInterval = Mathf.Min(1.0f,temp/this.panInterval)*panInterval;
		}
		else if ((player.position.x - viewOffset - this.transform.position.x) < 0 && !playerController.facingRight) {
			shouldPanLeft = true;
			shouldPanRight = false;
			float temp = (player.position.x - viewOffset - this.transform.position.x);
			this.panInterval = Mathf.Min(1.0f,-temp/this.panInterval)*panInterval;
		}
		else {
			shouldPanRight = false;
			shouldPanLeft = false;
		}

//		if ((player.position.x + viewOffset-this.transform.position.x) > 0.01f && playerController.facingRight) {
//			shouldPanRight = true;
//			shouldPanLeft = false;
//		}
//		else if ((player.position.x - viewOffset - this.transform.position.x) < -0.01f && !playerController.facingRight) {
//			shouldPanLeft = true;
//			shouldPanRight = false;
//		}
//		else {
//			shouldPanRight = false;
//			shouldPanLeft = false;
//		}
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
	
	void SetPanInterval() {
		
		if (Mathf.Abs (this.playerRigidBody.velocity.x) > 1.0f) {
			this.panInterval = maxPanInterval;
		}
		else {
			this.panInterval = minPanInterval;
		}
	}
	
	void MoveCamera() {
		SetPanInterval();
		SetPanDirection();
		Pan ();
		
		float newX = this.transform.position.x;
		float newY = player.position.y;
		
		newX = Mathf.Clamp(newX,xMin,xMax);
		newY = Mathf.Clamp(newY,yMin,yMax);
		
		this.transform.position = new Vector3(newX,newY,this.transform.position.z);
		
	}
	
	public bool IsInView(Vector2 position) {
		float sightY = Camera.main.orthographicSize;    
		float sightX = (sightY * Screen.width / Screen.height);
		float xDist = Mathf.Abs(this.transform.position.x - position.x);
		float yDist = Mathf.Abs (this.transform.position.y - position.y);
		if (xDist <= sightX && yDist <= sightY) {
			return true;
		}
		else {
			return false;
		}
	}
	
	
}
