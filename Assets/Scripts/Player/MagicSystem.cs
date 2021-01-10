using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSystem : MonoBehaviour
{
    [Header("Missile Message")]
    public GameObject missilePrefabs;
    public Transform missilePos;

    [Header("Magic Move Message")]
    public float yOffset; //y轴偏移 ----Avril重心在Bottom

    [Header("Magic Storage Message")]
    public float storageTime;
    public List<Transform> storagePos = new List<Transform>();
    public List<GameObject> missileStorge = new List<GameObject>();
    public int selectedStorageMissileIndex = 0;
    public float selectedScaleTime;
    
    [Header("Staff Throw Message")]
    public Transform myStaff;
    public Transform newStaffParent;
    private Transform oldStaffParent;
    private Vector3 oldStaffLocalPosition;

    private PlayerController playerController;
    private Animator anim;
    private Rigidbody2D myRigidbody;
    public Missile missile;

    //FSM
    private PlayerMagicBasicState curMagicState;
    public MagicDefaultState magicDefaultState = new MagicDefaultState();
    public MagicMissileState magicMissileState = new MagicMissileState();
    public MagicStorageState magicStorageState = new MagicStorageState();

    private void Start()
    {
        TranslateToState(magicDefaultState);

        playerController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();

        oldStaffLocalPosition = myStaff.localPosition;
    }

    private void Update()
    {
        playerController.haveStaff = myStaff.parent != newStaffParent;

        //翻滚、跳跃和下落时，需要取消施法动作
        if (playerController.isRoll || playerController.isJump || myRigidbody.velocity.y < 0)
        {
            playerController.isMagic = false;
            playerController.magicKind = -1;
            //销毁生成的missile
            if (missile != null)
            {
                Destroy(missile.gameObject);
            }
        }

        if (playerController.isGround)
        {
            MagicMissileInputCheck();
            ThrowStaffAndMagicMoveInputCheck();
            StorageMissileOperatorInputCheck();
        }

        curMagicState.OnUpdate(this, this.playerController);
        Debug.Log(curMagicState);
    }

    //转换
    public void TranslateToState(PlayerMagicBasicState playerMagicBasicState)
    {
        curMagicState = playerMagicBasicState;
        curMagicState.OnEnter(this, this.playerController);
    }

    //魔法飞弹输入相关
    public void MagicMissileInputCheck()
    {
        if (playerController.canMagic && Input.GetButtonDown("Magic"))
        {
            TranslateToState(magicMissileState);
            
        }
    }
    //发射魔法飞弹
    public void MagicLaunch()
    {
        playerController.isMagic = false;
        playerController.magicKind = -1;
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

    //储存魔法的操作输入检测
    public void StorageMissileOperatorInputCheck()
    {
        if (Input.GetButtonDown("Select Storage Missile") && missileStorge.Count != 0)
        {
            TranslateToState(magicStorageState);
        }
    }

    //法杖和魔法移动输入相关
    public void ThrowStaffAndMagicMoveInputCheck()
    {
        if (Input.GetButtonDown("Staff Throw"))
        {
            if (playerController.haveStaff && !playerController.isMagic)
            {
                //丢法杖
                anim.SetTrigger("throw");
            }
            else if (playerController.canMagic)
            {
                playerController.isMagic = true;
                playerController.magicKind = 1;
            }
        }
    }

    //动画事件(丢法杖)
    public void ThrowStaff()
    {
        oldStaffParent = myStaff.parent;
        myStaff.parent = newStaffParent;
        myStaff.GetComponent<StaffRotate>().enabled = true;
        playerController.haveStaff = false;
    }

    //动画事件(瞬移到法杖处)
    public void MagicMove()
    {
        transform.position = myStaff.position + new Vector3(0, -yOffset, 0);
        myStaff.parent = oldStaffParent;
        myStaff.localPosition = oldStaffLocalPosition;
        myStaff.localRotation = Quaternion.identity;
        myStaff.GetComponent<StaffRotate>().enabled = false;
        playerController.haveStaff = true;
    }

    public IEnumerator DestoryStorageMissileAfterTime()
    {
        yield return new WaitForSeconds(storageTime);
        if (missileStorge.Count != 0) 
        {
            Destroy(missileStorge[0]);
            missileStorge.RemoveAt(0);
            for (int i = 0; i < missileStorge.Count; i++) 
            {
                missileStorge[i].transform.parent = storagePos[i];
                missileStorge[i].transform.localPosition = Vector3.zero;
                missileStorge[i].transform.localScale = Vector3.one;
            }
        }
    }
}