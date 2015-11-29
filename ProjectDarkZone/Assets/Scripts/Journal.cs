using UnityEngine;
using System.Collections;

public class Journal : IItem {

    public string Name { get; set; }
    public Sprite Image { get; set; }
    public bool IsConsumable { get; set; }
    public GameObject Prefab { get; set; }

    public Journal(string name, Sprite image, bool isConsumable, GameObject prefab)
    {
        Name = name;
        Image = image;
        IsConsumable = IsConsumable;
        Prefab = prefab;
    }

    public void Use()
    {

    }
}
