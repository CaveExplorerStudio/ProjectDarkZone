using UnityEngine;
using System.Collections;


public class TorchPlacer : MonoBehaviour
{
	public GameObject torch;
	public GameObject torches;
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

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.T)) {
			/*
			Generator = GameObject.Find ("Map Generator");
			mapGen = Generator.GetComponent<MapGenerator>();
			Vector3 playerPos = new Vector3(transform.position.x,transform.position.y,0.0f);
			MapGenerator.Coord playerCoord = mapGen.Vector3ToCoord(playerPos);
			MapGenerator.Coord closestTile = mapGen.LeftTileClosestToPlayer(playerCoord);
			Debug.Log("Closest Tile: " + closestTile.tileX.ToString() + closestTile.tileY.ToString ());
			Debug.Log("Player Tile: " + playerCoord.tileX.ToString() + playerCoord.tileY.ToString ());
			*/

			float Xoffset = 0.6f;
			float YOffset = 0.1f;
			if(GetComponent<PlayerController>().facingRight == false){
				Xoffset = -Xoffset;
			}
			Vector3 torchPosition = new Vector3(transform.position.x + Xoffset, transform.position.y + YOffset, -1.0f);

			if(mapGenScript.IsInWall((Vector2)torchPosition) == false) //torch is not in wall
			{
				GameObject newTorch = Instantiate(this.torch, torchPosition, Quaternion.identity) as GameObject;

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

				if(GetComponent<PlayerController>().facingRight) {
					Vector3 newScale = new Vector3(newTorch.transform.localScale.x * -1, newTorch.transform.localScale.y, newTorch.transform.localScale.z);
					newTorch.transform.localScale = newScale;
				}

				newTorch.transform.parent = this.torches.transform; //Set as child parent object	
			}
		}
	}
}

