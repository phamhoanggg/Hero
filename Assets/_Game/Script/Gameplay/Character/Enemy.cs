using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SharedModules.ED;
using UnityEngine;

public class Enemy : MonoBehaviour, IAnimplayer
{
    [SerializeField] SpineController mainSpine;
    [SerializeField] WeaponSpine weaponSpine;
    [SerializeField] Skin weapon;
    [SerializeField] Anim attackAnim;
    [SerializeField] int attackRange;
    public bool canMove;
    bool isMoving;
    [SerializeField] float moveSpeed;
    List<Tween> moveTweens = new();
    public bool IsDead {  get; private set; }
    private void Awake()
    {
        SetupWeapon();
    }

    private void OnEnable()
    {
        EventDispatcher.RegisterListener(EventId.OnRewind, OnStartRewind);
        EventDispatcher.RegisterListener(EventId.OnStartMove, OnPlayerStartMove);
        EventDispatcher.RegisterListener(EventId.OnRewindCompleted, OnRewindCompleted);
    }

    private void OnDisable()
    {
        EventDispatcher.UnregisterListener(EventId.OnRewind, OnStartRewind);
        EventDispatcher.UnregisterListener(EventId.OnStartMove, OnPlayerStartMove);
        EventDispatcher.UnregisterListener(EventId.OnRewindCompleted, OnRewindCompleted);
    }

    public void OnPlayerStartMove(object args)
    {
        moveTweens.Clear();
    }
    public void OnStartRewind(object args)
    {
        if (canMove && isMoving && moveTweens.Count > 0)
        {
            int tweenId = moveTweens.Count - 1;
            moveTweens[tweenId].timeScale = CoregameManager.Ins.reverseRatio;
            moveTweens[tweenId].PlayBackwards();
            PlayAnim(Anim.Run);
        }
    }

    public void OnRewindCompleted(object args)
    {
        PlayAnim(Anim.Idle);
    }
    private void Start()
    {
        PlayAnim(Anim.Idle);
    }
    public void SetupWeapon()
    {
        weaponSpine.SetWeapon(weapon, attackAnim, attackRange, this);
    }
    public void PlayAnim(Anim anim, bool loop = true, float delayTime = 0)
    {
        StartCoroutine(mainSpine.Play(anim, loop, delayTime));
        StartCoroutine(weaponSpine.Play(anim, loop, delayTime));
    }

    public void PlayBackward(Anim anim, float startTrackTime = 1, bool loop = false)
    {
        mainSpine.PlayBackward(anim, startTrackTime, loop);
        weaponSpine.PlayBackward(anim, startTrackTime, loop);
    }

    public GameObject GetRoot()
    {
        return gameObject;
    }

    public void Move(Vector3 targetPos, bool playRun = true)
    {
        float dis = Vector2.Distance(transform.position, targetPos);
        float moveTime = dis / moveSpeed;
        int direct = (transform.position.x >= targetPos.x) ? 1 : -1;

        if (direct * transform.localScale.x < 0) 
            CoregameManager.Ins.listRewindEvent.Add(new("Enemy flip", () =>
            {
                transform.localScale = new(-direct, 1, 1);
            }));
        transform.localScale = new(direct, 1, 1);


        int tweenId = moveTweens.Count;
        isMoving = true;
        Tween moveTween = TweenUtil.RewindableTween(transform.DOMove(targetPos, moveTime),
            onRewinded: () =>
            {
                PlayAnim(Anim.Idle);
            },
            onCompleted: () =>
            {
                PlayAnim(Anim.Idle);
                isMoving = false;
                CoregameManager.Ins.listRewindEvent.Add(new("Enemy move rewind", () =>
                {
                    moveTweens[tweenId].timeScale = CoregameManager.Ins.reverseRatio;
                    moveTweens[tweenId].PlayBackwards();
                    PlayAnim(Anim.Run);
                }));
            });

        moveTweens.Add(moveTween);
        if (playRun) PlayAnim(Anim.Run);
    }

    public void Stop()
    {
        if (!canMove) return;
        transform.DOPause();
        if (moveTweens.Count > 0 && isMoving)
        {
            isMoving = false;
            int tweenId = moveTweens.Count - 1;
            CoregameManager.Ins.listRewindEvent.Add(new("Enemy move rewind after reverse attack", () =>
            {
                moveTweens[tweenId].timeScale = CoregameManager.Ins.reverseRatio;
                moveTweens[tweenId].PlayBackwards();
                PlayAnim(Anim.Run);
            }));
        }
        PlayAnim(Anim.Idle);
    }
    public virtual IEnumerator Die(Action onComplete = null) {
        weaponSpine.OnParentDie();
        CoregameManager.Ins.listRewindEvent.Add(new("Enemy play Die backward", () =>
        {
            PlayAnim(Anim.Idle);
        }));
        PlayAnim(Anim.Die, false);
        
        yield return new WaitForSeconds(mainSpine.GetAnimDuration(Anim.Die));
        CoregameManager.Ins.listRewindEvent.Add(new("Enemy play Die backward", () =>
        {
            PlayBackward(Anim.Die);
        }));

        IsDead = true;
        CoregameManager.Ins.listRewindEvent.Add(new("Enemy reset Dead", () => IsDead = false));
        gameObject.SetActive(false);
        CoregameManager.Ins.listRewindEvent.Add(new("Enemy set active true", () => gameObject.SetActive(true)));

        onComplete?.Invoke();
    }
}
