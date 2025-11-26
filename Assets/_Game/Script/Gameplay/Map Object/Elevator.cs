using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Elevator : RewindableObject
{
    [Header("ELEVATOR")]
    [SerializeField] List<Transform> route;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float speed;

    bool moveCompleted;
    int checkpointIndex;
    List<Tween> tweenMoveStack = new();
    int reverseIndex;
    bool isReversing;
    public void AddPlayer()
    {
        PlayerMove.Ins.Stop();
        PlayerMove.Ins.TF.SetParent(rectTransform, true);
        //CoregameManager.Ins.listRewindEvent.Add(new("", () => PlayerMove.Ins.TF.SetParent(CoregameManager.Ins.currentLevel.transform, true)));
        StartMove();
    }

    public void StartMove(object arg = null)
    {
        checkpointIndex = 0;
        isReversing = false;
        moveCompleted = false;
        tweenMoveStack.Clear();
        Move(checkpointIndex);
    }

    void Move(int idx)
    {
        float dis = Vector2.Distance(rectTransform.position, route[idx].position);
        float moveTime = dis / speed;
        Tween tween = TweenUtil.RewindableTween(rectTransform.DOMove(route[idx].position, moveTime),
            ReverseStepCompleted, MoveCompleted);
        tweenMoveStack.Add(tween);
        //rectTransform.DOMove(route.checkPoints[idx].position, moveTime);
    }

    public void MoveCompleted()
    {
        if (checkpointIndex + 1 < route.Count)
        {
            checkpointIndex++;
            Move(checkpointIndex);
        }
        else
        {
            moveCompleted = true;
            PlayerMove.Ins.TF.SetParent(CoregameManager.Ins.currentLevel.transform, true);
            PlayerMove.Ins.ContinueMove();
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                PlayerMove.Ins.TF.SetParent(rectTransform);
                //PlayerMove.Ins.Stop();
                StartReverse();
            }));
        }
    }

    public void StartReverse()
    {
        if (isReversing) return;
        if (tweenMoveStack.Count == 0) return;
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
        moveCompleted = false;
        PlayerMove.Ins.TF.SetParent(CoregameManager.Ins.currentLevel.transform, true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (collision.CompareTag(GameConst.TAG_PLAYER))
        {
            AddPlayer();
        }
    }

    public override void DelegateStartRewind(object args)
    {
        base.DelegateStartRewind(args);
        if (!moveCompleted) StartReverse();
    }
}
