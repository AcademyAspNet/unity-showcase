using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Класс "Менеджер волн врагов" - главный дирижёр всей системы волн врагов!
// Отвечает за: когда начинать волны, как создавать врагов, переключаться между волнами.
public class EnemyWavesManager : MonoBehaviour
{
    // === НАСТРОЙКИ В ИНСПЕКТОРЕ ===

    // Список конфигураций волн врагов

    // Список всех волн на этом уровне - основной контент уровня
    // Порядок волн: сверху вниз в Inspector = от первой к последней
    [SerializeField]
    private List<EnemyWaveConfiguration> _enemyWaveConfigurations = new();

    // Список начальных маршрутных точек, с которых враг может начать движение

    // Точки, с которых враги начинают путь. Можно задать несколько для разных сторон карты
    [SerializeField]
    private List<Waypoint> _startWaypoints = new();

    // Родительский объект для всех врагов - чтобы не захламлять иерархию объектов на игровой сцене
    [SerializeField]
    private GameObject _enemyParentObject;

    // Пауза перед началом первой волны (в секундах) - время на подготовку игрока
    [SerializeField]
    private int _timeBeforeFirstWave = 5;

    // Интервал между волнами врагов (в секундах)

    // Пауза между волнами (секунды) - время на починку башен, передышку
    [SerializeField]
    private int _intervalBetweenWaves = 15;

    // Событие "Все волны завершены" - вызывается когда волны врагов закончились
    [SerializeField]
    private UnityEvent _onAllEnemyWavesFinished;

    // === ВНУТРЕННИЕ ПЕРЕМЕННЫЕ (состояние системы) ===

    private bool _isPlaying = false; // Идет ли сейчас игра?
    private bool _isWaveStarted = false; // Начата ли текущая волна?
    private bool _isWaitingForNextWave = false; // Ожидаем ли следующую волну?

    private int _currentEnemyWaveIndex = -1; // Индекс текущей волны (-1 = еще не начали)

    // Очередь врагов текущей волны - кто будет появляться следующий
    private Queue<GameObject> _currentEnemiesQueue;

    // Паузы точек спавна: когда каждая точка снова будет доступна
    private readonly Dictionary<Waypoint, float> _startWaypointCooldowns = new();

    // Список созданных врагов - чтобы следить за активными врагами
    private readonly List<GameObject> _spawnedEnemyUnits = new();

    // Тайминги на основе Time.time

    private float _shouldStartFirstWaveAt = -1; // Когда начать первую волну
    private float _shouldStartNextWaveAt = -1; // Когда начать следующую волну

    private float _shouldSpawnNextEnemyUnitAt; // Когда создавать следующего врага

    // Начать игру, запустив первую волну - можно вызвать из UI (кнопка "Начать")
    public void StartWaves()
    {
        _isPlaying = true;
        _shouldStartFirstWaveAt = Time.time + _timeBeforeFirstWave;
    }

    // Start вызывается при старте сцены
    private void Start()
    {
        StartWaves(); // Автоматически начинаем волны
    }

    // Update вызывается каждый кадр - здесь вся логика работы волн
    private void Update()
    {
        if (!_isPlaying)
            return;

        UpdateFirstWave(); // Проверяем, не пора ли начать первую волну
        UpdateWaveSwitch(); // Проверяем переход между волнами

        if (IsWaveActive())
        {
            UpdateEnemyUnits(); // Создаем врагов, если волна активна
        }

        CleanupDestroyedEnemies(); // Убираем врагов из списка, если их не существует на игровой сцене
    }

    // Удаляем уничтоженных врагов из списка _spawnedEnemyUnits
    private void CleanupDestroyedEnemies()
    {
        if (_spawnedEnemyUnits == null)
            return;

        // Идем с конца списка, чтобы не сломать индексы при удалении
        for (int i = _spawnedEnemyUnits.Count - 1; i >= 0; i--)
        {
            if (_spawnedEnemyUnits[i] != null)
                continue;

            _spawnedEnemyUnits.RemoveAt(i);
        }
    }

    // Проверяем, не настало ли время для первой волны
    private void UpdateFirstWave()
    {
        if (_shouldStartFirstWaveAt < 0)
            return;

        if (Time.time < _shouldStartFirstWaveAt)
            return;

        // Время пришло! Начинаем первую волну

        _currentEnemyWaveIndex = -1;
        StartNextWave();

        _shouldStartFirstWaveAt = -1;
    }

    // Основной метод создания врагов в текущей волне
    private void UpdateEnemyUnits()
    {
        float currentTime = Time.time;

        // Еще не время создавать следующего врага
        if (currentTime < _shouldSpawnNextEnemyUnitAt)
            return;

        // Проверяем, есть ли враги в очереди
        if ((_currentEnemiesQueue == null) || (_currentEnemiesQueue.Count < 1))
            return;

        // Берем следующего врага из очереди
        GameObject nextEnemyPrefab = _currentEnemiesQueue.Dequeue();
        bool isEnemySpawned = SpawnEnemyUnit(nextEnemyPrefab);

        // Если враг не создался (нет свободных точек, например), выходим
        if (!isEnemySpawned)
            return;

        // Устанавливаем время для следующего создания врага
        _shouldSpawnNextEnemyUnitAt = currentTime + GetActiveWaveConfiguration().SpawnInterval;
    }

    // Управление переходами между волнами
    private void UpdateWaveSwitch()
    {
        if (!_isPlaying || !_isWaveStarted)
            return;

        // Если волна еще активна (есть враги в очереди или на карте) - не переключаем
        if (IsWaveActive())
            return;

        // Если еще не начали ждать следующую волну, начинаем
        if (!_isWaitingForNextWave)
        {
            _isWaitingForNextWave = true;
            _shouldStartNextWaveAt = Time.time + _intervalBetweenWaves;

            return;
        }

        // Проверяем, не настало ли время для следующей волны
        if (Time.time >= _shouldStartNextWaveAt)
        {
            _isWaitingForNextWave = false;
            _shouldStartNextWaveAt = -1;

            if (HasNextWave())
            {
                StartNextWave(); // Запускаем следующую волну
            }
            else
            {
                // Волны закончились!

                _isPlaying = false;
                _isWaveStarted = false;

                // Запускаем событие "все волны пройдены"
                _onAllEnemyWavesFinished?.Invoke();
            }
        }
    }

    // Проверка: активна ли текущая волна?
    private bool IsWaveActive()
    {
        bool hasEnemiesInQueue = _currentEnemiesQueue != null && _currentEnemiesQueue.Count > 0;
        bool hasAliveEnemies = _spawnedEnemyUnits != null && _spawnedEnemyUnits.Count > 0;

        // Волна активна, если есть враги в очереди ИЛИ есть живые враги на карте
        return hasEnemiesInQueue || hasAliveEnemies;
    }

    // Получить конфигурацию текущей волны
    private EnemyWaveConfiguration GetActiveWaveConfiguration()
    {
        return _enemyWaveConfigurations[_currentEnemyWaveIndex];
    }

    // Проверить, есть ли следующая волна
    private bool HasNextWave()
    {
        return _currentEnemyWaveIndex < (_enemyWaveConfigurations.Count - 1);
    }

    // Начать следующую волну
    private void StartNextWave()
    {
        _currentEnemyWaveIndex++;

        // Берем конфигурацию новой волны
        EnemyWaveConfiguration waveConfiguration = _enemyWaveConfigurations[_currentEnemyWaveIndex];

        // Создаем очередь врагов из конфигурации
        _currentEnemiesQueue = CreateEnemiesQueue(waveConfiguration);

        // Сбрасываем паузы точек появления врагов
        _startWaypointCooldowns.Clear();

        // Очищаем список созданных врагов
        _spawnedEnemyUnits.Clear();

        // Начинаем создавать врагов сразу
        _shouldSpawnNextEnemyUnitAt = 0;

        _isWaveStarted = true;
    }

    // Создание одного врага на сцене
    private bool SpawnEnemyUnit(GameObject enemyPrefab)
    {
        // Находим свободную начальную маршрутную точку
        Waypoint startWaypoint = GetAvailableStartWaypoint();

        if (startWaypoint == null)
            return false; // Все точки заблокированы по времени

        // Создаем врага как дочерний объект _enemyParentObject
        GameObject spawnedGameObject = Instantiate(enemyPrefab, _enemyParentObject.transform);

        if (spawnedGameObject == null)
            return false;

        // Даем понятное имя для отладки
        spawnedGameObject.name = $"Enemy Unit #{_spawnedEnemyUnits.Count + 1}";

        // Ставим врага на позицию точки появления
        spawnedGameObject.transform.position = startWaypoint.transform.position;

        // Настраиваем его поведение: указываем первую точку пути
        EnemyBehaviour enemyBehaviour = spawnedGameObject.GetComponent<EnemyBehaviour>();

        if (enemyBehaviour != null)
            enemyBehaviour.SetTargetWaypoint(startWaypoint);

        // Устанавливаем временную блокировку (паузу) на точку появления
        AddStartWaypointCooldown(startWaypoint);

        // Добавляем врага в общий список созданных врагов
        _spawnedEnemyUnits.Add(spawnedGameObject);

        return true;
    }

    // Метод, позволяющий установить паузу на точку появления врагов
    private void AddStartWaypointCooldown(Waypoint startWaypoint)
    {
        EnemyWaveConfiguration waveConfiguration = GetActiveWaveConfiguration();
        float cooldownDuration = waveConfiguration.SpawnPointCooldown;

        // Вычисляем, когда точка снова станет доступной
        float cooldownFinishAt = Time.time + cooldownDuration;

        if (!_startWaypointCooldowns.ContainsKey(startWaypoint))
        {
            _startWaypointCooldowns.Add(startWaypoint, cooldownFinishAt);
            return;
        }

        _startWaypointCooldowns[startWaypoint] = cooldownFinishAt;
    }

    // Поиск свободной точки появления врага
    private Waypoint GetAvailableStartWaypoint()
    {
        float currentTime = Time.time;

        // LINQ запрос: выбрать точки, у которых пауза уже прошла
        List<Waypoint> availableWaypoints = _startWaypoints
            .Where(waypoint => _startWaypointCooldowns.GetValueOrDefault(waypoint, 0) <= currentTime)
            .ToList();

        if (availableWaypoints.Count == 0)
            return null; // Все точки заняты

        // Выбираем случайную точку из доступных
        int randomIndex = Random.Range(0, availableWaypoints.Count);

        return availableWaypoints[randomIndex];
    }

    // Создать очередь врагов из конфигурации волны
    private static Queue<GameObject> CreateEnemiesQueue(EnemyWaveConfiguration waveConfiguration)
    {
        List<GameObject> allEnemies = new();

        // Проходим по всем группам врагов в волне
        foreach (EnemyGroupConfiguration groupConfiguration in waveConfiguration.EnemyGroups)
        {
            // Выбираем случайное количество врагов в группе
            int enemyUnitCount = Random.Range(
                groupConfiguration.MinCount,
                groupConfiguration.MaxCount + 1 // +1 потому что Random.Range для int не включает верхнюю границу
            );

            if (enemyUnitCount < 1)
                continue;

            // Добавляем нужное количество префабов в список
            for (int i = 0; i < enemyUnitCount; i++)
                allEnemies.Add(groupConfiguration.EnemyPrefab);
        }

        // Перемешиваем список, чтобы враги появлялись в случайном порядке
        ShuffleList(allEnemies);

        // Превращаем список в очередь (первый вошел - первый вышел)
        return new Queue<GameObject>(allEnemies);
    }

    // Алгоритм перемешивания списка (тасование Фишера-Йетса)
    private static void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // Меняем местами элементы
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}
