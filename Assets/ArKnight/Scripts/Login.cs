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
        //添加监听器
        loginButton.onClick.AddListener(SwitchScene);
    }

    private void SwitchScene()
    {
        //加载目标场景
        SceneManager.LoadScene("Start");
    }

}
