using UnityEngine;

public interface IAnimplayer
{
    public void PlayAnim(Anim anim, bool loop = true, float timeScale = 1);
    public GameObject GetRoot();
}
