using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : NormalEnemy
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().GetDamage(attackValue[attackRandomIndex]))
            {
                //击中玩家
                Debug.Log($"{name} 击中 {collision.gameObject.name} ----- 伤害为 {attackValue[attackRandomIndex]}");
            }
        }
    }
}
