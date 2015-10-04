using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class SampleHealthController : MonoBehaviour {
    public GameObject HeartHolder;
    public GameObject HeartImage;
    public Slider SanityBar;
    private Image[] hearts;
    private int heartIndex;

    // Use this for initialization
    void Start()
    {
        hearts = HeartHolder.GetComponentsInChildren<Image>();
        heartIndex = hearts.Length - 1;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (heartIndex >= 0)
                heartIndex--;
            updateHealth();
        }

        else if (Input.GetKeyDown(KeyCode.U))
        {
            if (heartIndex < hearts.Length - 1)
                heartIndex++;
            updateHealth();
        }

        else if (Input.GetKeyDown(KeyCode.H))
        {
            addHeart();
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (SanityBar.value >= 10)
                SanityBar.value -= 10;
            else
                SanityBar.value = 0;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (SanityBar.value <= 90)
                SanityBar.value += 10;
            else
                SanityBar.value = 100;
        }
	}

    private void addHealth()
    {
        hearts[heartIndex].sprite = Resources.Load<Sprite>("sampleFull");
    }

    private void removeHealth()
    {
        hearts[heartIndex].sprite = Resources.Load<Sprite>("sampleEmpty");
    }

    private void updateHealth()
    {
        for (int i = 0; i <= heartIndex; i++)
        {
            hearts[i].sprite = Resources.Load<Sprite>("sampleFull");
        }

        for (int i = heartIndex + 1; i < hearts.Length; i++)
        {
            hearts[i].sprite = Resources.Load<Sprite>("sampleEmpty");
        }
    }

    private void addHeart()
    {
        if (hearts.Length < 10)
        {
            GameObject newHeart = Instantiate(HeartImage);
            newHeart.transform.SetParent(HeartHolder.transform, false);
            hearts = HeartHolder.GetComponentsInChildren<Image>();
            updateHealth();
        }
    }
}
