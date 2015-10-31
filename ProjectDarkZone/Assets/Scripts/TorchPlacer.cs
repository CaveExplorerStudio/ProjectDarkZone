using UnityEngine;
using System.Collections;


public class TorchPlacer : MonoBehaviour
{
	public GameObject torch;
	public GameObject torches;
	public bool isHoldingTorch = false;
	MapGenerator mapGenScript;
	/*
	GameObject Generator;
	MapGenerator mapGen;
	*/
	void Start ()
	{
		if (this.torches == null) {
			this.torches = GameObject.Find("Torches");
		}
		
		mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
	}
	
	void ThrowTorch() {
		float Xoffset = 0.6f;
		float YOffset = 0.1f;
		if(GetComponent<PlayerController>().facingRight == false){
			Xoffset = -Xoffset;
		}
		Vector3 torchPosition = new Vector3(transform.position.x + Xoffset, transform.position.y + YOffset, -1.0f);
		
		GameObject newTorch = Instantiate(this.torch, torchPosition, Quaternion.identity) as GameObject;
		
		newTorch.AddComponent<Rigidbody2D>();
		newTorch.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		newTorch.transform.parent = this.torches.transform; //Set as child parent object	
		
		Vector2 throwVelocity = new Vector2(4.0f,5.0f);
		if(GetComponent<PlayerController>().facingRight == false){
			throwVelocity.x *= -1;
		}
		
		Rigidbody2D rigidBody = newTorch.GetComponent<Rigidbody2D>();
		rigidBody.velocity = throwVelocity;
		rigidBody.angularVelocity = 200.0f;
		
	}
	
	void PlaceTorchOnWall() {
		float Xoffset = 0.6f;
		float YOffset = 0.1f;
		if(GetComponent<PlayerController>().facingRight == false){
			Xoffset = -Xoffset;
		}
		Vector3 torchPosition = new Vector3(transform.position.x + Xoffset, transform.position.y + YOffset, -1.0f);
		
		if(mapGenScript.IsInWall((Vector2)torchPosition) == false) //torch is not in wall
		{
			GameObject newTorch = Instantiate(this.torch, torchPosition, Quaternion.identity) as GameObject;
			Destroy(newTorch.GetComponent<PolygonCollider2D>());
			
			if(GetComponent<PlayerController>().facingRight == false) {
				Vector3 newScale = new Vector3(newTorch.transform.localScale.x * -1, newTorch.transform.localScale.y, newTorch.transform.localScale.z);
				newTorch.transform.localScale = newScale;
			}
			
			newTorch.transform.parent = this.torches.transform; //Set as child parent object	
		}
		else {
			Coord wallTile = mapGenScript.GetTileFromScenePosition((Vector2)torchPosition);
			Vector2 wallScenePos = mapGenScript.GetTilePositionInScene(wallTile);
			Vector3 newTorchPosition = new Vector3(wallScenePos.x - Mathf.Sign(Xoffset)*mapGenScript.tileSize/1.5f,wallScenePos.y, -1.0f);
			
			GameObject newTorch = Instantiate(this.torch, newTorchPosition, Quaternion.identity) as GameObject;
			Destroy(newTorch.GetComponent<PolygonCollider2D>());
			
			if(GetComponent<PlayerController>().facingRight) {
				Vector3 newScale = new Vector3(newTorch.transform.localScale.x * -1, newTorch.transform.localScale.y, newTorch.transform.localScale.z);
				newTorch.transform.localScale = newScale;
			}
			
			newTorch.transform.parent = this.torches.transform; //Set as child parent object	
		}
	}
	
	void HoldTorch() {
		isHoldingTorch = true;
		
		float Xoffset = 0.6f;
		float YOffset = 0.1f;
		if(GetComponent<PlayerController>().facingRight == false){
			Xoffset = -Xoffset;
		}
		Vector3 torchPosition = new Vector3(transform.position.x + Xoffset, transform.position.y + YOffset, -1.0f);
		
		GameObject newTorch = Instantiate(this.torch, torchPosition, Quaternion.identity) as GameObject;
		Destroy(newTorch.GetComponent<PolygonCollider2D>());
		newTorch.name = "Player Torch";
		
		if(GetComponent<PlayerController>().facingRight == false) {
			Vector3 newScale = new Vector3(newTorch.transform.localScale.x * -1, newTorch.transform.localScale.y, newTorch.transform.localScale.z);
			newTorch.transform.localScale = newScale;
		}
		
		newTorch.transform.parent = this.torches.transform; //Set as child parent object
		
		TorchController torchController = newTorch.GetComponent<TorchController>();
		torchController.shouldFollowPlayer = true;
	}
	
	public void DimAllTorches(float dimPercentage, float duration) {
		foreach(Transform torch in this.torches.transform) {
			TorchController torchController = torch.gameObject.GetComponent<TorchController>();
			torchController.Dim (dimPercentage, duration);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.T)) {
			PlaceTorchOnWall();
		}
		if (Input.GetKeyDown(KeyCode.F) && isHoldingTorch == false) {
			HoldTorch();
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			ThrowTorch();
		}
		
	}
}

