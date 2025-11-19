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
