using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchResonanceMenu : MonoBehaviour
{

    [Header("Switch Resonance Menu")]
    public Transform selectionBox;
    private int curSelectIndex;
    private int maxSelectNum;

    [Header("Event")]
    public EventSystem system;

    private void OnEnable()
    {
        curSelectIndex = 0;
        maxSelectNum = transform.childCount;
        selectionBox.SetParent(transform.GetChild(curSelectIndex));
        selectionBox.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        system.SetSelectedGameObject(transform.GetChild(curSelectIndex).gameObject);
        Debug.Log(system.currentSelectedGameObject.name);
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            curSelectIndex--;
            curSelectIndex = curSelectIndex < 0 ? 0 : curSelectIndex;
            selectionBox.SetParent(transform.GetChild(curSelectIndex));
            selectionBox.localPosition = Vector3.zero;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            curSelectIndex++;
            curSelectIndex = curSelectIndex >= maxSelectNum ? maxSelectNum - 1 : curSelectIndex;
            selectionBox.SetParent(transform.GetChild(curSelectIndex));
            selectionBox.localPosition = Vector3.zero;
        }
    }

    public int GetSelectedIndex()
    {
        return curSelectIndex;
    }
}
