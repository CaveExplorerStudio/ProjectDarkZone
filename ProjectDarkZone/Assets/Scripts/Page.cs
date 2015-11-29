using UnityEngine;
using System.Collections;

public class Page : IItem{
    public string Name { get; set; }
    public Sprite Image { get; set; }
    public bool IsConsumable { get; set; }
    public GameObject Prefab { get; set; }
    public string header { get; set; }
    public string body { get; set; }
    public string footer { get; set; }
    public int pageNumber;

    public Page (string name, Sprite image, bool isConsumable, GameObject prefab)
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
