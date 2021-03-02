using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SlashAction : Action
{
    public SharedInt state;
    public float damage;

    private Vector3 playerPosition;
    public override void OnStart()
    {
        Owner.SetVariable("state", state);
        Owner.SetVariable("isSlash", (SharedBool)true);
        (Owner.GetVariable("weapon") as SharedGameObject).Value.GetComponent<WizardWeapon>().damage = damage;

        playerPosition = GameManager.Instance.playerData.transform.position;
        //转朝向
        transform.rotation = playerPosition.x < transform.position.x ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }

    public override TaskStatus OnUpdate()
    {
        if ((Owner.GetVariable("isSlash") as SharedBool).Value)
        {
            return TaskStatus.Running;
        }
        return TaskStatus.Success;
    }

    public override void OnEnd()
    {
        Owner.SetVariable("canIdle", (SharedBool)true);
    }
}
