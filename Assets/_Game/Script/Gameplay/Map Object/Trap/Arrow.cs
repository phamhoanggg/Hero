using DG.Tweening;
using UnityEngine;

public class Arrow : RewindableObject
{
    [SerializeField] RectTransform tf;
    private float speed;
    Tween tween;
    bool completed;

    private void Update()
    {
        if (tween == null) Debug.Log("Null Tween");
        else Debug.Log("HAS Tween");
    }
    public void FlyToTarget(Transform targetTf, float speed)
    {
        StartTimeStamp_SinceGameStart = Time.time - CoregameManager.Ins.startgameStamp;
        this.speed = speed;
        float dis = Vector2.Distance(targetTf.position, tf.position);
        float time = dis / speed;
        tween = TweenUtil.RewindableTween(
            tf.DOMove(targetTf.position, time).SetEase(Ease.Linear).OnComplete(() =>
            {
                completed = true;
                EndTimeStamp_SinceGameStart = Time.time - CoregameManager.Ins.startgameStamp;
                CoregameManager.Ins.listRewindEvent.Add(new("arrow reach floor", () =>
                {
                    tf.DOPause();
                    tween.timeScale = CoregameManager.Ins.reverseRatio;
                    tween.PlayBackwards();
                }));
            }), 
            RewindCompleted
        );
    }

    public override void DelegateRewind(object args)
    {
        tf.DOPause();
        if (tween == null) Debug.Log("Arrow tween null");
        if (tween == null || completed) return;
        Debug.Log("Tween play backward");
        tween.timeScale = CoregameManager.Ins.reverseRatio;
        tween.PlayBackwards();
    }

    void RewindCompleted() {
        Debug.Log("Arrow rewind");
        completed = false;
        tween.Kill();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (collision.CompareTag(GameConst.TAG_SHIELD)) {
            tf.DOPause();
            completed = true;

            transform.SetParent(rootParent);
            tf.SetParent(collision.gameObject.transform);
            CoregameManager.Ins.listRewindEvent.Add(new("Arrow reach shield", () =>
            {
                tf.DOPause();
                tf.SetParent(rootParent);
                float dis = Vector2.Distance(rootPosition, tf.position);
                float time = dis / speed;
                tf.DOAnchorPos(rootPosition, time / CoregameManager.Ins.reverseRatio);
            }));
        }
    }
}
