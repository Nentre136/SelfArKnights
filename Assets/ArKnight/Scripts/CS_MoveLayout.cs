using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CS_MoveLayout : MonoBehaviour,IDragHandler, IEndDragHandler
{
    private Vector2 originalPosition;//原始位置
    private Button moveLayout;//朝向Layout
    private Transform player;//角色实例化
    private bool isEndDrag = false;//布置完成
    private RoleControl roleControl;
    private GameManege gameManege;
    void Awake()
    {
        moveLayout = GetComponent<Button>();
        player = transform.parent.parent.parent;
        roleControl = player.GetComponent<RoleControl>();
        gameManege = GameObject.Find("GameManege").GetComponent<GameManege>();
    }
    void Start()
    {
        originalPosition = moveLayout.GetComponent<RectTransform>().position;
    }

    void Update()
    {

    }
    public void OnDrag(PointerEventData eventData)
    {
        //获取移动的量 两次位置的变化量
        Vector2 movePosition = moveLayout.GetComponent<RectTransform>().anchoredPosition + eventData.delta;
        float offsetY = Mathf.Abs(movePosition.y);
        float offsetX = Mathf.Abs(movePosition.x);
        float clampX = Mathf.Clamp(movePosition.x, -200.0f + offsetY, 200.0f - offsetY);
        float clampY = Mathf.Clamp(movePosition.y, -200.0f + offsetX, 200.0f - offsetX);
        //基于锚点坐标系进行位移
        moveLayout.GetComponent<RectTransform>().anchoredPosition = new Vector2(clampX, clampY);

        //判断朝向
        OnPutDirection(clampX,clampY);

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isEndDrag == false)
        {
            //未放置
            moveLayout.GetComponent<RectTransform>().position = originalPosition;
        }
        else
        {
            StartCoroutine(OnPlayerStart());
            gameManege.audio.GetComponent<CS_Audio>().audioSource.PlayOneShot(gameManege.audio.GetComponent<CS_Audio>().put);
            gameManege.playerList.Add(player.gameObject);
            UIManege uiManege = GameObject.Find("Canvas").GetComponent<UIManege>();
            uiManege.ResetButtonPosition(player.GetComponent<RoleControl>().playerButton);
            Time.timeScale = uiManege.uiPlay.timeSpeed;
        }
    }

    //角色放置的朝向
    private void OnPutDirection(float clampX,float clampY )
    {
        if (clampX <0f && Mathf.Abs(clampY)<30.0f)
        {
            isEndDrag = false;
            player.Find("AttackRange").gameObject.SetActive(false);
            if (clampX < -130.0f)
            {
                player.Find("AttackRange").rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                player.Find("body").rotation = Quaternion.Euler(new Vector3(-55, 180, 0));
                player.Find("skill").rotation = Quaternion.Euler(new Vector3(90, 0, 90));
                player.Find("AttackRange").gameObject.SetActive(true);
                isEndDrag = true;
            }
        }
        else if(clampX > 0f & Mathf.Abs(clampY) < 30.0f)
        {
            isEndDrag = false;
            player.Find("AttackRange").gameObject.SetActive(false);
            if (clampX > 130.0f)
            {
                player.Find("AttackRange").rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.Find("body").rotation = Quaternion.Euler(new Vector3(55, 0, 0));
                player.Find("skill").rotation = Quaternion.Euler(new Vector3(90, 0, -90));
                player.Find("AttackRange").gameObject.SetActive(true);
                isEndDrag = true;
            }
        }
        else if (clampY < 0 && Mathf.Abs(clampX) < 30.0f)
        {
            isEndDrag = false;
            player.Find("AttackRange").gameObject.SetActive(false);
            if (clampY < -130.0f)
            {
                player.Find("AttackRange").rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                player.Find("body").rotation = Quaternion.Euler(new Vector3(55, 0, 0));
                player.Find("skill").rotation = Quaternion.Euler(new Vector3(90, 0, 180));
                player.Find("AttackRange").gameObject.SetActive(true);
                isEndDrag = true;
            }
        }
        else if (clampY > 0 && Mathf.Abs(clampX) < 30.0f)
        {
            isEndDrag = false;
            player.Find("AttackRange").gameObject.SetActive(false);
            if (clampY > 130.0f)
            {
                player.Find("AttackRange").rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                player.Find("body").rotation = Quaternion.Euler(new Vector3(55, 0, 0));
                player.Find("skill").rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                player.Find("AttackRange").gameObject.SetActive(true);
                isEndDrag = true;
            }
        }
    }

    IEnumerator OnPlayerStart()
    {
        float AnimeTime = roleControl.roleAnimationSkele.skeleton.Data.FindAnimation("Start").Duration;
        roleControl.roleAnimationSkele.AnimationState.SetAnimation(0, "Start", false);
        player.GetComponent<Collider>().enabled = true;
        player.Find("BaseUI").Find("healthBackground").gameObject.SetActive(true);
        player.Find("AttackRange").gameObject.SetActive(false);
        player.Find("BaseUI").Find("PutUI").Find("TowardsLayout").gameObject.SetActive(false);
        player.Find("BaseUI").Find("PutUI").Find("CancelLayout").gameObject.SetActive(false);
        //先不显示imag   最后再让其失效  否者脚本被关闭  后续代码无法运行
        player.Find("BaseUI").Find("PutUI").Find("MoveLayout").GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(AnimeTime);
        roleControl.isPut = true;
        player.Find("BaseUI").Find("PutUI").Find("MoveLayout").gameObject.SetActive(false);
    }
}
