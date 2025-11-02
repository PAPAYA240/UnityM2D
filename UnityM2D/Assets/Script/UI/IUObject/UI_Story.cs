using UnityEngine;

public class UI_Story : UI_Base
{
    enum ButtonType
    {
        Button_StoryOpen,
    }

    enum ImageType
    {
        Image_Story,
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(ButtonType));
        BindImage(typeof(ImageType));
        GetButton(ButtonType.Button_StoryOpen).gameObject.BindEvent(OnStoryButtonClick, Defines.Input.Click);

        return true;
    }

    void OnStoryButtonClick()
    {
        GetImage(ImageType.Image_Story)?.gameObject.SetActive(true);
    }
    void Update()
    {
        
    }
}
