using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏结束观察者
/// </summary>
public interface IEndGameObserver
{
    /// <summary>
    /// 结束游戏的广播
    /// </summary>
    void EndNotify();

    /// <summary>
    /// Boss进行相应，返回Boss所属的位置（第几个Boss）
    /// 非Boss对象返回-1
    /// </summary>
    /// <returns></returns>
    int BossResponse();
}
