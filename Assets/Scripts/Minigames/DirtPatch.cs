using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DirtPatch : MonoBehaviour
{
    [Header("Dirt Settings")]
    public float cleaningThreshold = 1f;

    private float currentDirtAmount;
    private Renderer dirtRenderer;
    private Material dirtMaterial;
    private bool isCleaned = false;

    public bool IsCleaned => isCleaned;

    void Awake()
    {
        InitializeMaterial();
    }

    private void InitializeMaterial()
    {
        if (dirtMaterial != null) return;

        dirtRenderer = GetComponent<Renderer>();
        if (dirtRenderer == null)
        {
            Debug.LogError($"DirtPatch '{gameObject.name}' is missing a Renderer component!");
            return;
        }

      
        dirtMaterial = new Material(dirtRenderer.sharedMaterial);
        dirtRenderer.material = dirtMaterial;
    }

    public void Reset()
    {
     
        InitializeMaterial();

        if (dirtMaterial == null)
        {
            Debug.LogError($"DirtPatch '{gameObject.name}' has no material!");
            return;
        }

        currentDirtAmount = cleaningThreshold;
        isCleaned = false;

        Color color = dirtMaterial.color;
        color.a = 1f;
        dirtMaterial.color = color;

        gameObject.SetActive(true);
    }

  
    public bool Clean(float amount)
    {
        if (isCleaned) return false;

        if (dirtMaterial == null)
        {
            Debug.LogError($"DirtPatch '{gameObject.name}' material is null during cleaning!");
            return false;
        }

        currentDirtAmount -= amount;

      
        float cleanPercent = Mathf.Clamp01(currentDirtAmount / cleaningThreshold);
        Color color = dirtMaterial.color;
        color.a = cleanPercent;
        dirtMaterial.color = color;

     
        if (currentDirtAmount <= 0f && !isCleaned)
        {
            isCleaned = true;
            gameObject.SetActive(false);
            return true;
        }

        return false;
    }

    void OnDestroy()
    {
    
        if (dirtMaterial != null)
        {
            Destroy(dirtMaterial);
        }
    }
}