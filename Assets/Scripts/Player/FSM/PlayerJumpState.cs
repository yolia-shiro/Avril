using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBasicState
{
    private Animator anim;
    private Rigidbody2D myRigidbody;
    private bool isFirstJump;

    public override void OnEnter(PlayerController player)
    {
        anim = player.GetComponent<Animator>();
        myRigidbody = player.GetComponent<Rigidbody2D>();
        player.JumpPreparation();
        isFirstJump = true;
    }

    public override void OnFixedUpdate(PlayerController player)
    {
        //确保空中能够控制方向
        player.xAxis = Input.GetAxis("Horizontal");
        float speed = Mathf.Abs(player.xAxis) < 0.3 ? player.walkSpeed : player.runSpeed;
        player.Movement(speed);

        player.Jump();
    }

    public override void OnUpdate(PlayerController player)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Grab"))
        {
            player.PlayerState.isJump = false;
        }
        ////TODO: 跳跃过程中，点击攻击键
        //if (Input.GetButtonDown("Attack"))
        //{
        //    player.PlayerState.isJump = false;
        //    player.TranslateToState(player.playerAttackState);
        //    return;
        //}
        if (myRigidbody.velocity.y < 0)
        {
            //切换到下落动画
            player.PlayerState.isJump = false;
        }
        if (player.PlayerState.isGround && !isFirstJump && !anim.GetCurrentAnimatorStateInfo(0).IsName("Grab"))
        {
            player.PlayerState.isJump = false;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -0.11f);
            //跳跃状态结束
            player.TranslateToState(player.lastPlayerState);
            return;
        }
        if (isFirstJump)
        {
            isFirstJump = !isFirstJump;
        }
    }
}
