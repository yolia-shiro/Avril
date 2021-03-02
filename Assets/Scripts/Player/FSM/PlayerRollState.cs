using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollState : PlayerBasicState
{
    private Animator anim;
    public override void OnEnter(PlayerController player)
    {
        anim = player.GetComponent<Animator>();
        anim.SetTrigger("roll");
        player.PlayerState.isRoll = true;
    }

    public override void OnFixedUpdate(PlayerController player)
    {
        player.Roll();
    }

    public override void OnUpdate(PlayerController player)
    {
        if (!anim.GetCurrentAnimatorStateInfo(2).IsName("Roll"))
        {
            player.PlayerState.isRoll = false;
            player.TranslateToState(player.lastPlayerState);
            return;
        }
    }
}
