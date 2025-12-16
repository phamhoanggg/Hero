using DG.Tweening;
using SharedModules.ED;
using Spine.Unity;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] Transform tf;
    [SerializeField] SkeletonGraphic spine;

    Tween rotateTween;
    bool rotating;

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
        if (rotating)
        {
            rotating = false;
            rotateTween.timeScale = CoregameManager.Ins.reverseRatio;
            rotateTween.PlayBackwards();
        }
    }
    public void OnCollected(Transform parent)
    {
        Transform rootParent = tf.parent;
        CoregameManager.Ins.listRewindEvent.Add(new("", () =>
        {
            spine.AnimationState.ClearTrack(0);
            tf.parent = rootParent;
        }));

        circleCollider.radius = 35;
        circleCollider.offset = new Vector2(-20, 0);
        CoregameManager.Ins.listRewindEvent.Add(new("", () =>
        {
            circleCollider.radius = 20;
            circleCollider.offset = new Vector2(0, 12);
        }));

        spine.AnimationState.SetAnimation(0, "animation", true);
        tf.SetParent(parent);

        rotating = true;
        rotateTween = tf.DORotate(Vector3.zero, 0.25f).SetAutoKill(false).OnComplete(() =>
        {
            rotating = false;
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                rotateTween.timeScale = CoregameManager.Ins.reverseRatio;
                rotateTween.PlayBackwards();
            }));
        });
    }
}
