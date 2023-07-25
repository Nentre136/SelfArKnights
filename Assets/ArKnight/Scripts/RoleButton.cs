using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class RoleButton : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] GameObject role;

    private GameObject gameManege;
    private UIManege uiManege;
    private Text cdPercentText;
    //实例化的角色
    public GameObject player { protected set; get; }
    
    private Vector3 upPosition;
    private bool isFirstDrag = false;//是否首次拖动
    private bool isDragEnd = false;//是否停止拖动
    private bool isPut = false;//是否放置
    private float cdPerecent = 20f;
    public bool isCD = false;//是否在CD
    private GameObject hitGameObject;//击中地块
    private string hitLayerName;
    void Awake()
    {
        uiManege = transform.parent.parent.GetComponent<UIManege>();
        GetComponent<Button>().onClick.AddListener(() => OnButtonClick(GetComponent<Button>()));//有参传入监听
        gameManege = GameObject.Find("GameManege");
        cdPercentText = transform.Find("CD").Find("CDpercent").GetComponent<Text>();
    }
    void Start()
    {
        upPosition = new Vector3(0, 40, 0);
    }
    void FixedUpdate()
    {
        if (isCD == true)
            UpdateCD();
    }
    private void UpdateCD()
    {
        cdPercentText.text = cdPerecent.ToString("F1");
        cdPerecent -= Time.fixedDeltaTime;
        if (cdPerecent <= 0)
        {
            cdPerecent = 20f;
            transform.Find("CD").gameObject.SetActive(false);
            isCD = false;
        }
    }
    private void OnButtonClick(Button selectButton)
    {
        if (uiManege.viewButton == null)
        {
            selectButton.GetComponent<RectTransform>().position += upPosition;
            uiManege.viewButton = selectButton;
            if(Time.timeScale!=0)
                Time.timeScale = 0.1f;
            uiManege.CheckIsPutTile(role);
        }
        else if (uiManege.viewButton == selectButton)
        {
            selectButton.GetComponent<RectTransform>().position -= upPosition;
            uiManege.viewButton = null;
            if (Time.timeScale != 0)
                ResetTimeSpeed();
            uiManege.RestTileMaterail();
        }
        else if(uiManege.viewButton != selectButton)
        {
            selectButton.GetComponent<RectTransform>().position += upPosition;
            uiManege.viewButton.GetComponent<RectTransform>().position -= upPosition;
            uiManege.viewButton = selectButton;
            uiManege.CheckIsPutTile(role);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isCD == true)
            return;
        Vector3 mousePosition = Input.mousePosition;
        ////获取屏幕坐标
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(gameManege.transform.position);
        mousePosition.z = screenPosition.z;//赋予鼠标z坐标 才能显示
        //获取世界坐标
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (Time.timeScale != 0)
            Time.timeScale = 0.1f;
        uiManege.CheckIsPutTile(role);
        if (isFirstDrag == false)
        {
            player = GameObject.Instantiate(role, worldPosition, Quaternion.identity);
            player.GetComponent<RoleControl>().SetPlayerButton(gameObject);
            player.GetComponent<BoxCollider>().enabled = false;
            GetComponent<Button>().GetComponent<RectTransform>().position += upPosition;
            isFirstDrag = true;
        }
        else
        {
            //获取碰撞信息
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            Physics.Raycast(mouseRay, out hit);
            if (hit.collider == null)
                return;

            hitGameObject = hit.collider.gameObject;
            hitLayerName = LayerMask.LayerToName(hitGameObject.layer);
            //可部署位置吸附
            if (hitLayerName == LayerMask.LayerToName(player.layer) && hitGameObject.tag!="Player")
            {
                Vector3 offSet = new Vector3(0,2.25f,-0.5f);
                player.transform.position = hitGameObject.transform.position + offSet;
                player.transform.Find("AttackRange").gameObject.SetActive(true);//显示攻击范围
                isDragEnd = true;
            }
            else
            {
                player.transform.position = worldPosition;
                player.transform.Find("AttackRange").gameObject.SetActive(false);
                isDragEnd = false;
            }
            
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isCD == true)
            return;
        GetComponent<Button>().GetComponent<RectTransform>().position -= upPosition;
        uiManege.RestTileMaterail();
        if (isDragEnd)
        {
            player.transform.Find("AttackRange").gameObject.SetActive(false);
            //PutUI跟随角色
            Vector3 roleScreenPosition = Camera.main.WorldToScreenPoint(player.transform.position);
            player.transform.Find("BaseUI").Find("PutUI").position = roleScreenPosition;
            player.transform.Find("BaseUI").Find("PutUI").Find("TowardsLayout").gameObject.SetActive(true);
            player.transform.Find("BaseUI").Find("PutUI").Find("MoveLayout").gameObject.SetActive(true);
            Button cancelButton = player.transform.Find("BaseUI").Find("PutUI").Find("CancelLayout").GetComponent<Button>();
            cancelButton.gameObject.SetActive(true);
            cancelButton.onClick.AddListener(OnClickCancelButton);
            player.transform.Find("BaseUI").Find("PutUI").Find("EscapeButton").GetComponent<Button>().onClick.AddListener(uiManege.OnClickEscapeButton);
            player.transform.Find("BaseUI").Find("PutUI").Find("SkillButton").GetComponent<Button>().onClick.AddListener(uiManege.OnClickSkillButton);

        }
        else
        {
            //放置失败
            GameObject.Destroy(player);
            if (Time.timeScale != 0)
                ResetTimeSpeed();
        }
        isFirstDrag = false;
    }

    public void OnClickCancelButton()
    {
        GameObject.Destroy(player);
        ResetTimeSpeed();
    }

    public void ResetTimeSpeed()
    {
        Time.timeScale = gameManege.GetComponent<GameManege>().uiPlay.GetComponent<CS_UIPlay>().timeSpeed;
    }
}
