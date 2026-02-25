using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyBasedPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]

    [SerializeField]
    private float _walkSpeed = 5f;

    [SerializeField]
    private float _runSpeed = 10f;

    [SerializeField]
    private float _acceleration = 10f;

    [SerializeField]
    private float _jumpForce = 7f;

    [Header("Ground Check Settings")]

    [SerializeField]
    private LayerMask _groundLayerMask = 1;

    [SerializeField]
    private float _groundCheckDistance = 0.2f;

    [SerializeField]
    private Vector3 _groundCheckOffset = Vector3.zero;

    [Header("Input Actions")]

    [SerializeField]
    private InputActionReference _moveAction;

    [SerializeField]
    private InputActionReference _jumpAction;

    [SerializeField]
    private InputActionReference _runAction;

    private Rigidbody _rigidbody;

    private Vector2 _moveInput;
    private bool _isRunning;
    private bool _isJumpPressed;

    private float _currentMovementSpeed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (_moveAction != null)
            _moveAction.action.Enable();

        if (_runAction != null)
            _runAction.action.Enable();

        if (_jumpAction != null)
            _jumpAction.action.Enable();
    }

    private void OnEnable()
    {
        if (_moveAction != null)
        {
            _moveAction.action.performed += OnMove;
            _moveAction.action.canceled += OnMove;
        }

        if (_runAction != null)
        {
            _runAction.action.performed += OnRun;
            _runAction.action.canceled += OnRun;
        }

        if (_jumpAction != null)
            _jumpAction.action.performed += OnJump;
    }

    private void OnDisable()
    {
        if (_moveAction != null)
        {
            _moveAction.action.performed -= OnMove;
            _moveAction.action.canceled -= OnMove;
        }

        if (_runAction != null)
        {
            _runAction.action.performed -= OnRun;
            _runAction.action.canceled -= OnRun;
        }

        if (_jumpAction != null)
            _jumpAction.action.performed -= OnJump;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = true;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        _isRunning = context.ReadValueAsButton();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        float nextMovementSpeed = _isRunning ? _runSpeed : _walkSpeed;
        float targetSpeed = _moveInput.magnitude > 0.1f ? nextMovementSpeed : 0f;

        _currentMovementSpeed = Mathf.MoveTowards(_currentMovementSpeed, targetSpeed, _acceleration * Time.fixedDeltaTime);

        Vector3 moveDirection = (transform.forward * _moveInput.y + transform.right * _moveInput.x).normalized;

        Vector3 targetVelocity = moveDirection * _currentMovementSpeed;

        targetVelocity.y = _rigidbody.linearVelocity.y;

        _rigidbody.linearVelocity = targetVelocity;
    }

    private void HandleJump()
    {
        if (!_isJumpPressed)
            return;

        if (IsGrounded())
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

        _isJumpPressed = false;
    }

    private bool IsGrounded()
    {
        Vector3 start = transform.position + _groundCheckOffset;
        return Physics.Raycast(start, Vector3.down, _groundCheckDistance, _groundLayerMask);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 start = transform.position + _groundCheckOffset;
        Gizmos.DrawLine(start, start + Vector3.down * _groundCheckDistance);
    }
}
