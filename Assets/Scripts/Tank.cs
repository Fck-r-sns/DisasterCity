using UnityEngine;
using UnityEngine.AI;

public class Tank : MonoBehaviour
{
    static int IdGenerator;

    public int id { get; private set; }
    public bool isSelected { get { return _isSelected; } }
    public bool isDestroyed { get; private set; }

    [SerializeField]
    float _speed = 10f;
    [SerializeField]
    float _angularSpeed = 120f;
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
    Transform _selectionFrame;
    [SerializeField]
    Transform _shootingPoint;
    [SerializeField]
    GameObject _projectilePrefab;
    [SerializeField]
    GameObject[] _explosionsPrefabs;

    bool _isSelected;
    Transform _movementPointer;
    Vector3? _movementPosition;
    DragonController _monster;
    Transform _shootingTarget;
    float _yOffset;
    float _movementPointerYOffset;
    float _lastShotTime;
    NavMeshAgent _navMeshAgent;

    void Start()
    {
        id = ++IdGenerator;
        _yOffset = transform.position.y;
        _movementPointer = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
        _movementPointer.localScale = new Vector3(0.3f, 1f, 0.3f);
        _movementPointer.position = new Vector3(0f, 0.5f, 0f);
        _movementPointerYOffset = _movementPointer.position.y;
        _movementPointer.gameObject.SetActive(false);
        Destroy(_movementPointer.GetComponent<Collider>());

        GameControl.instance.RegisterUnit(this);
        _monster = GameControl.instance.GetMonster();
        _shootingTarget = _monster.GetChest();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = _speed;
        _navMeshAgent.angularSpeed = _angularSpeed;
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        _selectionFrame.gameObject.SetActive(_isSelected);
    }

    public void ToggleSelection()
    {
        _isSelected = !_isSelected;
        _selectionFrame.gameObject.SetActive(_isSelected);
    }

    public void GoTo(Vector3 position)
    {
        position.y = _yOffset;
        _movementPosition = position;
        position.y = _movementPointerYOffset;
        _movementPointer.position = position;
        _movementPointer.gameObject.SetActive(true);

        _navMeshAgent.destination = _movementPosition.Value;
    }

    void Update()
    {
        if (_movementPosition.HasValue)
        {
            if (Mathf.Approximately((transform.position - _movementPosition.Value).sqrMagnitude, 0f))
            {
                _movementPosition = null;
                _movementPointer.gameObject.SetActive(false);
            }
        }

        if (_shootingTarget != null)
        {
            Vector3 towerDirection = _shootingTarget.position - transform.position;
            towerDirection.y = 0f;
            _towerPivot.rotation = Quaternion.LookRotation(towerDirection);

            Vector3 gunDirection = transform.forward;
            float angle = -Mathf.Rad2Deg * Mathf.Atan((_shootingTarget.position.y - transform.position.y) / towerDirection.magnitude);
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

    void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.LookRotation(_shootingPoint.forward))
            .GetComponent<Projectile>();
        projectile.transform.Rotate(90f, 0f, 0f, Space.Self);
        projectile.SetUp(_projectileSpeed, _projectileDamage);
    }

    public void ReceiveAttack()
    {
        if (isDestroyed)
            return;

        isDestroyed = true;
        GameControl.instance.UnregisterUnit(this);
        GameObject explosion = Instantiate(_explosionsPrefabs[Random.Range(0, _explosionsPrefabs.Length)], transform.position, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * 10f;
        Destroy(gameObject);
        Destroy(_movementPointer.gameObject);
        Destroy(explosion, 1f);
    }
}
