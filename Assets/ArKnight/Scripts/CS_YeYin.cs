using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_YeYin : RoleControl
{
    protected List<GameObject> cureTargetList;
    protected new void Start()
    {
        base.Start();
        cureTargetList = new List<GameObject>();
    }
    public new void OnAttack()
    {
        //存在攻击对象
        if (cureTargetList.Count>0)
        {
            //进入攻击状态
            if (isAttack == false)
            {
                StartCoroutine(AttackTime());
                cureTargetList.Clear();
            }
        }
        //不存在攻击对象
        if (cureTargetList.Count<=0)
        {
            foreach (GameObject player in gameManege.playerList)
            {
                if (isInAttackRange(player))
                {
                    if(player.GetComponent<RoleControl>().IsCure())
                        cureTargetList.Add(player);
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
    protected new IEnumerator AttackTime()
    {

        roleAnimationSkele.AnimationState.SetAnimation(0, attackAnimeName, false);//轨道 - Name - Loop
        isAttack = true;
        yield return new WaitForSeconds(attackAnimeTime * 0.2f);//前摇

        foreach (GameObject target in cureTargetList)
        {
            if(target!=null)
                target.GetComponent<RoleControl>().BeCure(damage);
        }

        yield return new WaitForSeconds(attackAnimeTime - attackAnimeTime * 0.2f);//后摇
        isAttack = false;
    }
}
