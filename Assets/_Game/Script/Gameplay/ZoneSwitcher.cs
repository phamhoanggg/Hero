using DG.Tweening;
using Spine;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoneSwitcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    [SerializeField] int optionAmount = 2;
    [SerializeField] RectTransform scrollRect;
    [SerializeField] float scrollSpeed = 1;
    [SerializeField] AnimationCurve snapTweenCurve;
    [SerializeField] CheckPoint[] entryPoints;

    int currentOption = 0;
    bool isScrolling;
    Vector2 pre_pos;
    int lastDirect = 1;
    float lastMoveTime;
    public void OnPointerDown(PointerEventData eventData)
    {
        isScrolling = true;
        pre_pos = eventData.position;
        lastMoveTime = Time.time;
    }

    private void Update()
    {
        if (isScrolling)
        {
            float deltaY = Input.mousePosition.y - pre_pos.y;
            scrollRect.anchoredPosition += Vector2.up * deltaY * scrollSpeed;
            pre_pos = Input.mousePosition;
            lastDirect = deltaY < 0 ? 1 : -1;
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {           
        if (isScrolling) lastMoveTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.time - lastMoveTime < 0.1f)
        {
            currentOption += lastDirect;
            currentOption = Mathf.Clamp(currentOption, 0, optionAmount - 1);
        }

        SnappingToTab(currentOption);
        isScrolling = false;
    }

    public void SnappingToTab(int tabId)
    {
        scrollRect.DOAnchorPosY(tabId * -1280, 0.65f).SetEase(snapTweenCurve);
    }

    public CheckPoint GetFirstCheckpoint()
    {
        if (entryPoints.Length == 0) return null;

        return entryPoints[currentOption];
    }
}
