using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingAttackArea : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.GetComponent<WizardController>().GetDamage(playerController.fallingAttackHitDamage))
            {
                Debug.Log("Attack Enemy ------ Falling Attack");
            }
        }
    }
}
