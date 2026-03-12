using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CrystalProjectileController : MonoBehaviour
{
    [Header("Projectile Settings")]

    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _lifeTime = 10f;

    [SerializeField]
    private float _damage = 25;

    [Header("Target Settings")]

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private List<string> _enemyTags = new() { "Enemy" };

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        if (_target == null)
            return;

        Vector3 direction = (_target.position - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;

        transform.LookAt(_target);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidTarget(other))
            return;

        if (!other.TryGetComponent<DestroyableEntity>(out DestroyableEntity entity))
            return;

        if (entity.IsAlive())
            entity.TakeDamage(_damage);

        Destroy(gameObject);
    }

    private bool IsValidTarget(Collider other)
    {
        foreach (string enemyTag in _enemyTags)
        {
            if (other.CompareTag(enemyTag))
                return true;
        }

        return false;
    }
}
