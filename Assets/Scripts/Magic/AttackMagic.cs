using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMagic : Magic
{
    [Header("Numerical Information")]
    public float damage;
    public float damageForce;

    [Header("Effective State")]
    public float speed;
    public float existTime;

    [Header("Attack Magic Type")]
    public bool isNormal;
    public bool isDrag;
    public bool isTrack;

    [Header("Drag Message")]
    public float dragRadius;
    public float dragForce;
    public float dragEffectiveScale;
    private bool isDragEffective = false;

    [Header("Track Message")]
    public Transform trackTarget;
    private bool haveFindTrackTarget = false;
    public float trackOffset;

    [Tooltip("From 0% to 100%")]
    public float accuracy;
    public float fireRate;

    public GameObject muzzlePrefab;
    //public GameObject hitPrefab;
    public List<GameObject> trails;
    private bool muzzleCreated = false;

    private Vector3 offset;

    public override void Start()
    {
        base.Start();
        //used to create a radius for the accuracy and have a very unique randomness
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

    public override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (isDrag && !isDragEffective)
            {
                //牵引魔法生效
                transform.localScale *= dragEffectiveScale;
                isDragEffective = true;
            }
            if (isNormal || isTrack)
            {
                //造成伤害
                Debug.Log("Hit ++++ Damage : " + damage.ToString());
                if (!isDrag)
                {
                    collision.GetComponent<Rigidbody2D>().AddForce(transform.right * damageForce, ForceMode2D.Impulse);
                    //产生碰撞效果
                    CreateHitEffect(transform.position, Quaternion.identity);
                }
            }
        }
    }

    public override void Effective()
    {
        CreateMuzzleEffect();
        //消失计时开始
        StartCoroutine(WaitForDestory());
        Forward();
        //根据类型确定发射时的功能
        if (isNormal)
        {
            //正常类型
        }
        if (isDrag)
        {
            //牵引类型
            if (isDragEffective)
            {
                Drag();
            }
        }
        if (isTrack)
        {
            //追踪类型
            GetAttackTargetInCameraArea();
            if (trackTarget != null)
            {
                Track();
            }
        }
    }

    //设置状态
    public void SetAttackMagicType(int type)
    {
        //BitArray
        BitArray bitArray = new BitArray(3);
        for (int i = 0; i < 3; i++) 
        {
            bitArray.Set(i, (type & 1) != 0);
            type >>= 1;
        }

        isNormal = bitArray.Get(0);
        isDrag = bitArray.Get(1);
        isTrack = bitArray.Get(2);
    }

    //开始飞行
    public void Forward()
    {
        if (isDragEffective)
        {
            return;
        }
        transform.position = transform.position + (transform.right + offset) * speed * Time.deltaTime;
    }

    //范围牵引
    public void Drag()
    {
        Collider2D[] objsInBoundary = Physics2D.OverlapCircleAll(transform.position, dragRadius);
        foreach (var obj in objsInBoundary) 
        {
            if (obj.CompareTag("Enemy"))
            {
                Vector3 dragDir = transform.position - obj.transform.position;
                obj.GetComponent<Rigidbody2D>().AddForce(dragDir * dragForce * Vector3.Magnitude(transform.localScale), ForceMode2D.Force);
            }
        }
    }

    //获取摄像机范围内离魔法最近的Enemy对象
    public void GetAttackTargetInCameraArea()
    {
        if (haveFindTrackTarget) return;
        float orthograhicSize = Camera.main.orthographicSize;
        float halfWidth = orthograhicSize * ((float)Screen.width / (float)Screen.height);
        Vector2 leftTop = new Vector2(Camera.main.transform.position.x - halfWidth, Camera.main.transform.position.y + orthograhicSize);
        Vector2 rightBottom = new Vector2(Camera.main.transform.position.x + halfWidth, Camera.main.transform.position.y - orthograhicSize);
        Collider2D[] viewObjs = Physics2D.OverlapAreaAll(leftTop, rightBottom);
        //获取距离魔法最近的Enemy
        float minDis = -1.0f;
        foreach (var viewObj in viewObjs)
        {
            if (viewObj.CompareTag("Enemy"))
            {
                if (minDis < 0.0f)
                {
                    minDis = Vector3.Distance(transform.position, viewObj.transform.position);
                    trackTarget = viewObj.transform;
                }
                else if (Vector3.Distance(transform.position, viewObj.transform.position) < minDis)
                {
                    minDis = Vector3.Distance(transform.position, viewObj.transform.position);
                    trackTarget = viewObj.transform;
                }
            }
        }
        haveFindTrackTarget = true;
    }

    //魔法追踪
    public void Track()
    {
        if (trackTarget == null || this == null)
        {
            return;
        }
        Vector3 direction = (trackTarget.position - transform.position).normalized;
        transform.right = Vector3.MoveTowards(transform.right, direction, trackOffset * Time.deltaTime);
    }

    //创建起始特效
    public void CreateMuzzleEffect()
    {
        if (muzzleCreated) return;
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward + offset;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
        muzzleCreated = true;
    }

    ////创建结束的特效
    //public void CreateHitEffect(Vector3 pos, Quaternion rot)
    //{
    //    if (hitPrefab != null)
    //    {
    //        var hitVFX = Instantiate(hitPrefab, pos, rot) as GameObject;

    //        var ps = hitVFX.GetComponent<ParticleSystem>();
    //        if (ps == null)
    //        {
    //            var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
    //            Destroy(hitVFX, psChild.main.duration);
    //        }
    //        else
    //            Destroy(hitVFX, ps.main.duration);
    //    }
    //    StartCoroutine(DestroyParticle(0.0f));
    //}

    ////消除粒子效果
    //public IEnumerator DestroyParticle(float waitTime)
    //{

    //    if (transform.childCount > 0 && waitTime != 0)
    //    {
    //        List<Transform> tList = new List<Transform>();

    //        foreach (Transform t in transform.GetChild(0).transform)
    //        {
    //            tList.Add(t);
    //        }

    //        while (transform.GetChild(0).localScale.x > 0)
    //        {
    //            yield return new WaitForSeconds(0.01f);
    //            transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    //            for (int i = 0; i < tList.Count; i++)
    //            {
    //                tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    //            }
    //        }
    //    }

    //    yield return new WaitForSeconds(waitTime);
    //    Destroy(gameObject);
    //}

    //攻击通用
    //发射之后自动销毁
    public IEnumerator WaitForDestory()
    {
        yield return new WaitForSeconds(existTime);
        if (this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }


    //Scene辅助函数
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, radius);
        Gizmos.DrawWireSphere(this.transform.position, dragRadius);
    }
}
