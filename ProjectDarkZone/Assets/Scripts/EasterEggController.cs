using UnityEngine;
using System.Collections;

public class EasterEggController : MonoBehaviour {

    public Material CaveBackground;
    private const string easterEgg = "modi";
    private string enteredCharacters = string.Empty;
    private Texture originalCaveTexture;

    void Start()
    {
        originalCaveTexture = CaveBackground.mainTexture;
    }

	void Update()
    {
        foreach (var c in Input.inputString)
        {
            enteredCharacters += c;
            if (enteredCharacters.Contains(easterEgg))
            {
                CaveBackground.mainTexture = Resources.Load<Texture>("Modi");   
            }
        }
    }


    void OnApplicationQuit()
    {
        CaveBackground.mainTexture = originalCaveTexture;
    }

}
