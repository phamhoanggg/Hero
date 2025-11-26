
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
    public RectTransform GameplayHolder;
    public readonly float BASE_CAMERA_SIZE = 6.4f;
    public float reverseRatio;
    public List<EventCheckpoint> listRewindEvent;
    public float startgameStamp { get; private set; }
    public bool IsReversing { get; private set; }

    public List<Level> listLevel;
    public Level currentLevel;
    private void Start()
    {
        Application.targetFrameRate = 60;
        //Play();
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

        if (isDie) yield return new WaitForSeconds(2f / 3);
        IsReversing = true;
        if (isDie)
        {
            yield return new WaitForEndOfFrame();

            //PlayerMove.Ins.spine.Play(Anim.Die, false, - reverseRatio);
            yield return new WaitForSeconds(1f / 3);
        }
        StartCoroutine(PlayerMove.Ins.StartReverse());
        EventDispatcher.DispatchEvent(EventId.OnRewind);


        //StartCoroutine(ReverseCoroutine(startReverseTimestamp));
    }

    public void Win()
    {
        GamePanel.reverseButton.SetActive(false);
        GamePanel.playButton.SetActive(false);
        LoadSceneManager.Ins.LoadScene(SceneId.Game, () => { });
    }

    public void ShakeCamera()
    {
        //GameplayHolder.DOShakeAnchorPos(0.5f, strength: new Vector2(0f, 50));
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
