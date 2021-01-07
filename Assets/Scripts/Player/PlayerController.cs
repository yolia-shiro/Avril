using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //private Animator anim;
    private Rigidbody2D myRigidbody;
    private Missile missile;
    private Animator anim;
    private Collider2D myCollider;

    public float xAxis;

    [Header("Walk")]
    public float walkSpeed;

    [Header("Run")]
    public float runSpeed;

    [Header("Jump")]
    public float jumpForce;

    [Header("Magic Message")]
    public GameObject missilePrefabs;
    public Transform missilePos;

    [Header("Staff Throw Message")]
    public Transform myStaff;
    public Transform newStaffParent;
    private Transform oldStaffParent;
    private Vector3 oldStaffLocalPosition;

    [Header("Roll Message")]
    public float rollDistance;
    public float rollDistanceSmooth;
    public float rollDelta;
    public Vector3 rollTargetPosition;

    [Header("Ground Check")]
    public Transform checkPoint;
    public float checkRadius;
    public LayerMask groundLayer;

    [Header("State")]
    public bool isGround;

    public bool canJump;
    public bool isJump;

    public bool canMagic;
    public bool isMagic;

    public bool haveStaff;
    public int magicKind;

    public bool canRoll;
    public bool isRoll;

    public bool canAttack;
    public bool isAttack;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        myCollider = GetComponent<Collider2D>();
        oldStaffLocalPosition = myStaff.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (myRigidbody.velocity.y < 0)
        {
            isJump = false;
        }

        haveStaff = myStaff.parent != newStaffParent;

        CheckInput();
        StateCheck();
    }

    private void FixedUpdate()
    {
        Movement();
        Jump();
        PhysicCheck();
    }

    public void Movement() 
    {
        if (isRoll || isAttack) 
        {
            return;
        }
        xAxis = Input.GetAxis("Horizontal");
        float speed = Mathf.Abs(xAxis) < 0.3 ? walkSpeed : runSpeed;
        if (xAxis != 0) 
        {
            //翻转
            transform.rotation = xAxis < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0); 
        }

        myRigidbody.velocity = new Vector2(speed * xAxis, myRigidbody.velocity.y);
    }

    public void CheckInput() 
    {
        if(isGround)
        {
            if (canMagic && Input.GetButtonDown("Magic"))
            {
                isMagic = true;
                magicKind = 0;
                //产生飞弹
                missile = Instantiate(missilePrefabs, missilePos.position, Quaternion.identity).GetComponent<Missile>();
            }
            else if (isMagic && Input.GetButtonUp("Magic"))
            {
                isMagic = false;
                magicKind = -1;
                //结束发射飞弹 
                if (missile != null)
                {
                    missile.transform.rotation = transform.right.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                    missile.SwitchMissileState(Missile.missileState.Lauching);
                    missile.CreateMuzzleEffect();
                    //发射之后，将存储该飞弹的对象置空
                    missile = null;
                }
            }
            else if (isMagic && Input.GetButton("Magic"))
            {
                //飞弹跟随
                if (missile != null)
                {
                    missile.FollowCreatingPos(missilePos);
                }
            }
            if (Input.GetButtonDown("Staff Throw"))
            {
                if (haveStaff)
                {
                    //丢法杖
                    anim.SetTrigger("throw");
                }
                else if(canMagic)
                {
                    //瞬移到法杖处
                    transform.position = myStaff.position;
                    myStaff.parent = oldStaffParent;
                    myStaff.localPosition = oldStaffLocalPosition;
                    myStaff.localRotation = Quaternion.identity;
                    myStaff.GetComponent<StaffRotate>().enabled = false;
                    haveStaff = true;
                }
            }
            if (canRoll && Input.GetButtonDown("Roll"))
            {
                anim.SetTrigger("roll");
                rollTargetPosition = transform.position + new Vector3(rollDistance * transform.right.x, 0, 0);
            }
            if (canAttack && Input.GetButtonDown("Attack"))
            {
                anim.SetTrigger("attack");
                isAttack = true;
            }
        }
    }

    public void Jump()
    {
        if (canJump && Input.GetButton("Jump"))
        {
            //进行跳跃，取消当前所有操作
            isJump = true;
            isGround = false;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
        }
    }

    //状态检查
    //对当前状态进行行为处理
    public void StateCheck()
    {
        isRoll = anim.GetCurrentAnimatorStateInfo(2).IsName("Roll");
        isAttack = anim.GetCurrentAnimatorStateInfo(3).IsName("Physic Attack");

        //跳跃的条件：在地面上; 不在翻滚状态; 不在跳跃状态; 不在攻击状态;
        canJump = isGround && !isRoll && !isJump && !isAttack;
        //释放魔法的条件：在地面上; 不在翻滚状态; 不在释放魔法状态; /*有法杖(待定)*/ 不在攻击状态;
        canMagic = isGround && !isRoll && !isMagic && !isAttack; //&& haveStaff;
        //翻滚的条件：在地面上; 不在跳跃状态; 不在翻滚状态; 不在攻击状态;
        canRoll = isGround && !isJump && !isRoll && !isAttack;
        //攻击的条件：在地面上; 不在跳跃状态; 不在翻滚状态; 不在释放魔法状态; 不在攻击状态; 有法杖
        canAttack = isGround && !isJump && !isRoll && !isMagic && !isAttack && haveStaff;

        if (isRoll && Vector3.Distance(transform.position, rollTargetPosition) > rollDistanceSmooth)
        {
            //清除移动时残留的速度
            myRigidbody.velocity = Vector3.zero;

            transform.position = Vector3.MoveTowards(transform.position, rollTargetPosition, rollDelta * Time.deltaTime);
        }

        if (isAttack)
        {
            myRigidbody.velocity = Vector3.zero;
        }

        //翻滚、跳跃和下落时，需要取消施法动作
        if (isRoll || isJump || myRigidbody.velocity.y < 0)
        {
            isMagic = false;
            //销毁生成的missile
            if (missile != null)
            {
                Destroy(missile.gameObject);
            }
        }
    }

    public void PhysicCheck()
    {
        isGround = Physics2D.OverlapCircle(checkPoint.position, checkRadius, groundLayer);
    }

    //动画事件(丢法杖)
    public void ThrowStaff()
    {
        oldStaffParent = myStaff.parent;
        myStaff.parent = newStaffParent;
        myStaff.GetComponent<StaffRotate>().enabled = true;
        haveStaff = false;
    }

    //动画事件(翻滚)
    //无敌帧生效
    public void RollInvincibleStart()
    {
        myRigidbody.bodyType = RigidbodyType2D.Kinematic;
        myCollider.enabled = false;
    }
    //无敌帧结束
    public void RollInvincibleEnd()
    {
        myRigidbody.bodyType = RigidbodyType2D.Dynamic;
        myCollider.enabled = true;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(checkPoint.position, checkRadius);
    }
}
