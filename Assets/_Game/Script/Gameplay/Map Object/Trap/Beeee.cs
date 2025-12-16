using System.Collections;
using DG.Tweening;
using SharedModules.ED;
using Spine;
using Spine.Unity;
using UnityEngine;

public class Beeee : MonoBehaviour
{
    [SerializeField] SkeletonGraphic spine;
    [SerializeField] Transform tf;
    [SerializeField] float attackMoveSpeed;
    [SerializeField] Collider2D col2D;

    Tween attackTween;
    bool isAttacking;
    Coroutine dieRoutine;
    private void OnEnable()
    {
        EventDispatcher.RegisterListener(EventId.OnRewind, OnStartReverse);
    }

    private void OnDisable()
    {
        EventDispatcher.UnregisterListener(EventId.OnRewind, OnStartReverse);
    }
    public void OnStartReverse(object args)
    {
        if (isAttacking)
        {
            attackTween.Pause();
            isAttacking = false;
            attackTween.timeScale = CoregameManager.Ins.reverseRatio;
            attackTween.PlayBackwards();
        }

        if (dieRoutine != null)
        {
            StopCoroutine(dieRoutine);
            dieRoutine = null;
            if (gameObject.activeInHierarchy)
            {
                PlayBackward("Die");
            }
        }
    }
    public IEnumerator Die()
    {
        attackTween.Pause();
        isAttacking = false;
        col2D.enabled = false;
        CoregameManager.Ins.listRewindEvent.Add(new("", () =>
        {
            attackTween.timeScale = CoregameManager.Ins.reverseRatio;
            attackTween.PlayBackwards();
            col2D.enabled = true;
        }));

        Debug.Log("Bee Die");
        Play("Die", false);
        CoregameManager.Ins.listRewindEvent.Add(new("", () =>
        {
            Play("Idle");
        }));


        float dieTime = GetAnimDuration("Die");
        yield return new WaitForSeconds(dieTime);
        gameObject.SetActive(false);
        CoregameManager.Ins.listRewindEvent.Add(new("", () =>
        {
            gameObject.SetActive(true);
            PlayBackward("Die");
        }));
    }

    public void Attack(Transform target)
    {
        Debug.Log("Bee Attack");

        isAttacking = true;
        float dis = Vector2.Distance(tf.position, target.position);
        attackTween = tf.DOMove(target.position, dis / attackMoveSpeed).SetAutoKill(false).OnComplete(() =>
        {
            isAttacking = false;
            gameObject.SetActive(false);
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                attackTween.timeScale = CoregameManager.Ins.reverseRatio;
                attackTween.PlayBackwards();
                gameObject.SetActive(true);
            }));
        });
    }

    public void Play(string animName, bool loop = true)
    {
        // Start animation
        spine.AnimationState.SetAnimation(0, animName, loop);
    }

    public void PlayBackward(string animName, float startTrackTime = 1, bool loop = false)
    {
        float timeScale = -CoregameManager.Ins.reverseRatio;

        // Start animation
        var trackEntry = spine.AnimationState.SetAnimation(0, animName, loop);
        trackEntry.TrackTime = trackEntry.Animation.Duration * startTrackTime;
        trackEntry.TimeScale = timeScale;
    }

    public float GetAnimDuration(string animName)
    {
        Spine.Animation anim = spine.Skeleton.Data.FindAnimation(animName);
        return anim?.Duration ?? 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (collision.CompareTag(GameConst.TAG_TORCH))
        {
            dieRoutine = StartCoroutine(Die());
        }
        else if (collision.CompareTag(GameConst.TAG_PLAYER) || collision.CompareTag(GameConst.TAG_ENEMY))
        {
            gameObject.SetActive(false);
            attackTween.Pause();
            Play("Idle");
            isAttacking = false;
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                gameObject.SetActive(true);
                attackTween.timeScale = CoregameManager.Ins.reverseRatio;
                attackTween.PlayBackwards();
            }));
        }
        
    }
}
