using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RecipeEntry
{
    public ItemData item;
    public int count;
}

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Tavern Recipe Data", order = 1)]
public class TavernRecipeData : ScriptableObject
{
    public string id;
    public string Name;
    [TextArea(3, 10)]
    public string description;
    public TavernItemType type;
    public Rank rank;
    public int gold;

    [Header("Recipe")]
    [SerializeField]
    public List<RecipeEntry> requiredItems = new List<RecipeEntry>();
}
