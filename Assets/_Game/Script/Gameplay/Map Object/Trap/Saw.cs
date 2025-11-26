using System;
using System.Collections.Generic;
using DG.Tweening;
using SharedModules.ED;
using UnityEngine;

public class Saw : MonoBehaviour
{
    [SerializeField] List<Route> listRoute = new();
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float speed;
    int routeIndex;
    int checkpointIndex;
    List<Tween> tweenMoveStack = new();
    int reverseIndex;
    float lastMoveTIme;
    Route route => listRoute[routeIndex];
    bool isReversing;
    private void OnEnable()
    {
        EventDispatcher.RegisterListener(EventId.OnRewind, StartReverse);
        EventDispatcher.RegisterListener(EventId.OnStartMove, StartMove);
    }

    private void OnDisable()
    {
        EventDispatcher.UnregisterListener(EventId.OnRewind, StartReverse);
        EventDispatcher.UnregisterListener(EventId.OnStartMove, StartMove);
    }
    public void StartMove(object arg = null)
    {
        routeIndex = CoregameManager.Ins.currentLevel.zones[0].ZoneOption;
        checkpointIndex = 1;
        isReversing = false;
        tweenMoveStack.Clear();
        rectTransform.DORotate(Vector3.forward * 360, 10, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        Move(checkpointIndex);
    }

    void Move(int idx)
    {
        float dis = Vector2.Distance(rectTransform.position, route.checkPoints[idx].position);
        float moveTime = dis / speed;
        Tween tween = TweenUtil.RewindableTween(rectTransform.DOMove(route.checkPoints[idx].position, moveTime),
            ReverseStepCompleted, MoveCompleted);
        tweenMoveStack.Add(tween);
        //rectTransform.DOMove(route.checkPoints[idx].position, moveTime);
    }

    public void MoveCompleted()
    {
        lastMoveTIme = Time.fixedDeltaTime;
        if (checkpointIndex + 1 < route.checkPoints.Count)
        {
            checkpointIndex++;
            Move(checkpointIndex);
        }
        else
        {
            CoregameManager.Ins.listRewindEvent.Add(new("", () => StartReverse()));
        }
    }

    public void StartReverse(object arg = null)
    {
        if (isReversing) return;
        isReversing = true;
        rectTransform.DOPause();
        float reverseScale = CoregameManager.Ins.reverseRatio;
        foreach (var tween in tweenMoveStack)
            tween.timeScale = reverseScale;

        Debug.Log("Start Reverse: " + Time.fixedTime);

        reverseIndex = tweenMoveStack.Count - 1;
        tweenMoveStack[reverseIndex].PlayBackwards();
    }

    public void ReverseStepCompleted()
    {
        //tweenMoveStack[reverseIndex].Kill();
        reverseIndex--;
        if (reverseIndex >= 0) tweenMoveStack[reverseIndex].PlayBackwards();
        else
        {
            ReverseCompleted();
        }
    }

    public void ReverseCompleted()
    {
        isReversing = false;
    }
}

[Serializable]
public class Route
{
    public List<Transform> checkPoints;
}
