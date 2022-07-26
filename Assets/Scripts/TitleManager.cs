using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleManager : MonoBehaviour
{
    public InputField IpfUserName;
    
    //　非同期動作で使用するAsyncOperation
    private AsyncOperation async;
    //　シーンロード中に表示するUI画面
    [SerializeField]
    private GameObject loadUI = null;
    //　タイトルに表示するUI画面
    [SerializeField]
    private GameObject titleUI = null;
    //　読み込み率を表示するスライダー
    [SerializeField]
    private Slider slider = null;

    [SerializeField]
    TextMeshProUGUI result = new TextMeshProUGUI();
    // Start is called before the first frame update
    void Start()
    {
#if SERVER
        SceneManager.LoadScene("LobbyScene");
#endif

        if (result)
        {
            result.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void OnClickLoginButton()
    {
        // 入力したユーザー名の取得 
        UserLoginData.userName.Value = IpfUserName.text;

        // プレイ画面へ遷移
        //SceneManager.LoadScene("InGameScene");
        NextScene();
    }

    public void OnHomeScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    /// <summary>
    /// アプリ終了ボタン押下時の処理
    /// </summary>
    public void OnClickExitButton()
    {
        Application.Quit();
    }
    public void NextScene()
    {
        //　ロード画面UIをアクティブにする
        loadUI.SetActive(true);
        //　ロード画面UIをアクティブにする
        titleUI.SetActive(false);

        //　コルーチンを開始
        StartCoroutine("LoadData");
    }

    IEnumerator LoadData()
    {
        // シーンの読み込みをする
        async = SceneManager.LoadSceneAsync("LobbyScene");

        //　読み込みが終わるまで進捗状況をスライダーの値に反映させる
        while (!async.isDone)
        {
            var progressVal = Mathf.Clamp01(async.progress / 0.9f);
            slider.value = progressVal;
            yield return null;
        }
    }
}
