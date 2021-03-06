using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ResourceImage
{
    public ResourceType resourceType;
    public Sprite image;
}

[Serializable]
public struct LogEntry
{
    public GameTime time;
    public string text;
}

public class Guild : MonoBehaviour
{
    [Header("Game")]
    public string version = "v0.0.0";

    [Header("Time")]
    public bool timePaused = false;
    public float secondsPerDay = 1.0f;
    public int msPerTick = 1000;
    public int minutesPerTick = 5;

    //public GameTime currentTime = new GameTime(1, 5);
    public TimeOfDay timeOfDay;

    private float autoSaveTimer = 0;
    public float autoSaveInterval = 5;

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
    public StoryButton storyButton;
    public StoryPanel storyPanel;
    public SelectAdventurerPanel selectAdventurerPanel;
    public AdventurerInfoPanel adventurerInfoPanel;

    [Header("Game Data")]
    private GameData gameData;
    /* Accessors */
    public GameTime CurrentTime { get { return gameData.currentTime; } private set { } }
    public Rank Rank { get { return gameData.rank; } set { gameData.rank = value; } }
    public int Renown { get { return gameData.renown; } set { gameData.renown = value; } }
    public int Gold { get { return gameData.gold; } set { gameData.gold = value; } }
    public List<Adventurer> Adventurers { get { return gameData.adventurers; } private set { } }
    public List<Battle> Battles { get { return gameData.activeBattles; } private set { } }
    public List<StoryEntry> StoryEntries { get { return gameData.storyEntries; } private set { } }
    public List<LogEntry> LogEntries { get { return gameData.logEntries; } private set { } }

    public List<string> CompletedUpgrades { get { return gameData.completedUpgrades; } private set { } }
    public List<Upgrade> ActiveUpgrades { get { return gameData.activeUpgrades; } private set { } }
    public int MaxActiveUpgrades { get { return gameData.maxActiveUpgrades; } set { gameData.maxActiveUpgrades = value; } }

    public List<string> CompletedQuests { get { return gameData.completedQuests; } private set { } }
    public List<Quest> Quests { get { return gameData.quests; } private set { } }
    public int MaxGuildQuests { get { return gameData.maxGuildQuests; } set { gameData.maxGuildQuests = value; } }
    public int TotalGuildQuestsIssued { get { return gameData.totalGuildQuestsIssued; } set { gameData.totalGuildQuestsIssued = value; } }

    public Dictionary<string,int> ItemStatistics { get { return gameData.itemStatistics; } private set { } }

    // Start is called before the first frame update
    void Start()
    {
        //Load must occur after AWAKE
        Load();
    }

    private void OnDestroy()
    {
        //Save();
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
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= autoSaveInterval)
        {
            autoSaveTimer = 0;
            Save();
        }

        //Game Time
        if (timePaused) return;


        float elapsedTime = msPerTick / 1000f * Time.deltaTime;
        elapsedTime *= (minutesPerTick / 60f);

        CurrentTime.AddHours(elapsedTime);
        //CurrentTime.AddHours(Time.deltaTime / secondsPerDay * 24);      
    }

    public GameTime GetElapsedTime(GameTime startTime)
    {
        return CurrentTime.GetDifference(startTime);
    }

    /* Upgrades */
    public void CompleteUpgrade(string upgradeID)
    {
        CompletedUpgrades.Add(upgradeID);
        mainMenu.CompleteUpgrade(upgradeID);
        foreach (GuildHall hall in halls)
            hall.CompleteUpgrade(upgradeID);
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
                    gameData.gold += reward.value;
                    break;
                case ResourceType.Merit:
                    adventurer.merit += reward.value;
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

    /* Battles */
    //Once teams are implemented, this should apply the team members to the battle
    public void StartBattle(Battle battle)
    {
        gameData.activeBattles.Add(battle);
    }

    public void EndBattle(Battle battle)
    {
        gameData.activeBattles.Remove(battle);
        battle.ClearTeamBattle(this);
    }

    /* Utility */

    public void AddStoryEntry(string header, string text)
    {
        TriggerPopup("New Story Unlocked!");

        StoryEntry storyEntry = new StoryEntry();
        storyEntry.read = false;
        storyEntry.time = new GameTime(CurrentTime);
        storyEntry.header = header;
        storyEntry.text = text;
        StoryEntries.Add(storyEntry);

        storyButton.UpdateButtonState();
    }

    public void AddLogEntry(string logText)
    {
        LogEntry logEntry = new LogEntry();
        logEntry.time = new GameTime(CurrentTime);
        logEntry.text = logText;
        LogEntries.Add(logEntry);

        AddLogEntry(logEntry);
    }

    public void AddLogEntry(LogEntry entry)
    {
        GameObject entryObj = Instantiate(logEntryPrefab);
        entryObj.GetComponent<TextMeshProUGUI>().text = entry.time.GetFormattedTime() + " - " + entry.text;
        entryObj.transform.SetParent(logPanel.transform, false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(logPanel.GetComponent<RectTransform>());
    }

    public void TriggerPopup(string content)
    {
        popupPanel.Popup(CurrentTime.GetFormattedTime(), content);
    }

    public void LogAndPopup(string content)
    {
        AddLogEntry(content);
        TriggerPopup(content);
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

    public TavernRecipeData GetTavernRecipeData(string recipeID)
    {
        string fileName = "TavernRecipe_" + recipeID;

        return Resources.Load<TavernRecipeData>("Tavern/" + fileName);
    }

    public void UpdateItemStats(string itemID, int count = 1)
    {
        if (ItemStatistics.ContainsKey(itemID))
        {
            ItemStatistics[itemID] += count;
        }
        else
        {
            ItemStatistics.Add(itemID, count);
            //Inform recipes they may need to update?
        }
    }

    /* Resource Handling */

    public bool MeetsResourceRequirements(List<GameResource> resources)
    {
        foreach (GameResource resource in resources)
        {
            if (!HasResource(resource)) return false;
        }

        return true;
    }

    public bool HasResource(GameResource resource)
    {
        switch (resource.resourceType)
        {
            case ResourceType.Renown:
                return Renown >= resource.value;

            case ResourceType.Gold:
                return Gold >= resource.value;

            case ResourceType.Upgrade:
                return CompletedUpgrades.Contains(resource.strValue);
        }

        return false;
    }

    public Sprite GetResourceImage(ResourceType resourceType)
    {
        foreach (ResourceImage resourceImage in resourceImages)
            if (resourceImage.resourceType == resourceType) return resourceImage.image;

        return null;
    }

    /* Save / Load */

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

        mainMenu.Reset(gameData.completedUpgrades, true);

        foreach (Transform transform in logPanel.transform)
            Destroy(transform.gameObject);

        storyButton.UpdateButtonState();
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

        foreach (Battle battle in Battles)
        {
            battle.Load(this);
        }

        foreach (Quest quest in Quests)
        {
            quest.guild = this;
            quest.Load();
        }

        foreach (GuildHall hall in halls)
            hall.Load();

        mainMenu.Reset(gameData.completedUpgrades);

        foreach (LogEntry logEntry in LogEntries)
            AddLogEntry(logEntry);

        storyButton.UpdateButtonState();
    }
}
