﻿using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    static GameControl _instance;

    public static GameControl instance { get { return _instance; } }

    [SerializeField]
    GameObject _tankPrefab;
    [SerializeField]
    DragonController _monster;

    Dictionary<int, Tank> _totalUnits = new Dictionary<int, Tank>();
    Dictionary<int, Tank> _selectedUnits = new Dictionary<int, Tank>();
    Dictionary<int, Tank> _frameSelectionBuffer = new Dictionary<int, Tank>();
    Dictionary<int, BuildingController> _buildings = new Dictionary<int, BuildingController>();

    void Awake()
    {
        _instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetKey(KeyCode.Q))
            {
                if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Ground")))
                    Instantiate(_tankPrefab, hit.point, Quaternion.identity);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Buildings")))
                {
                    BuildingDamageReceiver building = hit.collider.GetComponent<BuildingDamageReceiver>();
                    if (building != null)
                        building.ReceiveAttack();
                }
            }
            else
            {
                if (Physics.Raycast(ray, out hit, float.MaxValue))
                {
                    Tank unit = hit.collider.GetComponent<Tank>();
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
                            _selectedUnits[unit.id] = unit;
                        else
                            _selectedUnits.Remove(unit.id);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Ground")))
            {
                foreach (var kv in _selectedUnits)
                    kv.Value.GoTo(hit.point);
            }
        }
    }

    public void RegisterUnit(Tank unit)
    {
        _totalUnits.Add(unit.id, unit);
    }

    public void UnregisterUnit(Tank unit)
    {
        _totalUnits.Remove(unit.id);
        _selectedUnits.Remove(unit.id);
        _frameSelectionBuffer.Remove(unit.id);
    }

    public void RegisterBuilding(BuildingController building)
    {
        _buildings.Add(building.id, building);
    }

    public void UnregisterBuilding(BuildingController building)
    {
        _buildings.Remove(building.id);
    }

    public Dictionary<int, Tank> GetUnits()
    {
        return _totalUnits;
    }

    public Dictionary<int, BuildingController> GetBuildings()
    {
        return _buildings;
    }

    public DragonController GetMonster()
    {
        return _monster;
    }

    void ResetSelection()
    {
        foreach (var kv in _selectedUnits)
            kv.Value.SetSelected(false);
        _selectedUnits.Clear();
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
            _selectedUnits[kv.Key] = kv.Value;
        _frameSelectionBuffer.Clear();
    }
}
