using System;
using DG.Tweening;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] RectTransform m_RectTransform;
    [SerializeField] float animTime;
    Tween putInLockTween;
    public void PlayPutInLockAnim(Action onComplete = null)
    {
        putInLockTween = m_RectTransform.DORotate(Vector3.forward * -60, 0.2f).OnComplete(() =>
        {
            m_RectTransform.DOAnchorPos(m_RectTransform.anchoredPosition + new Vector2(15, -15), 0.2f);
            m_RectTransform.DOScale(0, 0.2f).OnComplete(() => onComplete?.Invoke());
        });
    }
}
