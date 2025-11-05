using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Defines;

public class UI_Title : UI_Base
{
    public RectTransform targetCanvas;

    Animator startAnim;
    Animator logoAnim;

    enum ButtonType
    {
        StartButton,
    }
    enum ImageType
    {
        Image,
    }
    private void Start() => Init();
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(ButtonType));
        BindImage(typeof(ImageType));

        Button button = GetButton(ButtonType.StartButton);
        if (!button)
            return false;
        BindEvent(button.gameObject, OnButtonClicked);

        startAnim = button.GetComponent<Animator>();

        Image logo = GetImage(ImageType.Image);
        if (!logo)
            return false;
        logoAnim = logo.GetComponent<Animator>();
        return true;
    }
    void OnButtonClicked()
    {
        logoAnim.SetBool("bGameStart", true);
        startAnim.SetBool("bStart", true);
        Managers.Scene.ChangeScene(Defines.Scene.InGame);

        GameObject playerGo = GameObject.Find(strPlayerObject);
        PlayerController playerController = playerGo.GetComponent<PlayerController>();
        playerController.AnimState = AnimState.Run;

        GameObject storyUI = GameObject.Find("UI_Story");
        GameObject exitUI = GameObject.Find("UI_Exit");
        if (storyUI) Destroy(storyUI);
        if (exitUI) Destroy(exitUI);

        Setting_InGameUI();

        Destroy(this.gameObject, 3f);
    }

    void Setting_InGameUI()
    {
        GameObject playerGo = GameObject.Find(strPlayerObject);
        Managers.UIManager.ShowUI<UI_Slide>("UI_EX").RegisterInfo(UI_Slide.SlideTargetType.ExpBar);
        Managers.UIManager.ShowUI<UI_Folder>("UI_Folder");
        Managers.UIManager.ShowUI<UI_Timer>("UI_Timer");
    }
}
