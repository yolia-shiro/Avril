using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debuff : MonoBehaviour
{
    public Image bossDebuffFill;   //Debuff填充层
    public Text bossDebuffTypeText;     //Debuff类型
    public Text bossDebuffLayerNumText; //Debuff层数

    private float duration; //持续时间
    private float curDuration;  //当前持续时间
    private int bossDebuffLayerNum;

    public void Start()
    {
        //bossDebuffLayerNum = int.Parse(bossDebuffLayerNumText.text);
        //StartCoroutine(Begin());
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Debuff test"))
        {
            Restart();
        }
    }

    public IEnumerator Begin()
    {
        while (bossDebuffLayerNum > 0) 
        {
            while (curDuration < duration) 
            {
                curDuration += Time.fixedDeltaTime;
                bossDebuffFill.fillAmount -= (Time.fixedDeltaTime / duration);
                //yield return new WaitForSeconds(Time.fixedDeltaTime);
                yield return null;
            }
            curDuration = 0;
            bossDebuffFill.fillAmount = 1;
            bossDebuffLayerNum--;
            bossDebuffLayerNumText.text = bossDebuffLayerNum.ToString();
        }
        //UIManager.instance.RemoveBossDebuff(bossDebuffTypeText.text);
        Destroy(this.gameObject);
    }

    public void Restart()
    {
        StopAllCoroutines();
        curDuration = 0;
        bossDebuffFill.fillAmount = 1;
        StartCoroutine(Begin());
    }

    public void SetAttr(string type, int layerNum, float duration) 
    {
        this.duration = duration;
        bossDebuffFill.fillAmount = 1;
        bossDebuffTypeText.text = type;
        bossDebuffLayerNum = layerNum;
        bossDebuffLayerNumText.text = layerNum.ToString();
    }
}
