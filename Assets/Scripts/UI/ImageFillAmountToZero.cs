using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFillAmountToZero : MonoBehaviour
{
    public bool isStart = false;
    public Image opImage;
    public float duration;
    private float curTime = 0.0f;
    
    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            curTime += Time.deltaTime;
            opImage.fillAmount = Mathf.Lerp(1, 0, curTime / duration);
            if (curTime >= duration) 
            {
                //结束
                Destroy(this.gameObject);
            }
        }
    }

    public void TimeStart(Sprite img, float duration)
    {
        this.duration = duration;
        GetComponent<Image>().sprite = img;
        opImage.sprite = img;
        isStart = true;
        
    }
}
