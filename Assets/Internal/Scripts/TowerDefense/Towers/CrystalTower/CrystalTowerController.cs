using System.Collections.Generic;
using UnityEngine;

public class CrystalTowerController : BaseTowerController
{
    [Header("Crystal Projectile Settings")]

    [SerializeField]
    private GameObject _crystalProjectilePrefab;

    [SerializeField]
    private Transform _crystalProjectileSpawnPoint;

    [Header("Attack Settings")]

    [SerializeField]
    private float _attackInterval = 1f;

    [Header("Animator Settings")]

    [SerializeField]
    private string _attackAnimatorTriggerName = "Attack";

    private void FixedUpdate()
    {
        if (HasAttackCooldown())
            return;

        GameObject target = GetTarget();

        if (target == null)
            return;

        CreateCrystalProjectileFor(target.transform);
        SetAttackCooldown(_attackInterval);

        GetAnimator().SetTrigger(_attackAnimatorTriggerName);
    }

    private void CreateCrystalProjectileFor(Transform target)
    {
        GameObject createdCannonShell = Instantiate(
            _crystalProjectilePrefab,
            _crystalProjectileSpawnPoint.position,
            Quaternion.identity
        );

        createdCannonShell
            .GetComponent<CrystalProjectileController>()
            .SetTarget(target);
    }
}
