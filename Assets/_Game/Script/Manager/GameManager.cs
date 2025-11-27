using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField]
    private GameConfig config;

    private DateTime startupTime;
    public DateTime Now => startupTime + TimeSpan.FromSeconds(Time.realtimeSinceStartup);
    public GameConfig Config => config;
    public bool IsFirstAppOpen;
    private void OnEnable()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        SetStartupTime(DateTime.Now);
        //DataManager.Ins.LoadData();
    }

    public void SetStartupTime(DateTime startupTime)
    {
        this.startupTime = startupTime;
    }
}

[Serializable]
public class GameConfig
{
    Data gameData => DataManager.Ins.Data;
    public int backHomeAfterCompleteLevel;

    //[Header("ADS")]
    //public bool isFirstOfferRemoveAds;
    //[Tooltip("Enable interstitial from level: ")]
    //public int interstitialEnableLevel = 11;
    //public int interstitialCapping = 120;
    //public int interstitialRewardedCapping = 40;
    //public bool isShowAOAOnSwitch = true;
    //[Tooltip(
    //    "0: Disable\n" +
    //    "1: First app open\n" +
    //    "2: Every app open\n" +
    //    "3: Every app open except first app open")]
    //public int showAOAOnOpen = 3;
    //public bool isShowBannerHome;

    public bool CanBackHome()
    {
        return gameData.LevelIndex >= backHomeAfterCompleteLevel;
    }
}


