using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UI_Exit : UI_Base
{
    enum ButtonType
    {
        Button_StoryOpen,
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(ButtonType));
        GetButton(ButtonType.Button_StoryOpen).gameObject.BindEvent(OnStoryButtonClick, Defines.Input.Click);

        return true;
    }

    void OnStoryButtonClick()
    {
        Application.Quit();
    }
    void Update()
    {

    }
}
