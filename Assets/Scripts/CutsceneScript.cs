using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public AudioSource doorOpenSound;
    public AudioSource runningSound;
    public AudioSource creepyMusic;
    public TMP_Text escapeText; // Use TextMeshPro. If using UI Text, change to Text.
    public float fadeDuration = 2f;

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        // Play door open sound
        doorOpenSound.Play();
        yield return new WaitForSeconds(doorOpenSound.clip.length);

        // Play running sound
        runningSound.Play();
        yield return new WaitForSeconds(runningSound.clip.length);

        // Start creepy music and fade in text
        creepyMusic.Play();
        StartCoroutine(FadeInText());

        // Wait for 8 seconds before ending the scene
        yield return new WaitForSeconds(8f);

        // Load the next scene or quit
        SceneManager.LoadScene("MainMenu"); // Replace "NextScene" with the actual scene name
    }

    IEnumerator FadeInText()
    {
        float elapsedTime = 0f;
        Color textColor = escapeText.color;
        textColor.a = 0f;
        escapeText.color = textColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            escapeText.color = textColor;
            yield return null;
        }
    }
}

