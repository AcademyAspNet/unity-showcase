using UnityEngine;

// Класс "Поведение врага" - логика поведения каждого врага по отдельности
// Отвечает за движение по пути, повороты и переход между точками
public class EnemyBehaviour : MonoBehaviour
{
    // === НАСТРОЙКИ ДВИЖЕНИЯ ===
    // Эти параметры можно менять в Inspector для разных типов врагов

    [Header("Enemy Movement")] // Заголовок в Inspector для удобства

    [SerializeField]
    private float _movementSpeed = 3f; // Скорость перемещения (единиц в секунду)

    [SerializeField]
    private float _rotationSpeed = 3f; // Скорость поворота (плавность)

    [SerializeField]
    private float _arrivalDistance = 0.2f; // На каком расстоянии считать, что враг "дошел" до точки

    [Header("Animator Parameters")]

    [SerializeField]
    private string _isMovingAnimatorParameter = "IsMoving";

    private Animator _animator;

    // Текущая целевая точка пути, к которой враг сейчас идет
    private Waypoint _targetWaypoint;

    // Публичный метод для установки начальной точки пути
    // Вызывается при создании врага
    public void SetTargetWaypoint(Waypoint targetWaypoint)
    {
        _targetWaypoint = targetWaypoint;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update вызывается каждый кадр - здесь основная логика движения
    private void Update()
    {
        // Если целевой точки нет (например, не установили) - ничего не делаем
        if (_targetWaypoint == null)
            return;

        // 1. Поворачиваемся к целевой точке
        RotateToPosition(_targetWaypoint.transform.position);

        // 2. Двигаемся к целевой точке
        MoveToPosition(_targetWaypoint.transform.position);

        // 3. Проверяем, не дошли ли уже до точки, чтобы выбрать следующую
        SelectNextTargetWaypointIfPossible();
    }

    private void FixedUpdate()
    {
        bool hasTargetPoint = _targetWaypoint != null;
        _animator.SetBool(_isMovingAnimatorParameter, hasTargetPoint);
    }

    // Метод плавного поворота врага в сторону цели
    private void RotateToPosition(Vector3 position)
    {
        // Вычисляем направление: от текущей позиции к цели
        Vector3 direction = (position - transform.position).normalized;

        // Если направление нулевое (мы уже на месте) - выходим
        if (direction == Vector3.zero)
            return;

        // Создаем "целевой поворот" - куда мы должны смотреть
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Плавно (Slerp) поворачиваем от текущего поворота к целевому
        // Time.deltaTime делает поворот независящим от частоты кадров
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime
        );
    }

    // Метод движения к цели
    private void MoveToPosition(Vector3 position)
    {
        // MoveTowards плавно перемещает объект к цели с заданной скоростью
        // Time.deltaTime гарантирует, что скорость будет в единицах/секунду
        transform.position = Vector3.MoveTowards(
            transform.position,
            position,
            _movementSpeed * Time.deltaTime
        );
    }

    // Метод проверки: дошли ли до текущей точки и нужно ли выбрать следующую
    private void SelectNextTargetWaypointIfPossible()
    {
        // Вычисляем расстояние до текущей целевой точки
        float distanceToTargetWaypoint = Vector3.Distance(
            transform.position,
            _targetWaypoint.transform.position
        );

        // Если расстояние больше порогового (_arrivalDistance) - еще не дошли
        if (distanceToTargetWaypoint > _arrivalDistance)
            return;

        // Дошли! Выбираем следующую точку из возможных вариантов
        _targetWaypoint = GetNextWaypoint(_targetWaypoint);
    }

    // Статический метод для выбора следующей точки пути
    private static Waypoint GetNextWaypoint(Waypoint sourceWaypoint)
    {
        // Если текущей точки нет или у нее нет следующих точек - значит это конец пути
        if (sourceWaypoint == null || sourceWaypoint.NextWaypoints.Count < 1)
            return null;

        // Выбираем случайную следующую точку из списка
        // Это создает элемент неопределенности: враги могут ходить разными путями
        int randomIndex = Random.Range(0, sourceWaypoint.NextWaypoints.Count);

        // Возвращаем выбранную следующую точку
        return sourceWaypoint.NextWaypoints[randomIndex];
    }
}
