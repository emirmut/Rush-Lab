using UnityEngine;

public class SignalButton : MonoBehaviour
{
    [Header("Visual Settings")]
    public Material litMaterial;
    public Material unlitMaterial;
    public Color litColor = Color.yellow;
    public Color unlitColor = Color.gray;

    [Header("Optional: Use Emission")]
    public bool useEmission = true;
    public float emissionIntensity = 2f;

    private Renderer buttonRenderer;
    private Material instanceMaterial;
    private Collider buttonCollider;
    private int buttonIndex;
    private System.Action onClickCallback;

    public void Initialize(int index, System.Action clickCallback)
    {
        buttonIndex = index;
        onClickCallback = clickCallback;

        buttonRenderer = GetComponent<Renderer>();
        buttonCollider = GetComponent<Collider>();

        if (buttonRenderer == null)
        {
            Debug.LogError($"SignalButton '{gameObject.name}' is missing a Renderer!");
            return;
        }


        if (unlitMaterial != null)
        {
            instanceMaterial = new Material(unlitMaterial);
        }
        else
        {
            instanceMaterial = new Material(buttonRenderer.sharedMaterial);
        }

        buttonRenderer.material = instanceMaterial;
        TurnOff();
    }

    public void TurnOn()
    {
        if (instanceMaterial == null) return;

        if (litMaterial != null)
        {

            buttonRenderer.material = litMaterial;
        }
        else
        {

            instanceMaterial.color = litColor;

            if (useEmission)
            {
                instanceMaterial.EnableKeyword("_EMISSION");
                instanceMaterial.SetColor("_EmissionColor", (Color)(litColor * Mathf.LinearToGammaSpace(emissionIntensity)));
            }

            buttonRenderer.material = instanceMaterial;
        }
    }

    public void TurnOff()
    {
        if (instanceMaterial == null) return;

        if (useEmission)
        {
            instanceMaterial.SetColor("_EmissionColor", Color.black);
            instanceMaterial.DisableKeyword("_EMISSION");
        }


        if (unlitMaterial != null)
        {

            buttonRenderer.material = unlitMaterial;
        }
        else
        {

            instanceMaterial.color = unlitColor;

            if (useEmission)
            {
                instanceMaterial.SetColor("_EmissionColor", Color.black);
            }

            buttonRenderer.material = instanceMaterial;
        }
    }

    public bool IsHit(Collider hitCollider)
    {
        return hitCollider == buttonCollider;
    }

    void OnDestroy()
    {
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
        }
    }
}