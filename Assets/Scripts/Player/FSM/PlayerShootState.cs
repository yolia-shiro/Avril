using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootState : PlayerBasicState
{
    private Animator anim;
    private Rigidbody2D myRigidbody;
    private BasicMagicSkill basicMagicSkill;
    public override void OnEnter(PlayerController player)
    {
        anim = player.GetComponent<Animator>();
        myRigidbody = player.GetComponent<Rigidbody2D>();
        myRigidbody.velocity = Vector2.zero;

        player.PlayerState.isMagic = true;
        basicMagicSkill = player.magicSkillLists[player.curMagicSkillIndex];
        
        if (player.haveStorage)
        {
            player.StorageMagicShoot();
        }
        else
        {
            player.NormalMagicShoot();
        }
        
    }

    public override void OnFixedUpdate(PlayerController player)
    {
        
    }

    public override void OnUpdate(PlayerController player)
    {
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Magic State"))
        {
            player.PlayerState.isMagic = false;
            player.TranslateToState(player.playerMoveState);
            return;
        }
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Storage Shoot"))
        {
            float time = anim.GetCurrentAnimatorStateInfo(1).normalizedTime;
            myRigidbody.velocity = basicMagicSkill.reactionVelocityCurve.Evaluate(time) * -player.transform.right * basicMagicSkill.reactionVelocity;
        }
    }
}
