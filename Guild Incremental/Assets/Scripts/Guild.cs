using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TimeOfDay
{
    Morning,    //7
    Afternoon,  //5
    Evening,    //5
    Night,      //7
}

[Serializable]
public struct ResourceImage
{
    public ResourceType resourceType;
    public Sprite image;
}

public class Guild : MonoBehaviour
{
    [Header("Time")]
    public bool timePaused = false;
    public float secondsPerDay = 1.0f;

    public GameTime currentTime = new GameTime(1, 5);
    public TimeOfDay timeOfDay;
    public string timeString;

    [Header("Guild")]
    public int gold = 0;
    public int renown = 0;

    [Header("Guild Halls")]
    public MainMenu mainMenu;
    public GameObject logPanel;
    public GameObject logEntryPrefab;
    public List<string> completedBuildings = new List<string>();
    public List<GuildHall> halls = new List<GuildHall>();

    [Header("World")]
    public List<Location> locations;
    public List<string> completedQuests = new List<string>();

    [Header("Miscellaneous")]
    public HoverInfoPanel hoverInfoPanel;
    public PopupPanel popupPanel;
    public List<ResourceImage> resourceImages;

    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
    }

    /* TIME */
    private void UpdateTime()
    {
        if (timePaused) return;

        currentTime.AddHours(Time.deltaTime / secondsPerDay * 24);      

        if (currentTime.hour < 5) timeOfDay = TimeOfDay.Night;
        else if (currentTime.hour < 12) timeOfDay = TimeOfDay.Morning;
        else if (currentTime.hour < 17) timeOfDay = TimeOfDay.Afternoon;
        else if (currentTime.hour <= 22) timeOfDay = TimeOfDay.Evening;
        else timeOfDay = TimeOfDay.Night;

        int clock12 = Mathf.FloorToInt(currentTime.hour) % 12;
        if (clock12 == 0) clock12 = 12;
        timeString = "Day " + currentTime.day.ToString() + " " + clock12.ToString() + (Mathf.Floor(currentTime.hour) <= 11 ? "am" : "pm");
    }

    public GameTime GetElapsedTime(GameTime startTime)
    {
        return currentTime.GetDifference(startTime);
    }

    /* Construction */
    public void CompleteBuilding(string buildID)
    {
        completedBuildings.Add(buildID);
        mainMenu.CompleteBuilding(buildID);
        foreach (GuildHall hall in halls)
            hall.CompleteBuild(buildID);
    }

    /* QUESTS */
    public List<Quest> GetAvailableQuests()
    {
        foreach (GuildHall hall in halls)
            if (hall.data.ID == "QuestBoard")
                 return ((QuestBoard)hall).GetAvailableQuests();

        return new List<Quest>();
    }

    public bool CompleteQuest(Adventurer adventurer, Quest quest)
    {
        if (quest.data != null && quest.data.questID.Length > 0) completedQuests.Add(quest.data.questID);

        foreach (Resource reward in quest.rewards)
        {
            switch (reward.type)
            {
                case ResourceType.Renown:
                    renown += reward.value;
                    break;
                case ResourceType.Gold:
                    adventurer.GainGold(reward.value);
                    break;
                default:
                    Debug.LogWarning("Guild::CompleteQuest unhandled ResourceType [" + reward.type + "]");
                    break;
            }
        }                

        foreach (GuildHall hall in halls)
        {
            if (hall.data.ID == "QuestBoard")
            {
                ((QuestBoard)hall).CompleteQuest(quest);
                break;
            }
        }
            
        return true;
    }

    public void GetQuestGoldRates(int gold, out int coffers, out int adv)
    {
        coffers = (int)(gold * 0.1f);
        if (coffers == 0) coffers = 1;
        adv = gold - coffers;
    }

    /* Adventurers */

    //Spawns the initial adventurer
    public void SpawnFirstAdventurer()
    {
        foreach (GuildHall hall in halls)
            if (hall.data.ID == "MainHall")
            {
                ((MainHall)hall).SpawnFirstAdventurer();
                break;
            }
    }

    //Spawn a random adventurer. Filters/rules?
    public void SpawnAdventurer()
    {
        //
    }

    public void AddLogEntry(string logEntry)
    {
        GameObject entryObj = Instantiate(logEntryPrefab);
        entryObj.GetComponent<Text>().text = timeString + " - " + logEntry;
        entryObj.transform.SetParent(logPanel.transform, false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(logPanel.GetComponent<RectTransform>());
    }

    public void TriggerPopup(string content)
    {
        popupPanel.Popup(timeString, content);
    }

    public void LogAndPopup(string content)
    {
        AddLogEntry(content);
        TriggerPopup(content);
    }

    public Sprite GetResourceImage(ResourceType resourceType)
    {
        foreach (ResourceImage resourceImage in resourceImages)
            if (resourceImage.resourceType == resourceType) return resourceImage.image;

        return null;
    }

    public Location GetLocation(string locationID)
    {
        foreach (Location loc in locations)
        {
            if (loc.data.locationID == locationID)
                return loc;
        }

        return null;
    }
}
