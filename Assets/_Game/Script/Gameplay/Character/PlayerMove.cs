using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PlayerMove : SingletonMonoBehaviour<PlayerMove>, IAnimplayer
{
    public List<CheckPoint> route = new();
    public PlayerSpine spine;
    public WeaponSpine weaponSpine;
    [SerializeField] Transform tf;
    [SerializeField] float speed;
    [SerializeField] CheckPoint startCheckPoint;
    public Transform TF => tf;
    public Transform PlayerTf => tf;
    [SerializeField] private Door door;
    int checkpointIndex;
    int reverseIndex;

    List<Tween> tweenMoveStack;
    float lastMoveTime;
    Vector2 defaultPos;

    bool isMoving;

    private void Start()
    {
        if (door == null)
            Debug.LogError("NO DOORRRRR");
    }
    public void StartMove()
    {
        defaultPos = tf.position;
        route = CoregameManager.Ins.GenerateRouteForPlayer(startCheckPoint);
        if (route.Count == 0) route.Add(door.checkpoint);
        checkpointIndex = 0;
        tweenMoveStack = new();
        spine.StartMove();
        Move(checkpointIndex);
        PlayAnim(Anim.Run, true);
    }
    public void Move(int id)
    {
        float dis = Vector2.Distance(route[id].TF.position, tf.position);
        float time = dis / speed;
        isMoving = true;
        Tween moveTween = TweenUtil.RewindableTween(tf.DOMove(route[id].TF.position, time).OnUpdate(() => lastMoveTime = Time.realtimeSinceStartup), 
            ReverseStepCompleted, 
            MoveCompleted);
        int scaleX = (route[id].TF.position.x >= tf.position.x) ? 1 : -1;
        float prev_scaleX = tf.localScale.x;
        tf.localScale = new Vector3(scaleX, 1, 1);
        if (scaleX * prev_scaleX < 0)
            CoregameManager.Ins.listRewindEvent.Add(new("Player FlipX", () =>
            {
                tf.localScale = new Vector3(-scaleX, 1, 1);
            }));

        tweenMoveStack.Add(moveTween);
    }

    public void Move(Vector3 target)
    {
        float dis = Vector2.Distance(target, tf.position);
        float time = dis / speed;
        isMoving = true;
        Tween moveTween = TweenUtil.RewindableTween(tf.DOMove(target, time).OnUpdate(() => lastMoveTime = Time.realtimeSinceStartup),
            ReverseStepCompleted,
            MoveCompleted);
        int scaleX = (target.x >= tf.position.x) ? 1 : -1;
        float prev_scaleX = tf.localScale.x;
        tf.localScale = new Vector3(scaleX, 1, 1);
        if (scaleX * prev_scaleX < 0)
            CoregameManager.Ins.listRewindEvent.Add(new("Player FlipX", () =>
            {
                tf.localScale = new Vector3(-scaleX, 1, 1);
            }));

        tweenMoveStack.Add(moveTween);
    }

    public void MoveCompleted()
    {
        isMoving = false;
        int tweenID = tweenMoveStack.Count - 1;
        CoregameManager.Ins.listRewindEvent.Add(new("", () =>
        {
            tweenMoveStack[tweenID].timeScale = CoregameManager.Ins.reverseRatio;
            tweenMoveStack[tweenID].PlayBackwards();
            PlayBackward(Anim.Run, loop: true);
        }));
        if (checkpointIndex + 1 < route.Count)
        {
            checkpointIndex++;
            Move(checkpointIndex);
        }
        else
        {
            PlayAnim(Anim.Idle);
            if (door.Opened)
            {
                CoregameManager.Ins.Win();
                gameObject.SetActive(false);
                door.Invoke(nameof(door.Close), 0.15f);
            }
        }
    }
    public void Stop()
    {
        lastMoveTime = Time.realtimeSinceStartup;
        //Tween tween = tweenMoveStack.Last();
        //tween.Pause();
        TF.DOPause();
        PlayAnim(Anim.Idle, true);

        if (isMoving)
        {
            isMoving = false;
            int tweenID = tweenMoveStack.Count - 1;
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                tweenMoveStack[tweenID].timeScale = CoregameManager.Ins.reverseRatio;
                tweenMoveStack[tweenID].PlayBackwards();
                PlayBackward(Anim.Run, loop: true);
            }));
        }
        
    }
    public void ContinueMove()
    {
        PlayAnim(Anim.Run, true);
        Move(checkpointIndex);
    }

    public void SetWeapon(Weapon wp)
    {
        weaponSpine.SetWeapon(wp.weaponSkin, wp.attackAnim, wp.attackRange, this);
        wp.gameObject.SetActive(false);
        CoregameManager.Ins.listRewindEvent.Add(new("Equip weapon", () =>
        {
            wp.gameObject.SetActive(true);
            weaponSpine.SetWeapon(Skin.Normal, Anim.Bow, 0, this);
        }));
    }

    #region REVERSE
   
    public void StartReverse()
    {
        float reverseScale = CoregameManager.Ins.reverseRatio;
        Glitch.Ins.Play();

        if (isMoving)
        {
            isMoving = false;
            TF.DOPause();
            tweenMoveStack.Last().timeScale = reverseScale;
            tweenMoveStack.Last().PlayBackwards();
            PlayAnim(Anim.Run);
        }
        door.Close();

        reverseIndex = tweenMoveStack.Count - 1;
    }

    public void ReverseStepCompleted()
    {
        //tweenMoveStack.RemoveAt(reverseIndex);

        reverseIndex--;
        if (reverseIndex >= 0 && Vector2.Distance(TF.position, defaultPos) > 0.1f)
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
        reverseIndex = -1;
        Glitch.Ins.ResetNoise();
        CoregameManager.Ins.ReverseCompleted();
        PlayAnim(Anim.Idle, true);
    }

    public void PlayAnim(Anim anim, bool loop = true, float delayTime = 0)
    {
        StartCoroutine(spine.Play(anim, loop, delayTime));
        StartCoroutine(weaponSpine.Play(anim, loop, delayTime));
        if (anim == Anim.Die) weaponSpine.OnParentDie();
    }

    public void PlayBackward(Anim anim, float startTrackTime = 1, bool loop = false)
    {
        spine.PlayBackward(anim, startTrackTime, loop);
        weaponSpine.PlayBackward(anim, startTrackTime, loop);
    }

    public GameObject GetRoot()
    {
        return gameObject;
    }
    #endregion
}

public enum ShieldDirect
{
    Horizontal = 0,
    Vertical = 1,
}
