using UnityEngine;
using UnityEngine.UI;

public class UI_UltimateButton : UI_Base
{
    enum Images
    {
        ProjectCoolTime,
    }
    enum Buttons
    {
        Ultimate_Button,
        Airplane_Button,
        PetUltimate_Button,
    }

    float LastProjectCoolTime = 20f;
    float LastProjectTime = 0.0f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (!BindButton())
            Debug.Log("Failed Bind : UI_UltimateButton()");

        return true;
    }

    private void Update()
    {
        float ratio = GetProjectWaitRatio();
        GetImage(Images.ProjectCoolTime).fillAmount = 1.0f - ratio;
    }

    float GetProjectWaitRatio()
    {
        float playTime = Managers.PlayTime;
        float projectTime = LastProjectTime;

        float ratio = 1.0f;
        if (projectTime > 0 && projectTime < playTime)
            ratio = (playTime - projectTime) / LastProjectCoolTime;

        return ratio;
    }

    private void OnClickUltimateButton()
    {
        LastProjectTime = Time.deltaTime;
    }

    private void OnClickAirplaneButton()
    {
        LastProjectTime = Time.deltaTime;
    }

    private void OnClickPetButton()
    {
        LastProjectTime = Time.deltaTime;
    }

    #region Initialize
    private bool BindButton()
    {
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        Button UltimateButton = GetButton(Buttons.Ultimate_Button);
        if (UltimateButton != null)
            BindEvent(UltimateButton.gameObject, OnClickUltimateButton);
        
        Button AirplaneButton = GetButton(Buttons.Airplane_Button);
        if (AirplaneButton != null)
            BindEvent(AirplaneButton.gameObject, OnClickAirplaneButton);

        Button PetUltimateButton = GetButton(Buttons.PetUltimate_Button);
        if (PetUltimateButton != null)
            BindEvent(PetUltimateButton.gameObject, OnClickPetButton);

        GetImage(Images.ProjectCoolTime).fillAmount = 0;
        return true;
    }
    #endregion
}
