using System;

[Serializable]
public class CheckPointEndData
{
    public int RestartButton = 0;
    public int TotalDuration = 0;
    public int RestartLevel = 0;
    public int LoseLevel = 0;
    public int BoosterSpend = 0;

    public void Reset()
    {
        RestartButton = 0;
        TotalDuration = 0;
        RestartLevel = 0;
        LoseLevel = 0;
        BoosterSpend = 0;
    }
}

public class DCSessionData
{
    public int TotalDuration = 0;
    public int RetryCount = 0;

    public void Reset()
    {
        TotalDuration = 0;
        RetryCount = 0;
    }
}