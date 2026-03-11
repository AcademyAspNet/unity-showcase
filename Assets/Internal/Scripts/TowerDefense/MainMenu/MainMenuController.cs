using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("External References")]

    [SerializeField]
    private Animator _canvasAnimator;

    [Header("Trigger Names")]

    [SerializeField]
    private string _startNewGameTriggerName = "StartNewGame";

    [SerializeField]
    private string _openSettingsTriggerName = "ToSettingsMenu";

    [SerializeField]
    private string _exitGameTriggerName = "ExitGame";

    public void StartNewGame()
    {
        _canvasAnimator.SetTrigger(_startNewGameTriggerName);
    }

    public void OpenSettings()
    {
        _canvasAnimator.SetTrigger(_openSettingsTriggerName);
    }

    public void ExitGame()
    {
        _canvasAnimator.SetTrigger(_exitGameTriggerName);
    }

    public void SwitchToFirstLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}
