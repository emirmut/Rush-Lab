using UnityEngine;

public class NavigationPuzzleMinigame : MinigameBase
{
    [Header("Puzzle Settings")]
    public GameObject element;
    public Transform startPosition;
    public Transform targetContainer;
    public float moveSpeed = 2f;
    public Transform[] seesawObstacles;

    private Rigidbody elementRb;
    private bool elementFallen = false;

    protected override void OnMinigameStart()
    {
        elementRb = element.GetComponent<Rigidbody>();
        element.transform.position = startPosition.position;
        element.transform.rotation = startPosition.rotation;
        elementRb.isKinematic = false;
        elementFallen = false;
    }

    protected override void MinigameUpdate()
    {
        if (elementFallen) return;

        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        elementRb.MovePosition(elementRb.position + movement);

       
        if (element.transform.position.y < -5f)
        {
            elementFallen = true;
            EndMinigame(false);
        }
    }

    public void OnElementReachedTarget()
    {
        EndMinigame(true);
    }

    protected override void OnMinigameEnd(bool success)
    {
        Debug.Log("Navigation puzzle " + (success ? "completed!" : "failed!"));
    }
}