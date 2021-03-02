using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitState : PlayerBasicState
{
    private Animator anim;

    public override void OnEnter(PlayerController player)
    {
        player.PlayerState.isHit = true;
        player.PlayerState.isJump = false;
        anim = player.GetComponent<Animator>(); 
    }

    public override void OnFixedUpdate(PlayerController player)
    {
        
    }

    public override void OnUpdate(PlayerController player)
    {
        if (anim.GetCurrentAnimatorStateInfo(4).IsName("Hit") && anim.GetCurrentAnimatorStateInfo(4).normalizedTime >= 0.9f)
        {
            //结束
            player.PlayerState.isHit = false;
            player.TranslateToState(player.lastPlayerState);
            return;
        }
    }
}
