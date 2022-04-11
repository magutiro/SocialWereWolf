using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public InputField IpfUserName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickLoginButton()
    {
        // 入力したユーザー名の取得 
        UserLoginData.userName = IpfUserName.text;

        // プレイ画面へ遷移
        SceneManager.LoadScene("InGameScene");
    }

    /// <summary>
    /// アプリ終了ボタン押下時の処理
    /// </summary>
    public void OnClickExitButton()
    {
        Application.Quit();
    }

}
