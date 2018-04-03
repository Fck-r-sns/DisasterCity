using System;
using UnityEngine;

public abstract class UnitsProvider : MonoBehaviour
{
    [SerializeField]
    Defines.UnitType _unitType;
    [SerializeField]
    int _initialUnitsCount;
    [SerializeField]
    int _initialMaxUnitsCount;
    [SerializeField]
    float _initialProductionTime;
    [SerializeField]
    GameObject _deploymentZone;
    [SerializeField]
    GameObject _unitPrefab;

    public event Action onBecameEnabled;
    public event Action<int> onUnitsCountChanged;
    public event Action<int> onMaxUnitsCountChanged;

    public bool isEnabled { get; private set; }
    public int unitsCount { get; protected set; }
    public int maxUnitsCount { get; protected set; }
    public float productionTime { get; protected set; }

    private void Start()
    {
        unitsCount = _initialUnitsCount;
        maxUnitsCount = _initialMaxUnitsCount;
        productionTime = _initialProductionTime;

        Game.techTreeManager.onResearchFinished += OnResearchFinished;
    }

    protected void Enable()
    {
        isEnabled = true;
        if (onBecameEnabled != null)
            onBecameEnabled();
        if (unitsCount < maxUnitsCount)
            StartUnitProduction();
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

    protected abstract void OnResearchFinished(TechTreeNodeId nodeId);
}
