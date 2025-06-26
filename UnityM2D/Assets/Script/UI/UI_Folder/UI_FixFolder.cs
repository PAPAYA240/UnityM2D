using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Defines;

public class UI_FixFolder : UI_Base
{
    #region 변수
    enum Buttons
    {
        Upgrade_Icon,
    }

    enum Texts
    {
        Weapon_Name,
        Cnt_Text,
    }
    enum Images
    {
        Icon,
    }

    FixType myFixType = FixType.None_Fix;
    PlayerController Player = null;

    #endregion

    private void Start() => Init();
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (!InitBind())
            Debug.Log("Failed Bind : UI_FixFolder");

        return true;
    }

    void OnUpgradeButtonClick()
    {
        if (Player == null)
            return;

       switch(myFixType)
        {
            case FixType.Heal_Fix:
                break;

            case FixType.Bomber_Fix:
                Player.UseSkill(FixType.Bomber_Fix);
                break;

            default:
                break;
        }
    }

    public void SetInfo(FixType _type)
    {
        myFixType = _type;
        GameObject playerObj = GameObject.Find(strPlayerObject);
        if(playerObj != null)
            Player = playerObj.GetComponent<PlayerController>();
    }

    #region Initialize
    private bool InitBind()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        Button upgradeButton = GetButton(Buttons.Upgrade_Icon);
        if (upgradeButton == null)
            return false;
        BindEvent(upgradeButton.gameObject, OnUpgradeButtonClick);

        return true;
    }
    #endregion

}
