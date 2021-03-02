using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutAndDestroy : MonoBehaviour
{
    public float fadeOutTimer;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Color fade = spriteRenderer.color;
        fade.a = Mathf.MoveTowards(fade.a, 0, 1/fadeOutTimer * Time.deltaTime);
        spriteRenderer.color = fade;


        if (fade.a <= 0)
            gameObject.SetActive(false);
    }
}
