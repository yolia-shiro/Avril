using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalEnemy : Enemy,IDamageable
{
    [Header("Color")]
    public float colorChangeSpeed;
    private float originColorAlpha;

    [Header("Search Area")]
    public Transform searchPoint;
    public float searchRadius;
    public LayerMask searchLayer;
    [HideInInspector] public Transform searchTarget;

    [Header("Usual Patrol")]
    public Transform leftBoundaryPatrol;
    public Transform rightBoundaryPatrol;
    private Transform curPatrolTarget;      //当前巡逻目标
    public float patrolSpeed;

    [Header("Idle")]
    public float idleDuration;
    private float curIdleDuration;

    [Header("Track")]
    public float trackSpeed;
    public float minDisToTarget;    //距离目标的最近接触距离，小于该值将切换至攻击状态
    public float yOffset;

    [Header("Attack")]
    public List<float> attackValue = new List<float>();
    public float attackIdleDuration;
    protected float curAttackIdleDuration;
    protected bool canAttack = true;
    public List<string> attackAnimName = new List<string>();    //保持trigger和动画机名称一致
    protected int attackRandomIndex;

    [Header("Stiff")]
    public float stiffDuration; //僵直持续时间
    protected float curStiffDuration;
    protected bool isStiff;

    //FSM
    protected EnemyBasicState enemyState;
    public PatrolState patrolState = new PatrolState();
    public TrackState trackState = new TrackState();
    public AttackState attackState = new AttackState();
    public HitState hitState = new HitState();
    public StiffState stiffState = new StiffState();

    //private Renderer[] allRenderers;
    private SpriteRenderer[] allSpriteRenderers;

    public override void Start()
    {
        base.Start();
        //allRenderers = GetComponentsInChildren<Renderer>();
        allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        TranslateToState(patrolState);
    }


    // Update is called once per frame
    public virtual void Update()
    {
        if (isDeath)
        {
            myRigidbody.gravityScale = 1.0f;
            GameManager.Instance.playerData.GetComponent<PlayerController>().RemoveLockTarget(transform);
            //逐渐消隐
            if (ColorGradualChange(allSpriteRenderers) < 0.1f)
            {
                gameObject.SetActive(false);
            }

            return;
        }
        ui.transform.right = Vector3.right;
        PhysicsCheck();
        enemyState.OnUpdate(this);
    }

    /// <summary>
    /// 颜色渐变消失(SpriteRenderer)
    /// </summary>
    /// <returns></returns>
    public float ColorGradualChange(SpriteRenderer[] allSpriteRednerers)
    {
        float res = 0;
        foreach (var renderer in allSpriteRednerers)
        {
            float a = renderer.color.a;
            a = Mathf.Lerp(a, 0, colorChangeSpeed * Time.fixedDeltaTime);
            res = a;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, a);
        }
        return res;
    }

    public void TranslateToState(EnemyBasicState state)
    {
        enemyState = state;
        state.OnStart(this);
    }

    #region 巡逻
    /// <summary>
    /// 寻找距离最远的巡逻边界
    /// </summary>
    public void FindMaxDisPatrolPoint()
    {
        if (Vector3.Distance(transform.position, leftBoundaryPatrol.position) < Vector3.Distance(transform.position, rightBoundaryPatrol.position))
        {
            curPatrolTarget = rightBoundaryPatrol;
        }
        else
        {
            curPatrolTarget = leftBoundaryPatrol;
        }
    }

    public virtual void PatrolPreparation()
    {
        FindMaxDisPatrolPoint();
        anim.SetBool("isPatrol", true);
    }
    public virtual void Patrol()
    {
        if (searchTarget != null)
        {
            //寻找到目标，切换到追踪状态
            TranslateToState(trackState);
            return;
        }
        if (Vector3.Distance(transform.position, curPatrolTarget.position) < 2.0f)
        {
            //开始待机计时
            anim.SetBool("isPatrol", false);
            curIdleDuration += Time.deltaTime;
            if (curIdleDuration >= idleDuration)
            {
                //待机结束，重置信息
                curIdleDuration = 0;
                anim.SetBool("isPatrol", true);
                //切换巡逻目标
                FindMaxDisPatrolPoint();
            }
            return;
        }
        transform.rotation = transform.position.x < curPatrolTarget.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        transform.position = Vector3.MoveTowards(transform.position, curPatrolTarget.position, patrolSpeed * Time.deltaTime);
    }
    #endregion

    #region 追踪
    public virtual void TrackPreparation() { }
    public virtual void Track()
    {
        if (searchTarget == null)
        {
            //目标丢失，切换回巡逻状态
            TranslateToState(patrolState);
            return;
        }
        if (Vector3.Distance(transform.position, searchTarget.position + new Vector3(0, yOffset, 0)) < minDisToTarget)
        {
            //切换到攻击状态
            anim.SetBool("isPatrol", false);
            TranslateToState(attackState);
            return;
        }
        anim.SetBool("isPatrol", true);
        transform.rotation = transform.position.x < searchTarget.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        transform.position = Vector3.MoveTowards(transform.position, searchTarget.transform.position + new Vector3(0, yOffset, 0), trackSpeed * Time.deltaTime);
    }
    #endregion

    #region 攻击
    public virtual void AttackPreparation()
    {
        if (!canAttack)
        {
            //攻击CD未结束
            TranslateToState(trackState);
            return;
        }
        curAttackIdleDuration = 0;
        attackRandomIndex = Random.Range(0, attackAnimName.Count);
        anim.SetTrigger(attackAnimName[attackRandomIndex]);
        //转向
        if (searchTarget != null)
        {
            transform.rotation = transform.position.x < searchTarget.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        }
    }
    public virtual void Attack()
    {
        if (!anim.GetCurrentAnimatorStateInfo(1).IsName(attackAnimName[attackRandomIndex]) ||
            (anim.GetCurrentAnimatorStateInfo(1).IsName(attackAnimName[attackRandomIndex]) && anim.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1.0f))
        {
            canAttack = false;
            //开启计时
            StartCoroutine(WaitAttackCd());
            TranslateToState(trackState);
        }
    }
    #endregion

    #region 受击
    public virtual void HitPreparation()
    {
        //清除计时数据，避免出现错时的问题
        curAttackIdleDuration = 0;
        curStiffDuration = 0;

        //播放动画
        anim.SetTrigger("hit");
    }
    public virtual void Hit()
    {
        if ((anim.GetCurrentAnimatorStateInfo(2).IsName("hit") && anim.GetCurrentAnimatorStateInfo(2).normalizedTime >= 0.9f))
        {
            //取消自身受到的冲击力
            myRigidbody.velocity = Vector2.zero;
            //返回跟踪状态
            TranslateToState(trackState);
            return;
        }
    }
    #endregion

    #region 僵直
    public virtual void StiffPreparation()
    {
        //清除计时数据，避免出现错时的问题
        curAttackIdleDuration = 0;
        curStiffDuration = 0;

        //播放僵直动画
        isStiff = true;
        anim.SetBool("isStiff", isStiff);
        originGravityScale = myRigidbody.gravityScale;
        myRigidbody.gravityScale = 1.0f;
    }
    public virtual void Stiff()
    {
        if (anim.GetCurrentAnimatorStateInfo(2).IsName("Hit State") && !isStiff)
        {
            //僵直结束
            myRigidbody.gravityScale = originGravityScale;
            TranslateToState(trackState);
            return;
        }
        curStiffDuration += Time.deltaTime;
        if (curStiffDuration >= stiffDuration)
        {
            //还原
            curStiffDuration = 0;
            curToughness = maxToughness;
            sliderToughness.value = maxToughness;
            isStiff = false;
            anim.SetBool("isStiff", isStiff);
        }
    }
    #endregion

    #region 格挡
    public virtual void ShieldPreparation() { }
    public virtual void Shield() { }

    #endregion

    public void PhysicsCheck()
    {
        //检测玩家
        Collider2D searchColl = Physics2D.OverlapCircle(searchPoint.position, searchRadius, searchLayer);
        searchTarget = searchColl == null ? null : searchColl.transform;
    }

    public IEnumerator WaitAttackCd()
    {
        yield return new WaitForSeconds(attackIdleDuration);
        canAttack = true;
    }

    public virtual bool GetDamage(float value)
    {
        ui.SetActive(true);
        ResumeHP(value);
        sliderHP.value = curHP;

        if (!(enemyState is StiffState))
        {
            ResumeToughness(value * (1 - resistance));
            sliderToughness.value = curToughness;
        }

        if (curHP <= 0)
        {
            isDeath = true;
            anim.SetBool("isDeath", isDeath);
            ui.SetActive(false);
            return true;
        }
        if (maxToughness > 0 && curToughness <= 0)
        {
            //进入僵直状态
            if (!(enemyState is StiffState))
            {
                TranslateToState(stiffState);
            }
            return true;
        }
        else if (maxToughness == 0)
        {
            sliderToughness.gameObject.SetActive(false);
        }
        //进入受击状态
        TranslateToState(hitState);
        //anim.SetTrigger("hit");
        return true;
    }

    public override void EndNotify()
    {
        base.EndNotify();
        //还原状态
        gameObject.SetActive(true);
        isDeath = false;
        anim.SetBool("isDeath", isDeath);
        isStiff = false;
        anim.SetBool("isStiff", isStiff);
        TranslateToState(patrolState);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(searchPoint.position, searchRadius);
    }
}
