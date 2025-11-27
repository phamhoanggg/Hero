using SharedModules.ED;
using System;
using System.Collections;
using UnityEngine;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    const string Key = "Data";

    [SerializeField] float newDayCountdown;
    public float NewDayCountdown => newDayCountdown;
    [SerializeField] Data data;
    public Data Data => data;

    public bool IsLoaded { get; private set; }

    void OnApplicationPause(bool pause)
    {
        if (pause && IsLoaded)
        {
            SaveData();
        }
    }

    void OnApplicationQuit()
    {
        if (IsLoaded)
        {
            SaveData();
        }
    }

    #region SAVE & LOAD
    public void LoadData()
    {
        if (IsLoaded) return;

        if (!PlayerPrefs.HasKey(Key))
        {
            data = new Data();
        }
        else
        {
            string json = PlayerPrefs.GetString(Key);
            data = JsonUtility.FromJson<Data>(json);
        }

        IsLoaded = true;
        //FirebaseManager.Ins.SetAllUserProperty();
    }

    public void SaveData()
    {
        if (IsLoaded)
        {
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }
    }
    #endregion
}
