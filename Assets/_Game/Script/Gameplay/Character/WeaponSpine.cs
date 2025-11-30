using System.Collections;
using SharedModules.ED;
using UnityEngine;

public class WeaponSpine : SpineController
{
    [SerializeField] Anim attackAnim;
    [SerializeField] BoxCollider2D attackSensorCol;
    [SerializeField] IAnimplayer animPlayer;
    [SerializeField] GameObject damageZone;
    [Space]
    [SerializeField] Arrow arrowPrefab;
    [SerializeField] Transform arrowSpawnPos;
    [SerializeField] float arrowSpeed;

    Coroutine attackCoroutine;
    bool attackAnimDone = true;

    public override void DelegateStartRewind(object args)
    {
        base.DelegateStartRewind(args);
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        if (!attackAnimDone)
        {
            animPlayer.PlayBackward(attackAnim);
            attackAnimDone = true;
        }
    }
    public void SetWeapon(Skin weapon, Anim attackAnim, int attackRange, IAnimplayer animPlayer)
    {
        attackAnimDone = true;
        mainSpine.Skeleton.SetSkin(weapon.ToString());
        this.attackAnim = attackAnim;
        attackSensorCol.size = new (attackRange, 50);
        attackSensorCol.offset = new Vector2(InitRight ? attackRange / 2 : -attackRange / 2, 36);
        this.animPlayer = animPlayer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;
        if (collision.transform.parent == transform.parent) return;
        if (animPlayer == null) return;

        if (collision.CompareTag(GameConst.TAG_PLAYER))
        {
            Debug.Log("AttackPlayer");
            attackCoroutine = StartCoroutine(Attack(false, collision.transform));
        }
        else if (collision.CompareTag(GameConst.TAG_ENEMY))
        {
            PlayerMove.Ins.Stop();
            attackCoroutine = StartCoroutine(Attack(true, collision.transform));
        }
    }

    public IEnumerator Attack(bool isPlayer, Transform target)
    {
        attackAnimDone = false;
        //attackSensorCol.enabled = false;
        //CoregameManager.Ins.listRewindEvent.Add(new("Disable sensor", () => attackSensorCol.enabled = true));
        animPlayer.PlayAnim(attackAnim, false);
        CoregameManager.Ins.listRewindEvent.Add(new("", () => animPlayer.PlayAnim(Anim.Idle)));
        float animTime = GetAnimDuration(attackAnim);
        if (attackAnim == Anim.Sword)
        {
            StartCoroutine(EnableDamageZone());
            yield return new WaitForSeconds(animTime);
        }
        else if (attackAnim == Anim.Bow)
        {
            float waitForArrowSpawnTime = 0.36f;
            yield return new WaitForSeconds(waitForArrowSpawnTime);
            Arrow arrow = Instantiate(arrowPrefab, arrowSpawnPos.position, Quaternion.identity, CoregameManager.Ins.currentLevel.transform);
            arrow.transform.localScale = new Vector3(InitRight ? 1 : -1, 1, 1);
            Vector2 targetPos = target.position;
            targetPos.y = arrow.transform.position.y;
            arrow.FlyToTarget(targetPos, arrowSpeed);
            yield return new WaitForSeconds(animTime - waitForArrowSpawnTime);
        }

        attackAnimDone = true;
        CoregameManager.Ins.listRewindEvent.Add(new("", () => animPlayer.PlayBackward(attackAnim)));
        if (CoregameManager.Ins.IsReversing) yield break;
        animPlayer.PlayAnim(Anim.Idle);
        if (isPlayer) PlayerMove.Ins.ContinueMove();
        //attackSensorCol.enabled = true;
    }

    IEnumerator EnableDamageZone()
    {
        yield return new WaitForSeconds(0.3f);
        damageZone.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        damageZone.SetActive(false);
    }
}
