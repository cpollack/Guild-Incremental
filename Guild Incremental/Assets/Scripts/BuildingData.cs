using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Building Data", order = 1)]
public class BuildingData : ScriptableObject
{
    public string buildingID;
    public string buildingTitle;
    public string description;
    public string completeLog;

    public int requiredRenown = 0;
    public List<BuildingResource> cost;

    public int buildTimeDays = 0;
    public float buildTimeHours = 0f;

    public string requiresBuilding;
}
