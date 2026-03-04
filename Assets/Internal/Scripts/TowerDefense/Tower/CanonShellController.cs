using UnityEngine;

public class CannonShellController : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _lifeTime = 10f;

    [SerializeField]
    private Transform _target;

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
        if (!other.CompareTag("Enemy"))
            return;

        Destroy(gameObject);
    }
}
