using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSystem : MonoBehaviour
{
    [Header("Magic Message")]
    public GameObject[] magicPrefabs;
    public Transform magicPos;
    public float magicLaunchCameraDuration;
    public float magicLaunchCameraMigration;

    [Header("Magic Move Message")]
    public float yOffset; //y轴偏移 ----Avril重心在Bottom

    [Header("Magic Storage Message")]
    public float storageTime;
    public List<Transform> storagePos = new List<Transform>();
    public List<GameObject> magicStorge = new List<GameObject>();
    public int storageIndex = -1;
    public int selectedStorageMagicIndex = 0;
    public float selectedScaleTime;

    [Header("Magic Mix")]
    public float magicSpeed;
    
    [Header("Staff Throw Message")]
    public Transform myStaff;
    public Transform newStaffParent;
    private Transform oldStaffParent;
    private Vector3 oldStaffLocalPosition;

    private PlayerController playerController;
    private Animator anim;
    private Rigidbody2D myRigidbody;
    public Magic curMagic;

    public int curMagicKind;

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
            if (curMagic != null)
            {
                Destroy(curMagic.gameObject);
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

    //合并魔法
    public IEnumerator MergeMagic()
    {
        if (curMagic != null && magicStorge.Count > 0)
        {
            int mergeType = 0;
            BitArray bitArray = new BitArray(3);

            Magic m1 = curMagic;
            Magic m2 = magicStorge[selectedStorageMagicIndex].GetComponent<Magic>();
            if (m1 is AttackMagic && m2 is AttackMagic)
            {
                AttackMagic atm1 = m1 as AttackMagic;
                AttackMagic atm2 = m2 as AttackMagic;
                bitArray.Set(0, atm1.isNormal | atm2.isNormal);
                bitArray.Set(1, atm1.isDrag | atm2.isDrag);
                bitArray.Set(2, atm1.isTrack | atm2.isTrack);
            }
            else if (m1 is AssistMagic && m2 is AssistMagic)
            {
                AssistMagic asm1 = m1 as AssistMagic;
                AssistMagic asm2 = m2 as AssistMagic;
            }
            for (int i = 2; i >= 0; i--)
            {
                mergeType = (mergeType << 1) + Convert.ToInt32(bitArray.Get(i));
            }

            if (mergeType > 0)
            {
                magicStorge.RemoveAt(selectedStorageMagicIndex);
                storageIndex--;
                for (int i = 0; i < magicStorge.Count; i++)
                {
                    magicStorge[i].transform.parent = storagePos[i];
                    magicStorge[i].transform.localPosition = Vector3.zero;
                    magicStorge[i].transform.localScale = Vector3.one;
                }
                m2.enabled = false;

                Magic oldMagic = curMagic;
                Magic mixMagic = Instantiate(magicPrefabs[mergeType], magicPos.position, Quaternion.identity).GetComponent<Magic>();
                mixMagic.gameObject.SetActive(false);
                curMagic = mixMagic;
                while (curMagic != null && Vector3.Distance(oldMagic.transform.position, m2.transform.position) >= 0.1)
                {
                    oldMagic.transform.position = curMagic.transform.position;
                    m2.transform.position = Vector3.Lerp(m2.transform.position, oldMagic.transform.position, magicSpeed * Time.deltaTime);
                    yield return new WaitForSeconds(Time.deltaTime);
                }

                Destroy(oldMagic.gameObject);
                Destroy(m2.gameObject);
                //此处mixMagic和curMagic指向一个对象
                //不使用curMagic的原因是在释放魔法的时候，会将curMagic置null，导致出现异常。
                mixMagic.gameObject.SetActive(true);

                selectedStorageMagicIndex = 0;
            }
        }
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
        if (curMagic != null)
        {
            //设置curMagic的gameObject的原因：在魔法混合未结束的时候，curMagic.gameObject为false
            curMagic.gameObject.SetActive(true);
            curMagic.GetComponent<Collider2D>().enabled = true;
            curMagic.transform.rotation = transform.right.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            curMagic.SwitchMissileState(Magic.MagicState.Effective);
            //curMagic.CreateMuzzleEffect();
            //发射之后，将存储该飞弹的对象置空
            curMagic = null;
        }
    }

    //储存魔法的操作输入检测
    public void StorageMissileOperatorInputCheck()
    {
        if (Input.GetButtonDown("Select Storage Missile") && magicStorge.Count != 0)
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
        if (magicStorge.Count != 0) 
        {
            Magic tempMagic = magicStorge[0].GetComponent<Magic>();
            magicStorge.RemoveAt(0);
            storageIndex--;

            yield return StartCoroutine(tempMagic.MissileToTargetScale(Vector3.zero));
            Destroy(tempMagic.gameObject);
            
            for (int i = 0; i < magicStorge.Count; i++) 
            {
                magicStorge[i].transform.parent = storagePos[i];
                magicStorge[i].transform.localPosition = Vector3.zero;
                magicStorge[i].transform.localScale = Vector3.one;
            }
        }
    }
}