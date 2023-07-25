using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIManege: MonoBehaviour
{
    public GameObject[] roleButton;//��ɫ��ť
    [SerializeField] Material tallTileMaterial;
    [SerializeField] Material downTileMaterial;
    public List<GameObject> playerButton;//ʵ������ť
    private Transform canvas;//ui���� 
    private Transform cardGroup;//����
    public CS_UIPlay uiPlay { get; private set; }
    public GameManege gameManage { get; private set; }
    public Camera mainCamare;

    public Button viewButton;
    private Vector3 upPosition;
    public Vector3 offsetPosition { get; private set; }
    private GameObject chosenPlayer;
    private bool isLook = false;
    private Color tallOriginalColor;
    private Color downOriginalColor;
    void Awake()
    {
        canvas = GetComponent<Transform>();
        cardGroup = canvas.Find("cardGroup");
        mainCamare = GameObject.Find("Main Camera").GetComponent<Camera>();
        uiPlay = canvas.Find("gameUISystem").GetComponent<CS_UIPlay>();
        gameManage = GameObject.Find("GameManege").GetComponent<GameManege>();
        playerButton = new List<GameObject>();
        for (int i = 0; i < roleButton.Length; i++)
        {
            GameObject btn = Transform.Instantiate(roleButton[i], cardGroup);//ע��Ҫ��ʵ����
            playerButton.Add(btn);
            btn.GetComponent<RectTransform>().position += new Vector3((-110 * i), 0, 0);//ʵ������̬�ı�λ��
        }
    }
    private void Start()
    {
        upPosition = new Vector3(0, 40, 0);
        offsetPosition = mainCamare.transform.position - GameObject.Find("GameManege").transform.position;
        tallOriginalColor = tallTileMaterial.color;
        downOriginalColor = downTileMaterial.color;
    }

    void Update()
    {
        //�����Ļ��ԭ��ť û�е����ui��
        if (Input.GetMouseButton(0) && viewButton!=null)
        {
            if (EventSystem.current.IsPointerOverGameObject()==false)
            {
                ResetButton();
            }
        }

        //�������ɫ �۽���ɫ
        if (isLook == false && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);
            if (hit.collider.gameObject.tag == "Player")
            {
                chosenPlayer = hit.collider.gameObject;
                LookPlayer();
            }
        }
        else if(isLook == true && Input.GetMouseButtonDown(0))
        {
            if(EventSystem.current.IsPointerOverGameObject() == false)
                ResetChosen();
        }

    }

    private void ResetButton()
    {
        viewButton.GetComponent<RectTransform>().position -= upPosition;
        viewButton = null;
        Time.timeScale = uiPlay.timeSpeed;
    }

    //��������ťλ��
    public void ResetButtonPosition(GameObject deleteButton)
    {
        playerButton.Remove(deleteButton);
        deleteButton.SetActive(false);
        for (int index=0;index<playerButton.Count ;index++)
        {
            playerButton[index].GetComponent<RectTransform>().anchoredPosition = roleButton[0].GetComponent<RectTransform>().anchoredPosition;
            playerButton[index].GetComponent<RectTransform>().anchoredPosition += new Vector2((-110 * index), 0);
        }
    }
    //����  ûdeleteButton
    public void ResetButtonPosition()
    {
        for (int index = 0; index < playerButton.Count; index++)
        {
            playerButton[index].GetComponent<RectTransform>().anchoredPosition = roleButton[0].GetComponent<RectTransform>().anchoredPosition;
            playerButton[index].GetComponent<RectTransform>().anchoredPosition += new Vector2((-110 * index), 0);
        }

    }

    //�����ɫ
    private void LookPlayer()
    {
        isLook = true;
        if (Time.timeScale != 0)
            Time.timeScale = 0.1f;
        Vector3 movePosition = chosenPlayer.transform.position + offsetPosition;
        mainCamare.transform.position = movePosition;
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(chosenPlayer.transform.position);
        chosenPlayer.transform.Find("BaseUI").Find("PutUI").position = playerScreenPosition;
        chosenPlayer.GetComponent<RoleControl>().attackRange.gameObject.SetActive(true);
        chosenPlayer.transform.Find("BaseUI").Find("PutUI").Find("TowardsLayout").gameObject.SetActive(true);
        chosenPlayer.transform.Find("BaseUI").Find("PutUI").Find("EscapeButton").gameObject.SetActive(true);
        chosenPlayer.transform.Find("BaseUI").Find("PutUI").Find("SkillButton").gameObject.SetActive(true);
        
    }
    //���ò鿴
    private void ResetChosen()
    {
        isLook = false;
        if(Time.timeScale!=0)
            Time.timeScale = uiPlay.timeSpeed;
        mainCamare.transform.position = GameObject.Find("GameManege").transform.position + offsetPosition;
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(chosenPlayer.transform.position);
        chosenPlayer.transform.Find("BaseUI").Find("PutUI").position = playerScreenPosition;
        chosenPlayer.GetComponent<RoleControl>().attackRange.gameObject.SetActive(false);
        chosenPlayer.transform.Find("BaseUI").Find("PutUI").Find("TowardsLayout").gameObject.SetActive(false);
        chosenPlayer.transform.Find("BaseUI").Find("PutUI").Find("EscapeButton").gameObject.SetActive(false);
        chosenPlayer.transform.Find("BaseUI").Find("PutUI").Find("SkillButton").gameObject.SetActive(false);
    }
    //���³��˰�ť
    public void OnClickEscapeButton()
    {
        isLook = false;
        mainCamare.transform.position = GameObject.Find("GameManege").transform.position + offsetPosition;
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(chosenPlayer.transform.position);
        chosenPlayer.transform.Find("BaseUI").Find("PutUI").position = playerScreenPosition;
        ResetChosen();

        //�Żذ�ť��ʼCD
        GameObject play_btn = chosenPlayer.GetComponent<RoleControl>().playerButton;
        play_btn.SetActive(true);
        playerButton.Add(play_btn);
        play_btn.transform.Find("CD").gameObject.SetActive(true);
        play_btn.GetComponent<RoleButton>().isCD = true;
        
        ResetButtonPosition();
        gameManage.playerList.Remove(chosenPlayer);
        Destroy(chosenPlayer);
    }
    public void OnClickSkillButton()
    {
        chosenPlayer.GetComponent<RoleControl>().isSkill = true;
        //��Ч
        chosenPlayer.GetComponent<RoleControl>().ChangeRange();
        AudioSource audioSource = GameObject.Find("Audio").GetComponent<CS_Audio>().audioSource;
        audioSource.PlayOneShot(GameObject.Find("Audio").GetComponent<CS_Audio>().skill);
        chosenPlayer.transform.Find("BaseUI").Find("PutUI").Find("SkillButton").GetComponent<Button>().interactable = false;
        ResetChosen();
    }

    //�ɲ���ؿ���ɫ����ɫ
    public void CheckIsPutTile(GameObject checkRole)
    {
        Color grennColor;
        ColorUtility.TryParseHtmlString("#52C349", out grennColor);
        if (LayerMask.LayerToName(checkRole.layer) == "DownTag")
        {
            tallTileMaterial.color = tallOriginalColor;
            downTileMaterial.color = grennColor;
        }
        else if(LayerMask.LayerToName(checkRole.layer) == "UpTag")
        {
            downTileMaterial.color =downOriginalColor;
            tallTileMaterial.color = grennColor;
        }
    }
    //����ש����ɫ
    public void RestTileMaterail()
    {
        downTileMaterial.color = downOriginalColor;
        tallTileMaterial.color = tallOriginalColor;
    }
}
