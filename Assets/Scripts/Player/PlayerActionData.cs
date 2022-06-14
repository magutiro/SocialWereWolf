using Newtonsoft.Json;
using System.Collections.Generic;

public class PlayerActionData
{
    [JsonProperty("action")]
    public string action;

    [JsonProperty("room_no")]
    public int? room_no;

    [JsonProperty("user")]
    public string user;

    [JsonProperty("pos_x")]
    public float pos_x;

    [JsonProperty("pos_y")]
    public float pos_y;

    [JsonProperty("pos_z")]
    public float pos_z;

    [JsonProperty("way")]
    public string way;

    [JsonProperty("range")]
    public float range;

    /// <summary>
    /// �N���C�A���g����T�[�o�֑��M����f�[�^��JSON�`���ɕϊ�
    /// </summary>
    /// <returns></returns>
    public string ToJson()
    {
        // �I�u�W�F�N�g��json�ɕϊ�
        return JsonConvert.SerializeObject(this, Formatting.None);
    }

    /// <summary>
    /// �T�[�o�[���瑗�M���Ă���JSON�f�[�^��z��f�[�^�ɕϊ�
    /// </summary>
    /// <param name="json"></param>
    /// <param name="roomNo"></param>
    /// <returns></returns>
    public static Dictionary<string, PlayerActionData> FromJson(string json, int roomNo)
    {
        // json������𑽊K�w��Dictionary�ɕϊ�
        var jsonHash = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(json);

        // �߂�l��Dictionary�̏�����
        var playerActionHash = new Dictionary<string, PlayerActionData>();

        // json�̒��ɊY���̃��[���ԍ��̏�񂪂Ȃ���΋��Dictionary��ԋp
        if (!jsonHash.ContainsKey("room" + roomNo))
        {
            return playerActionHash;
        }

        // ���[���̒��Ƀ��[�U��񂪊܂܂�Ă���̂�PlayerActionData�^�ɕϊ�
        var roomPlayerHash = jsonHash["room" + roomNo];
        foreach (var playerHash in roomPlayerHash)
        {
            var PlayerActionData = new PlayerActionData
            {
                user = (string)playerHash.Value["user"],
                pos_x = float.Parse(playerHash.Value["pos_x"].ToString()),
                pos_y = float.Parse(playerHash.Value["pos_y"].ToString()),
                pos_z = float.Parse(playerHash.Value["pos_z"].ToString()),
                way = (string)playerHash.Value["way"],
                range = float.Parse(playerHash.Value["range"].ToString()),
            };
            playerActionHash.Add(PlayerActionData.user, PlayerActionData);
        }

        return playerActionHash;
    }
}