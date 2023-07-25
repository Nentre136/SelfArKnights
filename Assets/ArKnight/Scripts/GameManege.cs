using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//����ˢ��������Ϊ���˲��ο��ƺͷ���ʹ��
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
        StartCoroutine(BirthEnemy());//ֻ��Ҫ����һ��
        Time.timeScale = 1;
    }
    private void Awake()
    {
        enemyList = new List<GameObject>();//ʵ��������������
    }
    private int Compare(GameObject a,GameObject b)//�Ƚ��� �������Ž�������
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

            //�����²�ʱ��
            yield return new WaitForSeconds(wave.lastTime - 1f);

            //��һ������ǰ����ʾ�����ж�·��
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
        for (int i = 0; i < wave.count; i++)//ˢ������
        {
            //���ɵ��� 
            GameObject enemy = Transform.Instantiate(wave.enemy, wave.startNode.transform.position, Quaternion.identity);
            enemy.GetComponent<EnemyControl>().path = wave.path;
            enemyList.Add(enemy);//���˼����б�
            if (i < wave.count - 1)//���һ������ȴ
                yield return new WaitForSeconds(wave.spacingTime);
        }
        yield break;
    }
}
