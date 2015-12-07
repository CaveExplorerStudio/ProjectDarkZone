using UnityEngine;
using System.Collections;

/*
Currently unused be ready item
*/

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
        //Standard game object loading
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


        Vector2 throwVelocity = new Vector2(4.0f, 5.0f); //Controls initial throw speed and direction
        if(playerControl.facingRight == false)
        {
            throwVelocity.x *= -1; //Will reverse the X depending on how you are facing. 
        }

        pickBody.velocity = throwVelocity;
        pickBody.angularVelocity = 200.0f; //Gives rotation to the throw

        currentPickaxe = newPick;
        currentRigidBody = pickBody;

        leftSpike = GameObject.Find("LeftSpike"); //Used for collisions in the CheckCollision() methods, represents the tips of the axe. 
        rightSpike = GameObject.Find("RightSpike");

        leftSpike.name = "LeftSpike" + numOfAxes;
        rightSpike.name = "RightSpike" + numOfAxes;

        numOfAxes++;
    }

    public void checkCollision()
    {
        if (!(Physics2D.OverlapCircle(new Vector2(rightSpike.transform.position.x, rightSpike.transform.position.y), .1f, ground_layers) == null) || !(Physics2D.OverlapCircle(new Vector2(leftSpike.transform.position.x, leftSpike.transform.position.y), .1f, ground_layers) == null))
        {
            currentRigidBody.isKinematic = true;
        }
    }

    
	
}
