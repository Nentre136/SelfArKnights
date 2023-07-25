using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Demkni : RoleControl
{
    public new void OnAttack()
    {
        //存在攻击对象
        if (attackTarget != null)
        {
            //敌人不在攻击范围内
            if (isInAttackRange(attackTarget) == false && currentObstructCount == 0)    
            {
                attackTarget = null;
                //return;
            }
            if (obstructTargets.Count > 0)//存在阻挡对象
            {
                attackTarget = obstructTargets[0];
            }
            if (isAttack == false)//进入攻击状态
                StartCoroutine(AttackTime());

        }

        //不存在攻击对象
        if (attackTarget == null)
        {
            if (obstructTargets.Count > 0)//存在阻挡对象
            {
                attackTarget = obstructTargets[0];
                return;
            }
            foreach (GameObject enemy in gameManege.enemyList)
            {
                if (isInAttackRange(enemy))
                {
                    attackAnimeName = "Attack";
                    attackTarget = enemy.gameObject;//在范围内,则刷新攻击目标
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
            //阻挡判定
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
