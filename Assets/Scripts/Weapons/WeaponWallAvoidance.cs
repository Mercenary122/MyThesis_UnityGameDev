using UnityEngine;

public class WeaponWallAvoidance : MonoBehaviour
{
    [Header("避让设置")]
    public float checkDistance = 1.2f;      // 往前检测多远（大概是枪的长度）
    public Vector3 retractedPosition = new Vector3(0, -0.2f, -0.5f); // 撞墙时枪往回缩的位置(自己微调)
    public float smoothSpeed = 10f;         // 收枪和拔枪的平滑速度
    public LayerMask wallMask = ~0;         // 什么东西算墙（默认检测所有物体）

    private Vector3 originalPosition;       // 枪原本在手里正常的位置
    private Camera mainCam;

    void Start()
    {
        // 记录枪原本的位置
        originalPosition = transform.localPosition;
        mainCam = Camera.main;
    }

    void Update()
    {
        // 从摄像机中心往前打一根射线
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        RaycastHit hit;

        // 如果前方有墙，且距离小于枪的长度
        if (Physics.Raycast(ray, out hit, checkDistance, wallMask))
        {
            // 计算靠得有多近 (0是最远，1是完全贴脸)
            float distancePercent = 1f - (hit.distance / checkDistance);

            // 根据贴脸程度，计算枪应该缩回多少
            Vector3 targetPos = Vector3.Lerp(originalPosition, originalPosition + retractedPosition, distancePercent);

            // 平滑移动过去
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // 前面没墙，平滑恢复到正常持枪位置
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * smoothSpeed);
        }
    }
}