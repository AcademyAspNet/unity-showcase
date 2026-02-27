using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("External References")]

    [SerializeField]
    private Animator _cameraAnimator;

    [Header("Trigger Names")]

    [SerializeField]
    private string _changeCameraAngleTriggerName = "ChangeCameraAngle";

    public void ChangeCameraAngle()
    {
        _cameraAnimator.SetTrigger(_changeCameraAngleTriggerName);
    }
}
