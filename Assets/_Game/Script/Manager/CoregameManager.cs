
using System.Collections.Generic;

public class CoregameManager : SingletonMonoBehaviour<CoregameManager>
{
    public GamePanel GamePanel;
    public readonly float BASE_CAMERA_SIZE = 6.4f;
    public ZoneSwitcher[] zones;
    public CheckPoint Door;

    public void Play()
    {
        PlayerMove.Ins.StartMove();
        foreach (var zone in zones) zone.gameObject.SetActive(false);
    }

    public void Reverve()
    {
        PlayerMove.Ins.StartReverse();
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
