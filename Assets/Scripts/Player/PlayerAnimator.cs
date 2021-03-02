using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;
    private PlayerController playerController;
    private Rigidbody2D myRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("speed", Mathf.Abs(playerController.xAxis));
        anim.SetBool("isJump", playerController.PlayerState.isJump);
        anim.SetBool("isGround", playerController.PlayerState.isGround);
        anim.SetFloat("velocity_y", myRigidbody.velocity.y);
        anim.SetFloat("hitKind", playerController.PlayerState.hitKind);
        anim.SetBool("isResonance", playerController.PlayerState.isMagicResonance);
    }
}
