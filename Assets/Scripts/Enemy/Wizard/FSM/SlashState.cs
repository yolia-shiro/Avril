using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashState : EnemyBasicState
{
    public override void OnEnter(WizardController wizardController)
    {
        //设置状态
        wizardController.state = 2;
    }

    public override void OnUpdate(WizardController wizardController)
    {
        
    }
}
