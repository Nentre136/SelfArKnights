using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Login : MonoBehaviour
{
    private Button loginButton;
    void Start()
    {
        loginButton = GetComponent<Button>();
        //��Ӽ�����
        loginButton.onClick.AddListener(SwitchScene);
    }

    private void SwitchScene()
    {
        //����Ŀ�곡��
        SceneManager.LoadScene("Start");
    }

}
