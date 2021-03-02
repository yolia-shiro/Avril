using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
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
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && enemy is NormalEnemy)
            {
                //普通怪物
                NormalEnemy nEnemy = enemy as NormalEnemy;
                if (nEnemy.GetDamage(playerController.attackHitDamage))
                {
                    Time.timeScale = playerController.attackTimeScale;
                    Debug.Log($"Physic Attack Success ---- hit : {playerController.attackHitDamage}");
                }
                else
                {
                    //不生效，被反击
                    Debug.Log("近战攻击被反击");
                    playerController.Counterattacked();
                }
            }
            //else if(enemy != null && enemy is Boss)
            //{
            //    //Boss
            //    collision.GetComponent<WizardController>().OpenDefenseEffect(transform);
            //}
            StartCoroutine(MainCamera.Instance.Shake(playerController.attackDuration, playerController.attackMigration));
        }
    }
}
