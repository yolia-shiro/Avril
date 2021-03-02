using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 巡逻状态
/// </summary>
public class PatrolState : EnemyBasicState
{
    public override void OnStart(NormalEnemy enemy)
    {
        enemy.PatrolPreparation();
    }

    public override void OnUpdate(NormalEnemy enemy)
    {
        enemy.Patrol();
    }
}
