using UnityEngine;

public class AttackableTargetCollider : MonoBehaviour
{
    [SerializeField]
    private AttackableTargetPoint _attackPoint;

    public AttackableTargetPoint attackPoint { get { return _attackPoint; } }
}
