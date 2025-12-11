using DG.Tweening;
using UnityEngine;

public class PlayerSpine : SpineController
{
    bool hasKey;
    private Key key;
    [SerializeField] Shield shield;

    public void StartMove()
    {
        hasKey = false;
        key = null;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (other.CompareTag(GameConst.TAG_DIE))
        {
            PlayerMove.Ins.TF.DOPause();
            VibrationManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
            CoregameManager.Ins.ShakeCamera();
            PlayerMove.Ins.PlayAnim(Anim.Die, false);
            CoregameManager.Ins.StartCoroutine(CoregameManager.Ins.Reverse(true));
        }
        else if (other.CompareTag(GameConst.TAG_CHEST))
        {
            if (!hasKey) return;

            PlayerMove.Ins.Stop();
            key.PlayPutInLockAnim(() =>
            {
                if (CoregameManager.Ins.IsReversing) return;
                Chest chest = other.GetComponent<Chest>();
                chest.Open();
                shield.GetShield(chest.ShieldDirection);
            });

        }
        else if (other.CompareTag(GameConst.TAG_KEY))
        {
            key = other.GetComponent<Key>();
            key.OnCollected();
            hasKey = true;
        }
        else if (other.CompareTag(GameConst.TAG_WEAPON))
        {
            Weapon wp = other.GetComponent<Weapon>();
            if (wp != null)
            {
                PlayerMove.Ins.SetWeapon(wp);
            }
        }

    }
}
