using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player; // 玩家的位置

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPos = player.position;
            // 保持摄像机原来的高度(Y)，只更新 X 和 Z
            newPos.y = transform.position.y;
            transform.position = newPos;

            // 如果你想让地图跟着玩家旋转，就把下面这行注释取消掉
            // transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}