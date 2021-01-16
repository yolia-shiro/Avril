using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDefaultState : PlayerMagicBasicState
{
    public override void OnEnter(MagicSystem magicSystem, PlayerController playerController)
    {
        if (playerController != null) 
        {
            playerController.isMagic = false;
            playerController.magicKind = -1;
        }
    }

    public override void OnUpdate(MagicSystem magicSystem, PlayerController playerController)
    {
        
    }
}
