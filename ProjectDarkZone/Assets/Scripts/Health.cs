using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour {

    public Component healthContainer;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    private const int initHearts = 4;
    private const int heartValue = 2;

    private Image[] hearts;
    private int health, maxHearts, maxHealth;
    
	void Start () {
        hearts = healthContainer.GetComponentsInChildren<Image>();
        maxHearts = hearts.Length;
        maxHealth = heartValue * initHearts;
        health = maxHealth;

        for(int i = initHearts; i < maxHearts; i++)
        {
            hearts[i].enabled = false;
        }
	}

    void Update()
    {
        if(health == 0)
        {
            //GAME OVER
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
