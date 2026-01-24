using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float _cameraSensitivity = 10f;
    [SerializeField] private float _maxCameraAngle = 80f;
    [SerializeField] private Camera _camera;

    private float _pitch = 0f; // Track the up/down rotation

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();


        transform.Rotate(Vector3.up, _cameraSensitivity * inputVector.x);


        _pitch -= _cameraSensitivity * inputVector.y;
        _pitch = Mathf.Clamp(_pitch, -_maxCameraAngle, _maxCameraAngle);

        _camera.transform.localEulerAngles = new Vector3(_pitch, 0f, 0f);
    }
}
