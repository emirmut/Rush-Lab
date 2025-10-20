using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public MinigamePopup popupManager;
    public CleaningMinigame CleaningMinigame;
    public SignalMinigame signalMinigame;
    public BasketballMinigame basketballMinigame;

    void Start()
    {
        Debug.Log("MinigameManager Starting...");

        if (popupManager == null)
        {
            Debug.LogError("PopupManager is NULL! Assign it in Inspector.");
            return;
        }

        // Initialize all minigames
        if (CleaningMinigame != null)
        {
            Debug.Log("Initializing Cleaning Minigame");
            CleaningMinigame.Initialize(popupManager);
        }

        if (signalMinigame != null)
        {
            Debug.Log("Initializing Signal Minigame");
            signalMinigame.Initialize(popupManager);
        }

        if (basketballMinigame != null)
        {
            Debug.Log("Initializing Basketball Minigame");
            basketballMinigame.Initialize(popupManager);
        }

        Debug.Log("MinigameManager initialization complete!");
    }
}