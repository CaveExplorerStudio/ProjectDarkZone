using UnityEngine;
using System.Collections;

public class Pickaxe{
    //References
    private GameObject playerPosition;
    private GameObject player;
    private GameObject pickPrefab;
    private PlayerController playerControl;
    private GameObject currentPickaxe;
    private Rigidbody2D currentRigidBody;
    private GameObject leftSpike;
    private GameObject rightSpike;
    private LayerMask ground_layers;

    private static int numOfAxes = 0;

    public GameObject pickaxe;
    
    public Pickaxe()
    {
        playerPosition = GameObject.Find("FirePoint");
        player = GameObject.Find("Player");
        playerControl = player.GetComponent<PlayerController>();
        pickPrefab = Resources.Load("Axe", typeof(GameObject)) as GameObject;
        ground_layers = 1 << LayerMask.NameToLayer("Cave");
    }

    public void ThrowPickaxe()
    {
        Vector3 pickPosition = new Vector3(playerPosition.transform.position.x, playerPosition.transform.position.y, -1.0f);

        GameObject newPick = MonoBehaviour.Instantiate(pickPrefab, pickPosition, Quaternion.identity) as GameObject;

        Rigidbody2D pickBody = newPick.AddComponent<Rigidbody2D>();
        pickBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;


        Vector2 throwVelocity = new Vector2(4.0f, 5.0f);
        if(playerControl.facingRight == false)
        {
            throwVelocity.x *= -1;
        }

        pickBody.velocity = throwVelocity;
        pickBody.angularVelocity = 200.0f;

        currentPickaxe = newPick;
        currentRigidBody = pickBody;

        leftSpike = GameObject.Find("LeftSpike");
        rightSpike = GameObject.Find("RightSpike");

        leftSpike.name = "LeftSpike" + numOfAxes;
        rightSpike.name = "RightSpike" + numOfAxes;

        numOfAxes++;
    }

    public void checkCollision()
    {
        if (!(Physics2D.OverlapCircle(new Vector2(leftSpike.transform.position.x, rightSpike.transform.position.y), .2f, ground_layers) == null))
        {
            currentRigidBody.isKinematic = true;
        }
    }

    
	
}
