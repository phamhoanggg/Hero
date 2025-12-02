using System.Collections;
using SharedModules.ED;
using UnityEngine;

public class WeaponSpine : SpineController
{
    Anim _attackAnim;
    IAnimplayer _animPlayer;
    Skin _weaponSkin;
    int _attackRange;

    [SerializeField] BoxCollider2D attackSensorCol;
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
            _animPlayer.PlayBackward(_attackAnim);
            attackAnimDone = true;
        }
    }
    public void SetWeapon(Skin weapon, Anim attackAnim, int attackRange, IAnimplayer animPlayer)
    {
        this._attackAnim = attackAnim;
        this._attackRange = attackRange;
        this._animPlayer = animPlayer;
        _weaponSkin = weapon;

        attackAnimDone = true;
        mainSpine.Skeleton.SetSkin(weapon.ToString());
        attackSensorCol.size = new (attackRange, 50);
        attackSensorCol.offset = new Vector2(InitRight ? attackRange / 2 : -attackRange / 2, 36);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;
        if (collision.transform.parent == transform.parent) return;
        if (_animPlayer == null) return;

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
        _animPlayer.PlayAnim(_attackAnim, false);
        CoregameManager.Ins.listRewindEvent.Add(new("", () => _animPlayer.PlayAnim(Anim.Idle)));
        float animTime = GetAnimDuration(_attackAnim);
        if (_attackAnim == Anim.Sword)
        {
            StartCoroutine(EnableDamageZone());
            yield return new WaitForSeconds(animTime);
        }
        else if (_attackAnim == Anim.Bow)
        {
            float waitForArrowSpawnTime = 0.36f;
            yield return new WaitForSeconds(waitForArrowSpawnTime);
            Arrow arrow = Instantiate(arrowPrefab, arrowSpawnPos.position, Quaternion.identity, CoregameManager.Ins.currentLevel.transform);
            arrow.transform.localScale = new Vector3(InitRight ? 1 : -1, 1, 1);
            Vector3 direct = target.position - arrow.transform.position;
            direct = new Vector2(direct.x, 0).normalized;
            Vector2 targetPos = arrow.transform.position + direct * 1000;

            arrow.FlyToTarget(targetPos, arrowSpeed);
            yield return new WaitForSeconds(animTime - waitForArrowSpawnTime);
        }

        attackAnimDone = true;
        CoregameManager.Ins.listRewindEvent.Add(new("", () => _animPlayer.PlayBackward(_attackAnim)));
        if (CoregameManager.Ins.IsReversing) yield break;

        mainSpine.Skeleton.SetSkin(Skin.Normal.ToString());
        attackSensorCol.size = Vector2.zero;
        _animPlayer.PlayAnim(Anim.Idle);
        CoregameManager.Ins.listRewindEvent.Add(new("", () => SetWeapon(_weaponSkin, _attackAnim, _attackRange, _animPlayer)));
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
