using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseManager : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject pauseMenuUI;

    public GameObject optionsPanel;

    public static bool isPaused = false;
    void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false); // 【新增】确保选项面板默认关闭
        isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        bool escPressed = false;

        // --- 兼容新老两套输入系统的终极检测 ---
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
        Debug.Log("退出游戏！");
        Application.Quit(); // 打包后生效
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑器里生效：自动退出播放模式
#endif
    }
}