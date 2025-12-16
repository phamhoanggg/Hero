using DG.Tweening;
using SharedModules.ED;
using UnityEngine;

public class BeeHive : MonoBehaviour
{
    [SerializeField] Transform tf;
    [SerializeField] Transform fallPos;
    [SerializeField] Collider2D col;
    [SerializeField]
    Beeee[] listBees;
    Tween fallTween;
    bool isFalling;
    bool attacked;
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
        attacked = false;
        if (isFalling)
        {
            isFalling = false;
            fallTween.timeScale = CoregameManager.Ins.reverseRatio;
            fallTween.PlayBackwards();
        }
    }
    public void Fall()
    {
        isFalling = true;
        fallTween = tf.DOMove(fallPos.position, 0.75f).SetAutoKill(false).OnComplete(() =>
        {
            isFalling = false;
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                fallTween.timeScale = CoregameManager.Ins.reverseRatio;
                fallTween.PlayBackwards();
            }));
        });

    }

    void Attack(Transform target)
    {
        if (attacked) return;
        attacked = true;
        foreach (var bee in listBees) 
            bee.Attack(target);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (collision.CompareTag(GameConst.TAG_DIE))
        {
            Fall();
            col.enabled = false;
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                col.enabled = true;
            }));
        }
        else if (collision.CompareTag(GameConst.TAG_PLAYER) || collision.CompareTag(GameConst.TAG_ENEMY))
        {
            Attack(collision.transform);
        }
    }
}
