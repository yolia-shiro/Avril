using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCannonState : EnemyBasicState
{
    public override void OnEnter(WizardController wizardController)
    {
        wizardController.MagicCannonPrePreparation();
    }

    public override void OnUpdate(WizardController wizardController)
    {
        wizardController.MagicCannon();
    }
}
