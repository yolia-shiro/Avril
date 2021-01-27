using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBasicState
{
    public abstract void OnEnter(WizardController wizardController);

    public abstract void OnUpdate(WizardController wizardController);
}
