using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElementSprite 
{
    private MagicType type;
    private Vector2 pos;
    public MagicType Type { get { return type; } }
    public Vector2 Pos { get { return pos; } }

    public ElementSprite()
    { 
        
    }

    public ElementSprite(MagicType type, Vector2 pos)
    {
        this.type = type;
        this.pos = pos;
    }
}

/// <summary>
/// 管理整个场景的元素精灵
/// </summary>
public class ElementSpriteManager : Singleton<ElementSpriteManager>
{
    public List<GameObject> elementPrefabs = new List<GameObject>();
    public LayerMask elementLayer;
    public int elementNum;  //场景内生成的元素精灵总数
    public BoxCollider2D createArea;    //创造的范围
    public Transform elementParent;    //存储用父物体
    private Vector2 leftBottomPos, rightTopPos;
    public List<ElementSprite> elements = new List<ElementSprite>();
    public Dictionary<BasicElementSprite, ElementSprite> elementGameObjectToData = new Dictionary<BasicElementSprite, ElementSprite>();
    public List<IEndResonanceObserver> endResonanceObservers = new List<IEndResonanceObserver>();

    protected override void Awake()
    {
        base.Awake();
        leftBottomPos = createArea.bounds.min;
        rightTopPos = createArea.bounds.max;
        CreateSprites();
    }

    /// <summary>
    /// 观察者模式
    /// 注册观察对象
    /// </summary>
    /// <param name="endResonanceObserver"></param>
    public void RegisterEndResonanceObserver(IEndResonanceObserver endResonanceObserver)
    {
        endResonanceObservers.Add(endResonanceObserver);
    }

    /// <summary>
    /// 观察者模式
    /// 注销观察对象
    /// </summary>
    /// <param name="endResonanceObserver"></param>
    public void LogOutEndResonanceObserver(IEndResonanceObserver endResonanceObserver)
    {
        if (endResonanceObservers.Contains(endResonanceObserver))
        {
            endResonanceObservers.Remove(endResonanceObserver);
        }
    }

    /// <summary>
    /// 广播
    /// 结束共鸣后，销毁所有注册对象
    /// </summary>
    public void EndNotify()
    {
        
        foreach (var endResonanceObserver in endResonanceObservers)
        {
            endResonanceObserver.EndNotify();
        }
    }
    /// <summary>
    /// 广播
    /// 共鸣过程中，让所有注册对象移动到指定地点
    /// </summary>
    /// <param name="position"></param>
    public void MoveNotify(Vector2 position)
    {
        foreach (var endResonanceObserver in endResonanceObservers)
        {
            endResonanceObserver.MoveNotify(position);
        }
    }

    /// <summary>
    /// 生成场景内的元素精灵信息
    /// </summary>
    public void CreateSprites()
    {
        for (int i = 0; i < elementNum; i++) 
        {
            //获取随机位置
            float x = Random.Range(leftBottomPos.x, rightTopPos.x);
            float y = Random.Range(leftBottomPos.y, rightTopPos.y);
            //获取随机类型
            int type = Random.Range(1, 4);
            ElementSprite elementSprite = new ElementSprite((MagicType)type, new Vector2(x, y));
            elements.Add(elementSprite);
        }
    }

    /// <summary>
    /// 获取在玩家共鸣范围内的相应的元素精灵信息
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    public List<ElementSprite> GetInResonanceArea(Vector2 position, float radius, MagicType type)
    {
        List<ElementSprite> conformElements = new List<ElementSprite>();
        for (int i = 0; i < elements.Count; i++) 
        {
            float dis = Vector2.Distance(position, elements[i].Pos);
            if (dis <= radius && elements[i].Type == type)
            {
                conformElements.Add(elements[i]);
            }
        }
        return conformElements;
    }

    /// <summary>
    /// 在指定位置生成指定类型的元素精灵预构体
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public BasicElementSprite CreateElementSpritePrefabs(Vector2 pos, MagicType type)
    {
        GameObject go = Instantiate(elementPrefabs[(int)type - 1], pos, Quaternion.identity);
        go.transform.SetParent(elementParent);
        return go.GetComponent<BasicElementSprite>();
    }

    /// <summary>
    /// 在玩家共鸣范围内创建元素精灵实体
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="type"></param>
    public void CreateElementSpriteGameObjectInResonanceArea(Vector2 position, float radius, MagicType type)
    {
        //获取数据信息
        List<ElementSprite> conformElements = GetInResonanceArea(position, radius, type);
        //实例化
        foreach (var element in conformElements)
        {
            BasicElementSprite elementGameObject = CreateElementSpritePrefabs(element.Pos, element.Type);
            //将实例和数据进行映射
            elementGameObjectToData.Add(elementGameObject, element);
        }
    }

    /// <summary>
    /// 根据实例删除存储的数据
    /// </summary>
    /// <param name="removedBaiscElementSprite"></param>
    public void RemoveElementSpriteData(BasicElementSprite removedBaiscElementSprite)
    {
        if (elementGameObjectToData.ContainsKey(removedBaiscElementSprite))
        {
            ElementSprite removed = elementGameObjectToData[removedBaiscElementSprite];
            if (elements.Contains(removed))
            {
                elements.Remove(removed);
            }
        }
    }

    /// <summary>
    /// 在指定的圆形范围内生成指定类型的元素精灵，生成数量由剩余共鸣值决定
    /// </summary>
    /// <param name="type">元素类型</param>
    /// <param name="point">圆心位置</param>
    /// <param name="radius">半径</param>
    /// <param name="resonanceValue">剩余共鸣值</param>
    public void CreateTypeElementSpriteInResonanceArea(MagicType type, Vector3 point, float radius, float resonanceValue)
    {
        float perResonanceValue = elementPrefabs[(int)type - 1].GetComponent<BasicElementSprite>().elementValue;
        int createNum = (int)(resonanceValue / perResonanceValue);
        for (int i = 0; i < createNum; i++) 
        {
            float r = Random.Range(0, radius);
            float angle = Random.Range(0, 360.0f) * Mathf.Deg2Rad;
            float x = r * Mathf.Cos(angle);
            float y = r * Mathf.Sin(angle);
            elements.Add(new ElementSprite(type, new Vector2(point.x + x, point.y + y)));
        }
    }
}
