using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerHandle : MonoBehaviour
{
    [SerializeField] CanvasScaler scaler;
    private void Awake()
    {
        float baseRatio = 720f / 1080;
        float currentRatio = Screen.width * 1f / Screen.height;
        if (scaler == null) scaler = GetComponent<CanvasScaler>();
        scaler.matchWidthOrHeight = (baseRatio > currentRatio) ? 0 : 1;
    }

}
