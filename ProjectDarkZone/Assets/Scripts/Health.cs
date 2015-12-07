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
    
    private static int maxHealth = heartValue * initHearts;
    private static int health = maxHealth;

    private Image[] hearts;
    private Sanity sanity;
    private int maxHearts;
    private float sanityClock;

	private Animator playerAnimator;

    void Start ()
    {
        hearts = healthContainer.GetComponentsInChildren<Image>();
        sanity = GetComponent<Sanity>();
        maxHearts = hearts.Length;
        sanityClock = sanityTickTime;
		playerAnimator = GameObject.Find("Player").GetComponent<Animator>();

        for (int i = maxHealth / heartValue; i < maxHearts; i++)
        {
            hearts[i].enabled = false;
        }
        UpdateHearts();
	}

    void Update()
    {
        if (sanity.IsEmpty())
        {
            sanityClock -= Time.deltaTime;
            if (sanityClock <= 0)
            {
                AddHealth(-1);
                ResetSanityClock();
            }
        }
    }

    public void AddHealth(int amount)
    {

        health += amount;
		if (amount < 0) {
			playerAnimator.SetTrigger("Hurt");
		}

        if (health > maxHealth)
            health = maxHealth;
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

    public void ResetSanityClock()
    {
        sanityClock = sanityTickTime;
    }

    public int GetHealth()
    {
        return health;
    }

    public static void ResetAllValues()
    {
        maxHealth = heartValue * initHearts;
        health = maxHealth;
    }
}
