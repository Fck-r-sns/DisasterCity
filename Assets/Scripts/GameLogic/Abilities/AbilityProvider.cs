using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityProvider : MonoBehaviour
{
    [SerializeField]
    Defines.AbilityType _abilityType;
    [SerializeField]
    TechTreeNodeId _activationTechTreeNode;

    [SerializeField]
    float _initialRadius;
    [SerializeField]
    bool _initiallyLoaded;

    [SerializeField]
    float _initialDelayTime;
    [SerializeField]
    float _delayTimeUpgradeValue;
    [SerializeField]
    TechTreeNodeId _delayTimeTechTreeNode;

    [SerializeField]
    float _initialReloadTime;
    [SerializeField]
    float _reloadTimeUpgradeValue;
    [SerializeField]
    TechTreeNodeId _reloadTimeTechTreeNode;

    [SerializeField]
    float _initialDamage;
    [SerializeField]
    float _damageUpgradeValue;
    [SerializeField]
    TechTreeNodeId _damageTechTreeNode;

    [SerializeField]
    GameObject _affectedAreaMarker;
    [SerializeField]
    Ability _abilityLogic;

    public event Action onActivated;
    public event Action<bool> onLoadStateChanged;
    public event Action<float> onDelayTimeChanged;
    public event Action<float> onReloadTimeChanged;
    public event Action<float> onDamageChanged;

    public Defines.AbilityType abilityType { get { return _abilityType; } }
    public bool isActivated { get; private set; }
    public bool isLoaded { get; private set; }
    public float radius { get; private set; }
    public float delayTime { get; private set; }
    public float reloadTime { get; private set; }
    public float damage { get; private set; }

    private Dictionary<TechTreeNodeId, Action> _upgrades = new Dictionary<TechTreeNodeId, Action>();
    private Process _reloadingProcess;

    public void SetAffectedAreaMarkerPosition(Vector3 position)
    {
        position.y = _affectedAreaMarker.transform.position.y;
        _affectedAreaMarker.transform.position = position;
    }

    public void CallAbility(Vector3 position)
    {
        Game.instance.SetDefaultGameMode();
        StartDeploymentProcess(position);
        isLoaded = false;
        if (onLoadStateChanged != null)
            onLoadStateChanged(isLoaded);
    }

    private void Start()
    {
        radius = _initialRadius;
        delayTime = _initialDelayTime;
        reloadTime = _initialReloadTime;
        damage = _initialDamage;

        _upgrades.Add(_activationTechTreeNode, Activate);
        _upgrades.Add(_delayTimeTechTreeNode, UpgradeDelayTime);
        _upgrades.Add(_reloadTimeTechTreeNode, UpgradeReloadTime);
        _upgrades.Add(_damageTechTreeNode, UpgradeDamage);

        Game.techTreeManager.onResearchFinished += OnResearchFinished;
        Game.instance.onGameModeChanged += OnGameModeChanged;

        onLoadStateChanged += state => TryStartReloadingProcess();

        _affectedAreaMarker.transform.localScale = new Vector3(radius * 2f, _affectedAreaMarker.transform.localScale.y, radius * 2f);
    }

    private void Activate()
    {
        isActivated = true;
        if (onActivated != null)
            onActivated();
        isLoaded = _initiallyLoaded;
        if (onLoadStateChanged != null)
            onLoadStateChanged(isLoaded);
        TryStartReloadingProcess();
    }

    private void UpgradeDelayTime()
    {
        delayTime -= _delayTimeUpgradeValue;
        if (onDelayTimeChanged != null)
            onDelayTimeChanged(delayTime);
    }

    private void UpgradeReloadTime()
    {
        reloadTime -= _reloadTimeUpgradeValue;
        if (onReloadTimeChanged != null)
            onReloadTimeChanged(reloadTime);
    }

    private void UpgradeDamage()
    {
        damage += _damageUpgradeValue;
        if (onDamageChanged != null)
            onDamageChanged(damage);
    }

    private void OnResearchFinished(TechTreeNodeId nodeId)
    {
        Action upgradeAction = null;
        if (_upgrades.TryGetValue(nodeId, out upgradeAction))
            upgradeAction();
    }

    private void OnGameModeChanged(Game.Mode mode, object context)
    {
        SetAffectedAreaMarkerVisible(mode == Game.Mode.CallAbility && (Defines.AbilityType)context == _abilityType);
    }

    private void SetAffectedAreaMarkerVisible(bool visible)
    {
        _affectedAreaMarker.gameObject.SetActive(visible);
    }

    private void TryStartReloadingProcess()
    {
        if (isLoaded || _reloadingProcess != null)
            return;

        _reloadingProcess = new TimeProcess("Reloading: " + _abilityType, reloadTime);
        _reloadingProcess.onFinished += OnReloadingProcessFinished;
        Game.processesManager.StartProcess(_reloadingProcess);
    }

    private void StartDeploymentProcess(Vector3 position)
    {
        Process p = new TimeProcess("Calling: " + _abilityType, delayTime);
        p.onFinished += () => OnDeploymentProcessFinished(position);
        Game.processesManager.StartProcess(p);
    }

    private void OnReloadingProcessFinished()
    {
        _reloadingProcess = null;
        isLoaded = true;
        if (onLoadStateChanged != null)
            onLoadStateChanged(isLoaded);
    }

    private void OnDeploymentProcessFinished(Vector3 position)
    {
        _abilityLogic.Deploy(position, radius, damage);
    }
}
