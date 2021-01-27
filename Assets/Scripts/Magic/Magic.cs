using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    //魔法的所有状态值的枚举值
    public enum MagicState { Creating, Effective, ToStorage, Storage, Attachment}

    [Header("Magic Data")]
    public float magicConsumptionPerFrame;  //每帧魔力消耗量


    [Header("Creating State")]
    public float createSpeed;
    public float maxScale;
    [HideInInspector] public bool isCreateOver;

    [Header("To Storage State")]
    public float disappearSpeed;
    [Header("Storage State")]
    public float radius;
    public float storageSpeed;
    private Vector3 storageBeginLocalPos;
    private Vector3 randomDir;
    
    [Header("Effect Prefabs")]
    public GameObject hitPrefab;

    protected MagicState state;
    protected Vector3 targetScale;

    public virtual void Awake()
    {
        state = MagicState.Creating;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        targetScale = transform.localScale * maxScale;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //判断是否创建完毕魔法(不消耗魔力的阶段)
        isCreateOver = isCreateOver || state != MagicState.Creating;
        switch (state)
        {
            case MagicState.Creating:
                Creating();
                break;
            case MagicState.Effective:
                Effective();
                break;
            case MagicState.ToStorage:
                ToStorage();
                break;
            case MagicState.Storage:
                Storage();
                break;
            case MagicState.Attachment:
                Attachment();
                break;
        }
    }
    
    //创造状态下执行的动作
    public virtual void Creating()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, createSpeed * Time.deltaTime);
        isCreateOver = CheckCreateOver();
    }
    //判断创造状态是否结束
    public virtual bool CheckCreateOver()
    {
        if (Vector3.Distance(transform.localScale, targetScale) < 0.1f) 
        {
            return true;
        }
        return false;
    }

    //生效状态下执行的动作
    public virtual void Effective()
    { 

    }
    //设置向储存状态过渡期间的参数
    public void SetToStorageAttr(Vector3 pos)
    {
        transform.localPosition = pos;
        storageBeginLocalPos = transform.localPosition;
        randomDir = GetRandomPosInSphere();
    }
    //向储存状态过渡期间执行的动作
    public virtual void ToStorage()
    {
    }
    //储存期间执行的动作
    public virtual void Storage()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, storageBeginLocalPos + randomDir, storageSpeed * Time.deltaTime);
        if (Vector3.Magnitude(transform.localPosition - (storageBeginLocalPos + randomDir)) <= 0.01)
        {
            randomDir = GetRandomPosInSphere();
        }
    }

    //魔力附着期间的行为
    public virtual void Attachment() 
    {
        
    }

    //切换状态
    public void SwitchMissileState(MagicState state)
    {
        this.state = state;
    }

    //跟随特定目标
    public void FollowCreatingPos(Transform creatingPos)
    {
        transform.position = creatingPos.position;
    }

    //储存魔法用
    //随时间变换大小
    public IEnumerator MissileToTargetScale(Vector3 targetScale)
    {
        while (this != null && Vector3.Magnitude(transform.localScale - targetScale) > 0.1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, disappearSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    //创建结束的特效
    public void CreateHitEffect(Vector3 pos, Quaternion rot)
    {
        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot) as GameObject;

            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null)
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        }
        StartCoroutine(DestroyParticle(0.0f));
    }

    //消除粒子效果
    public IEnumerator DestroyParticle(float waitTime)
    {

        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                tList.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
    //确定圆形范围的随机位置
    //用于储存状态下的自动位置偏移
    public Vector3 GetRandomPosInSphere()
    {
        float angle = Random.Range(0, Mathf.PI);
        float r = Random.Range(0, radius);
        return new Vector3(r * Mathf.Cos(angle), r * Mathf.Sin(angle), 0);
    }
}
