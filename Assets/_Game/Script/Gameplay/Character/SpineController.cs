using SharedModules.ED;
using Spine;
using Spine.Unity;
using UnityEngine;

public abstract class SpineController : MonoBehaviour
{
    public SkeletonGraphic mainSpine;
    public bool InitRight = true;

    public RectTransform RectTf;

    bool isReversing;
    void Awake()
    {
        if (mainSpine == null)
            mainSpine = GetComponent<SkeletonGraphic>();
        if (RectTf == null)
            RectTf = GetComponent<RectTransform>();
        mainSpine.UpdateTiming = UpdateTiming.ManualUpdate;
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

    private void Update()
    {
        if (isReversing) mainSpine.Update(-Time.deltaTime);
        else mainSpine.Update(Time.deltaTime);
        mainSpine.ApplyAnimation();
    }

    /// <summary>
    /// Play a UI Spine animation by name with specified loop & speed.
    /// </summary>
    public void Play(Anim animName, bool loop = true)
    {
        mainSpine.initialFlipX = !InitRight;
        //mainSpine.AnimationState.ClearTracks();
        mainSpine.AnimationState.SetEmptyAnimation(0, 0f);
        isReversing = CoregameManager.Ins.IsReversing;
        float timeScale = isReversing ? -CoregameManager.Ins.reverseRatio : 1f;
        mainSpine.timeScale = timeScale;

        // Start animation
        mainSpine.AnimationState.AddAnimation(0, animName.ToString(), loop, 0);
    }

    public float GetAnimDuration(Anim animName)
    {
        Spine.Animation anim = mainSpine.Skeleton.Data.FindAnimation(animName.ToString());
        return anim?.Duration ?? 0f;
    }

    public void SetReverse(bool isReverse)
    {
        isReversing = isReverse;
    }

    public virtual void DelegateStartRewind(object args)
    {
        isReversing = true;
    }
    public void OnCompleteRewind(object args)
    {
        //mainSpine.AnimationState.SetEmptyAnimation(0, 0f);
        isReversing = false;
        Play(Anim.Idle);
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
