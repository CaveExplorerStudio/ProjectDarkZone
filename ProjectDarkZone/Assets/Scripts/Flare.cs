using UnityEngine;
using System.Collections;

public class Flare : IItem {
    private GameObject playerPosition;
    private GameObject player;
    private PlayerController playerControl;
    private GameObject currentFlare;
    private Rigidbody2D currentRigidBody;
    private LayerMask ground_layers;
    private Light flareLight;
    private int burnTime = 1000;
    private bool burning = true;

    private static int numOfFlares = 0;

    public GameObject pickaxe;


    public string Name { get; set; }
    public Sprite Image { get; set; }
    public bool IsConsumable { get; set; }
    public GameObject Prefab { get; set; }

    public Flare(string name, Sprite image, bool isConsumable, GameObject prefab)
    {
        Name = name;
        Image = image;
        IsConsumable = isConsumable;
        Prefab = prefab;
        playerPosition = GameObject.Find("FirePoint");
        player = GameObject.Find("Player");
        playerControl = player.GetComponent<PlayerController>();
        ground_layers = 1 << LayerMask.NameToLayer("Cave");
    }

    public void Use()
    {
        Vector3 pickPosition = new Vector3(playerPosition.transform.position.x, playerPosition.transform.position.y, -1.0f);

        GameObject newFlare = MonoBehaviour.Instantiate(Prefab, pickPosition, Quaternion.identity) as GameObject;

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

        flareLight = currentFlare.GetComponentInChildren<Light>();

        numOfFlares++;
    }

    public Light getLightComp()
    {
        if (flareLight != null)
            return flareLight;
        else
            return null;
    }

    public bool isLit()
    {
        return burning;
    }

    public void Burn()
    {
        burnTime--;

        if (burnTime <= 0)
        {
            burning = false;
            if (flareLight.intensity > 0)
            {
                flareLight.intensity -= .1f;
            }
            else
                MonoBehaviour.Destroy(flareLight);
        }
    }

    public void SetLightIntensity(int i)
    {
        flareLight.intensity = i;
    }
}
