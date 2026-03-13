using UnityEngine;

public class EntitySelector : MonoBehaviour
{
    [Header("Camera Settings")]

    [SerializeField]
    private Camera _camera;

    [Header("UI Settings")]

    [SerializeField]
    private EntityPanelController _entityPanelController;

    [Header("Raycast Settings")]

    [SerializeField]
    private LayerMask _selectLayers = -1;

    [SerializeField]
    private float _maxRayDistance = Mathf.Infinity;

    private void Start()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryToSelectEntity();
    }

    private void TryToSelectEntity()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, _maxRayDistance, _selectLayers))
        {
            GameObject selectedGameObject = hit.collider.gameObject;
            _entityPanelController.SetTarget(selectedGameObject);

            return;
        }

        _entityPanelController.ClearTarget();
    }
}
