using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("Attrributes")]
    private const float _maxHealth = 100f;
    public float Health;

    [Space]

    [Header("UI")]
    [SerializeField] private HealthBar _healthBar;

    public bool IsDead => Health <= 0;

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

    public void UpdateHealth(float amount)
    {
        Health += amount;
        _healthBar.UpdateBar(Health, _maxHealth);

        if (IsDead)
        {
            Die();
        }
    }
    
    private void Die()
    {
        UIManager.Instance.GameOverScreen.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
