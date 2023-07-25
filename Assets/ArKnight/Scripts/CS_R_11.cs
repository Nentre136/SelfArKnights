using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_R_11 : EnemyControl
{
    public int attackCount = 4;//��������
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

    new void OnAttack()
    {
        if (isAttack)
            return;
        //�������
        if (isAttack == false && attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
            return;
        }

        //���ڹ�������
        if (attackTargetList.Count>0)
        {

            //���˲��ڹ�����Χ��
            //if (isObstruct == false && isInAttackRange(attackTarget) == false && isAttack == false)
            //{
            //    attackTarget = null;
            //    return;
            //}

            //���빥��״̬
            if (isAttack == false && attackTimer <= 0)
            {
                if (attackTargetList.Count > 0)
                    StartCoroutine(AttackTime());
            }

        }


        //����������
        if (attackTargetList.Count<attackCount)
        {
            int count = attackTargetList.Count;
            //�Ӻ�����ȡ��ɫ  ��������ȱ�����
            for (int index = gameManege.playerList.Count - 1; index >= 0; index--)
            {
                GameObject target = gameManege.playerList[index];
                if (count >= attackCount)
                    return;
                if (!attackTargetList.Contains(target) && isInAttackRange(target))
                {
                    //�ڷ�Χ��,��ˢ�¹���Ŀ��
                    attackTargetList.Add(target);
                    count++;
                }
            }
        }
    }
    new IEnumerator AttackTime()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimeName, false);//��� - Name - Loop
        isAttack = true;
        yield return new WaitForSeconds(attackAnimeTime * 0.4f);//ǰҡ

        foreach (GameObject target in attackTargetList)
        {
            if (target != null)
                target.GetComponent<RoleControl>().BeAttack(gameObject);
            else
                attackTargetList.Remove(target);
        }

        yield return new WaitForSeconds(attackAnimeTime - attackAnimeTime * 0.4f);//��ҡ
        isAttack = false;
        if (isDie == false && obstructTarget != null)
            skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
        attackTimer = attackCapTime;//��ȴ
        attackTargetList.Clear();
    }
}
