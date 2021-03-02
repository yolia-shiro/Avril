using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

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

    //定时设置物体的Active(显隐)
    //默认为隐
    public IEnumerator SetObjActiveAfterTime(GameObject obj, float time, bool active = false)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(active);
    }

    //移动到指定位置
    public void MoveToTargetPos(Transform curObj, Vector3 target, bool blink, float speed = 10000.0f)
    {
        if (blink)
        {
            //瞬移
            curObj.position = target;
        }
        else
        {
            //匀速
            curObj.position = Vector3.MoveTowards(curObj.position, target, speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 计时设置值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="time"></param>
    /// <param name="origin"></param>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    public IEnumerator SetValueAfterTime<T>(float time, T origin, T targetValue)
    {
        yield return new WaitForSeconds(time);
        origin = targetValue;
    }

    /// <summary>
    /// 计时设置值（行为树用）
    /// </summary>
    /// <param name="time"></param>
    /// <param name="bt"></param>
    /// <param name="name"></param>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    public IEnumerator SetSharedValueAfterTimeToBT(float time, Behavior owner, string name, SharedVariable targetValue)
    {
        yield return new WaitForSeconds(time);
        owner.SetVariable(name, targetValue);
    }


    [HideInInspector]public bool isWaitOver;
    /// <summary>
    /// 计时
    /// </summary>
    /// <returns></returns>
    public IEnumerator Wait(float time)
    {
        isWaitOver = false;
        yield return new WaitForSeconds(time);
        isWaitOver = true;
    }

    /// <summary>
    /// 获取摄像机范围内的指定tag的对象集合
    /// </summary>
    /// <param name="tag">标签</param>
    /// <returns>指定tag的对象集合</returns>
    public List<Transform> GetTransformsInCameraArea(string tag)
    {
        List<Transform> trackTarget = new List<Transform>();
        float orthograhicSize = Camera.main.orthographicSize;
        float halfWidth = orthograhicSize * ((float)Screen.width / (float)Screen.height);
        Vector2 leftTop = new Vector2(Camera.main.transform.position.x - halfWidth, Camera.main.transform.position.y + orthograhicSize);
        Vector2 rightBottom = new Vector2(Camera.main.transform.position.x + halfWidth, Camera.main.transform.position.y - orthograhicSize);
        Collider2D[] viewObjs = Physics2D.OverlapAreaAll(leftTop, rightBottom);
        //获取摄像机范围内的Enemy
        foreach (var viewObj in viewObjs)
        {
            if (viewObj.CompareTag(tag))
            {
                trackTarget.Add(viewObj.transform);
            }
        }
        return trackTarget;
    }


    /// <summary>
    /// 获取摄像机范围内的指定tag的对象集合，并按照myCompare自定义的规则进行排序
    /// </summary>
    /// <param name="tag">标签</param>
    /// <param name="myCompare">自定义比较函数</param>
    /// <param name="res">输出</param>
    public void GetTransformsInCameraAreaSorted(string tag, IComparer<Transform> myCompare, out SortedSet<Transform> res)
    {
        SortedSet<Transform> trackTarget = new SortedSet<Transform>(myCompare);
        float orthograhicSize = Camera.main.orthographicSize;
        float halfWidth = orthograhicSize * ((float)Screen.width / (float)Screen.height);
        Vector2 leftTop = new Vector2(Camera.main.transform.position.x - halfWidth, Camera.main.transform.position.y + orthograhicSize);
        Vector2 rightBottom = new Vector2(Camera.main.transform.position.x + halfWidth, Camera.main.transform.position.y - orthograhicSize);
        Collider2D[] viewObjs = Physics2D.OverlapAreaAll(leftTop, rightBottom);
        //获取摄像机范围内的Enemy
        foreach (var viewObj in viewObjs)
        {
            if (viewObj.CompareTag(tag))
            {
                trackTarget.Add(viewObj.transform);
            }
        }
        res = trackTarget;
    }

    /// <summary>
    /// 获取摄像机范围内的指定tag的对象集合，并按照myCompare自定义的规则进行排序
    /// </summary>
    /// <param name="tag">标签</param>
    /// <param name="myCompare">自定义比较函数</param>
    /// <param name="res">输出</param>
    public void GetTransformsInCameraAreaSorted(string tag, IComparer<Transform> myCompare, out List<Transform> res)
    {
        SortedSet<Transform> trackTargets = new SortedSet<Transform>();
        GetTransformsInCameraAreaSorted(tag, myCompare, out trackTargets);
        List<Transform> trackTargetsList = new List<Transform>();
        foreach (var target in trackTargets)
        {
            trackTargetsList.Add(target);
        }
        res = trackTargetsList;
    }
}
