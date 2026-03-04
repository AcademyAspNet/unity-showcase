using System.Collections.Generic;
using UnityEngine;

public class CannonTowerController : MonoBehaviour
{
    [SerializeField]
    private GameObject _cannonShellPrefab;

    [SerializeField]
    private float _attackInterval = 1f;

    private List<GameObject> _targets = new List<GameObject>();
    private float _nextAttackAt = 0f;

    private void FixedUpdate()
    {
        if (_nextAttackAt <= Time.time)
        {
            Transform target = GetTarget();

            if (target != null)
            {
                Attack(target);
                _nextAttackAt = Time.time + _attackInterval;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        _targets.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        _targets.Remove(other.gameObject);
    }

    private Transform GetTarget()
    {
        if (_targets.Count < 1)
            return null;

        int randomIndex = Random.Range(0, _targets.Count);

        return _targets[randomIndex].transform;
    }

    private void Attack(Transform target)
    {
        GameObject createdCannonShell = Instantiate(
            _cannonShellPrefab,
            transform.position,
            Quaternion.identity
        );

        createdCannonShell
            .GetComponent<CannonShellController>()
            .SetTarget(target);
    }
}
