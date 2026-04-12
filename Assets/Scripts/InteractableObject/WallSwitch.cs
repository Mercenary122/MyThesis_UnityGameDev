using UnityEngine;

// Extends the PlayerInteractable
public class WallSwitch : PlayerInteractable
{
    [Header("开关设置")]
    [Tooltip("把场景里的门拖到这里")]
    public SlidingDoor targetDoor;
    public AudioSource switchSound;

    // 这里重写父类的虚方法
    protected override void Interact()
    {
        if (targetDoor != null)
        {
            Debug.Log("开关被触发！");
            targetDoor.OpenDoor();
        }

        // 2. 播放音效
        if (switchSound != null) switchSound.Play();
    }
}