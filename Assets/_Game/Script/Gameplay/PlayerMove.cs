using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerMove : SingletonMonoBehaviour<PlayerMove>
{
    public List<CheckPoint> route = new();
    [SerializeField] PlayerSpine spine;
    [SerializeField] Transform tf;
    [SerializeField] float speed;
    [SerializeField] float reverseSpeed;
    [Space]
    [SerializeField] Transform shieldRoot;
    [SerializeField] Transform shieldTf;

    private Key key;
    int checkpointIndex;
    bool hasKey;

    List<Tween> tweenMoveStack;
    public void StartMove()
    {
        route = CoregameManager.Ins.GenerateRouteForPlayer();
        checkpointIndex = 0;
        tweenMoveStack = new();
        Move(checkpointIndex);
        spine.PlayAnim(Anim.Run, true);
    }
    public void Move(int id)
    {
        float dis = Vector2.Distance(route[id].TF.position, tf.position);
        float time = dis / speed;
        Tween moveTween = tf.DOMove(route[id].TF.position, time).SetEase(Ease.Linear).SetAutoKill(false).OnRewind(ReverseStepCompleted).OnComplete(() =>
        {
            if (checkpointIndex + 1 < route.Count)
            {
                checkpointIndex++;
                Move(checkpointIndex);
            }
        });

        tweenMoveStack.Add(moveTween);
    }

    public void Stop()
    {
        tf.DOKill();
        spine.PlayAnim(Anim.Idle, true);
    }
    public void ContinueMove()
    {
        spine.PlayAnim(Anim.Run, true);
        Move(checkpointIndex);
    }

    public void GetShield(ShieldDirect direct)
    {
        shieldTf.localScale = Vector3.zero;
        shieldTf.DOScale(1, 0.25f);
        if (direct == ShieldDirect.Horizontal)
        {

        }
        else if (direct == ShieldDirect.Vertical)
        {
            shieldRoot.DORotate(Vector3.forward * 90, 0.25f);
        }

        Invoke(nameof(ContinueMove), 0.3f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Die"))
        {
            tf.DOKill();
            spine.PlayAnim(Anim.Die);
            CoregameManager.Ins.Invoke(nameof(CoregameManager.Ins.Reverve), 0.25f);
        }
        else if (other.CompareTag("Chest"))
        {
            if (!hasKey) return;

            Stop();
            key.PlayPutInLockAnim(() =>
            {
                Chest chest = other.GetComponent<Chest>();
                chest.Open();
                GetShield(chest.ShieldDirection);
            });

        }else if (other.CompareTag("Key"))
        {
            other.transform.SetParent(tf);
            key = other.GetComponent<Key>();
            hasKey = true;
        }
    }

    #region REVERSE
    public void StartReverse()
    {
        float reverseTimeScale = reverseSpeed / speed;
        foreach (var tween in tweenMoveStack)
            tween.timeScale = reverseTimeScale;

        spine.PlayAnim(Anim.Run, true, -1);
        tweenMoveStack[checkpointIndex].PlayBackwards();
    }

    public void ReverseStepCompleted()
    {
        checkpointIndex--;
        if (checkpointIndex >= 0) tweenMoveStack[checkpointIndex].PlayBackwards();
        else
        {
            ReverseCompleted();
        }
    }

    public void ReverseCompleted()
    {
        Debug.Log("Completed");
        CoregameManager.Ins.ReverseCompleted();
        spine.PlayAnim(Anim.Idle, true);
        foreach (var tween in tweenMoveStack)
            tween.timeScale = 1f;
    }
    #endregion
}

public enum ShieldDirect
{
    Horizontal = 0,
    Vertical = 1,
}
