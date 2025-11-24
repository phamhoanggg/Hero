using DG.Tweening;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] Arrow arrows;
    [SerializeField] Transform targetTf;
    [SerializeField] float speed;
    public void Fire()
    {
        arrows.FlyToTarget(targetTf, speed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            Fire();
        }
    }
}
