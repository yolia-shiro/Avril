using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //private Animator anim;
    private Rigidbody2D myRigidbody;
    private Missile missile;

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

    [Header("Ground Check")]
    public Transform checkPoint;
    public float checkRadius;
    public LayerMask groundLayer;

    [Header("State")]
    public bool isGround;
    public bool isJump;
    public bool isMagic;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
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
            if (Input.GetButtonDown("Magic"))
            {
                isMagic = true;
                //产生飞弹
                missile = Instantiate(missilePrefabs, missilePos.position, Quaternion.identity).GetComponent<Missile>();
            }
            else if (Input.GetButtonUp("Magic"))
            {
                isMagic = false;
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
            else if (Input.GetButton("Magic"))
            {
                //飞弹跟随
                if (missile != null)
                {
                    missile.FollowCreatingPos(missilePos);
                }
            }
        }
    }

    public void Jump()
    {
        if (Input.GetButton("Jump") && isGround)
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
        //跳跃和下落时，需要取消施法动作
        if (isJump || myRigidbody.velocity.y < 0)
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

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(checkPoint.position, checkRadius);
    }
}
