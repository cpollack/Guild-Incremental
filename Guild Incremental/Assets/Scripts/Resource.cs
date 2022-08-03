using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Renown,
    Gold,
    Merit,
    Item,
    Blueprint,
    Recipe,
    Upgrade,

    Fire = 100,
    Water,
    Plant,

    MonsterPart = 200,
    Food,
    Drink,

    //Time
    Day = 800,
    Night,

    //Misc
    Monster = 900,
    Map,
    Dungeon,
}

[Serializable]
public class Resource
{
    public ResourceType type;
    public int value;

    public Resource(ResourceType type, int val)
    {
        this.type = type;
        value = val;
    }

    public bool HasResource(Guild guild)
    {
        switch (type)
        {
            case ResourceType.Renown:
                if (guild.Renown >= value)
                    return true;
                break;
        }

        return false;
    }

    public bool Met(Guild guild)
    {
        return HasResource(guild);
    }
}
