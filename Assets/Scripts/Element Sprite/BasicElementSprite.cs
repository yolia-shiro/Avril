using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 元素精灵
/// </summary>
public class BasicElementSprite : MonoBehaviour,IEndResonanceObserver
{
    public MagicType curType;  //当前元素精灵的属性（不存在无属性的元素精灵）
    public float moveSpeed;     //移动速度
    public float elementValue;  //向元素条中填充的数值
    private Vector2 originPos;  //起始位置

    public void Awake()
    {
        originPos = transform.position;
        //向ElementSpriteManager中注册
        ElementSpriteManager.Instance.RegisterEndResonanceObserver(this);
    }

    private void OnDisable()
    {
        //从ElementSpriteManager中注销
        ElementSpriteManager.Instance.LogOutEndResonanceObserver(this);
    }

    public IEnumerator MoveToPos(Vector3 targetPos)
    {
        while ((this != null && this.gameObject != null) && Vector3.Distance(transform.position, targetPos) >= 0.1f) 
        {
            if (this == null || this.gameObject == null)
            {
                StopCoroutine(MoveToPos(targetPos));
            }
            else 
            {
                //transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void EndNotify()
    {
        //延时删除的原因是为了防止注销之后，导致广播遍历时迭代器出现错误
        Destroy(gameObject, Time.deltaTime);
    }

    public void MoveNotify(Vector2 position)
    {
        StartCoroutine(MoveToPos(position));
    }
}
