using UnityEngine;

public class EnergyTowerController : BaseTowerController
{
    [Header("Attack Settings")]

    [SerializeField]
    private float _damagePerSecond = 5;

    [Header("Energy Line Settings")]

    [SerializeField]
    private Transform _energyLineStartPosition;

    [SerializeField]
    private Material _energyLineMaterial;

    [SerializeField]
    private float _energyLineWidth = 0.1f;

    [SerializeField]
    private Color _energyLineColor = Color.blueViolet;

    private LineRenderer _lineRenderer;
    private GameObject _energyLineObject;

    [Header("Animator Settings")]

    [SerializeField]
    private string _attackingAnimatorBoolName = "Attacking";

    private void Start()
    {
        CreateEnergyLineObject();
    }

    private void CreateEnergyLineObject()
    {
        _energyLineObject = new GameObject("Energy Tower Line");

        _energyLineObject.transform.SetParent(transform);
        _energyLineObject.transform.localPosition = Vector3.zero;

        _lineRenderer = _energyLineObject.AddComponent<LineRenderer>();

        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = _energyLineWidth;
        _lineRenderer.endWidth = _energyLineWidth;

        _lineRenderer.material = _energyLineMaterial
            ?? new Material(Shader.Find("Sprites/Default"));

        _lineRenderer.startColor = _energyLineColor;
        _lineRenderer.endColor = _energyLineColor;
        _lineRenderer.enabled = false;
    }

    private void Update()
    {
        UpdateEnergyLine();
    }

    private void UpdateEnergyLine()
    {
        GameObject target = GetTarget();

        if (target == null)
        {
            if (_lineRenderer.enabled)
            {
                _lineRenderer.enabled = false;
                GetAnimator().SetBool(_attackingAnimatorBoolName, false);
            }

            return;
        }

        _lineRenderer.SetPosition(0, _energyLineStartPosition.position);
        _lineRenderer.SetPosition(1, target.transform.position);

        if (!_lineRenderer.enabled)
        {
            _lineRenderer.enabled = true;
            GetAnimator().SetBool(_attackingAnimatorBoolName, true);
        }

        AttackTarget(target);
    }

    private void AttackTarget(GameObject target)
    {
        float damageForThisTick = _damagePerSecond * Time.deltaTime;
        Attack(target, damageForThisTick);
    }
}
