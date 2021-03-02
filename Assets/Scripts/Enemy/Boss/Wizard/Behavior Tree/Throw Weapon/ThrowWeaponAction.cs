using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class ThrowWeaponAction : Action
{
    private GameObject player;

    public float cd;
    public SharedInt state;
    public float damage;

    public override void OnStart()
    {
        player = (Owner.GetVariable("player") as SharedGameObject).Value;
        Owner.SetVariable("state", state);
        //面向Player
        transform.rotation = transform.position.x < player.transform.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }

    public override TaskStatus OnUpdate()
    {
        if (!(Owner.GetVariable("isThrowWeaponOver") as SharedBool).Value && !(Owner.GetVariable("isThrowWeaponHit") as SharedBool).Value)
        {
            GameObject newWeapon = (Owner.GetVariable("newWeapon") as SharedGameObject).Value;
            if (newWeapon)
            {
                newWeapon.GetComponent<WizardWeapon>().damage = damage;
            }
            return TaskStatus.Running;
        }
        return TaskStatus.Success;
    }

    public override void OnEnd()
    {
        //Owner.SetVariable("haveWeapon", (SharedBool)!(Owner.GetVariable("haveWeapon") as SharedBool).Value);

        Owner.SetVariable("isThrowWeaponOver", (SharedBool)false);
        Owner.SetVariable("state", (SharedInt)0);
        Owner.SetVariable("canIdle", (SharedBool)true);
        Owner.SetVariable("canThrowWeapon", (SharedBool)false);
        StartCoroutine(Util.instance.SetSharedValueAfterTimeToBT(cd, Owner, "canThrowWeapon", (SharedBool)true));
    }
}
