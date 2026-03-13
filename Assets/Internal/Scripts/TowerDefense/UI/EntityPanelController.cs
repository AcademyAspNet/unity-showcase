using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityPanelController : MonoBehaviour
{
    [Header("Target Settings")]

    [SerializeField]
    private GameObject _target;

    [Header("UI Settings")]

    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private TMP_Text _entityNameText;

    [SerializeField]
    private TMP_Text _healthBarValueText;

    [SerializeField]
    private Image _healthBar;

    private Entity _asEntity;
    private DestroyableEntity _asDestroyableEntity;

    public void SetTarget(GameObject target)
    {
        ClearTarget();

        if (target == null)
            return;

        _target = target;

        if (target.TryGetComponent(out Entity entity))
            _asEntity = entity;

        if (target.TryGetComponent(out DestroyableEntity destroyableEntity))
            _asDestroyableEntity = destroyableEntity;
    }

    public void ClearTarget()
    {
        _target = null;
        _asEntity = null;
        _asDestroyableEntity = null;
    }

    private void FixedUpdate()
    {
        if (_target == null)
            return;

        UpdateEntityName();
        UpdateProgressBar();
        UpdatePanelPosition();
    }

    private void UpdateEntityName()
    {
        if (_asEntity == null)
            return;

        _entityNameText.text = _asEntity.Name;
    }

    private void UpdateProgressBar()
    {
        if (_asDestroyableEntity == null)
            return;

        float currentHealth = _asDestroyableEntity.GetHealth();
        float maxHealth = _asDestroyableEntity.GetMaxHealth();

        float healthRatio = currentHealth / maxHealth;

        _healthBarValueText.text = Mathf.Floor(currentHealth) + " / " + maxHealth;

        _healthBar.type = Image.Type.Filled;
        _healthBar.fillMethod = Image.FillMethod.Horizontal;
        _healthBar.fillOrigin = 0;
        _healthBar.fillAmount = healthRatio;
    }

    private void UpdatePanelPosition()
    {
        // Мировая позиция над врагом (можно добавить смещение по высоте)
        Vector3 worldPosition = _target.transform.position + Vector3.up * 4f;

        // Преобразуем мировую позицию в экранные координаты
        Camera camera = Camera.main;
        Vector3 screenPoint = camera.WorldToScreenPoint(worldPosition);

        // Определяем камеру для Canvas (зависит от режима рендеринга)
        Camera canvasCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

        // Преобразуем экранную точку в локальную позицию внутри Canvas
        RectTransform canvasRect = _canvas.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvasCamera, out Vector2 localPoint))
        {
            transform.localPosition = localPoint;
        }
    }
}
