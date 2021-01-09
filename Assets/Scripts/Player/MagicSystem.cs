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
    private List<GameObject> missileStorge = new List<GameObject>();
    private int selectedStorageMissileIndex = 0;
    public float selectedScaleTime;
    
    [Header("Staff Throw Message")]
    public Transform myStaff;
    public Transform newStaffParent;
    private Transform oldStaffParent;
    private Vector3 oldStaffLocalPosition;

    private PlayerController playerController;
    private Animator anim;
    private Rigidbody2D myRigidbody;
    private Missile missile;

    private void Start()
    {
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
    }

    //魔法飞弹输入相关
    public void MagicMissileInputCheck()
    {
        if (playerController.canMagic && Input.GetButtonDown("Magic"))
        {
            playerController.isMagic = true;
            playerController.magicKind = 0;
            //产生飞弹
            missile = Instantiate(missilePrefabs, missilePos.position, Quaternion.identity).GetComponent<Missile>();
        }
        else if (playerController.isMagic && Input.GetButtonUp("Magic"))
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
        else if (playerController.isMagic && Input.GetButton("Magic"))
        {
            if (missile != null && Input.GetButtonDown("Magic Storage"))
            {
                missile.SwitchMissileState(Missile.missileState.Storage);
                //放置到Magic Storage处
                if (missileStorge.Count < storagePos.Count)
                {
                    missile.transform.parent = storagePos[missileStorge.Count];
                    missile.transform.localPosition = Vector3.zero;
                    missile.transform.localScale = Vector3.one;
                    missileStorge.Add(missile.gameObject);
                    StartCoroutine(DestoryStorageMissileAfterTime());

                    missile = null;

                    playerController.isMagic = false;
                    playerController.magicKind = -1;
                }
            }
            //飞弹跟随
            if (missile != null)
            {
                missile.FollowCreatingPos(missilePos);
            }
        }
    }

    //储存魔法的操作输入检测
    public void StorageMissileOperatorInputCheck()
    {
        if (Input.GetButtonDown("Select Storage Missile") && missileStorge.Count != 0)
        {
            //时间变缓
            Time.timeScale = selectedScaleTime;
            //
            selectedStorageMissileIndex = 0;
        }
        else if (Input.GetButton("Select Storage Missile"))
        {
            if (missileStorge.Count == 0)
            {
                Time.timeScale = 1.0f;
                return;
            }
            if (selectedStorageMissileIndex >= missileStorge.Count)
            {
                selectedStorageMissileIndex = missileStorge.Count - 1;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedStorageMissileIndex--;
                selectedStorageMissileIndex = selectedStorageMissileIndex < 0 ? 0 : selectedStorageMissileIndex;
            } 
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedStorageMissileIndex++;
                selectedStorageMissileIndex = selectedStorageMissileIndex >= missileStorge.Count ? missileStorge.Count - 1 : selectedStorageMissileIndex;
            }
            //
            Debug.Log(selectedStorageMissileIndex + " : " + missileStorge[selectedStorageMissileIndex].transform.parent.name);
        }
        else if (Input.GetButtonUp("Select Storage Missile"))
        {
            Time.timeScale = 1.0f;
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