using UnityEngine;

public class WeaponSpine : SpineController
{
    [SerializeField] Anim attackAnim;
    [SerializeField] CircleCollider2D attackSensorCol;
    [SerializeField] int attackRange;
    public void SetSkin(Skin weapon)
    {
        mainSpine.initialSkinName = weapon.ToString();
        attackAnim = (Anim)weapon;
        attackSensorCol.radius = attackRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public void Attack()
    {
        
    }
}
