using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActionBarHandler : MonoBehaviour
{
    public GameObject selector;
    public GameObject actionBar;
    private RectTransform transform;
    private ActionBarItem[] items = new ActionBarItem[9];
    private GameObject[] images = new GameObject[9];
    private int selectedIndex = 0;

    void Start()
    {
        transform = selector.GetComponent<RectTransform>();
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = GameObject.Find("ItemImage" + i);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            transform.anchoredPosition = new Vector2(0, 0);
            selectedIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            transform.anchoredPosition = new Vector2(44, 0);
            selectedIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            transform.anchoredPosition = new Vector2(88, 0);
            selectedIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            transform.anchoredPosition = new Vector2(132, 0);
            selectedIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            transform.anchoredPosition = new Vector2(176, 0);
            selectedIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            transform.anchoredPosition = new Vector2(220, 0);
            selectedIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            transform.anchoredPosition = new Vector2(264, 0);
            selectedIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            transform.anchoredPosition = new Vector2(308, 0);
            selectedIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            transform.anchoredPosition = new Vector2(352, 0);
            selectedIndex = 8;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && transform.anchoredPosition.x < 352)
        {
            transform.Translate(44, 0, 0);
            selectedIndex++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && System.Math.Round(transform.anchoredPosition.x) > 0)
        {
            transform.Translate(-44, 0, 0);
            selectedIndex--;
        }

        // On item pickup
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Using randomized item names right now for demonstration
            System.Random rnd = new System.Random();
            addActionBarItem("ItemName" + rnd.Next(0, 9));
        }

        // On item use
        if (Input.GetKeyDown(KeyCode.U))
        {
            removeActionBarItem();
        }


    }

    private void removeActionBarItem()
    {
        if (items[selectedIndex] != null)
        {
            items[selectedIndex].amount--;
            if (items[selectedIndex].amount == 0)
            {
                items[selectedIndex] = null;
                images[selectedIndex].GetComponent<Image>().enabled = false;
                images[selectedIndex].GetComponentInChildren<Text>().text = string.Empty;
            }

            else
            {
                images[selectedIndex].GetComponentInChildren<Text>().text =
                    items[selectedIndex].amount.ToString();
            }
        }
    }

    private void addActionBarItem(string itemName)
    {
        bool OnActionBar = false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].name == itemName)
            {
                OnActionBar = true;
                items[i].amount++;
                images[i].GetComponentInChildren<Text>().text = items[i].amount.ToString();
            }
        }

        if (!OnActionBar)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    items[i] = new ActionBarItem(itemName, 1);
                    // Add item image
                    // images[i].GetComponent<Image>().sprite = Resources.Load("Image");
                    images[i].GetComponent<Image>().enabled = true;
                    images[i].GetComponentInChildren<Text>().text = items[i].amount.ToString();
                    break;
                }
            }
        }
    }
}
