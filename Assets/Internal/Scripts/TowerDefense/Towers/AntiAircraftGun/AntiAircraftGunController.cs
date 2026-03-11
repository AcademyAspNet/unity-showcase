using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AntiAircraftGunController : BaseTowerController
{
    [Header("Procedural Animation Settings")]

    [SerializeField]
    private List<Transform> _rotatableObjects = new();

    [SerializeField]
    private List<Transform> _aimableObjects = new();

    [SerializeField]
    private float _rotationSpeed = 3f;

    private Vector3 _currentTargetPosition;
    private Vector3 _nextTargetPosition;

    private void Update()
    {
        UpdateTargetPosition();
        InterpolateTargetPosition();
        ApplyRotationAndAim();
    }

    private void UpdateTargetPosition()
    {
        GameObject target = GetTarget();

        if (!target)
            return;

        _nextTargetPosition = target.transform.position;
    }

    private void InterpolateTargetPosition()
    {
        if (_nextTargetPosition == null)
            return;

        _currentTargetPosition = Vector3.Lerp(
            _currentTargetPosition,
            _nextTargetPosition,
            _rotationSpeed * Time.deltaTime);
    }

    private void ApplyRotationAndAim()
    {
        Vector3 targetPosition = _currentTargetPosition;

        if (targetPosition == null)
            return;

        foreach (Transform rotatableObject in _rotatableObjects)
        {
            targetPosition.y = rotatableObject.position.y;
            rotatableObject.LookAt(targetPosition);
        }

        foreach (Transform aimableObject in _aimableObjects)
        {
            aimableObject.LookAt(targetPosition);
        }
    }
}
