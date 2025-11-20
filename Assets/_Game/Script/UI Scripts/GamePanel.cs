using UnityEngine;

public class GamePanel : MonoBehaviour
{
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject reverseButton;

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
    }

    public void ReverseButton()
    {
        CoregameManager.Ins.Reverve();
        reverseButton.SetActive(false);
    }
    #endregion
}
