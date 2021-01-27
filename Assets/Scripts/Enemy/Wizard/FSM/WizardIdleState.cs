using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardIdleState : EnemyBasicState
{
    public override void OnEnter(WizardController wizardController)
    {
        wizardController.state = 0;
    }

    public override void OnUpdate(WizardController wizardController)
    {
    }
}
