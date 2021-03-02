using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家在休息点休息时的状态机
/// </summary>
public class PlayerSaveState : PlayerBasicState
{
    public override void OnEnter(PlayerController player)
    {
        player.SaveStatePreparation();
    }

    public override void OnFixedUpdate(PlayerController player)
    {
        
    }

    public override void OnUpdate(PlayerController player)
    {
        
    }
}
