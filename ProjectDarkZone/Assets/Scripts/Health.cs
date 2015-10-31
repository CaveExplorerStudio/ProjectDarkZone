using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour {

    public Component healthContainer;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;
    public float sanityTickTime;

    private const int initHearts = 4;
    private const int heartValue = 2;

    private Image[] hearts;
    private Sanity sanity;
    private int health, maxHearts, maxHealth;
    private float sanityClock;
    private bool pulseOn, pulseOff;
    
	void Start () {
        hearts = healthContainer.GetComponentsInChildren<Image>();
        sanity = GetComponent<Sanity>();
        maxHearts = hearts.Length;
        maxHealth = heartValue * initHearts;
        health = maxHealth;
        sanityClock = sanityTickTime;
        pulseOn = false;
        pulseOff = false;

        for (int i = initHearts; i < maxHearts; i++)
        {
            hearts[i].enabled = false;
        }
	}

    void Update()
    {
        if (health == 0)
        {
            //GAME OVER
        }

        if (sanity.IsEmpty())
        {
            sanityClock -= Time.deltaTime;
            if(sanityClock <= 0)
            {
                AddHealth(-1);
                sanityClock = sanityTickTime;
                if(!pulseOn)
                {
                    sanity.SliderPulseOn();
                    pulseOn = true;
                    pulseOff = false;
                }
            }
            else if(sanityClock <= sanityTickTime / 2 && !pulseOff)
                sanity.SliderPulseOn();
                pulseOff = true;
                pulseOn = true;
            {

            }
        }
    }

    public void AddHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHearts * heartValue;
        else if (health < 0)
            health = 0;
        UpdateHearts();
    }

    public void AddHeart()
    {
        if(maxHealth < maxHearts * heartValue)
        {
            maxHealth += heartValue;
            health += heartValue;
            hearts[maxHealth / 2 - 1].enabled = true;
        }
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        for(int i = 0; i < maxHealth / heartValue; i++)
        {
            if (health > i * heartValue + heartValue / 2)
                hearts[i].sprite = fullHeart;
            else if (health > i * heartValue)
                hearts[i].sprite = halfHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
