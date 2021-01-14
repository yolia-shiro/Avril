using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public enum missileState { Creating, Lauching, ToStorage, Storage }

    [Header("Creating State")]
    public float createSpeed;
    public float maxScale;
    [Header("Lauching State")]
    public float speed;    
    public float existTime;
    [Header("To Storage State")]
    public float disappearSpeed;
    [Header("Storage State")]
    public float radius;
    public float storageSpeed;
    public Vector3 storageBeginLocalPos;
    public Vector3 randomDir;

    [Tooltip("From 0% to 100%")]
    public float accuracy;
    public float fireRate;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public List<GameObject> trails;

    private Vector3 offset;

    private Vector3 targetScale;
    private missileState state;
    

    // Update is called once per frame

    private void OnEnable()
    {
        state = missileState.Creating;
    }

    private void Start()
    {
        //used to create a radius for the accuracy and have a very unique randomness
        if (accuracy != 100)
        {
            accuracy = 1 - (accuracy / 100);

            for (int i = 0; i < 1; i++)
            {
                var val = 1 * Random.Range(-accuracy, accuracy);
                var index = Random.Range(0, 1);
                if (i == 0)
                {
                    if (index == 0)
                        offset = new Vector3(0, -val, 0);
                    else
                        offset = new Vector3(0, val, 0);
                }
                else
                {
                    offset = new Vector3(0, offset.y, 0);
                }
            }
        }

        targetScale = transform.localScale * maxScale;
    }

    private void Update()
    {
        switch (state)
        {
            case missileState.Creating:
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, createSpeed * Time.deltaTime);
                break;
            case missileState.Lauching:
                Forward();
                break;
            case missileState.ToStorage:
                break;
            case missileState.Storage:
                transform.localPosition = Vector3.Lerp(transform.localPosition, storageBeginLocalPos + randomDir, storageSpeed * Time.deltaTime);
                if (Vector3.Magnitude(transform.localPosition - (storageBeginLocalPos + randomDir)) <= 0.01)
                {
                    randomDir = GetRandomPosInSphere();
                }
                break;
        }
    }

    //开始飞行
    public void Forward()
    {
        //消失计时开始
        StartCoroutine(WaitForDestory());
        transform.position = transform.position + (transform.right + offset) * speed * Time.deltaTime;
    }

    //切换状态
    public void SwitchMissileState(missileState state)
    {
        this.state = state;
    }

    //创造状态跟随
    public void FollowCreatingPos(Transform creatingPos)
    {
        transform.position = creatingPos.position;
    }

    //确定圆形范围的随机位置
    public Vector3 GetRandomPosInSphere()
    {
        float angle = Random.Range(0, Mathf.PI);
        float r = Random.Range(0, radius);
        return new Vector3(r * Mathf.Cos(angle), r * Mathf.Sin(angle), 0);
    }


    //创建发射时的起始特效
    public void CreateMuzzleEffect()
    {
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward + offset;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    public IEnumerator MissileToTargetScale(Vector3 targetScale)
    {
        while (this != null && Vector3.Magnitude(transform.localScale - targetScale) > 0.1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, disappearSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator WaitForDestory() 
    {
        yield return new WaitForSeconds(existTime);
        if (this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }
}
