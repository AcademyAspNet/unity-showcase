using UnityEngine;

public class TankPlayerController : MonoBehaviour
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

    [Header("Movement Settings")]

    [SerializeField]
    private float _drivingSpeed = 5f;

    [SerializeField]
    private float _rotationSpeed = 90f;

    [Header("Smoothing Settings")]

    [SerializeField]
    private float _speedSmoothTime = 0.3f;

    private float _currentMovementSpeed;

    private void Update()
    {
        float movementInput = 0f;
        
        if (Input.GetKey(_forwardKey))
            movementInput += 1;

        if (Input.GetKey(_backKey))
            movementInput -= 1;

        float rotationInput = 0f;

        if (Input.GetKey(_leftKey))
            rotationInput -= 1;

        if (Input.GetKey(_rightKey))
            rotationInput += 1;

        if (Mathf.Abs(movementInput) > 0)
        {
            float nextMovementSpeed = _drivingSpeed * Mathf.Abs(movementInput);
            float speedDelta = _speedSmoothTime * Time.deltaTime;

            _currentMovementSpeed = Mathf.Lerp(_currentMovementSpeed, nextMovementSpeed, speedDelta);
        }
        else
        {
            _currentMovementSpeed = Mathf.Lerp(_currentMovementSpeed, 0f, _speedSmoothTime * Time.deltaTime);
        }

        if (Mathf.Abs(rotationInput) > 0)
        {
            transform.Rotate(0, rotationInput * _rotationSpeed * Time.deltaTime, 0, Space.Self);
        }

        if (Mathf.Abs(movementInput) > 0)
        {
            Vector3 translation = transform.forward * _currentMovementSpeed * movementInput * Time.deltaTime;
            transform.Translate(translation, Space.World);
        }
    }
}
