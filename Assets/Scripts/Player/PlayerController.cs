using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D myRigidbody;
    private Animator anim;
    private Collider2D myCollider;

    public float xAxis;

    [Header("Walk")]
    public float walkSpeed;

    [Header("Run")]
    public float runSpeed;

    [Header("Jump")]
    public float jumpForce;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (myRigidbody.velocity.y < 0)
        {
            isJump = false;
        }

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
        if (isRoll || isAttack || magicKind >= 1) 
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

        //跳跃的条件：在地面上; 不在翻滚状态; 不在跳跃状态; 不在攻击状态; 不在释放能打断魔法的状态
        canJump = isGround && !isRoll && !isJump && !isAttack && magicKind <= 0;
        //释放魔法的条件：在地面上; 不在翻滚状态; 不在释放魔法状态; /*有法杖(待定)*/ 不在攻击状态;
        canMagic = isGround && !isRoll && !isMagic && !isAttack; //&& haveStaff;
        //翻滚的条件：在地面上; 不在跳跃状态; 不在翻滚状态; 不在攻击状态; 不在释放能打断魔法的状态
        canRoll = isGround && !isJump && !isRoll && !isAttack && magicKind <= 0;
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
    }

    public void PhysicCheck()
    {
        isGround = Physics2D.OverlapCircle(checkPoint.position, checkRadius, groundLayer);
    }

    public void MagicMoveEnd()
    {
        isMagic = false;
        magicKind = -1;
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
