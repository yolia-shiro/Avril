using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SavePosition : MonoBehaviour
{
    public GameObject saveTips;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!saveTips.activeInHierarchy)
        {
            saveTips.SetActive(true);
        }
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.F))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            //切换状态
            playerController.TranslateToState(playerController.playerSaveState);
            if (playerController != null)
            {
                //恢复玩家相关属性
                playerController.ResumePlayerDataToDefault();
            }

            //打开菜单
            UIManager.Instance.SetSaveMenuActive(true);
            //发送广播
            GameManager.Instance.NotifyObservers();
            //存档
            SaveManager.Instance.SavePlayeraData(); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (saveTips.activeInHierarchy)
        {
            saveTips.SetActive(false);
        }
    }

}
