using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    [SerializeField]
    GameObject _tankPrefab;

    Dictionary<int, Unit> _totalUnits = new Dictionary<int, Unit>();
    Dictionary<int, Unit> _selectedUnits = new Dictionary<int, Unit>();
    Dictionary<int, Unit> _frameSelectionBuffer = new Dictionary<int, Unit>();

    public void RegisterUnit(Unit unit)
    {
        _totalUnits.Add(unit.id, unit);
    }

    public void UnregisterUnit(Unit unit)
    {
        _totalUnits.Remove(unit.id);
        _selectedUnits.Remove(unit.id);
        _frameSelectionBuffer.Remove(unit.id);
    }

    public Dictionary<int, Unit> GetUnits()
    {
        return _totalUnits;
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

    void ResetSelection()
    {
        foreach (var kv in _selectedUnits)
            kv.Value.SetSelected(false);
        _selectedUnits.Clear();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Time.timeScale = Time.timeScale == 0f ? 1f : 0f;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetKey(KeyCode.Q))
            {
                if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Ground")))
                    Instantiate(_tankPrefab, hit.point, Quaternion.identity);
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
                    if (kv.Value.movement != null)
                        kv.Value.movement.GoTo(hit.point);
            }
        }
    }
}
