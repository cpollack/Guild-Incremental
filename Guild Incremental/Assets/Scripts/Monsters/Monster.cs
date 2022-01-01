using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : IFighter
{
    public string monsterID;
    public string Name;
    public string Description;
    public int health;
    public int attack;
    public int defence;
    public int speed;

    public float currentHealth;

    public int experience;
    public int gold;
    public List<ItemDrop> itemDrops;

    public Monster(MonsterData data)
    {
        monsterID = data.monsterID;
        Name = data.Name;
        Description = data.Description;
        health = data.health;
        currentHealth = data.health;
        attack = data.attack;
        defence = data.defence;
        speed = data.speed;

        experience = data.experience;
        gold = data.gold;
        itemDrops = data.itemDrops;
    }

    /* IFighter */
    public float GetCurrentLife()
    {
        return currentHealth;
    }

    public int GetMaxLife()
    {
        return health;
    }

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

    public bool IsAlive()
    {
        return currentHealth > 0;
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
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
    }
}
