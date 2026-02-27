using UnityEngine;

public class DialogController : MonoBehaviour
{
    [Header("External References")]

    [SerializeField]
    private Animator _canvasAnimator;

    [Header("Trigger Names")]

    [SerializeField]
    private string _hideDialogTriggerName = "ToMainMenu";

    public void HideDialog()
    {
        _canvasAnimator.SetTrigger(_hideDialogTriggerName);
    }
}
