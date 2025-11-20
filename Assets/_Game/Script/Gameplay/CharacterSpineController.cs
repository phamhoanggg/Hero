using Spine.Unity;
using UnityEngine;

public abstract class CharacterSpineController : MonoBehaviour
{
    public SkeletonGraphic mainSpine;
    public SkeletonGraphic[] subSpine;
    public bool InitRight = true;
    public Anim StartingAnim;

    private void Start()
    {
        mainSpine.initialFlipX = !InitRight;
        PlayAnim(StartingAnim, true);
    }

    public void PlayAnim(Anim anim, bool loop = false, float timeScale = 1)
    {
        mainSpine.AnimationState.ClearTrack(0);
        mainSpine.timeScale = timeScale;
        mainSpine.AnimationState.SetAnimation(0, anim.ToString(), loop);
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
