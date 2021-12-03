using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : IFighter
{
    public MonsterData data;

    private float currentHealth;

    public Monster(MonsterData data)
    {
        this.data = data;
        currentHealth = data.health;
    }

    /* IFighter */
    public float GetCurrentLife()
    {
        return currentHealth;
    }

    public int GetMaxLife()
    {
        return data.health;
    }

    public int GetAttack()
    {
        return data.attack;
    }

    public int GetDefence()
    {
        return data.defence;
    }   

    public int GetSpeed()
    {
        return data.speed;
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
