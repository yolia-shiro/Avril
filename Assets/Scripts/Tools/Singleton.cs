using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 单例模板
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance 
    {
        get 
        {
            return instance;
        }
    }
    public static bool IsInitialized
    {
        get
        {
            return instance != null;
        }
    }
    
    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }

    //销毁单例
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
