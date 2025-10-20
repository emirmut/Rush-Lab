using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [Header("Options Menu Hook")]
    [SerializeField] private OptionsMenu optionsMenu; // ← assign in Inspector

    private void Start()
    {
        // Wire buttons
        if (startButton) startButton.onClick.AddListener(OnStartButtonClicked);
        if (optionsButton) optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        if (quitButton) quitButton.onClick.AddListener(OnQuitButtonClicked);

        // In main menu we usually want the cursor visible & free
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnStartButtonClicked()
    {
        // Load your gameplay scene
        SceneManager.LoadScene("Main");
    }

    private void OnOptionsButtonClicked()
    {
        // Open the Options panel via your existing OptionsMenu API
        if (optionsMenu != null)
        {
            optionsMenu.OpenFromMainMenuButton();
        }
        else
        {
            Debug.LogWarning("OptionsMenu reference not set on MainMenu.");
        }
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
