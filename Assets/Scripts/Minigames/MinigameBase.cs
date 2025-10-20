using UnityEngine;

public abstract class MinigameBase : MonoBehaviour
{
    [Header("Minigame Settings")]
    public Camera minigameCamera;
    public Vector2 popupSize = new Vector2(800, 600);
    public Transform minigameArea;

    [Header("UI Elements")]
    public GameObject[] minigameUIElements;

    protected bool isActive = false;
    protected MinigamePopup popupManager;

    // Static flag to check if any minigame is active
    public static bool IsAnyMinigameActive { get; private set; }

    [SerializeField] private GameObject _playerGO;
    [SerializeField] private PlayerInteraction _playerInteraction;
    private Ore _currentlyInteractedOre;

    [SerializeField] private ExplosionBar _explosionBar;

    public virtual void Initialize(MinigamePopup popup)
    {
        popupManager = popup;
        minigameArea.gameObject.SetActive(false);
        HideUI();
    }

    public virtual void StartMinigame(Ore ore)
    {
        Debug.Log($"Starting minigame: {gameObject.name}");

        _currentlyInteractedOre = ore;
        isActive = true;
        IsAnyMinigameActive = true;

        minigameArea.gameObject.SetActive(true);
        ShowUI();

        // Disable player controls
        DisablePlayerControls();

        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        popupManager.ShowMinigame(minigameCamera, popupSize);
        OnMinigameStart();
    }

    public virtual void EndMinigame(bool success)
    {
        isActive = false;
        IsAnyMinigameActive = false;

        OnMinigameEnd(success);
        HideUI();

        // Re-enable player controls
        EnablePlayerControls();

        // Lock cursor again for first-person gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        popupManager.HideMinigame();
        minigameArea.gameObject.SetActive(false);

        if (success)
        {
            _currentlyInteractedOre.OreExploded -= _playerInteraction.OnOreExplode;
            _currentlyInteractedOre.DestroyOre();
        }

        _explosionBar.ResetBar();
    }

    private void DisablePlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Disable the specific player components
            var playerMovement = player.GetComponent<PlayerMovement>();
            var playerCamera = player.GetComponentInChildren<PlayerCamera>();
            var playerInteraction = player.GetComponent<PlayerInteraction>();

            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                Debug.Log("Disabled PlayerMovement");
            }

            if (playerCamera != null)
            {
                playerCamera.enabled = false;
                Debug.Log("Disabled PlayerCamera");
            }

            if (playerInteraction != null)
            {
                playerInteraction.enabled = false;
                Debug.Log("Disabled PlayerInteraction");
            }
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure player is tagged as 'Player'");
        }
    }

    private void EnablePlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Re-enable the specific player components
            var playerMovement = player.GetComponent<PlayerMovement>();
            var playerCamera = player.GetComponentInChildren<PlayerCamera>();
            var playerInteraction = player.GetComponent<PlayerInteraction>();

            if (playerMovement != null)
            {
                playerMovement.enabled = true;
                Debug.Log("Enabled PlayerMovement");
            }

            if (playerCamera != null)
            {
                playerCamera.enabled = true;
                Debug.Log("Enabled PlayerCamera");
            }

            if (playerInteraction != null)
            {
                playerInteraction.enabled = true;
                Debug.Log("Enabled PlayerInteraction");
            }
        }
    }

    protected void ShowUI()
    {
        foreach (var uiElement in minigameUIElements)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(true);
            }
        }
    }

    protected void HideUI()
    {
        foreach (var uiElement in minigameUIElements)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(false);
            }
        }
    }

    protected abstract void OnMinigameStart();
    protected abstract void OnMinigameEnd(bool success);

    void Update()
    {
        if (isActive)
        {
            MinigameUpdate();
        }
    }

    protected abstract void MinigameUpdate();
}