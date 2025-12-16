using UnityEngine;

public interface IAnimplayer
{
    public void Stop();
    public void PlayAnim(Anim anim, bool loop = true, float delayTime = 0);
    public void PlayBackward(Anim anim, float startTrackTime = 1, bool loop = false);
    public GameObject GetRoot();
}
