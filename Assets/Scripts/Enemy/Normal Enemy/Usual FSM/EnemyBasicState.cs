using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBasicState
{
    public abstract void OnStart(NormalEnemy enemy);
    public abstract void OnUpdate(NormalEnemy enemy);
}
