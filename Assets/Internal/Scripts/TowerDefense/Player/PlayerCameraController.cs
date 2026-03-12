using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Movement Settings")]

    [SerializeField]
    private float _movementSpeed = 10f;

    [SerializeField]
    private float _rotationSpeed = 45f;

    [SerializeField]
    private float _zoomSpeed = 5f;

    [Header("Lerp Smoothing Settings")]

    [SerializeField]
    private float _positionLerpSpeed = 5f;

    [SerializeField]
    private float _rotationLerpSpeed = 3f;

    [SerializeField]
    private float _zoomLerpSpeed = 4f;

    [Header("Area Settings")]

    [SerializeField]
    private Transform _fromPoint;

    [SerializeField]
    private Transform _toPoint;

    private Vector2 _movementInput;
    private Vector3 _rotationInput;
    private float _zoomInput;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private float _targetHeight;

    private Vector3 _fromPosition = Vector3.zero;
    private Vector3 _toPosition = Vector3.zero;

    public void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }

    public void OnRotate(InputValue inputValue)
    {
        _rotationInput = inputValue.Get<Vector2>();
    }

    public void OnZoom(InputValue inputValue)
    {
        _zoomInput = inputValue.Get<float>();
    }

    private void Start()
    {
        _targetPosition = transform.position;
        _targetRotation = transform.rotation;
        _targetHeight = transform.position.y;

        float xMin = Mathf.Min(_fromPoint.position.x, _toPoint.position.x);
        float yMin = Mathf.Min(_fromPoint.position.y, _toPoint.position.y);
        float zMin = Mathf.Min(_fromPoint.position.z, _toPoint.position.z);

        float xMax = Mathf.Max(_fromPoint.position.x, _toPoint.position.x);
        float yMax = Mathf.Max(_fromPoint.position.y, _toPoint.position.y);
        float zMax = Mathf.Max(_fromPoint.position.z, _toPoint.position.z);

        _fromPosition = new Vector3(xMin, yMin, zMin);
        _toPosition = new Vector3(xMax, yMax, zMax);
    }

    private void Update()
    {
        bool shouldUpdatePosition = Vector3.Distance(transform.position, _targetPosition) > 0.1f;
        bool shouldUpdateHeight = Mathf.Abs(_targetHeight - transform.position.y) > 0.1f;

        if (shouldUpdatePosition || shouldUpdateHeight)
        {
            Vector3 nextPosition = Vector3.Lerp(
                transform.position, _targetPosition, _positionLerpSpeed * Time.deltaTime);

            nextPosition.y = Mathf.Lerp(
                nextPosition.y, _targetHeight, _zoomLerpSpeed * Time.deltaTime);

            transform.position = nextPosition;
        }

        transform.rotation = Quaternion.Lerp(
            transform.rotation, _targetRotation, _rotationLerpSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        Vector3 forward = transform.forward;

        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;

        right.y = 0f;
        right.Normalize();

        _targetPosition += (forward * _movementInput.y + right * _movementInput.x) *
            _movementSpeed * Time.deltaTime;

        _targetPosition = ClampPosition(_targetPosition, _fromPosition, _toPosition);

        _targetRotation = Quaternion.AngleAxis(
            _rotationInput.x * _rotationSpeed * Time.deltaTime, Vector3.up) * _targetRotation;

        float minHeight = _fromPosition.y;
        float maxHeight = _toPosition.y;

        _targetHeight -= _zoomInput * _zoomSpeed * Time.deltaTime;
        _targetHeight = Mathf.Clamp(_targetHeight, minHeight, maxHeight);
    }

    private static Vector3 ClampPosition(Vector3 position, Vector3 mins, Vector3 maxs)
    {
        Vector3 clampedPosition = position;

        clampedPosition.x = Mathf.Clamp(position.x, mins.x, maxs.x);
        clampedPosition.y = Mathf.Clamp(position.y, mins.y, maxs.y);
        clampedPosition.z = Mathf.Clamp(position.z, mins.z, maxs.z);

        return clampedPosition;
    }
}
