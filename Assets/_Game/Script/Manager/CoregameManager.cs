
using DG.Tweening;
using SharedModules.ED;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoregameManager : SingletonMonoBehaviour<CoregameManager>
{
    public GamePanel GamePanel;
    public Transform shakeTf;
    public readonly float BASE_CAMERA_SIZE = 6.4f;
    public float reverseRatio;
    public List<EventCheckpoint> listRewindEvent;
    public float startgameStamp { get; private set; }
    public bool IsReversing { get; private set; }

    public List<Level> listLevel;
    public Level currentLevel;
    private void Start()
    {
        int levelIndex = DataManager.Ins.Data.LevelIndex;

        currentLevel = listLevel[levelIndex];
        listLevel[levelIndex].gameObject.SetActive(true);
    }

    public void Play()
    {
        EventDispatcher.DispatchEvent(EventId.OnStartMove);

        PlayerMove.Ins.StartMove();
        foreach (var zone in currentLevel.zones) zone.gameObject.SetActive(false);
        listRewindEvent = new();
        startgameStamp = Time.realtimeSinceStartup;
        IsReversing = false;
    }

    public IEnumerator Reverve(bool isDie)
    {
        if (IsReversing) yield break;
        GamePanel.reverseButton.SetActive(false);

        float dieAnimTime = PlayerMove.Ins.spine.GetAnimDuration(Anim.Die);
        if (isDie) yield return new WaitForSeconds(dieAnimTime);
        IsReversing = true;
        if (isDie)
        {
            yield return new WaitForEndOfFrame();

            PlayerMove.Ins.spine.PlayBackward(Anim.Die);
            StartCoroutine(PlayerMove.Ins.StartReverse());
            StartCoroutine(ReverseCoroutine(Time.realtimeSinceStartup - startgameStamp));
            EventDispatcher.DispatchEvent(EventId.OnRewind);
            yield return new WaitForSeconds(dieAnimTime / reverseRatio);
            PlayerMove.Ins.spine.PlayBackward(Anim.Run);
        }
        else
        {
            StartCoroutine(PlayerMove.Ins.StartReverse());
            StartCoroutine(ReverseCoroutine(Time.realtimeSinceStartup - startgameStamp));
            EventDispatcher.DispatchEvent(EventId.OnRewind);
        }
    }

    public void Win()
    {
        DataManager.Ins.Data.LevelIndex++;
        if (DataManager.Ins.Data.LevelIndex >= listLevel.Count) DataManager.Ins.Data.LevelIndex = 0;
        if (DataManager.Ins.Data.IsShowTut) DataManager.Ins.Data.IsShowTut = false;
        GamePanel.reverseButton.SetActive(false);
        GamePanel.playButton.SetActive(false);
        LoadSceneManager.Ins.LoadScene(SceneId.Game, () => { });
    }

    public void ShakeCamera()
    {
        Vector3 originPos = shakeTf.position;
        //shakeTf.DOShakePosition(0.25f, strength: new Vector2(0f, 50)).OnComplete(() => shakeTf.position = originPos);
    }
    IEnumerator ReverseCoroutine(float startReverse)
    {
        for (int i = listRewindEvent.Count - 1; i >= 0; i--)
        {
            var ev = listRewindEvent[i];
            float waitTime = startReverse - ev.triggerReverse_timeStamp;
            if (waitTime < 0) waitTime = 0;
            yield return new WaitForSeconds(waitTime / reverseRatio);
            ev.reverseAction?.Invoke();
            Debug.Log("Invoked: " + ev.eventName);
            startReverse = ev.triggerReverse_timeStamp;
        }
    }

    public void ReverseCompleted()
    {
        GamePanel.ReverseCompleted();
        IsReversing = false;
        EventDispatcher.DispatchEvent(EventId.OnRewindCompleted);
        foreach (var zone in currentLevel.zones) zone.gameObject.SetActive(true);
    }
    public List<CheckPoint> GenerateRouteForPlayer(CheckPoint start = null)
    {
        List<CheckPoint> route = new();
        if (start != null)
        {
            route.Add(start);
            while (route.Last().nextCheckPoint != null)
            {
                route.Add(route.Last().nextCheckPoint);
            }
        }

        foreach (var zone in currentLevel.zones)
        {
            CheckPoint checkPoint = zone.GetFirstCheckpoint();
            while (checkPoint != null)
            {
                route.Add(checkPoint);
                checkPoint = checkPoint.nextCheckPoint;
            }
        }

        return route;
    }
}

[Serializable]
public class EventCheckpoint
{
    public float triggerReverse_timeStamp;
    public Vector2 playerPosition;
    public string eventName;
    public Action reverseAction;
    public EventCheckpoint(string name, Action reverseAction)
    {
        this.triggerReverse_timeStamp = Time.realtimeSinceStartup - CoregameManager.Ins.startgameStamp;
        this.eventName = name;
        playerPosition = PlayerMove.Ins.PlayerTf.position;
        this.reverseAction = reverseAction;
    }
}
