using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] SpineController mainSpine;
    [SerializeField] WeaponSpine weaponSpine;
    [SerializeField] Skin weapon;

    private void Awake()
    {
        SetupWeapon(weapon);
    }

    private void Start()
    {
        PlayAnim(Anim.Idle);
    }
    public void SetupWeapon(Skin weaponSkin)
    {
        weaponSpine.SetSkin(weapon);
    }
    public void PlayAnim(Anim anim)
    {
        mainSpine.Play(anim);
        weaponSpine.Play(anim);
    }
}
