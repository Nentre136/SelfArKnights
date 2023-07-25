using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_YinHui : RoleControl
{
    public List<GameObject> skillTarget;//����ն ��5
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
            //�赲�ж�
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
                if (roleAnimationSkele.skeleton.Data.FindAnimation("Combat") != null)
                    attackAnimeName = "Combat";
                else
                    attackAnimeName = "Attack";
            }
            //���빥��״̬
            if (isAttack == false)
            {
                SetDamage(GetNormalDamage());
                StartCoroutine(AttackTime());
            }
        }

        //�����ڹ�������
        if (attackTarget == null)
        {
            if (obstructTargets.Count > 0)//�����赲����
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
                    attackTarget = enemy.gameObject;//�ڷ�Χ��,��ˢ�¹���Ŀ��
                    return;
                }
            }
        }
    }
    private void OnSkill()
    {
        //���Ŀ������5�� ����
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

        //����skillĿ��
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
        roleAnimationSkele.AnimationState.SetAnimation(0, "Skill", false);//��� - Name - Loop
        float currentAnimeTime = roleAnimationSkele.skeleton.Data.FindAnimation("Skill").Duration;
        yield return new WaitForSeconds(currentAnimeTime * 0.1f);//ǰҡ

        skillSprite.enabled = true;
        StartCoroutine(skillSprite.GetComponent<CS_YinHuiSkill>().SkillAnimation());

        yield return new WaitForSeconds(currentAnimeTime * 0.1f);//ǰҡ
        foreach (GameObject target in skillTarget)
        {
            if (target != null)
                target.GetComponent<EnemyControl>().BeAttack(gameObject);
        }

        yield return new WaitForSeconds(currentAnimeTime - currentAnimeTime * 0.2f);//��ҡ
        skillTarget.Clear();
        isAttack = false;
    }
}
