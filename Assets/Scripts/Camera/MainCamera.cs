using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(instance.gameObject);
        }
    }

    //方便用于延时
    public IEnumerator Shake(float duration, float migration)
    {
        Vector3 defaultLocalPostion = transform.localPosition;
        float curDuration = 0.0f;
        while (curDuration < duration) 
        {
            float x = Random.Range(-1, 1) * migration;
            float y = Random.Range(-1, 1) * migration;

            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z);
            curDuration += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = defaultLocalPostion;
    }

    
}
