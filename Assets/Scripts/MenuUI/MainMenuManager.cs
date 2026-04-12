using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene loading

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        // 极其重要的一步：确保在主菜单时，鼠标指针是解锁且可见的！
        // 因为如果玩家是从游戏里按退出返回主菜单的，鼠标可能还处于隐藏锁定状态。
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 给 NEW GAME 按钮用的方法
    public void PlayGame()
    {
        // 加载你的打枪场景（确保名字和你项目里的一模一样！）
        // 这里的 "SampleScene" 就是你之前的那个关卡名字
        SceneManager.LoadScene("SampleScene");
    }

    //{

    //    Debug.Log("选项菜单还没做呢，敬请期待！");
    //}

    // 给 QUIT TO DESKTOP 按钮用的方法
    public void QuitGame()
    {
        Debug.Log("退出游戏！");
        Application.Quit();
    }
}