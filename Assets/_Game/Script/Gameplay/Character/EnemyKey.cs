using System.Collections;
using UnityEngine;

public class EnemyKey : Enemy
{
    [SerializeField] Key key;

    public override IEnumerator Die()
    {
        key.OnRelease();
        return base.Die();
    }
}
