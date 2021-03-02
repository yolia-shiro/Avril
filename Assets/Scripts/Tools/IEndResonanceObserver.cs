using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEndResonanceObserver
{
    /// <summary>
    /// 结束共鸣时进行的广播
    /// </summary>
    void EndNotify();
    /// <summary>
    /// 移动时进行的广播
    /// </summary>
    /// <param name="position"></param>
    void MoveNotify(Vector2 position);
}
