using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button defaultButton; // Assign the Play button in Inspector

    private void Start()
    {
        // Ensure the cursor is visible and unlocked
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Select the default button to ensure UI interaction works
        if (defaultButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // Loads the game scene
    }

    public void OpenControls()
    {
        SceneManager.LoadScene("Controls"); // Loads the controls scene
    }

    public void ExitGame()
    {
        Debug.Log("Game Closed!");
        Application.Quit(); // Closes the game (Only works in a built application)
    }
}


