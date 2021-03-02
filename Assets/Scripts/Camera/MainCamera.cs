using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : Singleton<MainCamera>
{
    public enum CameraState { Follow, Lock }

    private Camera curCamera;
    private CameraState curCameraState;     //相机当前状态

    [Header("Follow Parameter")]
    public Vector3 disToPlayer;
    public float smooth;

    [Header("Boundary")]
    public Collider2D boundaryColl;
    private Vector2 minBorder;
    private Vector2 maxBorder;
    private float orthographicSize;     //相机的一半高度
    private float viewHalfWeight;       //相机的一半宽度


    private void Start()
    {
        //transform.position = new Vector3(SaveManager.Instance.CameraPositionX, SaveManager.Instance.CameraPositionY, transform.position.z);
        curCamera = GetComponent<Camera>();
        curCameraState = CameraState.Follow;

        minBorder = boundaryColl.bounds.min;
        maxBorder = boundaryColl.bounds.max;
        orthographicSize = curCamera.orthographicSize;
        viewHalfWeight = orthographicSize * (float)Screen.width / (float)Screen.height;

        transform.position = new Vector3(GameManager.Instance.playerData.transform.position.x, 
            GameManager.Instance.playerData.transform.position.y, 
            transform.position.z);

        UpdateCameraPos();
    }

    private void LateUpdate()
    {
        switch (curCameraState)
        {
            case CameraState.Follow:
                UpdateCameraPos();
                break;
            case CameraState.Lock:
                break;
        }
    }

    public void TranslateToState(CameraState state)
    {
        curCameraState = state;
    }

    //方便用于延时
    public IEnumerator Shake(float duration, float migration)
    {
        Vector3 defaultLocalPostion = transform.localPosition;
        float curDuration = 0.0f;
        while (curDuration < duration) 
        {
            float x = Random.Range(-1, 1) * migration;
            float y = Random.Range(-1, 1) * migration;

            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z);
            curDuration += Time.unscaledDeltaTime;

            yield return null;
        }

        transform.localPosition = defaultLocalPostion;
    }


    /// <summary>
    /// 更新相机位置
    /// </summary>
    public void UpdateCameraPos()
    {
        Vector2 pos = curCamera.transform.position;
        if (GameManager.Instance.playerData.transform != null)
        {
            pos = Vector2.Lerp(curCamera.transform.position, GameManager.Instance.playerData.transform.position + disToPlayer, smooth * Time.deltaTime);
        }
        pos.x = Mathf.Clamp(pos.x, minBorder.x + viewHalfWeight, maxBorder.x - viewHalfWeight);
        pos.y = Mathf.Clamp(pos.y, minBorder.y + orthographicSize, maxBorder.y - orthographicSize);

        curCamera.transform.position = new Vector3(pos.x, pos.y, curCamera.transform.position.z);
    }

    /// <summary>
    /// 设置相机位置(协程)
    /// </summary>
    /// <param name="cameraPos"></param>
    /// <returns></returns>
    public IEnumerator SetCameraPos(Vector3 cameraPos)
    {
        while (Vector3.Distance(transform.position, cameraPos) > 0.1f) 
        {
            transform.position = Vector3.Lerp(transform.position, cameraPos, smooth * Time.fixedDeltaTime);
            yield return null;
        }
    }
}
