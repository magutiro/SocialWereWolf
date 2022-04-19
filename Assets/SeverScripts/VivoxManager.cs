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
    /// ドメイン
    /// </summary>
    [SerializeField]
    private string _domain = "GET VALUE FROM VIVOX DEVELOPER PORTAL";

    /// <summary>
    /// 発行者
    /// </summary>
    [SerializeField]
    private string _issuer = "GET VALUE FROM VIVOX DEVELOPER PORTAL";


    /// <summary>
    /// API エンドポイント
    /// </summary>
    [SerializeField]
    private string _server = "https://GETFROMPORTAL.www.vivox.com/api2";

    /// <summary>
    /// シークレットキー
    /// </summary>
    [SerializeField]
    private string _tokenKey = "GET VALUE FROM VIVOX DEVELOPER PORTAL";

    /// <summary>
    /// 聞くときの位置定義
    /// </summary>
    [SerializeField]
    private Transform _ear = null;

    /// <summary>
    /// しゃべるときの位置定義
    /// </summary>
    [SerializeField]
    private Transform _mouth = null;
    /// <summary>
    /// 音声の距離などの定義
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
    /// アカウントを作成
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
                Debug.Log("TODO: ログインに失敗したときの処理をここに書く");
                return;
            }
        });
    }
    public void Logout()
    {
        if (_loginSession == null || _loginSession.State == LoginState.LoggingOut || _loginSession.State == LoginState.LoggedOut)
        {
            Debug.LogWarning("ログインしていません。");
            return;
        }

        _loginSession.Logout();
        _loginSession.PropertyChanged -= OnLoginStateChanged;

        Debug.Log("TODO: 意図したログアウトのときの処理をここに書く");
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
                    Debug.Log("TODO: ログインを開始したときの処理をここに書く");
                    break;

                case LoginState.LoggedIn:
                    Debug.Log("TODO: ログインが完了したときの処理をここに書く"); 
                    JoinChannel("Room", VivoxUnity.ChannelType.Positional);
                    break;

                case LoginState.LoggingOut:
                    // MEMO: ここに来ることはない？
                    break;

                case LoginState.LoggedOut:
                    _loginSession.PropertyChanged -= OnLoginStateChanged;
                    Debug.Log("TODO: 切断等、意図しないログアウトのときの処理をここに書く");
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
            // チャンネル作成時に次のようにイベントを登録
            channelSession.PropertyChanged += OnChannelStateChanged;

            string token = channelSession.GetConnectToken(_tokenKey, TimeSpan.FromSeconds(90d));
            // 1番目の引数は音声を使用するか、2番目の引数はテキストを使用するか
            channelSession.BeginConnect(true, false, true, token, asyncResult =>
            {
                try
                {
                    channelSession.EndConnect(asyncResult);
                }
                catch (Exception e)
                {
                    channelSession.Parent.DeleteChannelSession(channelSession.Channel);
                    Debug.Log("チャンネル参加に失敗");
                    return;
                }
            });
        }
        else
        {
            Debug.LogWarning("ログインしていないためチャンネルに参加できません。");
        }
    }
    public void LeaveChannel(string channelName)
    {
        var channelSession = _loginSession.ChannelSessions.FirstOrDefault(x => x.Channel.Name.Equals(channelName));
        if (channelSession != null)
        {
            channelSession.Disconnect();
            // ↓チャンネル退出時のイベントで呼ぶ
            // channelSession.Parent.DeleteChannelSession(channelSession.Channel);
        }
        else
        {
            Debug.LogWarning("該当のチャンネルに参加していません。");
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
                    //Debug.Log(channelSession.Channel.Name + "に" + participant.Account.DisplayName + "が参加中。");
                }
            }
        }
        if (_loginSession != null)
        {
            // MEMO: 本来は毎フレーム呼ぶようなことはしない
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
                        Debug.Log(channelSession.Channel + "から退出しました。");
                        break;

                    case ConnectionState.Connecting:
                        Debug.Log(channelSession.Channel + "に参加開始しました。");
                        break;

                    case ConnectionState.Connected:
                        Debug.Log(channelSession.Channel + "に参加しました。");
                        break;

                    case ConnectionState.Disconnecting:
                        Debug.Log(channelSession.Channel + "から退出開始しました。");
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
