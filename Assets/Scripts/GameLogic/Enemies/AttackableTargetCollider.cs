using UnityEngine;

public class AttackableTargetCollider : MonoBehaviour
{
    [SerializeField]
    private AttackableTarget _target;

    public AttackableTarget target { get { return _target; } }
}
