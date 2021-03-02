using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamageOnTrigger : MonoBehaviour
{
    [SerializeField]
    float damageValue;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().GetDamage(damageValue))
            {
                Debug.Log($"{name} Make Hit To {collision.name} : {damageValue}");
            }
        }
    }
}
