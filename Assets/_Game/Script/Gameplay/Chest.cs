using UnityEngine;

public class Chest : RewindableObject
{
    public ShieldDirect ShieldDirection;
    [SerializeField] GameObject chestClose;
    [SerializeField] GameObject chestOpen;

    public void Open()
    {
        chestClose.SetActive(false);
        chestOpen.SetActive(true);
        CoregameManager.Ins.listRewindEvent.Add(new(Time.time - CoregameManager.Ins.startgameStamp, "Chest Open", () =>
        {
            Close();
        }));
    }

    public void Close()
    {
        chestClose.SetActive(true);
        chestOpen.SetActive(false);
    }
}
