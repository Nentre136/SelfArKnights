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
        //û������ ִ���߼�
        if (isDie == false)
        {
            //�赲
            if (obstructTarget != null)
                OnObstruct(obstructTarget);

            BodyMove();
            OnAttack();
        }
    }
}
