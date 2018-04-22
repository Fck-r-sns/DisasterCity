using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitsProvider : MonoBehaviour
{
    [SerializeField]
    Defines.UnitType _unitType;
    [SerializeField]
    TechTreeNodeId _activationTechTreeNode;

    [SerializeField]
    int _initialUnitsCount;
    [SerializeField]
    float _initialUnitsDamage;

    [SerializeField]
    int _initialMaxUnitsCount;
    [SerializeField]
    int _maxUnitsCountUpgradeValue;
    [SerializeField]
    TechTreeNodeId _maxUnitsCountTechTreeNode;

    [SerializeField]
    float _initialProductionTime;
    [SerializeField]
    float _productionTimeUpgradeValue;
    [SerializeField]
    TechTreeNodeId _productionTimeTechTreeNode;

    [SerializeField]
    float _initialDeploymentTime;
    [SerializeField]
    float _deploymentTimeUpgradeValue;
    [SerializeField]
    TechTreeNodeId _deploymentTimeTechTreeNode;

    [SerializeField]
    GameObject _deploymentZone;
    [SerializeField]
    float _unitsFormationGap;
    [SerializeField]
    GameObject _unitPrefab;

    public event Action onActivated;
    public event Action<int> onUnitsCountChanged;
    public event Action<int> onMaxUnitsCountChanged;
    public event Action<float> onProductionTimeChanged;
    public event Action<float> onDeploymentTimeChanged;

    public Defines.UnitType unitType { get { return _unitType; } }
    public bool isActivated { get; private set; }
    public int unitsCount { get; private set; }
    public int maxUnitsCount { get; private set; }
    public float productionTime { get; private set; }
    public float deploymentTime { get; private set; }
    public float unitsDamage { get; private set; }

    private Dictionary<TechTreeNodeId, Action> _upgrades = new Dictionary<TechTreeNodeId, Action>();
    private Process _productionProcess;

    public void StartDeployment(Vector3 position, Vector3 direction)
    {
        Game.instance.SetDefaultGameMode();
        StartDeploymentProcess(position, direction, unitsCount);
        unitsCount = 0;
        if (onUnitsCountChanged != null)
            onUnitsCountChanged(unitsCount);
    }

    private void Start()
    {
        unitsCount = _initialUnitsCount;
        unitsDamage = _initialUnitsDamage;
        maxUnitsCount = _initialMaxUnitsCount;
        productionTime = _initialProductionTime;
        deploymentTime = _initialDeploymentTime;

        _upgrades.Add(_activationTechTreeNode, Activate);
        _upgrades.Add(_productionTimeTechTreeNode, UpgradeProductionTime);
        _upgrades.Add(_maxUnitsCountTechTreeNode, UpgradeMaxUnitsCount);
        _upgrades.Add(_deploymentTimeTechTreeNode, UpgradeDeploymentTime);

        onUnitsCountChanged += count => TryStartUnitProductionProcess();
        onMaxUnitsCountChanged += count => TryStartUnitProductionProcess();

        Game.techTreeManager.onResearchFinished += OnResearchFinished;
        Game.instance.onGameModeChanged += OnGameModeChanged;
    }

    private void Activate()
    {
        isActivated = true;
        if (onActivated != null)
            onActivated();
        TryStartUnitProductionProcess();
    }

    private void UpgradeProductionTime()
    {
        productionTime -= _productionTimeUpgradeValue;
        if (onProductionTimeChanged != null)
            onProductionTimeChanged(productionTime);
    }

    private void UpgradeDeploymentTime()
    {
        deploymentTime -= _deploymentTimeUpgradeValue;
        if (onDeploymentTimeChanged != null)
            onDeploymentTimeChanged(deploymentTime);
    }

    private void UpgradeMaxUnitsCount()
    {
        maxUnitsCount += _maxUnitsCountUpgradeValue;
        if (onMaxUnitsCountChanged != null)
            onMaxUnitsCountChanged(maxUnitsCount);
    }

    private void TryStartUnitProductionProcess()
    {
        if (unitsCount >= maxUnitsCount || _productionProcess != null)
            return;

        _productionProcess = new TimeProcess("Producing: " + _unitType, productionTime);
        _productionProcess.onFinished += OnProductionProcessFinished;
        Game.processesManager.StartProcess(_productionProcess);
    }

    private void StartDeploymentProcess(Vector3 position, Vector3 direction, int unitsCount)
    {
        Process p = new TimeProcess("Arriving: " + _unitType, deploymentTime);
        p.onFinished += () => OnDeploymentProcessFinished(position, direction, unitsCount);
        Game.processesManager.StartProcess(p);
    }

    private void OnGameModeChanged(Game.Mode mode, object context)
    {
        SetDeploymentZoneVisible(mode == Game.Mode.CallUnits && (Defines.UnitType)context == _unitType);
    }

    private void OnProductionProcessFinished()
    {
        _productionProcess = null;
        unitsCount++;
        if (onUnitsCountChanged != null)
            onUnitsCountChanged(unitsCount);
        TryStartUnitProductionProcess();
    }

    private void OnDeploymentProcessFinished(Vector3 position, Vector3 direction, int unitsCount)
    {
        Vector3 formationDirection = (Quaternion.Euler(0, 90f, 0) * direction).normalized;
        for (int i = 0; i < unitsCount; i++)
        {
            float formationGap = _unitsFormationGap * ((i + 1) / 2) * Mathf.Sign(i % 2 - 1);
            Instantiate(_unitPrefab, position + formationDirection * formationGap, Quaternion.LookRotation(direction));
        }
    }

    private void OnResearchFinished(TechTreeNodeId nodeId)
    {
        Action upgradeAction = null;
        if (_upgrades.TryGetValue(nodeId, out upgradeAction))
            upgradeAction();
    }

    private void SetDeploymentZoneVisible(bool visible)
    {
        _deploymentZone.gameObject.SetActive(visible);
    }
}
