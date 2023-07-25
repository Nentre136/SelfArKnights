using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_R_11 : EnemyControl
{
    public int attackCount = 4;//攻击人数
    private List<GameObject> attackTargetList;
    new void Start()
    {
        base.Start();
        moveAnimeName = "Move";
        diedAnimeName = "Die_2";
        attackTargetList = new List<GameObject>();
    }
    new void FixedUpdate()
    {

        //没有死亡 执行逻辑
        if (isDie == false)
        {
            //阻挡
            if (obstructTarget != null)
                OnObstruct(obstructTarget);

            BodyMove();
            OnAttack();
        }
    }

    new void OnAttack()
    {
        if (isAttack)
            return;
        //攻击间隔
        if (isAttack == false && attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
            return;
        }

        //存在攻击对象
        if (attackTargetList.Count>0)
        {

            //敌人不在攻击范围内
            //if (isObstruct == false && isInAttackRange(attackTarget) == false && isAttack == false)
            //{
            //    attackTarget = null;
            //    return;
            //}

            //进入攻击状态
            if (isAttack == false && attackTimer <= 0)
            {
                if (attackTargetList.Count > 0)
                    StartCoroutine(AttackTime());
            }

        }


        //攻击对象不满
        if (attackTargetList.Count<attackCount)
        {
            int count = attackTargetList.Count;
            //从后面提取角色  后放置优先被攻击
            for (int index = gameManege.playerList.Count - 1; index >= 0; index--)
            {
                GameObject target = gameManege.playerList[index];
                if (count >= attackCount)
                    return;
                if (!attackTargetList.Contains(target) && isInAttackRange(target))
                {
                    //在范围内,则刷新攻击目标
                    attackTargetList.Add(target);
                    count++;
                }
            }
        }
    }
    new IEnumerator AttackTime()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimeName, false);//轨道 - Name - Loop
        isAttack = true;
        yield return new WaitForSeconds(attackAnimeTime * 0.4f);//前摇

        foreach (GameObject target in attackTargetList)
        {
            if (target != null)
                target.GetComponent<RoleControl>().BeAttack(gameObject);
            else
                attackTargetList.Remove(target);
        }

        yield return new WaitForSeconds(attackAnimeTime - attackAnimeTime * 0.4f);//后摇
        isAttack = false;
        if (isDie == false && obstructTarget != null)
            skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
        attackTimer = attackCapTime;//冷却
        attackTargetList.Clear();
    }
}
