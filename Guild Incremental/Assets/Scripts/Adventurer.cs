using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Rank
{
    F,
    E,
    D,
    C,
    B,
    A,
    S,
}

public enum Race
{
    Human,
    Elf,
    Dwarf,
    Halfling,
}

public enum HeroClass
{
    Adventurer,
    Warrior,
    Thief,
    Hunter,
    Priest,
    Wizard,
}

public enum EquipmentSlot
{
    Weapon,
    Shield,

    Head,
    Body,
    Hands,
    Feet,

    Ring,
    Rune,
}

[Serializable]
public class Adventurer : IFighter
{    
    public Adventurer()
    {
        //empty constructor for json utility
    }

    public Adventurer(Guild guild)
    {
        currentLocation = null;
        targetLocation = null;
        this.guild = guild;

        InitializeStateMachine();
    }

    [NonSerialized] public Guild guild;
    [NonSerialized] public AdventurerPanel adventurerPanel;

    [Header("Attributes")]
    public string Name;
    public Race race = Race.Human;
    public HeroClass heroClass = HeroClass.Adventurer;
    public Rank rank = Rank.F;
    public int level = 1;
    public int experience = 0;
    public int merit = 0;

    public float currentLife = 10;
    public int life = 10;
    public float currentMagic = 0;
    public int magic = 0;

    public int strength = 1;
    public int agility = 1;
    public int intellect = 1;
    public float recoverPerHour = 0.1f;

    //[Serializable] public class DictionaryEquipment : SerializableDictionary<EquipmentSlot, Item> { }
    //[Serializable] public class DictionaryMonsterInt : SerializableDictionary<MonsterData, int> { }

    [Header("Items")]    
    public Dictionary<EquipmentSlot, Item> equipment = new Dictionary<EquipmentSlot, Item>();
    public Dictionary<string,int> inventory = new Dictionary<string, int>(); //If stack size or unstackable items will exist, move to class    
    public Dictionary<MonsterData, int> monsterSlayHistory = new Dictionary<MonsterData, int>();

    [Header("Adventuring")]
    public string actionString = "";
    public float actionPerc = 0f;
    public string currentLocationID = "";
    [NonSerialized] [SerializeReference] public Location currentLocation; //null implies guild
    public string targetLocationID = "";
    [NonSerialized] [SerializeReference] public Location targetLocation;
    [NonSerialized] [SerializeReference] public Quest currentQuest = null;
    public string assignedQuestID = "";
    [NonSerialized] [SerializeReference] public Quest assignedQuest = null;
    [NonSerialized] [SerializeReference] public Battle battle = null;

    [Header("Party")]
    public bool isLeader = true;
    //Party class?

    [Header("States")]
    [NonSerialized] public StateMachine StateMachine;
    public string currentState;
    public int currentSubState;
    public GameTime stateStartTime;
    public GameTime subStateStartTime;
    public GameTime stateLength;
    public bool Resting;

    private void InitializeStateMachine()
    {
        StateMachine = new StateMachine();
        var states = new Dictionary<Type, BaseState>
        {
            { typeof(StateIdle), new StateIdle(this) },
            { typeof(StateAdventurePrep), new StateAdventurePrep(this) },
            { typeof(StateTravel), new StateTravel(this) },
            { typeof(StateExplore), new StateExplore(this) },
            { typeof(StateRest), new StateRest(this) },
            { typeof(StateDeath), new StateDeath(this) },            
        };
        StateMachine.SetStates(states);
    }

    // Update is called once per frame
    public void Update()
    {
        StateMachine.Update();      
    }

    public void Save()
    {
        StateMachine.CurrentState?.Save();
        currentState = StateMachine.CurrentState?.GetType().ToString();
        currentSubState = StateMachine.CurrentState == null ? 0 : StateMachine.CurrentState.GetSubState();
    }

    public void Load()
    {
        if (currentLocationID.Length > 0 && currentLocation == null)
            currentLocation = guild.GetLocation(currentLocationID);

        if (targetLocationID.Length > 0 && targetLocation == null)
            targetLocation = guild.GetLocation(targetLocationID);

        //if (bossBattle != null)
        //    bossBattle.Load(guild);

        InitializeStateMachine();

        try
        {
            Type stateType = Type.GetType(currentState, true);
            StateMachine.ForceState(stateType);
            StateMachine.CurrentState.Load();
        }
        catch (TypeLoadException e)
        {
            Debug.LogError("Unable to load adventurer state [" + currentState + "] " + e.Message);
        }
    }

    public void SetActionText(string text)
    {
        actionString = text;
    }

    public void SetActionPercent(float percent)
    {
        actionPerc = Math.Min(percent * 100, 100);
    }

    public void HideActionPercent()
    {
        actionPerc = -1;
    }

    public bool ChooseQuest()
    {
        if (currentQuest != null) return true;
        List<Quest> availableQuests = guild.GetAvailableQuests();

        List<Quest> questPool = new List<Quest>();
        foreach (Quest quest in availableQuests)
        {
            //if (level / quest.recommendedLevel >= 0.9 && level / quest.recommendedLevel <= 1.2) questPool.Add(quest);

            //Main quests can only be assigned
            if (quest.category == QuestCategory.Main) continue;

            //Skip projects for now
            if (quest.category == QuestCategory.Project) continue;

            if (level >= quest.minLevel && level <= quest.maxLevel) questPool.Add(quest);
        }

        if (questPool.Count == 0) return false;

        Quest selectedQuest = questPool[UnityEngine.Random.Range(0,questPool.Count-1)];
        if (selectedQuest.Claim(this))
            currentQuest = selectedQuest;

        return (currentQuest != null);
    }

    public void TurnInQuest()
    {
        if (currentQuest == null) return;
        if (!currentQuest.objectiveMet) return;

        if (guild.CompleteQuest(this, currentQuest))
        {
            currentQuest = null;
        }
    }

    public bool IsMainQuesting()
    {
        if (currentQuest == null) return false;
        if (currentQuest.category == QuestCategory.Main) return true;
        return false;
    }

    public bool IsHuntingBoss()
    {
        if (!IsMainQuesting()) return false;
        if (currentQuest.type == QuestType.Boss) return true;
        return false;
    }

    public bool InBattle()
    {
        return battle != null;
    }

    public bool ChooseLocation()
    {
        if (currentQuest != null)
        {            
            targetLocation = currentQuest.targetLocation;
            targetLocationID = targetLocation.data.locationID;
        }
        else
        {
            //rules for choosing a random location?
            targetLocation = guild.locations[0];
            targetLocationID = targetLocation.data.locationID;
        }
        return targetLocation != null ? true : false;        
    }

    public float GetTravelSpeed()
    {
        float baseSpeed = 1;

        //adjust based on bonuses and horse etc

        return baseSpeed;
    }

    public int NextLevel()
    {
        return (int)Math.Round((0.7f * Mathf.Pow(level, 3)) + (level * 20));
    }

    public void GainExp(int exp)
    {
        experience += exp;

        int nextLevel = NextLevel();
        while (experience >= nextLevel)
        {
            LevelUp();
            experience -= nextLevel;
            nextLevel = NextLevel();
        }
    }

    public void LevelUp()
    {
        level++;

        strength += Dice.Roll("2d2");
        agility += Dice.Roll("2d2");
        intellect += Dice.Roll("2d2");
        life += 5;
        currentLife = life;
    }

    public void GainItem(ItemData itemData, int count = 1)
    {
        if (itemData == null) return;

        if (inventory.ContainsKey(itemData.Name))
        {
            inventory[itemData.Name] += count;
        }
        else
        {
            inventory.Add(itemData.Name, count);
        }

        if (currentQuest != null) currentQuest.UpdateStatus(itemData, inventory[itemData.Name]);
    }

    public void RemoveItem(ItemData itemData)
    {
        if (itemData == null) return;

        if (inventory.ContainsKey(itemData.Name))
        {
            inventory[itemData.Name]--;
            if (inventory[itemData.Name] <= 0)
            {
                inventory.Remove(itemData.Name);
            }
        }
    }

    public void Equip(ItemData itemData)
    {
        return;
    }

    public void UnEquip(EquipmentSlot slot)
    {

    }

    public Item GetEquipedItem(EquipmentSlot slot)
    {
        return equipment.ContainsKey(slot) ? equipment[slot] : null;
    }

    public void UpdateSlayHistory(string monsterID, int count = 1)
    {
        MonsterData data = guild.GetMonsterData(monsterID);
        //if (monsterSlayHistory.ContainsKey(monster)) monsterSlayHistory[monster] += count;
        //else monsterSlayHistory.Add(monster, count);
        if (currentQuest != null) currentQuest.UpdateStatus(data, count);
    }

    public void Recover(float hoursElapsed)
    {
        currentLife += recoverPerHour * hoursElapsed * life;
        if (currentLife > life) currentLife = life;
    }

    /* IFighter */
    public int GetAttack()
    {
        return strength;
    }

    public int GetDefence()
    {
        return 1;
    }

    public int GetSpeed()
    {
        return agility;
    }

    public int GetMaxLife()
    {
        return life;
    }

    public bool IsMaxLife()
    {
        return currentLife == life;
    }

    public float GetCurrentLife()
    {
        return currentLife;
    }

    public void ResetLife()
    {
        currentLife = life;
    }

    public bool IsAlive()
    {
        return currentLife > 0;
    }

    public bool CanAct()
    {
        //Extend with status effects
        return IsAlive();
    }

    public void Attack(IFighter target)
    {
        float damage = GetAttack() - target.GetDefence();
        damage = Mathf.Max(damage, 1);
        target.TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        currentLife -= damage;
        currentLife = Mathf.Max(currentLife, 0);
    }
}
