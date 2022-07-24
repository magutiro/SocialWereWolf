using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using Unity.Netcode;
using System;
using System.Linq;
using TMPro;

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
    private TextMeshProUGUI _timeText;
    [SerializeField]
    private GameObject _meetingPanel;
    [SerializeField]
    private GameObject _nightPanel;
    [SerializeField]
    private GameObject _dayPanel;
    [SerializeField]
    private GameObject _votePanel;
    [SerializeField]
    private float _morningTime = 90;
    [SerializeField]
    private float _dayTime = 300;
    [SerializeField]
    private float _nightTime = 60;
    [SerializeField]
    private float _eveningTime = 150;
    [SerializeField]
    private const int MAX_PLAYER = 9;

    public GameStateReactiveProperty _gameState = new GameStateReactiveProperty(GameState.Daytime);
    private NetworkVariable<int> _playerCount = new NetworkVariable<int>(0);

    private NetworkVariable<float> _gameTime = new NetworkVariable<float>(300);
    private NetworkVariable<int> _raidPlayerID = new NetworkVariable<int>();

    //�ύX���Ď�����l
    private ReactiveDictionary<int, string> _playerId = new ReactiveDictionary<int, string>();

    //���g�̒l���������J���邽�߂�Dictionary(����Dictionary�̒l��ς��Ă�ReactiveDictionary���͕ς��Ȃ�)
    public Dictionary<int, string> ValueDictionary => _playerId.ToDictionary(pair => pair.Key, pair => pair.Value);

    //ReactiveDictionary�̂���IObservable���������J���A������o�^�ł���悤��
    public IObservable<DictionaryAddEvent<int, string>> AddObservable => _playerId.ObserveAdd();

    public List<int> _jobConstitution = new List<int>();

    public PlayerManager pm;

    public PlayerVoteController _playerVoteController;
    public VoteController _voteController;

    public GameObject NightCamera;

    private bool isRaid = false;
    //�I�u�W�F�N�g���X�|�[�������Ƃ�
    public override void OnNetworkSpawn()
    {
        _gameTime.Value = _dayTime;
        if (IsOwner)
        {
            SetPlayerCountServerRpc();
        }
    }
    private void Awake()
    {
        //���g�̃v���C���[�J�E���g���������Ƃ���OnAddPlayerCount���Ăяo��
        if (IsClient)
        {
            _playerCount.OnValueChanged += OnAddPlayerCount();
        }
        if (IsServer)
        {
            PlayerJobSelecter.SetJobList();
        }
        _playerVoteController = GameObject.Find("Player(Clone)").GetComponent<PlayerVoteController>();
        _voteController = GameObject.Find("VoteController").GetComponent<VoteController>();
        pm = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "InGameScene")
        {
            _timeText = GameObject.Find("TimeText").GetComponent<TextMeshProUGUI>();
        }
        //�Q�[���̏�Ԃ��ω������Ƃ���Init()�����s
        _gameState
            .DistinctUntilChanged()
            .Subscribe(_ => Init());
        
    }
    void Update()
    {
        if (IsServer)
        {
            GameStateMethod();
            // �c�莞�Ԃ��v�Z����
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

    //�v���C���[�J�E���g�����������ƂɎ��g��ID�Ɩ��O��o�^����
    private NetworkVariable<int>.OnValueChangedDelegate OnAddPlayerCount()
    {
        if (IsOwner)
        {
            if (_playerId.ContainsKey(_playerCount.Value))
            {
                _playerId.Add(_playerCount.Value, UserLoginData.userName.Value);
            }
            SetPlayerIDServerRpc(_playerCount.Value, UserLoginData.userName.Value);
        }
        return null;
    }
    [ServerRpc]
    private void SetPlayerIDServerRpc(int key, string value)
    {
        _playerId.Add(key, value);
    }
    // Update is called once per frame

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
        if (_playerId.ContainsKey(key))
        {
            _playerId.Add(key, value);
        }
    }
    public void IsGameEnd()
    {
        int[] alive = { 0, 0 };
        int[] dead = {0, 0};
        foreach(var p in pm.playerList)
        {
            var player = p.GetComponent<PlayerController>()._player;
            var job = p.GetComponent<PlayerJobState>().playerjob.Value;
            if (player.playerState == Player.PlayerState.Alive)
            {
                
                alive[(int)job]++;
            }
            else
            {
                dead[(int)job]++;
            }
        }
        if(alive[0]+alive[1]+dead[0]+dead[1] != pm.playerList.Count)
        {
            Debug.Log("�l���G���[");
        }
        else if (alive[0] <= alive[1])
        {
            GameEnd(true);
        }else if (dead[1] == 1)
        {
            GameEnd(false);
        }
        else
        {
            GameEnd(false);
        }
    }
    public void GameEnd(bool isDualWinner)
    {
        if (isDualWinner) 
        {
            Debug.Log("�f���A���̏���"); 
            GameEndServerRpc();
            GameEndClientRpc();
        }
        else
        {
            Debug.Log("�V�~���[�̏���");
            GameEndServerRpc();
            GameEndClientRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void GameEndServerRpc()
    {

        SceneManager.LoadScene("TitleScene");
        NetworkManager.Singleton.StopAllCoroutines();
        NetworkManager.Singleton.Shutdown(false);
        
    }
    [ClientRpc]
    void GameEndClientRpc()
    {
        //if (IsHost) return;
        SceneManager.LoadScene("TitleScene");
    }

    void Init()
    {
        if (IsServer)
        {
            switch (_gameState.Value)
            {
                case GameState.Morning:
                    _gameTime.Value = _morningTime;
                    InitMorningClientRpc();
                    break;
                case GameState.Daytime:
                    _gameTime.Value = _dayTime;
                    InitDaytimeClientRpc();
                    break;
                case GameState.Evening:
                    _gameTime.Value = _eveningTime;
                    InitEveningClientRpc();
                    break;
                case GameState.Night:
                    _gameTime.Value = _nightTime;
                    InitNightClientRpc();
                    break;
            }
        }
    }
    [ClientRpc]
    void InitMorningClientRpc()
    {
        Debug.Log("<color=blue>�������܂����B</color>");
        if (IsServer)
        {
            _gameTime.Value = _morningTime;
            if (isRaid)
            {
                pm.playerList[_raidPlayerID.Value].GetComponent<PlayerController>().PlayerKilledServerRpc(UserLoginData.userName.Value,pm.playerList[_raidPlayerID.Value].GetComponent<PlayerController>()._name.Value);
            }
        }
        
        _nightPanel.SetActive(false);
        _meetingPanel.SetActive(true);
        NightCamera.SetActive(false);
        _voteController.ResetVoteImages();
    }

    [ClientRpc]
    void InitDaytimeClientRpc()
    {
        Debug.Log("<color=blue>�������܂����B</color>");
        if (IsServer)
        {
            _gameTime.Value = _dayTime;
        }
        _dayPanel.SetActive(true);
        _meetingPanel.SetActive(false);
    }
    [ClientRpc]
    void InitEveningClientRpc()
    {
        Debug.Log("<color=blue>�[�������܂����B</color>");
        if (IsServer)
        {
            _gameTime.Value = _eveningTime;
        }
        _dayPanel.SetActive(false);
        _meetingPanel.SetActive(true);
        _votePanel.SetActive(true);
        _voteController.ResetVoteImages();
    }
    [ClientRpc]
    void InitNightClientRpc()
    {
        Debug.Log("<color=blue>�邪���܂����B</color>");
        if (IsServer)
        {
            _gameTime.Value = _nightTime;
        }
        _votePanel.SetActive(false);
        _meetingPanel.SetActive(false);
        _nightPanel.SetActive(true);
        NightCamera.SetActive(true);
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
            _voteController. VoteSubmit();
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
    public void RaidPlayer(int id)
    {
        if (IsServer)
        {
            _raidPlayerID.Value = id;
            isRaid = true;
        }
    }
    public void SkipMeet()
    {
        _gameTime.Value = 1f;
    }
}
