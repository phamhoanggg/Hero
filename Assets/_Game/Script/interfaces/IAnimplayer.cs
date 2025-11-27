using UnityEngine;

public interface IAnimplayer
{
    public void PlayAnim(Anim anim, bool loop = true);
    public GameObject GetRoot();
}
