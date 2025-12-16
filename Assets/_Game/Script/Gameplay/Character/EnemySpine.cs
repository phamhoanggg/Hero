
using UnityEngine;

public class EnemySpine : SpineController
{
    [SerializeField] Enemy enemyRoot;
    [SerializeField] Collider2D col;

    Coroutine dieRoutine;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (collision.CompareTag(GameConst.TAG_DIE) || collision.CompareTag(GameConst.TAG_BEE))
        {
            Debug.LogWarning("Enemy trigger die");
            col.enabled = false;
            CoregameManager.Ins.listRewindEvent.Add(new("Disable col enemy", () => col.enabled = true));
            enemyRoot.Stop();
            dieRoutine = StartCoroutine(enemyRoot.Die(() => dieRoutine = null));
        }

        if (collision.CompareTag(GameConst.TAG_FALL))
        {
            enemyRoot.Stop();
            Vector3 targetPos = collision.transform.GetChild(0).transform.position;
            enemyRoot.Move(targetPos, false);
        }
    }

    public override void DelegateStartRewind(object args)
    {
        base.DelegateStartRewind(args);
        if (dieRoutine != null)
        {
            if (!enemyRoot.IsDead) 
            {
                StopCoroutine(dieRoutine);
                enemyRoot.PlayBackward(Anim.Die);
            }
            
            dieRoutine = null;
        }
    }
}
