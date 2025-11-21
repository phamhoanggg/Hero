using DG.Tweening;
using UnityEngine;

public class RewindableObject : MonoBehaviour
{
    public Transform rootParent;
    public Vector2 rootPosition;
    public Vector3 rootEuler;
    public float StartTimeStamp_SinceGameStart;
    public float EndTimeStamp_SinceGameStart;

    private void OnDisable()
    {
        CoregameManager.Ins.OnRewind -= DelegateRewind;
    }
    public virtual void Start()
    {
        rootParent = transform.parent;
        rootPosition = GetComponent<RectTransform>().anchoredPosition;
        rootEuler = transform.localEulerAngles;
        CoregameManager.Ins.OnRewind += DelegateRewind;
    }

    public virtual void  DelegateRewind()
    {

    }
}
