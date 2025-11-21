
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoregameManager : SingletonMonoBehaviour<CoregameManager>
{
    public GamePanel GamePanel;
    public readonly float BASE_CAMERA_SIZE = 6.4f;
    public ZoneSwitcher[] zones;
    public CheckPoint Door;
    public float reverseRatio;
    public List<EventCheckpoint> listRewindEvent;
    public Action OnRewind;
    public float startgameStamp { get; private set; }
    public bool IsReversing { get; private set; }
    public void Play()
    {
        PlayerMove.Ins.StartMove();
        foreach (var zone in zones) zone.gameObject.SetActive(false);
        listRewindEvent = new();
        startgameStamp = Time.realtimeSinceStartup;
        IsReversing = false;
    }

    public void Reverve()
    {
        if (IsReversing) return;

        PlayerMove.Ins.StartReverse();
        IsReversing = true;
        float startReverseTimestamp = Time.realtimeSinceStartup - startgameStamp;
        OnRewind?.Invoke();
        foreach (var ev in listRewindEvent)
        {

        }

        //StartCoroutine(ReverseCoroutine(startReverseTimestamp));
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
        foreach (var zone in zones) zone.gameObject.SetActive(true);
    }
    public List<CheckPoint> GenerateRouteForPlayer()
    {
        List<CheckPoint> route = new();
        foreach (var zone in zones)
        {
            CheckPoint checkPoint = zone.GetFirstCheckpoint();
            while (checkPoint != null)
            {
                route.Add(checkPoint);
                checkPoint = checkPoint.nextCheckPoint;
            }
        }

        route.Add(Door);
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
