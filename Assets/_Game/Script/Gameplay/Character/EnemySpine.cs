
using UnityEngine;

public class EnemySpine : SpineController
{
    [SerializeField] Enemy enemyRoot;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConst.TAG_DIE))
        {
            enemyRoot.PlayAnim(Anim.Die);
        }
    }
}
