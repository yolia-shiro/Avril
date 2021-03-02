using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StiffState : EnemyBasicState
{
    public override void OnStart(NormalEnemy enemy)
    {
        enemy.StiffPreparation();
    }

    public override void OnUpdate(NormalEnemy enemy)
    {
        enemy.Stiff();
    }
}
