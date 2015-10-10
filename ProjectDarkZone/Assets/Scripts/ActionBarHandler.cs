using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActionBarHandler : MonoBehaviour
{
    public GameObject selector;
    private RectTransform transform;

    void Start()
    {
        transform = selector.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            transform.anchoredPosition = new Vector2(0, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            transform.anchoredPosition = new Vector2(44, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            transform.anchoredPosition = new Vector2(88, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            transform.anchoredPosition = new Vector2(132, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            transform.anchoredPosition = new Vector2(176, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            transform.anchoredPosition = new Vector2(220, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            transform.anchoredPosition = new Vector2(264, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            transform.anchoredPosition = new Vector2(308, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            transform.anchoredPosition = new Vector2(352, 0);
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && transform.anchoredPosition.x < 352)
            transform.Translate(44, 0, 0);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && System.Math.Round(transform.anchoredPosition.x) > 0)
            transform.Translate(-44, 0, 0);
    }
}
