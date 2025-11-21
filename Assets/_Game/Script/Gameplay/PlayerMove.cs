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
    [SerializeField] Shield shield;

    public Transform PlayerTf => tf;

    private Key key;
    int checkpointIndex;
    int reverseIndex;
    bool hasKey;

    List<Tween> tweenMoveStack;
    float lastMoveTime;

    public void StartMove()
    {
        route = CoregameManager.Ins.GenerateRouteForPlayer();
        checkpointIndex = 0;
        tweenMoveStack = new();
        hasKey = false;
        key = null;
        Move(checkpointIndex);
        spine.Play(Anim.Run, true);
        Debug.Log("Start move: " + Time.fixedTime);
    }
    public void Move(int id)
    {
        float dis = Vector2.Distance(route[id].TF.position, tf.position);
        float time = dis / speed;
        Tween moveTween = tf.DOMove(route[id].TF.position, time).SetEase(Ease.Linear).SetAutoKill(false).SetUpdate(UpdateType.Fixed).OnRewind(ReverseStepCompleted).OnComplete(() =>
        {
            if (checkpointIndex + 1 < route.Count)
            {
                checkpointIndex++;
                Move(checkpointIndex);
            }
            else lastMoveTime = Time.fixedTime;
        });

        tweenMoveStack.Add(moveTween);
    }

    public void Stop()
    {
        lastMoveTime = Time.fixedTime;
        tf.DOPause();
        spine.Play(Anim.Idle, true);
    }
    public void ContinueMove()
    {
        tweenMoveStack.Add(DOVirtual.Float(0, 1, Time.fixedTime - lastMoveTime, (float update) => { }).SetAutoKill(false).OnRewind(ReverseStepCompleted));
        spine.Play(Anim.Run, true);
        Move(checkpointIndex);
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (other.CompareTag("Die"))
        {
            tf.DOPause();
            spine.Play(Anim.Die, false);
            CoregameManager.Ins.Invoke(nameof(CoregameManager.Ins.Reverve), 0.25f);
        }
        else if (other.CompareTag("Chest"))
        {
            if (!hasKey) return;

            Stop();
            key.PlayPutInLockAnim(() =>
            {
                if (CoregameManager.Ins.IsReversing) return;
                Chest chest = other.GetComponent<Chest>();
                chest.Open();
                shield.GetShield(chest.ShieldDirection);
            });

        }else if (other.CompareTag("Key"))
        {
            key = other.GetComponent<Key>();
            key.OnCollected();
            hasKey = true;
        }
    }

    #region REVERSE
    private void Update()
    {
        if (CoregameManager.Ins.IsReversing)
        {
            foreach (var ev in CoregameManager.Ins.listRewindEvent)
            {
                if (Vector2.Distance(ev.playerPosition, PlayerTf.position) < 0.05f)
                {
                    ev.reverseAction?.Invoke();
                    CoregameManager.Ins.listRewindEvent.Remove(ev);
                    return;
                }
            }
        }
    }
    public void StartReverse()
    {
        foreach (var tween in tweenMoveStack)
            tween.timeScale = CoregameManager.Ins.reverseRatio;

        Debug.Log("Start Reverse: " + Time.fixedTime);

        reverseIndex = tweenMoveStack.Count - 1;
        Glitch.Ins.Play();
        spine.Play(Anim.Run, true, 2);
        tweenMoveStack[reverseIndex].PlayBackwards();
    }

    public void ReverseStepCompleted()
    {
        reverseIndex--;
        if (reverseIndex >= 0) tweenMoveStack[reverseIndex].PlayBackwards();
        else
        {
            ReverseCompleted();
        }
    }

    public void ReverseCompleted()
    {
        Debug.Log("Completed: " + Time.fixedTime);
        Glitch.Ins.ResetNoise();
        CoregameManager.Ins.ReverseCompleted();
        spine.Play(Anim.Idle, true);
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
