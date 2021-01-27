using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardController : MonoBehaviour
{
    [HideInInspector] public int state;
    public Transform weapon;

    [Header("Player Message")]
    public PlayerController playerController;

    [Header("Sprint")]
    public Transform leftSprintTarget;
    public Transform rightSprintTarget;
    private Vector3 sprintTarget;
    [HideInInspector] public bool isPreSprintOver;
    public float sprintWaitTime;    //等待一段时间后释放
    private float curSprintTime;
    public float sprintSpeed;
    private int sprintNum;  //冲刺次数
    public GameObject sprintEffect;

    [Header("Sword Aim")]
    public Transform releasePos;
    public GameObject swordPrefabs;
    public int swordGenerateNum;
    public float swordCreateRadius; //生成半径
    public Transform swordCreateParent;
    private List<GameObject> swordLists = new List<GameObject>();
    public float aimSpeed;  //瞄准速度
    public float aimTime;   //瞄准时间，之后进行发射
    private float curAimTime;
    public float launchSpeed;   //发射速度

    [Header("Magic Cannon")]
    public Transform cannonLeftReleasePos;      //释放魔炮的左边界位置
    public Transform cannonRightReleasePos;     //释放魔炮的右边界位置
    private Transform curCannonReleasePos;
    public float jumpBackNeedTime;
    public float jumpBackStartAngle;    //后跳的起始角度
    public float g;
    private Vector2 startVelocity;
    private float curJumpBackResumeTime;    //当前后跳消耗的时间
    private Vector3 jumpBackStartPos;       //起跳时的起始位置
    [HideInInspector] public bool isJumpBackOver;
    public float cannonStorageTime;     //蓄力时间
    private float curCannonStorageTime;
    public float cannonLastTime;    //持续时间
    private float curCannonLastTime;
    public Collider2D cannonCheckArea;  //生效范围的碰撞体


    public GameObject magicCannonEffect;
    

    [Header("FSM")]
    private EnemyBasicState enemyState;
    public WizardIdleState wizardIdleState = new WizardIdleState();
    public SprintState sprintState = new SprintState();
    public SlashState slashState = new SlashState();
    public SwordAimState swordAimState = new SwordAimState();
    public MagicCannonState magicCannonState = new MagicCannonState();

    private Animator anim;
    private Rigidbody2D myRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();

        isPreSprintOver = false;
        TranslateToState(wizardIdleState);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Sprint State")) 
        {
            TranslateToState(sprintState);
        }

        if (GUILayout.Button("Slash State")) 
        {
            TranslateToState(slashState);
        }

        if (GUILayout.Button("Sword Aim State"))
        {
            TranslateToState(swordAimState);
        }

        if (GUILayout.Button("Magic Cannon State"))
        {
            TranslateToState(magicCannonState);
        }
    }

    // Update is called once per frame
    void Update()
    {
        enemyState.OnUpdate(this);
    }

    //切换状态
    public void TranslateToState(EnemyBasicState eState)
    {
        enemyState = eState;
        enemyState.OnEnter(this);
    }

    //冲刺前置准备
    public void SprintPrePreparation()
    {
        isPreSprintOver = false;
        curSprintTime = 0;
        //设置冲刺状态
        state = 1;
        //获取目标点
        //根据玩家位置确定
        sprintTarget = playerController.transform.position.x > transform.position.x ? rightSprintTarget.position : leftSprintTarget.position;
        transform.rotation = playerController.transform.position.x < transform.position.x ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }
    //动画事件
    public void SprintPreOver()
    {
        isPreSprintOver = true;
    }
    //开始冲刺攻击
    public void Sprint()
    {
        if (isPreSprintOver)
        {
            if (curSprintTime < sprintWaitTime)
            {
                curSprintTime += Time.deltaTime;
            }
            else
            {
                sprintEffect.SetActive(true);
                transform.position = Vector3.MoveTowards(transform.position, sprintTarget, sprintSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, sprintTarget) < 0.1f)
                {
                    state = 0;
                    sprintEffect.SetActive(false);
                    if (!anim.GetCurrentAnimatorStateInfo(1).IsName("back_sprint"))
                    {
                        //如果当前血量小于50%(待定)
                        //进行二段冲刺
                        sprintNum++;
                        if (sprintNum < 2)
                        {
                            TranslateToState(sprintState);
                         }
                        else
                        {
                            sprintNum = 0;
                            //切换状态
                            TranslateToState(wizardIdleState);
                        }
                    }
                }
            }
        }
    }

    //动画事件
    public void SlashOver() 
    {
        TranslateToState(wizardIdleState);
    }

    //移动到指定位置
    public void MoveToTargetPos(Transform curObj, Vector3 target, bool blink, float speed = 10000.0f)
    {
        if (blink)
        {
            //瞬移
            curObj.position = target;
        }
        else 
        {
            //匀速
            curObj.position = Vector3.MoveTowards(curObj.position, target, speed * Time.deltaTime);
        }
    }

    //获取随机生成位置
    public Vector2 GetRandomPos()
    {
        float r = Random.Range(0, swordCreateRadius);
        float angle = Random.Range(0, Mathf.PI);
        float x = r * Mathf.Cos(angle);
        float y = r * Mathf.Sin(angle);
        return new Vector2(x, y);
    }

    //魔法剑随机生成
    public void GenerateMagicSword()
    {
        //生成魔法剑
        for (int i = 0; i < swordGenerateNum; i++) 
        {
            Vector2 GeneratePos = GetRandomPos();
            swordLists.Add(Instantiate(swordPrefabs, swordCreateParent));
            swordLists[swordLists.Count - 1].transform.localPosition = GeneratePos;
            swordLists[swordLists.Count - 1].SetActive(false);
        }
    }

    public void SwordAimPrePreparation()
    {
        state = 3;
        //移动任务到指定位置
        MoveToTargetPos(transform, releasePos.position, true);
        //关闭重力
        myRigidbody.bodyType = RigidbodyType2D.Kinematic;
        //生成魔法剑
        GenerateMagicSword();
        curAimTime = 0;
    }

    public void SwordAim()
    {
        if (swordLists.Count == 0)
        {
            //结束当前状态
            TranslateToState(wizardIdleState);
            //打开重力
            myRigidbody.bodyType = RigidbodyType2D.Dynamic;
            return;
        }
        if (curAimTime < aimTime)
        {
            //打开第一把剑
            swordLists[0].SetActive(true);
            //瞄准Player
            Vector3 dir = (swordLists[0].transform.position - playerController.transform.position).normalized;
            swordLists[0].transform.up = Vector3.MoveTowards(swordLists[0].transform.up, dir, aimSpeed * Time.deltaTime);
            curAimTime += Time.deltaTime;
        }
        else
        {
            //发射
            swordLists[0].GetComponent<MagicSword>().SetAttrToStartLaunch(launchSpeed);
            swordLists.RemoveAt(0);
            curAimTime = 0;
        }
    }

    public void MagicCannonPrePreparation()
    {
        state = 4;
        isJumpBackOver = false;
        //面向Player
        transform.rotation = transform.position.x < playerController.transform.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        //确定释放位置
        //向着远离Player的方向
        curCannonReleasePos = transform.position.x < playerController.transform.position.x ? cannonLeftReleasePos : cannonRightReleasePos;
        //开始后跳
        myRigidbody.bodyType = RigidbodyType2D.Kinematic;
        //计算后跳抛物线所需要的变量
        jumpBackStartPos = transform.position;
        startVelocity.x = Mathf.Abs(curCannonReleasePos.position.x - transform.position.x) / jumpBackNeedTime;
        startVelocity.y = -0.5f * g * jumpBackNeedTime;
        StartCoroutine(JumpBack());
        curCannonStorageTime = 0;
        curCannonLastTime = 0;
    }

    public IEnumerator JumpBack()
    {
        while (Vector3.Distance(transform.position, curCannonReleasePos.position) >= 0.1f) 
        {
            float x = startVelocity.x * curJumpBackResumeTime;
            float y = startVelocity.y * curJumpBackResumeTime + 0.5f * g * curJumpBackResumeTime * curJumpBackResumeTime;
            transform.position = jumpBackStartPos + new Vector3((transform.right.x < 0 ? 1 : -1) * x, y, 0);
            curJumpBackResumeTime += Time.fixedDeltaTime;
            yield return null;
        }
        isJumpBackOver = true;
        myRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    public void MagicCannon()
    {
        if (curCannonStorageTime < cannonStorageTime)
        {
            curCannonStorageTime += Time.deltaTime;
        }
        else
        {
            //释放
            magicCannonEffect.transform.GetChild(1).gameObject.SetActive(true);
            cannonCheckArea.enabled = true;
            if (curCannonLastTime >= cannonLastTime)
            {
                //释放结束
                magicCannonEffect.transform.GetChild(1).gameObject.SetActive(false);
                magicCannonEffect.SetActive(false);
                cannonCheckArea.enabled = false;
                TranslateToState(wizardIdleState);
            }
            curCannonLastTime += Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, swordCreateRadius);
    }
}
