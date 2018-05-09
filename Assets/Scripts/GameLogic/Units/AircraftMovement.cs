using UnityEngine;

public class AircraftMovement : Movement
{
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

    private Transform _targetPointer;
    private Vector3 _targetPosition;
    private Vector3 _curDirection;
    private float _curSpeed;

    void Start()
    {
        _targetPointer = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
        _targetPointer.localScale = new Vector3(0.3f, _height, 0.3f);
        _targetPointer.position = new Vector3(0f, _height / 2f, 0f);
        _targetPointer.gameObject.SetActive(false);
        Destroy(_targetPointer.GetComponent<Collider>());

        unit.defence.onDestroyed += OnDestroyed;

        transform.position = new Vector3(transform.position.x, _height, transform.position.z);
        _curDirection = transform.forward;
        _curSpeed = _minSpeed;
        SetTargetPosition(transform.position);
    }

    void OnDestroyed()
    {
        Destroy(_targetPointer.gameObject);
    }

    public override void SetTargetPosition(Vector3 position)
    {
        position.y = _height;
        _targetPosition = position;
        _targetPointer.position = position;
        _targetPointer.gameObject.SetActive(true);
    }

    void Update()
    {
        Vector3 toTarget = _targetPosition - transform.position;
        _curDirection = Vector3.RotateTowards(_curDirection, toTarget, _maxTurnAngle * Time.deltaTime * Mathf.Deg2Rad, 0f);
        float targetSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, 1f - Mathf.Clamp01(Vector3.Angle(_curDirection, toTarget) / 180f));
        float acceleration = targetSpeed >= _curSpeed ? _accelerationSpeed : _breakingSpeed;
        _curSpeed = Mathf.MoveTowards(_curSpeed, targetSpeed, acceleration * Time.deltaTime);
        transform.position += _curDirection * _curSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(_curDirection);
    }
}
