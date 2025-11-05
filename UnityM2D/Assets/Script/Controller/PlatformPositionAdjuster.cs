using UnityEngine;

public class PlatformPositionAdjuster : MonoBehaviour
{
    private Vector3 initialViewportPos;
    private float distanceFromCamera;
    private Vector3 initialLocalScale;

    // 기준 해상도
    private const float ReferenceScreenWidth = 1080f;
    private const float ReferenceScreenHeight = 1920f;

    // 현재 스케일 팩터를 저장 (다른 스크립트에서 접근 가능)
    public static float CurrentScaleFactor { get; private set; } = 1f;

    void Start()
    {
        Vector3 currentWorldPos = transform.position;
        initialViewportPos = Camera.main.WorldToViewportPoint(currentWorldPos);
        distanceFromCamera = initialViewportPos.z;
        initialLocalScale = transform.localScale;
    }

    void LateUpdate()
    {
        //AdjustTransformToPlatform();
        AdjustScaleToPlatform();
    }

    void AdjustTransformToPlatform()
    {
        Vector3 newViewportPos = new Vector3(
            initialViewportPos.x,
            initialViewportPos.y,
            distanceFromCamera
        );
        transform.position = Camera.main.ViewportToWorldPoint(newViewportPos);
    }

    void AdjustScaleToPlatform()
    {
        float referenceAspect = ReferenceScreenWidth / ReferenceScreenHeight;
        float currentAspect = (float)Screen.width / Screen.height;

        float scaleFactor;

        if (currentAspect > referenceAspect)
        {
            scaleFactor = Screen.height / ReferenceScreenHeight;
        }
        else
        {
            scaleFactor = Screen.width / ReferenceScreenWidth;
        }

        // Static 변수에 저장하여 다른 곳에서도 사용 가능
        CurrentScaleFactor = scaleFactor;

        Vector3 newScale = new Vector3(
            initialLocalScale.x * scaleFactor,
            initialLocalScale.y * scaleFactor,
            initialLocalScale.z
        );

        transform.localScale = newScale;
    }
}