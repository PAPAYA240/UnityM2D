using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Defines;

public class UI_Dead : UI_Base
{
    enum TextType
    {
        Text_UserName,
        Text_Level,
    }
    enum ButtonType
    {
        ContinueButton,
    }

    private void Start() => Init();
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(ButtonType));
        BindText(typeof(TextType));

        GameObject button = GetButton(ButtonType.ContinueButton).gameObject;
        BindEvent(button, () => ContinueButton());

        GameObject PlayerObject = GameObject.Find(strPlayerObject);
        PlayerController Player = PlayerObject.GetComponent<PlayerController>();
        GetText(TextType.Text_Level).text = String.Format($"LEVEL {Player?.data.Level} CLEAR!");
        return true;
    }

    private void ContinueButton()
    {
        // 게임 리셋
        SceneManager.LoadScene(GetSceneName(Defines.Scene.None));
    }
    string GetSceneName(Defines.Scene type)
    {
        string name = System.Enum.GetName(typeof(Defines.Scene), type);
        char[] letters = name.ToLower().ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new string(letters);
    }
}
