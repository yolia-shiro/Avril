using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCannonCheckArea : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Magic Cannon Hit Player ------ Magic Cannon");
        }
    }
}
