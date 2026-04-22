using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;

    public GameObject optionsPanel;

    public static bool isPaused = false;
    void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false); // Ensure options panel is closed by default
        isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        bool escPressed = false;

        // --- Compatible with both old and new Input Systems ---
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            escPressed = true;
#else
        if (Input.GetKeyDown(KeyCode.Escape))
            escPressed = true;
#endif
        // -------------------------------------

        if (escPressed)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game!");
        Application.Quit(); // Works in built game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Works in Editor: stop play mode
#endif
    }
}