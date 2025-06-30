using GoogleMobileAds.Api;
using System;
using UnityEngine;
using System.Threading.Tasks;

public class AdsManager
{
    public enum AdsStateType
    {
        None,
        Failed,
        Success
    }

    bool TEST_MODE = true;

    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    private Action _rewardedCallback; // This will now be primarily used to store the user's custom reward logic

    private string _interstitialAdUnitId;
    private string _rewardedAdUnitId;

    const string TEST_APP_ID = "ca-app-pub-3940256099942544~3347511713";
    const string TEST_ANDROID_INTERSTITIAL = "ca-app-pub-3940256099942544/1033173712";
    const string TEST_ANDROID_REWARDED = "ca-app-pub-3940256099942544/5224354917";
    const string TEST_IOS_INTERSTITIAL = "ca-app-pub-3940256099942544/4411468910";
    const string TEST_IOS_REWARDED = "ca-app-pub-3940256099942544/1712485313";

    private const string ANDROID_INTERSTITIAL_ID = "ca-app-pub-4385483914896399/1780279477";
    private const string ANDROID_REWARDED_ID = "YOUR_ANDROID_REWARDED_ID";
    private const string IOS_INTERSTITIAL_ID = "YOUR_IOS_INTERSTITIAL_ID";
    private const string IOS_REWARDED_ID = "YOUR_IOS_REWARDED_ID";

    public void Init()
    {
#if UNITY_ANDROID
        _interstitialAdUnitId = TEST_MODE ? TEST_ANDROID_INTERSTITIAL : ANDROID_INTERSTITIAL_ID;
        _rewardedAdUnitId = TEST_MODE ? TEST_ANDROID_REWARDED : ANDROID_REWARDED_ID;
#elif UNITY_IOS
        _interstitialAdUnitId = TEST_MODE ? TEST_IOS_INTERSTITIAL : IOS_INTERSTITIAL_ID;
        _rewardedAdUnitId = TEST_MODE ? TEST_IOS_REWARDED : IOS_REWARDED_ID;
#else
        _interstitialAdUnitId = TEST_ANDROID_INTERSTITIAL;
        _rewardedAdUnitId = TEST_ANDROID_REWARDED;
#endif

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK initialized.");
            PrepareAds();
        });
    }

    public void PrepareAds()
    {
        LoadInterstitialAd();
        LoadRewardedAd();
    }

    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log($"Attempting to load interstitial ad with ID: {_interstitialAdUnitId}");
        var request = new AdRequest();

        InterstitialAd.Load(_interstitialAdUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"Interstitial ad failed to load: {error?.GetMessage()}");
                return;
            }

            Debug.Log("Interstitial ad loaded successfully.");
            _interstitialAd = ad;

            _interstitialAd.OnAdFullScreenContentOpened += HandleOnAdOpened;
            _interstitialAd.OnAdFullScreenContentClosed += HandleOnAdClosed;
            _interstitialAd.OnAdFullScreenContentFailed += HandleOnAdFailedToLoad;
        });
    }

    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log($"Attempting to load rewarded ad with ID: {_rewardedAdUnitId}");
        var request = new AdRequest();

        RewardedAd.Load(_rewardedAdUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"Rewarded ad failed to load: {error?.GetMessage()}");
                return;
            }

            Debug.Log("Rewarded ad loaded successfully.");
            _rewardedAd = ad;

            _rewardedAd.OnAdFullScreenContentOpened += HandleOnAdOpened;
            _rewardedAd.OnAdFullScreenContentClosed += HandleOnAdClosed;
            _rewardedAd.OnAdFullScreenContentFailed += HandleOnAdFailedToLoad;
        });
    }

    public void HandleOnAdFailedToLoad(AdError adError)
    {
        Debug.LogError($"Ad FullScreen Content Failed: {adError.GetMessage()}");
        PrepareAds();
    }

    public void HandleOnAdOpened()
    {
        Debug.Log("Ad FullScreen Content Opened");
    }

    public void HandleOnAdClosed()
    {
        Debug.Log("Ad FullScreen Content Closed");
        PrepareAds();
    }

    public void HandleUserEarnedReward(Reward reward)
    {
        Debug.Log($"User Earned Reward (from internal callback): Type={reward.Type}, Amount={reward.Amount}");
    }

    public void ShowInterstitialAds()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready. Attempting to load a new one.");
            LoadInterstitialAd();
        }
    }

    public void ShowRewardedAds(Action rewardedCallback)
    {
        _rewardedCallback = rewardedCallback;

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"User Earned Reward (from Show() callback): Type={reward.Type}, Amount={reward.Amount}");
                _rewardedCallback?.Invoke();
                _rewardedCallback = null;
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready. Attempting to load a new one.");
            LoadRewardedAd();
        }
    }
}