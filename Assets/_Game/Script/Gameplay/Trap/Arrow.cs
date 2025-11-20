using DG.Tweening;
using UnityEngine;

public class Arrow : RewindableObject
{
    [SerializeField] RectTransform tf;
    private float speed;
    Tween tween;
    public void FlyToTarget(Transform targetTf, float speed)
    {
        StartTimeStamp_SinceGameStart = Time.time - CoregameManager.Ins.startgameStamp;
        this.speed = speed;
        float dis = Vector2.Distance(targetTf.position, tf.position);
        float time = dis / speed;
        tween = tf.DOMove(targetTf.position, time).SetEase(Ease.Linear).SetAutoKill(false).OnComplete(() =>
        {
            EndTimeStamp_SinceGameStart = Time.time - CoregameManager.Ins.startgameStamp;
            CoregameManager.Ins.listRewindEvent.Add(new(Time.time - CoregameManager.Ins.startgameStamp, "arrow reach floor", () =>
            {
                tf.DOPause();
                float dis = Vector2.Distance(rootPosition, tf.position);
                float time = dis / speed;
                tf.DOAnchorPos(rootPosition, time / CoregameManager.Ins.reverseRatio);
            }));
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (collision.CompareTag("Shield")) {
            tf.DOKill();
            transform.SetParent(rootParent);
            tf.SetParent(collision.gameObject.transform);
            CoregameManager.Ins.listRewindEvent.Add(new(Time.time - CoregameManager.Ins.startgameStamp, "Arrow reach shield", () =>
            {
                tf.DOPause();
                float dis = Vector2.Distance(rootPosition, tf.position);
                float time = dis / speed;
                tf.DOAnchorPos(rootPosition, time / CoregameManager.Ins.reverseRatio);
            }));

            CoregameManager.Ins.listRewindEvent.Add(new(Time.time - CoregameManager.Ins.startgameStamp, "Arrow change parent", () =>
            {
                tf.SetParent(rootParent);
            }));
        }
    }

    public override void Reverse()
    {
        float dis = Vector2.Distance(rootPosition, tf.position);
        float time = dis / speed / CoregameManager.Ins.reverseRatio;
        tf.DOMove(rootPosition, time).SetEase(Ease.Linear);
    }
}
