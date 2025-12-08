using System;
using DG.Tweening;
using UnityEngine;

public class Key : RewindableObject
{
    [SerializeField] RectTransform m_RectTransform;
    [SerializeField] Collider2D col2D;

    public void OnRelease()
    {
        Transform parent = m_RectTransform.parent;
        m_RectTransform.SetParent(CoregameManager.Ins.currentLevel.transform, true);
        col2D.enabled = true;
        CoregameManager.Ins.listRewindEvent.Add(new("Key Collect", () =>
        {
            m_RectTransform.SetParent(parent, true);
            col2D.enabled = false;
        }));
    }
    public void OnCollected()
    {
        rootParent = m_RectTransform.parent;
        m_RectTransform.SetParent(PlayerMove.Ins.PlayerTf);
        float originRotate = m_RectTransform.eulerAngles.z;
        m_RectTransform.DOLocalRotate(Vector3.zero, 0.25f);

        CoregameManager.Ins.listRewindEvent.Add(new("Key Collect", () =>
        {
            m_RectTransform.SetParent(rootParent);
            m_RectTransform.anchoredPosition = rootPosition;
            m_RectTransform.DOLocalRotate(Vector3.forward * originRotate, 0.25f);
        }));
    }
    public void PlayPutInLockAnim(Action onComplete = null)
    {
        Vector2 pre_unlock_pos = m_RectTransform.anchoredPosition;
        m_RectTransform.DORotate(Vector3.forward * -60, 0.2f).OnComplete(() =>
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
