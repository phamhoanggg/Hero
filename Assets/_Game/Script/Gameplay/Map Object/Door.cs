using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject open;
    public bool Opened;
    public CheckPoint checkpoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Open();
        }
    }

    public void Open()
    {
        open.SetActive(true);
        Opened = true;
    }

    public void Close()
    {
        open.SetActive(false);
        Opened = false;
    }
}
