using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using Unity.Netcode;
using System;
using System.Linq;

public enum GameState
{
    Room = 0,
    Morning,
    Daytime,
    Evening,
    Night
}
[System.Serializable]
public class GameStateReactiveProperty : ReactiveProperty<GameState>
{
    public GameStateReactiveProperty() { }

    public GameStateReactiveProperty(GameState initialValue) : base(initialValue) { }

}
public class GameController : NetworkBehaviour
{
    //float _gameTime = 300;
    [SerializeField]
    private Text _timeText;
    [SerializeField]
    private GameObject _meetingPanel;
    [SerializeField]
    private GameObject _nightPanel;
    [SerializeField]
    private GameObject _dayPanel;
    [SerializeField]
    private const float _morningTime = 90;
    [SerializeField]
    private const float _dayTime = 300;
    [SerializeField]
    private const float _nightTime = 60;
    [SerializeField]
    private const float _eveningTime = 150;

    public GameStateReactiveProperty _gameState = new GameStateReactiveProperty(GameState.Daytime);

    private NetworkVariable<int> _playerCount = new NetworkVariable<int>(0);

    private NetworkVariable<float> _gameTime = new NetworkVariable<float>(300);

    //変更を監視する値
    private ReactiveDictionary<int, string> _playerId = new ReactiveDictionary<int, string>();

    //中身の値だけを公開するためのDictionary(このDictionaryの値を変えてもReactiveDictionary側は変わらない)
    public Dictionary<int, string> ValueDictionary => _playerId.ToDictionary(pair => pair.Key, pair => pair.Value);

    //ReactiveDictionaryのうちIObservableだけを公開し、処理を登録できるように
    public IObservable<DictionaryAddEvent<int, string>> AddObservable => _playerId.ObserveAdd();

    public List<int> _jobConstitution = new List<int>();
    //オブジェクトがスポーンしたとき
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetPlayerCountServerRpc();
        }
    }
    // サーバー側で実行される
    [Unity.Netcode.ServerRpc(RequireOwnership = true)]
    //サーバーが持つプレイヤーIDをクライアントに送信
    private void SetPlayerCountServerRpc()
    {
        Dictionary<int, string> vueDictionary = ValueDictionary.ToDictionary(pair => pair.Key, pair => pair.Value);

        foreach (var dir in vueDictionary)
        {
            SetPlayerDictionaryClientRpc(dir.Key, dir.Value);
        }
        //プレイヤーの人数カウントを追加
        _playerCount.Value++;
    }

    [Unity.Netcode.ClientRpc]
    private void SetPlayerDictionaryClientRpc(int key, string value)
    {
        _playerId.Add(key, value);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "InGameScene")
        {
            _timeText = GameObject.Find("TimeText").GetComponent<Text>();
        }
        //ゲームの状態が変化したときにInit()を実行
        _gameState
            .DistinctUntilChanged()
            .Subscribe(_ => Init());
        //自身のプレイヤーカウントが増えたときにOnAddPlayerCountを呼び出す
        if (IsClient)
        {
            _playerCount.OnValueChanged += OnAddPlayerCount();
        }
    }
    //プレイヤーカウントが増えたあとに自身のIDと名前を登録する
    private NetworkVariable<int>.OnValueChangedDelegate OnAddPlayerCount()
    {
        _playerId.Add(_playerCount.Value, UserLoginData.userName.Value);
        SetPlayerIDServerRpc(_playerCount.Value, UserLoginData.userName.Value);
        return null;
    }
    [ServerRpc]
    private void SetPlayerIDServerRpc(int key, string value)
    {
        _playerId.Add(key, value);
    }
    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            GameStateMethod();// 残り時間を計算する
            _gameTime.Value -= Time.deltaTime;
            // ゼロ秒以下にならないようにする
            if (_gameTime.Value <= 0.0f)
            {
                _gameTime.Value = 0.0f;
            }
        }
        //xx：xx
        _timeText.text = Mathf.FloorToInt(_gameTime.Value / 60) + ":" + (_gameTime.Value % 60).ToString("f1");

    }
    void Init()
    {
        switch (_gameState.Value)
        {
            case GameState.Morning:
                InitMorning();
                break;
            case GameState.Daytime:
                InitDaytime();
                break;
            case GameState.Evening:
                InitEvening();
                break;
            case GameState.Night:
                InitNight();
                break;
        }
    }
    void InitMorning()
    {
        if (IsServer)
        {
            _gameTime.Value = _morningTime;
        }
        _meetingPanel.SetActive(true);
        _nightPanel.SetActive(false);
    }

    void InitDaytime()
    {
        if (IsServer)
        {
            _gameTime.Value = _dayTime;
        }
        _dayPanel.SetActive(true);
        _meetingPanel.SetActive(false);
    }
    void InitEvening()
    {
        if (IsServer)
        {
            _gameTime.Value = _eveningTime;
        }
        _dayPanel.SetActive(false);
        _meetingPanel.SetActive(true);
    }
    void InitNight()
    {
        if (IsServer)
        {
            _gameTime.Value = _nightTime;
        }
        _meetingPanel.SetActive(false);
        _nightPanel.SetActive(true);
    }
    void GameStateMethod()
    {
        switch (_gameState.Value)
        {
            case GameState.Morning:
                UpdateMorning();
                break;
            case GameState.Daytime:
                UpdeteDaytime();
                break;
            case GameState.Evening:
                UpdateEvening();
                break;
            case GameState.Night:
                UpdateNight();
                break;
        }
    }

    void UpdateMorning()
    {
        if (_gameTime.Value <= 0.0f)
        {
            _gameState.Value = GameState.Daytime;
        }
    }

    void UpdeteDaytime()
    {
        if (_gameTime.Value <= 0.0f)
        {
            _gameState.Value = GameState.Evening;
        }
    }

    void UpdateEvening()
    {
        if (_gameTime.Value <= 0.0f)
        {
            _gameState.Value = GameState.Night;
        }
    }
    void UpdateNight()
    {
        if (_gameTime.Value <= 0.0f)
        {
            _gameState.Value = GameState.Morning;
        }
    }

}
