using System;
using System.Collections;
using UnityEngine;

public class EnemyKey : Enemy
{
    [SerializeField] Key key;

    public override IEnumerator Die(Action onCompleted = null)
    {
        key.OnRelease();
        return base.Die(onCompleted);
    }
}
