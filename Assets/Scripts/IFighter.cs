using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFighter
{
    int GetAttack();
    int GetDefence();
    int GetSpeed();

    int GetMaxLife();
    float GetCurrentLife();

    bool IsAlive();
    bool CanAct();

    void Attack(IFighter target);
    void TakeDamage(float damage);
}
