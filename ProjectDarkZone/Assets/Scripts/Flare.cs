using UnityEngine;
using System.Collections;

public class Flare : IItem {
    private GameObject playerPosition;
    private GameObject player;
    private GameObject pickPrefab;
    private PlayerController playerControl;
    private GameObject currentFlare;
    private Rigidbody2D currentRigidBody;
    private LayerMask ground_layers;

    private static int numOfFlares = 0;

    public GameObject pickaxe;


    public string Name { get; set; }
    public Sprite Image { get; set; }
    public bool IsConsumable { get; set; }
    public GameObject Prefab { get; set; }

    public void Use()
    {
        playerPosition = GameObject.Find("FirePoint");
        player = GameObject.Find("Player");
        playerControl = player.GetComponent<PlayerController>();
        pickPrefab = Resources.Load("Axe", typeof(GameObject)) as GameObject;
        ground_layers = 1 << LayerMask.NameToLayer("Cave");

        Vector3 pickPosition = new Vector3(playerPosition.transform.position.x, playerPosition.transform.position.y, -1.0f);

        GameObject newFlare = MonoBehaviour.Instantiate(pickPrefab, pickPosition, Quaternion.identity) as GameObject;

        Rigidbody2D flareBody = newFlare.AddComponent<Rigidbody2D>();
        flareBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;


        Vector2 throwVelocity = new Vector2(4.0f, 5.0f);
        if (playerControl.facingRight == false)
        {
            throwVelocity.x *= -1;
        }

        flareBody.velocity = throwVelocity;
        flareBody.angularVelocity = 200.0f;

        currentFlare = newFlare;
        currentRigidBody = flareBody;

        numOfFlares++;
    }
}
