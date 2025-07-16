using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.AI; // Required for NavMeshAgent

public class MonsterJumpscare : MonoBehaviour
{
    public GameObject jumpscareUI;
    public AudioSource jumpscareAudioSource;
    public AudioClip jumpscareSound;
    public float jumpscareDuration = 2f;
    public float jumpscareVolume = 1.0f;
    public float jumpscareRange = 5f;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.5f;

    private Transform playerTransform;
    private bool isJumpscareActive = false;

    public Camera monsterCamera; // Assign the MonsterCamera in Inspector
    private Camera playerCamera;
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    private MonsterMovement monsterMovement;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerCamera = Camera.main; // Get the main player camera
        }
        else
        {
            Debug.LogError("Player not found! Ensure the player has the 'Player' tag.");
        }

        if (monsterCamera != null)
        {
            initialLocalPosition = monsterCamera.transform.localPosition;
            initialLocalRotation = monsterCamera.transform.localRotation;
            monsterCamera.gameObject.SetActive(false); // Ensure MonsterCamera is off initially
        }

        monsterMovement = GetComponent<MonsterMovement>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isJumpscareActive && playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= jumpscareRange)
            {
                StartCoroutine(TriggerJumpscare());
            }
        }
    }

    private IEnumerator TriggerJumpscare()
    {
        isJumpscareActive = true;

        // **Disable Monster Movement Script**
        if (monsterMovement != null)
        {
            monsterMovement.enabled = false;
        }

        // **Stop NavMeshAgent safely**
        if (agent != null)
        {
            agent.isStopped = true;  // Stop movement
            agent.velocity = Vector3.zero; // Prevent sliding
        }

        // **Stop Walking Animation (Freeze Monster)**
        if (monsterMovement != null)
        {
            monsterMovement.StopWalkingAnimation(); // Stop walking animation
        }

        yield return new WaitForSeconds(0.3f); // Small delay before camera switch

        // **Switch to Monster Camera**
        ApplySceneLightingToMonsterCamera();
        if (monsterCamera != null && playerCamera != null)
        {
            monsterCamera.transform.localPosition = initialLocalPosition;
            monsterCamera.transform.localRotation = initialLocalRotation;

            playerCamera.gameObject.SetActive(false);
            monsterCamera.gameObject.SetActive(true);
        }

        // **Activate jumpscare UI**
        if (jumpscareUI != null)
        {
            jumpscareUI.SetActive(true);
        }

        // **Play jumpscare sound**
        if (jumpscareAudioSource != null && jumpscareSound != null)
        {
            jumpscareAudioSource.volume = jumpscareVolume;
            jumpscareAudioSource.PlayOneShot(jumpscareSound);
        }

        // **Shake effect**
        StartCoroutine(ShakeCamera());

        yield return new WaitForSeconds(jumpscareDuration);

        // **Switch back to Player Camera**
        if (playerCamera != null && monsterCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
            monsterCamera.gameObject.SetActive(false);
        }

        // **Re-enable monster movement properly**
        if (monsterMovement != null)
        {
            monsterMovement.enabled = true;
        }

        // **Resume NavMeshAgent**
        if (agent != null)
        {
            agent.isStopped = false; // Resume movement
        }

        // **Resume Walking Animation**
        if (monsterMovement != null)
        {
            monsterMovement.PlayWalkingAnimation(); // Play walking animation again
        }

        // **Load Scene
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator ShakeCamera()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            monsterCamera.transform.localPosition = initialLocalPosition + (Random.insideUnitSphere * shakeIntensity);
            elapsed += Time.deltaTime;
            yield return null;
        }
        monsterCamera.transform.localPosition = initialLocalPosition;
    }

    private void ApplySceneLightingToMonsterCamera()
    {
        monsterCamera.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.05f;
        RenderSettings.fogMode = FogMode.ExponentialSquared;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.3f, 0.3f, 0.3f);
    }
}

















