//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MagicSystem : MonoBehaviour
//{
//    public enum MagicKind
//    {
//        NormalMagic = 1,
//        DragMagic = 2,
//        TrackMagic = 4,
//    }

//    [Header("Magic Message")]
//    public GameObject[] attackMagicPrefabs;
//    public GameObject[] assistMagicPrefabs;
//    public Transform magicPos;
//    public Transform healPos;
//    public Transform defensePos;
//    public float magicLaunchCameraDuration;
//    public float magicLaunchCameraMigration;

//    [Header("Magic Move Message")]
//    public float yOffset; //y轴偏移 ----Avril重心在Bottom

//    [Header("Magic Storage Message")]
//    public float storageTime;
//    public List<Transform> storagePos = new List<Transform>();
//    [HideInInspector] public List<GameObject> magicStorge = new List<GameObject>();
//    private int storageIndex = -1;
//    [HideInInspector] public int selectedStorageMagicIndex = 0;
//    public float selectedScaleTime;     //选择融合魔法时的时间系数

//    [Header("Magic Mix")]
//    public float magicSpeed;

//    [Header("Staff Throw Message")]
//    public Transform myStaff;
//    public GameObject staffEffect;      //武器附魔特效
//    public Transform newStaffParent;
//    private Transform oldStaffParent;
//    private Vector3 oldStaffLocalPosition;

//    //生成抛物线轨迹
//    [Header("Parabolic Trajectory")]
//    public LineRenderer staffThrowLineRenderer;
//    public int parabolaPointsSize;
//    public float minThrowSpeed;
//    public float maxThrowSpeed;
//    private float throwSpeed;
//    public float lineRendererDistance;
//    public float gravity;

//    [HideInInspector] public PlayerData playerData;
//    private PlayerController playerController;
//    private Animator anim;
//    private Rigidbody2D myRigidbody;
//    [HideInInspector] public Magic curMagic;

//    //攻击魔法的种类
//    [HideInInspector] public int curAttackMagicKind;
//    //辅助魔法的种类
//    [HideInInspector] public int curAssistMagicKind;

//    //FSM
//    private PlayerMagicBasicState curMagicState;
//    public MagicDefaultState magicDefaultState = new MagicDefaultState();
//    public MagicMissileState magicMissileState = new MagicMissileState();
//    public MagicAssistState magicAssistState = new MagicAssistState();
//    public MagicStorageState magicStorageState = new MagicStorageState();

//    private void Start()
//    {
//        TranslateToState(magicDefaultState);

//        playerData = GetComponent<PlayerData>();
//        playerController = GetComponent<PlayerController>();
//        anim = GetComponent<Animator>();
//        myRigidbody = GetComponent<Rigidbody2D>();

//        oldStaffLocalPosition = myStaff.localPosition;
//        throwSpeed = minThrowSpeed;

//        curAttackMagicKind = (int)MagicKind.NormalMagic;
//        curAssistMagicKind = 0;
//        UIManager.instance.UpdateAttackMagicKind(curAttackMagicKind);
//        UIManager.instance.UpdateAssistMagicKind(curAssistMagicKind);
//    }

//    private void Update()
//    {
//        if (playerController.isDead)
//        {
//            return;
//        }
//        //playerController.haveStaff = myStaff.parent != newStaffParent;
//        playerData.curMagicAttachValue = myStaff.GetComponent<AttackMagic>() == null ? 0 : myStaff.GetComponent<AttackMagic>().curAttachValue;
//        if (playerData.curMagicAttachValue <= 0)
//        {
//            //关闭武器附魔特效
//            SwitchWeaponEffect(false);
//        }
//        //切换攻击魔法
//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            curAttackMagicKind = (curAttackMagicKind << 1) % (attackMagicPrefabs.Length - 1);
//            //更新UI
//            UIManager.instance.UpdateAttackMagicKind(curAttackMagicKind);
//        }
//        //切换辅助魔法
//        if (Input.GetKeyDown(KeyCode.D))
//        {
//            curAssistMagicKind = (curAssistMagicKind + 1) % (assistMagicPrefabs.Length);
//            //更新UI
//            UIManager.instance.UpdateAssistMagicKind(curAssistMagicKind);
//        }


//        //翻滚、跳跃和下落时，需要取消施法动作
//        if (playerController.isRoll || playerController.isJump || myRigidbody.velocity.y < -0.1)
//        {
//            //playerController.isMagic = false;
//            //playerController.magicKind = -1;
//            //销毁生成的missile
//            if (curMagic != null)
//            {
//                Destroy(curMagic.gameObject);
//            }
//        }
//        ////跳跃、翻滚、下落、攻击、施法时取消瞄准动作
//        //if (!playerController.canThrow)
//        //{
//        //    staffThrowLineRenderer.gameObject.SetActive(false);
//        //    playerController.isAim = false;
//        //}


//        if (playerController.isGround)
//        {
//            MagicInputCheck();
//            ThrowStaffAndMagicMoveInputCheck();
//            StorageMissileOperatorInputCheck();
//        }

//        curMagicState.OnUpdate(this, this.playerController);
//        Debug.Log(curMagicState);
//    }

//    //转换
//    public void TranslateToState(PlayerMagicBasicState playerMagicBasicState)
//    {
//        curMagicState = playerMagicBasicState;
//        curMagicState.OnEnter(this, this.playerController);
//    }

//    //消耗魔力
//    public void ResumeMagic(float value)
//    {
//        //施法、施法尚未结束
//        if (playerController.isMagic && curMagic != null && !curMagic.isCreateOver)
//        {
//            playerData.curMP -= value * Time.deltaTime;
//            if (playerData.curMP <= 0)
//            {
//                playerData.curMP = 0;
//                if (curMagic is AttackMagic)
//                {
//                    //发射
//                    MagicLaunch();
//                }
//                else if (curMagic is AssistMagic)
//                {
//                    //切换至生效状态
//                    curMagic.SwitchMissileState(Magic.MagicState.Effective);
//                }
//                //角色状态切换
//                TranslateToState(magicDefaultState);
//            }
//        }
//    }

//    //生成攻击魔法
//    public void GenerateAttackMagic()
//    {
//        //playerController.isMagic = true;
//        //playerController.magicKind = 0;

//        if (curMagic == null)
//        {
//            AttackMagic attackMagic = Instantiate(attackMagicPrefabs[curAttackMagicKind],
//                magicPos.position, Quaternion.identity).GetComponent<AttackMagic>();
//            attackMagic.transform.rotation = transform.right.x > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
//            attackMagic.SetAttackMagicType(curAttackMagicKind);
//            curMagic = attackMagic;
//        }
//    }

//    //生成辅助魔法
//    public void GenerateAssistMagic()
//    {
//        //playerController.isMagic = true;
//        //if (curAssistMagicKind == 0)
//        //{
//        //    //治愈魔法
//        //    playerController.magicKind = 2;
//        //}
//        //else if (curAssistMagicKind == 1)
//        //{
//        //    //防御魔法
//        //    playerController.magicKind = 3;
//        //}

//        //生成魔法
//        if (curMagic == null)
//        {
//            AssistMagic assistMagic = null;
//            switch (curAssistMagicKind)
//            {
//                case 0:
//                    assistMagic = Instantiate(assistMagicPrefabs[curAssistMagicKind],
//                        healPos.position, Quaternion.identity).GetComponent<AssistMagic>();
//                    assistMagic.transform.rotation = transform.right.x > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
//                    break;
//                case 1:
//                    assistMagic = Instantiate(assistMagicPrefabs[curAssistMagicKind],
//                        defensePos.position, Quaternion.identity).GetComponent<AssistMagic>();
//                    assistMagic.transform.rotation = transform.right.x > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
//                    break;
//            }
//            curMagic = assistMagic;
//        }
//    }

//    //武器附魔
//    public void WeaponMagicAttach()
//    {
//        if (myStaff.GetComponent<AttackMagic>() == null)
//        {
//            myStaff.gameObject.AddComponent<AttackMagic>();
//        }
//        AttackMagic weaponAttachMagic = myStaff.GetComponent<AttackMagic>();
//        weaponAttachMagic.SwitchMissileState(Magic.MagicState.Attachment);
//        weaponAttachMagic.SetMagicBurstAttr(curMagic as AttackMagic);
//        weaponAttachMagic.SetWeaponAttachAttr(curMagic as AttackMagic);
//        weaponAttachMagic.curAttachValue += (curMagic as AttackMagic).attachValuePerMagic;
//        SwitchWeaponEffect(true);
//        Destroy(curMagic.gameObject);
//        curMagic = null;
//    }

//    public IEnumerator MissileToStoragePos(Magic toStorageMagic)
//    {
//        StartCoroutine(DestoryStorageMissileAfterTime());
//        //更新UI
//        UIManager.instance.UpdateStorageMagicUI(toStorageMagic is AttackMagic ? toStorageMagic as AttackMagic : null);

//        Vector3 storageScale = toStorageMagic.transform.localScale / toStorageMagic.maxScale;

//        //提前设置父物体，以防连续存储时，发生储存位置重叠的问题
//        storageIndex++;
//        toStorageMagic.transform.parent = storagePos[storageIndex];
//        //消散
//        yield return StartCoroutine(toStorageMagic.MissileToTargetScale(Vector3.zero));
//        //放置到Magic Storage处
//        toStorageMagic.SwitchMissileState(Magic.MagicState.Storage);
//        toStorageMagic.SetToStorageAttr(Vector3.zero);
//        //还原到储存大小
//        yield return toStorageMagic.StartCoroutine(toStorageMagic.MissileToTargetScale(storageScale));
//        //进行数据存储
//        //防止出现未存储完毕的魔法被用去混合
//        magicStorge.Add(toStorageMagic.gameObject);
//    }

//    //合并魔法
//    public IEnumerator MergeMagic()
//    {
//        if (curMagic != null && magicStorge.Count > 0)
//        {
//            int mergeType = 0;

//            Magic m1 = curMagic;
//            Magic m2 = magicStorge[selectedStorageMagicIndex].GetComponent<Magic>();
//            if (m1 is AttackMagic && m2 is AttackMagic)
//            {
//                AttackMagic atm1 = m1 as AttackMagic;
//                AttackMagic atm2 = m2 as AttackMagic;
//                mergeType = atm1.GetMagicType() | atm2.GetMagicType();
//            }
//            else if (m1 is AssistMagic && m2 is AssistMagic)
//            {
//                AssistMagic asm1 = m1 as AssistMagic;
//                AssistMagic asm2 = m2 as AssistMagic;
//            }

//            if (mergeType > 0)
//            {
//                magicStorge.RemoveAt(selectedStorageMagicIndex);
//                //更新UI
//                UIManager.instance.RemoveStorageMagicUI(selectedStorageMagicIndex);
//                //
//                storageIndex--;
//                for (int i = 0; i < magicStorge.Count; i++)
//                {
//                    magicStorge[i].transform.parent = storagePos[i];
//                    magicStorge[i].transform.localPosition = Vector3.zero;
//                    magicStorge[i].transform.localScale = Vector3.one;
//                }
//                m2.enabled = false;

//                Magic oldMagic = curMagic;
//                Magic mixMagic = Instantiate(attackMagicPrefabs[mergeType], magicPos.position, Quaternion.identity).GetComponent<Magic>();
//                mixMagic.gameObject.SetActive(false);
//                curMagic = mixMagic;
//                while (curMagic != null && Vector3.Distance(oldMagic.transform.position, m2.transform.position) >= 0.1)
//                {
//                    oldMagic.transform.position = curMagic.transform.position;
//                    m2.transform.position = Vector3.Lerp(m2.transform.position, oldMagic.transform.position, magicSpeed * Time.deltaTime);
//                    yield return new WaitForSeconds(Time.deltaTime);
//                }

//                Destroy(oldMagic.gameObject);
//                Destroy(m2.gameObject);
//                //此处mixMagic和curMagic指向一个对象
//                //不使用curMagic的原因是在释放魔法的时候，会将curMagic置null，导致出现异常。
//                mixMagic.gameObject.SetActive(true);
//                //重置选择
//                selectedStorageMagicIndex = 0;
//            }
//        }
//    }

//    //魔法飞弹输入相关
//    public void MagicInputCheck()
//    {
//        //if (playerController.canMagic)
//        //{
//        //    if (Input.GetButtonDown("Magic"))
//        //    {
//        //        if (playerController.haveStaff && playerData.curMP > 0)
//        //        {
//        //            TranslateToState(magicMissileState);
//        //        }
//        //        else 
//        //        {
//        //            //法杖处进行魔力爆发(不需要魔力)
//        //            AttackMagic attackMagic = myStaff.GetComponent<AttackMagic>();
//        //            if (attackMagic != null)
//        //            {
//        //                Debug.Log("魔力爆发");
//        //                attackMagic.MagicBurst();
//        //                SwitchWeaponEffect(false);
//        //            }
//        //        }
//        //    }
//        //    else if (Input.GetKeyDown(KeyCode.V) && playerController.haveStaff && playerData.curMP > 0)
//        //    {
//        //        //取消刚体速度
//        //        myRigidbody.velocity = Vector3.zero;
//        //        TranslateToState(magicAssistState);
//        //    }
//        //}
//    }
//    //发射魔法飞弹
//    public void MagicLaunch()
//    {
//        //playerController.isMagic = false;
//        //playerController.magicKind = -1;
//        //结束发射飞弹 
//        if (curMagic != null)
//        {
//            //设置curMagic的gameObject的原因：在魔法混合未结束的时候，curMagic.gameObject为false
//            curMagic.gameObject.SetActive(true);
//            curMagic.GetComponent<Collider2D>().enabled = true;
//            curMagic.transform.rotation = transform.right.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
//            curMagic.SwitchMissileState(Magic.MagicState.Effective);
//            //curMagic.CreateMuzzleEffect();
//            //发射之后，将存储该飞弹的对象置空
//            curMagic = null;
//        }
//    }

//    //创建（打开）武器附魔特效
//    public void SwitchWeaponEffect(bool effectState)
//    {
//        staffEffect.SetActive(effectState);
//    }

//    //储存魔法的操作输入检测
//    public void StorageMissileOperatorInputCheck()
//    {
//        if (Input.GetButtonDown("Select Storage Missile") && magicStorge.Count != 0)
//        {
//            TranslateToState(magicStorageState);
//        }
//    }

//    //法杖和魔法移动输入相关
//    public void ThrowStaffAndMagicMoveInputCheck()
//    {
//        //if (Input.GetButton("Staff Throw") && playerController.canThrow)
//        //{
//        //    playerController.isAim = true;
//        //    myRigidbody.velocity = Vector2.zero;
//        //    //Input.ResetInputAxes();
//        //    float horAxis = Input.GetAxis("Horizontal");
//        //    throwSpeed = transform.right.x < 0 ? throwSpeed - horAxis : throwSpeed + horAxis;
//        //    throwSpeed = throwSpeed < minThrowSpeed ? minThrowSpeed : throwSpeed;
//        //    throwSpeed = throwSpeed > maxThrowSpeed ? maxThrowSpeed : throwSpeed;
//        //    //生成抛物线轨迹瞄准
//        //    Debug.Log("生成抛物线");
//        //    staffThrowLineRenderer.gameObject.SetActive(true);
//        //    float angle = Mathf.Deg2Rad * staffThrowLineRenderer.transform.eulerAngles.z;
//        //    Vector3 []points = Util.instance.ProjectileArcPoints(parabolaPointsSize, throwSpeed, lineRendererDistance, gravity, angle);
//        //    myStaff.GetComponent<StaffRotate>().SetMoveAttr(gravity, throwSpeed, angle);
//        //    List<Vector3> lineRenerderPoints = new List<Vector3>();
//        //    bool isOver = false;
//        //    for (int i = 0; i < parabolaPointsSize; i++)
//        //    {
//        //        Vector3 checkPoint = staffThrowLineRenderer.transform.position +
//        //            new Vector3((staffThrowLineRenderer.transform.right.x < 0 ? -1 : 1) * points[i].x, points[i].y, -5);
//        //        Collider2D []tempColl = null;
//        //        if ((tempColl = Physics2D.OverlapPointAll(checkPoint)) != null)
//        //        {
//        //            //标签判断
//        //            foreach (var coll in tempColl) 
//        //            {
//        //                if (coll.CompareTag("Ground"))
//        //                {
//        //                    isOver = true;
//        //                    break;
//        //                }
//        //            }
//        //        }
//        //        lineRenerderPoints.Add(checkPoint);
//        //        if (isOver)
//        //        {
//        //            break;
//        //        }
//        //    }
//        //    staffThrowLineRenderer.positionCount = lineRenerderPoints.Count;
//        //    staffThrowLineRenderer.SetPositions(lineRenerderPoints.ToArray());
//        //}
//        //if (Input.GetButtonUp("Staff Throw"))
//        //{
//        //    if (playerController.canThrow)
//        //    {
//        //        //丢法杖
//        //        staffThrowLineRenderer.gameObject.SetActive(false);
//        //        playerController.isAim = false;
//        //        throwSpeed = minThrowSpeed;
//        //        anim.SetTrigger("throw");
//        //        playerController.haveStaff = false;
//        //    }
//        //    else if (playerController.canMagic)
//        //    {
//        //        playerController.isMagic = true;
//        //        playerController.magicKind = 1;
//        //    }
//        //}
//    }

//    //动画事件(丢法杖)
//    public void ThrowStaff()
//    {
//        //oldStaffParent = myStaff.parent;
//        //myStaff.parent = newStaffParent;
//        //myStaff.GetComponent<StaffRotate>().enabled = true;
//        //playerController.haveStaff = false;
//    }

//    //动画事件(瞬移到法杖处)
//    public void MagicMove()
//    {
//        //transform.position = myStaff.position + new Vector3(0, -yOffset, 0);
//        //myStaff.parent = oldStaffParent;
//        //myStaff.localPosition = oldStaffLocalPosition;
//        //myStaff.localRotation = Quaternion.identity;
//        //myStaff.GetComponent<StaffRotate>().enabled = false;
//        //playerController.haveStaff = true;
//    }

//    public IEnumerator DestoryStorageMissileAfterTime()
//    {
//        yield return new WaitForSeconds(storageTime);
//        if (magicStorge.Count != 0)
//        {
//            Magic tempMagic = magicStorge[0].GetComponent<Magic>();
//            magicStorge.RemoveAt(0);
//            storageIndex--;

//            yield return StartCoroutine(tempMagic.MissileToTargetScale(Vector3.zero));
//            Destroy(tempMagic.gameObject);

//            for (int i = 0; i < magicStorge.Count; i++)
//            {
//                magicStorge[i].transform.parent = storagePos[i];
//                magicStorge[i].transform.localPosition = Vector3.zero;
//                magicStorge[i].transform.localScale = Vector3.one;
//            }
//        }
//    }
//}