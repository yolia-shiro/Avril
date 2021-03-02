using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class WizardWeapon : MonoBehaviour
{
    private float moveSpeed;
    private bool isMove = false;
    private WizardController wizardController;
    public float damage;    //伤害值

    private void Start()
    {
        wizardController = GetComponentInParent<WizardController>() == null ? wizardController : GetComponentInParent<WizardController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeInHierarchy == false)
        {
            return;
        }
        if (isMove)
        {
            WeaponForward();
        }
        
    }

    public void SetMoveAttrAndStart(float speed, WizardController controller)
    {
        moveSpeed = speed;
        isMove = true;
        wizardController = controller;
    }
    public void WeaponForward()
    {
        transform.position = transform.position + transform.up * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Map Boundary"))
        {
            isMove = false;
            return;
        }
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (!isMove)
            {
                if (wizardController.state == 1)
                {
                    //冲刺
                    if (playerController.GetDamage(damage))
                    {
                        Debug.Log("Enemy Attack Player ---- Sprint");
                    }
                }
                else if(wizardController.state == 2)
                {
                    //挥砍
                    if (playerController.GetDamage(damage))
                    {
                        Debug.Log("Enemy Attack Player ---- Slash");
                    }
                }
            }
            else
            {
                if (playerController.GetDamage(damage))
                {
                    Debug.Log("Enemy Attack Player ----- Throw Weapon");
                    //wizardController.behaviorTree.SetVariable("isThrowWeaponHit", (SharedBool)true);
                }
            }
        }
    }
}
