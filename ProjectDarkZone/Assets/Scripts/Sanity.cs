using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Sanity : MonoBehaviour
{
    public Component sanityBar;
    public int sanityLifetime;
    public Color normalColor, dangerColor;

    private static bool deplete = false;
    private static float value = 100f;

    private Slider sanity;
    private float degenPerSec, fadeTime;
    private bool faded;
    private Image foreground, background;
    private Health health;

    void Start()
    {
        sanity = sanityBar.GetComponent<Slider>();
        background = sanityBar.GetComponentsInChildren<Image>()[0];
        foreground = sanityBar.GetComponentsInChildren<Image>()[1];
        degenPerSec = sanity.maxValue / sanityLifetime;
        health = GetComponent<Health>();
        fadeTime = health.sanityTickTime;
        faded = false;
        sanity.value = value;
    }

    void Update()
    {
        value = sanity.value;

        if (deplete)
        {
            if (sanity.value > sanity.minValue)
                sanity.value -= Time.deltaTime * degenPerSec;
            else if (sanity.value <= sanity.minValue && !faded)
            {
                foreground.enabled = false;
                background.CrossFadeColor(dangerColor, fadeTime, true, false);
                faded = true;
            }
        }
        else
        {
            if (sanity.value == 0 && faded)
            {
                foreground.enabled = false;
                background.CrossFadeColor(normalColor, fadeTime, true, false);
                faded = false;
            }
            if (sanity.value < sanity.maxValue)
                sanity.value += Time.deltaTime * degenPerSec;
        }
    }

    public bool IsEmpty()
    {
        return sanity.value <= sanity.minValue;
    }

    public static void SetDepleteSanity(bool deplete)
    {
        Sanity.deplete = deplete;
    }

    public void restoreSanityBy(float percent)
    {
        if (IsEmpty())
        {
            health.ResetSanityClock();
        }

        sanity.value += sanity.maxValue * percent;
    }
}
