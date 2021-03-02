using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWoodMagic : BasicMagic
{
    [Header("Exclusive")]
    public bool canHeal;    //能否治疗
    public float healValue;     //治疗量

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && enemy is NormalEnemy)
            {
                //普通怪物
                NormalEnemy nEnemy = enemy as NormalEnemy;
                if (nEnemy.GetDamage(hit))
                {
                    Debug.Log($"Magic Attack Success ---- hit : {hit}");
                    //施加冲量
                    Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 dir = (enemy.transform.position - transform.position).normalized;
                        rb.AddForce(impulse * dir, ForceMode2D.Impulse);
                    }
                    if (GameManager.Instance.playerData.GetComponent<PlayerController>().AddValueToHp(healValue))
                    {
                        Debug.Log($"Magic Attack Success ---- heal : {healValue}");
                    }
                }
            }
            else if (enemy != null && enemy is Boss)
            {
                //Boss
                Boss boss = enemy as Boss;
                if (!boss.GetDamage(hit))
                {   
                    //攻击失败
                    Debug.Log($"Magic Attack Failure ---- hit : {hit}");
                }
                else
                {
                    //回复HP
                    if (GameManager.Instance.playerData.GetComponent<PlayerController>().AddValueToHp(healValue))
                    {
                        Debug.Log($"Magic Attack Success ---- heal : {healValue}");
                    }
                }
            }
            CreateHitEffect(transform.position, Quaternion.identity);
        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            CreateHitEffect(transform.position, Quaternion.identity);
        }
    }

}
