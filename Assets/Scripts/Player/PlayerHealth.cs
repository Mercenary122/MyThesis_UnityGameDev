using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // 引入 Input System 命名空间
#endif

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    private float maxHealth = 100f;
    public float chipSpeed = 2f;

    [Header("UI References")]
    public Image frontHealthBar;
    public Image backHealthBar;
    public GameObject deathScreen; // 这里要拖拽刚才做的 DeathScreen 面板

    [Header("Audio Settings")]
    public AudioClip deathClip;       // 拖拽你的 Game Over 音效到这里
    public AudioSource audioSource;   // 用来播放声音的组件

    public bool isDead = false;

    void Start()
    {
        health = maxHealth;
        if (frontHealthBar != null) frontHealthBar.fillAmount = 1f;
        if (backHealthBar != null) backHealthBar.fillAmount = 1f;

        // 游戏开始时确保死亡屏幕是关掉的
        if (deathScreen != null) deathScreen.SetActive(false);
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0f, maxHealth);
        UpdateHealthUI();

        // 只有没死且血量归零时才触发
        if (!isDead && health <= 0f)
        {
            Die();
        }
    }

    public void UpdateHealthUI()
    {
        // ... (这部分UI血条逻辑保持你原来的不变，此处省略以节省篇幅) ...
        // 复制你原来的 UpdateHealthUI 代码即可，不需要改动
        float fillF = frontHealthBar != null ? frontHealthBar.fillAmount : 0f;
        float fillB = backHealthBar != null ? backHealthBar.fillAmount : 0f;
        float hFraction = health / maxHealth;

        if (fillB > hFraction)
        {
            if (frontHealthBar != null) frontHealthBar.fillAmount = hFraction;
            if (backHealthBar != null)
            {
                backHealthBar.color = Color.red;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete *= percentComplete;
                backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
            }
        }

        if (fillF < hFraction)
        {
            if (backHealthBar != null)
            {
                backHealthBar.color = Color.green;
                backHealthBar.fillAmount = hFraction;
            }
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            if (frontHealthBar != null)
                frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
    }

    public bool RestoreHealth(float healAmount)
    {
        // 1. 如果血已经是满的，直接返回 false（不要浪费血包）
        if (health >= maxHealth)
        {
            return false;
        }

        // 2. 加血逻辑
        health += healAmount;
        health = Mathf.Clamp(health, 0f, maxHealth);
        lerpTimer = 0f; // 重置血条动画计时器

        // 3. 返回 true，告诉血包“我吃掉你了”
        return true;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player Died");

        if (audioSource != null && deathClip != null)
        {
            // PlayOneShot 的好处是：即使 AudioSource 正在播别的（比如脚步声），
            // 这个声音也会叠加播放，不会被打断。
            audioSource.PlayOneShot(deathClip);
        }

        // 1. 禁用输入系统 (这是让你无法移动的关键！)
        // 尝试获取 PlayerInput 组件并禁用它
#if ENABLE_INPUT_SYSTEM
        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null) input.enabled = false;
     #endif

        // 2. 为了保险，也尝试禁用 FirstPersonController (如果存在)
        // 这里的 StarterAssets.FirstPersonController 是默认命名空间，如果不报错就留着
        // var controller = GetComponent<StarterAssets.FirstPersonController>();
        // if (controller != null) controller.enabled = false;

        // 3. 禁用武器开火
        // 找到 WeaponHolder 并禁用它，这样你就不能开枪了
        Transform weaponHolder = transform.Find("PlayerCameraRoot/WeaponHolder"); // 根据你的层级路径查找
        if (weaponHolder != null) weaponHolder.gameObject.SetActive(false);

        // 4. 处理鼠标光标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 5. 显示黑屏 UI
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
    }
}