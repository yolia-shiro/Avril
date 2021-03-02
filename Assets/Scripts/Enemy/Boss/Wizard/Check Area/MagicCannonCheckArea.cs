using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCannonCheckArea : MonoBehaviour
{
    [HideInInspector] public float damage;  //每次生效的伤害值

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().GetDamage(damage))
            {
                Debug.Log("Magic Cannon Hit Player ------ Magic Cannon");
            }
        }
    }
}
