using UnityEngine;
using System.Collections;

public class ActionBarItem
{
    public IItem Item { get; set; }
    public int Amount { get; set; }

    public ActionBarItem(IItem item, int amount)
    {
        this.Item = item;
        this.Amount = amount;
    }

    public ActionBarItem()
    {

    }
}
