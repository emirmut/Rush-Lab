using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BasketballMinigame : MinigameBase
{

    [Header("Win/Loss Conditions")]
    public int shotsNeeded = 3;     // already present
    public int maxAttempts = 15;    // NEW: cap attempts

    [Header("Basketball Settings")]
    public GameObject orePrefab;
    public Transform spawnPoint;
    public Transform targetContainer;
    public float minThrowForce = 5f;
    public float maxThrowForce = 15f;
    public float chargeSpeed = 5f;
    public float resetDelay = 2f;

    [Header("UI Elements")]
    public Slider powerMeter;
    public TextMeshProUGUI scoreText;
    public Text instructionText;

    [Header("Trajectory Preview")]
    public bool showTrajectory = false;    // <- new
    public LineRenderer trajectoryLine;
    public int trajectoryPoints = 30;
    public float trajectoryTimeStep = 0.1f;

    // --- Audio (Basketball) ---
    [Header("Audio")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip sfxThrow;      // when mouse released / ore spawned
    [SerializeField] private AudioClip sfxRimHit;     // contacted rim/backboard
    [SerializeField] private AudioClip sfxScore;      // scored
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.7f;



    private GameObject currentOre;
    private int successfulShots = 0;
    private int totalAttempts = 0;
    private bool canThrow = true;
    private bool isCharging = false;
    private float currentPower = 0f;

    public override void Initialize(MinigamePopup popup)
    {
        base.Initialize(popup);

        // Ensure AudioSource exists & is 2D
        if (sfx == null) sfx = gameObject.GetComponent<AudioSource>();
        if (sfx == null) sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f; // 2D for popup


        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }
        if (trajectoryLine != null) trajectoryLine.enabled = false;

    }

    protected override void OnMinigameStart()
    {
        successfulShots = 0;
        totalAttempts = 0;
        UpdateScoreDisplay();
        SpawnOre();

        if (instructionText != null)
        {
            instructionText.text = $"Hold Left Click to charge, release to throw! ({successfulShots}/{shotsNeeded})";
        }
    }

    private void SpawnOre()
    {
        if (currentOre != null)
            Destroy(currentOre);

        currentOre = Instantiate(orePrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = currentOre.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

      
        BasketballOre oreComponent = currentOre.GetComponent<BasketballOre>();
        if (oreComponent == null)
        {
            oreComponent = currentOre.AddComponent<BasketballOre>();
        }
        oreComponent.Initialize(this);

        canThrow = true;
        currentPower = 0f;

        if (powerMeter != null)
        {
            powerMeter.value = 0f;
        }
    }

    protected override void MinigameUpdate()
    {
        if (canThrow)
        {
            HandleThrowInput();
        }
    }

    private void HandleThrowInput()
    {
 
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            currentPower = 0f;
        }


        if (Input.GetMouseButton(0) && isCharging)
        {
            currentPower += chargeSpeed * Time.deltaTime;
            currentPower = Mathf.Clamp01(currentPower);

            if (powerMeter != null)
            {
                powerMeter.value = currentPower;
            }

            if (showTrajectory && trajectoryLine != null) { ShowTrajectoryPreview(); }

        }


        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            isCharging = false;
            ThrowOre();

            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = false;
            }
        }
    }

    private void ShowTrajectoryPreview()
    {
        if (trajectoryLine == null || currentOre == null) return;

        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = trajectoryPoints;

        Vector3 startPosition = spawnPoint.position;
        Vector3 throwDirection = CalculateThrowDirection();
        float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, currentPower);
        Vector3 velocity = throwDirection * throwForce;

       
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * trajectoryTimeStep;
            Vector3 point = startPosition + velocity * time + 0.5f * Physics.gravity * time * time;
            trajectoryLine.SetPosition(i, point);
        }
    }

    private void ThrowOre()
    {
        if (currentOre == null) return;

        canThrow = false;
        totalAttempts++;

        Rigidbody rb = currentOre.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;


        Vector3 throwDirection = CalculateThrowDirection();
        float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, currentPower);

        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

        rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);

        if (powerMeter != null)
        {
            powerMeter.value = 0f;
        }

        if (sfxThrow) sfx.PlayOneShot(sfxThrow, sfxVolume);


        Invoke(nameof(CheckMissAndReset), resetDelay);
    }

    private Vector3 CalculateThrowDirection()
    {
 
        Vector3 direction = (targetContainer.position - spawnPoint.position).normalized;


        float distance = Vector3.Distance(spawnPoint.position, targetContainer.position);
        float arcHeight = Mathf.Clamp(distance * 0.3f, 0.5f, 2f);
        direction.y += arcHeight;

        return direction.normalized;
    }

    private void CheckMissAndReset()
    {

        if (canThrow) return;

        SpawnOre();

        if (instructionText != null)
        {
            instructionText.text = $"Missed! Try again. ({successfulShots}/{shotsNeeded})";
        }
        CheckEndConditionsOrContinue(false);

    }

    public void OnOreScored()
    {
        CancelInvoke(nameof(CheckMissAndReset));

        successfulShots++;
        UpdateScoreDisplay();
        CheckEndConditionsOrContinue(true);

        if (sfxScore) sfx.PlayOneShot(sfxScore, sfxVolume);


        if (instructionText != null)
        {
            instructionText.text = $"Score! ({successfulShots}/{shotsNeeded})";
        }

        if (successfulShots >= shotsNeeded)
        {
            Invoke(nameof(CompleteMinigame), 1.5f);
        }
        else
        {
            Invoke(nameof(SpawnOre), 1.5f);
        }
    }

    private void CompleteMinigame()
    {
        EndMinigame(true);
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {successfulShots}/{shotsNeeded}";
        }
    }

    protected override void OnMinigameEnd(bool success)
    {
        if (currentOre != null)
            Destroy(currentOre);

        CancelInvoke();

        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }

        Debug.Log($"Basketball minigame {(success ? "completed!" : "failed!")} - Score: {successfulShots}/{totalAttempts}");
    }

    private void CheckEndConditionsOrContinue(bool justScored)
    {
        // Win?
        if (successfulShots >= shotsNeeded)
        {
            Invoke(nameof(CompleteMinigame), 1.0f);
            return;
        }

        // Lose?
        if (totalAttempts >= maxAttempts)
        {
            // Optional: message
            if (instructionText != null)
                instructionText.text = $"Out of shots! ({successfulShots}/{shotsNeeded})";
            Invoke(nameof(FailMinigame), 1.0f);
            return;
        }

        // Otherwise continue
        if (!justScored)
            SpawnOre(); // if you didn�t already schedule it
    }
    private void FailMinigame() => EndMinigame(false);

}