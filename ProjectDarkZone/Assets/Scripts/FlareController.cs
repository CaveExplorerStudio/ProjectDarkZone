using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlareController : MonoBehaviour {
    //References

    List<Flare> flares;


    // Use this for initialization
    void Start()
    {
        flares = new List<Flare>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Flare flare = new Flare();
            flare.Use();
            flares.Add(flare);
        }

    }
}
