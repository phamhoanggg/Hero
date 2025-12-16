using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SharedModules.ED;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform rotateImage;
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed = 1;
    [SerializeField] InvertCheckPoint startPoint;
    List<InvertCheckPoint> route;
    int checkpointIndex;
    List<Tween> tweenMoveStack = new();
    int reverseIndex;
    bool moveDone;
    bool isReversing;
    Tween rotateTween;
    private void OnEnable()
    {
        EventDispatcher.RegisterListener(EventId.OnRewind, OnPlayerStartReverse);
        EventDispatcher.RegisterListener(EventId.OnStartMove, StartMove);
    }

    private void OnDisable()
    {
        EventDispatcher.UnregisterListener(EventId.OnRewind, OnPlayerStartReverse);
        EventDispatcher.UnregisterListener(EventId.OnStartMove, StartMove);
    }

    public void OnPlayerStartReverse(object arg = null)
    {
        if (!moveDone)
        {
            tweenMoveStack.Last().timeScale = CoregameManager.Ins.reverseRatio;
            tweenMoveStack.Last().PlayBackwards();
            StartReverse();
        }
    }

    public void StartMove(object arg = null)
    {
        route = CoregameManager.Ins.GenerateInvertRouteForRotateObject(startPoint);
        checkpointIndex = 0;
        isReversing = false;
        tweenMoveStack.Clear();
        rotateTween = rectTransform.DORotate(Vector3.forward * 360, 1 / rotateSpeed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).SetAutoKill(false);
        Move(checkpointIndex);
        moveDone = false;
    }

    void Move(int idx)
    {
        float dis = Vector2.Distance(rectTransform.position, route[idx].TF.position);
        float moveTime = dis / speed;

        float direct = rotateTween.timeScale;
        if (Mathf.Abs(route[idx].TF.position.x - rectTransform.position.x) > 0.1f)
        {
            direct = (route[idx].TF.position.x < rectTransform.position.x) ? 1 : -1;
        }
        
        if (direct * rotateTween.timeScale < 0)
        {
            rotateTween.timeScale = direct;
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                rotateTween.timeScale = -direct * CoregameManager.Ins.reverseRatio;
            }));
        }

        Tween tween = TweenUtil.RewindableTween(rectTransform.DOMove(route[idx].TF.position, moveTime),
            ReverseStepCompleted, MoveCompleted);
        tweenMoveStack.Add(tween);
        //rectTransform.DOMove(route.checkPoints[idx].position, moveTime);
    }

    public void MoveCompleted()
    {
        int moveTweenId = tweenMoveStack.Count - 1;
        CoregameManager.Ins.listRewindEvent.Add(new("", () =>
        {
            tweenMoveStack[moveTweenId].timeScale = CoregameManager.Ins.reverseRatio;
            tweenMoveStack[moveTweenId].PlayBackwards();
        }));

        if (checkpointIndex + 1 < route.Count)
        {
            checkpointIndex++;
            Move(checkpointIndex);
        }
        else
        {
            rectTransform.DOPause();
            CoregameManager.Ins.listRewindEvent.Add(new("Rotate object start rewind", () => StartReverse()));
            moveDone = true;
        }
    }

    public void StartReverse()
    {
        if (isReversing) return;
        isReversing = true;
        rotateTween.Pause();
        rotateTween.timeScale = CoregameManager.Ins.reverseRatio;
        rotateTween.PlayBackwards();
        reverseIndex = tweenMoveStack.Count - 1;
    }

    public void ReverseStepCompleted()
    {
        //tweenMoveStack[reverseIndex].Kill();
        reverseIndex--;
        if (reverseIndex >= 0)
        {
            //tweenMoveStack[reverseIndex].PlayBackwards();
        }
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
