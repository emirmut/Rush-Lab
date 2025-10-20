using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Ore _oreInTriggerRange;
    private Ore _currentlyInteractedOre;
    private bool _hasOreAttached;

    [SerializeField] private InteractionIndicator _interactionIndicatorUI;
    [SerializeField] private ExplosionBar _explosionBar;

    private void Update()
    {   
        if (Input.GetKeyDown(KeyCode.E) && _interactionIndicatorUI.gameObject.activeInHierarchy)
        {
            if (_hasOreAttached == false)
            {
                AttachOre();
            }
            else
            {
                DetachOre();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ore") && _hasOreAttached == false)
        {
            _oreInTriggerRange = other.gameObject.GetComponentInParent<Ore>();
            _interactionIndicatorUI.Initialize(_hasOreAttached);
        }

        if (other.gameObject.CompareTag("OreDeliveryPlace") && _hasOreAttached)
        {
            _interactionIndicatorUI.Initialize(_hasOreAttached);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ore") && _hasOreAttached == false)
        {
            _oreInTriggerRange = null;
            _interactionIndicatorUI.DeInitialize();
        }

        if (other.gameObject.CompareTag("OreDeliveryPlace") && _hasOreAttached)
        {
            _interactionIndicatorUI.DeInitialize();
        }
    }

    public void AttachOre()
    {
        _interactionIndicatorUI.DeInitialize();
        _hasOreAttached = true;
        _currentlyInteractedOre = _oreInTriggerRange;
        _currentlyInteractedOre.AttachOreToPlayer(transform);
        _currentlyInteractedOre.OreExploded += OnOreExplode;
        _explosionBar.Initialize(0f, 60f);
    }

    private void DetachOre()
    {
        _hasOreAttached = false;
        _interactionIndicatorUI.DeInitialize();
        // _currentlyInteractedOre.OreExploded -= OnOreExplode;
        // _currentlyInteractedOre.DestroyOre();
    }

    public void OnOreExplode()
    {
        _currentlyInteractedOre.DestroyOre();
        _oreInTriggerRange = null;
        _hasOreAttached = false;
        _interactionIndicatorUI.gameObject.SetActive(false);
    }
}
