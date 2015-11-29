using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlareController : MonoBehaviour {
    //References
    float strobe = 6;
    float increment = .05f;
    public List<Flare> flares;


    // Use this for initialization
    void Start()
    {
        flares = new List<Flare>();
    }

    // Update is called once per frame
    void Update()
    {
        if (strobe > 8)
            increment *= -1;
        else if (strobe < 4)
            increment *= -1;

        strobe += increment;

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    Flare flare = new Flare();
        //    flare.Use();
        //    flares.Add(flare);
        //}

        foreach (Flare f in flares)
        {
            if (f.getLightComp() != null)
            {
                f.Burn();

                if (f.isLit())
                {
                    f.getLightComp().intensity = strobe;
                }
            }
        }
    }
}
