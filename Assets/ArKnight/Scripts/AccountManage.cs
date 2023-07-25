using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class AccountManage : MonoBehaviour
{
    [SerializeField] GameObject accountList;//���
    [SerializeField] GameObject switchSignButton;//�л�ע�ᰴť
    [SerializeField] GameObject signLayout;//ע�����
    [SerializeField] GameObject loginLayout;//��¼����
    [SerializeField] GameObject loginButton;//��¼��ť
    [SerializeField] GameObject entAccount;//�����˺�
    [SerializeField] GameObject entPassword;//��������
    [SerializeField] GameObject agreement;//Э��
    [SerializeField] GameObject tips;//��ʾ
    [SerializeField] GameObject signButton;//ע�ᰴť
    [SerializeField] GameObject breakButton;//����
    [SerializeField] AccountDataList accountDatasList;//�˺�������
    private List<TMP_Dropdown.OptionData> accountListOptions;//�˺��б�
    private string accountPath = "D:/UnityAccountData/accout.json";

    //�˺�������
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
        //�����˺���Դ�ļ�����ȡ�����б�
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
    //����ע�����˺�
    private void ClickSwitchSignButton()
    {
        loginLayout.SetActive(false);
        signLayout.SetActive(true);
    }
    //���µ�¼��ť
    private void ClickLoginButton()
    {
        SceneManager.LoadScene("Start");
    }
    //����ע���˺Ű�ť
    private void ClickSignButton()
    {
        //Э�鹴ѡ
        if (agreement.GetComponent<Toggle>().isOn == false)
        {
            StartCoroutine(ErrorTips(0));
            return;
        }

        //δ�����˺�
        if(entAccount.GetComponent<InputField>().text == "")
        {
            StartCoroutine(ErrorTips(1));
            return;
        }

        //����������
        if (entPassword.GetComponent<InputField>().text == "")
        {
            StartCoroutine(ErrorTips(2));
            return;
        }

        //�����˺���Ϣ
        AccountData currentData = new AccountData();
        currentData.userAccount = entAccount.GetComponent<InputField>().text;
        currentData.password = entPassword.GetComponent<InputField>().text;

        //�ظ��˺ż��
        //�˺�������ȷ  -> ��¼
        //�˺��������  -> ����
        //�ɼ��٣����˺��б����� ���ֲ���
        foreach (AccountData data in accountDatasList.accountDatas)
        {
            //�˺�������ȷ
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


        //���˺�ע�Ტ�洢����
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
            //��һ��ע��
            accountDatasList = new AccountDataList();
            accountListOptions = new List<TMP_Dropdown.OptionData>();
            accountDatasList.accountDatas.Add(currentData);
            TMP_Dropdown.OptionData currentOption = new TMP_Dropdown.OptionData();
            currentOption.text = currentData.userAccount;
            accountListOptions.Add(currentOption);
            string json = JsonUtility.ToJson(accountDatasList);
            File.WriteAllText(accountPath, json);
        }

        //ҳ����ת��ѡ���˺�ҳ��
        entAccount.GetComponent<InputField>().text = "";
        entPassword.GetComponent<InputField>().text = "";
        agreement.GetComponent<Toggle>().isOn = false;
        signLayout.SetActive(false);
        loginLayout.SetActive(true);
        
    }
    //������ʾ
    IEnumerator ErrorTips(int index)
    {
        switch (index)
        {
            case 0:
                tips.GetComponent<Text>().text = "�빴ѡͬ��Э�飡";
                tips.SetActive(true);
                yield return new WaitForSeconds(3);
                tips.SetActive(false);
                yield break;
            case 1:
                tips.GetComponent<Text>().text = "��������ȷ�����룡";
                tips.SetActive(true);
                yield return new WaitForSeconds(3);
                tips.SetActive(false);
                yield break;
            case 2:
                tips.GetComponent<Text>().text = "���������룡";
                tips.SetActive(true);
                yield return new WaitForSeconds(3);
                tips.SetActive(false);
                yield break;
        }
    }
    //���·��ذ�ť
    private void ClickBreakButton()
    {
        signLayout.SetActive(false);
        loginLayout.SetActive(true);
        accountList.GetComponent<TMP_Dropdown>().options = accountListOptions;
    }
}
