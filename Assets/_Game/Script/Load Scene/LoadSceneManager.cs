using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : SingletonMonoBehaviour<LoadSceneManager>
{
    [SerializeField, ReadOnly] SceneId currentSceneId = SceneId.Load;
    [SerializeField, ReadOnly] SceneId loadingSceneId = SceneId.None;
    [SerializeField] SceneTransition sceneTransition;
    public SceneId CurrentSceneId => currentSceneId;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (loadingSceneId == SceneId.None) return;
        currentSceneId = loadingSceneId;
    }

    void CleanUpScene()
    {
        DOTween.KillAll();
        //EventDispatcher.ClearAllListeners();
    }

    #region LOAD SCENE
    [Button(ButtonStyle.FoldoutButton)]
    public void LoadScene(SceneId sceneId)
    {
        loadingSceneId = sceneId;
        StartCoroutine(LoadSceneCoroutine(sceneId));
    }

    IEnumerator LoadSceneCoroutine(SceneId sceneId)
    {
        CleanUpScene();
        sceneTransition.Close(() =>
        {
            SceneManager.LoadSceneAsync((int)sceneId);
        });
        yield return new WaitForSeconds(1f);
        sceneTransition.Open();
    }

    public void LoadScene(SceneId sceneId, Action onLoading, Action onComplete = null)
    {
        loadingSceneId = sceneId;
        StartCoroutine(LoadSceneCoroutine(sceneId, onLoading, onComplete));
    }

    IEnumerator LoadSceneCoroutine(SceneId sceneId, Action onLoading, Action onComplete)
    {
        CleanUpScene();
        bool closeDone = false;
        sceneTransition.Close(() => closeDone = true);
        yield return new WaitUntil(() => closeDone);
        AsyncOperation operation = SceneManager.LoadSceneAsync((int)sceneId);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            onLoading?.Invoke();
            yield return null;
            if (operation.progress >= 0.9f && !operation.allowSceneActivation)
            {
                yield return null;
                operation.allowSceneActivation = true;
            }
        }
        yield return new WaitUntil(() => operation.isDone);
        yield return new WaitForSeconds(1.5f);

        sceneTransition.Open(onComplete);
    }
    #endregion
}
