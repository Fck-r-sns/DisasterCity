using UnityEngine;

public abstract class Attack : UnitComponent
{
    [SerializeField]
    private float _reloadTime = 3f;
    [SerializeField]
    private float _projectileSpeed = 20f;
    [SerializeField]
    private float _projectileDamage = 10f;
    [SerializeField]
    private Projectile _projectilePrefab;
    [SerializeField]
    protected Transform _shootingPoint;

    protected Transform _target;
    private float _lastShotTime;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    protected bool IsLoaded()
    {
        return Time.time - _lastShotTime >= _reloadTime;
    }

    protected bool IsTargetInLineOfSight()
    {
        RaycastHit hit;
        Ray ray = new Ray(_shootingPoint.position, _shootingPoint.forward);
        int layerMask = Defines.Layers.Combine(Defines.Layers.projectileColliderMask, Defines.Layers.buildingsMask, Defines.Layers.unitsMask);
        return Physics.Raycast(ray, out hit, float.MaxValue, layerMask) && hit.collider.gameObject.layer == Defines.Layers.projectileColliderLayer;
    }

    protected void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.LookRotation(_shootingPoint.forward));
        projectile.transform.Rotate(90f, 0f, 0f, Space.Self);
        projectile.SetUp(_projectileSpeed, _projectileDamage);
        _lastShotTime = Time.time;
    }
}
