using DG.Tweening;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Arrow : MonoBehaviour
{
    [SerializeField] Transform tf;
    public void FlyToTarget(Transform targetTf, float speed)
    {
        float dis = Vector2.Distance(targetTf.position, tf.position);
        float time = dis / speed;
        tf.DOMove(targetTf.position, time).SetEase(Ease.Linear);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shield")) {
            tf.DOKill();
            tf.SetParent(collision.gameObject.transform);
        }
    }
}
