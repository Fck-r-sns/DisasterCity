using UnityEngine;

public class AircraftMovement : Movement
{
    private float AttackDistance = 150f;
    private float AttackTurnStopSqrRadius = 100f;

    [SerializeField]
    private float _height;
    [SerializeField]
    private float _minSpeed;
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _accelerationSpeed;
    [SerializeField]
    private float _breakingSpeed;
    [SerializeField]
    private float _maxTurnAngle;
    [SerializeField]
    private float _maxBodyTurn;
    [SerializeField]
    private float _bodyTurnSpeed;
    [SerializeField]
    private Transform _body;

    private Transform _movementPointer;
    private Vector3 _movementTarget;
    private AttackableTarget _attackTarget;
    private Vector3 _curDirection;
    private float _curSpeed;
    private float _curBodyTurn;

    private void Start()
    {
        _movementPointer = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
        _movementPointer.localScale = new Vector3(0.3f, _height, 0.3f);
        _movementPointer.position = new Vector3(0f, _height / 2f, 0f);
        _movementPointer.gameObject.SetActive(false);
        Destroy(_movementPointer.GetComponent<Collider>());

        unit.defence.onDestroyed += OnDestroyed;
        unit.attack.onTargetChanged += OnAttackTargetChanged;

        transform.position = new Vector3(transform.position.x, _height, transform.position.z);
        _curDirection = transform.forward;
        _curSpeed = _minSpeed;
        SetMovementTarget(transform.position);
    }

    private void OnDestroyed()
    {
        Destroy(_movementPointer.gameObject);
    }

    private void OnAttackTargetChanged(AttackableTarget target)
    {
        _attackTarget = target;
        if (_attackTarget != null)
            SetMovementTargetInternal(GetMovementPositionForAttack());
    }

    public override void SetMovementTarget(Vector3 position)
    {
        SetMovementTargetInternal(position);
        _attackTarget = null; // Disable target following after manual position set
    }

    private void SetMovementTargetInternal(Vector3 position)
    {
        position.y = _height;
        _movementTarget = position;
        _movementPointer.position = position;
        _movementPointer.gameObject.SetActive(true);
    }

    private Vector3 GetMovementPositionForAttack()
    {
        Vector3 targetPosition = _attackTarget.transform.position;
        targetPosition.y = transform.position.y;
        Vector3 direction = (targetPosition - transform.position).normalized;
        return _attackTarget.transform.position + direction * AttackDistance;
    }

    private void Update()
    {
        Vector3 toMovementTarget = _movementTarget - transform.position;
        if (_attackTarget != null)
        {
            Vector3 toAttackTarget = _attackTarget.transform.position - transform.position;
            if (Vector3.SqrMagnitude(toMovementTarget) <= AttackTurnStopSqrRadius || Vector3.Dot(transform.forward, toAttackTarget) > 0f)
                SetMovementTargetInternal(GetMovementPositionForAttack());
        }

        float maxTurnAngle = _maxTurnAngle * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(_curDirection, toMovementTarget, maxTurnAngle * Mathf.Deg2Rad, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        float targetSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, 1f - Mathf.Clamp01(Vector3.Angle(newDirection, toMovementTarget) / 180f));
        float acceleration = targetSpeed >= _curSpeed ? _accelerationSpeed : _breakingSpeed;
        _curSpeed = Mathf.MoveTowards(_curSpeed, targetSpeed, acceleration * Time.deltaTime);
        transform.position += newDirection * _curSpeed * Time.deltaTime;

        float normalizedTurnAngle = Vector3.SignedAngle(newDirection, _curDirection, Vector3.up) / maxTurnAngle;
        float normalizedSpeed = (_curSpeed - _minSpeed) / (_maxSpeed - _minSpeed);
        float targetBodyTurn = normalizedTurnAngle * Mathf.Lerp(0.1f, 1f, normalizedSpeed) * _maxBodyTurn;
        _curBodyTurn = Mathf.MoveTowards(_curBodyTurn, targetBodyTurn, _bodyTurnSpeed * Time.deltaTime);
        _body.transform.localEulerAngles = new Vector3(0f, 0f, _curBodyTurn);

        _curDirection = newDirection;
    }
}
