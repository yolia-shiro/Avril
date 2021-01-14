using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    //测试强制继承之间强制转换是否会造成数据丢失
    // Start is called before the first frame update
    void Start()
    {
        Magic[] ms = GetComponents<Magic>();
        AttackMagic newAm = null;
        foreach (var m in ms)
        {
            if (newAm == null)
            {
                newAm = (AttackMagic)m;
            }
            else
            {
                //newAm.SetAttackMagicType(newAm.isNormal | ((AttackMagic)m).isNormal
                //    , newAm.isDrag | ((AttackMagic)m).isDrag
                //    , newAm.isTrack | ((AttackMagic)m).isTrack);
            }
        }
        
        Debug.Log("isNormal : " + newAm.isNormal.ToString() + ", isDrag : " + newAm.isDrag.ToString() + ", isTrack : " + newAm.isTrack.ToString());
    }
}
