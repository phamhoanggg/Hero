using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;


#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor.SceneManagement;
#endif

public class Load : MonoBehaviour
{
    [SerializeField] bool localTime;
    //[SerializeField] TimeFetcher timeFetcher;
    [SerializeField] SkeletonGraphic gameLogoSkeleton;
    [SerializeField] CanvasScaler loadCanvas;
    //[SerializeField] AdsManager adsManager;

    IEnumerator Start()
    {
        float screenRatio = Screen.width * 1f / Screen.height;
        gameLogoSkeleton.AnimationState.SetAnimation(0, "Start", false);

        gameLogoSkeleton.AnimationState.SetAnimation(1, "Idle", true);
        // Fake load
        Tween tween = DOVirtual.Float(0, 0.8f, 3, progress =>
        {
            SetProgress(progress);
        });
        DataManager.Ins.LoadData();
        //AudioManager.Ins.Init();
        VibrationManager.Init();
        // Load scene
        yield return new WaitUntil(() => !tween.active);
        SceneId scene = SceneId.Game;
        SetProgress(1);
        LoadSceneManager.Ins.LoadScene(scene, () => { });
    }

    void SetProgress(float progress)
    {
        //fillImage.fillAmount = progress;
    }

#if UNITY_EDITOR
    [Button, HorizontalGroup("row")]
    public void CheatWithoutAds()
    {
        //cheatMode = true;
        localTime = true;
        //adsManager.AdsEnabled = false;
        //EditorUtility.SetDirty(levelManager);
        //EditorUtility.SetDirty(adsManager);
        SaveScene();
    }

    [Button, HorizontalGroup("row")]
    public void CheatWithAds()
    {
        //cheatMode = true;
        localTime = true;
        //adsManager.AdsEnabled = true;
        //EditorUtility.SetDirty(levelManager);
        //EditorUtility.SetDirty(adsManager);
        SaveScene();
    }

    [Button, HorizontalGroup("row")]
    public void SubmitBuild()
    {
        //cheatMode = false;
        localTime = false;
        //adsManager.AdsEnabled = true;
        //EditorUtility.SetDirty(levelManager);
        //EditorUtility.SetDirty(adsManager);
        SaveScene();
    }

    void SaveScene()
    {
        if (Application.isPlaying) return;
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.isDirty)
        {
            EditorSceneManager.SaveScene(activeScene);
        }
    }
#endif
}
