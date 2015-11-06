using UnityEngine;
using System.Collections;

public class Flare : IItem {

    public string Name { get; set; }
    public Sprite Image { get; set; }
    public bool IsConsumable { get; set; }
    public GameObject Prefab { get; set; }

    public void Use()
    {

    }
}
