using UnityEngine;
using UnityEngine.UI;

public class CleaningMinigame : MinigameBase
{
    [Header("Cleaning Settings")]
    public Slider cleaningProgress;
    public GameObject oreObject;
    public DirtPatch[] dirtPatches; // Custom class for dirt management
    public float cleaningRadius = 0.3f;
    public float cleaningSpeed = 2f;
    public LayerMask cleanableLayer; // Layer for ore and dirt

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;

    // --- Audio (Cleaning) ---
    [Header("Audio")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip sfxBroomLoop;   // soft looping scrub
    [SerializeField] private AudioClip sfxPatchClean;  // short 'ping' when a patch is fully cleaned
    [SerializeField] private AudioClip sfxComplete;    // all patches cleaned
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.6f;
    private bool broomLoopPlaying = false;



    private int patchesCleaned = 0;
    private int lastCleanCount = 0;

    public override void Initialize(MinigamePopup popup)
    {
        base.Initialize(popup);
        Debug.Log($"CleaningMinigame initialized with {dirtPatches.Length} dirt patches");

        // Ensure AudioSource exists & is 2D
        if (sfx == null) sfx = gameObject.GetComponent<AudioSource>();
        if (sfx == null) sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f;

        // Prepare loop channel characteristics (no reverb, steady volume)
        sfx.loop = false; // default; we'll toggle loop only while cleaning


        // Make sure camera is looking at the ore
        if (minigameCamera != null && oreObject != null)
        {
            // Calculate optimal camera position
            Vector3 orePos = oreObject.transform.position;
            Vector3 cameraOffset = new Vector3(0, 2, 5); // 5 units back, 2 units up
            minigameCamera.transform.position = orePos + cameraOffset;
            minigameCamera.transform.LookAt(oreObject.transform);

            Debug.Log($"Cleaning Camera Setup:");
            Debug.Log($"  Camera Position: {minigameCamera.transform.position}");
            Debug.Log($"  Ore Position: {oreObject.transform.position}");
            Debug.Log($"  Distance: {Vector3.Distance(minigameCamera.transform.position, oreObject.transform.position)}");
            Debug.Log($"  Camera Forward: {minigameCamera.transform.forward}");
        }
    }


    protected override void OnMinigameStart()
    {
        Debug.Log("CleaningMinigame OnMinigameStart called");

        patchesCleaned = 0;

        if (cleaningProgress != null)
        {
            cleaningProgress.value = 0;
        }
        else
        {
            Debug.LogWarning("Cleaning progress slider is null!");
        }

        if (dirtPatches == null || dirtPatches.Length == 0)
        {
            Debug.LogError("No dirt patches assigned!");
            return;
        }

        foreach (var patch in dirtPatches)
        {
            if (patch != null)
            {
                patch.Reset();
            }
            else
            {
                Debug.LogWarning("Null dirt patch found in array!");
            }
        }

        Debug.Log("CleaningMinigame started successfully");
    }

    protected override void MinigameUpdate()
    {
        HandleOreRotation();
        HandleCleaning();
    }

    private void HandleOreRotation()
    {
        if (oreObject == null) return;

        float horizontalRotation = 0f;
        float verticalRotation = 0f;

        if (Input.GetKey(KeyCode.A))
            horizontalRotation = -rotationSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D))
            horizontalRotation = rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
            verticalRotation = -rotationSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            verticalRotation = rotationSpeed * Time.deltaTime;

        if (horizontalRotation != 0f || verticalRotation != 0f)
        {
            oreObject.transform.Rotate(Vector3.up, horizontalRotation, Space.World);
            oreObject.transform.Rotate(minigameCamera.transform.right, verticalRotation, Space.World);
        }
    }

    private void HandleCleaning()
    {
        if (!Input.GetMouseButton(0)) return;

        
        Vector2 localPoint;
        RectTransform rawImageRect = popupManager.minigameDisplay.GetComponent<RectTransform>(); 

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rawImageRect,
                Input.mousePosition,
                popupManager.GetComponentInParent<Canvas>().worldCamera,
                out localPoint))
        {
            return;
        }

       
        Rect rect = rawImageRect.rect;
        float normalizedX = (localPoint.x - rect.x) / rect.width;
        float normalizedY = (localPoint.y - rect.y) / rect.height;

        Vector3 viewportPoint = new Vector3(normalizedX, normalizedY, 0f);
        Ray ray = minigameCamera.ViewportPointToRay(viewportPoint);


        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, cleanableLayer);
        if (hits == null || hits.Length == 0)
        {
            Debug.Log("No hits at all � check cleanableLayer or collider setup!");
            return;
        }

       
        foreach (var h in hits)
        {
            Debug.Log($"Hit {h.collider.name} on layer {LayerMask.LayerToName(h.collider.gameObject.layer)}");
        }


        DirtPatch directPatch = null;
        float bestDist = float.MaxValue;
        foreach (var h in hits)
        {
            var p = h.collider.GetComponentInParent<DirtPatch>();
            if (p != null && !p.IsCleaned) 
            {
                float d = Vector3.Distance(minigameCamera.transform.position, h.point);
                if (d < bestDist) { bestDist = d; directPatch = p; }
            }
        }

        bool cleanedAny = false;

        if (directPatch != null)
        {
            
            cleanedAny |= directPatch.Clean(cleaningSpeed * Time.deltaTime); 
        }
        else
        {
          
            Vector3 samplePoint = hits[0].point;

            foreach (var patch in dirtPatches)
            {
                if (patch == null || patch.IsCleaned) continue;

                if (IsCursorOverDirtPatch(samplePoint, patch))
                {
                    cleanedAny |= patch.Clean(cleaningSpeed * Time.deltaTime); 
                }
            }
        }

       
        if (cleanedAny)
        {
           
            patchesCleaned = 0;
            foreach (var patch in dirtPatches)
                if (patch != null && patch.IsCleaned) patchesCleaned++;

            if (cleaningProgress != null)
                cleaningProgress.value = (float)patchesCleaned / dirtPatches.Length;

            if (patchesCleaned >= dirtPatches.Length)
                EndMinigame(true);
        }

        // Start/stop broom loop based on real cleaning activity
        if (Input.GetMouseButton(0) && cleanedAny)
        {
            if (!broomLoopPlaying && sfxBroomLoop)
            {
                sfx.clip = sfxBroomLoop;
                sfx.volume = sfxVolume;
                sfx.loop = true;
                sfx.Play();
                broomLoopPlaying = true;
            }
        }
        else
        {
            if (broomLoopPlaying)
            {
                sfx.loop = false;
                sfx.Stop();
                broomLoopPlaying = false;
            }
        }

    }

    private void PlayPatchFinishIfNewlyCleaned()
    {
        int count = 0;
        foreach (var p in dirtPatches)
            if (p != null && p.IsCleaned) count++;

        if (count > lastCleanCount && sfxPatchClean)
            sfx.PlayOneShot(sfxPatchClean, sfxVolume);

        lastCleanCount = count;
    }


    private bool IsCursorOverDirtPatch(Vector3 hitPoint, DirtPatch patch)
    {
        // Get the closest point on the dirt patch's surface to our hit point
        Collider patchCollider = patch.GetComponent<Collider>();
        if (patchCollider == null) return false;

        Vector3 closestPoint = patchCollider.ClosestPoint(hitPoint);

        // Check if the hit point is close enough to the dirt patch surface
        float distance = Vector3.Distance(hitPoint, closestPoint);
        return distance < cleaningRadius;
    }

    protected override void OnMinigameEnd(bool success)
    {
        Debug.Log("Cleaning minigame " + (success ? "completed!" : "failed!"));
    }
}