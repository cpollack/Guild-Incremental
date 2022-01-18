using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDrop
{
    public ItemData data;
    [Range(0, 1)]
    public float dropChance;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Monster Data", order = 2)]
public class MonsterData : ScriptableObject
{
    [Header("Attributes")]
    public string monsterID;
    public string Name;
    public string Description;
    public int health;
    public int attack;
    public int defence;
    public int speed;

    [Header("Requirements")]
    [Range(0,1)]
    public float explorationPercent;

    [Header("Rewards")]
    public int experience;
    public int gold;
    public List<ItemDrop> itemDrops;
}
