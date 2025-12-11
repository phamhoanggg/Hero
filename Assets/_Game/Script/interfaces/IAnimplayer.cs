using UnityEngine;

public interface IAnimplayer
{
    public void PlayAnim(Anim anim, bool loop = true, float delayTime = 0);
    public void PlayBackward(Anim anim, float startTrackTime = 1);
    public GameObject GetRoot();
}
