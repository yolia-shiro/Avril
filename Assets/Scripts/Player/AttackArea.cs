using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public float attackDuration;
    public float attackMigration;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Attack Enemy");
            StartCoroutine(MainCamera.instance.Shake(attackDuration, attackMigration));
        }
    }
}
