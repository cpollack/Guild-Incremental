using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EncounterType
{
    None,
    Battle,
    Resource,
    Treasure,
    Secret,
}

[Serializable]
public class EncounterRate
{
    public EncounterType type; 
    public int rate;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Location Data", order = 1)]
public class LocationData : ScriptableObject
{
    public string Name;
    public string Description;

    public int minLevel = 1;
    public int maxLevel = 1;
    public float distance;
    public int depthInHours;
    public int maxExplore;

    public List<EncounterRate> encounterRates;

    //monsters
    public List<MonsterData> monsters;
    public MonsterData boss;

    //gatherables
    public List<ItemData> gatherables;

    //Quests?
}
