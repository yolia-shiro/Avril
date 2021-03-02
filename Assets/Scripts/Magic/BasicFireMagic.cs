using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFireMagic : BasicMagic
{
    [Header("Fire")]
    public float burnCumulateValue; //灼伤积累值
    public float burnValuePerSecond; //灼伤之后，每秒的灼伤值

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        //
        if (collision.CompareTag("Enemy"))
        {
            //调用受击对象的灼伤判断
            if (collision.GetComponent<Enemy>().CalCulateCumulateValue(burnCumulateValue, burnValuePerSecond))
            {
                Debug.Log("灼伤值积累");
            }
        }
    }
}
