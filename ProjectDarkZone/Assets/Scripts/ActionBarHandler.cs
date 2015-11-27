using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ActionBarHandler : MonoBehaviour
{
    public GameObject selector;
    public GameObject actionBar;
    public int framesBeforeActionBarHide;
    public List<GameObject> itemPrefabs;
    private RectTransform selectorTransform;
    private RectTransform actionBarTransform;
    private ActionBarItem[] items = new ActionBarItem[9];
    private GameObject[] images = new GameObject[9];
    private int selectedIndex = 0;
    private int unused = 0;

    void Start()
    {
        actionBarTransform = actionBar.GetComponent<RectTransform>();
        selectorTransform = selector.GetComponent<RectTransform>();
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = GameObject.Find("ItemImage" + i);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectorTransform.anchoredPosition = new Vector2(0, 0);
            selectedIndex = 0;
            unused = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectorTransform.anchoredPosition = new Vector2(44, 0);
            selectedIndex = 1;
            unused = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectorTransform.anchoredPosition = new Vector2(88, 0);
            selectedIndex = 2;
            unused = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectorTransform.anchoredPosition = new Vector2(132, 0);
            selectedIndex = 3;
            unused = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectorTransform.anchoredPosition = new Vector2(176, 0);
            selectedIndex = 4;
            unused = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectorTransform.anchoredPosition = new Vector2(220, 0);
            selectedIndex = 5;
            unused = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectorTransform.anchoredPosition = new Vector2(264, 0);
            selectedIndex = 6;
            unused = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            selectorTransform.anchoredPosition = new Vector2(308, 0);
            selectedIndex = 7;
            unused = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            selectorTransform.anchoredPosition = new Vector2(352, 0);
            selectedIndex = 8;
            unused = 0;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectorTransform.anchoredPosition.x < 352)
        {
            selectorTransform.Translate(44, 0, 0);
            selectedIndex++;
            unused = 0;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && System.Math.Round(selectorTransform.anchoredPosition.x) > 0)
        {
            selectorTransform.Translate(-44, 0, 0);
            selectedIndex--;
            unused = 0;
        }

        else
        {
            unused++;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            // Add item to action bar
            // Torch as example for now
            Torch torch = new Torch("Torch", Resources.Load<Sprite>("torch"), true, itemPrefabs[0]);
            Flare flare = new Flare("Flare", Resources.Load<Sprite>("torch"), true, itemPrefabs[1]);

            addActionBarItem(torch);
            addActionBarItem(flare);
            addActionBarItem(GameObject.Find("Player").GetComponent<GrapplingHookController>());
            GameObject.Find("Player").GetComponent<FlareController>().flares.Add(flare);
            unused = 0;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            // Use item
            if (items[selectedIndex] != null)
                items[selectedIndex].Item.Use();

            if (items[selectedIndex] != null && items[selectedIndex].Item.IsConsumable)
                removeActionBarItem();

            unused = 0;
        }

        if (unused > framesBeforeActionBarHide && actionBarTransform.anchoredPosition.y > -50)
        {
            actionBarTransform.Translate(0, -5, 0);
        }

        else if (unused < framesBeforeActionBarHide && actionBarTransform.anchoredPosition.y < 0)
        {
            actionBarTransform.Translate(0, 5, 0);
        }

    }

    private void removeActionBarItem()
    {
        if (items[selectedIndex] != null)
        {
            items[selectedIndex].Amount--;
            if (items[selectedIndex].Amount == 0)
            {
                items[selectedIndex] = null;
                images[selectedIndex].GetComponent<Image>().enabled = false;
                images[selectedIndex].GetComponentInChildren<Text>().text = string.Empty;
            }

            else
            {
                images[selectedIndex].GetComponentInChildren<Text>().text =
                    items[selectedIndex].Amount.ToString();
            }
        }
    }

    private void addActionBarItem(IItem item)
    {
        bool OnActionBar = false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].Item.Name== item.Name)
            {
                OnActionBar = true;
                items[i].Amount++;
                images[i].GetComponentInChildren<Text>().text = items[i].Amount.ToString();
            }
        }

        if (!OnActionBar)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    items[i] = new ActionBarItem(item, 1);
                    images[i].GetComponent<Image>().sprite = item.Image;
                    images[i].GetComponent<Image>().enabled = true;
                    images[i].GetComponentInChildren<Text>().text = items[i].Amount.ToString();
                    break;
                }
            }
        }
    }
}
