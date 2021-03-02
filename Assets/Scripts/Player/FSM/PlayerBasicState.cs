using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBasicState
{
    public abstract void OnEnter(PlayerController player);
    public abstract void OnUpdate(PlayerController player);
    public abstract void OnFixedUpdate(PlayerController player);
}
