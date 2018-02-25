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

    const float WalkingSpeed = 15f;
    const float RunningSpeed = 100f;
    const float AngularSpeed = 120f;
    const float AttackDistance = 20f;
    const float TargetUpdatePeriod = 1f;
    const float AttackPeriod = 1f;
    const float RageDuration = 10f;

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

    Dictionary<int, Tank> _units = new Dictionary<int, Tank>();
    Tank _target;
    float _lastTargetUpdateTime;
    float _lastAttackTime;
    float _lastDefendTime;
    float _timeToNextDefend;
    float _rageStopTime;
    NavMeshAgent _navMeshAgent;

    float _generalHealth;
    Dictionary<BodyPart, float> _bodyPartsHealth;
    bool _isDead;
    bool _isScreaming;
    bool _isDefending;
    bool _isRage;

    void Start()
    {
        _units = UnitsControl.instance.GetUnits();

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
    }

    void Update()
    {
        if (_isDead || _isScreaming || _isDefending)
            return;

        if (_isRage && Time.time >= _rageStopTime)
            _isRage = false;

        if (Time.time - _lastTargetUpdateTime > TargetUpdatePeriod)
        {
            _lastTargetUpdateTime = Time.time;
            Tank nearestUnit = null;
            float minDistance = float.MaxValue;
            foreach (var kv in _units)
            {
                float dst = Vector3.Distance(transform.position, kv.Value.transform.position);
                if (nearestUnit == null || dst < minDistance)
                {
                    nearestUnit = kv.Value;
                    minDistance = dst;
                }
            }

            if (_target == null || minDistance < Vector3.Distance(transform.position, _target.transform.position))
                _target = nearestUnit;
        }

        if (_target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
            if (distanceToTarget > AttackDistance)
            {
                if ((_navMeshAgent.destination - _target.transform.position).sqrMagnitude > 0.01f)
                {
                    if (_isRage)
                        RunTo(_target.transform.position);
                    else
                        WalkTo(_target.transform.position);
                }
            }
            else
            {
                Stop();
                Vector3 rotationDir = Vector3.RotateTowards(transform.forward, _target.transform.position - transform.position, Time.deltaTime * AngularSpeed, 0f);
                transform.rotation = Quaternion.LookRotation(rotationDir);
                if (Time.time - _lastAttackTime > AttackPeriod && Vector3.Distance(transform.position, _target.transform.position) <= AttackDistance)
                {
                    _lastAttackTime = Time.time;
                    Attack();
                }
            }
        }
        else
        {
            _animator.SetBool("IsWalking", false);
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
        if (_generalHealth <= 0f)
            Die();
        else
            OnTakeDamage(bodyPartDestroyed);
    }

    void WalkTo(Vector3 position)
    {
        _navMeshAgent.speed = WalkingSpeed;
        _navMeshAgent.destination = position;
        _animator.SetBool("IsRunning", false);
        _animator.SetBool("IsWalking", true);
    }

    void RunTo(Vector3 position)
    {
        _navMeshAgent.speed = RunningSpeed;
        _navMeshAgent.destination = position;
        _animator.SetBool("IsWalking", false);
        _animator.SetBool("IsRunning", true);
    }

    void Stop()
    {
        _navMeshAgent.destination = transform.position;
        _animator.SetBool("IsWalking", false);
        _animator.SetBool("IsRunning", false);
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
            _timeToNextDefend = Random.Range(5f, 15f);
            if (!_isRage && !_isScreaming)
            {
                Stop();
                StartCoroutine(Defend());
            }
        }
    }
}
