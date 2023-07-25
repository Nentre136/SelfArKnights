using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Bugger : EnemyControl
{
    private new void Start()
    {
        base.Start();
    }
    private new void FixedUpdate()
    {
        //Ã»ÓÐËÀÍö Ö´ÐÐÂß¼­
        if (isDie == false)
        {
            //×èµ²
            if (obstructTarget != null)
                OnObstruct(obstructTarget);

            BodyMove();
            OnAttack();
        }
    }
}
