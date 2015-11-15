using UnityEngine;
using System.Collections;

public class Torch : IItem
{
    public string Name { get; set; }
    public Sprite Image { get; set; }
    public bool IsConsumable { get; set; }
    public GameObject Prefab { get; set; }
    public GameObject Torches { get; set; }
    public GameObject Player { get; set; }
    public MapGenerator MapGenScript { get; set; }


    public Torch (string name, Sprite image, bool isConsumable, GameObject prefab)
    {
        Name = name;
        Image = image;
        IsConsumable = isConsumable;
        Prefab = prefab;
        Torches = GameObject.Find("Torches");
        Player = GameObject.Find("Player");
        MapGenScript = GameObject.Find("Map Generator").GetComponent<MapGenerator>();
    }


    public void Use()
    {
        float Xoffset = 0.6f;
        float YOffset = 0.1f;
        if (Player.GetComponent<PlayerController>().facingRight == false)
        {
            Xoffset = -Xoffset;
        }
        Vector3 torchPosition = new Vector3(Player.transform.position.x + Xoffset, Player.transform.position.y + YOffset, -1.0f);

        if (MapGenScript.IsInWall((Vector2)torchPosition) == false) //torch is not in wall
        {
            GameObject newTorch = Object.Instantiate(Prefab, torchPosition, Quaternion.identity) as GameObject;
            Object.Destroy(newTorch.GetComponent<PolygonCollider2D>());

            if (Player.GetComponent<PlayerController>().facingRight == false)
            {
                Vector3 newScale = new Vector3(newTorch.transform.localScale.x * -1, newTorch.transform.localScale.y, newTorch.transform.localScale.z);
                newTorch.transform.localScale = newScale;
            }

            newTorch.transform.parent = Torches.transform; //Set as child parent object	
        }
        else
        {
            Coord wallTile = MapGenScript.GetTileFromScenePosition((Vector2)torchPosition);
            Vector2 wallScenePos = MapGenScript.GetTilePositionInScene(wallTile);
            Vector3 newTorchPosition = new Vector3(wallScenePos.x - Mathf.Sign(Xoffset) * MapGenScript.tileSize / 1.5f, wallScenePos.y, -1.0f);

            GameObject newTorch = Object.Instantiate(Prefab, newTorchPosition, Quaternion.identity) as GameObject;
            Object.Destroy(newTorch.GetComponent<PolygonCollider2D>());

            if (Player.GetComponent<PlayerController>().facingRight)
            {
                Vector3 newScale = new Vector3(newTorch.transform.localScale.x * -1, newTorch.transform.localScale.y, newTorch.transform.localScale.z);
                newTorch.transform.localScale = newScale;
            }

            newTorch.transform.parent = Torches.transform; //Set as child parent object	
        }
    }
}