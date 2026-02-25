using UnityEngine;

public class SmoothCameraFollower : MonoBehaviour
{
    public enum RotationMode
    {
        MatchTargetRotation,
        LookAtTarget
    }

    [Header("General Settings")]

    [SerializeField]
    private Transform _cameraTarget;

    [SerializeField]
    private Vector3 _cameraPositionOffset = Vector3.zero;

    [Header("Follow Settings")]

    [SerializeField]
    private bool _shouldFollowPosition = true;

    [SerializeField]
    private bool _shouldFollowRotation = false;

    [SerializeField]
    private RotationMode _rotationMode = RotationMode.LookAtTarget;

    [Header("Smoothing Settings")]

    [SerializeField]
    private float _positionSmoothTime = 0.3f;

    [SerializeField]
    private float _rotationSmoothTime = 0.3f;

    private Vector3 _positionVelocity;

    private void LateUpdate()
    {
        if (_cameraTarget == null)
            return;

        if (_shouldFollowPosition)
            UpdateCameraPosition();

        if (_shouldFollowRotation)
            UpdateCameraRotation();
    }

    private void UpdateCameraPosition()
    {
        Vector3 desiredPosition = _cameraTarget.position + _cameraTarget.rotation * _cameraPositionOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _positionVelocity,
            _positionSmoothTime);
    }

    private void UpdateCameraRotation()
    {
        Quaternion desiredRotation = CalculateDesiredRotation();
        float rotationLerpRatio = 1f - Mathf.Exp(-(1f / _rotationSmoothTime) * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationLerpRatio);
    }

    private Quaternion CalculateDesiredRotation()
    {
        if (_rotationMode == RotationMode.MatchTargetRotation)
            return _cameraTarget.rotation;

        Vector3 directionToTarget = _cameraTarget.position - transform.position;

        if (directionToTarget == Vector3.zero)
            return transform.rotation;

        return Quaternion.LookRotation(directionToTarget, Vector3.up);
    }
}
