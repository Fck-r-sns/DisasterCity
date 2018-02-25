using UnityEngine;

public class DragonAttackCollider : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        var tank = collision.contacts[0].otherCollider.GetComponent<Tank>();
        if (tank != null)
            tank.ReceiveAttack();
    }
}
