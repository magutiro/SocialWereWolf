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

    //�ύX���Ď�����l
    private ReactiveDictionary<int, string> _playerId = new ReactiveDictionary<int, string>();

    //���g�̒l���������J���邽�߂�Dictionary(����Dictionary�̒l��ς��Ă�ReactiveDictionary���͕ς��Ȃ�)
    public Dictionary<int, string> ValueDictionary => _playerId.ToDictionary(pair => pair.Key, pair => pair.Value);

    //ReactiveDictionary�̂���IObservable���������J���A������o�^�ł���悤��
    public IObservable<DictionaryAddEvent<int, string>> AddObservable => _playerId.ObserveAdd();

    public List<int> _jobConstitution = new List<int>();
    //�I�u�W�F�N�g���X�|�[�������Ƃ�
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetPlayerCountServerRpc();
        }
    }
    // �T�[�o�[���Ŏ��s�����
    [Unity.Netcode.ServerRpc(RequireOwnership = true)]
    //�T�[�o�[�����v���C���[ID���N���C�A���g�ɑ��M
    private void SetPlayerCountServerRpc()
    {
        Dictionary<int, string> vueDictionary = ValueDictionary.ToDictionary(pair => pair.Key, pair => pair.Value);

        foreach (var dir in vueDictionary)
        {
            SetPlayerDictionaryClientRpc(dir.Key, dir.Value);
        }
        //�v���C���[�̐l���J�E���g��ǉ�
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
        //�Q�[���̏�Ԃ��ω������Ƃ���Init()�����s
        _gameState
            .DistinctUntilChanged()
            .Subscribe(_ => Init());
        //���g�̃v���C���[�J�E���g���������Ƃ���OnAddPlayerCount���Ăяo��
        if (IsClient)
        {
            _playerCount.OnValueChanged += OnAddPlayerCount();
        }
    }
    //�v���C���[�J�E���g�����������ƂɎ��g��ID�Ɩ��O��o�^����
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
            GameStateMethod();// �c�莞�Ԃ��v�Z����
            _gameTime.Value -= Time.deltaTime;
            // �[���b�ȉ��ɂȂ�Ȃ��悤�ɂ���
            if (_gameTime.Value <= 0.0f)
            {
                _gameTime.Value = 0.0f;
            }
        }
        //xx�Fxx
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
