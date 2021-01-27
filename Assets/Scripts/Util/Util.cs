using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static Util instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(this.gameObject);
        }
    }
    public void StartSlowTime(float timeScale, float duration)
    {
        StartCoroutine(SlowTime(timeScale, duration));
    }

    public IEnumerator SlowTime(float timeScale, float duration)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSeconds(duration);
        Time.timeScale = 1.0f;
    }

    //获取抛物线上的点
    public Vector3[] ProjectileArcPoints(int pointsNum, float speed, float distance, float gravity, float angle)
    {
        float size = distance / pointsNum;
        float radians = angle;

        Vector3[] points = new Vector3[pointsNum];
        for (int i = 0; i < pointsNum; i++)
        {
            float x = size * i;
            float t = x / (speed * Mathf.Cos(radians));
            //vt - 0.5gt^2;
            float y = (speed * Mathf.Sin(radians) * t) + 0.5f * gravity * t * t;
            Vector3 p = new Vector3(x, y, 0);
            points[i] = p;
        }
        return points;
    }
}
