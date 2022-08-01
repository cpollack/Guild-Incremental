using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Hall Data", order = 1)]
public class HallData : ScriptableObject
{
    public string ID;
    public string Name;    
    public string requiresUpgrade;
}
