using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class AccountManage : MonoBehaviour
{
    [SerializeField] GameObject accountList;//组件
    [SerializeField] GameObject switchSignButton;//切换注册按钮
    [SerializeField] GameObject signLayout;//注册界面
    [SerializeField] GameObject loginLayout;//登录界面
    [SerializeField] GameObject loginButton;//登录按钮
    [SerializeField] GameObject entAccount;//输入账号
    [SerializeField] GameObject entPassword;//输入密码
    [SerializeField] GameObject agreement;//协议
    [SerializeField] GameObject tips;//提示
    [SerializeField] GameObject signButton;//注册按钮
    [SerializeField] GameObject breakButton;//返回
    [SerializeField] AccountDataList accountDatasList;//账号类数据
    private List<TMP_Dropdown.OptionData> accountListOptions;//账号列表
    private string accountPath = "D:/UnityAccountData/accout.json";

    //账号类数据
    public class AccountDataList
    {
        public List<AccountData> accountDatas;
        public AccountDataList()
        {
            accountDatas = new List<AccountData>();
        }
    }

    void Start()
    {
        //存在账号资源文件，读取载入列表
        if (File.Exists(accountPath))
        {
            accountListOptions = new List<TMP_Dropdown.OptionData>();
            string accoutJson = File.ReadAllText(accountPath);
            accountDatasList = JsonUtility.FromJson<AccountDataList>(accoutJson);
            foreach(AccountData accountData in accountDatasList.accountDatas)
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
                optionData.text = accountData.userAccount;
                accountListOptions.Add(optionData);
            }
            accountList.GetComponent<TMP_Dropdown>().options = accountListOptions;
        }

        switchSignButton.GetComponent<Button>().onClick.AddListener(ClickSwitchSignButton);
        loginButton.GetComponent<Button>().onClick.AddListener(ClickLoginButton);
        signButton.GetComponent<Button>().onClick.AddListener(ClickSignButton);
        breakButton.GetComponent<Button>().onClick.AddListener(ClickBreakButton);
    }
    //按下注册新账号
    private void ClickSwitchSignButton()
    {
        loginLayout.SetActive(false);
        signLayout.SetActive(true);
    }
    //按下登录按钮
    private void ClickLoginButton()
    {
        SceneManager.LoadScene("Start");
    }
    //按下注册账号按钮
    private void ClickSignButton()
    {
        //协议勾选
        if (agreement.GetComponent<Toggle>().isOn == false)
        {
            StartCoroutine(ErrorTips(0));
            return;
        }

        //未输入账号
        if(entAccount.GetComponent<InputField>().text == "")
        {
            StartCoroutine(ErrorTips(1));
            return;
        }

        //请输入密码
        if (entPassword.GetComponent<InputField>().text == "")
        {
            StartCoroutine(ErrorTips(2));
            return;
        }

        //输入账号信息
        AccountData currentData = new AccountData();
        currentData.userAccount = entAccount.GetComponent<InputField>().text;
        currentData.password = entPassword.GetComponent<InputField>().text;

        //重复账号检测
        //账号密码正确  -> 登录
        //账号密码错误  -> 报错
        //可加速，让账号列表有序 二分查找
        foreach (AccountData data in accountDatasList.accountDatas)
        {
            //账号密码正确
            if (currentData.userAccount == data.userAccount && currentData.password == data.password)
            {
                entAccount.GetComponent<InputField>().text = "";
                entPassword.GetComponent<InputField>().text = "";
                agreement.GetComponent<Toggle>().isOn = false;
                signLayout.SetActive(false);
                loginLayout.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            else if(currentData.userAccount == data.userAccount && currentData.password != data.password)
            {
                StartCoroutine(ErrorTips(1));
                return;
            }
        }


        //将账号注册并存储起来
        if (File.Exists(accountPath))
        {
            accountDatasList.accountDatas.Add(currentData);
            TMP_Dropdown.OptionData currentOption = new TMP_Dropdown.OptionData();
            currentOption.text = currentData.userAccount;
            accountListOptions.Add(currentOption);
            accountList.GetComponent<TMP_Dropdown>().options = accountListOptions;
            string json =  JsonUtility.ToJson(accountDatasList);
            File.WriteAllText(accountPath, json);
        }
        else
        {
            //第一次注册
            accountDatasList = new AccountDataList();
            accountListOptions = new List<TMP_Dropdown.OptionData>();
            accountDatasList.accountDatas.Add(currentData);
            TMP_Dropdown.OptionData currentOption = new TMP_Dropdown.OptionData();
            currentOption.text = currentData.userAccount;
            accountListOptions.Add(currentOption);
            string json = JsonUtility.ToJson(accountDatasList);
            File.WriteAllText(accountPath, json);
        }

        //页面跳转回选择账号页面
        entAccount.GetComponent<InputField>().text = "";
        entPassword.GetComponent<InputField>().text = "";
        agreement.GetComponent<Toggle>().isOn = false;
        signLayout.SetActive(false);
        loginLayout.SetActive(true);
        
    }
    //错误提示
    IEnumerator ErrorTips(int index)
    {
        switch (index)
        {
            case 0:
                tips.GetComponent<Text>().text = "请勾选同意协议！";
                tips.SetActive(true);
                yield return new WaitForSeconds(3);
                tips.SetActive(false);
                yield break;
            case 1:
                tips.GetComponent<Text>().text = "请输入正确的密码！";
                tips.SetActive(true);
                yield return new WaitForSeconds(3);
                tips.SetActive(false);
                yield break;
            case 2:
                tips.GetComponent<Text>().text = "请输入密码！";
                tips.SetActive(true);
                yield return new WaitForSeconds(3);
                tips.SetActive(false);
                yield break;
        }
    }
    //按下返回按钮
    private void ClickBreakButton()
    {
        signLayout.SetActive(false);
        loginLayout.SetActive(true);
        accountList.GetComponent<TMP_Dropdown>().options = accountListOptions;
    }
}
