using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSword : MonoBehaviour
{
    private float launchSpeed;
    private bool isLaunch;
    public float damage;

    [Header("Ground Check")]
    public Transform checkPoint;
    public float checkRadius;   //检测地面
    public LayerMask checkMask;

    [Header("Burst")]
    public float waitToBurstTime;   //等待爆炸的时间
    public GameObject burstEffect;
    public Transform burstPos;  //爆炸特效的位置
    public float burstRadius;   //爆炸半径
    public float burstDamge;    //爆炸伤害


    // Start is called before the first frame update
    void Start()
    {
        isLaunch = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLaunch)
        {
            transform.position = transform.position + (-transform.up) * launchSpeed * Time.deltaTime;
            if (Physics2D.OverlapCircle(checkPoint.position, checkRadius, checkMask))
            {
                isLaunch = false;
                //开始计时，准备爆炸
                StartCoroutine(BurstAfterTime());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLaunch)
        {
            if (collision.CompareTag("Player"))
            {
                if (collision.GetComponent<PlayerController>().GetDamage(damage))
                {
                    Debug.Log("Magic Sword Hit Player ----- Magic Sword");
                }
            }
        }
    }

    public void SetAttrToStartLaunch(float launchSpeed)
    {
        this.launchSpeed = launchSpeed;
        this.isLaunch = true;
    }

    public IEnumerator BurstAfterTime()
    {
        yield return new WaitForSeconds(waitToBurstTime);
        //爆炸
        Instantiate(burstEffect, burstPos.position, Quaternion.identity);
        foreach (var coll in Physics2D.OverlapCircleAll(burstPos.position, burstRadius)) 
        {
            if (coll.CompareTag("Player"))
            {
                if (coll.GetComponent<PlayerController>().GetDamage(burstDamge))
                {
                    Debug.Log("Magic Sword Burst Hit Player ----- Burst");
                }
            }
        }
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(checkPoint.position, checkRadius);   
        Gizmos.DrawWireSphere(burstPos.position, burstRadius);   
    }
}
