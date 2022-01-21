using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocationType
{
    Area,
    Dungeon,
}

public enum EncounterType
{
    None,
    Battle,
    Resource,
    BossBattle,
    Treasure,
    Secret,
}

[Serializable]
public class EncounterRate
{
    public EncounterType type; 
    public int rate;
}

[Serializable]
public class Gatherable
{
    public ItemData item;
    public int weightedRate = 1;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Location Data", order = 1)]
public class LocationData : ScriptableObject
{
    public string locationID;
    public string Name;
    public LocationType type;
    [TextArea(3, 10)]
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
    public List<Gatherable> gatherables;
}
