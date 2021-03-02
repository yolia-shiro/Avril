using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicForward : MonoBehaviour
{
    public float speed;
    public float duration;
    public float hit;

    public GameObject hitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterDuration());
    }

    // Update is called once per frame
    void Update()
    {
        Forward();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!collision.GetComponent<PlayerController>().GetDamage(hit))
            {
                Debug.Log($"{name}攻击未生效");
            }
            CreateHitEffect(transform.position, Quaternion.identity);
        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            CreateHitEffect(transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void Forward()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    public IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    //创建结束的特效
    public void CreateHitEffect(Vector3 pos, Quaternion rot)
    {
        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot) as GameObject;

            hitVFX.transform.right = gameObject.transform.right;

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
}
