using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 追踪状态
/// </summary>
public class TrackState : EnemyBasicState
{
    public override void OnStart(NormalEnemy enemy)
    {
        enemy.TrackPreparation();
    }

    public override void OnUpdate(NormalEnemy enemy)
    {
        enemy.Track();
    }
}
