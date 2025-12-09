using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PlayerMove : SingletonMonoBehaviour<PlayerMove>, IAnimplayer
{
    public List<CheckPoint> route = new();
    public PlayerSpine spine;
    public WeaponSpine weaponSpine;
    [SerializeField] Transform tf;
    [SerializeField] float speed;
    [SerializeField] CheckPoint startCheckPoint;
    public Transform TF => tf;
    public Transform PlayerTf => tf;
    [SerializeField] private Door door;
    int checkpointIndex;
    int reverseIndex;

    List<Tween> tweenMoveStack;
    float lastMoveTime;
    Vector2 defaultPos;

    private void Start()
    {
        if (door == null)
            Debug.LogError("NO DOORRRRR");
    }
    public void StartMove()
    {
        defaultPos = tf.position;
        route = CoregameManager.Ins.GenerateRouteForPlayer(startCheckPoint);
        if (route.Count == 0) route.Add(door.checkpoint);
        checkpointIndex = 0;
        tweenMoveStack = new();
        spine.StartMove();
        Move(checkpointIndex);
        PlayAnim(Anim.Run, true);
    }
    public void Move(int id)
    {
        float dis = Vector2.Distance(route[id].TF.position, tf.position);
        float time = dis / speed;
        Tween moveTween = TweenUtil.RewindableTween(tf.DOMove(route[id].TF.position, time).OnUpdate(() => lastMoveTime = Time.fixedTime), ReverseStepCompleted, MoveCompleted);
        int scaleX = (route[id].TF.position.x >= tf.position.x) ? 1 : -1;
        float prev_scaleX = tf.localScale.x;
        tf.localScale = new Vector3(scaleX, 1, 1);
        if (scaleX * prev_scaleX < 0)
            CoregameManager.Ins.listRewindEvent.Add(new("", () =>
            {
                tf.localScale = new Vector3(-scaleX, 1, 1);
            }));

        tweenMoveStack.Add(moveTween);
    }

    public void MoveCompleted()
    {
        if (checkpointIndex + 1 < route.Count)
        {
            checkpointIndex++;
            Move(checkpointIndex);
        }
        else
        {
            PlayAnim(Anim.Idle);
            if (door.Opened)
            {
                CoregameManager.Ins.Win();
                gameObject.SetActive(false);
                door.Invoke(nameof(door.Close), 0.15f);
            }
        }
    }
    public void Stop()
    {
        lastMoveTime = Time.fixedTime;
        Tween tween = tweenMoveStack.Last();
        tween.Pause();
        PlayAnim(Anim.Idle, true);
        //CoregameManager.Ins.listRewindEvent.Add(new("", () => PlayAnim(Anim.Run, true, -CoregameManager.Ins.reverseRatio)));
        CoregameManager.Ins.listRewindEvent.Add(new("", () =>
        {
            tweenMoveStack[reverseIndex].PlayBackwards();
            PlayAnim(Anim.Run, true);
        }));
    }
    public void ContinueMove()
    {
        CoregameManager.Ins.listRewindEvent.Add(new("", () =>
        {
            tweenMoveStack[reverseIndex].Pause();
            PlayAnim(Anim.Idle, true);
        }));
        //tweenMoveStack.Add(DOVirtual.Float(0, 1, Time.fixedTime - lastMoveTime, (float update) => { }).SetAutoKill(false).OnRewind(ReverseStepCompleted));
        PlayAnim(Anim.Run, true);
        Move(checkpointIndex);
    }

    public void SetWeapon(Weapon wp)
    {
        weaponSpine.SetWeapon(wp.weaponSkin, wp.attackAnim, wp.attackRange, this);
        wp.gameObject.SetActive(false);
        CoregameManager.Ins.listRewindEvent.Add(new("Equip weapon", () =>
        {
            wp.gameObject.SetActive(true);
            weaponSpine.SetWeapon(Skin.Normal, Anim.Bow, 0, null);
        }));
        //PlayAnim(Anim.Idle, true);
    }

    #region REVERSE
   
    public IEnumerator StartReverse()
    {
        float waitTime = Time.fixedTime - lastMoveTime;
        float reverseScale = CoregameManager.Ins.reverseRatio;

        yield return new WaitForSeconds(waitTime / reverseScale);

        door.Close();
        foreach (var tween in tweenMoveStack)
            tween.timeScale = reverseScale;


        reverseIndex = tweenMoveStack.Count - 1;
        Glitch.Ins.Play();
        PlayAnim(Anim.Run, true);
        tweenMoveStack[reverseIndex].PlayBackwards();
    }

    public void ReverseStepCompleted()
    {
        tweenMoveStack.RemoveAt(reverseIndex);

        reverseIndex--;
        if (reverseIndex >= 0 && (Vector2.Distance(tf.position, defaultPos) > 0.1f)) tweenMoveStack[reverseIndex].PlayBackwards();
        else 
        {
            ReverseCompleted();
        }
    }

    public void ReverseCompleted()
    {
        reverseIndex = -1;
        Glitch.Ins.ResetNoise();
        CoregameManager.Ins.ReverseCompleted();
        PlayAnim(Anim.Idle, true);
    }

    public void PlayAnim(Anim anim, bool loop = true, float delayTime = 0)
    {
        StartCoroutine(spine.Play(anim, loop, delayTime));
        StartCoroutine(weaponSpine.Play(anim, loop, delayTime));
        if (anim == Anim.Die) weaponSpine.OnParentDie();
    }

    public void PlayBackward(Anim anim)
    {
        spine.PlayBackward(anim);
        weaponSpine.PlayBackward(anim);
    }

    public GameObject GetRoot()
    {
        return gameObject;
    }
    #endregion
}

public enum ShieldDirect
{
    Horizontal = 0,
    Vertical = 1,
}
