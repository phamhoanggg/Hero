using UnityEngine;

public class Weapon_Arrow : RewindableObject, IEquiptable
{
    public Skin weaponSkin;
    public Anim attackAnim;
    public float attackRange;
    public CircleCollider2D sensorRange;

    public void Init()
    {

    }
    public void Equipped()
    {
        
    }
}
