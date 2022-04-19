using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public InputField IpfUserName;
    
    //�@�񓯊�����Ŏg�p����AsyncOperation
    private AsyncOperation async;
    //�@�V�[�����[�h���ɕ\������UI���
    [SerializeField]
    private GameObject loadUI = null;
    //�@�^�C�g���ɕ\������UI���
    [SerializeField]
    private GameObject titleUI = null;
    //�@�ǂݍ��ݗ���\������X���C�_�[
    [SerializeField]
    private Slider slider = null;
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
        //SceneManager.LoadScene("InGameScene");
        NextScene();
    }

    /// <summary>
    /// �A�v���I���{�^���������̏���
    /// </summary>
    public void OnClickExitButton()
    {
        Application.Quit();
    }
    public void NextScene()
    {
        //�@���[�h���UI���A�N�e�B�u�ɂ���
        loadUI.SetActive(true);
        //�@���[�h���UI���A�N�e�B�u�ɂ���
        titleUI.SetActive(false);

        //�@�R���[�`�����J�n
        StartCoroutine("LoadData");
    }

    IEnumerator LoadData()
    {
        // �V�[���̓ǂݍ��݂�����
        async = SceneManager.LoadSceneAsync("LobbyScene");

        //�@�ǂݍ��݂��I���܂Ői���󋵂��X���C�_�[�̒l�ɔ��f������
        while (!async.isDone)
        {
            var progressVal = Mathf.Clamp01(async.progress / 0.9f);
            slider.value = progressVal;
            yield return null;
        }
    }
}
