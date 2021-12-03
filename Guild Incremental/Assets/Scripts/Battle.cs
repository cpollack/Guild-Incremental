using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Battle
{
    public Battle()
    {

    }

    private List<Adventurer> team = new List<Adventurer>();
    private List<Monster> monsters = new List<Monster>();

    private bool ranAway = false;

    public void addAdventurer(Adventurer adventurer)
    {
        team.Add(adventurer);
    }

    public void addMonster(MonsterData data)
    {
        Monster monster = new Monster(data);
        monsters.Add(monster);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Bool - Whether the adventurer(s) were victorious</returns>
    public bool Process()
    {
        bool continueBattle = true;

        while (continueBattle)
        {
            List<IFighter> pool = GetFighterPool();

            while (pool.Count > 0)
            {
                IFighter nextFighter = pool[0];
                pool.RemoveAt(0);

                if (nextFighter.CanAct())
                {
                    List<IFighter> targets = GetTargetPool(nextFighter);
                    nextFighter.Attack(targets[0]);
                }

                if (!CanContinue())
                {
                    continueBattle = false;
                    break;
                }
            }
        }

        return DidTeamWin();
    }

    private List<IFighter> GetFighterPool()
    {
        List<IFighter> pool = new List<IFighter>();

        foreach (IFighter fighter in team)
        {
            if (fighter.CanAct())
            {
                pool.Add(fighter);
            }
        }

        foreach (IFighter fighter in monsters)
        {
            if (fighter.CanAct())
            {
                pool.Add(fighter);
            }
        }

        List<IFighter> SortedList = pool.OrderByDescending(o => o.GetSpeed()).ToList();

        return SortedList;
    }

    private List<IFighter> GetTargetPool(IFighter attacker)
    {
        List<IFighter> pool = new List<IFighter>();

        if (attacker is Monster)
        {
            foreach (IFighter fighter in team)
            {
                if (fighter.IsAlive())
                {
                    pool.Add(fighter);
                }
            }
        }
        else
        {
            foreach (IFighter fighter in monsters)
            {
                if (fighter.IsAlive())
                {
                    pool.Add(fighter);
                }
            }
        }

        return pool;
    }

    private bool CanContinue()
    {
        bool alive = false;
        foreach (IFighter fighter in team)
        {
            if (fighter.IsAlive())
            {
                alive = true;
                break;
            }
        }
        if (!alive) return false;

        alive = false;
        foreach (IFighter fighter in monsters)
        {
            if (fighter.IsAlive())
            {
                alive = true;
                break;
            }
        }
        return alive;
    }

    private bool DidTeamWin()
    {
        bool alive = false;
        foreach (IFighter fighter in team)
        {
            if (fighter.IsAlive())
            {
                alive = true;
                break;
            }
        }
        return alive;
    }

    public void UpdateMonsterKills()
    {
        foreach (Adventurer adventurer in team)
        {
            foreach (Monster monster in monsters)
            {
                adventurer.UpdateSlayHistory(monster.data, 1);
            }
        }
    }

    public void AwardExp()
    {
        int exp = 0;
        foreach (Monster monster in monsters)
        {
            exp += monster.data.experience;
        }

        foreach (Adventurer adventurer in team)
        {
            adventurer.GainExp(exp);
        }
    }

    public void AwardGold()
    {
        int gold = 0;
        foreach (Monster monster in monsters)
        {
            gold += monster.data.gold;
        }

        foreach (Adventurer adventurer in team)
        {
            adventurer.GainGold(gold);
        }
    }

    public void RollDrops()
    {
        Dictionary<ItemData, int> drops = new Dictionary<ItemData, int>();

        //Roll the drop list
        foreach (Monster monster in monsters)
        {
            foreach (ItemDrop drop in monster.data.itemDrops)
            {
                float rand = UnityEngine.Random.Range(0, 1f);
                if (rand >= drop.dropChance)
                {
                    if (drops.ContainsKey(drop.data)) drops[drop.data]++;
                    else drops.Add(drop.data, 1);
                }
            }
        }

        //Award drops to adventurers
        foreach (Adventurer adventurer in team)
        {
            foreach (KeyValuePair<ItemData, int> drop in drops)
            {
                adventurer.GainItem(drop.Key, drop.Value);
            }
        }
    }
}
