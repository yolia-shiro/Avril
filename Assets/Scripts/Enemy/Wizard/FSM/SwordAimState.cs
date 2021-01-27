using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAimState : EnemyBasicState
{
    public override void OnEnter(WizardController wizardController)
    {
        wizardController.SwordAimPrePreparation();
    }

    public override void OnUpdate(WizardController wizardController)
    {
        wizardController.SwordAim();
    }
}
