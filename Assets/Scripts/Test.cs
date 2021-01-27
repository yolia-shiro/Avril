using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    //public float ShotSpeed = 10; // 抛出的速度
    //private float time;          // A-B的时间
    //public Transform pointA;     // 起点
    //public Transform pointB;     // 终点
    //public float g = -10;        // 重力加速度
    //private Vector3 speed;       // 初速度向量
    //private Vector3 Gravity;                // 重力向量
    //private Vector3 currentAngle;// 当前角度

    public int pointsNum;
    public float speed;
    public float distance;
    public float gravity;
    public float angle;
    public Vector3 direction;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        UpdateArc(speed, distance, gravity, Mathf.Deg2Rad * transform.eulerAngles.z, direction, true);
    }

    //获取抛物线上的点
    public Vector3[] ProjectileArcPoints(int pointsNum, float speed, float distance, float gravity, float angle)
    {
        float size = distance / pointsNum;
        float radians = angle;

        Vector3 []points = new Vector3[pointsNum];
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

    //可视化
    public void UpdateArc(float speed, float distance, float gravity, float angle, Vector3 direction, bool vaild)
    {
        Vector3 []arcPoints = ProjectileArcPoints(pointsNum, speed, distance, gravity, angle);
        Vector3[] points = new Vector3[pointsNum];
        for (int i = 0; i < pointsNum; i++)
        {
            points[i] = transform.position + new Vector3((transform.right.x < 0 ? -1 : 1) * arcPoints[i].x, arcPoints[i].y);
        }
        lineRenderer.positionCount = pointsNum;
        lineRenderer.SetPositions(points);
    }
}
