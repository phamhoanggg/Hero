using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class Conveyor : SpineController
{
    [Header("Converyor")]
    [SerializeField] RectTransform checkPoint;
    [SerializeField] float moveSpeed;
    [SerializeField] Collider2D col2d;

    string currentANimName;
    IAnimplayer animPlayer;
    bool hasTweenMoving;
    List<Tween> listTween = new();
    public override void DelegateStartRewind(object args)
    {
        base.DelegateStartRewind(args);
        PlayBackward(currentANimName, loop: true);

        if (animPlayer != null && hasTweenMoving)
        {
            hasTweenMoving = false;
            int tweenId = listTween.Count - 1;
            listTween[tweenId].Pause();
            listTween[tweenId].timeScale = CoregameManager.Ins.reverseRatio;
            listTween[tweenId].PlayBackwards();
        }
    }

    public override void OnCompleteRewind(object args)
    {
        base.OnCompleteRewind(args);
        StartCoroutine(Play(currentANimName));
        col2d.enabled = false;
    }

    public override void OnStartGame(object args)
    {
        base.OnStartGame(args);
        col2d.enabled = true;
    }
    private void Start()
    {
        currentANimName = "On-Left";
        col2d.enabled = false;
        StartCoroutine(Play(currentANimName));
    }
    public void ChangeDirect()
    {
        string preAnim = currentANimName;
        currentANimName = "On-Left2";
        StartCoroutine(Play(currentANimName));

        Vector3 localPos = checkPoint.transform.localPosition;
        checkPoint.transform.localPosition = new Vector3(-localPos.x, localPos.y, localPos.z);
        CoregameManager.Ins.listRewindEvent.Add(new("", () =>
        {
            currentANimName = preAnim;
            PlayBackward(preAnim, loop: true);
            checkPoint.transform.localPosition = localPos;
        }));

        if (animPlayer != null && hasTweenMoving)
        {
            hasTweenMoving = false;
            int tweenId = listTween.Count - 1;
            listTween[tweenId].Pause();
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                listTween[tweenId].timeScale = CoregameManager.Ins.reverseRatio;
                listTween[tweenId].PlayBackwards();
            }));

            MoveAlongConveyor();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (collision.CompareTag(GameConst.TAG_ENEMY))
        {
            animPlayer = collision.GetComponentInParent<IAnimplayer>();
            if (animPlayer != null)
            {
                MoveAlongConveyor();
            }
        }
    }

    public void MoveAlongConveyor()
    {
        animPlayer.Stop();
        animPlayer.PlayAnim(Anim.Idle);
        hasTweenMoving = true;
        float dis = Vector2.Distance(animPlayer.GetRoot().transform.position, checkPoint.position);
        Tween tween = TweenUtil.RewindableTween(animPlayer.GetRoot().transform.DOMove(checkPoint.position, dis / moveSpeed),
            onRewinded: () => { },
            onCompleted: () =>
            {
                hasTweenMoving = false;
                animPlayer = null;
                int tweenId = listTween.Count - 1;
                CoregameManager.Ins.listRewindEvent.Add(new("", () =>
                {
                    listTween[tweenId].timeScale = CoregameManager.Ins.reverseRatio;
                    listTween[tweenId].PlayBackwards();
                }));
            });

        listTween.Add(tween);
    }
}
