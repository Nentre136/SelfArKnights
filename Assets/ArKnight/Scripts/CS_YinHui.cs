using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_YinHui : RoleControl
{
    public List<GameObject> skillTarget;//真银斩 砍5
    protected SpriteRenderer skillSprite;
    protected int skillDamage = 1200;

    new void Start()
    {
        base.Start();
        skillSprite = transform.Find("skill").GetComponent<SpriteRenderer>();
    }
    new void FixedUpdate()
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

            if (isSkill == false)
                OnAttack();
            else
                OnSkill();
        }
    }
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
                if (roleAnimationSkele.skeleton.Data.FindAnimation("Combat") != null)
                    attackAnimeName = "Combat";
                else
                    attackAnimeName = "Attack";
            }
            //进入攻击状态
            if (isAttack == false)
            {
                SetDamage(GetNormalDamage());
                StartCoroutine(AttackTime());
            }
        }

        //不存在攻击对象
        if (attackTarget == null)
        {
            if (obstructTargets.Count > 0)//存在阻挡对象
            {
                attackTarget = obstructTargets[0];
                attackAnimeName = "Combat";
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
    private void OnSkill()
    {
        //如果目标少于5人 查找
        if (skillTarget.Count < 5 && isAttack==false)
        {
            foreach (GameObject enemy in gameManege.enemyList)
            {
                if (isInAttackRange(enemy))
                {
                    skillTarget.Add(enemy);
                }
                if (skillTarget.Count >= 5)
                    break;
            }
        }

        //存在skill目标
        if (skillTarget.Count > 0)
        {
            if (isAttack == false)
            {
                SetDamage(skillDamage);
                StartCoroutine(SkillTime());
            }
        }

    }
    IEnumerator SkillTime()
    {
        isAttack = true;
        roleAnimationSkele.AnimationState.SetAnimation(0, "Skill", false);//轨道 - Name - Loop
        float currentAnimeTime = roleAnimationSkele.skeleton.Data.FindAnimation("Skill").Duration;
        yield return new WaitForSeconds(currentAnimeTime * 0.1f);//前摇

        skillSprite.enabled = true;
        StartCoroutine(skillSprite.GetComponent<CS_YinHuiSkill>().SkillAnimation());

        yield return new WaitForSeconds(currentAnimeTime * 0.1f);//前摇
        foreach (GameObject target in skillTarget)
        {
            if (target != null)
                target.GetComponent<EnemyControl>().BeAttack(gameObject);
        }

        yield return new WaitForSeconds(currentAnimeTime - currentAnimeTime * 0.2f);//后摇
        skillTarget.Clear();
        isAttack = false;
    }
}
