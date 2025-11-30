using System.Collections;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

public class TestControlSpine : MonoBehaviour
{
    public SkeletonGraphic mainSpine;
    public bool InitRight = true;

    public RectTransform RectTf;

    public bool isReversing;
    void Awake()
    {
        if (mainSpine == null)
            mainSpine = GetComponent<SkeletonGraphic>();
        if (RectTf == null)
            RectTf = GetComponent<RectTransform>();
        //mainSpine.UpdateTiming = UpdateTiming.ManualUpdate;
    }

    //private void Update()
    //{
    //    if (isReversing) mainSpine.Update(- 2 * Time.deltaTime);
    //    else mainSpine.Update(Time.deltaTime);
    //    mainSpine.ApplyAnimation();
    //}

    /// <summary>
    /// Play a UI Spine animation by name with specified loop & speed.
    /// </summary>
    [Button]
    public IEnumerator Play(Anim animName, bool loop = true, float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);
        mainSpine.initialFlipX = !InitRight;
        //mainSpine.AnimationState.ClearTracks();
        mainSpine.AnimationState.SetEmptyAnimation(0, 0.2f);
        float timeScale = isReversing ? -2 : 1f;
        mainSpine.timeScale = timeScale;

        // Start animation
        mainSpine.AnimationState.AddAnimation(0, animName.ToString(), loop, 0);
    }

    [Button]
    public void PlayBackward(Anim animName, bool loop = true)
    {
        mainSpine.initialFlipX = !InitRight;
        float timeScale = -2;

        // Start animation
        var trackEntry = mainSpine.AnimationState.SetAnimation(0, animName.ToString(), loop);
        trackEntry.TimeScale = timeScale;
        trackEntry.TrackTime = trackEntry.Animation.Duration;
        StartCoroutine(Play(Anim.Idle, true, GetAnimDuration(Anim.Idle) / 2 - 0.2f));
    }
    public float GetAnimDuration(Anim animName)
    {
        Spine.Animation anim = mainSpine.Skeleton.Data.FindAnimation(animName.ToString());
        return anim?.Duration ?? 0f;
    }

    public void OnCompleteRewind(object args)
    {
        //mainSpine.AnimationState.SetEmptyAnimation(0, 0f);
        isReversing = false;
        Play(Anim.Idle);
    }
}