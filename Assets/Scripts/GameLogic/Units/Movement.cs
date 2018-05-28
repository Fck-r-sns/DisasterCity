using UnityEngine;

public abstract class Movement : UnitComponent
{
    [SerializeField]
    private bool _isFlying;

    public bool isFlying { get { return _isFlying; } }

    public abstract void SetMovementTarget(Vector3 position);
}
