using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : EnemyBasicState
{
    public override void OnStart(NormalEnemy enemy)
    {
        enemy.HitPreparation();
    }

    public override void OnUpdate(NormalEnemy enemy)
    {
        enemy.Hit();
    }
}
