using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础魔法
/// 包括魔法的共有属性
/// </summary>
public class BasicMagic : MonoBehaviour
{
    [Header("Effect")]
    public GameObject hitPrefab;
    public GameObject muzzlePrefab;
    public List<GameObject> trails;

    [Header("Shoot Offset")]
    public float accuracy;
    protected Vector3 offset;

    [Header("Shoot")]
    public float speed;     //发射速度
    public float duration;      //存在时间

    [Header("Value")]
    public float hit;

    [Header("impulse")]
    public float impulse;

    private void OnEnable()
    {
        CreateMuzzleEffect();
        InitShootOffset();
        StartCoroutine(DestroyAfterTime(duration));
    }

    public virtual void Start()
    { 
    
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //发射
        transform.position = transform.position + (transform.right + offset) * speed * Time.deltaTime;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
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
                    //施加冲量
                    Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 dir = (enemy.transform.position - transform.position).normalized;
                        rb.AddForce(impulse * dir, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    Debug.Log($"Magic Attack Failure ---- hit : {hit}");
                }
            }
            else if (enemy != null && enemy is Boss)
            {
                //Boss
                Boss boss = enemy as Boss;
                if (!boss.GetDamage(hit))
                {
                    Debug.Log($"Magic Attack Failure ---- hit : {hit}");
                }
            }
            CreateHitEffect(transform.position, Quaternion.identity);
        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            CreateHitEffect(transform.position, Quaternion.identity);
        }
    }

    //生成随机偏移
    public void InitShootOffset()
    {
        if (accuracy != 100)
        {
            accuracy = 1 - (accuracy / 100);

            for (int i = 0; i < 1; i++)
            {
                var val = 1 * Random.Range(-accuracy, accuracy);
                var index = Random.Range(0, 1);
                if (i == 0)
                {
                    if (index == 0)
                        offset = new Vector3(0, -val, 0);
                    else
                        offset = new Vector3(0, val, 0);
                }
                else
                {
                    offset = new Vector3(0, offset.y, 0);
                }
            }
        }
    }

    //创建起始特效
    public void CreateMuzzleEffect()
    {
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.right = gameObject.transform.right + offset;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    //创建结束的特效
    public void CreateHitEffect(Vector3 pos, Quaternion rot)
    {
        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot) as GameObject;

            hitVFX.transform.right = gameObject.transform.right;

            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null)
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        }
        StartCoroutine(DestroyParticle(0.0f));
    }

    //消除粒子效果
    public IEnumerator DestroyParticle(float waitTime)
    {

        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                tList.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }

    public IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
