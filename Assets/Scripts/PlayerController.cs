using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //private Animator anim;
    private Rigidbody2D myRigidbody;

    public float xAxis;

    [Header("Walk")]
    public float walkSpeed;

    [Header("Run")]
    public float runSpeed;

    [Header("Jump")]
    public float jumpForce;

    [Header("Ground Check")]
    public Transform checkPoint;
    public float checkRadius;
    public LayerMask groundLayer;

    [Header("State")]
    public bool isGround;
    public bool isJump;

    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myRigidbody.velocity.y < 0)
        {
            isJump = false;
        }
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

    public void Jump()
    {
        if (Input.GetButton("Jump") && isGround)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
            isJump = true;
            isGround = false;
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
