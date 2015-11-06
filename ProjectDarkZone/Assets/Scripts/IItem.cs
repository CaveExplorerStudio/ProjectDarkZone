using UnityEngine;

public interface IItem
{
    string Name { get; set; }
    Sprite Image { get; set; }
    bool IsConsumable { get; set; }
    GameObject Prefab { get; set; }
    void Use();
}