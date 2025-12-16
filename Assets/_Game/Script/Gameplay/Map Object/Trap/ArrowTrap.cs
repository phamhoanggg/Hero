using DG.Tweening;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] Arrow arrows;
    [SerializeField] Transform targetTf;
    [SerializeField] float speed;
    bool fired;
    public void Fire()
    {
        if (fired) return;
        
        fired = true;
        CoregameManager.Ins.listRewindEvent.Add(new("", () => fired = false));
        arrows.FlyToTarget(targetTf.position, speed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConst.TAG_PLAYER) || collision.CompareTag(GameConst.TAG_ENEMY)) {
            Fire();
        }
    }
}
