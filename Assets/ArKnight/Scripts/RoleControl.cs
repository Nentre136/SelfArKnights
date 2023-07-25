using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
public class RoleControl : MonoBehaviour
{

    [SerializeField] int maxHealth = 1000;
    protected int currentHealth;
    [SerializeField] int normalDamage;
    public int damage { private set; get; }
    public List<GameObject> obstructTargets;//��������
    public int obstructCount = 2;//�赲��
    public int currentObstructCount = 0;//��ǰ�赲��
    public bool isObstruct = true;//�Ƿ��赲
    public Transform attackRange { private set; get; }//������Χ
    protected Transform skillRange;//���ܷ�Χ
    protected float attackAnimeTime;//��������ʱ��
    protected bool isAttack = false;//�Ƿ����ڹ���
    public bool isSkill = false;//�Ƿ��ü���
    protected float skillTime = 30f;//30s ����ʱ��
    public bool isPut = false;//�Ƿ����
    protected bool isDie = false;
    protected string attackAnimeName;
    public GameObject attackTarget;
    public GameObject playerButton { get; protected set; }
    public GameObject roleButton { get; private set; }

    protected GameManege gameManege;
    protected UIManege uiManege;
    public SkeletonAnimation roleAnimationSkele;//�������
    protected RectTransform healthImag;
    


    protected void Awake()
    {
        gameManege = GameObject.Find("GameManege").GetComponent<GameManege>();
        uiManege = GameObject.Find("Canvas").GetComponent<UIManege>();
        roleAnimationSkele = transform.Find("body").GetComponent<SkeletonAnimation>();

    }
    protected void Start()
    {
        attackRange = transform.Find("AttackRange");
        skillRange = transform.Find("SkillRange");
        attackAnimeName = "Attack";//Ĭ��
        currentHealth = maxHealth;
        damage = normalDamage;
        obstructTargets = new List<GameObject>();
        //��ȡAttack��������ʱ��
        attackAnimeTime = roleAnimationSkele.skeleton.Data.FindAnimation("Attack").Duration;
        healthImag = transform.Find("BaseUI").Find("healthBackground").Find("health").GetComponent<RectTransform>();
    }

    protected void FixedUpdate()
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
    public void BeAttack(GameObject attackObject)//�ܵ�����
    {
        int damage = attackObject.GetComponent<EnemyControl>().GetDamage();
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
            healthImag.localScale -= new Vector3((float)damage / (float)maxHealth, 0, 0);
        }
        else
        {
            //û��������
            if (isDie==false)
                StartCoroutine(OnDie(attackObject));
        }
    }
    public void BeCure(int CureValue)
    {
        if (currentHealth + CureValue < maxHealth)
        {
            currentHealth += CureValue;
            healthImag.localScale += new Vector3((float)CureValue / (float)maxHealth, 0, 0);
        }
        else
        {
            healthImag.localScale += new Vector3((float)(maxHealth -currentHealth)/ (float)maxHealth, 0, 0);
            currentHealth = maxHealth;
        }
    }
    public bool IsCure() { return currentHealth == maxHealth ? false : true; }
    public void OnAttack()
    {
        //���ڹ�������
        if (attackTarget != null)
        {
            //���˲��ڹ�����Χ��
            if (isInAttackRange(attackTarget) == false && currentObstructCount == 0 )
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
            if (isAttack==false)//���빥��״̬
                StartCoroutine(AttackTime());

        }

        //�����ڹ�������
        if (attackTarget == null)
        {
            if (obstructTargets.Count > 0)//�����赲����
            {
                attackTarget = obstructTargets[0];
                if (roleAnimationSkele.skeleton.Data.FindAnimation("Combat") != null)
                    attackAnimeName = "Combat";
                else
                    attackAnimeName = "Attack";
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

    protected IEnumerator AttackTime()
    {
        
        roleAnimationSkele.AnimationState.SetAnimation(0, attackAnimeName, false);//��� - Name - Loop
        isAttack = true;
        yield return new WaitForSeconds(attackAnimeTime*0.45f);//ǰҡ

        if (attackTarget == null)
        {
            isAttack = false;
            OnIdle();
            yield break;
        }

        attackTarget.GetComponent<EnemyControl>().BeAttack(gameObject);
        yield return new WaitForSeconds(attackAnimeTime - attackAnimeTime * 0.45f);//��ҡ
        isAttack = false;
    }
    protected IEnumerator OnDie(GameObject attackObject)
    {
        //������Ҫ�ӽ�ɫ�б���ɾ��
        gameManege.playerList.Remove(gameObject);
        gameManege.audio.GetComponent<CS_Audio>().audioSource.PlayOneShot(gameManege.audio.GetComponent<CS_Audio>().die);
        healthImag.parent.gameObject.SetActive(false);

        playerButton.SetActive(true);
        uiManege.playerButton.Add(playerButton);
        playerButton.transform.Find("CD").gameObject.SetActive(true);
        playerButton.GetComponent<RoleButton>().isCD = true;
        uiManege.ResetButtonPosition();

        isDie = true;
        roleAnimationSkele.AnimationState.SetAnimation(0, "Die", false);//��� - Name - Loop
        yield return new WaitForSeconds(roleAnimationSkele.skeleton.Data.FindAnimation("Die").Duration * 0.8f);

        attackObject.GetComponent<EnemyControl>().attackTarget = null;
        attackObject.GetComponent<EnemyControl>().obstructTarget = null;
        attackObject.GetComponent<EnemyControl>().isObstruct = false;
        attackObject.GetComponent<EnemyControl>().ResetSpeed();

        Destroy(gameObject);
    }
    protected void OnIdle()
    {
        string currentAnimeName = roleAnimationSkele.AnimationState.GetCurrent(0).Animation.Name;
        float AnimeTime = roleAnimationSkele.skeleton.Data.FindAnimation(currentAnimeName).Duration;
        float currentAnimeTime = roleAnimationSkele.AnimationState.GetCurrent(0).AnimationTime;
        if(AnimeTime - currentAnimeTime <= 0.05f)
            roleAnimationSkele.AnimationState.SetAnimation(0, "Idle", false);
    }
    public int GetNormalDamage() { return normalDamage; }
    public void SetDamage(int val) { damage = val; }
    protected bool isInAttackRange(GameObject enemy)
    {
        for (int index = 0; index < attackRange.childCount; index++)//�жϵ����Ƿ��ڷ�Χ��
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
    public void SetPlayerButton(GameObject btn)
    {
        playerButton = btn;
    }

    public void ChangeRange()//�����������Ҷ���ı乥����Χ�Ľӿ�
    {
        if (skillRange != null)
        {
            attackRange.gameObject.SetActive(false);
            Quaternion originalQua = attackRange.rotation;
            attackRange = skillRange;
            attackRange.rotation = originalQua;
        }
    }
}
