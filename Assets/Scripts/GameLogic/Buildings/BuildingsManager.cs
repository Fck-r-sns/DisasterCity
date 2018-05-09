using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{
    public event Action<int> onBuildingsLostCountChanged;

    Dictionary<int, BuildingController> _buildings = new Dictionary<int, BuildingController>();
    int _buildingsLostCount;

    public void RegisterBuilding(BuildingController building)
    {
        _buildings.Add(building.id, building);
    }

    public void UnregisterBuilding(BuildingController building)
    {
        _buildings.Remove(building.id);
        _buildingsLostCount++;
        if (onBuildingsLostCountChanged != null)
            onBuildingsLostCountChanged(_buildingsLostCount);
    }

    public Dictionary<int, BuildingController> GetBuildings()
    {
        return _buildings;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.E))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, float.MaxValue, Defines.Layers.buildingsMask))
                {
                    BuildingDamageReceiver building = hit.collider.GetComponent<BuildingDamageReceiver>();
                    if (building != null)
                        building.ReceiveAttack();
                }
            }
        }
    }
}
