using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击状态
/// </summary>
public class AttackState : EnemyBasicState
{
    public override void OnStart(NormalEnemy enemy)
    {
        enemy.AttackPreparation();
    }

    public override void OnUpdate(NormalEnemy enemy)
    {
        enemy.Attack();
    }
}
