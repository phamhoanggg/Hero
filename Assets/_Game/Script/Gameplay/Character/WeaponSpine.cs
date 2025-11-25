using System.Collections;
using UnityEngine;

public class WeaponSpine : SpineController
{
    [SerializeField] Anim attackAnim;
    [SerializeField] CircleCollider2D attackSensorCol;
    [SerializeField] int attackRange;
    [SerializeField] IAnimplayer animPlayer;
    [SerializeField] GameObject damageZone;
    public void SetWeapon(Skin weapon, Anim attackAnim)
    {
        mainSpine.initialSkinName = weapon.ToString();
        this.attackAnim = attackAnim;
        attackSensorCol.radius = attackRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConst.TAG_PLAYER))
        {
            Attack();
        }
    }

    public void Attack()
    {
        animPlayer.PlayAnim(attackAnim, false);
        if (attackAnim == Anim.Sword)
        {
            StartCoroutine(EnableDamageZone());
        }
    }

    IEnumerator EnableDamageZone()
    {
        yield return new WaitForSeconds(0.3f);
        damageZone.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        damageZone.SetActive(false);
    }
}
