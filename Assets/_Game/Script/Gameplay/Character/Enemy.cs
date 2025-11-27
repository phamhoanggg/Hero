using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IAnimplayer
{
    [SerializeField] SpineController mainSpine;
    [SerializeField] WeaponSpine weaponSpine;
    [SerializeField] Skin weapon;
    [SerializeField] Anim attackAnim;
    [SerializeField] int attackRange;

    public bool IsDead {  get; private set; }
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
        weaponSpine.SetWeapon(weapon, attackAnim, attackRange, this);
    }
    public void PlayAnim(Anim anim, bool loop = true)
    {
        mainSpine.Play(anim, loop);
        weaponSpine.Play(anim, loop);
    }

    public GameObject GetRoot()
    {
        return gameObject;
    }

    public IEnumerator Die() {
        PlayAnim(Anim.Die, false);
        IsDead = true;
        CoregameManager.Ins.listRewindEvent.Add(new("", () => IsDead = false));
        yield return new WaitForSeconds(mainSpine.GetAnimDuration(Anim.Die));
        gameObject.SetActive(false);
        CoregameManager.Ins.listRewindEvent.Add(new("", () => gameObject.SetActive(true)));
    }
}
