using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Upgrade Data", order = 1)]
public class UpgradeData : ScriptableObject
{
    public string upgradeID;
    public string title;
    public string description;
    public string completeLog;

    public List<GameResource> requires;
    public List<GameResource> cost;    

    public int timeDays = 0;
    public float timeHours = 0f;

    public string requiresUpgrade;
}
