using System;
using DG.Tweening;
using UnityEngine;

public class TweenUtil
{
    public static Tween RewindableTween(Tween tween, Action onRewinded)
    {
        tween.SetAutoKill(false);
        tween.OnRewind(() => onRewinded.Invoke());

        return tween;
    }
}
