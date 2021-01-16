using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    public Text showText;
    private static string text;

    // Update is called once per frame
    void Update()
    {
        showText.text = text;
        
    }

    public static void SetText(string t)
    {
        text = t + '\n';
    }
}
