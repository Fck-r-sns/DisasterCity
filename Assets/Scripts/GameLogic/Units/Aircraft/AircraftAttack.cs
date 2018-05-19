using UnityEngine;

public class AircraftAttack : Attack
{
    [SerializeField]
    private Transform _gunPivot;
    [SerializeField]
    private float _maxGunLatitude;
    [SerializeField]
    private float _maxGunLongitude;

    void Update()
    {
        if (_targetPoint != null)
        {
            Vector3 directionToTarget = _targetPoint.transform.position - transform.position;
            _gunPivot.rotation = Quaternion.LookRotation(directionToTarget);
            _gunPivot.localEulerAngles = new Vector3(
                Mathf.Clamp(_gunPivot.localEulerAngles.x, 0f, _maxGunLatitude),
                Mathf.Clamp(_gunPivot.localEulerAngles.y, -_maxGunLongitude, _maxGunLongitude),
                0f);

            if (IsLoaded() && IsTargetInLineOfSight())
                Shoot();
        }
    }
}
