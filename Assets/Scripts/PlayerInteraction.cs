using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Import UI namespace

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f; // Max interaction distance
    public LayerMask interactLayer; // Layer mask for interactable objects
    public GameObject monster; // Reference to the monster GameObject
    public GameObject interactableText; // Reference to UI text (Disabled initially)

    private MonsterMovement monsterMovement; // Reference to the MonsterMovement script

    void Start()
    {
        if (monster != null)
        {
            monsterMovement = monster.GetComponent<MonsterMovement>(); // Get the MonsterMovement script attached to the monster
        }

        if (interactableText != null)
        {
            interactableText.SetActive(false); // Ensure it's disabled at start
        }
    }

    private void Update()
    {
        // Ensure Camera.main is available to avoid NullReferenceException
        if (Camera.main == null)
        {
            Debug.LogWarning("Main Camera is missing!");
            return; // Exit Update() to prevent errors
        }

        RaycastHit hit;
        bool doorDetected = false; // Flag to track if a door is detected

        // Perform a raycast to check for a door
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, interactLayer))
        {
            if (hit.collider != null && hit.collider.CompareTag("Door")) // Ensure hit.collider is not null
            {
                doorDetected = true;
                if (interactableText != null)
                {
                    interactableText.SetActive(true); // Show UI text when near a door
                }
            }
        }

        // Hide the text if no door is detected
        if (!doorDetected && interactableText != null)
        {
            interactableText.SetActive(false);
        }

        // Interaction logic
        if (Input.GetKeyDown(KeyCode.E) && doorDetected)
        {
            Debug.Log("Raycast hit: " + hit.collider.name); // Log the name of the hit object

            // Ensure hit.collider is valid before accessing components
            if (hit.collider != null)
            {
                FakeDoor fakeDoor = hit.collider.GetComponent<FakeDoor>();

                if (fakeDoor != null)
                {
                    Debug.Log("Fake door detected.");

                    // Start flickering the assigned ceiling light
                    if (fakeDoor.ceilingLight != null)
                    {
                        StartCoroutine(FlickerLight(fakeDoor.ceilingLight));
                    }

                    // Make the fake door disappear
                    fakeDoor.gameObject.SetActive(false);
                    Debug.Log("Fake door disappeared!");

                    // Move the monster to the fake door's position
                    if (monster != null && monsterMovement != null)
                    {
                        monsterMovement.MoveToFakeDoor(hit.collider.transform.position);
                        Debug.Log("Monster is heading to the fake door.");
                    }
                }
                else
                {
                    Debug.Log("Real door interacted with. Loading CutsceneScene...");
                    SceneManager.LoadScene("Cutscene"); // Load the cutscene scene
                }
            }
        }
    }

    // Flicker the ceiling light for 30 seconds
    private IEnumerator FlickerLight(Light ceilingLight)
    {
        if (ceilingLight == null) yield break;

        float flickerDuration = 30f;
        float flickerInterval = 0.1f;
        Color originalColor = ceilingLight.color;

        // Get the AudioSource from the ceiling light GameObject
        AudioSource alarmSound = ceilingLight.GetComponent<AudioSource>();

        // Start playing the alarm sound if available
        if (alarmSound != null)
        {
            alarmSound.Play();
        }

        float elapsed = 0f;
        while (elapsed < flickerDuration)
        {
            ceilingLight.color = Color.red; // Turn red
            ceilingLight.enabled = !ceilingLight.enabled; // Flicker on/off

            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }

        // Restore original settings
        ceilingLight.enabled = true;
        ceilingLight.color = originalColor;

        // Stop the alarm sound when flickering ends
        if (alarmSound != null)
        {
            alarmSound.Stop();
        }
    }

    // Call this method when the monster catches the player (to prevent errors)
    public void DisableInteraction()
    {
        Debug.Log("Disabling PlayerInteraction due to jumpscare.");
        this.enabled = false; // Disable script to prevent errors
        if (interactableText != null)
        {
            interactableText.SetActive(false); // Hide interactable text
        }
    }
}
















