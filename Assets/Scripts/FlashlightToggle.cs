using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    private Light spotlight;

    void Start()
    {
        spotlight = GetComponentInChildren<Light>();
        if (spotlight == null)
        {
            Debug.LogError("No Spotlight found as a child of this object!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Press 'F' to toggle the flashlight
        {
            if (spotlight != null)
            {
                spotlight.enabled = !spotlight.enabled;
            }
        }
    }
}

