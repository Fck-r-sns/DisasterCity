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
    TechTreeNodeId _deplymentTimeTechTreeNode;

    [SerializeField]
    GameObject _deploymentZone;
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
    int _unitsCountToDeploy;

    public void StartDeployment(Vector3 position)
    {
        Game.instance.SetDefaultGameMode();
        SetDeploymentZoneVisible(false);
        _unitsCountToDeploy = unitsCount;
        unitsCount = 0;
        StartDeploymentProcess(position);
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

        Game.techTreeManager.onResearchFinished += OnResearchFinished;
        Game.instance.onGameModeChanged += OnGameModeChanged;
    }

    private void Activate()
    {
        isActivated = true;
        if (onActivated != null)
            onActivated();
        if (unitsCount < maxUnitsCount)
            StartUnitProductionProcess();
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

    private void StartUnitProductionProcess()
    {
        Process p = new TimeProcess("Producing: " + _unitType, productionTime);
        p.onFinished += OnUnitProduced;
        Game.processesManager.StartProcess(p);
    }

    private void StartDeploymentProcess(Vector3 position)
    {
        Process p = new TimeProcess("Arriving: " + _unitType, deploymentTime);
        p.onFinished += () => OnDeploymentProcessFinished(position);
        Game.processesManager.StartProcess(p);
    }

    private void OnGameModeChanged(Game.Mode mode, object context)
    {
        SetDeploymentZoneVisible(mode == Game.Mode.CallUnits && (Defines.UnitType)context == _unitType);
    }

    private void OnUnitProduced()
    {
        unitsCount++;
        if (onUnitsCountChanged != null)
            onUnitsCountChanged(unitsCount);
        if (unitsCount < maxUnitsCount)
            StartUnitProductionProcess();
    }

    private void OnDeploymentProcessFinished(Vector3 position)
    {
        for (int i = 0; i < _unitsCountToDeploy; i++)
            Instantiate(_unitPrefab, position, Quaternion.identity);
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
