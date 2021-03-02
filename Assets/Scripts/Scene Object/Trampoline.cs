using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField]
    float propultionForce;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRigidbody = collision.collider.GetComponent<Rigidbody2D>();
            playerRigidbody.AddForce(collision.transform.up * propultionForce, ForceMode2D.Impulse);

            PlayerController playerController = collision.collider.GetComponent<PlayerController>();
            playerController.TranslateToState(playerController.playerJumpState);
        }
        animator.SetTrigger("boing");
    }
}
