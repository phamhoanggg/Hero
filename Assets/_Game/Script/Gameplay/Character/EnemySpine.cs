
using UnityEngine;

public class EnemySpine : SpineController
{
    [SerializeField] Enemy enemyRoot;
    [SerializeField] Collider2D col;

    Coroutine dieRoutine;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (collision.CompareTag(GameConst.TAG_DIE))
        {
            Debug.LogWarning("Enemy trigger die");
            col.enabled = false;
            CoregameManager.Ins.listRewindEvent.Add(new("Disable col enemy", () => col.enabled = true));
            dieRoutine = StartCoroutine(enemyRoot.Die());
        }
    }

    public override void DelegateStartRewind(object args)
    {
        base.DelegateStartRewind(args);
        if (dieRoutine != null)
        {
            StopCoroutine(dieRoutine);
            enemyRoot.PlayBackward(Anim.Die);
            //enemyRoot.PlayAnim(Anim.Idle, true, GetAnimDuration(Anim.Die) / CoregameManager.Ins.reverseRatio);
            dieRoutine = null;
        }
    }
}
