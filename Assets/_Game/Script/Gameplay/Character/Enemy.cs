using UnityEngine;

public class Enemy : MonoBehaviour, IAnimplayer
{
    [SerializeField] SpineController mainSpine;
    [SerializeField] WeaponSpine weaponSpine;
    [SerializeField] Skin weapon;
    [SerializeField] Anim attackAnim;

    private void Awake()
    {
        SetupWeapon();
    }

    private void Start()
    {
        PlayAnim(Anim.Idle);
    }
    public void SetupWeapon()
    {
        weaponSpine.SetWeapon(weapon, attackAnim);
    }
    public void PlayAnim(Anim anim, bool loop = true, float timeScale = 1)
    {
        mainSpine.Play(anim, loop, timeScale);
        weaponSpine.Play(anim, loop, timeScale);
    }
}
