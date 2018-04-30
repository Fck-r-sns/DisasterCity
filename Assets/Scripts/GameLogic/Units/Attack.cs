using UnityEngine;

public class Attack : UnitComponent
{
    [SerializeField]
    float _shootingPeriod = 3f;
    [SerializeField]
    float _projectileSpeed = 20f;
    [SerializeField]
    float _projectileDamage = 10f;
    [SerializeField]
    Transform _towerPivot;
    [SerializeField]
    Transform _gunPivot;
    [SerializeField]
    Transform _shootingPoint;
    [SerializeField]
    Projectile _projectilePrefab;

    Transform _target;
    float _lastShotTime;

    void Start()
    {
        _target = Game.enemiesManager.GetMonster().GetChest();
    }

    void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.LookRotation(_shootingPoint.forward));
        projectile.transform.Rotate(90f, 0f, 0f, Space.Self);
        projectile.SetUp(_projectileSpeed, _projectileDamage);
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != null)
        {
            Vector3 towerDirection = _target.position - transform.position;
            towerDirection.y = 0f;
            _towerPivot.rotation = Quaternion.LookRotation(towerDirection);

            Vector3 gunDirection = transform.forward;
            float angle = -Mathf.Rad2Deg * Mathf.Atan((_target.position.y - transform.position.y) / towerDirection.magnitude);
            _gunPivot.localEulerAngles = new Vector3(angle, 0f, 0f);

            if (Time.time - _lastShotTime > _shootingPeriod)
            {
                RaycastHit hit;
                Ray ray = new Ray(_shootingPoint.position, _shootingPoint.forward);
                if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("ProjectileCollider", "Buildings", "Units")))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ProjectileCollider"))
                    {
                        _lastShotTime = Time.time;
                        Shoot();
                    }
                }
            }
        }
    }
}
