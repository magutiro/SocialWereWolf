using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using VivoxUnity;
public class VivoxManager : MonoBehaviour
{
    private Client _client = null;
    /// <summary>
    /// �h���C��
    /// </summary>
    [SerializeField]
    private string _domain = "GET VALUE FROM VIVOX DEVELOPER PORTAL";

    /// <summary>
    /// ���s��
    /// </summary>
    [SerializeField]
    private string _issuer = "GET VALUE FROM VIVOX DEVELOPER PORTAL";


    /// <summary>
    /// API �G���h�|�C���g
    /// </summary>
    [SerializeField]
    private string _server = "https://GETFROMPORTAL.www.vivox.com/api2";

    /// <summary>
    /// �V�[�N���b�g�L�[
    /// </summary>
    [SerializeField]
    private string _tokenKey = "GET VALUE FROM VIVOX DEVELOPER PORTAL";

    /// <summary>
    /// �����Ƃ��̈ʒu��`
    /// </summary>
    [SerializeField]
    private Transform _ear = null;

    /// <summary>
    /// ����ׂ�Ƃ��̈ʒu��`
    /// </summary>
    [SerializeField]
    private Transform _mouth = null;
    /// <summary>
    /// �����̋����Ȃǂ̒�`
    /// </summary>
    [SerializeField]
    private int audibleDistance = 10;
    [SerializeField]
    private int conversationalDistance = 1;
    [SerializeField]
    float audioFadeIntensityByDistanceaudio = 1.2f;

    Channel3DProperties channel;
    private AccountId _accountId = null;

    private ILoginSession _loginSession = null;
    void Start()
    {
        channel = new Channel3DProperties(audibleDistance,conversationalDistance,audioFadeIntensityByDistanceaudio,AudioFadeModel.LinearByDistance);
        Login();
    }

    void Update()
    {
        if (!_ear && GameObject.Find("Player(Clone)"))
        {
            _ear = _mouth = GameObject.Find("Player(Clone)").transform;
        }
    }
    private void Awake()
    {
        CreateAccount("adcde"+ UserLoginData.userName, UserLoginData.userName);
        _client = new Client();
        _client.Initialize();
    }
    /// <summary>
    /// �A�J�E���g���쐬
    /// </summary>
    /// <param name="uniqueId"></param>
    /// <param name="displayName"></param>
    public void CreateAccount(string uniqueId, string displayName)
    {
        _accountId = new AccountId(_issuer, uniqueId, _domain, displayName);
    }
    public void Login()
    {
        _loginSession = _client.GetLoginSession(_accountId);

        Uri serverUri = new Uri(_server);
        string token = _loginSession.GetLoginToken(_tokenKey, TimeSpan.FromSeconds(90d));
        _loginSession.PropertyChanged += OnLoginStateChanged;
        _loginSession.BeginLogin(serverUri, token, asyncResult =>
        {
            try
            {
                _loginSession.EndLogin(asyncResult);
            }
            catch (Exception e)
            {
                _loginSession.PropertyChanged -= OnLoginStateChanged;
                Debug.Log("TODO: ���O�C���Ɏ��s�����Ƃ��̏����������ɏ���");
                return;
            }
        });
    }
    public void Logout()
    {
        if (_loginSession == null || _loginSession.State == LoginState.LoggingOut || _loginSession.State == LoginState.LoggedOut)
        {
            Debug.LogWarning("���O�C�����Ă��܂���B");
            return;
        }

        _loginSession.Logout();
        _loginSession.PropertyChanged -= OnLoginStateChanged;

        Debug.Log("TODO: �Ӑ}�������O�A�E�g�̂Ƃ��̏����������ɏ���");
    }
    private void OnLoginStateChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (propertyChangedEventArgs.PropertyName != nameof(ILoginSession.State))
        {
            return;
        }

        if (sender is ILoginSession loginSession)
        {
            switch (loginSession.State)
            {
                case LoginState.LoggingIn:
                    Debug.Log("TODO: ���O�C�����J�n�����Ƃ��̏����������ɏ���");
                    break;

                case LoginState.LoggedIn:
                    Debug.Log("TODO: ���O�C�������������Ƃ��̏����������ɏ���"); 
                    JoinChannel("Room", VivoxUnity.ChannelType.Positional);
                    break;

                case LoginState.LoggingOut:
                    // MEMO: �����ɗ��邱�Ƃ͂Ȃ��H
                    break;

                case LoginState.LoggedOut:
                    _loginSession.PropertyChanged -= OnLoginStateChanged;
                    Debug.Log("TODO: �ؒf���A�Ӑ}���Ȃ����O�A�E�g�̂Ƃ��̏����������ɏ���");
                    break;

                default:
                    break;
            }
        }
    }
    public void JoinChannel(string channelName, ChannelType channelType)
    {
        if (_loginSession.State == LoginState.LoggedIn)
        {
            ChannelId channelId;
            if (channelType == ChannelType.Positional)
            {
                channelId = new ChannelId(_issuer, channelName, _domain, channelType,channel);
            }
            else
            {
                channelId = new ChannelId(_issuer, channelName, _domain, channelType);
            }
            IChannelSession channelSession = _loginSession.GetChannelSession(channelId);
            // �`�����l���쐬���Ɏ��̂悤�ɃC�x���g��o�^
            channelSession.PropertyChanged += OnChannelStateChanged;

            string token = channelSession.GetConnectToken(_tokenKey, TimeSpan.FromSeconds(90d));
            // 1�Ԗڂ̈����͉������g�p���邩�A2�Ԗڂ̈����̓e�L�X�g���g�p���邩
            channelSession.BeginConnect(true, false, true, token, asyncResult =>
            {
                try
                {
                    channelSession.EndConnect(asyncResult);
                }
                catch (Exception e)
                {
                    channelSession.Parent.DeleteChannelSession(channelSession.Channel);
                    Debug.Log("�`�����l���Q���Ɏ��s");
                    return;
                }
            });
        }
        else
        {
            Debug.LogWarning("���O�C�����Ă��Ȃ����߃`�����l���ɎQ���ł��܂���B");
        }
    }
    public void LeaveChannel(string channelName)
    {
        var channelSession = _loginSession.ChannelSessions.FirstOrDefault(x => x.Channel.Name.Equals(channelName));
        if (channelSession != null)
        {
            channelSession.Disconnect();
            // ���`�����l���ޏo���̃C�x���g�ŌĂ�
            // channelSession.Parent.DeleteChannelSession(channelSession.Channel);
        }
        else
        {
            Debug.LogWarning("�Y���̃`�����l���ɎQ�����Ă��܂���B");
        }
    }
    private void LateUpdate()
    {
        var channelSessions = _loginSession?.ChannelSessions;
        if (channelSessions != null)
        {
            foreach (var channelSession in channelSessions)
            {
                foreach (var participant in channelSession.Participants)
                {
                    //Debug.Log(channelSession.Channel.Name + "��" + participant.Account.DisplayName + "���Q�����B");
                }
            }
        }
        if (_loginSession != null)
        {
            // MEMO: �{���͖��t���[���ĂԂ悤�Ȃ��Ƃ͂��Ȃ�
            foreach (var channelSession in _loginSession.ChannelSessions.Where(x => x.Channel.Type == ChannelType.Positional && x.AudioState == ConnectionState.Connected))
            {
                channelSession.Set3DPosition(_mouth.position, _ear.position, _ear.forward, _ear.up);
            }
        }
    }
    private void OnChannelStateChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (sender is IChannelSession channelSession)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(IChannelSession.ChannelState))
            {
                switch (channelSession.ChannelState)
                {
                    case ConnectionState.Disconnected:
                        channelSession.PropertyChanged -= OnChannelStateChanged;
                        channelSession.Parent.DeleteChannelSession(channelSession.Channel);
                        Debug.Log(channelSession.Channel + "����ޏo���܂����B");
                        break;

                    case ConnectionState.Connecting:
                        Debug.Log(channelSession.Channel + "�ɎQ���J�n���܂����B");
                        break;

                    case ConnectionState.Connected:
                        Debug.Log(channelSession.Channel + "�ɎQ�����܂����B");
                        break;

                    case ConnectionState.Disconnecting:
                        Debug.Log(channelSession.Channel + "����ޏo�J�n���܂����B");
                        break;

                    default:
                        break;
                }
            }
        }
    }
    private void OnApplicationQuit()
    {
        if (_client != null)
        {
            _client.Uninitialize();
            _client = null;
        }
    }
}
