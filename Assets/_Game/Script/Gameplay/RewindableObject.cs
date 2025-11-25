using DG.Tweening;
using SharedModules.ED;
using UnityEngine;

public class RewindableObject : MonoBehaviour
{
    public Transform rootParent;
    public Vector2 rootPosition;
    public Vector3 rootEuler;
    public float StartTimeStamp_SinceGameStart;
    public float EndTimeStamp_SinceGameStart;

    private void OnEnable()
    {
        EventDispatcher.RegisterListener(EventId.OnRewind, DelegateRewind);
    }

    private void OnDisable()
    {
        EventDispatcher.UnregisterListener(EventId.OnRewind, DelegateRewind);

    }
    public virtual void Start()
    {
        rootParent = transform.parent;
        rootPosition = GetComponent<RectTransform>().anchoredPosition;
        rootEuler = transform.localEulerAngles;
    }

    public virtual void  DelegateRewind(object args)
    {

    }
}
