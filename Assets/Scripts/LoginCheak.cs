using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginCheak : MonoBehaviour
{
    public Sprite[] pic;
    public InputField userNameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Image Avatar;
    public Image Accountpass;
    public Image PassWord;

    public static String userName;
    public static Sprite avatar;

    void Start()
    {
        //为Button添加OnClick事件
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }


    private void Update()
    {
        if (userNameInput.text == "admin01")
        {
            Avatar.sprite = pic[1];
            Accountpass.sprite = pic[4];
            if(passwordInput.text == "123123")
            {
                PassWord.sprite = pic[4];
            }
        }
        else if(userNameInput.text == "admin02")
        {
            Avatar.sprite = pic[2];
            Accountpass.sprite = pic[4];
            if (passwordInput.text == "123123")
            {
                PassWord.sprite = pic[4];
            }
        }
        else if(userNameInput.text == "admin03")
        {
            Avatar.sprite = pic[3];
            Accountpass.sprite = pic[4];
            if (passwordInput.text == "123123")
            {
                PassWord.sprite = pic[4];
            }
        }
        else
        {
            Accountpass.sprite = pic[0];
        }
    }
    void OnLoginButtonClicked()
    {
        // 进行用户名和密码验证
        if((userNameInput.text == "admin01" && passwordInput.text == "123123")
            || (userNameInput.text == "admin02" && passwordInput.text == "123123")
            || (userNameInput.text == "admin03" && passwordInput.text == "123123"))
        {
            // 验证通过，跳转到游戏场景
            userName = userNameInput.text;
            avatar = Avatar.sprite;
            SceneManager.LoadScene("CampusScenes");
        }
        else
        {
            // 验证失败，弹出提示信息
            Debug.Log("用户名或密码错误！");
        }

    }
}

