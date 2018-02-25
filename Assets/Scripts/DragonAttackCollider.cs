using UnityEngine;

public class DragonAttackCollider : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            var tank = contact.otherCollider.GetComponent<Tank>();
            if (tank != null)
                tank.ReceiveAttack();

            var building = contact.otherCollider.GetComponent<BuildingDamageReceiver>();
            if (building != null)
                building.ReceiveAttack();
        }
    }
}
