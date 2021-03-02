using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    public float alpha;
    private float oldAlpha;
    private float targetAlpha;
    public float duration;

    private Image image;

    // Start is called before the first frame update
    void OnEnable()
    {
        image = GetComponent<Image>();
        oldAlpha = image.color.a;
        targetAlpha = oldAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        if (image.canvasRenderer.GetAlpha() == targetAlpha)
        {
            targetAlpha = targetAlpha == alpha ? oldAlpha : alpha;
            image.CrossFadeAlpha(targetAlpha, duration, true);
        }
    }
}
