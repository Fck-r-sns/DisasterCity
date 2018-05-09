using UnityEngine;
using UnityEngine.AI;

public class TankMovement : Movement
{
    [SerializeField]
    private float _speed = 10f;
    [SerializeField]
    private float _angularSpeed = 120f;

    private Transform _targetPointer;
    private Vector3? _targetPosition;
    private float _yOffset;
    private float _movementPointerYOffset;
    private NavMeshAgent _navMeshAgent;

    void Start()
    {
        _yOffset = transform.position.y;
        _targetPointer = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
        _targetPointer.localScale = new Vector3(0.3f, 1f, 0.3f);
        _targetPointer.position = new Vector3(0f, 0.5f, 0f);
        _movementPointerYOffset = _targetPointer.position.y;
        _targetPointer.gameObject.SetActive(false);
        Destroy(_targetPointer.GetComponent<Collider>());

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = _speed;
        _navMeshAgent.angularSpeed = _angularSpeed;

        unit.defence.onDestroyed += OnDestroyed;
    }

    void OnDestroyed()
    {
        Destroy(_targetPointer.gameObject);
    }

    public override void SetTargetPosition(Vector3 position)
    {
        position.y = _yOffset;
        _targetPosition = position;
        position.y = _movementPointerYOffset;
        _targetPointer.position = position;
        _targetPointer.gameObject.SetActive(true);

        _navMeshAgent.destination = _targetPosition.Value;
    }

    void Update()
    {
        if (_targetPosition.HasValue)
        {
            if (Mathf.Approximately((transform.position - _targetPosition.Value).sqrMagnitude, 0f))
            {
                _targetPosition = null;
                _targetPointer.gameObject.SetActive(false);
            }
        }
    }
}
