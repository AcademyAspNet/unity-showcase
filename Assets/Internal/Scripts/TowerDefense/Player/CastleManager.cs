using UnityEngine;

public class CastleManager : MonoBehaviour
{
    [Header("General Settings")]

    [SerializeField]
    private DestroyableEntity _castle;

    [SerializeField]
    private GameObject _gameOverMenu;

    [SerializeField]
    private GameObject _mainCamera;

    [SerializeField]
    private GameObject _cutsceneCamera;

    private bool _isActivated;

    private void Start()
    {
        _gameOverMenu.SetActive(false);
        _cutsceneCamera.SetActive(false);

        _isActivated = false;
    }

    private void FixedUpdate()
    {
        if (_castle.IsAlive())
            return;

        if (_isActivated)
            return;

        OnGameOver();

        _isActivated = true;
    }

    private void OnGameOver()
    {
        _mainCamera.SetActive(false);
        _cutsceneCamera.SetActive(true);
        _gameOverMenu.SetActive(true);
    }
}
