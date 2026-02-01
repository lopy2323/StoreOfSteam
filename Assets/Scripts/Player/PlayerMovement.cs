using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _walkSpeed = 7f;
    [SerializeField] private float _sprintSpeed = 14f;

    private float _moveSpeed = 0f;

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

        _moveSpeed = _walkSpeed;
    }

    void FixedUpdate()
    {
        Moveplayer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _moveSpeed = _sprintSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _moveSpeed = _walkSpeed;
        }
    }

    private void Moveplayer()
    {
        Vector3 moveDirection = _orientation.forward * _moveInput.z + _orientation.right * _moveInput.x;
        _rb.AddForce(moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);

        if (_moveInput.x != 0f || _moveInput.z != 0)
        {
            _cameraYOffset += Time.deltaTime * _walkBobSpeed * (((_moveSpeed / _walkSpeed - 1) / 2) + 1);
            _moveCamera.cameraOffsetY = Mathf.Sin(_cameraYOffset) * _walkBobAmount * (((_moveSpeed / _walkSpeed - 1) / 5) + 1);
        }
        else
        {
            _cameraYOffset = 0f;
            _moveCamera.cameraOffsetY = Mathf.Lerp(_moveCamera.cameraOffsetY, 0f, Time.deltaTime * 8f);
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();

        _moveInput = new Vector3(inputVector.x, 0, inputVector.y);
    }
}
