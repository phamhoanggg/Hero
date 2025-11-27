
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    [Header("LEVEL")]
    public int LevelIndex;
    public int LevelCheckpoint;
    public CheckPointEndData endData;

    [Header("TUTORIALS")]
    public bool IsShowTut;
    
    public Data()
    {
        LevelIndex = 0;
        IsShowTut = true;
    }
}
