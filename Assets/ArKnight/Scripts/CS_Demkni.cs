using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Demkni : RoleControl
{
    public new void OnAttack()
    {
        //���ڹ�������
        if (attackTarget != null)
        {
            //���˲��ڹ�����Χ��
            if (isInAttackRange(attackTarget) == false && currentObstructCount == 0)    
            {
                attackTarget = null;
                //return;
            }
            if (obstructTargets.Count > 0)//�����赲����
            {
                attackTarget = obstructTargets[0];
            }
            if (isAttack == false)//���빥��״̬
                StartCoroutine(AttackTime());

        }

        //�����ڹ�������
        if (attackTarget == null)
        {
            if (obstructTargets.Count > 0)//�����赲����
            {
                attackTarget = obstructTargets[0];
                return;
            }
            foreach (GameObject enemy in gameManege.enemyList)
            {
                if (isInAttackRange(enemy))
                {
                    attackAnimeName = "Attack";
                    attackTarget = enemy.gameObject;//�ڷ�Χ��,��ˢ�¹���Ŀ��
                    return;
                }
            }
        }

    }
    protected new void FixedUpdate()
    {
        if (isPut == false)
            return;

        if (isDie == false)
        {
            //�赲�ж�
            if (currentObstructCount >= obstructCount)
                isObstruct = false;
            else
                isObstruct = true;

            if (isAttack == false)
                OnIdle();
            OnAttack();
        }
    }
}
