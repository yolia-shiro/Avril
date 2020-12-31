using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("speed", Mathf.Abs(playerController.xAxis));
        anim.SetBool("isJump", playerController.isJump);
        anim.SetBool("isGround", playerController.isGround);
    }
}
