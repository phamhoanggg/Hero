using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public CheckPoint nextCheckPoint;
    public CheckPoint[] nextCandidates;
    public Transform TF;

    private void OnDrawGizmos()
    {
        if (nextCheckPoint != null)
        {
            Debug.DrawLine(TF.position, nextCheckPoint.TF.position, Color.red);
        }
    }
}
