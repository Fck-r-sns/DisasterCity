using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float _cameraMovementSpeed = 10f;
    [SerializeField]
    float _cameraRotationSpeed = 10f;
    [SerializeField]
    float _minCameraHeight = 10f;
    [SerializeField]
    float _maxCameraHeight = 200f;

    void Update()
    {
        float sideSpeed = _cameraMovementSpeed * Input.GetAxis("Horizontal");
        float verticalScroll = _cameraMovementSpeed * -Input.mouseScrollDelta.y;
        float forwardSpeed = _cameraMovementSpeed * Input.GetAxis("Vertical");
        Vector3 forward = transform.forward * 1000f;
        forward.y = 0f;
        Vector3 newPosition = transform.position;
        newPosition = Vector3.MoveTowards(newPosition, newPosition + forward, forwardSpeed);
        newPosition = Vector3.MoveTowards(newPosition, newPosition + forward, forwardSpeed);
        newPosition = Vector3.MoveTowards(newPosition, newPosition + transform.right * 1000f, sideSpeed);
        newPosition = Vector3.MoveTowards(newPosition, newPosition + Vector3.up * 1000f, verticalScroll);
        newPosition.y = Mathf.Clamp(newPosition.y, _minCameraHeight, _maxCameraHeight);
        transform.position = newPosition;

        if (Input.GetMouseButton(2))
        {
            float h = _cameraRotationSpeed * Input.GetAxis("Mouse X");
            float v = -_cameraRotationSpeed * Input.GetAxis("Mouse Y");
            transform.Rotate(0f, h, 0f, Space.World);
            transform.Rotate(v, 0f, 0f, Space.Self);
        }
    }
}
