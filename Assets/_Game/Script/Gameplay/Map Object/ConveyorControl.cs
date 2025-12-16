using UnityEngine;

public class ConveyorControl : MonoBehaviour
{
    [SerializeField] GameObject leftObj;
    [SerializeField] GameObject rightObj;
    [SerializeField] bool startRight;
    [SerializeField] Conveyor controlledConveyor;
    bool isRight;
    private void Start()
    {
        isRight = startRight;
        leftObj.SetActive(!isRight);
        rightObj.SetActive(isRight);
    }

    void ChangeDirect()
    {
        isRight = !isRight;
        leftObj.SetActive(!isRight);
        rightObj.SetActive(isRight);
        controlledConveyor?.ChangeDirect();

        CoregameManager.Ins.listRewindEvent.Add(new("Conveyor control change direct", () =>
        {
            isRight = !isRight;
            leftObj.SetActive(!isRight);
            rightObj.SetActive(isRight);
        }));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoregameManager.Ins.IsReversing) return;

        if (collision.CompareTag(GameConst.TAG_PLAYER))
        {
            PlayerMove.Ins.Stop();
            ChangeDirect();
            PlayerMove.Ins.Invoke(nameof(PlayerMove.Ins.ContinueMove), 0.25f);
        }
    }
}
