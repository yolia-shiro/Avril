using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class WizardController : Boss
{
    [HideInInspector] public int state;
    [HideInInspector] public bool isJumpBackOver;
    [HideInInspector] public bool haveWeapon = true;
    [HideInInspector] public bool isThrowWeaponHit;    //是否击中

    [Header("Ground Check")]
    public float groundCheckRadius;
    public Transform groundCheckPoint;
    public LayerMask groundCheckLayer;
    [HideInInspector] public bool isGround;

    [Header("Defense")]
    public GameObject defenseEffect;
    public Vector2 defaultDefensePosOffset;
    public float defenseEffectDuration;     //防御特效持续时间

    [Header("Weapon")]
    public Transform weapon;

    [Header("ThrowWeapon")]
    public Transform newWeaponParent;
    private GameObject newWeapon;
    public float throwSpeed;    //抛出速度

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        //获取状态值
        state = (behaviorTree.GetVariable("state") as SharedInt).Value;
        //检测地面
        isGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundCheckLayer);
        behaviorTree.SetVariable("isGround", (SharedBool)isGround);
        //后跳是否结束
        isJumpBackOver = (behaviorTree.GetVariable("isJumpBackOver") as SharedBool).Value;
        //是否持有武器
        haveWeapon = (behaviorTree.GetVariable("haveWeapon") as SharedBool).Value;
        //投掷的武器是否击中
        isThrowWeaponHit = (behaviorTree.GetVariable("isThrowWeaponHit") as SharedBool).Value;
    }

    public void OpenDefenseEffect(Transform createTransform, bool isDefault = false)
    {
        //生成防御特效
        if (isDefault)
        {
            int dir = createTransform.position.x < transform.position.x ? -1 : 1;
            defenseEffect.transform.position = new Vector3(transform.position.x + dir * defaultDefensePosOffset.x, defaultDefensePosOffset.y, 0);
            defenseEffect.transform.localRotation = createTransform.right.x > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        }
        else
        {
            defenseEffect.transform.position = createTransform.position;
            defenseEffect.transform.localRotation = createTransform.right.x > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        }
        defenseEffect.SetActive(false);
        defenseEffect.SetActive(true);
    }

    //动画事件(冲刺结束)
    public void SprintPreOver()
    {
        behaviorTree.SetVariable("isPreSprintOver", (SharedBool)true);
    }

    //动画事件(打开武器碰撞器)
    public void WeaponColliderOpen()
    {
        weapon.GetComponentInChildren<Collider2D>().enabled = true;
    }

    //动画事件(挥砍结束)
    public void SlashOver()
    {
        weapon.GetComponentInChildren<Collider2D>().enabled = false;
        //防止动画重复播放
        state = 0;
        behaviorTree.SetVariable("state", (SharedInt)0);
        behaviorTree.SetVariable("isSlash", (SharedBool)false);
    }

    //动画事件(投掷武器)
    public void ThrowWeapon()
    {
        newWeapon = Instantiate(weapon.gameObject, newWeaponParent);
        weapon.gameObject.SetActive(false);
        newWeapon.transform.position = weapon.position;
        newWeapon.transform.rotation = weapon.transform.up.x < 0 ? Quaternion.Euler(180, 0, 90) : Quaternion.Euler(0, 0, -90);
        newWeapon.GetComponentInChildren<Collider2D>().enabled = true;
        newWeapon.GetComponent<WizardWeapon>().SetMoveAttrAndStart(throwSpeed, this);

        behaviorTree.SetVariable("haveWeapon", (SharedBool)false);
        behaviorTree.SetVariable("newWeapon", (SharedGameObject)newWeapon);
    }

    //动画事件(投掷武器结束)
    public void ThrowWeaponOver()
    {
        behaviorTree.SetVariable("isThrowWeaponOver", (SharedBool)true);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
