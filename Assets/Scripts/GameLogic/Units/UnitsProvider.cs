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
    float _initialUnitsDamage;
    [SerializeField]
    float _unitsDamageUpgradeValue;
    [SerializeField]
    TechTreeNodeId _unitsDamageTechTreeNode;

    [SerializeField]
    GameObject _deploymentZone;
    [SerializeField]
    GameObject _unitPrefab;

    public event Action onActivated;
    public event Action<int> onUnitsCountChanged;
    public event Action<int> onMaxUnitsCountChanged;
    public event Action<float> onProductionTimeChanged;
    public event Action<float> onUnitsDamageChanged;

    public Defines.UnitType unitType { get { return _unitType; } }
    public bool isActivated { get; private set; }
    public int unitsCount { get; private set; }
    public int maxUnitsCount { get; private set; }
    public float productionTime { get; private set; }
    public float unitsDamage { get; private set; }

    private Dictionary<TechTreeNodeId, Action> _upgrades = new Dictionary<TechTreeNodeId, Action>();

    private void Start()
    {
        unitsCount = _initialUnitsCount;
        maxUnitsCount = _initialMaxUnitsCount;
        productionTime = _initialProductionTime;
        unitsDamage = _initialUnitsDamage;

        _upgrades.Add(_activationTechTreeNode, Activate);
        _upgrades.Add(_productionTimeTechTreeNode, UpgradeProductionTime);
        _upgrades.Add(_maxUnitsCountTechTreeNode, UpgradeMaxUnitsCount);
        _upgrades.Add(_unitsDamageTechTreeNode, UpgradeUnitsDamage);

        Game.techTreeManager.onResearchFinished += OnResearchFinished;
    }

    private void Activate()
    {
        isActivated = true;
        if (onActivated != null)
            onActivated();
        if (unitsCount < maxUnitsCount)
            StartUnitProduction();
    }

    private void UpgradeProductionTime()
    {
        productionTime -= _productionTimeUpgradeValue;
        if (onProductionTimeChanged != null)
            onProductionTimeChanged(productionTime);
    }

    private void UpgradeMaxUnitsCount()
    {
        maxUnitsCount += _maxUnitsCountUpgradeValue;
        if (onMaxUnitsCountChanged != null)
            onMaxUnitsCountChanged(maxUnitsCount);
    }

    private void UpgradeUnitsDamage()
    {
        unitsDamage += _unitsDamageUpgradeValue;
        if (onUnitsDamageChanged != null)
            onUnitsDamageChanged(unitsDamage);
    }

    private void StartUnitProduction()
    {
        Process p = new TimeProcess("Producing: " + _unitType, productionTime);
        p.onFinished += OnUnitProduced;
        Game.processesManager.StartProcess(p);
    }

    private void OnUnitProduced()
    {
        unitsCount++;
        if (onUnitsCountChanged != null)
            onUnitsCountChanged(unitsCount);
        if (unitsCount < maxUnitsCount)
            StartUnitProduction();
    }

    private void OnResearchFinished(TechTreeNodeId nodeId)
    {
        Action upgradeAction = null;
        if (_upgrades.TryGetValue(nodeId, out upgradeAction))
            upgradeAction();
    }
}
