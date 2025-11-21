using Spine;
using Spine.Unity;
using UnityEngine;

public abstract class CharacterSpineController : MonoBehaviour
{
    public SkeletonGraphic mainSpine;
    public SkeletonGraphic[] subSpine;
    public bool InitRight = true;
    public Anim StartingAnim;

    private TrackEntry currentTrack;
    void Awake()
    {
        if (mainSpine == null)
            mainSpine = GetComponent<SkeletonGraphic>();
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(StartingAnim.ToString()))
            Play(StartingAnim, true, 1);
    }

    /// <summary>
    /// Play a UI Spine animation by name with specified loop & speed.
    /// </summary>
    public void Play(Anim animName, bool loop = true, float timeScale = 1f)
    {
        mainSpine.AnimationState.SetEmptyAnimation(0, 0);

        //// Make sure animation exists
        //Spine.Animation anim = mainSpine.Skeleton.Data.FindAnimation(animName.ToString());
        //if (anim == null)
        //{
        //    Debug.LogWarning($"[Spine UI] Animation '{animName}' not found!");
        //    return;
        //}

        // Start animation
        currentTrack = mainSpine.AnimationState.SetAnimation(0, animName.ToString(), loop);

        // Set speed
        currentTrack.TimeScale = timeScale;

        // Keep global timescale default (to avoid affecting all)
        mainSpine.timeScale = 1f;
    }

    /// <summary>
    /// Play an animation backwards with a custom speed.
    /// </summary>
    public void PlayReverse(Anim animName, float reverseSpeed = 1f)
    {
        mainSpine.AnimationState.ClearTrack(0);

        Spine.Animation anim = mainSpine.Skeleton.Data.FindAnimation(animName.ToString());
        if (anim == null)
        {
            Debug.LogWarning($"[Spine UI] Animation '{animName}' not found!");
            return;
        }

        // Play forward first (required to get TrackEntry)
        currentTrack = mainSpine.AnimationState.SetAnimation(0, animName.ToString(), false);

        // Move playback position to animation end
        currentTrack.TrackTime = anim.Duration;

        // Negative speed = backward play
        currentTrack.TimeScale = -Mathf.Abs(reverseSpeed);
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
