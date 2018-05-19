using UnityEngine;

public abstract class Attack : UnitComponent
{
    const float SphereCastProjectileRadiusCoef = 1.1f;

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

    protected AttackableTargetPoint _targetPoint;
    private float _lastShotTime;

    public void SetTarget(AttackableTargetPoint targetPoint)
    {
        _targetPoint = targetPoint;
    }

    protected bool IsLoaded()
    {
        return Time.time - _lastShotTime >= _reloadTime;
    }

    protected bool IsTargetInLineOfSight()
    {
        if (_targetPoint == null)
            return false;

        Ray ray = new Ray(_shootingPoint.position, _shootingPoint.forward);
        int layerMask = Defines.Layers.Combine(Defines.Layers.projectileColliderMask, Defines.Layers.buildingsMask, Defines.Layers.unitsMask);
        float projectileRadius = Mathf.Max(_projectilePrefab.transform.localScale.x, _projectilePrefab.transform.localScale.z) / 2f;
        float sphereCastRadius = projectileRadius * SphereCastProjectileRadiusCoef;
        RaycastHit[] hits = Physics.SphereCastAll(ray, sphereCastRadius, float.MaxValue, layerMask);
        float minDistance = float.MaxValue;
        int nearestHitLayer = 0;
        bool targetInLineOfSight = false;
        foreach (var hit in hits)
        {
            if (hit.distance < minDistance)
            {
                nearestHitLayer = hit.collider.gameObject.layer;
                minDistance = hit.distance;
            }
            var targetCollider = hit.collider.GetComponent<AttackableTargetCollider>();
            if (targetCollider != null && targetCollider.attackPoint == _targetPoint)
                targetInLineOfSight = true;
        }
        return targetInLineOfSight && nearestHitLayer == Defines.Layers.projectileColliderLayer;
    }

    protected void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.LookRotation(_shootingPoint.forward));
        projectile.transform.Rotate(90f, 0f, 0f, Space.Self);
        projectile.SetUp(_projectileSpeed, _projectileDamage);
        _lastShotTime = Time.time;
    }
}
