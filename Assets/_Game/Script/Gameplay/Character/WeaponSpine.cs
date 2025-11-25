using System.Collections;
using UnityEngine;

public class WeaponSpine : SpineController
{
    [SerializeField] Anim attackAnim;
    [SerializeField] CircleCollider2D attackSensorCol;
    [SerializeField] IAnimplayer animPlayer;
    [SerializeField] GameObject damageZone;
    public void SetWeapon(Skin weapon, Anim attackAnim, int attackRange, IAnimplayer animPlayer)
    {
        mainSpine.Skeleton.SetSkin(weapon.ToString());
        this.attackAnim = attackAnim;
        attackSensorCol.radius = attackRange;
        this.animPlayer = animPlayer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;
        if (collision.gameObject == transform.parent.gameObject) return;
        if (animPlayer == null) return;

        if (collision.CompareTag(GameConst.TAG_PLAYER))
        {
            StartCoroutine(Attack(false));
        }
        else if (collision.CompareTag(GameConst.TAG_ENEMY))
        {
            PlayerMove.Ins.Stop();
            StartCoroutine(Attack(true));
        }
    }

    public IEnumerator Attack(bool isPlayer)
    {
        animPlayer.PlayAnim(attackAnim, false);
        if (attackAnim == Anim.Sword)
        {
            StartCoroutine(EnableDamageZone());
        }
        else if (attackAnim == Anim.Bow)
        {

        }
        float animTime = GetAnimDuration(attackAnim.ToString());
        yield return new WaitForSeconds(animTime);
        Play(Anim.Idle, true);
        if (isPlayer) PlayerMove.Ins.ContinueMove();
    }

    IEnumerator EnableDamageZone()
    {
        yield return new WaitForSeconds(0.3f);
        damageZone.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        damageZone.SetActive(false);
    }
}
