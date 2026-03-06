using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TowerTargetPriority
{
    Random,
    Nearest,
    First,
    Last
}

[RequireComponent(typeof(Animator))]
public abstract class BaseTowerController : MonoBehaviour
{
    [Header("Target Settings")]

    [SerializeField]
    private List<string> _targetTags = new() { "Enemy" };

    [SerializeField]
    private TowerTargetPriority _targetSelectMode;

    private readonly List<GameObject> _targets = new();
    private Animator _animator;

    private float _nextAttackAt = 0f;

    protected GameObject GetTarget()
    {
        return _targetSelectMode switch
        {
            TowerTargetPriority.Random => GetRandomTarget(),
            TowerTargetPriority.Nearest => GetNearestTarget(),
            TowerTargetPriority.First => GetFirstTarget(),
            TowerTargetPriority.Last => GetLastTarget(),
            _ => null,
        };
    }

    protected GameObject GetTarget(Func<GameObject, bool> predicate)
    {
        foreach (GameObject target in _targets)
            if (predicate.Invoke(target))
                return target;

        return null;
    }

    protected List<GameObject> GetTargets()
    {
        return new List<GameObject>(_targets);
    }

    protected List<GameObject> GetTargets(Func<GameObject, bool> predicate)
    {
        List<GameObject> targets = new();

        foreach (GameObject target in _targets)
            if (predicate.Invoke(target))
                targets.Add(target);

        return targets;
    }

    protected GameObject GetFirstTarget()
    {
        return _targets.FirstOrDefault();
    }

    protected GameObject GetLastTarget()
    {
        return _targets.LastOrDefault();
    }

    protected GameObject GetRandomTarget()
    {
        if (_targets.Count < 1)
            return null;

        int randomIndex = Random.Range(0, _targets.Count);
        return _targets[randomIndex];
    }

    protected GameObject GetNearestTarget()
    {
        if (_targets.Count < 1)
            return null;

        Vector3 towerPosition = transform.position;

        return _targets
            .OrderBy(target => Vector3.Distance(towerPosition, target.transform.position))
            .FirstOrDefault();
    }

    protected Animator GetAnimator()
    {
        return _animator;
    }

    protected bool HasAttackCooldown()
    {
        return _nextAttackAt > Time.time;
    }

    protected void SetAttackCooldown(float duration)
    {
        _nextAttackAt = Time.time + duration;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ShouldProcessCollider(other))
            return;

        _targets.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!ShouldProcessCollider(other))
            return;

        _targets.Remove(other.gameObject);
    }

    private bool ShouldProcessCollider(Collider other)
    {
        foreach (string targetTag in _targetTags)
            if (other.CompareTag(targetTag))
                return true;

        return false;
    }
}
