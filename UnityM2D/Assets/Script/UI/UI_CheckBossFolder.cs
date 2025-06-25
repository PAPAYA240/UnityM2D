using System;
using UnityEngine;
using UnityEngine.UI;
using static Defines;

// 변수명 변경 완료 

public class UI_CheckBossFolder : UI_Base
{
    private RuntimeAnimatorController pendingLoadAnim;

    Action _onClickYesButton;

    enum Buttons
    {
        No_Button,
        Yes_Button
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        // 취소 버튼

        BindButton(typeof(Buttons));

        BindEvent(GetButton(Buttons.No_Button).gameObject, CancelButton);
        BindEvent(GetButton(Buttons.Yes_Button).gameObject, AcceptanceButten);

        return true;
    }
    private void CancelButton()
    {
         gameObject.SetActive(false);
    }

    public void AcceptanceButten()
    {
        gameObject.SetActive(false);

        _onClickYesButton.Invoke();
    }

    // Boss 창 열 때 정보 넘기기
    public void ActiveCheckBossFolder(Action _invoke, bool _active = true)
    {
        _onClickYesButton = _invoke;
        this.gameObject.SetActive(_active);
    }
}
