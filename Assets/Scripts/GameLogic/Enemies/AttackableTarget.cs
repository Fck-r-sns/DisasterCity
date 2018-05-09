using UnityEngine;

public class AttackableTarget : MonoBehaviour
{
    [SerializeField]
    private Transform _attackPoint;

    public Transform attackPoint { get { return _attackPoint; } }
}
