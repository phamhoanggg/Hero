using System;
using DG.Tweening;
using UnityEngine;

public class TweenUtil
{
    public static Tween RewindableTween(Tween tween, Action onRewinded, Action onCompleted = null)
    {
        tween.SetAutoKill(false);
        tween.OnRewind(() => onRewinded.Invoke());
        tween.SetEase(Ease.Linear);
        tween.SetUpdate(UpdateType.Fixed);
        if (onCompleted != null) tween.OnComplete(() => onCompleted.Invoke());
        return tween;
    }
}
