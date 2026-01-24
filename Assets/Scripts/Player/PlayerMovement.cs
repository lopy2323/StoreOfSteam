using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1000f;

    private Rigidbody _rb;
    private Vector3 _moveInput;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 moveDirection = transform.right * _moveInput.x + transform.forward * _moveInput.z;
        _rb.linearVelocity = moveDirection;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();

        _moveInput = new Vector3(inputVector.x * _moveSpeed, 0, inputVector.y * _moveSpeed);
    }
}
