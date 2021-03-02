using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBasicState
{
    private Animator anim;
    private Rigidbody2D myRigidbody;

    private float enterTime;

    public override void OnEnter(PlayerController player)
    {
        anim = player.GetComponent<Animator>();
        myRigidbody = player.GetComponent<Rigidbody2D>();
        myRigidbody.velocity = Vector2.zero;

        anim.SetTrigger("attack");
        player.PlayerState.isAttack = true;
        enterTime = Time.time;
    }

    public override void OnFixedUpdate(PlayerController player)
    {
        if (!player.PlayerState.isGround)
        {
            //下落攻击
            //myRigidbody.velocity = new Vector2(0, player.fallingAttackVelocity * player.fallingAttackCurve.Evaluate(Time.time - enterTime));
            myRigidbody.AddForce(new Vector2(0, player.fallingAttackForce), ForceMode2D.Impulse);
        }
        //未制作动画，之后更换为动画事件执行，此处仅进行测试
        if (anim.GetCurrentAnimatorStateInfo(3).IsName("Falling Buffer") && anim.GetCurrentAnimatorStateInfo(3).normalizedTime == 0.0f)
        {
            player.StartCoroutine(MainCamera.Instance.Shake(player.fallingAttackDuration, player.fallingAttackMigration));
        }
    }

    public override void OnUpdate(PlayerController player)
    {
        if (anim.GetCurrentAnimatorStateInfo(3).IsName("Attack State"))
        {
            //攻击动画结束
            Time.timeScale = 1.0f;
            player.TranslateToState(player.lastPlayerState);
            player.PlayerState.isAttack = false;
            return;
        }
    }
}
