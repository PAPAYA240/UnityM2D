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
    enum ImageType
    {
        Dead_Icon,
    }
    private void Start() => Init();
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Canvas canvas = GetComponent<Canvas>();

        if (canvas != null)
            canvas.sortingOrder = 50;

        BindButton(typeof(ButtonType));
        BindText(typeof(TextType));
        BindImage(typeof(ImageType));

        GameObject button = GetButton(ButtonType.ContinueButton).gameObject;
        BindEvent(button, () => ContinueButton());

        GameObject PlayerObject = GameObject.Find(strPlayerObject);
        PlayerController Player = PlayerObject.GetComponent<PlayerController>();
        GetText(TextType.Text_Level).text = String.Format($"LEVEL {Player?.data.Level} CLEAR!");
        return true;
    }
    float _timer = 0f;
    void Update()
    {
        _timer+= Time.deltaTime;
        if(_timer >= 2.0f)
            GetImage(ImageType.Dead_Icon).gameObject.SetActive(false);
    }
    private void ContinueButton()
    {
        // Manager -> Player/Monster -> UI 순
        // 게임 리셋
        // Manager
        Managers.TimerManager.Clear();
        Managers.TurnManager.Clear();

        Destroy(Managers.UIManager.FindUI<UI_Slide>().gameObject);
        Destroy(Managers.UIManager.FindUI<UI_Folder>().gameObject);
        Destroy(Managers.UIManager.FindUI<UI_Timer>().gameObject);

        // Enemy
        GameObject PlayerObject = GameObject.Find(strPlayerObject);
        PlayerController Player = PlayerObject.GetComponent<PlayerController>();
        Destroy(Player.TargetObject);

        GameObject enemy = Managers.Resource.Instantiate(strEnemyPath);
        enemy.name = strEnemyObject;

        // Player
        Destroy(Player.gameObject);
        GameObject player = Managers.Resource.Instantiate(strPlayerPath);
        player.name = strPlayerObject;
        PlayerController newPlayer = PlayerObject.GetComponent<PlayerController>();
        newPlayer.TargetObject = enemy;

        // UI
        GameObject uiStart = Managers.Resource.Instantiate($"UI/Start_UI");
        uiStart.name = "Start_UI";
        GameObject uiStory = Managers.Resource.Instantiate($"UI/UI_Story");
        uiStory.name = "UI_Story";
        GameObject uiExit = Managers.Resource.Instantiate($"UI/UI_Exit");
        uiStory.name = "UI_Exit";

        Destroy(gameObject);
        Managers.Scene.ChangeScene(Defines.Scene.None);
        //SceneManager.LoadScene(GetSceneName(Defines.Scene.None));
    }
    string GetSceneName(Defines.Scene type)
    {
        string name = System.Enum.GetName(typeof(Defines.Scene), type);
        char[] letters = name.ToLower().ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new string(letters);
    }
}
