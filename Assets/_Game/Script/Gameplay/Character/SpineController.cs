using SharedModules.ED;
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

    public virtual void DelegateStartRewind(object args)
    {

    }
    private void FixedUpdate()
    {
        if (CoregameManager.Ins.IsReversing) mainSpine.Update(-Time.fixedDeltaTime);
        else mainSpine.Update(Time.fixedDeltaTime);
        mainSpine.ApplyAnimation();
    }

    /// <summary>
    /// Play a UI Spine animation by name with specified loop & speed.
    /// </summary>
    public void Play(Anim animName, bool loop = true, float timeScale = 1f)
    {
        mainSpine.initialFlipX = !InitRight;
        //mainSpine.AnimationState.ClearTracks();
        mainSpine.AnimationState.SetEmptyAnimation(0, 0f);
        mainSpine.timeScale = timeScale;

        // Start animation
        mainSpine.AnimationState.SetAnimation(0, animName.ToString(), loop);
    }

    public float GetAnimDuration(Anim animName)
    {
        Spine.Animation anim = mainSpine.Skeleton.Data.FindAnimation(animName.ToString());
        return anim?.Duration ?? 0f;
    }

    public void OnCompleteRewind(object args)
    {
        mainSpine.AnimationState.SetEmptyAnimation(0, 0f);
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
