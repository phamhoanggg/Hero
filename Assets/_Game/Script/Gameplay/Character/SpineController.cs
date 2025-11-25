using SharedModules.ED;
using Spine;
using Spine.Unity;
using UnityEngine;

public abstract class SpineController : MonoBehaviour
{
    public SkeletonGraphic mainSpine;
    public bool InitRight = true;

    private TrackEntry currentTrack;
    void Awake()
    {
        if (mainSpine == null)
            mainSpine = GetComponent<SkeletonGraphic>();
    }

    private void OnEnable()
    {
        EventDispatcher.RegisterListener(EventId.OnRewindCompleted, OnCompleteRewind);
    }

    private void OnDisable()
    {
        EventDispatcher.UnregisterListener(EventId.OnRewindCompleted, OnCompleteRewind);
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
        mainSpine.AnimationState.SetEmptyAnimation(0, 0);

        mainSpine.timeScale = timeScale;

        // Start animation
        mainSpine.AnimationState.SetAnimation(0, animName.ToString(), loop);

    }

    public void OnCompleteRewind(object args)
    {
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
