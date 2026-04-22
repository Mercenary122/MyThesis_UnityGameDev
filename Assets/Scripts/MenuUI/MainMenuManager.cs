using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene loading

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        // Ensure cursor is unlocked and visible in main menu
        // The cursor may still be locked if returning from gameplay
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Called by the NEW GAME button
    public void PlayGame()
    {
        // Load the gameplay scene
        SceneManager.LoadScene("SampleScene");
    }

    // Called by the QUIT TO DESKTOP button
    public void QuitGame()
    {
        Debug.Log("Quitting game£¡");
        Application.Quit();
    }
}