using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    [SerializeField]
    private Transform _providers;
    [SerializeField]
    private KeyCode _tankKeyCode;
    [SerializeField]
    private GameObject _tankPrefab;
    [SerializeField]
    private KeyCode _aircraftKeyCode;
    [SerializeField]
    private GameObject _aircraftPrefab;

    public event Action<int> onUnitsCreatedCountChanged;
    public event Action<int> onUnitsLostCountChanged;
    public event Action<int> onUnitsSelectionChanged;
    public event Action<AttackableTargetPoint> onCurAttackableTargetPointChanged;
    public event Action<AttackableTargetPoint> onAttackableTargetPointSelected;

    Dictionary<Defines.UnitType, UnitsProvider> _unitsProviders = new Dictionary<Defines.UnitType, UnitsProvider>();
    Dictionary<Defines.AbilityType, AbilityProvider> _abilitiesProviders = new Dictionary<Defines.AbilityType, AbilityProvider>();
    Dictionary<int, Unit> _totalUnits = new Dictionary<int, Unit>();
    Dictionary<int, Unit> _selectedUnits = new Dictionary<int, Unit>();
    Dictionary<int, Unit> _frameSelectionBuffer = new Dictionary<int, Unit>();
    int _unitsCreatedCount;
    int _unitsLostCount;
    UnitsProvider _curUnitsProvider;
    AbilityProvider _curAbilityProvider;
    AttackableTargetPoint _curAttackableTargetPoint;

    private void Awake()
    {
        foreach (UnitsProvider unitsProvider in _providers.GetComponentsInChildren<UnitsProvider>())
            _unitsProviders.Add(unitsProvider.unitType, unitsProvider);
        foreach (AbilityProvider abilityProvider in _providers.GetComponentsInChildren<AbilityProvider>())
            _abilitiesProviders.Add(abilityProvider.abilityType, abilityProvider);
    }

    private void Start()
    {
        Game.instance.onGameModeChanged += OnGameModeChanged;
    }

    public void RegisterUnit(Unit unit)
    {
        _totalUnits.Add(unit.id, unit);
        _unitsCreatedCount++;
        if (onUnitsCreatedCountChanged != null)
            onUnitsCreatedCountChanged(_unitsCreatedCount);
    }

    public void UnregisterUnit(Unit unit)
    {
        RemoveUnitFromSelection(unit);
        _totalUnits.Remove(unit.id);
        _frameSelectionBuffer.Remove(unit.id);
        _unitsLostCount++;
        if (onUnitsLostCountChanged != null)
            onUnitsLostCountChanged(_unitsLostCount);
    }

    public Dictionary<int, Unit> GetUnits()
    {
        return _totalUnits;
    }

    public UnitsProvider GetUnitsProvider(Defines.UnitType unitType)
    {
        return _unitsProviders[unitType];
    }

    public AbilityProvider GetAbilityProvider(Defines.AbilityType abilityType)
    {
        return _abilitiesProviders[abilityType];
    }

    public void BeginFrameSelection()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            ResetSelection();
    }

    public void UpdateFrameSelection(Rect frameRect)
    {
        foreach (var kv in _frameSelectionBuffer)
            kv.Value.SetSelected(false);
        _frameSelectionBuffer.Clear();
        Camera camera = Camera.main;
        foreach (var kv in _totalUnits)
        {
            if (frameRect.Contains(camera.WorldToScreenPoint(kv.Value.transform.position)))
            {
                kv.Value.SetSelected(true);
                _frameSelectionBuffer[kv.Key] = kv.Value;
            }
        }
    }

    public void EndFrameSelection()
    {
        foreach (var kv in _frameSelectionBuffer)
            AddUnitToSelection(kv.Value);
        _frameSelectionBuffer.Clear();
    }

    void AddUnitToSelection(Unit unit)
    {
        _selectedUnits[unit.id] = unit;
        if (onUnitsSelectionChanged != null)
            onUnitsSelectionChanged(_selectedUnits.Count);
    }

    void RemoveUnitFromSelection(Unit unit)
    {
        _selectedUnits.Remove(unit.id);
        if (onUnitsSelectionChanged != null)
            onUnitsSelectionChanged(_selectedUnits.Count);
    }

    void ResetSelection()
    {
        foreach (var kv in _selectedUnits)
            kv.Value.SetSelected(false);
        _selectedUnits.Clear();
        if (onUnitsSelectionChanged != null)
            onUnitsSelectionChanged(_selectedUnits.Count);
    }

    void Update()
    {
        bool leftMouseButtonDown = Input.GetMouseButtonDown(0);
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);
        bool middleMouseButtonDown = Input.GetMouseButtonDown(2);
        bool noMouseButtonsDown = !leftMouseButtonDown && !rightMouseButtonDown && !middleMouseButtonDown;

        RaycastHit hit;
        if (noMouseButtonsDown)
        {
            if (_curAbilityProvider != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, float.MaxValue, Defines.Layers.groundMask))
                    _curAbilityProvider.SetAffectedAreaMarkerPosition(hit.point);
            }

            if (_selectedUnits.Count > 0)
            {
                AttackableTargetPoint newAttackableTargetPoint = null;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, float.MaxValue, Defines.Layers.projectileColliderMask))
                {
                    var collider = hit.collider.GetComponent<AttackableTargetCollider>();
                    if (collider != null)
                        newAttackableTargetPoint = collider.attackPoint;
                }

                if (newAttackableTargetPoint != _curAttackableTargetPoint)
                {
                    _curAttackableTargetPoint = newAttackableTargetPoint;
                    if (onCurAttackableTargetPointChanged != null)
                        onCurAttackableTargetPointChanged(_curAttackableTargetPoint);
                }
            }
        }
        else if (leftMouseButtonDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (_curUnitsProvider != null)
            {
                if (Physics.Raycast(ray, out hit, float.MaxValue, Defines.Layers.deploymentZoneMask))
                {
                    DeploymentZone zone = hit.collider.GetComponent<DeploymentZone>();
                    _curUnitsProvider.StartDeployment(hit.point, zone.direction);
                }
            }
            else if (_curAbilityProvider != null)
            {
                if (Physics.Raycast(ray, out hit, float.MaxValue, Defines.Layers.groundMask))
                    _curAbilityProvider.CallAbility(hit.point);
            }
            else if (Input.GetKey(_tankKeyCode))
            {
                if (Physics.Raycast(ray, out hit, float.MaxValue, Defines.Layers.groundMask))
                    Instantiate(_tankPrefab, hit.point, Quaternion.identity);
            }
            else if (Input.GetKey(_aircraftKeyCode))
            {
                if (Physics.Raycast(ray, out hit, float.MaxValue, Defines.Layers.groundMask))
                    Instantiate(_aircraftPrefab, hit.point, Quaternion.identity);
            }
            else
            {
                if (Physics.Raycast(ray, out hit, float.MaxValue))
                {
                    Unit unit = hit.collider.GetComponent<Unit>();
                    if (unit != null)
                    {
                        bool shiftModifier = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                        if (shiftModifier)
                        {
                            unit.ToggleSelection();
                        }
                        else
                        {
                            ResetSelection();
                            unit.SetSelected(true);
                        }

                        if (unit.isSelected)
                            AddUnitToSelection(unit);
                        else
                            RemoveUnitFromSelection(unit);
                    }
                }
            }
        }
        else if (rightMouseButtonDown)
        {
            if (_selectedUnits.Count > 0)
            {
                if (_curAttackableTargetPoint != null)
                {
                    foreach (var kv in _selectedUnits)
                        if (kv.Value.attack != null)
                            kv.Value.attack.SetTarget(_curAttackableTargetPoint.transform);
                    if (onAttackableTargetPointSelected != null)
                        onAttackableTargetPointSelected(_curAttackableTargetPoint);
                }
                else
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, float.MaxValue, Defines.Layers.groundMask))
                    {
                        foreach (var kv in _selectedUnits)
                            if (kv.Value.movement != null)
                                kv.Value.movement.SetTargetPosition(hit.point);
                    }
                }
            }
        }
    }

    private void OnGameModeChanged(Game.Mode mode, object context)
    {
        _curUnitsProvider = (mode == Game.Mode.CallUnits) ? GetUnitsProvider((Defines.UnitType)context) : null;
        _curAbilityProvider = (mode == Game.Mode.CallAbility) ? GetAbilityProvider((Defines.AbilityType)context) : null;
    }
}
