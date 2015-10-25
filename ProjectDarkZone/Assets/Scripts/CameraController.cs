using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {


	// The camera will automatically follow the player, but stop when the view touches the edge of the cave.
	// Automatically adjusts to any screen size.

	public Transform player;

	public MapGenerator mapGenScript;

	Vector2 maxPosition;

	float xMax;
	float xMin;
	float yMax;
	float yMin;
	
	void Start () {

		if (player == null) {
			player = GameObject.Find ("Player").transform;
		}

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

	void FixedUpdate () {
		MoveCamera();
	}

	void MoveCamera() {
		float newX = player.position.x;
		float newY = player.position.y;

		newX = Mathf.Clamp(newX,xMin,xMax);
		newY = Mathf.Clamp(newY,yMin,yMax);

		this.transform.position = new Vector3(newX,newY,this.transform.position.z);

	}

}
