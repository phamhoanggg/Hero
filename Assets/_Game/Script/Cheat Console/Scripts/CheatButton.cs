using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Cheat
{
    public class CheatButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] RectTransform myTransform;
        [SerializeField] RectTransform canvasTransform;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] CheatConsole cheatConsole;
        [SerializeField] UnityEvent onClick;

        bool isDragging = false;
        IEnumerator moveCoroutine = null;
        IEnumerator fadeCoroutine = null;
        WaitForSeconds wait0P5 = new WaitForSeconds(1f);

        private void Awake()
        {
            if (cheatConsole.IsHideButton)
            {
                fadeCoroutine = FadeAnimation();
                StartCoroutine(fadeCoroutine);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            isDragging = true;
            canvasGroup.alpha = 1f;
            myTransform.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            myTransform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            SnapToEdge();
        }

        public void OnPointerDown(PointerEventData eventData) { }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging)
            {
                onClick?.Invoke();
            }
        }

        void SnapToEdge()
        {
            Vector2 snapPosition = FindSnapPosition();
            moveCoroutine = MoveToPosition(snapPosition);
            StartCoroutine(moveCoroutine);

            if (cheatConsole.IsHideButton)
            {
                fadeCoroutine = FadeAnimation();
                StartCoroutine(fadeCoroutine);
            }
        }

        Vector2 FindSnapPosition()
        {
            Vector2 myPosition = myTransform.anchoredPosition;
            Vector2 result = myPosition;
            Vector2 padding = 10f * Vector2.one;
            Vector2 corner = 0.5f * (canvasTransform.sizeDelta - myTransform.sizeDelta) - padding;

            if (myPosition.x < 0f)
            {
                corner.x = -corner.x;
            }

            if (myPosition.y < 0f)
            {
                corner.y = -corner.y;
            }

            if (Mathf.Abs(corner.x - myPosition.x) > Mathf.Abs(corner.y - myPosition.y))
            {
                result.y = corner.y;

                if (Mathf.Abs(result.x) > Mathf.Abs(corner.x))
                {
                    result.x = corner.x;
                }
            }
            else
            {
                result.x = corner.x;

                if (Mathf.Abs(result.y) > Mathf.Abs(corner.y))
                {
                    result.y = corner.y;
                }
            }

            return result;
        }

        IEnumerator MoveToPosition(Vector2 targetPosition)
        {
            float modifier = 0f;
            Vector2 initialPosition = myTransform.anchoredPosition;

            while (modifier < 1f)
            {
                modifier += 10f * Time.unscaledDeltaTime;
                myTransform.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, modifier);

                yield return null;
            }
        }

        IEnumerator FadeAnimation()
        {
            yield return wait0P5;

            float modifier = 0f;

            while (modifier < 1f)
            {
                modifier += 1f * Time.unscaledDeltaTime;
                canvasGroup.alpha = 1f - modifier;

                yield return null;
            }
        }

        public void ShowButtonTemporarily()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            canvasGroup.alpha = 1f;

            fadeCoroutine = FadeAnimation();
            StartCoroutine(fadeCoroutine);
        }
    }
}
