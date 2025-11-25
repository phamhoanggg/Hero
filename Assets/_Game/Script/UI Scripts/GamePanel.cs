using UnityEngine;

public class GamePanel : MonoBehaviour
{
    public GameObject playButton;
    public GameObject reverseButton;

    public void ReverseCompleted()
    {
        playButton.SetActive(true);
        reverseButton.SetActive(false);
    }
    #region UI Events
    public void PlayButton()
    {
        CoregameManager.Ins.Play();
        playButton.SetActive(false);
        reverseButton.SetActive(true);
        VibrationManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
    }

    public void ReverseButton()
    {
        StartCoroutine(CoregameManager.Ins.Reverve(false));
        reverseButton.SetActive(false);
        VibrationManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
    }
    #endregion
}
