using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

    public float coolDown = 0f;
    public float fireRate = 0f;

    public bool isFiring = false;

    public Transform firePoint; //Where the objects will shoot from

    //Projectile Object
    public GameObject ropePrefab;

	// Use this for initialization
	void Start () {
        isFiring = false;
	}
	
	// Update is called once per frame
	void Update () {
        CheckInput();

        if(isFiring == true)
        {
            Fire();
        }
	}

    void CheckInput()
    {
        if (Input.GetKeyDown("space"))
        {
            isFiring = true;
        }
        else
        {
            isFiring = false;
        }

    }

    void Fire()
    {
        GameObject.Instantiate(ropePrefab, firePoint.position, firePoint.rotation);


    }
}
