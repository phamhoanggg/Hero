using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerMove : SingletonMonoBehaviour<PlayerMove>, IAnimplayer
{
    public List<CheckPoint> route = new();
    public PlayerSpine spine;
    public WeaponSpine weaponSpine;
    [SerializeField] Transform tf;
    [SerializeField] float speed;
    [SerializeField] Shield shield;

    public Transform PlayerTf => tf;
    [SerializeField] private Door door;
    private Key key;
    int checkpointIndex;
    int reverseIndex;
    bool hasKey;

    List<Tween> tweenMoveStack;
    float lastMoveTime;

    private void Start()
    {
        if (door == null)
            Debug.LogError("NO DOORRRRR");
    }
    public void StartMove()
    {
        route = CoregameManager.Ins.GenerateRouteForPlayer();
        if (route.Count == 0) route.Add(door.checkpoint);
        checkpointIndex = 0;
        tweenMoveStack = new();
        hasKey = false;
        key = null;
        Move(checkpointIndex);
        PlayAnim(Anim.Run, true);
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
            else
            {
                CoregameManager.Ins.Win();
                gameObject.SetActive(false);
                door.Close();
            }
        });

        tweenMoveStack.Add(moveTween);
    }

    public void Stop()
    {
        lastMoveTime = Time.fixedTime;
        tf.DOPause();
        PlayAnim(Anim.Idle, true);
    }
    public void ContinueMove()
    {
        tweenMoveStack.Add(DOVirtual.Float(0, 1, Time.fixedTime - lastMoveTime, (float update) => { }).SetAutoKill(false).OnRewind(ReverseStepCompleted));
        PlayAnim(Anim.Run, true);
        Move(checkpointIndex);
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (other.CompareTag(GameConst.TAG_DIE))
        {
            tf.DOPause();
            VibrationManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
            CoregameManager.Ins.ShakeCamera();
            PlayAnim(Anim.Die, false);
            CoregameManager.Ins.StartCoroutine(CoregameManager.Ins.Reverve(true));
        }
        else if (other.CompareTag(GameConst.TAG_CHEST))
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

        }else if (other.CompareTag(GameConst.TAG_KEY))
        {
            key = other.GetComponent<Key>();
            key.OnCollected();
            hasKey = true;
        }
    }

    #region REVERSE
    
    private void FixedUpdate()
    {
        if (CoregameManager.Ins.IsReversing)
        {
            foreach (var ev in CoregameManager.Ins.listRewindEvent)
            {
                if (Vector2.Distance(ev.playerPosition, PlayerTf.position) < 0.25f)
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
        float reverseScale = CoregameManager.Ins.reverseRatio;
        foreach (var tween in tweenMoveStack)
            tween.timeScale = reverseScale;

        Debug.Log("Start Reverse: " + Time.fixedTime);

        reverseIndex = tweenMoveStack.Count - 1;
        Glitch.Ins.Play();
        PlayAnim(Anim.Run, true, -reverseScale);
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
    }

    public void PlayAnim(Anim anim, bool loop = true, float timeScale = 1)
    {
        spine.Play(anim, loop, timeScale);
        weaponSpine.Play(anim, loop, timeScale);
    }
    #endregion
}

public enum ShieldDirect
{
    Horizontal = 0,
    Vertical = 1,
}
