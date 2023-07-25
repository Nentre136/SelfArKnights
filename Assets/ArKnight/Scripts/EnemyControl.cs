using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine.Collections;
public class EnemyControl : MonoBehaviour
{
    protected GameManege gameManege;//���˷�����
    protected SkeletonAnimation skeletonAnimation;//�������
    protected GameObject UISystem;//UI�������
    protected Rigidbody enemyBody;
    public GameObject path;
    protected Transform[] paths;//·��
    public GameObject endNode { private set; get; }//�յ�
    protected RectTransform healthImag;//Ѫ��
    


    [SerializeField] float maxHealth=500f;
    protected float currentHealth;
    [SerializeField] int damage =100;
    [SerializeField] float staticSpeed = 2f;//�����ٶ�
    protected float currentspeed;
    protected float movingWaitTime;
    public float attackCapTime = 1f;
    protected float attackTimer;
    protected int index=1;
    protected bool isAttack = false;//�Ƿ����ڹ���
    public bool isObstruct = false;//�Ƿ������赲
    protected bool isDie = false;
    public GameObject attackTarget;//��������
    public GameObject obstructTarget;//�赲����
    private Transform attackRange;//������Χ
    protected string attackAnimeName="Attack";
    protected string moveAnimeName;
    protected string diedAnimeName;
    protected float attackAnimeTime;


    protected void Awake()
    {
        enemyBody = GetComponent<Rigidbody>();
        gameManege = GameObject.Find("GameManege").GetComponent<GameManege>();
        UISystem = GameObject.Find("Canvas").transform.Find("gameUISystem").gameObject;
        skeletonAnimation = transform.Find("body").GetComponent<SkeletonAnimation>();
    }
    protected void Start()
    {
        paths = path.GetComponentsInChildren<Transform>();//��ȡ�������е������壬���������屾��
        healthImag = transform.Find("BaseUI Enemy").Find("healthBackground").Find("health").GetComponent<RectTransform>();
        endNode = paths[paths.Length - 1].gameObject;//�յ�
        attackRange = transform.Find("AttackRange");//��ȡ������ ������Χ
        attackAnimeTime = skeletonAnimation.skeleton.Data.FindAnimation(attackAnimeName).Duration;
        currentHealth = maxHealth;
        attackTimer = attackCapTime;
        currentspeed = staticSpeed;
        moveAnimeName = "Move_Loop";
        diedAnimeName = "Die";
    }

    protected void FixedUpdate()
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
    protected void BodyMove()
    {
        if (isAttack == true)
            return;
        if (currentspeed != 0)
            OnMoveAnime();
        //�ƶ�����
        Vector3 moveDirection = (paths[index].position - enemyBody.position).normalized;
        Vector3 movePosition = moveDirection * currentspeed * Time.deltaTime;

        if (moveDirection.x > 0)
        {
            transform.Find("body").rotation = Quaternion.Euler(new Vector3(60, 0, 0));
        }
        else if(moveDirection.x < 0)
        {
            transform.Find("body").rotation = Quaternion.Euler(new Vector3(-60, 180, 0));
        }
        //�ƶ�    
        enemyBody.MovePosition(enemyBody.position + movePosition);
        //�������������ľ��룬С��һ���̶Ⱦ��㵽���ǰ����һλ��
        if (Vector3.Distance(paths[index].position, transform.position) < 0.1f)
        {
            if (paths[index].name == "Wait")
            {
                if(movingWaitTime==0)
                    skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
                if (movingWaitTime< paths[index].GetComponent<Wait>().waitTime)
                {
                    movingWaitTime += Time.fixedDeltaTime;
                    currentspeed = 0f;
                    return;
                }
                movingWaitTime = 0;
                currentspeed = staticSpeed;
            }
            index++;
        }
        
        //�ִ��յ�
        if (Vector3.Distance(paths[paths.Length - 1].position, transform.position) < 0.5f)
        {
            GameObject.Destroy(gameObject);
            gameManege.audio.GetComponent<CS_Audio>().audioSource.PlayOneShot(gameManege.audio.GetComponent<CS_Audio>().hpDecrease);
            gameManege.enemyList.Remove(gameObject);
            if(paths[paths.Length - 1].tag=="Base")
                UISystem.GetComponent<CS_UIPlay>().OnEnemyEnterBase();
            UISystem.GetComponent<CS_UIPlay>().UpdateProgress();
            gameManege.AddBeatEnemyCount();
        }
    }
    protected void OnObstruct(GameObject obstructTarget)
    {
        //�����ڻ򲻿��赲 �����ж�
        if (obstructTarget == null)
        {
            currentspeed = staticSpeed;
            return;
        }
        currentspeed = 0f;
    }
    protected void OnAttack()
    {
        //�������
        if (isAttack==false && attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
            return;
        }

        //���ڹ�������
        if (attackTarget != null)
        {
            
            //���˲��ڹ�����Χ��
            if (isObstruct==false && isInAttackRange(attackTarget) == false && isAttack==false)
            {
                attackTarget = null;
                return;
            }

            //���빥��״̬
            if (isAttack==false && attackTimer <= 0)
            {
                if(attackTarget!=null)
                    StartCoroutine(AttackTime());
                attackTarget = null;
            }
                
        }

        //�����ڹ�������
        if (attackTarget == null)
        {
            //�Ӻ�����ȡ��ɫ������õ��ȱ���
            for(int index= gameManege.playerList.Count-1;index>=0 ;index--)
            {
                GameObject player = gameManege.playerList[index];
                if (isInAttackRange(player))
                {
                    //�ڷ�Χ��,��ˢ�¹���Ŀ��
                    attackTarget = player.gameObject;
                    return;
                }
            }
        }
    }
    protected void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && isObstruct==false)//��Ա enemyδ���赲
        {
            GameObject _otherGameobject = other.gameObject;
            RoleControl role = _otherGameobject.GetComponent<RoleControl>();
            if (role.isObstruct && role.currentObstructCount + 1 <= role.obstructCount)//role���赲 ���ܳ����赲��
            {
                isObstruct = true;
                attackTarget = _otherGameobject;
                obstructTarget = _otherGameobject;
                role.currentObstructCount++;
                role.obstructTargets.Add(gameObject);//����赲
            }
        }

        //�����赲����
        if (isAttack==false && attackTimer <= 0)
        {
            if(attackTarget!=null)
                StartCoroutine(AttackTime());
        }
            
    }
    protected void OnTriggerExit(Collider other)
    {
        if (obstructTarget == other.gameObject && isObstruct)//��other�赲�� �Ŵ��ڷ����赲�������
        {

            attackTarget = null;//���ù�������
            obstructTarget = null;//�����赲����
            isObstruct = false;
            currentspeed = staticSpeed;//�黹�ٶ�
            other.GetComponent<RoleControl>().obstructTargets.Remove(gameObject);//ɾ���赲
            other.gameObject.GetComponent<RoleControl>().currentObstructCount--;//�����赲��
        }
    }
    

    public void BeAttack(GameObject attackObject)
    {
        
        healthImag.parent.gameObject.SetActive(true);
        float damage = attackObject.GetComponent<RoleControl>().damage;
        StartCoroutine(ChangeColor());
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
            healthImag.localScale -= new Vector3((damage / maxHealth), 0, 0);
        }
        else
        {
            attackObject.GetComponent<RoleControl>().attackTarget = null;
            //������Ϸ����
            UISystem.GetComponent<CS_UIPlay>().UpdateProgress();
            //û��������   ��ֹ���ֶ����������
            if (isDie==false)
                StartCoroutine(OnDie());
        }
            
    }

    //�жϵ����Ƿ��ڹ�����Χ��
    protected bool isInAttackRange(GameObject enemy)
    {
        for (int index = 0; index < attackRange.childCount; index++)
        {
            if (enemy == null)
                return false;
            Vector3 rangePosition = attackRange.GetChild(index).position;
            Vector3 enemyPosition = enemy.transform.position;
            if ((enemyPosition.x > rangePosition.x - 2 && enemyPosition.x < rangePosition.x + 2) &&
                (enemyPosition.z > rangePosition.z - 2 && enemyPosition.z < rangePosition.z + 2))
            {

                return true;
            }
        }
        return false;
    }

    //��������
    protected IEnumerator AttackTime()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimeName, false);//��� - Name - Loop
        isAttack = true;
        yield return new WaitForSeconds(attackAnimeTime * 0.4f);//ǰҡ
        if (attackTarget == null)
        {
            isAttack = false;
            yield break;
        }

        attackTarget.GetComponent<RoleControl>().BeAttack(gameObject);

        yield return new WaitForSeconds(attackAnimeTime - attackAnimeTime * 0.4f);//��ҡ
        isAttack = false;
        if(isDie==false && obstructTarget!=null)
            skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
        attackTimer = attackCapTime;//��ȴ
    }
    protected IEnumerator OnDie()
    {
        //����赲��enemy�����������赲��
        if (isObstruct && obstructTarget!=null)
        {
            obstructTarget.GetComponent<RoleControl>().currentObstructCount--;
            obstructTarget.GetComponent<RoleControl>().obstructTargets.Remove(gameObject);
        }
        //������Ҫ�ӵ����б���ɾ��
        gameManege.enemyList.Remove(gameObject);
        gameManege.AddBeatEnemyCount();
        healthImag.parent.gameObject.SetActive(false);
        isDie = true;
        skeletonAnimation.AnimationState.SetAnimation(0, diedAnimeName, false);//��� - Name - Loop
        currentspeed = 0f;
        yield return new WaitForSeconds(skeletonAnimation.skeleton.Data.FindAnimation(diedAnimeName).Duration);
        Destroy(gameObject);
    }
    protected void OnMoveAnime()
    {
        string currentAnimeName = skeletonAnimation.AnimationState.GetCurrent(0).Animation.Name;
        float AnimeTime = skeletonAnimation.skeleton.Data.FindAnimation(currentAnimeName).Duration;
        float currentAnimeTime = skeletonAnimation.AnimationState.GetCurrent(0).AnimationTime;
        if (currentAnimeTime / AnimeTime >= 0.96f)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, moveAnimeName, false);
            currentspeed = staticSpeed;//�ָ�����
        }
        else if (currentAnimeName == attackAnimeName)//���Ŷ���Ϊ������������Ҫ�ȹ���������ƶ�
            currentspeed = 0f;
    }
    public int GetDamage() { return damage; }
    public void ResetSpeed() { currentspeed = staticSpeed;}
    protected IEnumerator ChangeColor()
    {
        Material[] body = transform.Find("body").GetComponent<MeshRenderer>().materials;
        foreach (Material material in body)
        {
            material.SetFloat("_FillPhase", 0.5f);
        }
        yield return new WaitForSeconds(0.08f);

        foreach (Material material in body)
        {
            material.SetFloat("_FillPhase", 0);
        }
    }
}
