using DG.Tweening;
using UnityEngine;

public class Arrow : RewindableObject
{
    [SerializeField] RectTransform tf;
    private float speed;
    Tween tween;
    bool completed;
    public void FlyToTarget(Transform targetTf, float speed)
    {
        StartTimeStamp_SinceGameStart = Time.time - CoregameManager.Ins.startgameStamp;
        this.speed = speed;
        float dis = Vector2.Distance(targetTf.position, tf.position);
        float time = dis / speed;
        tween = tf.DOMove(targetTf.position, time).SetEase(Ease.Linear).OnRewind(RewindCompleted).SetAutoKill(false).OnComplete(() =>
        {
            completed = true;
            EndTimeStamp_SinceGameStart = Time.time - CoregameManager.Ins.startgameStamp;
            CoregameManager.Ins.listRewindEvent.Add(new("arrow reach floor", () =>
            {
                tf.DOPause();
                float dis = Vector2.Distance(rootPosition, tf.position);
                float time = dis / speed;
                tf.DOAnchorPos(rootPosition, time / CoregameManager.Ins.reverseRatio);
            }));
        });
    }

    public override void DelegateRewind()
    {
        tf.DOPause();
        if (tween == null || completed) return;
        tween.timeScale = CoregameManager.Ins.reverseRatio;
        tween.PlayBackwards();
    }

    void RewindCompleted()
    {
        completed = false;
        tween.Kill();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (collision.CompareTag("Shield")) {
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
