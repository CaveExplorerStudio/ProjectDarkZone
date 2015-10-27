using UnityEngine;
using System.Collections;

public class BatSwarm : MonoBehaviour {
	
	
	private MapGenerator mapGenScript;
	
	private GameObject[] bats;
	
	private int batAmount = 15;
	private float spawnRadius = 1.0f;
	private float swarmDuration = 10.0f;
	private float dispersionDuration = 2.5f;
	
	AudioSource audioSource;
	
	void Start () {
		
		mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		CreateBats();
		audioSource = this.GetComponent<AudioSource>();
		audioSource.Play();
	}
	
	void CreateBats() {
		this.bats = new GameObject[this.batAmount];
		
		for (int i = 0; i < this.batAmount;i++) {
			
			Vector2 randOffset = (UnityEngine.Random.insideUnitCircle)*spawnRadius;
			Vector2 randPosition = (Vector2)this.transform.position + randOffset;
			
			GameObject bat = Instantiate(mapGenScript.bat, randPosition, Quaternion.identity) as GameObject;
			bat.transform.parent = mapGenScript.bats.transform; //Set as child parent object;
			this.bats[i] = bat;
			
			BatController batController = bat.GetComponent<BatController>();
			batController.StartSwarm(randPosition,swarmDuration,true,dispersionDuration);
		}
	}
	
	
	void Update () {
		if (audioSource.isPlaying == false) {
			Destroy(this.gameObject);
		}
	}
}
