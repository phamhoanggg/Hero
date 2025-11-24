using DG.Tweening;
using UnityEngine;

public class Shield : RewindableObject
{
    [SerializeField] Transform shieldRoot;
    [SerializeField] Transform shieldTf;

    Sequence seq;
    bool tweenCompleted;
    public void GetShield(ShieldDirect direct)
    {
        seq = DOTween.Sequence();
        seq.timeScale = 1;
        tweenCompleted = false;
        shieldTf.localScale = Vector3.zero;
        seq.Append(shieldTf.DOScale(1, 0.25f));

        if (direct == ShieldDirect.Horizontal)
        {
        }
        else if (direct == ShieldDirect.Vertical)
        {
            seq.Join(shieldRoot.DORotate(Vector3.forward * 90, 0.25f));
        }

        seq.SetAutoKill(false).Play().OnComplete(() =>
        {
            tweenCompleted = true;
            CoregameManager.Ins.listRewindEvent.Add(new("Shield On", () =>
            {
                seq.timeScale = 1 / CoregameManager.Ins.reverseRatio;
                seq.PlayBackwards();
            }));
        });
        if (CoregameManager.Ins.IsReversing) return;
        PlayerMove.Ins.Invoke(nameof(PlayerMove.Ins.ContinueMove), 0.3f);
    }

    public override void DelegateRewind()
    {
        if (seq == null || tweenCompleted) return;
        seq.Pause();
        seq.timeScale = 1 / CoregameManager.Ins.reverseRatio;
        seq.PlayBackwards();
    }
}
