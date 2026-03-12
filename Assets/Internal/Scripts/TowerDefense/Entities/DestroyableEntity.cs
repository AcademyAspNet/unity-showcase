using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class DestroyableEntity : MonoBehaviour
{
    [Header("Health Settings")]

    [SerializeField]
    private float _defaultHealth = 100f;

    [SerializeField]
    private float _maxHealth = 100f;

    [Header("Animator Settings")]

    [SerializeField]
    private string _destroyAnimatorTriggerName = "Destroy";

    private Animator _animator;

    private float _health = 100f;
    private bool _isAlive = true;

    public float GetHealth()
    {
        return _health;
    }

    public bool SetHealth(float health)
    {
        if (!_isAlive)
        {
            Debug.LogWarning(
                "Нельзя изменить значение здоровья сущности, у которой" +
                "состояние IsAlive() равно false!");

            return false;
        }

        float clampedHealth = Mathf.Clamp(health, 0, _maxHealth);

        OnHealthUpdate(_health, clampedHealth);

        _health = clampedHealth;
        _isAlive = clampedHealth > 0;

        return true;
    }

    public bool IsAlive()
    {
        return _isAlive;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive())
        {
            Debug.LogWarning(
                "Попытка применить урон к сущности с состоянием IsAlive() равным false!");

            return;
        }

        float nextHealth = _health - damage;

        OnTakingDamage(damage, nextHealth);
        SetHealth(nextHealth);
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_defaultHealth < 0)
        {
            Debug.LogError(
                "Начальное здоровье сущности не должно быть отрицательным! " +
                "Значение было автоматически преобразовано в положительное.");

            _defaultHealth = Mathf.Abs(_defaultHealth);
        }

        if (_maxHealth < 0)
        {
            Debug.LogError(
                "Максимальное здоровье сущности не должно быть отрицательным! " +
                "Значение было автоматически преобразовано в положительное.");

            _maxHealth = Mathf.Abs(_maxHealth);
        }

        SetHealth(_defaultHealth);
    }

    private void OnHealthUpdate(float previousValue, float newValue)
    {
        if (newValue <= 0)
        {
            if (_animator == null)
            {
                Destroy();
                return;
            }

            _animator.SetTrigger(_destroyAnimatorTriggerName);
        }
    }

    private void OnTakingDamage(float damage, float nextHealth)
    {

    }
}
