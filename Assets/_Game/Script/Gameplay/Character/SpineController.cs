using System.Collections;
using SharedModules.ED;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;

public abstract class SpineController : MonoBehaviour
{
    public SkeletonGraphic mainSpine;
    public bool InitRight = true;

    public RectTransform RectTf;
    void Awake()
    {
        if (mainSpine == null)
            mainSpine = GetComponent<SkeletonGraphic>();
        if (RectTf == null)
            RectTf = GetComponent<RectTransform>();

        mainSpine.UpdateTiming = UpdateTiming.InFixedUpdate;
    }

    private void OnEnable()
    {
        EventDispatcher.RegisterListener(EventId.OnRewind, DelegateStartRewind);
        EventDispatcher.RegisterListener(EventId.OnRewindCompleted, OnCompleteRewind);
    }

    private void OnDisable()
    {
        EventDispatcher.UnregisterListener(EventId.OnRewindCompleted, OnCompleteRewind);
        EventDispatcher.UnregisterListener(EventId.OnRewind, DelegateStartRewind);
    }

    /// <summary>
    /// Play a UI Spine animation by name with specified loop & speed.
    /// </summary>
    public IEnumerator Play(Anim animName, bool loop = true, float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);
        mainSpine.initialFlipX = !InitRight;
        //mainSpine.AnimationState.ClearTracks();
        //mainSpine.AnimationState.SetEmptyAnimation(0, 0.2f);

        // Start animation
        mainSpine.AnimationState.SetAnimation(0, animName.ToString(), loop);
    }

    [Button]
    public void PlayBackward(Anim animName, float startTrackTime = 1)
    {
        mainSpine.initialFlipX = !InitRight;
        float timeScale = -CoregameManager.Ins.reverseRatio;

        // Start animation
        var trackEntry = mainSpine.AnimationState.SetAnimation(0, animName.ToString(), false);
        trackEntry.TimeScale = timeScale;
        trackEntry.TrackTime = trackEntry.Animation.Duration * startTrackTime;
    }
    public float GetAnimDuration(Anim animName)
    {
        Spine.Animation anim = mainSpine.Skeleton.Data.FindAnimation(animName.ToString());
        return anim?.Duration ?? 0f;
    }

    public virtual void DelegateStartRewind(object args)
    {
        //isReversing = true;
    }
    public void OnCompleteRewind(object args)
    {

    }
}


public enum Skin
{
    Bow = 0,
    Normal,
    Shield1,
    Sword,
}

public enum Anim
{
    None = -1,
    Bow = 0,
    Die,
    Idle,
    Run,
    Sword
}
