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
		GameObject newMarker = Instantiate(this.marker, mapGenScript.GetTilePositionInScene(newMarkerTile), Quaternion.identity) as GameObject;
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
		newMarkerTile = FindTile(); //if crouched, look at floor tiles
		tempGhostMarker = Instantiate(this.ghostMarker, mapGenScript.GetTilePositionInScene(newMarkerTile), Quaternion.identity) as GameObject;
		tempGhostMarker.transform.parent = this.markers.transform;
		tempGhostMarker.transform.rotation = this.markers.transform.rotation;
	}

	void HandleGhost(KeyCode direction)
	{
		tempGhostMarker.transform.rotation = this.markers.transform.rotation;
		float markerRotation = 0.0f;
		if(direction == KeyCode.DownArrow)
			markerRotation = 0.0f;
		else if(direction == KeyCode.UpArrow)
			markerRotation = 180.0f;
		else if(direction == KeyCode.LeftArrow)
			markerRotation = 90.0f;
		else if(direction == KeyCode.RightArrow)
			markerRotation = 270.0f;

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
			if(Input.GetKeyDown(KeyCode.UpArrow)) {
				HandleGhost(KeyCode.UpArrow);
			}
			if(Input.GetKeyDown(KeyCode.DownArrow)) {
				HandleGhost(KeyCode.DownArrow);
			}
			if(Input.GetKeyDown(KeyCode.LeftArrow)) {
				HandleGhost(KeyCode.LeftArrow);
			}
			if(Input.GetKeyDown(KeyCode.RightArrow)) {
				HandleGhost(KeyCode.RightArrow);
			}
		}
	}
}
