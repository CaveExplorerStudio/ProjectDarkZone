using UnityEngine;
using System.Collections;

public class ActionBarItem
{
    public string name { get; set; }
    public int amount { get; set; }

    public ActionBarItem(string name, int amount)
    {
        this.name = name;
        this.amount = amount;
    }

    public ActionBarItem()
    {

    }
}
