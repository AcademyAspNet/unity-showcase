using UnityEngine;

public class AxisBasedTurtlePlayerController : MonoBehaviour
{
    [Header("Input Settings")]

    [SerializeField]
    private KeyCode _runKey = KeyCode.LeftShift;

    [Header("Player Settings")]

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
        float horizontalDirection = Input.GetAxis("Horizontal");
        float verticalDirection = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalDirection, 0f, verticalDirection);

        float nextMovementSpeed = Input.GetKey(_runKey) ? _runSpeed : _walkSpeed;
        _movementSpeed = Mathf.Lerp(_movementSpeed, nextMovementSpeed, _speedSmoothTime);

        Vector3 translation = movementDirection.normalized * _movementSpeed * Time.deltaTime;
        transform.Translate(translation, Space.World);
    }
}
