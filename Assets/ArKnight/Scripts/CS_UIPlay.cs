using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CS_UIPlay : MonoBehaviour
{
    [SerializeField] GameObject pausePlan;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject speed1xButton;
    [SerializeField] GameObject speed2xButton;
    [SerializeField] GameObject infoBar;
    [SerializeField] GameObject missionFailed;
    [SerializeField] GameObject quitPlan;
    [SerializeField] Text inBarEenemyNumber;
    [SerializeField] Slider progress;
    [SerializeField] Text progressText;
    [SerializeField] Image winImage;
    [SerializeField] Image winBackground;
    public float timeSpeed { private set; get; }
    private float decreaseEnemy = 0f;
    private float baseHp = 10f;
    private float enemyNumber_bar = 0f;
    private bool isQuit = false;
    private bool isPuase = false;

    //���¿�ʼ��ť
    public void OnClickPlayButton()
    {
        pausePlan.SetActive(false);
        playButton.SetActive(false);
        pauseButton.SetActive(true);
        isPuase = false;
        Time.timeScale = timeSpeed;
    }

    //������ͣ��ť
    public void OnClickPauseButton()
    {
        pausePlan.SetActive(true);
        playButton.SetActive(true);
        pauseButton.SetActive(false);
        isPuase = true;
        Time.timeScale = 0;
    }
    public void OnSpeed_1x()
    {
        timeSpeed = 2f;
        if(Time.timeScale!=0)
            Time.timeScale = timeSpeed;
        speed1xButton.SetActive(false);
        speed2xButton.SetActive(true);
    }
    public void OnSpeed_2x()
    {
        timeSpeed = 1f;
        if (Time.timeScale != 0)
            Time.timeScale = timeSpeed;
        speed1xButton.SetActive(true);
        speed2xButton.SetActive(false);
    }
    public void OnEnemyEnterBase()
    {
        Debug.Log("���˽���ķ�����!");
        Text baseNumber = infoBar.transform.Find("BaseNumber").Find("Number").GetComponent<Text>();
        GameObject hpDecrease = infoBar.transform.Find("HpDecrease").gameObject;
        Text decreaseCount = hpDecrease.transform.Find("DecreaseCount").GetComponent<Text>();
        hpDecrease.SetActive(true);
        decreaseCount.text = (decreaseEnemy -= 1).ToString();
        baseNumber.text = (baseHp -= 1).ToString();
    }
    //���������� ������Ϸ���� bar
    public void UpdateProgress()
    {
        float allNumber = 43f;
        enemyNumber_bar += 1;
        float enemyNumber = enemyNumber_bar;
        inBarEenemyNumber.text = (enemyNumber).ToString() + "/" + allNumber.ToString();
        progress.value = (float)enemyNumber / (float)allNumber;
        progressText.text = ((int)(progress.value * 100)).ToString() + "%";
    }
    
    //����ϵͳ��ť
    public void OnClickSystemButton()
    {
        quitPlan.SetActive(true);
        Time.timeScale = 0;
    }
    //���·��ذ�ť
    public void OnClickCancelButton()
    {
        quitPlan.SetActive(false);
        if(isPuase==false)
            Time.timeScale = timeSpeed;
    }
    //�����뿪��ť
    public void OnClickQuitButton()
    {
        quitPlan.SetActive(false);
        missionFailed.SetActive(true);
        isQuit = true;
    }

    //��Ϸʤ��  ʤ��UI
    public void OnMissionAccomplished()
    {
        winBackground.gameObject.SetActive(true);
        //��ê���������ƶ�����
        Vector2 currentPosition = new Vector2(winImage.rectTransform.anchoredPosition.x, winImage.rectTransform.anchoredPosition.y);
        if (currentPosition.x < -0.4f)
        {
            //���� * ���ٶ� * ʱ�䣩 ->  ���� * ·�� = �ƶ�λ������
            //���ݾ������ĵľ��������ƶ��ٶ�
            Vector2 movePosition = new Vector2(1, 0) * Mathf.Abs(currentPosition.x - 0) * 1.5f * Time.deltaTime; 
            winImage.rectTransform.anchoredPosition += movePosition;
        }
        else
        {
            Vector2 movePosition = new Vector2(1, 0) * 40f * Time.deltaTime;
            winImage.rectTransform.anchoredPosition += movePosition;
            if (currentPosition.x > 30f)
                SceneManager.LoadScene("Login");
        }
    }
    void Update()
    {
        if (baseHp <= 0)
        {
            missionFailed.SetActive(true);
            Time.timeScale = 0;
            isQuit = true;
        }
        //�뿪ʱ������Ļ ������ҳ��     
        if (isQuit && Input.GetMouseButton(0))
            SceneManager.LoadScene("Login");

    }
    void Start()
    {
        timeSpeed = 1f;
    }
}
