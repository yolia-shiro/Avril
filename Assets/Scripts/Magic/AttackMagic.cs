using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMagic : Magic
{
    [Header("Numerical Information")]
    public float damage;
    public float damageForce;

    [Header("Effective State")]
    //攻击魔法生效状态表现为向前(目标)移动
    public float speed;
    public float existTime;     //未命中目标时的存在时长

    [Header("Attack Magic Type")]
    public bool isNormal;
    public bool isDrag;
    public bool isTrack;

    [Header("Drag Message")]
    public float dragRadius;
    public float dragForce;
    public float dragEffectiveScale;    //牵引魔法生效时的特效半径
    private bool isDragEffective = false;

    [Header("Track Message")]
    public Transform trackTarget;
    private bool haveFindTrackTarget = false;
    public float trackOffset;       //trackDelta, MoveTowards用

    [Header("Spell Burst")]
    //魔力爆发
    public float burstRadius;
    public float burstDamage;
    public float burstForce;
    public GameObject MagicBurstEffect;

    [Header("Weapon Attach")]
    //魔力附着
    [Range(0, 1)] public float attachValuePerMagic; //每次附加值
    [HideInInspector] public float curAttachValue; //当前武器魔法附着值
    public float attachLossRate; //魔力附着流逝速度
    
    [Header("Launch Offset")]       //发射偏移
    [Tooltip("From 0% to 100%")]    
    public float accuracy;
    public float fireRate;
    private Vector3 offset;

    [Header("Effect Prefabs")]
    public GameObject muzzlePrefab;
    public List<GameObject> trails;
    private bool muzzleCreated = false;

    public override void Start()
    {
        base.Start();
        //产生发射偏移
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == MagicState.Attachment)
        {
            //附着状态不执行碰撞器函数的内容
            return;
        }
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

    //附着状态下逐渐消耗附着值
    public override void Attachment()
    {
        curAttachValue -= attachLossRate * Time.deltaTime;
        if (curAttachValue <= 0) 
        {
            curAttachValue = 0;
            //魔力附着结束，移除当前脚本
            Destroy(this);
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
            //牵引生效时，不向前移动
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

    //武器附魔相关属性赋值
    public void SetWeaponAttachAttr(AttackMagic attackMagic)
    {
        attachLossRate = attackMagic.attachLossRate;
    }

    //魔力爆发相关属性赋值
    public void SetMagicBurstAttr(AttackMagic attackMagic)
    {
        //
        isNormal = attackMagic.isNormal;
        isDrag = attackMagic.isDrag;
        isTrack = attackMagic.isTrack;
        //
        burstRadius = attackMagic.burstRadius;
        burstDamage = attackMagic.burstDamage;
        burstForce = attackMagic.burstForce;
        MagicBurstEffect = attackMagic.MagicBurstEffect;
    }

    //魔力爆发
    public void MagicBurst()
    {
        CreateBurstEffect();
        Debug.Log("处于魔力爆发范围内的可攻击目标");
        foreach (var collider in Physics2D.OverlapCircleAll(transform.position, burstRadius))
        {
            if (collider.CompareTag("Enemy"))
            {
                Debug.Log("Hit Obejct : " + collider.gameObject.name + ", Damage : " + burstDamage * curAttachValue + ", Force : " + burstForce);
                Vector3 forceDir = collider.transform.position - transform.position;
                collider.GetComponent<Rigidbody2D>().AddForce(forceDir * burstForce, ForceMode2D.Impulse);
            }
        }
        //魔力爆发结束后销毁当前脚本
        Destroy(this);
    }

    //创建魔力爆发特效
    public void CreateBurstEffect() 
    {
        if (MagicBurstEffect != null)
        {
            var VFX = Instantiate(MagicBurstEffect, transform.position, Quaternion.identity) as GameObject;

            var ps = VFX.GetComponent<ParticleSystem>();
            if (ps == null)
            {
                var psChild = VFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(VFX, psChild.main.duration);
            }
            else
                Destroy(VFX, ps.main.duration);
        }
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


    public int GetMagicType() 
    {
        int magicType = 0;
        BitArray bitArray = new BitArray(3);
        bitArray.Set(0, isNormal);
        bitArray.Set(1, isDrag);
        bitArray.Set(2, isTrack);

        for (int i = 2; i >= 0; i--)
        {
            magicType = (magicType << 1) + System.Convert.ToInt32(bitArray.Get(i));
        }
        return magicType;
    }

    //Scene辅助函数
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, radius);
        Gizmos.DrawWireSphere(this.transform.position, dragRadius);
        //魔力爆发范围
        Gizmos.DrawWireSphere(this.transform.position, burstRadius);
    }
}
