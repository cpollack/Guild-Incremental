using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item Data", order = 3)]
public class ItemData : ScriptableObject
{
    [Header("Attributes")]
    public string itemID;
    public string Name;
    public string Description;
    public ResourceType resourceType;
}
