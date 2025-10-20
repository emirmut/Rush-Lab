using UnityEngine;

public class MinigameInteractable : MonoBehaviour
{
    [Header("References")]
    public MinigameBase minigame;

    [SerializeField] public float interactDistance;
    
    [Header("UI")]
    public GameObject interactPrompt;
    
    private Transform playerTransform;
    private bool playerInRange = false;
    
    void Start()
    {
        Debug.Log($"MinigameInteractable Start on {gameObject.name}");
        
        if (minigame == null)
        {
            Debug.LogError($"Minigame reference is NULL on {gameObject.name}!");
        }
        else
        {
            Debug.Log($"Minigame assigned: {minigame.gameObject.name}");
        }
        
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }
    
    void Update()
    {        
        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange);
        
        // Handle interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player.transform;
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            playerInRange = distance <= interactDistance;

            Debug.Log($"Interact key pressed! Starting minigame: {minigame.gameObject.name}");

            Ore ore = playerTransform.GetComponentInChildren<Ore>();
            if (minigame != null && playerInRange && ore != null)
            {
                minigame.StartMinigame(ore);
            }
            else
            {
                Debug.LogError("Cannot start minigame - reference is null!");
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}