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
        // ���͂������[�U�[���̎擾 
        UserLoginData.userName = IpfUserName.text;

        // �v���C��ʂ֑J��
        SceneManager.LoadScene("InGameScene");
    }

    /// <summary>
    /// �A�v���I���{�^���������̏���
    /// </summary>
    public void OnClickExitButton()
    {
        Application.Quit();
    }

}
