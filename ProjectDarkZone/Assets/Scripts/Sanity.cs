using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Sanity : MonoBehaviour {

    public Component sanityBar;
    public int sanityLifetime;
    public Color targetColor;

    private const float error = 1f / 255;

    private Slider sanity;
    private Color origColor;
    private float value, degenPerSec, fadeTime;
    private bool faded;
    private Image foreground, background;

    void Start()
    {
        sanity = sanityBar.GetComponent<Slider>();
        background = sanityBar.GetComponentsInChildren<Image>()[0];
        origColor = background.color;
        foreground = sanityBar.GetComponentsInChildren<Image>()[1];
        degenPerSec = sanity.maxValue / sanityLifetime;
        fadeTime = GetComponent<Health>().sanityTickTime;
        faded = false;
    }
	
	void Update ()
    {
        value = sanity.value;

        if (sanity.value > sanity.minValue)
            sanity.value -= Time.deltaTime * degenPerSec;
        else if (sanity.value <= sanity.minValue && !faded)
        {
            foreground.enabled = false;
            background.CrossFadeColor(targetColor, fadeTime, true, false);
            faded = true;
        }
	}

    public bool IsEmpty()
    {
        return sanity.value <= sanity.minValue;
    }

    public void SliderPulseOn()
    {
        background.CrossFadeColor(origColor, fadeTime / 2, true, false);
    }

    public void SliderPulseOff()
    {
        background.CrossFadeColor(targetColor, fadeTime / 2, true, false);
    }
}
