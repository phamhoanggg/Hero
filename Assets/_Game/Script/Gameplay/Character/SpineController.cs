using Spine;
using Spine.Unity;
using UnityEngine;

public abstract class SpineController : MonoBehaviour
{
    public SkeletonGraphic mainSpine;
    public bool InitRight = true;
    public Anim StartingAnim;

    private TrackEntry currentTrack;
    void Awake()
    {
        if (mainSpine == null)
            mainSpine = GetComponent<SkeletonGraphic>();
    }

    /// <summary>
    /// Play a UI Spine animation by name with specified loop & speed.
    /// </summary>
    public void Play(Anim animName, bool loop = true, float timeScale = 1f)
    {
        mainSpine.initialFlipX = !InitRight;
        mainSpine.AnimationState.SetEmptyAnimation(0, 0);

        // Start animation
        mainSpine.AnimationState.SetAnimation(0, animName.ToString(), loop);

        // Set speed
        //currentTrack.TimeScale = timeScale;

        // Keep global timescale default (to avoid affecting all)
        mainSpine.timeScale = 1f;
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
