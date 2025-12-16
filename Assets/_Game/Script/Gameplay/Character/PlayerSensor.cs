using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    [SerializeField] Enemy enemyParent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;
        if (!enemyParent.canMove) return;
        if (collision.CompareTag(GameConst.TAG_PLAYER))
        {
            enemyParent.Move(new(collision.transform.position.x, enemyParent.transform.position.y));
        }
    }
}
