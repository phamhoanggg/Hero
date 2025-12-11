using System;
using System.Collections.Generic;
using DG.Tweening;
using SharedModules.ED;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed = 1;
    [SerializeField] InvertCheckPoint startPoint;
    List<InvertCheckPoint> route;
    int checkpointIndex;
    List<Tween> tweenMoveStack = new();
    int reverseIndex;
    float lastMoveTIme;
    bool isReversing;
    Tween rotateTween;
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
        route = CoregameManager.Ins.GenerateInvertRouteForRotateObject(startPoint);
        checkpointIndex = 0;
        isReversing = false;
        tweenMoveStack.Clear();
        rotateTween = rectTransform.DORotate(Vector3.forward * 360, 1 / rotateSpeed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).SetAutoKill(false);
        Move(checkpointIndex);
    }

    void Move(int idx)
    {
        float dis = Vector2.Distance(rectTransform.position, route[idx].TF.position);
        float moveTime = dis / speed;
        Tween tween = TweenUtil.RewindableTween(rectTransform.DOMove(route[idx].TF.position, moveTime),
            ReverseStepCompleted, MoveCompleted);
        tweenMoveStack.Add(tween);
        //rectTransform.DOMove(route.checkPoints[idx].position, moveTime);
    }

    public void MoveCompleted()
    {
        lastMoveTIme = Time.fixedDeltaTime;
        if (checkpointIndex + 1 < route.Count)
        {
            checkpointIndex++;
            Move(checkpointIndex);
        }
        else
        {
            rectTransform.DOPause();
            CoregameManager.Ins.listRewindEvent.Add(new("Rotate object start rewind", () => StartReverse()));
        }
    }

    public void StartReverse(object arg = null)
    {
        if (isReversing) return;
        isReversing = true;
        rotateTween.Pause();
        rotateTween.timeScale = CoregameManager.Ins.reverseRatio;
        rotateTween.PlayBackwards();
        //rectTransform.DORotate(Vector3.forward * -360, 1 / rotateSpeed / CoregameManager.Ins.reverseRatio, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        float reverseScale = CoregameManager.Ins.reverseRatio;
        foreach (var tween in tweenMoveStack)
            tween.timeScale = reverseScale;


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
