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
    public List<GameObject> obstructTargets;//主档对象
    public int obstructCount = 2;//阻挡数
    public int currentObstructCount = 0;//当前阻挡数
    public bool isObstruct = true;//是否阻挡
    public Transform attackRange { private set; get; }//攻击范围
    protected Transform skillRange;//技能范围
    protected float attackAnimeTime;//攻击动画时长
    protected bool isAttack = false;//是否正在攻击
    public bool isSkill = false;//是否用技能
    protected float skillTime = 30f;//30s 技能时间
    public bool isPut = false;//是否放置
    protected bool isDie = false;
    protected string attackAnimeName;
    public GameObject attackTarget;
    public GameObject playerButton { get; protected set; }
    public GameObject roleButton { get; private set; }

    protected GameManege gameManege;
    protected UIManege uiManege;
    public SkeletonAnimation roleAnimationSkele;//动画组件
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
        attackAnimeName = "Attack";//默认
        currentHealth = maxHealth;
        damage = normalDamage;
        obstructTargets = new List<GameObject>();
        //获取Attack动画的总时间
        attackAnimeTime = roleAnimationSkele.skeleton.Data.FindAnimation("Attack").Duration;
        healthImag = transform.Find("BaseUI").Find("healthBackground").Find("health").GetComponent<RectTransform>();
    }

    protected void FixedUpdate()
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
    public void BeAttack(GameObject attackObject)//受到攻击
    {
        int damage = attackObject.GetComponent<EnemyControl>().GetDamage();
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
            healthImag.localScale -= new Vector3((float)damage / (float)maxHealth, 0, 0);
        }
        else
        {
            //没死才能死
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
        //存在攻击对象
        if (attackTarget != null)
        {
            //敌人不在攻击范围内
            if (isInAttackRange(attackTarget) == false && currentObstructCount == 0 )
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
            if (isAttack==false)//进入攻击状态
                StartCoroutine(AttackTime());

        }

        //不存在攻击对象
        if (attackTarget == null)
        {
            if (obstructTargets.Count > 0)//存在阻挡对象
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
                    attackTarget = enemy.gameObject;//在范围内,则刷新攻击目标
                    return;
                }
            }
        }
        
    }

    protected IEnumerator AttackTime()
    {
        
        roleAnimationSkele.AnimationState.SetAnimation(0, attackAnimeName, false);//轨道 - Name - Loop
        isAttack = true;
        yield return new WaitForSeconds(attackAnimeTime*0.45f);//前摇

        if (attackTarget == null)
        {
            isAttack = false;
            OnIdle();
            yield break;
        }

        attackTarget.GetComponent<EnemyControl>().BeAttack(gameObject);
        yield return new WaitForSeconds(attackAnimeTime - attackAnimeTime * 0.45f);//后摇
        isAttack = false;
    }
    protected IEnumerator OnDie(GameObject attackObject)
    {
        //死亡需要从角色列表中删除
        gameManege.playerList.Remove(gameObject);
        gameManege.audio.GetComponent<CS_Audio>().audioSource.PlayOneShot(gameManege.audio.GetComponent<CS_Audio>().die);
        healthImag.parent.gameObject.SetActive(false);

        playerButton.SetActive(true);
        uiManege.playerButton.Add(playerButton);
        playerButton.transform.Find("CD").gameObject.SetActive(true);
        playerButton.GetComponent<RoleButton>().isCD = true;
        uiManege.ResetButtonPosition();

        isDie = true;
        roleAnimationSkele.AnimationState.SetAnimation(0, "Die", false);//轨道 - Name - Loop
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
        for (int index = 0; index < attackRange.childCount; index++)//判断敌人是否在范围内
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

    public void ChangeRange()//仅仅用于银灰对外改变攻击范围的接口
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
