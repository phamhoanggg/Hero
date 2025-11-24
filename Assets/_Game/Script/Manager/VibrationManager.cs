using MoreMountains.NiceVibrations;
using UnityEngine;

public static class VibrationManager
{
    const string Key = "Vibration";

    static bool enabled = true;
    public static bool Enabled => enabled;

    public static void Init()
    {
        enabled = PlayerPrefs.GetInt(Key, 1) == 1;
        ReloadVibration();
    }

    public static void ToggleVibration()
    {
        enabled = !enabled;
        PlayerPrefs.SetInt(Key, enabled ? 1 : 0);
        ReloadVibration();
    }

    static void ReloadVibration()
    {
        MMVibrationManager.SetHapticsActive(enabled);
    }

    public static void Vibrate(HapticTypes type = HapticTypes.MediumImpact)
    {
        if (enabled)
        {
            MMVibrationManager.Haptic(type);
        }
    }
}
