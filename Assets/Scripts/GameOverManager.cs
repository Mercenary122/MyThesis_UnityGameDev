using UnityEngine;
using UnityEngine.SceneManagement; // To reload the scene

public class GameOverManager : MonoBehaviour
{
    // Bind this function to OnClick of Restart Button
    public void RestartGame()
    {
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Make the cursor invisible after restart
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }
}