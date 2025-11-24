
using UnityEngine;

public class EnemySpine : SpineController
{
    [SerializeField] float attackRange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tag))
        {

        }
    }
}
