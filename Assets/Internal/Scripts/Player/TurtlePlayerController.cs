using UnityEngine;

public class TurtlePlayerController : MonoBehaviour
{
    [Header("Input Settings")]

    [SerializeField]
    private KeyCode _forwardKey = KeyCode.W;

    [SerializeField]
    private KeyCode _backKey = KeyCode.S;

    [SerializeField]
    private KeyCode _leftKey = KeyCode.A;

    [SerializeField]
    private KeyCode _rightKey = KeyCode.D;

    [SerializeField]
    private KeyCode _runKey = KeyCode.LeftShift;

    [Header("Movement Settings")]

    [SerializeField]
    private float _walkSpeed = 5f;

    [SerializeField]
    private float _runSpeed = 7.5f;

    [Header("Smoothing Settings")]

    [SerializeField]
    private float _speedSmoothTime = 0.3f;

    private float _movementSpeed;

    private void Update()
    {
        Vector3 movementDirection = Vector3.zero;

        if (Input.GetKey(_forwardKey))
            movementDirection.z += 1;

        if (Input.GetKey(_backKey))
            movementDirection.z -= 1;

        if (Input.GetKey(_leftKey))
            movementDirection.x -= 1;

        if (Input.GetKey(_rightKey))
            movementDirection.x += 1;

        if (movementDirection == Vector3.zero)
            return;

        float nextMovementSpeed = Input.GetKey(_runKey) ? _runSpeed : _walkSpeed;
        _movementSpeed = Mathf.Lerp(_movementSpeed, nextMovementSpeed, _speedSmoothTime);

        Vector3 translation = movementDirection.normalized * _movementSpeed * Time.deltaTime;
        transform.Translate(translation, Space.World);
    }
}
