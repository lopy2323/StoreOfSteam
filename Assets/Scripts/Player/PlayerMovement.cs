using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1000f;

    [SerializeField] private Transform _orientation;

    private Rigidbody _rb;
    private Vector3 _moveInput;


    private float _cameraYOffset;
    [SerializeField] private float _walkBobAmount = 0.05f;
    [SerializeField] private float _walkBobSpeed = 15f;
    [SerializeField] private MoveCamera _moveCamera;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        Moveplayer();
    }

    private void Moveplayer()
    {
        Vector3 moveDirection = _orientation.forward * _moveInput.z + _orientation.right * _moveInput.x;
        _rb.AddForce(moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);

        if (_moveInput.x != 0f || _moveInput.z != 0)
        {
            _cameraYOffset += Time.deltaTime * _walkBobSpeed;
            _moveCamera.cameraOffsetY = Mathf.Sin(_cameraYOffset) * _walkBobAmount;
        }
        else
        {
            _cameraYOffset = 0f;
            _moveCamera.cameraOffsetY = 0f;

        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();

        _moveInput = new Vector3(inputVector.x, 0, inputVector.y);
    }
}
