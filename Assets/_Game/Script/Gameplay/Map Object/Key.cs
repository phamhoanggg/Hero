using System;
using DG.Tweening;
using UnityEngine;

public class Key : RewindableObject
{
    [SerializeField] RectTransform m_RectTransform;
    [SerializeField] float animTime;
    Tween putInLockTween;
    public void OnCollected()
    {
        rootParent = m_RectTransform.parent;
        m_RectTransform.SetParent(PlayerMove.Ins.PlayerTf);
        CoregameManager.Ins.listRewindEvent.Add(new("Key Collect", () =>
        {
            m_RectTransform.SetParent(rootParent);
            m_RectTransform.anchoredPosition = rootPosition;
        }));
    }
    public void PlayPutInLockAnim(Action onComplete = null)
    {
        Vector2 pre_unlock_pos = m_RectTransform.anchoredPosition;
        putInLockTween = m_RectTransform.DORotate(Vector3.forward * -60, 0.2f).OnComplete(() =>
        {
            pre_unlock_pos = m_RectTransform.anchoredPosition;
            m_RectTransform.DOAnchorPos(m_RectTransform.anchoredPosition + new Vector2(15, -15), 0.2f);
            m_RectTransform.DOScale(0, 0.2f).OnComplete(() => onComplete?.Invoke());
        });

        CoregameManager.Ins.listRewindEvent.Add(new("Key rotate", () =>
        {
            m_RectTransform.DOPause();

            m_RectTransform.DOAnchorPos(pre_unlock_pos, 0.2f / CoregameManager.Ins.reverseRatio);
            m_RectTransform.DOScale(1, 0.2f / CoregameManager.Ins.reverseRatio).OnComplete(() =>
            {
                m_RectTransform.DORotate(rootEuler, 0.2f / CoregameManager.Ins.reverseRatio);
            });
        }));
    }
}
