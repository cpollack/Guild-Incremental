using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Quest Data", order = 1)]
public class QuestData : ScriptableObject
{
    public string questID;
    public string questName;
    public string questDescription;

    public QuestCategory category;
    public QuestType type;

    public string locationID;
    public string monsterId;
    public string itemID;
    public int count;

    public List<Resource> rewards;

    public List<Resource> unlockRequirements;

    [TextArea(3,5)]
    public string unlockPopup;
    public string unlockLog;
}
