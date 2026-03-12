using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Main Settings")]

    [SerializeField]
    private GameObject _loadingScreen;

    [SerializeField]
    private Slider _progressBar;

    [SerializeField]
    private TMP_Text _progressText;

    [SerializeField]
    private TMP_Text _hintText;

    [Header("Hint Settings")]

    [SerializeField]
    private List<string> _hints = new();

    private float _loadingProgress = 0f;
    private bool _isMouseClicked = false;

    public void LoadLevel(int sceneIndex)
    {
        _loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    private void Start()
    {
        _loadingScreen.SetActive(false);

        _progressBar.minValue = 0;
        _progressBar.maxValue = 1;
        _progressBar.value = 0;

        _progressText.text = "0 %";

        if (_hints.Count > 0)
        {
            int randomIndex = Random.Range(0, _hints.Count);
            string randomHint = _hints[randomIndex];

            _hintText.text = randomHint;
        }
        else
        {
            _hintText.text = "[Список подсказок не заполнен]";
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && _loadingProgress == 1f)
        {
            _isMouseClicked = true;
        }
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOperation.allowSceneActivation = false;

        while (_loadingProgress < 1f)
        {
            _loadingProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            _progressBar.value = _loadingProgress;
            _progressText.text = (_loadingProgress * 100f) + " %";

            yield return null;
        }

        _hintText.text = "[НАЖМИТЕ ЛЕВУЮ КНОПКУ МЫШИ, ЧТОБЫ НАЧАТЬ ИГРУ]";

        while (!_isMouseClicked)
        {
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
    }
}
