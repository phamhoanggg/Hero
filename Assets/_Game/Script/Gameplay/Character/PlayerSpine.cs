using DG.Tweening;
using UnityEngine;

public class PlayerSpine : SpineController
{
    bool hasKey;
    private Key key;
    [SerializeField] Shield shield;
    [SerializeField] Collider2D col2d;
    public void StartMove()
    {
        hasKey = false;
        key = null;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (other.CompareTag(GameConst.TAG_DIE) || other.CompareTag(GameConst.TAG_BEE))
        {
            PlayerMove.Ins.Stop();
            VibrationManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
            CoregameManager.Ins.ShakeCamera();
            col2d.enabled = false;
            CoregameManager.Ins.listRewindEvent.Add(new("", () => col2d.enabled = true));
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
        else if (other.CompareTag(GameConst.TAG_SHIELD))
        {
            if (other.gameObject == shield.shieldTf.gameObject) return;

            PlayerMove.Ins.Stop();
            other.gameObject.SetActive(false);
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                other.gameObject.SetActive(true);
            }));
            shield.GetShield(ShieldDirect.Horizontal);
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
        else if (other.CompareTag(GameConst.TAG_FALL))
        {
            PlayerMove.Ins.Stop();
            Vector3 targetPos = other.transform.GetChild(0).transform.position;
            PlayerMove.Ins.Move(targetPos);
        }else if (other.CompareTag(GameConst.TAG_TORCH))
        {
            Torch torch = other.GetComponent<Torch>();
            torch.OnCollected(RectTf);
        }
    }
}
