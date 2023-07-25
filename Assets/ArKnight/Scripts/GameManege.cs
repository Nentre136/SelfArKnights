using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//敌人刷新器，做为敌人波次控制和孵化使用
public class GameManege : MonoBehaviour
{
    [SerializeField] GameObject enemyTips;
    public Waves[] waves;
    public List<GameObject> enemyList;
    public List<GameObject> playerList;
    public GameObject audio;
    public GameObject uiPlay;
    private int beatEnemyNumber=0;
    private int allEnemyNumber = 43;
    private bool isWin = false;
    void Start()
    {
        StartCoroutine(BirthEnemy());//只需要调用一遍
        Time.timeScale = 1;
    }
    private void Awake()
    {
        enemyList = new List<GameObject>();//实例化化敌人链表
    }
    private int Compare(GameObject a,GameObject b)//比较器 距离蓝门近的优先
    {
        EnemyControl e1 = a.GetComponent<EnemyControl>();
        EnemyControl e2 = b.GetComponent<EnemyControl>();
        float distance1 = Vector3.Distance(e1.transform.position, e1.endNode.transform.position);
        float distance2 = Vector3.Distance(e2.transform.position, e2.endNode.transform.position);
        if (distance1 == distance2)
            return 0;
        return distance1 < distance2 ? -1 : 1;
    }
    void Update()
    {
        if(enemyList.Count>=2)
            enemyList.Sort((a, b) => Compare(a, b));

        if (beatEnemyNumber == allEnemyNumber)
            StartCoroutine(OnWin());
    }
    public void AddBeatEnemyCount() { beatEnemyNumber++; }
    IEnumerator OnWin()
    {
        yield return new WaitForSeconds(1f);
        if (isWin == false)
        {
            isWin = true;
            audio.GetComponent<AudioSource>().volume = 1;
            audio.GetComponent<CS_Audio>().audioSource.PlayOneShot(audio.GetComponent<CS_Audio>().win);
        }
        else
            uiPlay.GetComponent<CS_UIPlay>().OnMissionAccomplished();   
    }

    IEnumerator BirthEnemy()
    {
        for (int index = 0; index < waves.Length; index++)
        {
            Waves wave = waves[index];
            StartCoroutine(UpdateEnemy(wave));

            //距离下波时间
            yield return new WaitForSeconds(wave.lastTime - 1f);

            //下一波来临前，提示怪物行动路径
            if (index < waves.Length - 1)
            {
                GameObject tips = Transform.Instantiate(enemyTips, waves[index + 1].startNode.transform.position, Quaternion.identity);
                tips.GetComponent<CS_EnemyTips>().path = waves[index + 1].path;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator UpdateEnemy(Waves wave)
    {
        for (int i = 0; i < wave.count; i++)//刷新数量
        {
            //生成敌人 
            GameObject enemy = Transform.Instantiate(wave.enemy, wave.startNode.transform.position, Quaternion.identity);
            enemy.GetComponent<EnemyControl>().path = wave.path;
            enemyList.Add(enemy);//敌人加入列表
            if (i < wave.count - 1)//最后一个不冷却
                yield return new WaitForSeconds(wave.spacingTime);
        }
        yield break;
    }
}
