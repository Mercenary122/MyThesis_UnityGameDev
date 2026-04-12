using UnityEngine;
using TMPro; // 【新增】引入 TextMeshPro UI 命名空间！
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInteract : MonoBehaviour
{
    [Header("设置")]
    public float distance = 3f;
    public LayerMask mask = ~0;

    public Camera cam;

    [Header("UI 提示")]
    public TextMeshProUGUI promptText; // 【新增】拖入你刚才创建的文本框！

    void Start()
    {
        if (cam == null) cam = GetComponentInChildren<Camera>();

        // 游戏刚开始时，确保屏幕中间没字
        if (promptText != null) promptText.text = "";
    }

    void Update()
    {
        // 【关键】每一帧开始时，先把提示清空。这样只要你不盯着物品，字就会消失！
        if (promptText != null) promptText.text = "";

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            PlayerInteractable interactable = hitInfo.collider.GetComponentInParent<PlayerInteractable>();
            WeaponPickup weaponPickup = hitInfo.collider.GetComponent<WeaponPickup>();
            if (weaponPickup == null) weaponPickup = hitInfo.collider.GetComponentInParent<WeaponPickup>();

            // --- UI 提示逻辑开始 ---
            if (interactable != null && promptText != null)
            {
                promptText.text = "Press [E] to interact"; // 看向开关时显示的字
            }
            else if (weaponPickup != null && promptText != null)
            {
                // 这里为了方便，直接读取了模型物体的名字
                // 如果你想固定写死，可以改成 promptText.text = "按 [E] 捡起武器";
                promptText.text = "Press [E] to pick up " + hitInfo.collider.gameObject.name;
            }
            // --- UI 提示逻辑结束 ---

            // --- 实际按键拾取逻辑（和以前一样） ---
            if (interactable != null || weaponPickup != null)
            {
                bool isPressed = false;
#if ENABLE_INPUT_SYSTEM
                if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
                    isPressed = true;
#else
                if (Input.GetKeyDown(KeyCode.E))
                    isPressed = true;
#endif

                if (isPressed)
                {
                    if (interactable != null) interactable.BaseInteract();
                    if (weaponPickup != null) weaponPickup.PickUpWeapon(this.gameObject);
                }
            }
        }
    }
}