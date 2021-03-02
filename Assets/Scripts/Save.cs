using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public float playerPositionX;
    public float playerPositionY;
    public bool isNewGame;

    //摄像机信息
    public float cameraPositionX;
    public float cameraPositionY;

    //场景号
    public int sceneIndex;
}
