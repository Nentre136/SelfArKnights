using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Mocock : EnemyControl
{
    protected new void Start()
    {
        base.Start();
        moveAnimeName = "Move";
    }
}
