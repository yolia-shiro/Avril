using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MagicType { 
    Nihility = 0,
    Fire = 1,
    Water = 2,
    Wood = 3
}

public class PlayerController : MonoBehaviour,IDamageable
{
    private Rigidbody2D myRigidbody;
    private Animator anim;
    private PlayerData playerData;
    private PlayerState playerState;
    public PlayerData PlayerData { get { return playerData; } }
    public PlayerState PlayerState { get { return playerState; } }
    private SpriteRenderer []spriteRenderers;

    [HideInInspector] public float xAxis;
    private float originGravityScale;

    [Header("Physic Material")]
    public PhysicsMaterial2D physicMaterial;

    [Header("Walk")]
    public float walkSpeed;

    [Header("Run")]
    public float runSpeed;

    [Header("Jump")]
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float jumpForce;

    [Header("Grab")]
    public Collider2D grabCollider;
    public float grabDiff;      //上限到下限的差值
    public Vector2 grabOffset;
    public float grabSpeed;
    public bool isGrab;

    [Header("Roll")]
    public AnimationCurve rollCurve;
    public float rollBasicVelocity;

    [Header("Attack")]
    public float attackTimeScale;   //用于卡肉的实现
    public float attackDuration;
    public float attackMigration;
    public float attackHitDamage;

    [Header("Falling Attack")]
    public float fallingAttackDuration;
    public float fallingAttackMigration;
    public float fallingAttackHitDamage;
    public float fallingAttackForce;
    //public float fallingAttackVelocity;
    //public AnimationCurve fallingAttackCurve;

    [Header("Magic Resonance")]
    public MagicType wantResonanceType = MagicType.Fire; //想要去共鸣的元素类型
    //public MagicType curResonanceType;     //当前共鸣元素的类型
    public Vector3 toCenterOffset;  //到中心的偏移
    public GameObject resonanceLight;

    [Header("Magic Skill")]
    public Transform releasePos;
    public MagicSkillManager magicSkillManager;
    public MagicType curMaigcType;     //当前魔法类别
    [HideInInspector] public List<BasicMagicSkill> magicSkillLists = new List<BasicMagicSkill>();     //当前魔法类别下佩戴的魔法
    [HideInInspector] public int curMagicSkillIndex = 0;

    [Header("Normal Magic")]
    public int normalMagicNums;
    private int normalMagicIndex = 0;

    [Header("Storage Magic")]   //蓄力魔法
    public float resumeMagicPerFrame;
    public float curResumeMagic;   //当前蓄力已经消耗的魔力
    public Transform storagePos;    //储存位置
    [HideInInspector] public bool haveStorage;
    public float storageShakeDuration;
    public float storageShakeMigration;

    [Header("Ground Check")]
    public Transform bottomcCheckPoint;
    public Vector2 bottomCheckSize;
    public Collider2D gradCheck;
    public LayerMask groundLayer;

    [Header("Hit")]
    public float seriousHitInvincibleTime;     //大硬直受击后的无敌时间
    public float minSwitchHitValue;
    private Color oldSpriteColor;   //原来的精灵图层颜色
    public float twinkleIntervalTime;      //闪烁间隔时间
    public float twinkleSpeed;
    public Color hitInvincibleTwinkleColor;     //无敌期间的闪烁色
    private Color twinkleTargetColor;


    [Header("Lock")]
    public List<Transform> enemysInCamera = null;
    public Transform curLockEnemy;
    public int curLockIndex;
    public bool isLock = false;


    [Header("Platform")]
    public float platformRestoreTime;   //单向平台恢复时间

    //攻击是否生效
    public bool isAttackEffective = true;

    //FSM
    private PlayerBasicState playerBasicState;
    public PlayerBasicState lastPlayerState = null;
    public PlayerMoveState playerMoveState = new PlayerMoveState();
    public PlayerJumpState playerJumpState = new PlayerJumpState();
    public PlayerRollState playerRollState = new PlayerRollState();
    public PlayerAttackState playerAttackState = new PlayerAttackState();
    public PlayerResonanceState playerResonanceState = new PlayerResonanceState();
    public PlayerShootState playerShootState = new PlayerShootState();
    public PlayerStorageShootState playerStorageShootState = new PlayerStorageShootState();
    public PlayerHitState playerHitState = new PlayerHitState();
    public PlayerSaveState playerSaveState = new PlayerSaveState();

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerData = GetComponent<PlayerData>();
        playerState = GetComponent<PlayerState>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        oldSpriteColor = spriteRenderers[0].color;

        originGravityScale = myRigidbody.gravityScale;
    }

    private void OnEnable()
    {
        //注册用户数据
        GameManager.Instance.RegisterPlayerData(playerData);
    }

    // Start is called before the first frame update
    void Start()
    {
        //获取数据
        SaveManager.Instance.LoadPlayerData();
        //transform.position = new Vector3(SaveManager.Instance.PlayerPositionX, SaveManager.Instance.PlayerPositionY, 0);

        magicSkillLists = magicSkillManager.allMagicSkills[playerData.CurResonanceType];

        TranslateToState(playerMoveState);
        StartCoroutine(RestoreMagic());
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(playerBasicState);
        if (playerState.isDead)
        {
            //玩家死亡，进行广播
            //GameManager.Instance.NotifyObservers();
            ////释放共鸣的元素
            //ElementRelease();
            ////关闭锁定
            //if (isLock)
            //{
            //    LockEnemy();
            //}
            ////停止线程
            //StopAllCoroutines();
            ////读取存档
            //GameManager.Instance.Reset();
            ////重置Player信息
            //ResumePlayerDataToDefault();
            ////还原
            //TranslateToState(playerMoveState);
            //StartCoroutine(RestoreMagic());

            //playerData.isDead = false;

            return;
        }
        InputCheck();
        playerBasicState.OnUpdate(this);
    }

    private void FixedUpdate()
    {
        if (playerState.isDead)
        {
            return;
        }
        //解决斜坡下滑和卡墙问题
        if (!playerState.isGround)
        {
            myRigidbody.sharedMaterial = physicMaterial;
        }
        else
        {
            myRigidbody.sharedMaterial = null;
        }
        //
        PhysicCheck();
        if (myRigidbody.velocity.y < 0)
        {
            //下落
            myRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        playerBasicState.OnFixedUpdate(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerBasicState is PlayerResonanceState && 
            collision.gameObject.layer == LayerMask.NameToLayer("Element") && 
            playerData.CurResonanceValue < playerData.MaxResonanceValue)
        {
            BasicElementSprite basicElementSprite = collision.GetComponent<BasicElementSprite>();
            if ((MagicType)playerData.CurResonanceType == MagicType.Nihility || (MagicType)playerData.CurResonanceType == wantResonanceType)
            {
                if ((MagicType)playerData.CurResonanceType == MagicType.Nihility)
                {
                    playerData.CurResonanceType = (int)wantResonanceType;
                    magicSkillLists = magicSkillManager.allMagicSkills[playerData.CurResonanceType];
                }
                playerData.CurResonanceValue += basicElementSprite.elementValue;
                //更新UI
                //UIManager.instance.UpdateResonance(curResonanceType, curResonanceValue);
            }

            //共鸣成功，将元素精灵的数据从ElementSpriteManager中移除
            ElementSpriteManager.Instance.RemoveElementSpriteData(basicElementSprite);
            //销毁共鸣成功的元素精灵
            Destroy(basicElementSprite.gameObject);
        }
    }

    /// <summary>
    /// 输入检测
    /// </summary>
    public void InputCheck()
    {
        if (Input.GetButton("Switch Resonance Type"))
        {
            UIManager.Instance.SwitchResonanceMenu(true);
        }
        else if (Input.GetButtonUp("Switch Resonance Type"))
        {
            wantResonanceType = UIManager.Instance.ReturnSelectedResonanceType();
            UIManager.Instance.SwitchResonanceMenu(false);
        }

        if (Input.GetButtonDown("Lock Mode"))
        {
            LockEnemy();
        }
        if (isLock && Input.GetButtonDown("Switch Lock Target"))
        {
            ChangeLockTarget();
        }
    }

    public IEnumerator UpdatePlatform(PlatformEffector2D platform)
    {
        grabCollider.enabled = false;
        playerState.isDownPlatform = true;
        platform.colliderMask ^= 1 << LayerMask.NameToLayer("Player");
        platform.colliderMask ^= 1 << LayerMask.NameToLayer("Grab");
        
        yield return new WaitForSeconds(platformRestoreTime);

        platform.colliderMask |= 1 << LayerMask.NameToLayer("Player");
        platform.colliderMask |= 1 << LayerMask.NameToLayer("Grab");
        playerState.isDownPlatform = false;
        grabCollider.enabled = true;
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="state"></param>
    public void TranslateToState(PlayerBasicState state)
    {
        if (state == playerBasicState)
        {
            return;
        }
        if ((state is PlayerHitState || state is PlayerAttackState) 
            && !(playerBasicState is PlayerMoveState || playerBasicState is PlayerStorageShootState))
        {
            //如果是受伤状态|攻击状态，上一状态只能是Move状态或者Sotrage状态
            lastPlayerState = playerMoveState;
        }
        else
        {
            lastPlayerState = playerBasicState;
        }
        playerBasicState = state;
        playerBasicState.OnEnter(this);
    }

    #region 移动
    public void Movement(float speed) 
    {
        if (xAxis != 0) 
        {
            //翻转
            transform.rotation = xAxis < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0); 
        }

        myRigidbody.velocity = new Vector2(speed * xAxis, myRigidbody.velocity.y);
    }
    #endregion

    #region 跳跃
    public void JumpPreparation()
    {
        playerState.isJump = true;
        playerState.isGround = false;
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0);
        myRigidbody.velocity += Vector2.up * jumpForce;


        //playerState.isGround = false;
    }
    public void Jump()
    {
        #region
        //if (myRigidbody.velocity.y < 0)
        //{
        //    //下落
        //    myRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        //}
        //else 
        #endregion
        if (myRigidbody.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            myRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

    }
    #endregion

    #region 翻滚
    public void Roll()
    {
        if (playerState.isRoll && anim.GetCurrentAnimatorStateInfo(2).IsName("Roll"))
        {
            float xVelocity = rollCurve.Evaluate(anim.GetCurrentAnimatorStateInfo(2).normalizedTime);
            myRigidbody.velocity = new Vector2((transform.right.x < 0 ? -1 : 1) * xVelocity * rollBasicVelocity, myRigidbody.velocity.y);
        }
    }
    #endregion

    #region 抓取墙壁边缘
    public IEnumerator Grab(Vector3 targetPos) 
    {
        if (!isGrab)
        {
            isGrab = true;
            myRigidbody.gravityScale = 0.0f;
            myRigidbody.velocity = Vector2.zero;    //取消跳跃的速度
            anim.SetTrigger("grab");
            yield return null;  //等待下一帧进入动画
            while (Vector3.Distance(transform.position, targetPos) > 0.1f && anim.GetCurrentAnimatorStateInfo(0).IsName("Grab"))
            {
                //float y = Mathf.MoveTowards(transform.position.y, targetPos.y, grabSpeed * Time.deltaTime);
                //transform.position = new Vector3(transform.position.x, y, transform.position.z) ;
                transform.position = Vector3.MoveTowards(transform.position, 
                    new Vector3(targetPos.x, targetPos.y, transform.position.z), 
                    grabSpeed * Time.deltaTime);
                yield return null;
            }
            isGrab = false;
            myRigidbody.gravityScale = originGravityScale;
        }
    }
    #endregion

    #region 元素共鸣
    //元素共鸣
    public void MagicResonance()
    {
        playerState.isMagicResonance = true;
        resonanceLight.SetActive(true);
        myRigidbody.velocity = Vector2.zero;
        xAxis = 0;

        //创建共鸣元素的实体
        ElementSpriteManager.Instance.CreateElementSpriteGameObjectInResonanceArea(
            transform.position + toCenterOffset,
            PlayerData.ResonanceRadius,
            wantResonanceType);
        //进行移动广播
        ElementSpriteManager.Instance.MoveNotify(transform.position + toCenterOffset);
    }

    //元素共鸣结束
    public void MagicResonanceOver()
    {
        //进行结束共鸣广播
        ElementSpriteManager.Instance.EndNotify();
        
        resonanceLight.SetActive(false);
    }

    /// <summary>
    /// 元素释放,在共鸣范围内随机生成当前元素精灵
    /// </summary>
    public void ElementRelease()
    {
        if ((MagicType)playerData.CurResonanceType == MagicType.Nihility)
        {
            return;
        }
        //元素释放
        ElementSpriteManager.Instance.CreateTypeElementSpriteInResonanceArea((MagicType)playerData.CurResonanceType, transform.position + toCenterOffset, 
                                                                        playerData.ResonanceRadius, playerData.CurResonanceValue);
        playerData.CurResonanceType = (int)MagicType.Nihility;
        playerData.CurResonanceValue = 0;
        magicSkillLists = magicSkillManager.allMagicSkills[playerData.CurResonanceType];
    }
    #endregion

    #region 魔法攻击
    //普通魔法攻击
    public void NormalMagicShoot()
    {
        if (!canMagic(-magicSkillLists[curMagicSkillIndex].resumeMPInNormal))
        {
            //魔力或者共鸣值不足
            return;
        }
        anim.SetTrigger("shoot");
        anim.SetBool("storaged", haveStorage);
        anim.SetFloat("normalMagicIndex", normalMagicIndex);
        normalMagicIndex = (normalMagicIndex + 1) % normalMagicNums;
        if (isLock && curLockEnemy != null)
        {
            //释放魔法的位置到锁定的敌人的方向
            //转向敌人位置
            transform.rotation = transform.position.x < curLockEnemy.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            Vector3 dir = curLockEnemy.position - releasePos.position;
            magicSkillLists[curMagicSkillIndex].NormalShoot(releasePos.position, dir);
        }
        else
        {
            magicSkillLists[curMagicSkillIndex].NormalShoot(releasePos.position, transform.right);
        }
        //消耗魔力
        MagicResume(-magicSkillLists[curMagicSkillIndex].resumeMPInNormal);
    }
    
    //蓄力魔法攻击
    public void StorageMagicShoot()
    {
        anim.SetTrigger("shoot");
        BasicMagicSkill basicMagicSkill = magicSkillLists[curMagicSkillIndex];
        basicMagicSkill.StorageShoot(releasePos.position, transform.right);
        StartCoroutine(MainCamera.Instance.Shake(storageShakeDuration, storageShakeMigration));
        //
        ClearStoragePos();
    }

    //消除蓄力魔法的标识
    public void ClearStoragePos()
    {
        haveStorage = false;
        for (int i = storagePos.childCount - 1; i >= 0; i--)
        {
            Destroy(storagePos.GetChild(i).gameObject);
        }
    }

    public bool IsStorageMagicOver()
    {
        if (curResumeMagic >= magicSkillLists[curMagicSkillIndex].resumeMPInStorage)
        {
            haveStorage = true;
            curResumeMagic = 0;
            return true;
        }
        return false;
    }
    #endregion

    #region 休息 | 存档
    public void SaveStatePreparation()
    {
        myRigidbody.velocity = Vector2.zero;
        //TODO: 播放休息动画

    }

    public void SaveState()
    { 
        
    }
    #endregion

    public void Counterattacked()
    {
        anim.SetTrigger("counterattacked");
    }

    private void PhysicCheck()
    {
        playerState.isGround = Physics2D.OverlapBox(bottomcCheckPoint.position, bottomCheckSize, 0, groundLayer);
    }


    public IEnumerator SeriousHitInvincibleOverAfterTime()
    {
        yield return new WaitForSeconds(seriousHitInvincibleTime);
        isAttackEffective = true;
    }

    public IEnumerator TwinkleColor()
    {
        twinkleTargetColor = hitInvincibleTwinkleColor;
        while (!isAttackEffective) 
        {
            while (spriteRenderers.Length > 0 && spriteRenderers[0].color != twinkleTargetColor)
            {
                foreach (var spriteRender in spriteRenderers)
                {
                    spriteRender.color = Color.Lerp(spriteRender.color, twinkleTargetColor, twinkleSpeed * Time.deltaTime);
                }
                yield return null;
            }
            //yield return new WaitForSeconds(twinkleIntervalTime);
            twinkleTargetColor = twinkleTargetColor == oldSpriteColor ? hitInvincibleTwinkleColor : oldSpriteColor;
        }
        foreach (var spriteRender in spriteRenderers)
        {
            spriteRender.color = oldSpriteColor;
        }
    }

    //动画事件
    //大硬直受伤结束
    public void SeriousHitEnd() 
    {
        isAttackEffective = false;
        StartCoroutine(SeriousHitInvincibleOverAfterTime());
        StartCoroutine(TwinkleColor());
    }

    //动画事件(翻滚) 
    //无敌帧生效
    public void RollInvincibleStart()
    {
        isAttackEffective = false;
    }
    //无敌帧结束
    public void RollInvincibleEnd()
    {
        isAttackEffective = true;
    }
    /// <summary>
    /// 动画事件 死亡结束
    /// </summary>
    public void DeathAnimatorEvent()
    {
        //TODO: 屏幕淡出

        //加载存档
        SceneController.Instance.TransitionToLoadGame();
    }


    #region 锁定
    //锁定开关
    public void LockEnemy()
    {
        isLock = !isLock;
        UIManager.Instance.SetLockModeActive(isLock);
        if (isLock)
        {
            if (enemysInCamera.Count != 0)
            {
                curLockEnemy = enemysInCamera[0];
                curLockIndex = 0;
                curLockEnemy.GetComponent<Enemy>().SetLockFlagActive(true);
            }
        }
        else
        {
            if (curLockEnemy != null) 
            {
                //关闭锁定标记
                curLockEnemy.GetComponent<Enemy>().SetLockFlagActive(false);
            }
        }
    }

    //切换锁定目标
    public void ChangeLockTarget()
    {
        //关闭上一个锁定标志
        if(curLockEnemy != null)
        {
            curLockEnemy.GetComponent<Enemy>().SetLockFlagActive(false);
        }

        curLockIndex = (curLockIndex + 1) % enemysInCamera.Count;
        curLockEnemy = enemysInCamera[curLockIndex];
        curLockEnemy.GetComponent<Enemy>().SetLockFlagActive(true);
    }

    //添加锁定目标
    public void AddLockTarget(Transform addTF)
    {
        if (!enemysInCamera.Contains(addTF))
        {
            enemysInCamera.Add(addTF);
            if (isLock && curLockEnemy == null)
            {
                curLockIndex = (curLockIndex + 1) % enemysInCamera.Count;
                curLockEnemy = enemysInCamera[curLockIndex];
                curLockEnemy.GetComponent<Enemy>().SetLockFlagActive(true);
            }
        }
    }

    //清除锁定目标
    public void RemoveLockTarget(Transform removeTF)
    {
        if (enemysInCamera.Contains(removeTF))
        {
            enemysInCamera.Remove(removeTF);
            if (isLock && removeTF == curLockEnemy) 
            {
                //关闭上一个锁定标志
                curLockEnemy.GetComponent<Enemy>().SetLockFlagActive(false);

                curLockEnemy = enemysInCamera.Count == 0 ? null : enemysInCamera[0];
                if (curLockEnemy != null)
                {
                    curLockEnemy.GetComponent<Enemy>().SetLockFlagActive(true);
                }  
            }
        }
    }
    #endregion

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bottomcCheckPoint.position, bottomCheckSize);
        Gizmos.DrawWireCube(gradCheck.bounds.center, gradCheck.bounds.size);
        if (playerData != null)
        {
            Gizmos.DrawWireSphere(transform.position + toCenterOffset, playerData.ResonanceRadius);
        }
    }


    public bool GetDamage(float value)
    {
        if (!isAttackEffective)
        {
            return false;
        }
        playerData.CurHP -= value;
        if (playerData.CurHP <= 0) 
        {
            playerData.CurHP = 0;
            playerState.isDead = true;
            //播放死亡动画
            anim.SetTrigger("death");
            return true;
        }
        //播放受击动画
        anim.SetTrigger("hit");
        if (value < minSwitchHitValue)
        {
            playerState.hitKind = 0;
        }
        else
        {
            playerState.hitKind = 1;
            isAttackEffective = false;
            TranslateToState(playerHitState);
        }
        return true;
    }

    /// <summary>
    /// 判断能否进行施法
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool canMagic(float value)
    {
        if(playerData.CurMP < Mathf.Abs(value) || ((MagicType)playerData.CurResonanceType != MagicType.Nihility && playerData.CurResonanceValue < Mathf.Abs(value)))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 同步释放魔法消耗
    /// </summary>
    /// <returns></returns>
    public bool MagicResume(float value)
    {
        playerData.CurMP += value;
        if ((MagicType)playerData.CurResonanceType != MagicType.Nihility)
        {
            playerData.CurResonanceValue += value;
            if (playerData.CurResonanceValue == 0)
            {
                //当前共鸣元素转换为无属性
                playerData.CurResonanceType = (int)MagicType.Nihility;
                magicSkillLists = magicSkillManager.allMagicSkills[playerData.CurResonanceType];
            }
        }
        return true;
    }

    /// <summary>
    /// 恢复魔力
    /// </summary>
    /// <returns></returns>
    public IEnumerator RestoreMagic()
    {
        while (true)
        {
            yield return null;
            if (playerBasicState is PlayerShootState || playerBasicState is PlayerStorageShootState)
            {
                //处于释放魔法和蓄力魔法的过程中，不进行魔力的恢复
                continue;
            }
            if (!AddValueToMP(playerData.RestoreMPPerFrame))
            {
                Debug.Log("回蓝失败");
            }
            playerData.CurMP = playerData.CurMP > playerData.MaxMP ? playerData.MaxMP : playerData.CurMP;
        }
    }

    public bool AddValueToMP(float value)
    {
        if (value < 0 && playerData.CurMP < Mathf.Abs(value))
        {
            return false;
        }
        playerData.CurMP += value;
        return true;
    }

    public bool AddValueToResonanceValue(float value)
    {
        if ((MagicType)playerData.CurResonanceType == MagicType.Nihility)
        {
            return true;
        }
        if (value < 0 && playerData.CurResonanceValue < Mathf.Abs(value))
        {
            return false;
        }
        playerData.CurResonanceValue += value;
        return true;
    }

    public bool AddValueToHp(float value)
    {
        if (value < 0 && playerData.CurHP < Mathf.Abs(value))
        {
            return false;
        }
        playerData.CurHP += value;
        playerData.CurHP = playerData.CurHP > playerData.MaxHP ? playerData.MaxHP : playerData.CurHP;
        return true;
    }

    public void ResumePlayerDataToDefault()
    {
        playerData.CurHP = playerData.MaxHP;
        playerData.CurMP = playerData.MaxMP;
        playerData.CurBP = playerData.MaxBP;

        foreach (var spriteRender in spriteRenderers)
        {
            spriteRender.color = oldSpriteColor;
        }
    }
}
