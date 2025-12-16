using UnityEngine;

public class InvertCheckPoint : MonoBehaviour
{
    public InvertCheckPoint next;
    public Transform TF;
    public bool IsLastPoint;
    private void OnDrawGizmos()
    {
        if (next != null)
        {
            Debug.DrawLine(TF.position, next.TF.position, Color.blue);
        }
    }
}
