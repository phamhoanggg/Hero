using UnityEngine;

public class Chest : MonoBehaviour
{
    public ShieldDirect ShieldDirection;
    [SerializeField] GameObject chestClose;
    [SerializeField] GameObject chestOpen;

    public void Open()
    {
        chestClose.SetActive(false);
        chestOpen.SetActive(true);
    }
}
