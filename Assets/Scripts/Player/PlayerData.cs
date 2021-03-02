using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//存放用户数据信息
public class PlayerData : MonoBehaviour
{
    public PlayerStatus_SO templateData_SO;
    public PlayerStatus_SO playerStatus_SO;

    #region 数据存储
    public float MaxHP
    {
        get
        {
           if (playerStatus_SO != null)
            {
                return playerStatus_SO.maxHP;
            }
            else
            {
                return 0;
            }
        }
        set 
        {
            playerStatus_SO.maxHP = value;
        }
    }
    public float CurHP
    {
        get
        {
            if (playerStatus_SO != null)
            {
                return playerStatus_SO.curHP;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            playerStatus_SO.curHP = value;
        }
    }
    public float MaxMP
    {
        get
        {
            if (playerStatus_SO != null)
            {
                return playerStatus_SO.maxMP;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            playerStatus_SO.maxMP = value;
        }
    }
    public float CurMP
    {
        get
        {
            if (playerStatus_SO != null)
            {
                return playerStatus_SO.curMP;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            playerStatus_SO.curMP = value;
        }
    }
    public float RestoreMPPerFrame
    {
        get
        {
            if (playerStatus_SO != null)
            {
                return playerStatus_SO.restoreMPPerFrame;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            playerStatus_SO.restoreMPPerFrame = value;
        }
    }
    public float MaxBP
    {
        get
        {
            if (playerStatus_SO != null)
            {
                return playerStatus_SO.maxBP;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            playerStatus_SO.maxBP = value;
        }
    }
    public float CurBP
    {
        get
        {
            if (playerStatus_SO != null)
            {
                return playerStatus_SO.curBP;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            playerStatus_SO.curBP = value;
        }
    }
    public int CurResonanceType
    {
        get
        {
            if (playerStatus_SO != null)
            {
                return playerStatus_SO.curResonanceType;
            }
            else
            {
                return -1;
            }
        }
        set 
        {
            playerStatus_SO.curResonanceType = value;
        }
    }
    public float MaxResonanceValue
    {
        get
        {
            if (playerStatus_SO != null)
            {
                return playerStatus_SO.maxResonanceValue;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            playerStatus_SO.maxResonanceValue = value;
        }
    }
    public float CurResonanceValue
    {
        get
        {
            if (playerStatus_SO != null)
            {
                return playerStatus_SO.curResonanceValue;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            playerStatus_SO.curResonanceValue = value;
        }
    }
    public float ResonanceRadius
    {
        get
        {
            if (playerStatus_SO != null)
            {
                return playerStatus_SO.resonanceRadius;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            playerStatus_SO.resonanceRadius = value;
        }
    }
    #endregion


    private void Awake()
    {
        if (templateData_SO != null)
        {
            playerStatus_SO = Instantiate(templateData_SO);
        }
    }

}
