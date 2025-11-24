
using UnityEngine;

public class EnemySpine : CharacterSpineController
{
    [SerializeField] float attackRange;
    [SerializeField] CircleCollider2D attackSensorCol;
    [SerializeField] Anim attackAnim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tag))
        {

        }
    }
}
