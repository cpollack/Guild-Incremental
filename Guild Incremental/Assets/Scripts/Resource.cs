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
}
