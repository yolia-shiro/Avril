using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldState : EnemyBasicState
{
    public override void OnStart(NormalEnemy enemy)
    {
        enemy.ShieldPreparation();
    }

    public override void OnUpdate(NormalEnemy enemy)
    {
        enemy.Shield();
    }
}
