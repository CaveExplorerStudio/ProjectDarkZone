using UnityEngine;
using System.Collections;

public class MarkerPlacer : MonoBehaviour {
	public GameObject marker;
	public GameObject markers;
	public GameObject ghostMarker;
	GameObject tempGhostMarker;
	PlayerController playerController;
	Coord newMarkerTile;
	MapGenerator mapGenScript;
	bool inPlacementMode = false;
	
	// Use this for initialization
	void Start () {
		if(this.markers == null) {
			this.markers = GameObject.Find("Markers");
		}
		mapGenScript = GameObject.Find ("Map Generator").GetComponent<MapGenerator>();
		playerController = GetComponent<PlayerController>();
	}
	
	void AddMarker() {
		Vector2 tilePos = mapGenScript.GetTilePositionInScene(newMarkerTile);
		Vector3 markerPosition = new Vector3(tilePos.x,tilePos.y,0.01f);
		GameObject newMarker = Instantiate(this.marker, markerPosition, Quaternion.identity) as GameObject;
		newMarker.transform.parent = this.markers.transform;
		newMarker.transform.rotation = tempGhostMarker.transform.rotation;
	}
	
	Coord FindTile()
	{	
		float Xoffset = 0.6f;
		float YOffset = 0.1f;
		if(playerController.facingRight == false){
			Xoffset = -Xoffset;
		}
		Vector3 handPosition = new Vector3(transform.position.x + Xoffset, transform.position.y + YOffset, -1.0f);
		return mapGenScript.GetClosestWallTile(mapGenScript.GetTileFromScenePosition(handPosition));
	}
	
	
	void ActivatePlacementMode()
	{
		inPlacementMode = true;
		playerController.disableMovement();
		//		newMarkerTile = FindTile(); //if crouched, look at floor tiles
		newMarkerTile = mapGenScript.GetTileFromScenePosition(this.transform.position);
		tempGhostMarker = Instantiate(this.ghostMarker, mapGenScript.GetTilePositionInScene(newMarkerTile), Quaternion.identity) as GameObject;
		tempGhostMarker.transform.parent = this.markers.transform;
		tempGhostMarker.transform.rotation = this.markers.transform.rotation;
	}
	
	void HandleGhost(string direction)
	{
		tempGhostMarker.transform.rotation = this.markers.transform.rotation;
		float markerRotation = 0.0f;
		if(direction == "D")
			markerRotation = 0.0f;
		else if(direction == "U")
			markerRotation = 180.0f;
		else if(direction == "L")
			markerRotation = 90.0f;
		else if(direction == "R")
			markerRotation = 270.0f;
		else if(direction == "UR")
			markerRotation = 225.0f;
		else if(direction == "UL")
			markerRotation = 135.0f;
		else if(direction == "DR")
			markerRotation = 315.0f;
		else if(direction == "DL")
			markerRotation = 45.0f;
		
		
		
		
		tempGhostMarker.transform.Rotate(Vector3.back, markerRotation);
	}
	
	void DeactivatePlacementMode()
	{
		inPlacementMode = false;
		playerController.enableMovement();
		DestroyObject(tempGhostMarker);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Y) && playerController.grounded) {
			if(inPlacementMode == false) {
				ActivatePlacementMode();
			}
			else {
				DeactivatePlacementMode();
			}
		}
		if(inPlacementMode)
		{
			if(Input.GetKeyDown(KeyCode.Return)) {
				AddMarker();
				DeactivatePlacementMode();
			}
			if(Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow)) {
				HandleGhost("UR");
			} 
			else if(Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow)) {
				HandleGhost("UL");
			}
			else if(Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow)) {
				HandleGhost("DR");
			} 
			else if(Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow)) {
				HandleGhost("DL");
			}
			else if(Input.GetKeyDown(KeyCode.UpArrow)) {
				HandleGhost("U");
			}
			else if(Input.GetKeyDown(KeyCode.DownArrow)) {
				HandleGhost("D");
			}
			else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
				HandleGhost("L");
			}
			else if(Input.GetKeyDown(KeyCode.RightArrow)) {
				HandleGhost("R");
			}
		}
	}
}
