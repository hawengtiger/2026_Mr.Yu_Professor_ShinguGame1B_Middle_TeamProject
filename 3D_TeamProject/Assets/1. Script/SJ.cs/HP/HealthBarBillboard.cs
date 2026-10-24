using UnityEngine;

public class HealthBarBillboard : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        transform.rotation = mainCamera.rotation;
    }
}