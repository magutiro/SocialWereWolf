using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class ChatController : NetworkBehaviour
{
    public GameObject ChatPrefab;
    PlayerManager playerManager;

    public TMP_InputField inputField;
    public TextMeshProUGUI text;

    public GameObject ChatView;

    public GameObject ChatPanel;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        inputField = inputField.GetComponent<TMP_InputField>();
        text = text.GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnChatButton()
    {
        ChatPanel.SetActive(!ChatPanel.activeSelf);
    }
    public void OnSendButton()
    {
        SendChatMessageServerRpc(UserLoginData.userName.Value, inputField.text);
    }
    [ServerRpc]
    public void SendChatMessageServerRpc(string name, string chatText)
    {
        SendChatClientRpc(name, chatText);
    }
    [ClientRpc]
    public void SendChatClientRpc(string name, string chatText)
    {
        SetChatContents(name, chatText);
    }
    void SetChatContents(string name, string chatText)
    {
        var prefab = Instantiate(ChatPrefab);
        prefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        prefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = chatText;
        prefab.transform.SetParent(ChatView.transform, false);
        //ChatPrefab.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Item/Item" );
    }
}
