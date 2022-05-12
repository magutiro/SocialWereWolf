using UnityEngine;
using System.IO;
using JetBrains.Annotations;


    // �ڑ����ۑ����邽�߂̐ݒ�
    [System.Serializable]
    public class ConnectInfo
    {
        //�ڑ���IP�A�h���X
        [SerializeField]
        public string ipAddr;
        // �|�[�g�ԍ�
        [SerializeField]
        public int port;
        //�����[�T�[�o�[�̗L��
        [SerializeField]
        public bool useRelay;

        // �����[�T�[�o�[��IP�A�h���X
        [SerializeField]
        public string relayIpAddr;
        // �����[�T�[�o�[�̃|�[�g�ԍ�
        [SerializeField]
        public int relayPort;

        // �v���C���[��
        // ���ڑ����Ƃ͊֌W�Ȃ��̂ł����A���߂Ė��O���炢�͂Ǝv���c
        [SerializeField]
        public string playerName;



        public static ConnectInfo GetDefault()
        {
            ConnectInfo info = new ConnectInfo();
            info.useRelay = false;
            info.ipAddr = "127.0.0.1";
            info.port = 7777;
            info.playerName = "�f�t�H���g�l�[��";

            info.relayIpAddr = "184.72.104.138";
            info.relayPort = 8888;
            return info;
        }

        private static string ConfigFile
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return "connectInfo.json";
#else
            return Path.Combine(Application.persistentDataPath, "connectInfo.json");
#endif
            }
        }

        public static ConnectInfo LoadFromFile()
        {
            var configFilePath = ConfigFile;
            if (!File.Exists(configFilePath))
            {
                return GetDefault();
            }
            string jsonStr = File.ReadAllText(configFilePath);
            var connectInfo = JsonUtility.FromJson<ConnectInfo>(jsonStr);
            return connectInfo;
        }

        public void SaveToFile()
        {
            string jsonStr = JsonUtility.ToJson(this);
            File.WriteAllText(ConfigFile, jsonStr);
        }
    }
