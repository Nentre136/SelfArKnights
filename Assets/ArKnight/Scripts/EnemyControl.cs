using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine.Collections;
public class EnemyControl : MonoBehaviour
{
    protected GameManege gameManege;//敌人孵化器
    protected SkeletonAnimation skeletonAnimation;//动画组件
    protected GameObject UISystem;//UI界面控制
    protected Rigidbody enemyBody;
    public GameObject path;
    protected Transform[] paths;//路径
    public GameObject endNode { private set; get; }//终点
    protected RectTransform healthImag;//血条
    


    [SerializeField] float maxHealth=500f;
    protected float currentHealth;
    [SerializeField] int damage =100;
    [SerializeField] float staticSpeed = 2f;//不变速度
    protected float currentspeed;
    protected float movingWaitTime;
    public float attackCapTime = 1f;
    protected float attackTimer;
    protected int index=1;
    protected bool isAttack = false;//是否正在攻击
    public bool isObstruct = false;//是否正在阻挡
    protected bool isDie = false;
    public GameObject attackTarget;//攻击对象
    public GameObject obstructTarget;//阻挡对象
    private Transform attackRange;//攻击范围
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
        paths = path.GetComponentsInChildren<Transform>();//获取的是所有的子物体，包括父物体本身
        healthImag = transform.Find("BaseUI Enemy").Find("healthBackground").Find("health").GetComponent<RectTransform>();
        endNode = paths[paths.Length - 1].gameObject;//终点
        attackRange = transform.Find("AttackRange");//获取子物体 攻击范围
        attackAnimeTime = skeletonAnimation.skeleton.Data.FindAnimation(attackAnimeName).Duration;
        currentHealth = maxHealth;
        attackTimer = attackCapTime;
        currentspeed = staticSpeed;
        moveAnimeName = "Move_Loop";
        diedAnimeName = "Die";
    }

    protected void FixedUpdate()
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
    protected void BodyMove()
    {
        if (isAttack == true)
            return;
        if (currentspeed != 0)
            OnMoveAnime();
        //移动方向
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
        //移动    
        enemyBody.MovePosition(enemyBody.position + movePosition);
        //计算两个向量的距离，小于一定程度就算到达，则前往下一位置
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
        
        //抵达终点
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
        //不存在或不可阻挡 继续行动
        if (obstructTarget == null)
        {
            currentspeed = staticSpeed;
            return;
        }
        currentspeed = 0f;
    }
    protected void OnAttack()
    {
        //攻击间隔
        if (isAttack==false && attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
            return;
        }

        //存在攻击对象
        if (attackTarget != null)
        {
            
            //敌人不在攻击范围内
            if (isObstruct==false && isInAttackRange(attackTarget) == false && isAttack==false)
            {
                attackTarget = null;
                return;
            }

            //进入攻击状态
            if (isAttack==false && attackTimer <= 0)
            {
                if(attackTarget!=null)
                    StartCoroutine(AttackTime());
                attackTarget = null;
            }
                
        }

        //不存在攻击对象
        if (attackTarget == null)
        {
            //从后面提取角色，后放置的先被打
            for(int index= gameManege.playerList.Count-1;index>=0 ;index--)
            {
                GameObject player = gameManege.playerList[index];
                if (isInAttackRange(player))
                {
                    //在范围内,则刷新攻击目标
                    attackTarget = player.gameObject;
                    return;
                }
            }
        }
    }
    protected void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && isObstruct==false)//干员 enemy未被阻挡
        {
            GameObject _otherGameobject = other.gameObject;
            RoleControl role = _otherGameobject.GetComponent<RoleControl>();
            if (role.isObstruct && role.currentObstructCount + 1 <= role.obstructCount)//role可阻挡 不能超过阻挡数
            {
                isObstruct = true;
                attackTarget = _otherGameobject;
                obstructTarget = _otherGameobject;
                role.currentObstructCount++;
                role.obstructTargets.Add(gameObject);//添加阻挡
            }
        }

        //攻击阻挡对象
        if (isAttack==false && attackTimer <= 0)
        {
            if(attackTarget!=null)
                StartCoroutine(AttackTime());
        }
            
    }
    protected void OnTriggerExit(Collider other)
    {
        if (obstructTarget == other.gameObject && isObstruct)//被other阻挡过 才存在返回阻挡数的情况
        {

            attackTarget = null;//重置攻击对象
            obstructTarget = null;//重置阻挡对象
            isObstruct = false;
            currentspeed = staticSpeed;//归还速度
            other.GetComponent<RoleControl>().obstructTargets.Remove(gameObject);//删除阻挡
            other.gameObject.GetComponent<RoleControl>().currentObstructCount--;//返回阻挡数
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
            //更新游戏进度
            UISystem.GetComponent<CS_UIPlay>().UpdateProgress();
            //没死才能死   防止出现多次死亡动画
            if (isDie==false)
                StartCoroutine(OnDie());
        }
            
    }

    //判断敌人是否在攻击范围内
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

    //攻击动画
    protected IEnumerator AttackTime()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimeName, false);//轨道 - Name - Loop
        isAttack = true;
        yield return new WaitForSeconds(attackAnimeTime * 0.4f);//前摇
        if (attackTarget == null)
        {
            isAttack = false;
            yield break;
        }

        attackTarget.GetComponent<RoleControl>().BeAttack(gameObject);

        yield return new WaitForSeconds(attackAnimeTime - attackAnimeTime * 0.4f);//后摇
        isAttack = false;
        if(isDie==false && obstructTarget!=null)
            skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
        attackTimer = attackCapTime;//冷却
    }
    protected IEnumerator OnDie()
    {
        //如果阻挡的enemy死亡，返回阻挡数
        if (isObstruct && obstructTarget!=null)
        {
            obstructTarget.GetComponent<RoleControl>().currentObstructCount--;
            obstructTarget.GetComponent<RoleControl>().obstructTargets.Remove(gameObject);
        }
        //死亡需要从敌人列表中删除
        gameManege.enemyList.Remove(gameObject);
        gameManege.AddBeatEnemyCount();
        healthImag.parent.gameObject.SetActive(false);
        isDie = true;
        skeletonAnimation.AnimationState.SetAnimation(0, diedAnimeName, false);//轨道 - Name - Loop
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
            currentspeed = staticSpeed;//恢复移速
        }
        else if (currentAnimeName == attackAnimeName)//播放动画为攻击动化，需要等攻击完才能移动
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
