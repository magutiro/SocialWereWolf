using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public enum SkillPost
{
    Similer,
    Dual
}
public interface SkillBase
{
    public void Effect1();
    public void Effect2();
}

public class PlayerSkillController : NetworkBehaviour
{
    public int SkillID = 1;

    SkillBase skill;

    float coolTime = 0;
    public string skillName = "";

    Button SkillButton;
    public void Start()
    {
        SceneManager.sceneLoaded += SceneUnloaded;
        switch (SkillID)
        {
            case 1:
                gameObject.AddComponent<PlayerSkillHealer>();
                skill = GetComponent<PlayerSkillHealer>();
                break;
            case 2:
                gameObject.AddComponent<PlayerSkillFortuner>(); 
                skill = GetComponent<PlayerSkillFortuner>();
                break;
        }

    }
    void SceneUnloaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "InGameScene" || !IsOwner)
        {
            return;
        }
        SkillButton = GameObject.Find("SkillButton").GetComponent<Button>();
        SkillButton.onClick.AddListener(() => OnSkillUse());
    }
    public void OnSkillUse()
    {
        switch (SkillID)
        {
            case 1:
                skill.Effect1();
                break;
            case 2:
                skill.Effect1();
                break;
        }
    }
}
