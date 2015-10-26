using UnityEngine;
using System.Collections;


public class TorchPlacer : MonoBehaviour
{
	public Sprite torchSprite;
	public GameObject torches;
	/*
	GameObject Generator;
	MapGenerator mapGen;
	*/
	void Start ()
	{

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

			int directionMult = 1;
			if(GetComponent<PlayerController>().facingRight)
				directionMult = 1;
			else
				directionMult = -1;

			Vector3 torchPosition = new Vector3(transform.position.x + 1*directionMult, transform.position.y + 0.40f, transform.position.z - 1);
			if(true) //torch is not in wall
			{
				GameObject torch = new GameObject("Torch");
				torch.transform.parent = torches.transform;
				SpriteRenderer torchSpriteRenderer = torch.AddComponent<SpriteRenderer>();
				torchSpriteRenderer.sprite = torchSprite;
				torch.transform.position = torchPosition;
				torch.transform.rotation = transform.rotation;
				
				torch.transform.localScale = new Vector3(10*directionMult,10,10);


				torch.AddComponent<Light>();	
				Light light = torch.GetComponent<Light>();
				light.intensity = 8;
				light.renderMode = LightRenderMode.ForcePixel;
			}
		}
	}
}

