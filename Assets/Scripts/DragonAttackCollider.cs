using UnityEngine;

public class DragonAttackCollider : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            var defence = contact.otherCollider.GetComponent<Defence>();
            if (defence != null)
                defence.ReceiveAttack();

            var building = contact.otherCollider.GetComponent<BuildingDamageReceiver>();
            if (building != null)
                building.ReceiveAttack();
        }
    }
}
