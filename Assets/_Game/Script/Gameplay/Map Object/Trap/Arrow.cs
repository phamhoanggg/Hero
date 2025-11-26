using DG.Tweening;
using UnityEngine;

public class Arrow : RewindableObject
{
    [Header("ARROW")]
    [SerializeField] RectTransform tf;
    [SerializeField] bool isTrap;
    private float speed;
    Tween tween;
    bool completed;

    private void Update()
    {
        if (tween == null) Debug.Log("Null Tween");
        else Debug.Log("HAS Tween");
    }
    public void FlyToTarget(Vector2 targetPos, float speed)
    {
        StartTimeStamp_SinceGameStart = Time.time - CoregameManager.Ins.startgameStamp;
        this.speed = speed;
        float dis = Vector2.Distance(targetPos, tf.position);
        float time = dis / speed;
        tween = TweenUtil.RewindableTween(
            tf.DOMove(targetPos, time),
            RewindCompleted,
            OnArrowReachTarget
        );
    }

    public void OnArrowReachTarget()
    {
        completed = true;
        EndTimeStamp_SinceGameStart = Time.time - CoregameManager.Ins.startgameStamp;
        gameObject.SetActive(isTrap);
        CoregameManager.Ins.listRewindEvent.Add(new("arrow reach floor", () =>
        {
            tf.DOPause();
            gameObject.SetActive(true);
            tween.timeScale = CoregameManager.Ins.reverseRatio;
            tween.PlayBackwards();
        }));
    }
    public override void DelegateStartRewind(object args)
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
        if (!isTrap) gameObject.SetActive(false);
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
