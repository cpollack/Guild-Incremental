using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EquipmentSlot
{
    Head,
    Body,
    Hands,
    Waist,
    Legs,
    Feet,

    Lefthand,
    RightHand,
    TwoHand,

    Ring1,
    Ring2,
    Earring,
    Necklace,
}

[Serializable]
public class Adventurer : MonoBehaviour, IFighter
{
    public StateMachine StateMachine => GetComponent<StateMachine>();

    public Guild guild;

    [Header("Attributes")]
    public string Name;
    public int level = 1;
    public int experience = 0;
    public int gold = 0;
    public float currentLife = 10;
    public int life = 10;
    public int attack = 1;
    public int defence = 1;
    public int speed = 3;
    public float recoverPerHour = 0.1f;

    [Header("Items")]
    public Dictionary<EquipmentSlot, ItemData> equipment = new Dictionary<EquipmentSlot, ItemData>();
    public Dictionary<string, int> inventory = new Dictionary<string, int>(); //If stack size or unstackable items will exist, move to class

    private Dictionary<MonsterData, int> monsterSlayHistory = new Dictionary<MonsterData, int>();
    [SerializeReference] public Quest currentQuest = null;

    [Header("Adventuring")]
    [SerializeReference] public Location currentLocation; //null implies guild
    [SerializeReference] public Location targetLocation;
    public string actionString;
    public float actionPerc;

    private void Awake()
    {
        currentLocation = null;
        targetLocation = null;
        currentLife = life;

        guild = GameObject.Find("Guild").GetComponent<Guild>();

        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>
        {
            { typeof(StateIdle), new StateIdle(this) },
            { typeof(StateAdventurePrep), new StateAdventurePrep(this) },
            { typeof(StateTravel), new StateTravel(this) },
            { typeof(StateExplore), new StateExplore(this) },
            { typeof(StateDeath), new StateDeath(this) },
        };
        StateMachine.SetStates(states);
    }

    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        //HandleActionState();
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

    public bool ChooseLocation()
    {
        if (currentQuest != null) targetLocation = currentQuest.targetLocation;
        else
        {
            //rules for choosing a random location?
            targetLocation = guild.locations[0];
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
        return level * (level + 1);
    }

    public void GainExp(int exp)
    {
        experience += exp;

        int nextLevel = NextLevel();
        while (experience > nextLevel)
        {
            LevelUp();
            experience -= nextLevel;
            nextLevel = NextLevel();
        }
    }

    public void LevelUp()
    {
        level++;
        //other bonuses?

        /*TEMP*/
        attack += 1;
        defence += 1;
        speed += 1;
        life += 5;
        currentLife = life;
    }

    public void GainGold(int amount)
    {
        gold += amount;
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

    public void UpdateSlayHistory(MonsterData monster, int count = 1)
    {
        if (monsterSlayHistory.ContainsKey(monster)) monsterSlayHistory[monster] += count;
        else monsterSlayHistory.Add(monster, count);
        if (currentQuest != null) currentQuest.UpdateStatus(monster, count);
    }

    public void Recover(float hoursElapsed)
    {
        currentLife += recoverPerHour * hoursElapsed * life;
        if (currentLife > life) currentLife = life;
    }

    /* IFighter */
    public int GetAttack()
    {
        return attack;
    }

    public int GetDefence()
    {
        return defence;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public int GetMaxLife()
    {
        return life;
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
