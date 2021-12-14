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

    //public GameTime currentTime = new GameTime(1, 5);
    public TimeOfDay timeOfDay;
    public string timeString;

    private float elapsed = 0;
    public float autoSaveTimer = 30;

    [Header("Guild Halls")]
    public MainMenu mainMenu;
    public GameObject logPanel;
    public GameObject logEntryPrefab;
    
    public List<GuildHall> halls = new List<GuildHall>();

    [Header("World")]
    public List<Location> locations;

    [Header("Miscellaneous")]
    public HoverInfoPanel hoverInfoPanel;
    public PopupPanel popupPanel;
    public List<ResourceImage> resourceImages;

    [Header("Game Data")]
    private GameData gameData;
    /* Accessors */
    public GameTime CurrentTime { get { return gameData.currentTime; } private set { } }
    public int Renown { get { return gameData.renown; } set { gameData.renown = value; } }
    public int Gold { get { return gameData.gold; } set { gameData.gold = value; } }
    public List<Adventurer> Adventurers { get { return gameData.adventurers; } private set { } }
    
    public List<string> CompletedBuildings { get { return gameData.completedBuildings; } private set { } }
    public List<Building> CurrentBuildProjects { get { return gameData.currentBuildProjects; } private set { } }
    public int MaxConstructionJobs { get { return gameData.maxConstructionJobs; } set { gameData.maxConstructionJobs = value; } }

    public List<string> CompletedQuests { get { return gameData.completedQuests; } private set { } }
    public List<Quest> Quests { get { return gameData.quests; } private set { } }
    public int MaxGuildQuests { get { return gameData.maxGuildQuests; } set { gameData.maxGuildQuests = value; } }
    public int TotalGuildQuestsIssued { get { return gameData.totalGuildQuestsIssued; } set { gameData.totalGuildQuestsIssued = value; } }

    // Start is called before the first frame update
    void Start()
    {
        //Load must occur after AWAKE
        Load();
    }

    private void OnDestroy()
    {
        Save();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        foreach (Adventurer adventurer in Adventurers)
            adventurer.Update();
    }

    /* TIME */
    private void UpdateTime()
    {
        //Real Time
        elapsed += Time.deltaTime;
        if (elapsed >= autoSaveTimer)
        {
            elapsed = 0;
            Save();
        }

        //Game Time
        if (timePaused) return;

        CurrentTime.AddHours(Time.deltaTime / secondsPerDay * 24);      

        if (CurrentTime.hour < 5) timeOfDay = TimeOfDay.Night;
        else if (CurrentTime.hour < 12) timeOfDay = TimeOfDay.Morning;
        else if (CurrentTime.hour < 17) timeOfDay = TimeOfDay.Afternoon;
        else if (CurrentTime.hour <= 22) timeOfDay = TimeOfDay.Evening;
        else timeOfDay = TimeOfDay.Night;

        int clock12 = Mathf.FloorToInt(CurrentTime.hour) % 12;
        if (clock12 == 0) clock12 = 12;
        timeString = "Day " + CurrentTime.day.ToString() + " " + clock12.ToString() + (Mathf.Floor(CurrentTime.hour) <= 11 ? "am" : "pm");
    }

    public GameTime GetElapsedTime(GameTime startTime)
    {
        return CurrentTime.GetDifference(startTime);
    }

    /* Construction */
    public void CompleteBuilding(string buildID)
    {
        CompletedBuildings.Add(buildID);
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
        if (quest.data != null && quest.data.questID.Length > 0) CompletedQuests.Add(quest.data.questID);

        foreach (Resource reward in quest.rewards)
        {
            switch (reward.type)
            {
                case ResourceType.Renown:
                    gameData.renown += reward.value;
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

    /* Utility */

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

    public Adventurer GetAdventurer(string adventurerName)
    {
        foreach (Adventurer adventurer in Adventurers)
        {
            if (adventurer.Name == adventurerName)
                return adventurer;
        }

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

    public QuestData GetQuestData(string questID)
    {
        string fileName = "Quest_" + questID;

        return Resources.Load<QuestData>("Quests/" + fileName);
    }

    public MonsterData GetMonsterData(string monsterID)
    {
        string fileName = "monster_" + monsterID;

        return Resources.Load<MonsterData>("Monsters/" + fileName);
    }

    public ItemData GetItemData(string itemID)
    {
        string fileName = "Item_" + itemID;

        return Resources.Load<ItemData>("Items/" + fileName);
    }

    public void Save()
    {
        foreach (Adventurer adventurer in Adventurers)
            adventurer.Save();

        DataAccessor.Save(gameData);
    }

    public void ResetSave()
    {
        gameData = new GameData();

        foreach (GuildHall hall in halls)
            hall.ResetGame();

        mainMenu.Reset(gameData.completedBuildings, true);
    }

    public void Load()
    {
        gameData = DataAccessor.Load();
        if (gameData == null) return;

        foreach (Adventurer adventurer in Adventurers)
        {
            adventurer.guild = this;
            adventurer.Load();
        }

        foreach (Quest quest in Quests)
        {
            quest.guild = this;
            quest.Load();
        }

        foreach (GuildHall hall in halls)
            hall.Load();

        mainMenu.Reset(gameData.completedBuildings);
    }
}
