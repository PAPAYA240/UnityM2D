using System;
using UnityEngine;
using static Defines;

public class UI_AdsFolder : UI_Base
{
    public enum RewardType
    {
        Reward_Speed,
        Reward_Hill,
        Reward_Coin,
    }

    enum Buttons
    {
        Button,
    }
    enum Images
    {
        ProjectCoolTime,
    }
    
    private RewardType rewardType = RewardType.Reward_Speed;
    private PlayerController playerController = null;
    private int rewardMoney = 500;

    private void Start() =>Init();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        GameObject button = GetButton(Buttons.Button).gameObject;
        if (button == null)
            return false;
        BindEvent(button, OnShowAdsClick);

        GameObject PlayerObject = GameObject.Find(strPlayerObject);
        playerController =PlayerObject.GetComponent<PlayerController>();
        if (playerController == null)
            return false;

        return true;
    }

    private void Update()
    {
        float ratio = GetProjectWaitRatio();
        GetImage(Images.ProjectCoolTime).fillAmount = 1.0f - ratio;
    }
    void OnShowAdsClick()
    {
        Managers.Ads.ShowRewardedAds(() => { GiveReward(); });
    }
    void GiveReward()
    {
        if (playerController == null)
            return;

        switch (rewardType)
        {
            case RewardType.Reward_Speed:
                break;
            case RewardType.Reward_Hill:
                playerController.data.Hp = playerController.data.MaxHp;
                break;
            case RewardType.Reward_Coin:
                playerController.data.Money += rewardMoney;
                break;
        }
        LastProjectTime = Managers.PlayTime;
    }

    public void SetInfo(RewardType _type)
    {
        rewardType = _type;
    }
}
