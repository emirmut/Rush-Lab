using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameOverScreen GameOverScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
