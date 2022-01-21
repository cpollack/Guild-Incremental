using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Quest Data", order = 1)]
public class QuestData : ScriptableObject
{
    public string questID;
    public string questName;
    [TextArea(3, 10)]
    public string questDescription;

    public QuestCategory category;
    public QuestType type;

    public string locationID;
    public string monsterID;
    public string itemID;
    public int count;

    public List<Resource> rewards;

    public List<Resource> unlockRequirements;

    public string unlockStoryHeader;
    [TextArea(3, 5)]
    public string unlockStory;
    public string unlockLog;
}
