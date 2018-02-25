using UnityEngine;

public class BuildingDamageReceiver : MonoBehaviour
{
    BuildingController _buildingController;

    public void Start()
    {
        Transform t = transform;
        while (_buildingController == null && t != null)
        {
            _buildingController = t.GetComponent<BuildingController>();
            t = t.parent;
        }
    }

    public void ReceiveAttack()
    {
        _buildingController.ReceiveAttack();
    }
}
