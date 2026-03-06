using UnityEngine;

public class CrystalProjectileController : MonoBehaviour
{
    [Header("Projectile Settings")]

    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _lifeTime = 10f;

    [Header("Target Settings")]

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private string _enemyTag = "Enemy";

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
        if (!other.CompareTag(_enemyTag))
            return;

        Destroy(gameObject);
    }
}
