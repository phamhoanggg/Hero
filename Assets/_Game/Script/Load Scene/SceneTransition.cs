using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] Image transitionBgImage;

    private void OnValidate()
    {

    }

    public void Close(Action onClosed = null)
    {
        transitionBgImage.gameObject.SetActive(true);
        transitionBgImage.DOFade(1, 1f).OnComplete(() => onClosed.Invoke());
    }

    public void Open(Action onOpened = null)
    {
        transitionBgImage.DOFade(0, 1f).OnComplete(() =>
        {
            transitionBgImage.gameObject.SetActive(false);
            onOpened?.Invoke();
            if (LoadSceneManager.Ins.CurrentSceneId == SceneId.Game)
            {
                CoregameManager.Ins.Play();
            }
        });
    }
}
