using UnityEngine;

public class ArtilleryStrike : Ability
{
    [SerializeField]
    float _projectilesCount;
    [SerializeField]
    float _projectileSpawnHeightMin;
    [SerializeField]
    float _projectileSpawnHeightMax;
    [SerializeField]
    float _projectileSpeed;
    [SerializeField]
    Projectile _projectilePrefab;

    const int GaussianIterations = 4;

    public override void Deploy(Vector3 position, float radius, float damage)
    {
        float projectileDamage = damage / _projectilesCount;
        for (int i = 0; i < _projectilesCount; ++i)
        {
            Vector2 shift = Vector2.zero;
            for (int j = 0; j < GaussianIterations; ++j)
                shift += Random.insideUnitCircle * radius;
            shift /= GaussianIterations;

            float spawnHeight = Random.Range(_projectileSpawnHeightMin, _projectileSpawnHeightMax);
            Vector3 spawnPos = position + new Vector3(shift.x, spawnHeight, shift.y);

            Projectile projectile = Instantiate(_projectilePrefab, spawnPos, Quaternion.LookRotation(Vector3.down));
            projectile.transform.Rotate(90f, 0f, 0f, Space.Self);
            projectile.transform.localScale *= 3;
            projectile.SetUp(_projectileSpeed, projectileDamage);
            projectile.SetExplosionScale(10f);
        }
    }
}
