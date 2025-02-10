using UnityEngine;

public class CameraController : MonoBehaviour
{
    public void MoveCameraToObject(Transform target)
    {
        if (target == null)
        {
            Debug.LogError("目标变换为空。");
            return;
        }

        // 计算目标在屏幕上的期望大小（屏幕高度的一半）
        float desiredSizeOnScreen = Screen.height * 0.5f;

        // 根据期望大小计算摄像机到目标的距离
        float distance = Vector3.Distance(transform.position, target.position);
        float sizeInWorld = desiredSizeOnScreen / Screen.height * 2.0f * distance * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);

        // 调整距离以使目标占据屏幕的一半
        distance = sizeInWorld / (Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2.0f);

        // 计算摄像机的新位置
        Vector3 newPosition = target.position - transform.forward * distance;

        // 使用缓动效果移动摄像机到新位置
        StartCoroutine(MoveToPosition(newPosition));

        // 确保摄像机看向目标
        transform.LookAt(target);
    }

    // 缓动移动到指定位置
    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float duration = 1.0f; // 缓动持续时间
        float elapsedTime = 0.0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }

        transform.position = targetPosition; // 确保最终位置准确
    }
}