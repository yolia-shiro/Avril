using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoWizard : MonoBehaviour
{
    public float movePower = 10f;
    public float jumpPower = 15f; //Set Gravity Scale in Rigidbody2D Component to 5

    private Rigidbody2D rb;
    private Animator anim;
    Vector3 movement;
    private int direction=1;
    bool isJumping = false;


    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();

    }

    private void FixedUpdate() {
        Jump();
        Run();
    }
    private void OnTriggerEnter2D(Collider2D other) {
            anim.SetBool("isJumping",false);
    }


    void Run(){
        Vector3 moveVelocity= Vector3.zero;
            anim.SetBool("isRunning",false);


        if( Input.GetAxisRaw("Horizontal")<0){
            direction= -1;
            moveVelocity = Vector3.left;

            transform.localScale = new Vector3(direction,1,1);
            anim.SetBool("isRunning",true);

        }
        if( Input.GetAxisRaw("Horizontal")>0){
            direction= 1;
            moveVelocity = Vector3.right;

            transform.localScale = new Vector3(direction,1,1);
            anim.SetBool("isRunning",true);

        }
        transform.position+=moveVelocity*movePower*Time.deltaTime;
    }
    void Jump(){
        if( ( Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical")>0 )
        &&!anim.GetBool("isJumping")){
            isJumping=true;
            anim.SetTrigger("doJumping");
            anim.SetBool("isJumping",true);
        }
        if(!isJumping){
            return;
        }

        rb.velocity = Vector2.zero;

        Vector2 jumpVelocity = new Vector2(0,jumpPower);
        rb.AddForce(jumpVelocity,ForceMode2D.Impulse);

        isJumping=false;
    }
    
}
