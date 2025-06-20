using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Defines;

public class UI_Title : UI_Base
{
    public RectTransform targetCanvas;

    [SerializeField]
    Image Logo;
    [SerializeField]
    Button button;

    Animator startAnim;
    Animator logoAnim;

    void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(OnButtonClicked);

        startAnim = button.GetComponent<Animator>();
        logoAnim = Logo.GetComponent<Animator>();


    }

    void OnButtonClicked()
    {
        logoAnim.SetBool("bGameStart", true);
        startAnim.SetBool("bStart", true);
        Managers.Scene.ChangeScene(Defines.Scene.InGame);

        GameObject playerGo = GameObject.Find("Player");
        PlayerController playerController = playerGo.GetComponent<PlayerController>();
        playerController.PlayerAnim = AnimState.Run;

        Setting_InGameUI();

        Destroy(this.gameObject, 3f);
    }

    void Setting_InGameUI()
    {
        GameObject playerGo = GameObject.Find("Player");
        Managers.UIManager.ShowUI<UI_Slide>("UI_EX").RegisterInfo(UI_Slide.SlideTargetType.ExpBar);
        Managers.UIManager.ShowUI<UI_Folder>("UI_Folder");
        Managers.UIManager.ShowUI<UI_Timer>("UI_Timer");
    }
}
