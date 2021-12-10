using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Renown,
    Bank,
    Gold,
    Merit,
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
                if (guild.renown >= value)
                    return true;
                break;
        }

        return false;
    }
}
