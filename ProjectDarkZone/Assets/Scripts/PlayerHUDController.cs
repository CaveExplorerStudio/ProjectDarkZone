using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerHUDController : MonoBehaviour {

    public Image heldGem;
    public Image collection;
    public Sprite gem1, gem2, gem3, gem4, gem5, gem6, gem7, gem8, crown;

    [HideInInspector]
    public static char[] nums = new char[]{ '1', '2', '3', '4', '5', '6', '7', '8' };
    
    private Dictionary<int, Sprite> gems;
    private Dictionary<int, Image> collected;
    private Image newGem;

    void Start()
    {
        gems = new Dictionary<int, Sprite>(){
            {1, gem1},
            {2, gem2},
            {3, gem3},
            {4, gem4},
            {5, gem5},
            {6, gem6},
            {7, gem7},
            {8, gem8}
        };

        if(collection != null)
        {
            newGem = collection.GetComponentsInChildren<Image>()[9];
            newGem.enabled = false;

            collection.enabled = false;
            collected = new Dictionary<int, Image>(){
                {1, collection.GetComponentsInChildren<Image>()[1]},
                {2, collection.GetComponentsInChildren<Image>()[2]},
                {3, collection.GetComponentsInChildren<Image>()[3]},
                {4, collection.GetComponentsInChildren<Image>()[4]},
                {5, collection.GetComponentsInChildren<Image>()[5]},
                {6, collection.GetComponentsInChildren<Image>()[6]},
                {7, collection.GetComponentsInChildren<Image>()[7]},
                {8, collection.GetComponentsInChildren<Image>()[8]}
            };
            foreach (Image image in collected.Values)
            {
                image.enabled = false;
            }
        }
    }

    public void UpdateGemIcon(string gem)
    {
        if(gem == null)
        {
            heldGem.enabled = false;
        }
        else
        {
            Sprite sprite;
            heldGem.enabled = true;
            gems.TryGetValue(GetNum(gem), out sprite);
            heldGem.sprite = sprite;
        }
    }

    public void OpenCollection(string[] gems)
    {
        collection.enabled = true;
        foreach (Image image in collected.Values)
        {
            image.enabled = true;
        }
        foreach (string gem in gems) {
            if (gem != null)
            {
                int num = GetNum(gem);
                Image image;
                collected.TryGetValue(num, out image);
                Sprite sprite;
                this.gems.TryGetValue(num, out sprite);
                image.sprite = sprite;
            }
        }
    }

    public IEnumerator AddGemToCollection(string gem)
    {
        Image image;
        collected.TryGetValue(GetNum(gem), out image);
        Sprite sprite;
        gems.TryGetValue(GetNum(gem), out sprite);
        Vector3 pos = image.rectTransform.position;
        newGem.rectTransform.position = new Vector3(pos.x, pos.y - 20, pos.z);
        newGem.sprite = sprite;

        newGem.enabled = true;
        for (int i = 0; i < 20; i++)
        {
            NewGemAnim();
            yield return new WaitForSeconds(0.05f);
        }
        newGem.enabled = false;
        image.sprite = sprite;

        if (CollectionFull())
        {
            StartCoroutine(Victory());
        }
    }

    public void CloseCollection()
    {
        collection.enabled = false;
        foreach (Image image in collected.Values)
        {
            image.enabled = false;
        }
    }

    private int GetNum(string name)
    {
        int index = name.IndexOfAny(nums);
        return int.Parse(name.Substring(index, 1));
    }

    private void NewGemAnim()
    {
        Vector3 pos = newGem.rectTransform.position;
        newGem.rectTransform.position = new Vector3(pos.x, pos.y + 1, pos.z);
    }

    private bool CollectionFull()
    {
        for(int i = 1; i <= 8; i++)
        {
            Image image;
            collected.TryGetValue(i, out image);
            if (!image.sprite.Equals(gems[i]))
                return false;
        }
        return true;
    }

    public IEnumerator Victory()
    {
        foreach (Image image in collected.Values)
        {
            image.enabled = false;
        }
        newGem.sprite = crown;
        newGem.transform.position.Set(0, 0, 0);
        newGem.enabled = true;
        yield return new WaitForSeconds(5f);
        PlayerController.ResetAllValues();
        Health.ResetAllValues();
        Sanity.ResetAllValues();
        Application.LoadLevel("Title Screen");
    }
}
