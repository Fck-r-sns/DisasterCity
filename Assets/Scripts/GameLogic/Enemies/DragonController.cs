using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonController : MonoBehaviour
{
    public enum BodyPart
    {
        Body,
        Neck,
        Head,
        FrontLeftLeg,
        FrontRightLeg,
        BackLeftLeg,
        BackRightLeg,
        Tail,
        LeftWing,
        RightWing,
    }

    enum MovementState
    {
        Stop,
        Walk,
        Run,
    }

    enum BehaviorState
    {
        DestroyCity,
        AttackEnemies,
    }

    const bool ShowMovementPointer = false;
    const float CityWidth = 320f;
    const float CityHeight = 320f;
    const float WalkingSpeed = 15f;
    const float RunningSpeed = 100f;
    const float AngularSpeed = 120f;
    const float AttackDistance = 30f;
    const float TargetUpdatePeriod = 1f;
    const float AttackPeriod = 3f;
    const float RageAttackPeriod = 1f;
    const float RageDuration = 10f;
    const float TimeBetweenBeingAttackedAndStartPatroling = 3f;
    const float MinAttackBuildingPeriod = 5f;
    const float MaxAttackBuildingPeriod = 10f;
    const float MinDefendPeriod = 5f;
    const float MaxDefendPeriod = 15f;

    [SerializeField]
    Animator _animator;
    [SerializeField]
    Transform _chest;
    [SerializeField]
    DragonAttackCollider _jawAttackCollider;
    [SerializeField]
    DragonAttackCollider _leftClawAttackCollider;
    [SerializeField]
    DragonAttackCollider _rightClawAttackCollider;

    Dictionary<int, Unit> _units;
    Dictionary<int, BuildingController> _buildings;
    Transform _movementPointer;
    Vector3? _patrolingPosition;
    Transform _target;
    float _nextBuildingAttackTime;
    float _lastReceivedDamageTime = -float.MaxValue;
    float _lastTargetUpdateTime;
    float _lastAttackTime;
    float _lastDefendTime;
    float _timeToNextDefend;
    float _rageStopTime;
    NavMeshAgent _navMeshAgent;
    MovementState _movementState;
    BehaviorState _behaviorState;

    float _generalHealth;
    Dictionary<BodyPart, float> _bodyPartsHealth;
    bool _isDead;
    bool _isScreaming;
    bool _isDefending;
    bool _isRage;

    void Start()
    {
        _units = Game.unitsManager.GetUnits();
        _buildings = Game.buildingsManager.GetBuildings();

        _generalHealth = 2500f;
        _bodyPartsHealth = new Dictionary<BodyPart, float>
        {
            { BodyPart.Body, 300f },
            { BodyPart.Neck, 200f },
            { BodyPart.Head, 200f },
            { BodyPart.Tail, 200f },
            { BodyPart.LeftWing, 250f },
            { BodyPart.RightWing, 250f },
            { BodyPart.FrontLeftLeg, 300f },
            { BodyPart.FrontRightLeg, 300f },
            { BodyPart.BackLeftLeg, 300f },
            { BodyPart.BackRightLeg, 300f },
        };

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = WalkingSpeed;
        _navMeshAgent.angularSpeed = AngularSpeed;

        if (ShowMovementPointer)
        {
            _movementPointer = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
            _movementPointer.localScale = new Vector3(0.3f, 150f, 0.3f);
            _movementPointer.position = new Vector3(0f, 75f, 0f);
            _movementPointer.gameObject.SetActive(false);
            Destroy(_movementPointer.GetComponent<Collider>());
        }
    }

    void Update()
    {
        if (_isDead || _isScreaming || _isDefending)
            return;

        if (_isRage && Time.time >= _rageStopTime)
            _isRage = false;

        BehaviorState newBehaviorState = Time.time - _lastReceivedDamageTime >= TimeBetweenBeingAttackedAndStartPatroling ?
            BehaviorState.DestroyCity : BehaviorState.AttackEnemies;
        if (_behaviorState != newBehaviorState)
        {
            _behaviorState = newBehaviorState;
            _patrolingPosition = null;
            _target = null;
        }

        if (_behaviorState == BehaviorState.DestroyCity)
            ProcessDestroyCityState();
        else if (_behaviorState == BehaviorState.AttackEnemies)
            ProcessAttackUnitsState();

        if (_target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, _target.position);
            if (distanceToTarget > AttackDistance)
            {
                if ((_navMeshAgent.destination - _target.position).sqrMagnitude > 0.01f)
                {
                    if (_isRage)
                        RunTo(_target.position);
                    else
                        WalkTo(_target.position);
                }
            }
            else
            {
                Stop();
                Vector3 rotationDir = Vector3.RotateTowards(transform.forward, _target.position - transform.position,
                    Time.deltaTime * AngularSpeed * Mathf.Deg2Rad, 0f);
                transform.rotation = Quaternion.LookRotation(rotationDir);
                float attackPeriod = _isRage ? RageAttackPeriod : AttackPeriod;
                if (Time.time - _lastAttackTime > attackPeriod && Vector3.Distance(transform.position, _target.position) <= AttackDistance)
                {
                    _lastAttackTime = Time.time;
                    Attack();
                }
            }
        }
        else if (_patrolingPosition.HasValue)
        {
            WalkTo(_patrolingPosition.Value);
        }
        else
        {
            Stop();
        }
    }

    public void ProcessDestroyCityState()
    {
        if (_target != null)
        {
            BuildingController building = _target.GetComponent<BuildingController>();
            if (building != null && building.isDestroyed)
                _target = null;
        }

        if (_patrolingPosition.HasValue && Vector3.Distance(transform.position, _patrolingPosition.Value) < 30f)
            _patrolingPosition = null;

        if (!_patrolingPosition.HasValue)
        {
            _patrolingPosition = new Vector3(
                Random.Range(-CityWidth / 2f, CityWidth / 2f),
                0f,
                Random.Range(-CityHeight / 2f, CityHeight / 2f));
        }

        if (Time.time >= _nextBuildingAttackTime)
        {
            _nextBuildingAttackTime = Time.time + Random.Range(MinAttackBuildingPeriod, MaxAttackBuildingPeriod);

            BuildingController nearestBuilding = null;
            float minDistance = float.MaxValue;
            foreach (var kv in _buildings)
            {
                float dst = Vector3.Distance(transform.position, kv.Value.transform.position);
                if (nearestBuilding == null || dst < minDistance)
                {
                    nearestBuilding = kv.Value;
                    minDistance = dst;
                }
            }

            if (nearestBuilding != null && (_target == null || minDistance < Vector3.Distance(transform.position, _target.position)))
                _target = nearestBuilding.transform;
        }
    }

    public void ProcessAttackUnitsState()
    {
        if (Time.time - _lastTargetUpdateTime > TargetUpdatePeriod)
        {
            _lastTargetUpdateTime = Time.time;
            Unit nearestUnit = null;
            float minDistance = float.MaxValue;
            foreach (var kv in _units)
            {
                // Todo: Make it possible to attack flying units
                if (kv.Value.movement.isFlying)
                    continue;

                float dst = Vector3.Distance(transform.position, kv.Value.transform.position);
                if (nearestUnit == null || dst < minDistance)
                {
                    nearestUnit = kv.Value;
                    minDistance = dst;
                }
            }

            if (nearestUnit != null && (_target == null || minDistance < Vector3.Distance(transform.position, _target.position)))
                _target = nearestUnit.transform;
        }
    }

    public Transform GetChest()
    {
        return _chest;
    }

    public void ReceiveDamage(BodyPart bodyPart, float rawDamage)
    {
        if (_isDead)
            return;

        float generalDamage = rawDamage;
        bool bodyPartDestroyed = false;
        if (_bodyPartsHealth.ContainsKey(bodyPart))
        {
            _bodyPartsHealth[bodyPart] -= rawDamage;
            if (_bodyPartsHealth[bodyPart] <= 0)
            {
                _bodyPartsHealth.Remove(bodyPart);
                bodyPartDestroyed = true;
            }
        }
        else
        {
            generalDamage /= 2f;
        }

        _generalHealth -= generalDamage;
        _lastReceivedDamageTime = Time.time;

        if (_generalHealth <= 0f)
            Die();
        else
            OnTakeDamage(bodyPartDestroyed);
    }

    void WalkTo(Vector3 position)
    {
        if (_movementState != MovementState.Walk)
        {
            _movementState = MovementState.Walk;
            _animator.SetBool("IsRunning", false);
            _animator.SetBool("IsWalking", true);
            _navMeshAgent.speed = WalkingSpeed;
        }

        if ((_navMeshAgent.destination - position).sqrMagnitude > 0.01f)
            _navMeshAgent.destination = position;

        if (ShowMovementPointer)
        {
            _movementPointer.position = position;
            _movementPointer.gameObject.SetActive(true);
        }
    }

    void RunTo(Vector3 position)
    {
        if (_movementState != MovementState.Run)
        {
            _movementState = MovementState.Run;
            _navMeshAgent.speed = RunningSpeed;
            _animator.SetBool("IsWalking", false);
            _animator.SetBool("IsRunning", true);
        }

        if ((_navMeshAgent.destination - position).sqrMagnitude > 0.01f)
            _navMeshAgent.destination = position;

        if (ShowMovementPointer)
        {
            _movementPointer.position = position;
            _movementPointer.gameObject.SetActive(true);
        }
    }

    void Stop()
    {
        if (_movementState != MovementState.Stop)
        {
            _movementState = MovementState.Stop;
            _animator.SetBool("IsWalking", false);
            _animator.SetBool("IsRunning", false);
        }

        _navMeshAgent.destination = transform.position;

        if (ShowMovementPointer)
            _movementPointer.gameObject.SetActive(false);
    }

    void Attack()
    {
        int attackIndex = Random.Range(0, 1000) % 2;
        _animator.SetTrigger("Attack" + attackIndex);
        StartCoroutine(Attack(attackIndex));
    }

    IEnumerator Attack(int index)
    {
        if (index == 0)
        {
            _jawAttackCollider.gameObject.SetActive(true);
        }
        else
        {
            _leftClawAttackCollider.gameObject.SetActive(true);
            _rightClawAttackCollider.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        if (index == 0)
        {
            _jawAttackCollider.gameObject.SetActive(false);
        }
        else
        {
            _leftClawAttackCollider.gameObject.SetActive(false);
            _rightClawAttackCollider.gameObject.SetActive(false);
        }
    }

    void Die()
    {
        Stop();
        _isDead = true;
        _animator.SetTrigger("Die");
    }

    void Rage()
    {
        _isRage = true;
        _rageStopTime = Time.time + RageDuration;
    }

    IEnumerator Scream()
    {
        _animator.SetTrigger("Scream");
        _isScreaming = true;
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Scream"))
            yield return new WaitWhile(() => !_animator.GetCurrentAnimatorStateInfo(0).IsName("Scream"));
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).IsName("Scream"));
        _isScreaming = false;
        Rage();
    }

    IEnumerator Defend()
    {
        _animator.SetTrigger("Defend");
        _isDefending = true;
        if (!_animator.GetCurrentAnimatorStateInfo(1).IsName("Defend"))
            yield return new WaitWhile(() => !_animator.GetCurrentAnimatorStateInfo(1).IsName("Defend"));
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(1).IsName("Defend"));
        _isDefending = false;
    }

    void OnTakeDamage(bool bodyPartDestroyed)
    {
        if (bodyPartDestroyed)
        {
            if (_isRage)
            {
                Rage();
            }
            else if (!_isScreaming)
            {
                Stop();
                StartCoroutine(Scream());
            }
        }
        else if (Time.time >= _lastDefendTime + _timeToNextDefend)
        {
            _lastDefendTime = Time.time;
            _timeToNextDefend = Random.Range(MinDefendPeriod, MaxDefendPeriod);
            if (!_isRage && !_isScreaming)
            {
                Stop();
                StartCoroutine(Defend());
            }
        }
    }
}
