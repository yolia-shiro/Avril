using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintState : EnemyBasicState
{
    public override void OnEnter(WizardController wizardController)
    {
        //进行冲刺攻击的前置操作
        wizardController.SprintPrePreparation();
    }

    public override void OnUpdate(WizardController wizardController)
    {
        if (wizardController.isPreSprintOver)
        {
            wizardController.Sprint();
        }
    }
}
