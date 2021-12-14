using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public Item(ItemData data, int count = 1)
    {
        itemID = data.itemID;
        name = data.Name;
        description = data.Description;
        this.count = count;
    }

    public string itemID;
    public string name;
    public string description;

    public int count;
}
